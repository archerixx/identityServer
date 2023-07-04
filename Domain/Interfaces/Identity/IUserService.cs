using Microsoft.AspNetCore.Identity;
using Project.Domain.Models.Identity;

namespace Project.Domain.Interfaces.Identity
{
    public interface IUserService
    {
        public Task<Users> GetByIdAsync(string id);

        public Task<IdentityResult> CreateAsync(Users user, string password);

        public Task<IdentityResult> UpdateAsync(Users user);

        public Task<IdentityResult> DeleteAsync(Users user);
    }
}
