using JobApplication.Application.Common;
using JobApplication.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JobApplication.Application.Services
{
    public interface IJobService
    {
        Task<Result<JobResponseDto>> CreateAsync(int recruiterId,CreateJobDto dto);
        Task<List<JobResponseDto>> GetAllAsync(bool? isActive);
        Task<Result<JobResponseDto>> GetByIdAsync(int id);
        Task<Result<JobResponseDto>> UpdateAsync(int id,int recruiterId, UpdateJobDto dto);
        Task<Result<JobResponseDto>> CloseAsync(int id, int recruiterId);
    }
}
