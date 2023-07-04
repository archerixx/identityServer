using IdentityModel;
using IdentityServer4.Models;
using IdentityServer4.Validation;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Project.Domain.Models.Identity;
using Project.Infrastructure;
using System.Security.Claims;
using static IdentityModel.OidcConstants;

namespace Project.IdentityServer.IdentitySpecific
{
    public class ResourceOwnerPasswordValidator : IResourceOwnerPasswordValidator
    {
        private readonly ILogger<ResourceOwnerPasswordValidator> _logger;
        private readonly Context _dbContext;
        private readonly UserManager<Users> _userManager;

        public ResourceOwnerPasswordValidator(ILogger<ResourceOwnerPasswordValidator> logger,
                                              Context dbContext,
                                              UserManager<Users> userManager)
        {
            _logger = logger;
            _dbContext = dbContext;
            _userManager = userManager;
        }
        public virtual async Task ValidateAsync(ResourceOwnerPasswordValidationContext context)
        {
            try
            {
                Users? user = await _dbContext.Users.FirstOrDefaultAsync(x => x.UserName == context.UserName);
                if (user == null)
                    return;

                // valdate password
                if (_userManager.PasswordHasher.VerifyHashedPassword(user, user.PasswordHash, context.Password) == PasswordVerificationResult.Success)
                {
                    context.Result = new GrantValidationResult(
                        subject: user.Id.ToString(),
                        authenticationMethod: AuthenticationMethods.Password);
                    // await _userManager.AddLoginAsync(user, new UserLoginInfo("BasicLogin", "0", user.UserName)); // In case we want to keep track of logins
                    // UserToken logic can be implemented, but it is meant only for external authentication
                }
                // if password validation fails
                else
                {
                    await UpdateUserDataOnFailedLogin(user);
                    context.Result = new GrantValidationResult(TokenRequestErrors.InvalidGrant);
                }
            }
            catch (Exception e)
            {
                _logger.LogError("Unable to validate credentials: " + JsonConvert.SerializeObject(e));
                context.Result = new GrantValidationResult(TokenRequestErrors.InvalidRequest);
            }
        }

        // check if user has assigned role
        public async Task<bool> ValidateRoleAsync(string username, string role)
        {
            var users = await _userManager.GetUsersInRoleAsync(role);
            if (users.FirstOrDefault(u => u.UserName == username) != null)
                return true;
            return false;
        }

        private async Task UpdateUserDataOnFailedLogin(Users user)
        {
            try
            {
                user.AccessFailedCount += 1;
                if (user.AccessFailedCount > 10)
                {
                    user.LockoutEnabled = true;
                    user.LockoutEnd = DateTime.Now.AddHours(1);
                }

                _dbContext.Users.Update(user);
                await _dbContext.SaveChangesAsync();
                _logger.LogInformation("Update user data on failed login success for user: {username} ", user.UserName);
            }
            catch (Exception e)
            {
                _logger.LogError("Update user data failed for user: {username}, reason: " + e.Message, user.UserName);

            }
        }

    }
}
