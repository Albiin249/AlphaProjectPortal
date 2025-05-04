using Data.Entities;
using Domain.Models;
using System.Linq.Expressions;

namespace Data.Interfaces;

public interface IClientRepository : IBaseRepository<ClientEntity, Client>
{
    Task<ClientEntity?> GetEntityAsync(Expression<Func<ClientEntity, bool>> predicate);
}

