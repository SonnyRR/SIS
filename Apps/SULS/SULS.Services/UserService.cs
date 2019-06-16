namespace SULS.Services
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Security.Cryptography;
    using System.Text;

    using SULS.Data;
    using SULS.Models;

    public class UserService : IUserService
    {
        private readonly SULSContext context;

        public UserService(SULSContext context)
        {
            this.context = context;
        }

        public string CreateUser(string username, string email, string password)
        {
            var user = new User
            {
                Username = username,
                Email = email,
                Password = this.HashPassword(password),
            };

            this.context.Users.Add(user);
            this.context.SaveChanges();

            return user.Id;
        }

        public IEnumerable<string> GetUsernames()
        {
            var usernames = this.context
                .Users
                .Select(x => x.Username)
                .ToList();

            return usernames;
        }

        public User GetUserOrNull(string username, string password)
        {
            var passwordHash = this.HashPassword(password);

            var user = this.context
                .Users
                .FirstOrDefault(x => x.Username == username && x.Password == passwordHash);

            return user;
        }

        private string HashPassword(string password)
        {
            using (SHA256 sha256Hash = SHA256.Create())
            {
                return Encoding.UTF8.GetString(sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(password)));
            }
        }
    }
}