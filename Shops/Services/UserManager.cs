using System.Collections.Generic;
using Shops.Models;

namespace Shops.Services
{
    public class UserManager
    {
        private List<User> _users;

        public UserManager()
        {
            _users = new List<User>();
        }

        public void RegisterUser(User user)
        {
            _users.Add(user);
        }

        public IReadOnlyList<User> GetAllUsers() => _users.AsReadOnly();
    }
}