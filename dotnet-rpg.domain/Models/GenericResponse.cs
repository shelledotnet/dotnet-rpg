using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dotnet_rpg.domain.Models
{
    public class GenericResponse<T>
    {
        public T? Data { get; set; }

        public string? Code { get; set; }

        public string? Description { get; set; }
    }
    public class GenericFailed
    {
      

        public string? Code { get; set; }

        public string? Description { get; set; }
    }


}
