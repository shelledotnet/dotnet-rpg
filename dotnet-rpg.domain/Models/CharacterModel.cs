using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dotnet_rpg.domain.Models
{
    public class CharacterModel
    {
        [Required(ErrorMessage ="{0} required")]
        [Range(1,20,ErrorMessage ="{0} allowed values are from 1 to 20")]
        public int Id { get; set; }
    }
}
