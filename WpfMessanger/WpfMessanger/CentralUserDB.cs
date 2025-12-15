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
using MessagesNameSpace;
using System.IO;
namespace DataBank
{
    public interface IDatabase
    {
        public SqliteConnection connection { get; }
    }
    public interface IRepository<T> where T : IDatabase
    {
         List<T> repositoryList {get;}
    }
    public class CentralUserDB : IDatabase
    {
        private static readonly Lazy<CentralUserDB> _instance =
      new Lazy<CentralUserDB>(() => new CentralUserDB());

        // Zugriffspunkt
        public static CentralUserDB Instance => _instance.Value;

        // Verhindert, dass man new CentralUserDB() aufrufen kann
        private CentralUserDB()
        {
            string dbPath = Path.Combine(HelperClass.CreateFolder("CentralUserDB"), "clientdata.db");
            Console.WriteLine("Datenbankpfad: " + dbPath);

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

        // Dein Property
        public SqliteConnection connection { get; private set; }
        //public SqliteConnection connection { get; private set; }
        //public CentralUserDB()
        //{


        //    string dbPath = Path.Combine(CreateFolder("CentralUserDB"), "clientdata.db");
        //    Console.WriteLine("Datenbankpfad: " + dbPath);
        //    connection = new SqliteConnection($"Data Source={dbPath}");
        //    connection.Open();

        //    using var cmd = new SqliteCommand(@"
        //    CREATE TABLE IF NOT EXISTS UserData(
        //    UserId INTEGER PRIMARY KEY AUTOINCREMENT,
        //    UserName TEXT NOT NULL UNIQUE,
        //    PasswordHash TEXT NOT NULL,
        //    PasswordSalt TEXT NOT NULL
        //    );", connection);
        //    cmd.ExecuteNonQuery();
        //}
        public void RegisterUser(User user)
        {
            if (DataBaseHelper.UserExists(user.username))
            {
                Console.WriteLine("Benutzer existiert schon");
                return;
            }
            using var insertCmd = new SqliteCommand(
                "INSERT INTO UserData (UserName, PasswordHash, PasswordSalt) VALUES (@u, @p, @s)",
                connection
            );
            var (hash, salt) = DataBaseHelper.HashPassword(user.password);
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
            string userSalt = DataBaseHelper.GetUserSalt(username);
            if (DataBaseHelper.VerifyPassword(password, DataBaseHelper.GetPasswordHash(username), userSalt))
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

    }
    public static class DataBaseHelper
    {
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
        public static string GetPasswordHash(string username)
        {
            using var hashCmd = new SqliteCommand(
                "SELECT PasswordHash FROM UserData WHERE UserName = @u",
                CentralUserDB.Instance.connection
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
        public static bool UserExists(string username)
        {
            using var checkCmd = new SqliteCommand(
                "SELECT COUNT(*) FROM UserData WHERE UserName = @u",
                 CentralUserDB.Instance.connection
            );
            checkCmd.Parameters.AddWithValue("@u", username);

            long count = (long)checkCmd.ExecuteScalar();
            return count > 0;
        }
        public static string GetUserName(string username)
        {
            using var idCmd = new SqliteCommand(
                "SELECT UserId FROM UserData WHERE UserName = @u",
                 CentralUserDB.Instance.connection
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
        public static SqliteConnection GetUserConnection(string username)
        {
            if (string.IsNullOrEmpty(username))
                throw new InvalidOperationException($"User not found: {username}");

            string userDir = HelperClass.FindPathUserId(username);
            if (string.IsNullOrEmpty(userDir))
                throw new FileNotFoundException($"User folder not found for userId={username}");

            string dbPath = Path.Combine(userDir, "messages", $"{username}messagedata.db");
            string? dir = Path.GetDirectoryName(dbPath);
            if (string.IsNullOrEmpty(dir))
                throw new InvalidOperationException($"Cannot determine directory for dbPath={dbPath}");
            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);

            var csb = new SqliteConnectionStringBuilder
            {
                DataSource = dbPath,
                Mode = SqliteOpenMode.ReadWriteCreate,
                Cache = SqliteCacheMode.Shared
            };
            var userConnection = new SqliteConnection(csb.ToString());
            userConnection.Open();
            return userConnection;
        }
        public static SqliteConnection GetUserConnectionById(string userId)
        {
            string dbPath = HelperClass.FindPathUserId(userId);
            var userConnection = new SqliteConnection($"Data Source={dbPath}");
            userConnection.Open();
            return userConnection;
        }
        public static List<Message> GetMessageObject(SqliteConnection connection)
        {
    var messages = new List<Message>();

    // Sicherstellen, dass die Tabelle existiert, bevor wir SELECT ausführen
    using (var createCmd = new SqliteCommand(@"
        CREATE TABLE IF NOT EXISTS Messages(
            MessageId INTEGER PRIMARY KEY AUTOINCREMENT,
            SenderId TEXT NOT NULL,
            ReceiverId TEXT NOT NULL,
            Content TEXT NOT NULL,
            Timestamp TEXT NOT NULL
        );", connection))
    {
        createCmd.ExecuteNonQuery();
    }

    using var cmd = new SqliteCommand(
        "SELECT SenderId, ReceiverId, Content, Timestamp FROM Messages ORDER BY Timestamp",
        connection
    );

    using var reader = cmd.ExecuteReader();
    while (reader.Read())
    {
        Message message = new Message
        (
            reader.GetString(0),
            reader.GetString(1),
            reader.GetString(2)
        );
        message.timestamp = DateTime.Parse(reader.GetString(3));
        messages.Add(message);
    }

    return messages;
        }
        public static string GetUserId(string username)
        {
            using var idCmd = new SqliteCommand(
                "SELECT UserId FROM UserData WHERE UserName = @u",
                 CentralUserDB.Instance.connection
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
        public static string GetUserSalt(string username)
        {
            using var saltCmd = new SqliteCommand(
                "SELECT PasswordSalt FROM UserData WHERE UserName = @u",
                 CentralUserDB.Instance.connection
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
    }
    public class MessageRepository : IRepository<MessageDataBase>
    {
        public SqliteConnection connection { get; set; }
        public List<MessageDataBase> repositoryList { get; private set; } = new List<MessageDataBase>();
        public MessageRepository()
        {
            //string dbPath = Path.Combine(HelperClass.CreateFolder("ClientsDB\\" + userId), userId + "messagedata.db");
        }
        public void RegisterNewMessage(Message message)
        {
            MessageDataBase messageDataBase = new MessageDataBase(message);
            repositoryList.Add(messageDataBase);
        }
        public List<MessageDataBase> GetAllMessages()
        {
            return repositoryList;
        }
        public void ClearMessages()
        {
            repositoryList.Clear();
        }
        public void CloseConnection()
        {
            connection.Close();
        }
        public MessageDataBase FindMessage(string content)
        {
            return repositoryList.Find(m => m.content == content);
        }
    }
        
    public class MessageDataBase : IDatabase
    {
        public SqliteConnection connection { get; set; }
        public string senderId { get; set; }
        public string receiverId { get; set; }
        public string content { get; set; }
        public DateTime Timestamp { get; set; }
        //public MessageDataBase(string senderId, string receiverId, string content)
        //{
        //    SenderId = senderId;
        //    ReceiverId = receiverId;
        //    Content = content;
        //    Timestamp = DateTime.UtcNow;
        //}
        public MessageDataBase(Message message)
        {
            senderId = message.senderId;
            receiverId = message.receiverId;
            content = message.content;
            Timestamp = DateTime.UtcNow;
            string folder = HelperClass.CreateFolder($"ClientsDB\\{message.senderId}\\messages");
            string dbPath = Path.Combine(folder, $"{message.senderId}messagedata.db");
            connection = new SqliteConnection($"Data Source={dbPath}");
            connection.Open();
            using var cmd = new SqliteCommand(@"
            CREATE TABLE IF NOT EXISTS Messages(
            MessageId INTEGER PRIMARY KEY AUTOINCREMENT,
            SenderId TEXT NOT NULL,
            ReceiverId TEXT NOT NULL,
            Content TEXT NOT NULL,
            Timestamp TEXT NOT NULL
            );", connection);
            cmd.ExecuteNonQuery();
            AddMessageToDB();
        }
        public void AddMessageToDB()
        {
            using var insertCmd = new SqliteCommand(
                "INSERT INTO Messages (SenderId, ReceiverId, Content, Timestamp) VALUES (@s, @r, @c, @t)",
                connection
            );
            insertCmd.Parameters.AddWithValue("@s", senderId);
            insertCmd.Parameters.AddWithValue("@r", receiverId);
            insertCmd.Parameters.AddWithValue("@c", content);
            insertCmd.Parameters.AddWithValue("@t", Timestamp.ToString("o"));
            insertCmd.ExecuteNonQuery();
        }
    }
}
