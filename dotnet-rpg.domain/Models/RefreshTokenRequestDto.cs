﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dotnet_rpg.domain.Models
{
    public class RefreshTokenRequestDto
    {
        public string? Token { get; set; }
        public string? RefreshToken { get; set; }
    }
}
