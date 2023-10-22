using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dotnet_rpg.domain.Models
{
    public class UserDto
    {
        [DefaultValue("Ola")]
        public string? Username { get; set; }

        [DefaultValue("ola@21")]
        public string? Password { get; set; }

      //  [DefaultValue("User")]
        public List<string> Role { get; set; }
    }

    
    public class LoginDto
    {
        [DefaultValue("Ola")]
        public string? Username { get; set; }

        [DefaultValue("ola@21")]
        public string? Password { get; set; }

    }
}
