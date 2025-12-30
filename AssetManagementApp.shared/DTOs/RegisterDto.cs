using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace AssetManagementApp.shared.DTOs
{
    public class RegisterDto
    {
        [Required] public string FullName { get; set; }
        [Required] public string EmploymentId { get; set; }
        [Required, EmailAddress] public string Email { get; set; }
        [Required, MinLength(6)] public string Password { get; set; }
        [Required] public string ConfirmPassword { get; set; }
    }

}
