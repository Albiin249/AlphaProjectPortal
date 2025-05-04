using Data.Contexts;
using Data.Entities;
using Data.Interfaces;
using Data.Models;
using Domain.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Data.Repositories;

public class UserRepository(AppDbContext context) : BaseRepository<UserEntity, User>(context), IUserRepository
{
    public async Task<UserEntity?> GetEntityAsync(Expression<Func<UserEntity, bool>> predicate)
    {
        return await _table.FirstOrDefaultAsync(predicate);
    }




}

