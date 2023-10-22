using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dotnet_rpg.domain.Models
{
    public class ProjectOptions
    {
        [Required(AllowEmptyStrings = false)]
        public string? XApiKey { get; set; }

        [Required(AllowEmptyStrings = false)]
        public string? SecreteKey { get; set; }

        [Required(AllowEmptyStrings = false)]
        public string? Audience { get; set; }

        [Required(AllowEmptyStrings = false)]
        public string? Issuer { get; set; }

        [Required(AllowEmptyStrings = false)]
        public TimeSpan TokenLifeTime { get; set; }


        #region MyRegion
        //[Required(AllowEmptyStrings = false)]
        //public string? Version { get; set; }

        ////[Range(0, 100)]
        ////public int? Mission { get; set; } = null;

        //[Required(AllowEmptyStrings = false)]
        //public string? Mission { get; set; }

        //[Required(AllowEmptyStrings = false)]
        //public string? CookieName { get; set; }

        //[Required(AllowEmptyStrings = false)]
        //public string? Description { get; set; }

        //[Required(AllowEmptyStrings = false)]
        //public string? Error { get; set; } 
        #endregion
    }
}
