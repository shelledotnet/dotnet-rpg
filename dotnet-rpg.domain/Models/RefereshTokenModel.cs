using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace dotnet_rpg.domain.Models
{

    public class SHA512Converter
    {
        public static string GenerateSHA512String(string inputString)
        {
            SHA512 sha512 = SHA512Managed.Create();
            byte[] bytes = Encoding.UTF8.GetBytes(inputString);
            byte[] hash = sha512.ComputeHash(bytes);
            return GetStringFromHash(hash);
        }

        public static string GetStringFromHash(byte[] hash)
        {
            StringBuilder result = new StringBuilder();
            for (int i = 0; i < hash.Length; i++)
            {
                result.Append(hash[i].ToString("X2"));
            }
            return result.ToString();
        }
    }

    public class RefereshTokenModel
    {
        public int Id { get; set; }
        public string? UserId { get; set; }
        public string? JwtId { get; set; }   //jwt id of the refereshToken
        public string? Token { get; set; }  //refereshToken

        public bool IsUsed { get; set; }
        public bool IsRevoked { get; set; }

        public DateTime DateCreated { get; set; }
        public DateTime DateExpired { get; set; }

        [ForeignKey(nameof(UserId))]
        public User User { get; set; }



    }
}
