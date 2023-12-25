using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Text.Json.Serialization;

namespace dotnet_rpg.domain.Models
{
    public class Users
    {
        [Key]
        public int Id { get; set; }

        public string? Username { get; set; }

        //byte[] this will help us saving binary nformation here
        public byte[] Password { get; set; }

        //salt key to add more security to our password
        public byte[] PasswordKey { get; set; }

        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Email { get; set; }

        public bool Active { get; set; }
        public bool Blocked { get; set; }

        public ICollection<Order> Orders { get; set; }

        public ICollection<Role> Roles { get; set; }

        [DataType(DataType.Date)]
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime? CreatedDate { get; set; }
    }
    public class LoginRequestDto
    {

        [Required(ErrorMessage = "{0} is required")]
        [DefaultValue("adeolas")]
        public string? Username { get; set; }

        [Required(ErrorMessage = "{0} is required")]
        [DefaultValue("ade1234")]
        [DataType(DataType.Password)]
        public string? Password { get; set; }

    }
    public class UsersDto
    {
        public string? Username { get; set; }

        public string? Password { get; set; }

    }
    public class LoginResponseDto
    {
        public string? Username { get; set; }

        public string? Token { get; set; }

    }
    public class RegisterResponseDto
    {
        public string? Username { get; set; }

        public string? Token { get; set; }

    }

    public class RegisterRequestDto
    {
        [Required]
        [StringLength(50, MinimumLength = 7)]
        [DefaultValue("ade@yahoo.com")]
        [EmailAddress]
        public string? Email { get; set; }

        [Required(ErrorMessage = "FirstName is required")]
        [RegularExpression(@"^[a-zA-Z]+$", ErrorMessage = "invalid {0} ")]
        [DefaultValue("ade")]
        [StringLength(30, ErrorMessage = "{0} max Length is 30"), MinLength(3, ErrorMessage = "{0} must be at least 3 characters long")]
        public string? FirstName { get; set; }

        [Required(ErrorMessage = "{0} is required")]
        [RegularExpression(@"^[a-zA-Z]+$", ErrorMessage = "invalid {0} ")]
        [DefaultValue("ade")]
        [StringLength(30, ErrorMessage = "{0} max Length is 30"), MinLength(3, ErrorMessage = "{0} must be at least 3 characters long")]
        public string? LastName { get; set; }


        [Required]
        [DefaultValue("ade")]
        public string? Username { get; set; }

        [Required(ErrorMessage = "{0} is required")]
        [RegularExpression(@"^[0-9a-zA-Z]+$", ErrorMessage = "invalid {0} ")]
        [DefaultValue("ade1234")]
        [StringLength(30, ErrorMessage = "{0} max Length is 30"), MinLength(7, ErrorMessage = "{0} must be at least 7 characters long")]
        [DataType(DataType.Password)]
        public string? Password { get; set; }

        [Required]
        [StringLength(30, ErrorMessage = "{0} max Length is 30"), MinLength(7, ErrorMessage = "{0} must be at least 7 characters long")]
        [DataType(DataType.Password)]
        [DefaultValue("ade1234")]
        [Compare("Password", ErrorMessage = "password and confirmation Password must match.")]
        public string? ConfirmPassword { get; set; }
    }
    public class RefereshTokenModels
    {
        [Key]
        public int Id { get; set; }
        public int UsersId { get; set; }
        public string? JwtId { get; set; }   //jwt id of the refereshToken
        public string? Token { get; set; }  //refereshToken

        public bool IsUsed { get; set; }
        public bool IsRevoked { get; set; }

        public DateTime DateCreated { get; set; }
        public DateTime DateExpired { get; set; }

        [ForeignKey(nameof(UsersId))]
        public Users? Users { get; set; }

    }

    public class Order
    {
        public int Id { get; set; }
        public string? Status { get; set; }
        public int Quantity { get; set; }
        public decimal Total { get; set; }
        public string? Currency { get; set; }
        public int UsersId { get; set; }

        [JsonIgnore]
        [ForeignKey(nameof(UsersId))]
        public Users? Users { get; set; }
        //the JsonIgnore Attribute to the users object in order to hide it when doing Json serialization

        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime CreatedDate { get; set; }
    }

    public class Role
    {
        [Key]
        public int Id { get; set; }
        public string? Name { get; set; }

        public int UsersId { get; set; }

        [JsonIgnore]
        [ForeignKey(nameof(UsersId))]
        public Users? Users { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime CreatedDate { get; set; }
    }
}
