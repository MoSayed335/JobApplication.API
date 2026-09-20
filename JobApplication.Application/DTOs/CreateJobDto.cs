using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace JobApplication.Application.DTOs
{
    public class CreateJobDto
    {
        [Required, StringLength(150)] public string Title { get; set; } = string.Empty;
        [Required, StringLength(4000)] public string Description { get; set; } = string.Empty;
    }
}
