using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JobApplication.Application.DTOs
{
    public class UpdateJobDto
    {
        [Required, StringLength(150)] public string Title { get; set; } = string.Empty;
        [Required, StringLength(4000)] public string Description { get; set; } = string.Empty;
    }
}
