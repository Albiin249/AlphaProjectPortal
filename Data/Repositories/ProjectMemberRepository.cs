using Data.Contexts;
using Data.Entities;
using Data.Interfaces;
using Domain.Models;

namespace Data.Repositories;

public class ProjectMemberRepository(AppDbContext context) : BaseRepository<ProjectMemberEntity, ProjectMember>(context), IProjectMemberRepository
{
}

