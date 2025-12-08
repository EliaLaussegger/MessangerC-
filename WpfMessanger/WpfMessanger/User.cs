using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Helper;
using DataBank;
using Delegates;
using System.IO;
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
        public void SetLoggedIn(bool status)
        {
            loggedIn = status;
        }

    }
    public static class UserFunctions
    {
        public static void CreateUserDb(User user)
        {
            string userDbPath = Path.Combine(HelperClass.CreateFolder("ClientsDB\\" + user.userId), user.userId + "userdata.db");
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
            using var insertCmd = new SqliteCommand(
                "INSERT INTO Contacts (ContactName, ContactEmail, ContactDateOfBirth,ContactId) VALUES (@n, @e, @d,@i)",
                userConnection
            );
            insertCmd.Parameters.AddWithValue("@n", user.username);
            insertCmd.Parameters.AddWithValue("@e", user.email);
            insertCmd.Parameters.AddWithValue("@d", user.dateOfBirth);
            insertCmd.Parameters.AddWithValue("@i", user.userId);
            insertCmd.ExecuteNonQuery();
            //Console.WriteLine($"User-DB für User {userId} erstellt unter: {userDbPath}");
        }
        public static User CreateUser()
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

            CentralUserDB.Instance.RegisterUser(newUser);
            return newUser;
        }
        public static User LoginUser(string username, string password)
        {
            if (CentralUserDB.Instance.Login(username, password))
            {
                Console.WriteLine("Login successful");
                User user = new User(username, "", DateTime.Now, password);
                user.userId = DataBaseHelper.GetUserId(username);
                return user;
            }
            else
            {
                Console.WriteLine("Login failed");
                return null;
            }
        }
    }
}
