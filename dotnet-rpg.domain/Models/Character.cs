using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dotnet_rpg.domain.Models
{
    public class Character
    {
        public int Id { get; set; }
        public string Name { get; set; } 
        public int HitPoints   { get; set; }
        public int Strength { get; set; } 
        public int Defense { get; set; } 

        public int Intelligence { get; set; } 

        [EnumDataType
            (typeof(RpgClass), ErrorMessage = "invalid value for Class Type")]
        public RpgClass Class { get; set; }



    }
}
