using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dotnet_rpg.domain.Models
{
    public class FiltersByState
    {
        const int maxPageSize = 20;

        private int _pageSize = 10;

        [RegularExpression(@"^[A-Za-z]+$", ErrorMessage = "invalid {0} ")]
        [StringLength(50, ErrorMessage = "{0} max length is 50")]
        [DefaultValue("lagos")]
        public string? mainCategory { get; set; }

        [RegularExpression(@"^[A-Za-z]+$", ErrorMessage = "invalid {0} ")]
        [StringLength(50, ErrorMessage = "{0} max length is 50")]
        [DefaultValue("lagos")]
        public string? searchQuery { get; set; }

        [RegularExpression(@"^[0-9]+$", ErrorMessage = "invalid {0} ")]
        [Range(1,100,ErrorMessage ="maximum page number allowed is 100")]
        public int pageNumber { get; set; } = 1;

        [RegularExpression(@"^[0-9]+$", ErrorMessage = "invalid {0} ")]
        [Range(1,20,ErrorMessage ="20 data per page allowed")]
        public int pageSize 
        {
            get => _pageSize;
            set => _pageSize = (value > maxPageSize) ?  maxPageSize : value;
        }

        public string? orderBy { get; set; } = "name";
    }

    public class SalaryByGender
    {
        [Required(ErrorMessage ="{0} is required")]
        [RegularExpression(@"^[A-Za-z]+$", ErrorMessage = "invalid {0} ")]
        [StringLength(6, ErrorMessage = "{0} max length is 6")]
        [DefaultValue("male")]
        public string? gender { get; set; }

       
    }
}
