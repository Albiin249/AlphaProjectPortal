using AlphaPortal.Hubs;
using AlphaPortal.Models;
using Business.Interfaces;
using Data.Contexts;
using Data.Entities;
using Data.Interfaces;
using Domain.Dtos;
using Domain.Extensions;
using Domain.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.SignalR;


namespace AlphaPortal.Controllers;

[Authorize]
public class ProjectsController(IProjectService projectService, IClientService clientService, IUserService userService, IProjectMemberRepository projectMemberRepository, IStatusService statusService, INotificationService notificationService, IHubContext<NotificationHub> notificationHub) : Controller
{
    private readonly IProjectService _projectService = projectService;
    private readonly IClientService _clientService = clientService;
    private readonly IUserService _userService = userService;
    private readonly IProjectMemberRepository _projectMemberRepository = projectMemberRepository;
    private readonly IStatusService _statusService = statusService;
    private readonly INotificationService _notificationService = notificationService;
    private readonly IHubContext<NotificationHub> _notificationHub = notificationHub;

    public async Task<IActionResult> Index(int? status = null)

    {
        Response.Cookies.Append("SessionCookie", "Essential", new CookieOptions
        {
            IsEssential = true,
            Expires = DateTimeOffset.UtcNow.AddYears(1)
        });

        var result = await _projectService.GetProjectsAsync();

        if (!result.Succeeded || result.Result == null)
        {
            return View(new ProjectsViewModel());
        }
        var allProjects = result.Result ?? new List<Project>();
        var ongoingCount = allProjects.Count(p => p.StatusId == 1);
        var completedCount = allProjects.Count(p => p.StatusId == 2);
        var totalCount = allProjects.Count();

        var projectViewModels = result.Result.Select(project =>
        {
            var projectViewModel = project.MapTo<ProjectViewModel>();
            projectViewModel.StatusName = project.Status?.StatusName ?? "Okänd status";
            if (!string.IsNullOrEmpty(project.Image))
                projectViewModel.ProjectImage = project.Image;
            else
                projectViewModel.ProjectImage = "images/projects/project-template.svg";

            return projectViewModel;

        }).ToList();

        if (status.HasValue)
        {
            projectViewModels = projectViewModels
                .Where(p => p.StatusId == status.Value)
                .ToList();
        }

        foreach (var project in projectViewModels)
        {
            
            var memberResult = await _projectMemberRepository.GetAllAsync(
                where: pm => pm.ProjectId == project.Id ) ;
            
            var members = new List<MemberImageViewModel>();

            if (memberResult.Succeeded && memberResult.Result != null)
            {
                foreach (var projectMember in memberResult.Result)
                {
                    var userResult = await _userService.GetUserAsync(projectMember.UserId);
                    var user = userResult.Result?.FirstOrDefault();
                    if (user != null)
                    {
                        members.Add(new MemberImageViewModel
                        {
                            Id = user.Id,
                            FirstName = user.FirstName,
                            LastName = user.LastName,
                            ProfileImage = user.ProfileImage
                        });
                    }
                }
            }
            project.Members = members;

            if (!string.IsNullOrEmpty(project.ClientId))
            {
                var client = await _clientService.GetClientByIdAsync(project.ClientId);
                project.ClientName = client.Result?.ClientName ?? "Unknown Client";
            }
        }

        var clients = await SetClients();
        var membersDropdown = await SetMembers();
        var statuses = await SetStatuses();

        var model = new ProjectsViewModel
        {
            Projects = projectViewModels,
            AddProjectFormData = new AddProjectViewModel
            {
                Clients = clients,
                Members = membersDropdown,
                Statuses = statuses
            },
            EditProjectFormData = new EditProjectViewModel
            {
                Clients = clients,
                Members = membersDropdown,
                Statuses = statuses
            },
            TotalCount = totalCount,
            OngoingCount = ongoingCount,
            CompletedCount = completedCount
        };

        return View(model);
    }


    [HttpPost]
    public async Task<IActionResult> Add(AddProjectViewModel model)
    {
        var userId = _userService.GetUserId(User);

        if (userId == null)
        {
            ModelState.AddModelError(string.Empty, "You must be signed in to create a project.");
            return BadRequest(new { success = false, message = "You must be signed in to create a project." });
        }

        if (!ModelState.IsValid)
        {
            var errors = ModelState
                .Where(x => x.Value?.Errors.Count > 0)
                .ToDictionary(
                    kvp => kvp.Key,
                    kvp => kvp.Value?.Errors.Select(x => x.ErrorMessage).ToArray()
                );
            return BadRequest(new { success = false, errors });
        }


        string? imagePath = null;

        if (model.ImageFile != null && model.ImageFile.Length > 0)
        {
            var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "projects");
            if (!Directory.Exists(uploadsFolder))
                Directory.CreateDirectory(uploadsFolder);

            var originalFileName = Path.GetFileName(model.ImageFile.FileName);
            var guid = Guid.NewGuid().ToString();
            var shortGuid = guid.Substring(0, 10);
            var uniqueFileName = $"{shortGuid}_{originalFileName}";
            var filePath = Path.Combine(uploadsFolder, uniqueFileName);
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await model.ImageFile.CopyToAsync(stream);
            }

            imagePath = Path.Combine("images", "projects", uniqueFileName).Replace("\\", "/");
        }

        

        var formData = model.MapTo<AddProjectFormData>();

        formData.UserId = userId;
        formData.Image = imagePath;
        formData.MemberIds = model.SelectedUserIds;

        await _projectService.CreateProjectAsync(formData);

        var project = await _projectService.GetProjectAsync(formData.Id!);
        if (project != null)
        {
            var notificationEntity = new NotificationEntity
            {
                Message = $"Project {formData.ProjectName} has been created.",
                NotificationTypeId = 2
            };
            await _notificationService.AddNotificationAsync(notificationEntity);
            var notifications = await _notificationService.GetNotificationsAsync(formData.Id!);
            var newNotification = notifications.OrderByDescending(x => x.Created).FirstOrDefault();

            if (newNotification != null)
            {
                await _notificationHub.Clients.All.SendAsync("ReceiveNotification", newNotification);
            }
        }

        return Ok();
    }



    [HttpPost]
    public async Task<IActionResult> Edit(EditProjectViewModel model)
    {
        var projectResult = await _projectService.GetProjectAsync(model.Id);


        if (!ModelState.IsValid)
        {
            var errors = ModelState
                .Where(x => x.Value?.Errors.Count > 0)
                .ToDictionary(
                    kvp => kvp.Key,
                    kvp => kvp.Value?.Errors.Select(x => x.ErrorMessage).ToArray()
                );
            return BadRequest(new { success = false, errors });
        }
        string? imagePath = model.ExistingProfileImagePath;

        if (model.ImageFile != null && model.ImageFile.Length > 0)
        {

            var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "projects");
            if (!Directory.Exists(uploadsFolder))
                Directory.CreateDirectory(uploadsFolder);

            var originalFileName = Path.GetFileName(model.ImageFile.FileName);
            var guid = Guid.NewGuid().ToString();
            var shortGuid = guid.Substring(0, 10);
            var uniqueFileName = $"{shortGuid}_{originalFileName}";
            var filePath = Path.Combine(uploadsFolder, uniqueFileName);
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await model.ImageFile.CopyToAsync(stream);
            }

            imagePath = Path.Combine("images", "projects", uniqueFileName).Replace("\\", "/");
        }

        var updateProjectFormData = model.MapTo<EditProjectFormData>();
        updateProjectFormData.MemberIds = model.SelectedUserIds;
        updateProjectFormData.StatusId = model.StatusId;
        updateProjectFormData.Image = imagePath;

        var result = await _projectService.UpdateProjectAsync(updateProjectFormData);

        if (!result.Succeeded)
        {
            return Json(new { success = false, message = result.Error ?? "Något gick fel vid uppdateringen." });
        }



        return Ok(); 
    }





    [HttpPost]
    public async Task<IActionResult> Delete(string id)
    {
        if (string.IsNullOrWhiteSpace(id))
            return BadRequest(new { success = false, message = "Project ID is missing." });

        var result = await _projectService.DeleteProjectAsync(id);

        if (!result.Succeeded)
            return StatusCode(result.StatusCode, new { success = false, message = result.Error });

        return RedirectToAction("Index");
    }


    private async Task<IEnumerable<SelectListItem>> SetClients()
    {
        var clients = await _clientService.GetClientsAsync();

        var exportClients = clients.Result.Select(client => new SelectListItem
        {
            Value = client.Id.ToString(),
            Text = client.ClientName        

        });

        return exportClients;
    }

    private async Task<IEnumerable<SelectListItem>> SetMembers()
    {
        var members = await _userService.GetUsersAsync();

        var exportMembers = members.Result.Select(member => new SelectListItem
        {
            Value = member.Id.ToString(),   
            Text = member.Email        

        });

        return exportMembers;
    }

    private async Task<IEnumerable<SelectListItem>> SetStatuses()
    {
        var statuses = await _statusService.GetStatusesAsync();

        var exportStatuses = statuses.Result.Select(status => new SelectListItem
        {
            Value = status.Id.ToString(),
            Text = status.StatusName

        });

        return exportStatuses;
    }

    [HttpGet]
    public async Task<JsonResult> SearchUsers(string term)
    {
        if (string.IsNullOrWhiteSpace(term))
            return Json(new List<object>());

        var result = await _userService.GetUsersAsync();

        var users = result.Result
       .Where(x =>
           (x.FirstName ?? "").Contains(term) ||
           (x.LastName ?? "").Contains(term) ||
           x.Email.Contains(term))
       .Select(x => new
       {
           x.Id,
           FullName = $"{x.FirstName} {x.LastName}".Trim(),
           imageUrl = string.IsNullOrEmpty(x.ProfileImage)
            ? "user-template-male.svg"
            : x.ProfileImage
       })
       .ToList();

        return Json(users);
    }
}
