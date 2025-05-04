using Data.Contexts;
using Data.Entities;
using Data.Interfaces;
using Domain.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Data.Repositories;

public class ClientRepository(AppDbContext context) : BaseRepository<ClientEntity, Client>(context), IClientRepository
{
    public async Task<ClientEntity?> GetEntityAsync(Expression<Func<ClientEntity, bool>> predicate)
    {
        return await _table.FirstOrDefaultAsync(predicate);
    }

}

