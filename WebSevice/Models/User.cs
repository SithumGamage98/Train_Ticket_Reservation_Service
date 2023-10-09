/* 
 * File: User.cs
 * Description: This file contains the User class which is used for modeling user data.
 */

using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace WebSevice.Models
{
    public class User
    {
        [BsonId]
        [BsonRepresentation(BsonType.String)]
        public string ID { get; set; }
        public string NIC { get; set; }
        public string UName { get; set; }
        public string FName { get; set; }
        public string LName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string RePassword { get; set; }
        public string CNumber { get; set; }
        public string UserStatus { get; set; }
        public string UserType { get; set; }
    }
}