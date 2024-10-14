using System.ComponentModel.DataAnnotations;

namespace AppCoreAPI.Dtos
{
    public class RegisterDto
    {
        [Required]
        public string UserName { get; set; }

        [Required]
        [StringLength(50)]
        public string DisplayName { get; set; }

        [Required]
        [StringLength(25, MinimumLength = 6)]
        public string Password { get; set; }
    }
}
