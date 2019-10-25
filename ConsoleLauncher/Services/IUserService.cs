using System.Threading.Tasks;
using ConsoleLauncher.Models;

namespace ConsoleLauncher.Services
{
    public interface IUserService
    {
        Task<User> GetUser();
        Task<bool> HasSession();
        Task Login(string email, string password);
    }
}