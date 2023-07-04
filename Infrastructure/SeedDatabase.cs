using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Project.Domain.Models.Identity;
using System.Security.Claims;

namespace Project.Infrastructure
{
    public class SeedDatabase
    {
        public SeedDatabase(IServiceScopeFactory serviceScopeFactory)
        {
            using (var scope = serviceScopeFactory.CreateScope())
            {
                EnsureUsers(scope);
            }
        }

        // adding test users through seed
        private static void EnsureUsers(IServiceScope scope)
        {
            try
            {
                var userManager = scope.ServiceProvider.GetRequiredService<UserManager<Users>>();
                var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<Roles>>();

                var role = roleManager.FindByNameAsync("admin").Result;
                if (role == null)
                {
                    role = new Roles()
                    {
                        Name = "admin"
                    };
                    // creat role
                    var result = roleManager.CreateAsync(role).Result;

                    if (!result.Succeeded)
                    {
                        throw new Exception(result.Errors.First().Description);
                    }

                    Console.WriteLine($"{role.Name} role created");
                }

                var user = userManager.FindByNameAsync("qss").Result;
                if (user == null)
                {
                    user = new Users()
                    {
                        UserName = "qss",
                        Email = "qss@qssbh.com",
                        Active = true
                    };

                    // creates user
                    var result = userManager.CreateAsync(user, password: "Test1234!").Result;
                    // creates role
                    result = userManager.AddToRoleAsync(user, "admin").Result;

                    if (!result.Succeeded)
                    {
                        throw new Exception(result.Errors.First().ToString());
                    }

                    Console.WriteLine("User created");
                }
                else
                {
                    Console.WriteLine($"{user.UserName} already exists");
                }

                role = roleManager.FindByNameAsync("basic").Result;
                if (role == null)
                {
                    role = new Roles()
                    {
                        Name = "basic"
                    };
                    // creat role
                    var result = roleManager.CreateAsync(role).Result;

                    if (!result.Succeeded)
                    {
                        throw new Exception(result.Errors.First().Description);
                    }

                    Console.WriteLine($"{role.Name} role created");
                }

                user = userManager.FindByNameAsync("test").Result;
                if (user == null)
                {
                    user = new Users()
                    {
                        UserName = "test",
                        Email = "test@test.com",
                        Active = true
                    };

                    // creates user
                    var result = userManager.CreateAsync(user, password: "Test1234!").Result;
                    // creates role
                    result = userManager.AddToRoleAsync(user, "basic").Result;

                    if (!result.Succeeded)
                    {
                        throw new Exception(result.Errors.First().ToString());
                    }

                    Console.WriteLine("User created");
                }
                else
                {
                    Console.WriteLine($"{user.UserName} already exists");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception while seeding data: " + ex.Message);
            }
        }
    }
}
