using Data.Entities;
using Data.Models;
using Domain.Models;
using System.Linq.Expressions;

namespace Data.Interfaces;

public interface IUserRepository : IBaseRepository<UserEntity, User>
{
    Task<UserEntity?> GetEntityAsync(Expression<Func<UserEntity, bool>> predicate);
}

