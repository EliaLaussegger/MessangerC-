using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using UserNamespace;
namespace DataBank
{
    public class CentralUserDB
    {
        private SqliteConnection connection;
        public CentralUserDB()
        {
            string folderPath = "C:\\Users\\elial\\source\\repos\\MessangerC-\\Messanger\\CentralUserDB";
            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
                Console.WriteLine("Ordner erstellt: " + folderPath);
            }
            else
            {
                Console.WriteLine("Ordner existiert bereits: " + folderPath);
            }
            string dbPath = Path.Combine(folderPath, "clientdata.db");
            connection = new SqliteConnection($"Data Source={dbPath}");
            connection.Open();

            using var cmd = new SqliteCommand(@"
            CREATE TABLE IF NOT EXISTS UserData(
            UserId INTEGER PRIMARY KEY AUTOINCREMENT,
            UserAccounts TEXT NOT NULL UNIQUE,
            PasswordHash TEXT NOT NULL,
            PasswordSalt TEXT NOT NULL
            );", connection);
            cmd.ExecuteNonQuery();
        }
        public void RegisterUser(string username, string email, DateTime dateOfBirth, string userId, string password)
        {
            if (UserExists(username))
            {
                Console.WriteLine("Benutzer existiert schon");
                return;
            }
            using var insertCmd = new SqliteCommand(
                "INSERT INTO UserData (UserAccounts, PasswordHash, PasswordSalt) VALUES (@u, @p, @s)",
                connection
            );
            var (hash, salt) = HashPassword(password);
            insertCmd.Parameters.AddWithValue("@u", username);
            insertCmd.Parameters.AddWithValue("@p", hash);
            insertCmd.Parameters.AddWithValue("@s", salt);
            insertCmd.ExecuteNonQuery();
            TestRegistration(username);

        }
        public void TestRegistration(string username)
        {
            using var testCmd = new SqliteCommand(
                "SELECT UserId, UserAccounts FROM UserData WHERE UserAccounts = @u",
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
                "SELECT COUNT(*) FROM UserData WHERE UserAccounts = @u",
                connection
            );
            checkCmd.Parameters.AddWithValue("@u", username);

            long count = (long)checkCmd.ExecuteScalar();
            return count > 0;
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
