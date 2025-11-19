using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Helper;
namespace UserNamespace
{
    public class User
    {

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
    }
}
