using Microsoft.AspNetCore.Identity;
using Project.Domain.Interfaces.Identity;
using Project.Domain.Models.Identity;

namespace Project.Infrastructure.Repositories.Identity
{
    public class UserRepository : IUserRepository
    {
        private readonly UserManager<Users> _userManager;

        public UserRepository(UserManager<Users> userManager)
        {
            _userManager = userManager;
        }

        public async Task<Users> GetByIdAsync(string id) =>
            await _userManager.FindByIdAsync(id);

        public async Task<IdentityResult> CreateAsync(Users user, string password) =>
            await _userManager.CreateAsync(user, password);

        public async Task<IdentityResult> UpdateAsync(Users user) =>
            await _userManager.UpdateAsync(user);

        public async Task<IdentityResult> DeleteAsync(Users user) =>
            await _userManager.DeleteAsync(user);
    }
}
