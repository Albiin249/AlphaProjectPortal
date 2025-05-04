using Business.Interfaces;
using Business.Models;
using Data.Contexts;
using Data.Entities;
using Data.Interfaces;
using Data.Repositories;
using Domain.Dtos;
using Domain.Extensions;
using Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace Business.Services;

public class StatusService(IStatusRepository statusRepository, AppDbContext context) : IStatusService
{
    private readonly IStatusRepository _statusRepository = statusRepository;
    private readonly AppDbContext _context = context;



    //CREATE
    public async Task<StatusResult> CreateStatusAsync(StatusEntity statusEntity)
    {
        if (statusEntity == null)
            return new StatusResult { Succeeded = false, StatusCode = 400, Error = "Not all required fields are supplied." };

    
        if (statusEntity.Id == 0)
            statusEntity.Id = 0;  
        

        try
        {
            //Tog hjälp från ChatGPT för att aktivera IDENTITY_INSERT, funkade inte annars.
            await _context.Database.ExecuteSqlRawAsync("SET IDENTITY_INSERT Statuses ON");

            await _statusRepository.AddAsync(statusEntity);

            await _context.SaveChangesAsync();

            await _context.Database.ExecuteSqlRawAsync("SET IDENTITY_INSERT Statuses OFF");

            return new StatusResult { Succeeded = true, StatusCode = 201 };
        }
        catch (Exception ex)
        {
            return new StatusResult { Succeeded = false, StatusCode = 500, Error = ex.Message };
        }
    }


    //READ
    public async Task<StatusResult<IEnumerable<Status>>> GetStatusesAsync()
    {
        var result = await _statusRepository.GetAllAsync();
        return result.Succeeded
            ? new StatusResult<IEnumerable<Status>> { Succeeded = true, StatusCode = 200, Result = result.Result }
            : new StatusResult<IEnumerable<Status>> { Succeeded = false, StatusCode = result.StatusCode, Error = result.Error };
    }

    public async Task<StatusResult<Status>> GetStatusByNameAsync(string statusName)
    {
        var result = await _statusRepository.GetAsync(x => x.StatusName == statusName);
        return result.Succeeded
            ? new StatusResult<Status> { Succeeded = true, StatusCode = 200, Result = result.Result }
            : new StatusResult<Status> { Succeeded = false, StatusCode = result.StatusCode, Error = result.Error };
    }
    public async Task<StatusResult<Status>> GetStatusByIdAsync(int id)
    {
        var result = await _statusRepository.GetAsync(x => x.Id == id);
        return result.Succeeded
            ? new StatusResult<Status> { Succeeded = true, StatusCode = 200, Result = result.Result }
            : new StatusResult<Status> { Succeeded = false, StatusCode = result.StatusCode, Error = result.Error };
    }

    //UPDATE
    public async Task<StatusResult> UpdateStatusAsync(StatusEntity statusEntity)
    {
        if (statusEntity == null)
            return new StatusResult { Succeeded = false, StatusCode = 400, Error = "Not all required fields are supplied." };

        var result = await _statusRepository.UpdateAsync(statusEntity);

        return result.Succeeded
            ? new StatusResult { Succeeded = true, StatusCode = 200 }
            : new StatusResult { Succeeded = false, StatusCode = result.StatusCode, Error = result.Error };
    }

    //DELETE
    public async Task<StatusResult> DeleteStatusAsync(int id)
    {
        var statusEntity = await _statusRepository.GetEntityAsync(s => s.Id == id);
        if (statusEntity == null)
            return new StatusResult { Succeeded = false, StatusCode = 404, Error = "Status not found." };


        var result = await _statusRepository.DeleteAsync(statusEntity);

        return result.Succeeded
            ? new StatusResult { Succeeded = true, StatusCode = 200 }
            : new StatusResult { Succeeded = false, StatusCode = result.StatusCode, Error = result.Error };
    }
}
