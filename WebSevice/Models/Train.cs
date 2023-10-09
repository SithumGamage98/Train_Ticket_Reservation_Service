/* 
 * File: Train.cs
 * Description: This file contains the Train class which is used for modeling train data.
 */

using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace WebSevice.Models
{
    public class Train
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string ID { get; set; }
        public string UserId { get; set; }
        public string TrainID { get; set; }
        public string TrainName { get; set; }
        public string TrainDriver { get; set; }
        public string DeDateTime { get; set; }
        public string ArDateTime { get; set; }
        public string TrainStatus { get; set; }
    }
}