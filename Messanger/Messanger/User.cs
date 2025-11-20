using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Helper;
using DataBank;
namespace UserNamespace
{
    public class User
    {
        public bool loggedIn { get; private set; }
        public string username { get; set; }
        public string userId { get; set; }
        public string password { get; private set; }
        public string email { get; set; }
        public DateTime dateOfBirth { get; set; }
        public User(string username, string email, DateTime dateOfBirth,  string password)
        {
            this.username = username;
            this.email = email;
            this.dateOfBirth = dateOfBirth;
            this.password = password;
        }

    }
    public class UserFunctions
    {
        HelperClass helper = new HelperClass();
        public void CreateUserDb(User user)
        {
            string userDbPath = Path.Combine(helper.CreateFolder("ClientsDB\\" + user.userId), user.userId + "userdata.db");
            using var userConnection = new SqliteConnection($"Data Source={userDbPath}");
            userConnection.Open();
            using var cmd = new SqliteCommand(@"
            CREATE TABLE IF NOT EXISTS Contacts(
            ContactId INTEGER PRIMARY KEY AUTOINCREMENT,
            ContactName TEXT NOT NULL,
            ContactEmail TEXT,
            ContactDateOfBirth TEXT
            );", userConnection);
            cmd.ExecuteNonQuery();
            //Console.WriteLine($"User-DB für User {userId} erstellt unter: {userDbPath}");
        }
        public User CreateUser()
        {
            Console.WriteLine("Enter name");
            string nameInput = Console.ReadLine();
            Console.WriteLine("Enter email");
            string emailInput = Console.ReadLine();
            Console.WriteLine("Enter date of birth (yyyy-MM-dd)");
            string dobInput = Console.ReadLine();
            DateTime dob = DateTime.Parse(dobInput);
            Console.WriteLine("Enter password");
            string passwordInput = Console.ReadLine();
            User newUser = new User(nameInput, emailInput, dob, passwordInput);
            CentralUserDB centralUserDB = new CentralUserDB();

            centralUserDB.RegisterUser(newUser);
            return newUser;
        }
    }
}
