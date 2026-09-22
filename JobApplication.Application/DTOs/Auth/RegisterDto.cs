using JobApplication.Domain.Enums;
using System;
using System.ComponentModel.DataAnnotations;
namespace JobApplication.Application.DTOs.Auth
{
    public class RegisterDto
    {
        [Required, MinLength(10),StringLength(100)] public string FullName { get; set; } = string.Empty;
        [Required, EmailAddress] public string Email { get; set; } = string.Empty;
        [Required, MinLength(8)] public string Password { get; set; } = string.Empty;
        [EnumDataType(typeof(UserRole))] public UserRole Role { get; set; }
    }
}
