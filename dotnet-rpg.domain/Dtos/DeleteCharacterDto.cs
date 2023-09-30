using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace dotnet_rpg.domain.Dtos
{
    public class DeleteCharacterDto
    {
        [Required(ErrorMessage = "{0} required")]
        [Range(1, 20, ErrorMessage = "{0} allowed values are from 1 to 20")]
        [DefaultValue(1)]
        public int Id { get; set; }
    }
}