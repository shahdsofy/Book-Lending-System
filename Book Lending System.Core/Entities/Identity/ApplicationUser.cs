
using Microsoft.AspNetCore.Identity;
namespace Book_Lending_System.Core.Entities.Identity
{
    public class ApplicationUser : IdentityUser
    {
        public required string DisplayName { get; set; }
    }
}


