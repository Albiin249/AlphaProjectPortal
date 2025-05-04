using Business.Interfaces;
using Business.Models;
using Data.Entities;
using Data.Interfaces;
using Domain.Dtos;
using Domain.Extensions;
using Domain.Models;

namespace Business.Services;

public class ProjectService(IProjectRepository projectRepository, IStatusService statusService, IProjectMemberRepository projectMemberRepository) : IProjectService
{
    private readonly IProjectRepository _projectRepository = projectRepository;
    private readonly IStatusService _statusService = statusService;
    private readonly IProjectMemberRepository _projectMemberRepository = projectMemberRepository;

    //CREATE
    public async Task<ProjectResult> CreateProjectAsync(AddProjectFormData formData)
    {
        if (formData == null)
            return new ProjectResult { Succeeded = false, StatusCode = 400, Error = "Not all required fields are supplied." };
        var projectEntity = formData.MapTo<ProjectEntity>();
        var statusResult = await _statusService.GetStatusByIdAsync(1);
        var status = statusResult.Result;

        if (status == null)
            return new ProjectResult { Succeeded = false, StatusCode = 404, Error = "Status not found." };

        projectEntity.StatusId = status.Id;

        var result = await _projectRepository.AddAsync(projectEntity);
        if (!result.Succeeded)
            return new ProjectResult { Succeeded = false, StatusCode = result.StatusCode, Error = result.Error };


        if (formData?.MemberIds != null && formData.MemberIds.Any())
        {
            foreach (var memberId in formData.MemberIds)
            {
                if (string.IsNullOrEmpty(memberId))
                {
                    Console.WriteLine("MemberId is empty or null.");
                    continue; 
                }

                var cleanedMemberId = memberId.Trim('[', ']', '"');
                if (Guid.TryParse(cleanedMemberId, out Guid parsedMemberId))
                {
                    var memberEntity = new ProjectMemberEntity
                    {
                        ProjectId = projectEntity.Id,
                        UserId = parsedMemberId.ToString()
                    };

                    await _projectMemberRepository.AddAsync(memberEntity);
                }
                else
                {
                    Console.WriteLine($"Invalid GUID format for MemberId: {memberId}");
                }
            }
        }
        else
        {
            Console.WriteLine("No member IDs provided or MemberIds is null.");
        }

        return new ProjectResult { Succeeded = true, StatusCode = 201 };
    }

    //READ
    public async Task<ProjectResult<IEnumerable<Project>>> GetProjectsAsync()
    {
        var response = await _projectRepository.GetAllAsync(orderByDescending: true, sortBy: s => s.Created, where: null, i => i.User, i => i.Status, i => i.Client);

        return response.MapTo<ProjectResult<IEnumerable<Project>>>();
    }

    public async Task<ProjectResult<Project>> GetProjectAsync(string id)
    {
        var response = await _projectRepository.GetAsync(where: x => x.Id == id, i => i.User, i => i.Status, i => i.Client);
        return response.Succeeded
            ? response.MapTo<ProjectResult<Project>>()
           : new ProjectResult<Project> { Succeeded = false, StatusCode = 404, Error = $"Project with '{id}' was not found." };
    }

    //UPDATE
    public async Task<ProjectResult> UpdateProjectAsync(EditProjectFormData model)
    {
        if (model == null)
            return new ProjectResult { Succeeded = false, StatusCode = 400, Error = "Not all required fields are supplied." };

        var existingProject = await _projectRepository.GetEntityAsync(p => p.Id == model.Id);
        if (existingProject == null)
            return new ProjectResult { Succeeded = false, StatusCode = 400, Error = "Project not found." };

        existingProject.ProjectName = model.ProjectName!;
        existingProject.Budget = model.Budget;
        existingProject.Description = model.Description;
        existingProject.ClientId = model.ClientId!;
        existingProject.StatusId = model.StatusId;
        existingProject.StartDate = model.StartDate;
        existingProject.EndDate = model.EndDate;
        existingProject.Image = model.Image;
        var clearResult = await _projectMemberRepository.GetAllAsync(where: pm => pm.ProjectId == model.Id);

        foreach (var projectMember in clearResult.Result!)
        {
            var deleteMember = projectMember.MapTo<ProjectMemberEntity>();
            await _projectMemberRepository.DeleteAsync(deleteMember);
        }

        existingProject.Members.Clear();

        foreach (var memberId in model.MemberIds)
        {
            var newMember = new ProjectMemberEntity
            {
                UserId = memberId,
                ProjectId = existingProject.Id
            };

            existingProject.Members.Add(newMember);
        }

        var result = await _projectRepository.UpdateAsync(existingProject);

        return result.Succeeded
       ? new ProjectResult { Succeeded = true, StatusCode = 200 }
       : new ProjectResult { Succeeded = false, Error = result.Error };
    }

    //DELETE
    public async Task<ProjectResult> DeleteProjectAsync(string id)
    {
        if (string.IsNullOrEmpty(id))
            return new ProjectResult { Succeeded = false, StatusCode = 400, Error = "Project ID is required." };

        var projectEntity = await _projectRepository.GetEntityAsync(p => p.Id == id);
        if (projectEntity == null)
            return new ProjectResult { Succeeded = false, StatusCode = 404, Error = "Project not found." };


        var result = await _projectRepository.DeleteAsync(projectEntity);

        return result.Succeeded
            ? new ProjectResult { Succeeded = true, StatusCode = 200 }
            : new ProjectResult { Succeeded = false, StatusCode = result.StatusCode, Error = result.Error };
    }
}
