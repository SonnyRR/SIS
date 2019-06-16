using Panda.Models;
using System.Collections.Generic;

namespace Panda.Services
{
    public interface IUserService
    {
        User GetUserOrNull(string username, string password);

        string CreateUser(string username, string email, string password);

        string GetUserIdByUsername(string username);

        ICollection<string> GetUsernames();
    }
}
