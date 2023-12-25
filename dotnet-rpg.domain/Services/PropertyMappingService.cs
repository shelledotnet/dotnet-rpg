using dotnet_rpg.domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dotnet_rpg.domain.Services
{
   

    public class PropertyMappingService : IPropertyMappingService
    {
        //ensure to register this service 
        //    builder.Services.AddTransient<IPropertyMappingService,PropertyMappingService>();
        private readonly Dictionary<string, PropertyMappingValue> _authorPropertyMapping =
        new(StringComparer.OrdinalIgnoreCase)
        {
            //key mapping defination for this properties only else thee will be an exception
            {"Id", new(new[] {"Id"}) },
            {"State", new(new[] { "State" }) },
            {"Age", new(new[]{"Dob"},true) },
            {"Name", new(new[]{ "FirstName", "LastName" }) }

        };

        //always initialsed collections to avoid null refernce
        private readonly IList<IPropertyMapping> _propertyMappings = new List<IPropertyMapping>();

        public PropertyMappingService()
        {
            _propertyMappings.Add(new PropertyMapping<Employee, EmployeeResponseDto>(_authorPropertyMapping));
        }

        public Dictionary<string, PropertyMappingValue> GetPropertyMapping
            <TSource, TDestination>()
        {
            // get matching mapping 
            var matchingMapping = _propertyMappings
                                  .OfType<PropertyMapping<TSource, TDestination>>();
            if (matchingMapping.Count() == 1)
            {
                return matchingMapping.First().MappingDictionary;
            }
            throw new Exception($"Cannot find exact property mapping instance " +
                $" for <{typeof(TSource)},{typeof(TDestination)}");

        }
        public bool ValidMappingExistsFor<TSource, TDestination>(string fields)
        {
            var propertyMapping = GetPropertyMapping<TSource, TDestination>();
            if (string.IsNullOrWhiteSpace(fields))
            {
                return true;
            }
            // the string is seperted by "," so we split it.
            var fieldsAftrSplit = fields.Split(',');

            //run through the fileds clauses
            foreach (var field in fieldsAftrSplit)
            {
                //trim
                var trimmedField = field.Trim();

                //remove eerything after the first " " - if the fields are coming from an orderBy string , 
                //this part must be ignored
                var indexOfFirstSpace = trimmedField.IndexOf(' ');
                var propertyName = indexOfFirstSpace == -1 ? trimmedField : trimmedField.Remove(indexOfFirstSpace);

                //find the matching property
                if (!propertyMapping.ContainsKey(propertyName))
                {
                    return false;
                }
            }
            return true;
        }
    }

}
