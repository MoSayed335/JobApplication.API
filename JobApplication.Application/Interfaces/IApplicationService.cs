using JobApplication.Application.Common;
using JobApplication.Application.DTOs;
using JobApplication.Domain.Enums;

namespace JobApplication.Application.Interfaces
{
    public interface IApplicationService
    {
        Task<Result<ApplicationResponseDto>> CreateAsync(int candidateId, CreateApplicationDto dto);
        Task<List<ApplicationResponseDto>> GetAllAsync(int? jobId, int? candidateId, int? recruiterId, JobApplicationStatus? status);
        Task<Result<ApplicationResponseDto>> ReviewAsync(int id, int recruiterId, ReviewApplicationDto dto);
        Task<Result<ApplicationResponseDto>> CancelAsync(int id, int candidateId);
    }
}
