using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dotnet_rpg.domain.Models
{
    public class GenToken
    {
        public string RefereshTokenId { get; set; }
        public string Token { get; set; }
        public string Username { get; set; }
        public DateTime ValidTo { get; set; }
        public DateTime ValidFrom { get; set; }
    }
}
