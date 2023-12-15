using System.ComponentModel.DataAnnotations;

namespace dotnet_rpg.domain.ValidationAttributes
{
    public class ValidDobDateForUpdate : ValidationAttribute
    {

        protected override ValidationResult
                IsValid(object value, ValidationContext validationContext)
        {

            if (value == null)
                return ValidationResult.Success;


            if (!DateTime.TryParse(value?.ToString(), out DateTime result))
                return new ValidationResult
                   ("invalid value for DOB");


            DateTime _dateJoin = Convert.ToDateTime(result);

            if (_dateJoin.Date > DateTime.Now.Date)
            {
                return new ValidationResult
                     ("DOB  can't be greater than  current date.");
            }
            return ValidationResult.Success;

        }
    }
}
