using Panda.Data;
using Panda.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace Panda.Services
{
    public class UserService : IUserService
    {
        private readonly PandaDbContext PandaDbContext;

        public UserService(PandaDbContext context)
        {
            this.PandaDbContext = context;
        }

        public User GetUserOrNull(string username, string password)
        {
            string hashedPassword = this.HashPassword(password);

            return this.PandaDbContext
                .Users
                .FirstOrDefault(u => u.Username == username
                                  && u.Password == hashedPassword);
        }

        public string CreateUser(string username, string email, string password)
        {
            var user = new User()
            {
                Username = username,
                Email = email,
                Password = this.HashPassword(password)
            };

            this.PandaDbContext.Add(user);
            this.PandaDbContext.SaveChanges();

            return user.Id;

        }

        private string HashPassword(string password)
        {
            using (SHA256 sha256Hash = SHA256.Create())
            {
                return Encoding.UTF8.GetString(sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(password)));
            }
        }

        public string GetUserIdByUsername(string username)
        {
            return this.PandaDbContext.Users.SingleOrDefault(u => u.Username == username).Id;
        }

        public ICollection<string> GetUsernames()
        {
            return this.PandaDbContext.Users.Select(u => u.Username).ToList();
        }
    }
}
