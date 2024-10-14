using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace AppCoreAPI.Data.Entities
{
    public class AppRole : IdentityRole<int>
    {
        [Required]
        [MaxLength(200)]
        public required string DisplayName { get; set; }
    }
}
