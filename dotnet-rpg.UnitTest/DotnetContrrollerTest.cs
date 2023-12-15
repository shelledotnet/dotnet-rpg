using dotnet_rpg.domain.Models;
using dotnet_rpg.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dotnet_rpg.UnitTest
{
    public class DotnetContrrollerTest
    {
        [Test]
        public void User_ShouldPassValidation_WhenModelIsValid()
        {
            var model = new Customer { FirstName = "Davide", LastName = "Bellone", Age = 32 };
            var validationResult = ModelValidationHelper.ValidateModel(model);
            Assert.That(validationResult, Is.Empty);
        }

        [Test]
        public void User_ShouldNotPassValidation_WhenLastNameIsEmpty()
        {
            var model = new Customer { FirstName = "Davide", LastName = null, Age = 32 };
            var validationResult = ModelValidationHelper.ValidateModel(model);
            Assert.That(validationResult, Is.Not.Empty);
        }


        [Test]
        public void User_ShouldNotPassValidation_WhenAgeIsLessThan18()
        {
            var model = new Customer { FirstName = "Davide", LastName = "Bellone", Age = 10 };
            var validationResult = ModelValidationHelper.ValidateModel(model);
            Assert.That(validationResult, Is.Not.Empty);
        }
    }
}
