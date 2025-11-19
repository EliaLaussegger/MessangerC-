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
            string folderPath = "C:\\Users\\elial\\source\\repos\\Messanger\\CentralUserDB";
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
            CREATE TABLE IF NOT EXISTS User(
                    UserId INTEGER PRIMARY KEY AUTOINCREMENT,
                    Username TEXT NOT NULL UNIQUE,
                    PasswordHash TEXT NOT NULL,
                    PasswordSalt TEXT NOT NULL
            );", connection);
            cmd.ExecuteNonQuery();
        }
        public void RegisterUser(string username, string email, DateTime dateOfBirth, string userId, string password)
        {
            using var insertCmd = new SqliteCommand("INSERT INTO User (User, UserID, Password, Salt) VALUES (@u,@i,@p,@s)", connection);
            var (hash, salt) = HashPassword(password);
            insertCmd.Parameters.AddWithValue("@u", username);
            insertCmd.Parameters.AddWithValue("@i", userId);
            insertCmd.Parameters.AddWithValue("@p", hash);
            insertCmd.Parameters.AddWithValue("@s", salt);
            //insertCmd.Parameters.AddWithValue("@t", DateTime.Now.ToString("o"));
            insertCmd.ExecuteNonQuery();
            using var selectCmd = new SqliteCommand("SELECT * FROM Messages", connection);
            using var reader = selectCmd.ExecuteReader();
            while (reader.Read())
            {
                Console.WriteLine($"{reader.GetInt32(0)}: {reader.GetString(1)} - {reader.GetString(2)}");
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
