using dotnet_rpg.domain.Models;
using System.ComponentModel.DataAnnotations;

namespace dotnet_rpg.domain.Dtos
{
    public class GetCharacterDto
    {
        public int Id { get; set; }
        public string? Name { get; set; }

        public int HitPoints { get; set; }

        public int Strength { get; set; }

        public int Defense { get; set; }

        public int Intelligence { get; set; }

        public string? State { get; set; }

        public string? Gender { get; set; }

       
        public RpgClass Class { get; set; }
    }
}
