﻿using dotnet_rpg.domain.Models;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace dotnet_rpg.domain.Dtos
{
  
    public class AddCharacterDto
    {
        //public int Id { get; set; }

        [Required(ErrorMessage = "{0} is required")]
        [RegularExpression(@"^[A-Za-z.-]+$", ErrorMessage = "invalid {0} ")]
        [StringLength(50,ErrorMessage ="{0} max length is 50"),MinLength(3,ErrorMessage ="{0} min length is 3")]
        [DefaultValue("sheyi")]
        public string? Name { get; set; }

        [Required(ErrorMessage = "{0} is required")]
        [RegularExpression(@"^[0-9]+$", ErrorMessage = "invalid {0} ")]
        [Range(1, 300, ErrorMessage = "invalid value for {0}")]
        [DefaultValue(10)]
        public int HitPoints { get; set; }

        [Required(ErrorMessage = "{0} is required")]
        [RegularExpression(@"^[0-9]+$", ErrorMessage = "invalid {0} ")]
        [Range(1, 300, ErrorMessage = "invalid value for {0}")]
        [DefaultValue(10)]
        public int Strength { get; set; }

        [Required(ErrorMessage = "{0} is required")]
        [RegularExpression(@"^[0-9]+$", ErrorMessage = "invalid {0} ")]
        [Range(1, 300, ErrorMessage = "invalid value for {0}")]
        [DefaultValue(10)]
        public int Defense { get; set; }

        [Required(ErrorMessage = "{0} is required")]
        [RegularExpression(@"^[0-9]+$", ErrorMessage = "invalid {0} ")]
        [Range(1, 300, ErrorMessage = "invalid value for {0}")]
        [DefaultValue(10)]
        public int Intelligence { get; set; }


        [EnumDataType
            (typeof(RpgClass), ErrorMessage = "character value can only be Knight , Mage or Cleric")]
        [DefaultValue("Knight")]
        public string Class { get; set; } 

        [DataType(DataType.Date)]
        [ValidDobDate]
        public string DOB { get; set; }



    }

    public class ValidDobDate : ValidationAttribute
    {

        protected override ValidationResult
                IsValid(object value, ValidationContext validationContext)
        {

            if (!DateTime.TryParse(value.ToString(), out DateTime result))
                return new ValidationResult
                   ("invalid value for DOB");


            DateTime _dateJoin = Convert.ToDateTime(result);

            if (_dateJoin.Date > DateTime.Now.Date)
            {
                return new ValidationResult
                     ("DOB  can't be greater than  current date.");
            }
            else
            {
                return ValidationResult.Success;

            }
        }
    }

    public class ValidClass : ValidationAttribute
    {
        protected override ValidationResult
                IsValid(object value, ValidationContext validationContext)
        {

            if (value.ToString().ToUpper() != "KNIGHT" && value.ToString().ToUpper() != "MAGE" && value.ToString().ToUpper() != "CLERIC")
            {
                return new ValidationResult
                     ("character value can only be Knight , Mage or Cleric");
            }

            else
            {
                return ValidationResult.Success;

            }
        }
    }
}
