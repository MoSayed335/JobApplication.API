using JobApplication.Domain.Entities;
using JobApplication.Domain.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace JobApplication.Application.DTOs
{
     public class CreateApplicationDto
    {
        [Range(1, int.MaxValue)] public int JobId { get; set; }
       }
}
