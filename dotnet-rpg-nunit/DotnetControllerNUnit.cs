using dotnet_rpg.domain.Models;
using dotnet_rpg.Helper;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dotnet_rpg_nunit
{
    public class DotnetControllerNUnit
    {
        [Test]
        public void Customer_ShouldPassValidation_WhenModelIsValid()
        {
            var model = new Customer { FirstName = "Davide", LastName = "Bellone", Age = 32 };
            var validationResult = ModelValidationHelper.ValidateModel(model);
            Assert.That(validationResult, Is.Empty);
            //Is.Empty  means the we should assert  that there is no failed value which most passed

        }

        [Test]
        public void Customer_ShouldNotPassValidation_WhenLastNameIsEmpty()
        {
            var model = new Customer { FirstName = "Davide", LastName = null, Age = 32 };
            var validationResult = ModelValidationHelper.ValidateModel(model);
            Assert.That(validationResult, Is.Not.Empty);
            //Is.Not.Empty  means the we should assert  that there is a  failed value which most passed

        }


        [Test]
        public void Customer_ShouldNotPassValidation_WhenLastNameIsLessThan_RequiredLength()
        {
            var model = new Customer { FirstName = "Davide", LastName = "ad", Age = 32 };
            var validationResult = ModelValidationHelper.ValidateModel(model);
            Assert.That(validationResult, Is.Not.Empty);
            //Is.Not.Empty  means the we should assert  that there is a  failed value which mosnt passed
        }





        [Test]
        public void Customer_ShouldNotPassValidation_WhenAgeIsLessThan18()
        {
            var model = new Customer { FirstName = "Davide", LastName = "Bellone", Age = 10 };
            var validationResult = ModelValidationHelper.ValidateModel(model);
            Assert.That(validationResult, Is.Not.Empty);
            //Is.Not.Empty  means the we should assert  that there is a  failed value which most passed

        }
    }
}
