using Business.Models;
using Data.Entities;
using Domain.Models;

namespace Business.Interfaces
{
    public interface IStatusService
    {
        Task<StatusResult> CreateStatusAsync(StatusEntity statusEntity);
        Task<StatusResult> DeleteStatusAsync(int id);
        Task<StatusResult<Status>> GetStatusByIdAsync(int id);
        Task<StatusResult<Status>> GetStatusByNameAsync(string statusName);
        Task<StatusResult<IEnumerable<Status>>> GetStatusesAsync();
        Task<StatusResult> UpdateStatusAsync(StatusEntity statusEntity);
    }
}