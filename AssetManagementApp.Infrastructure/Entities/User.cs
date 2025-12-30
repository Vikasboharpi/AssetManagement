using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;

namespace AssetManagementApp.Infrastructure.Entities
{
    public class User: IdentityUser
    {
        public string FullName { get; set; }
        public string EmploymentId { get; set; }
    }
}
