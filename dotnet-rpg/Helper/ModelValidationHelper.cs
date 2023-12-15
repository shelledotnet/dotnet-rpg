﻿using System.ComponentModel.DataAnnotations;

namespace dotnet_rpg.Helper
{
    public class ModelValidationHelper
    {

        public static IList<ValidationResult> ValidateModel(object model)
        {
            var results = new List<ValidationResult>();

            var validationContext = new ValidationContext(model, null, null);

            Validator.TryValidateObject(model, validationContext, results, true);

            if (model is IValidatableObject validatableModel)
                results.AddRange(validatableModel.Validate(validationContext));

            return results;
        }
    }
}
