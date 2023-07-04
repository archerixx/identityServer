using Microsoft.AspNetCore.Identity;

namespace Project.Domain.Models.Identity
{
    public class Roles : IdentityRole<Guid>
    {
        public virtual ICollection<UserRoles>? UserRoles { get; set; }
    }
}
