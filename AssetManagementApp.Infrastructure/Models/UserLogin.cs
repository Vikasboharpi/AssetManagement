using System;
using System.Collections.Generic;
using System.Text;

namespace AssetManagementApp.Infrastructure.Models
{
    public class UserLogin
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public DateTime LoginTime { get; set; } = DateTime.UtcNow;
        public string IpAddress { get; set; }
    }
}
