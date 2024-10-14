using Microsoft.AspNetCore.Identity;

namespace AppCoreAPI.Data.Entities
{
    public class AppUser : IdentityUser<int>
    {
        public string DisplayName { get; set; }


        public RootFolder RootFolder { get; set; }
    }
}
