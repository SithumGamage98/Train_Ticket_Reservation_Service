/*
 * File: TrainsController.cs
 * Description: This file contains the TrainsController class which handles train operations.
 */

using System.Web.Http;
using MongoDB.Bson;
using MongoDB.Driver;
using WebSevice.Models;

namespace WebSevice.Controllers
{
    [RoutePrefix("api/trains")]
    public class TrainsController : ApiController
    {
        private readonly IMongoCollection<Train> _trainsCollection;

        public TrainsController()
        {
            var connectionString = "mongodb+srv://teamparadox199:TeamPara123@cluster0.x2dsxn6.mongodb.net";
            var collectionName = "Trains";
            var client = new MongoClient(connectionString);
            var database = client.GetDatabase("EAD");
            _trainsCollection = database.GetCollection<Train>(collectionName);
        }

        /*
 * Handles creation of a new train.
 */
        [HttpPost]
        [Route("create")]
        public IHttpActionResult CreateTrain(Train train)
        {
            train.ID = ObjectId.GenerateNewId().ToString();
            train.TrainStatus = "Active";
            var userId = train.UserId;
            _trainsCollection.InsertOne(train);
            return Ok(train);
        }

        /*
 * Retrieves all trains.
 */
        [HttpGet]
        [Route("getall")]
        public IHttpActionResult GetAllTrains()
        {
            var trains = _trainsCollection.Find(new BsonDocument()).ToList();
            return Ok(trains);
        }

        /*
 * Retrieves all active trains.
 */
        [HttpGet]
        [Route("getallActive")]
        public IHttpActionResult GetActiveTrains()
        {
            var activeTrains = _trainsCollection.Find(t => t.TrainStatus == "Active").ToList();
            return Ok(activeTrains);
        }

        /*
 * Retrieves a train by its ID.
 */
        [HttpGet]
        [Route("get/{id}")]
        public IHttpActionResult GetTrainById(string id)
        {
            var train = _trainsCollection.Find(t => t.ID == id).FirstOrDefault();
            if (train == null)
            {
                return NotFound();
            }
            return Ok(train);
        }

        /*
 * Updates a train's information by its ID.
 */
        [HttpPut]
        [Route("update/{id}")]
        public IHttpActionResult updatetrain(string id, Train updatedTrain)
        {
            var filter = Builders<Train>.Filter.Eq(t => t.ID, id);
            var update = Builders<Train>.Update
                .Set(t => t.TrainID, updatedTrain.TrainID)
                .Set(t => t.TrainName, updatedTrain.TrainName)
                .Set(t => t.TrainDriver, updatedTrain.TrainDriver)
                .Set(t => t.DeDateTime, updatedTrain.DeDateTime)
                .Set(t => t.ArDateTime, updatedTrain.ArDateTime)
                .Set(t => t.TrainStatus, updatedTrain.TrainStatus);

            var result = _trainsCollection.UpdateOne(filter, update);

            if (result.ModifiedCount == 0)
            {
                return NotFound();
            }

            return Ok();
        }

        /*
 * Deletes a train by its ID.
 */
        [HttpDelete]
        [Route("delete/{id}")]
        public IHttpActionResult DeleteTrain(string id)
        {
            var result = _trainsCollection.DeleteOne(t => t.ID == id);

            if (result.DeletedCount == 0)
            {
                return NotFound();
            }

            return Ok();
        }
    }
}