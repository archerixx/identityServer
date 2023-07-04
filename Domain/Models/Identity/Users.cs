using Domain.Models.Identity;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace Project.Domain.Models.Identity
{
    public class Users : IdentityUser<Guid>
    {
        [StringLength(50)]
        public string? FirstName { get; set; }

        [StringLength(50)]
        public string? LastName { get; set; }

        public string? Address { get; set; }
        public string? Place { get; set; }
        public bool Active { get; set; }
        public DateTime CreatedOn { get; set; }

        public virtual ICollection<UserClaims>? UserClaims { get; set; }
        public virtual ICollection<UserLogins>? UserLogins { get; set; }
        public virtual ICollection<UserTokens>? UserTokens { get; set; }
        public virtual ICollection<UserRoles>? UserRoles { get; set; }
    }
}
