using System.ComponentModel.DataAnnotations;

namespace JWTBackendAuth.Models
{
    public struct TokenModel
    {
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
    }

    public class RefreshTokenModel
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public required string UserName { get; set; }
        [Required]
        public required string RefreshToken { get; set; }
        public bool IsActive { get; set; } = true;
    }
}
