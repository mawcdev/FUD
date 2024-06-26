using Microsoft.AspNetCore.Identity;

namespace FUD.Services.AuthAPI.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string Name { get; set; }
    }
}
