using dotnet_rpg.domain.Dtos;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using dotnet_rpg.domain.ValidationAttributes;

namespace dotnet_rpg.domain.Models
{
    public class UpdateCharacterDto
    {
        //[Required(ErrorMessage = "{0} is required")]
        [RegularExpression(@"^[0-9]+$", ErrorMessage = "invalid {0} ")]
        [DefaultValue(1)]
        public int? Id { get; set; }

        //[Required(ErrorMessage = "{0} is required")]
        [RegularExpression(@"^[A-Za-z.-]+$", ErrorMessage = "invalid {0} ")]
        [StringLength(50, ErrorMessage = "{0} max length is 50"), MinLength(3, ErrorMessage = "{0} min length is 3")]
        [DefaultValue("sheyi")]
        public string? Name { get; set; }

        //[Required(ErrorMessage = "{0} is required")]
        [RegularExpression(@"^[0-9]+$", ErrorMessage = "invalid {0} ")]
        [Range(1, 300, ErrorMessage = "invalid value for {0}")]
        //[DefaultValue(10)]
        public int? HitPoints { get; set; }

        //[Required(ErrorMessage = "{0} is required")]
        [RegularExpression(@"^[0-9]+$", ErrorMessage = "invalid {0} ")]
        [Range(1, 300, ErrorMessage = "invalid value for {0}")]
        //[DefaultValue(10)]
        public int? Strength { get; set; }

        //[Required(ErrorMessage = "{0} is required")]
        [RegularExpression(@"^[0-9]+$", ErrorMessage = "invalid {0} ")]
        [Range(1, 300, ErrorMessage = "invalid value for {0}")]
        //[DefaultValue(10)]
        public int? Defense { get; set; }

        //[Required(ErrorMessage = "{0} is required")]
        [RegularExpression(@"^[0-9]+$", ErrorMessage = "invalid {0} ")]
        [Range(1, 300, ErrorMessage = "invalid value for {0}")]
        //[DefaultValue(10)]
        public int? Intelligence { get; set; }


        [EnumDataType
            (typeof(RpgClass), ErrorMessage = "class value can only be Knight , Mage or Cleric")]
        [DefaultValue("Knight")]
        public string? Class { get; set; }

        [Required(ErrorMessage = "{0} is required")]
        [DataType(DataType.Date)]
        [ValidDobDateForUpdate]
        public string? DOB { get; set; }
    }
    
}
