using dotnet_rpg.domain.Dtos;
using System.ComponentModel.DataAnnotations;

namespace dotnet_rpg.domain.ValidationAttributes
{
    public class StrengthValueMustBeDifferentFromDefense : ValidationAttribute
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            var model = (AddCharacterDto)validationContext.ObjectInstance;

            if (model.Strength == model.Defense)
            {
                 return new ValidationResult("the provided strength value should be diffeerent from Defense");
            }

            return ValidationResult.Success;
        }
    }
}
