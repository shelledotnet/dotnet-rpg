using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dotnet_rpg.domain.Models
{
    public class ServiceResponse<T>
    {
        public T? Data { get; set; }

        public bool Success { get; set; } = true;

        public string Message { get; set; } = string.Empty;
    }
    public class ServiceResponse
    {


        public bool Success { get; set; } = false;

        public string Message { get; set; } = "failed";
    }
    public class ServiceBadResponse
    {


        public bool Success { get; set; } = false;

        public List<string>? Message { get; set; } 
    }
    public class ServiceFailedResponse
    {


        public bool Success { get; set; } = false;

        public string? Message { get; set; }
    }
}
