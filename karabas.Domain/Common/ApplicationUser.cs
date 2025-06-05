using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace rezolvam.Domain.Common
{
    public class ApplicationUser : IdentityUser
    {
        public string? FullName { get; set; }
    }
}