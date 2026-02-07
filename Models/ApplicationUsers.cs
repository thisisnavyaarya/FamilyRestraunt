
using Microsoft.AspNetCore.Identity;

namespace FamilyRestraunt.Models
{
    public class ApplicationUsers : IdentityUser
    {
        public ICollection<Orders>? Orders { get; set; }
    }
}
