using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JsonParser
{
    class JSONParser
    {

    }
    public interface IJSONSerializable
    {
        string ToJSON();
        void FromJSON(string json);
    }
    public class UserSerializable : IJSONSerializable
    {
        public string username { get; set; }
        public string email { get; set; }
        public string userId { get; set; }
        public DateTime dateOfBirth { get; set; }
        public string ToJSON()
        {
            return System.Text.Json.JsonSerializer.Serialize(this);
        }
        public void FromJSON(string json)
        {
            var obj = System.Text.Json.JsonSerializer.Deserialize<UserSerializable>(json);
            if (obj != null)
            {
                this.username = obj.username;
                this.email = obj.email;
                this.dateOfBirth = obj.dateOfBirth;
                this.userId = obj.userId;
            }
        }
    }
    public class MessageSerializable : IJSONSerializable
    {
        public string SenderId { get; set; }
        public string ReceiverId { get; set; }
        public string Content { get; set; }
        public DateTime Timestamp { get; set; }
        public string ToJSON()
        {
            return System.Text.Json.JsonSerializer.Serialize(this);
        }
        public void FromJSON(string json)
        {
            var obj = System.Text.Json.JsonSerializer.Deserialize<MessageSerializable>(json);
            if (obj != null)
            {
                this.SenderId = obj.SenderId;
                this.ReceiverId = obj.ReceiverId;
                this.Content = obj.Content;
                this.Timestamp = obj.Timestamp;
            }
        }
    }
    class LoginSendModel
    {
        public string type { get; set; } = "login";
        public string username { get; set; }
        public string password { get; set; }
    }
    class MessageSendModel
    {
        public string type { get; set; } = "message";
        public string senderId { get; set; }
        public string receiverId { get; set; }
        public string content { get; set; }
    }
    class LoginResponseModel
    {
        public string type { get; set; } = "loginResponse";
        public string status { get; set; }
        public string message { get; set; }
        public UserSerializable user { get; set; }
    }
}
