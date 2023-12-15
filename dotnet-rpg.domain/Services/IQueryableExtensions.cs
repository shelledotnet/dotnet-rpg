using dotnet_rpg.domain.Services;
using Serilog;
using System.Linq.Dynamic.Core;

namespace dotnet_rpg.domain.Services
{
    public static class IQueryableExtensions
    {

        public static IQueryable<T> ApplySort<T>(
            this IQueryable<T> source,
            string orderBy,
            Dictionary<string,PropertyMappingValue> mappingDictionary)
        {
            try
            {
                if (source == null)
                {
                    throw new ArgumentNullException(nameof(source));
                }
                if (mappingDictionary == null)
                {
                    throw new ArgumentNullException(nameof(mappingDictionary));
                }
                if (string.IsNullOrWhiteSpace(orderBy))
                {
                    return source;
                }

                var orderByString = string.Empty;

                //the ordeby string is seperated by "," so we split it
                var orderByAfterSplit = orderBy.Split(",");

                //apply each orderby clause
                foreach (var orderByClause in orderByAfterSplit)
                {
                    //trim the orderBy clause , as it might contain leading or trailing spaces..
                    //cant trim var in foreach, so use another var 
                    var trimmedOrderByClause = orderByClause.Trim();

                    //if the sort option ends with desc, we order descending  otherwise ascending
                    var orderDescending = trimmedOrderByClause.EndsWith(" desc");

                    //remove asc or desc from the orderBy clause so we get the property name to look for in the mapping dictionary
                    var indexOfFirstSpace = trimmedOrderByClause.IndexOf(" ");

                    var propertyName = indexOfFirstSpace == -1 ? trimmedOrderByClause : trimmedOrderByClause
                                       .Remove(indexOfFirstSpace);
                    //find the matching property
                    if (!mappingDictionary.ContainsKey(propertyName))
                    {
                        throw new ArgumentException($"key mapping for {propertyName} is missing");
                    }
                    //get the propertyMappingvalue
                    var propertyMappingValue = mappingDictionary[propertyName];
                    if (propertyMappingValue == null)
                    {
                        throw new ArgumentNullException(nameof(propertyMappingValue));
                    }

                    //revert sort ordr if necessary
                    if (propertyMappingValue.Revert)
                    {
                        orderDescending = !orderDescending;
                    }
                    //Run through the property nams
                    foreach (var destinationProperty in propertyMappingValue.DestinationProperties)
                    {
                        orderByString = orderByString
                            + (string.IsNullOrWhiteSpace(orderByString) ? string.Empty : ", ")
                            + destinationProperty
                            + (orderDescending ? " descending" : " ascending");
                    }

                }
                return source.OrderBy(orderByString);

            }
            catch (Exception ex)
            {
                string message = $"{ex}";
                Log.Error(message);
                throw;
            }
        }
    }
}
