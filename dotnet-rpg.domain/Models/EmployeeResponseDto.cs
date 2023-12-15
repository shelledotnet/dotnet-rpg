using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dotnet_rpg.domain.Models
{
    public class EmployeeResponseDto
    {
        public int Id { get; set; }

        public string? Name { get; set; }
        public bool IsDeveloper { get; set; }
        public int Age { get; set; }
        public string? State { get; set; }
        public double Salary { get; set; }
        public string? Gender { get; set; }

        #region Reference Navigation Property
       

        public DepartmentDto? Department { get; set; }
        #endregion

        public DateTime CreatedDate { get; set; }

    }
}
