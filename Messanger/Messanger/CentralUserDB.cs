using Helper;
using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using UserNamespace;
using static Helper.HelperClass;
namespace DataBank
{
    public class CentralUserDB
    {
        private SqliteConnection connection;
        public CentralUserDB()
        {

            
            string dbPath = Path.Combine(CreateFolder("CentralUserDB"), "clientdata.db");
            connection = new SqliteConnection($"Data Source={dbPath}");
            connection.Open();

            using var cmd = new SqliteCommand(@"
            CREATE TABLE IF NOT EXISTS UserData(
            UserId INTEGER PRIMARY KEY AUTOINCREMENT,
            UserName TEXT NOT NULL UNIQUE,
            PasswordHash TEXT NOT NULL,
            PasswordSalt TEXT NOT NULL
            );", connection);
            cmd.ExecuteNonQuery();
        }
        public void RegisterUser(User user)
        {
            if (UserExists(user.username))
            {
                Console.WriteLine("Benutzer existiert schon");
                return;
            }
            using var insertCmd = new SqliteCommand(
                "INSERT INTO UserData (UserName, PasswordHash, PasswordSalt) VALUES (@u, @p, @s)",
                connection
            );
            var (hash, salt) = HashPassword(user.password);
            insertCmd.Parameters.AddWithValue("@u", user.username);
            insertCmd.Parameters.AddWithValue("@p", hash);
            insertCmd.Parameters.AddWithValue("@s", salt);
            insertCmd.ExecuteNonQuery();
            using var idCmd = new SqliteCommand("SELECT last_insert_rowid();", connection);
            long userId = (long)idCmd.ExecuteScalar();
            user.userId = userId.ToString();
            UserFunctions.CreateUserDb(user);
            TestRegistration(user.username);
        }
        public bool Login(string username, string password)
        {
            string userSalt = GetUserSalt(username);
            if (VerifyPassword(password, GetPasswordHash(username), userSalt))
            {
                return true;
                Console.WriteLine("Login erfolgreich!");
            }
            else
            {
                return false;
                Console.WriteLine("Login fehlgeschlagen!");
            }
        }
        public string GetPasswordHash(string username)
        {
            using var hashCmd = new SqliteCommand(
                "SELECT PasswordHash FROM UserData WHERE UserName = @u",
                connection
            );
            hashCmd.Parameters.AddWithValue("@u", username);
            using var reader = hashCmd.ExecuteReader();
            if (reader.Read())
            {
                return reader.GetString(0);
            }
            else
            {
                return null;
            }
        }
        public void TestRegistration(string username)
        {
            using var testCmd = new SqliteCommand(
                "SELECT UserId, UserName FROM UserData WHERE UserName = @u",
                connection
            );

            testCmd.Parameters.AddWithValue("@u", username);

            using var reader = testCmd.ExecuteReader();

            if (reader.Read())
            {
                Console.WriteLine("Benutzer wurde erfolgreich registriert:");
                Console.WriteLine("UserId: " + reader.GetInt32(0));
                Console.WriteLine("Username: " + reader.GetString(1));
            }
            else
            {
                Console.WriteLine("Benutzer wurde NICHT in der Datenbank gefunden!");
            }
        }
        public bool UserExists(string username)
        {
            using var checkCmd = new SqliteCommand(
                "SELECT COUNT(*) FROM UserData WHERE UserName = @u",
                connection
            );
            checkCmd.Parameters.AddWithValue("@u", username);

            long count = (long)checkCmd.ExecuteScalar();
            return count > 0;
        }
        public string GetUserId(string username)
        {
            using var idCmd = new SqliteCommand(
                "SELECT UserId FROM UserData WHERE UserName = @u",
                connection
            );
            idCmd.Parameters.AddWithValue("@u", username);
            using var reader = idCmd.ExecuteReader();
            if (reader.Read())
            {
                return reader.GetInt32(0).ToString();
            }
            else
            {
                return null;
            }
        }
        public string GetUserSalt(string username)
        {
            using var saltCmd = new SqliteCommand(
                "SELECT PasswordSalt FROM UserData WHERE UserName = @u",
                connection
            );
            saltCmd.Parameters.AddWithValue("@u", username);
            using var reader = saltCmd.ExecuteReader();
            if (reader.Read())
            {
                return reader.GetString(0);
            }
            else
            {
                return null;
            }
        }   
        public static (string hash, string salt) HashPassword(string password)
        {
            byte[] saltBytes = RandomNumberGenerator.GetBytes(16);
            byte[] hashBytes = new Rfc2898DeriveBytes(password, saltBytes, 100_000, HashAlgorithmName.SHA256).GetBytes(32);

            return (Convert.ToBase64String(hashBytes), Convert.ToBase64String(saltBytes));
        }
        public static bool VerifyPassword(string password, string storedHash, string storedSalt)
        {
            byte[] saltBytes = Convert.FromBase64String(storedSalt);
            byte[] hashBytes = new Rfc2898DeriveBytes(password, saltBytes, 100_000, HashAlgorithmName.SHA256).GetBytes(32);

            return Convert.ToBase64String(hashBytes) == storedHash;
        }
    }
}
