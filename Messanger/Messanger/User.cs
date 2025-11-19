using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserNamespace
{
    class User
    {
        public string Username { get; set; }
        public string UserId { get; set; }
        string Password { get; set; }
        public string Email { get; set; }
        public DateTime DateOfBirth { get; set; }
        public User(string username, string email, DateTime dateOfBirth, string userId, string password)
        {
            Username = username;
            Email = email;
            DateOfBirth = dateOfBirth;
            UserId = userId;
            Password = password;
        }
    }
}
