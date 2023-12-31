﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dotnet_rpg.domain.Models
{
    public class User
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string? Username { get; set; }
        public byte[] PasswordHash { get; set; }
        public List<string> Role { get; set; }
        public byte[] PasswordSalt { get; set; }

        public string? RefreshToken { get; set; }
        public DateTime TokenCreated { get; set; }
        public DateTime TokenExpired { get; set; }
    }
}
