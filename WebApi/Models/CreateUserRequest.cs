using Project.Domain.Models.Identity;

namespace Project.WebApi.Models
{
    public class CreateUserRequest
    {
        public Users User { get; set; }
        public string password { get; set; }
    }
}
