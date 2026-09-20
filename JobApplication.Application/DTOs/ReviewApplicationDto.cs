using JobApplication.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace JobApplication.Application.DTOs
{

    public class ReviewApplicationDto
    {
        [EnumDataType(typeof(JobApplicationStatus))]
        public JobApplicationStatus NewStatus { get; set; }
    }
}
