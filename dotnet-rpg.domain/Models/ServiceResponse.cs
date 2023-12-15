using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dotnet_rpg.domain.Models
{
    public class ServiceResponse<T>
    {
        public dynamic? PaginationParas { get; set; }
        public T? Data { get; set; }
        

        public string ResponseId { get; set; } = DateTime.Now.ToString("yyyyMMddHHmmssfffffff");
        public bool Success { get; set; } = true;

        public string? Message { get; set; } 
    }
    public class ServiceResponse
    {


        public bool Success { get; set; } = false;

        public string Message { get; set; } = "failed";
    }
    public class ServiceBadResponse
    {


        public bool Success { get; set; } = false;
        public string ResponseId { get; set; } = DateTime.Now.ToString("yyyyMMddHHmmssfffffff");

        public List<string>? Message { get; set; } 
    }
    public class ServiceMethodNotAailabeResponse
    {
        public string ResponseId { get; set; } = DateTime.Now.ToString("yyyyMMddHHmmssfffffff");


        public bool Success { get; set; } = false;

        public string? Message { get; set; }
    }
    public class ServiceFailedResponse
    {
        public string ResponseId { get; set; } = DateTime.Now.ToString("yyyyMMddHHmmssfffffff");


        public bool Success { get; set; } = false;

        public string? Message { get; set; }
    }

    public class ServiceForbidenResponse
    {
        public string ResponseId { get; set; } = DateTime.Now.ToString("yyyyMMddHHmmssfffffff");


        public bool Success { get; set; } = false;

        public string? Message { get; set; } = "unauthorized";
    }
}
