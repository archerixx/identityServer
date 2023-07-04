using Microsoft.AspNetCore.Identity;
using Project.Domain.Interfaces.Identity;
using Project.Domain.Models.Identity;

namespace Project.Service.Identity
{
    public class UserService : IUserService
    {
        private readonly IUserRepository userRepository;

        public UserService(IUserRepository userRepository)
        {
            this.userRepository = userRepository;
        }

        public async Task<Users> GetByIdAsync(string id) =>
            await userRepository.GetByIdAsync(id);

        public async Task<IdentityResult> CreateAsync(Users user, string password) =>
            await userRepository.CreateAsync(user, password);

        public async Task<IdentityResult> UpdateAsync(Users user) =>
            await userRepository.UpdateAsync(user);

        public async Task<IdentityResult> DeleteAsync(Users user) =>
            await userRepository.DeleteAsync(user);
    }
}
