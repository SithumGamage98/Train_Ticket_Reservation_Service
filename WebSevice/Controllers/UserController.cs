/* 
 * File: UsersController.cs
 * Description: This file contains the UsersController class which handles users operations.
 */
using System.Linq;
using System.Web.Http;
using WebSevice.Models;
using MongoDB.Bson;
using MongoDB.Driver;

namespace WebSevice.Controllers
{
    [RoutePrefix("api/users")]
    public class UsersController : ApiController
    {
        private readonly IMongoCollection<User> _usersCollection;

        public UsersController()
        {
            var connectionString = "mongodb+srv://teamparadox199:TeamPara123@cluster0.x2dsxn6.mongodb.net";
            var collectionName = "Users";
            var client = new MongoClient(connectionString);
            var database = client.GetDatabase("EAD");
            _usersCollection = database.GetCollection<User>(collectionName);
        }

        /*
 * Handles user signup.
 */
        [HttpPost]
        [Route("signup")]
        public IHttpActionResult signup(User user)
        {
            var existingUser = _usersCollection.Find(u => u.NIC == user.NIC).FirstOrDefault();

            if (existingUser != null)
            {
                return BadRequest("User with this NIC already exists.");
            }

            if (user.Password != user.RePassword)
            {
                return BadRequest("Passwords do not match.");
            }

            var newUser = new User
            {
                ID = ObjectId.GenerateNewId().ToString(),
                NIC = user.NIC,
                UName = user.UName,
                FName = user.FName,
                LName = user.LName,
                Email = user.Email,
                Password = PasswordHasher.HashPassword(user.Password),
                RePassword = PasswordHasher.HashPassword(user.RePassword),
                CNumber = user.CNumber,
                UserType = user.UserType,
                UserStatus = "Active"
            };
            _usersCollection.InsertOne(newUser);
            return Ok(newUser);
        }

        /*
 * Handles user signin.
 */
        [HttpPost]
        [Route("signin")]
        public IHttpActionResult signin(User user)
        {
            var existingUser = _usersCollection.Find(u => u.NIC == user.NIC).FirstOrDefault();

            if (existingUser == null)
            {
                return NotFound();
            }

            if (existingUser.UserStatus == "Inactive")
            {
                return BadRequest("User is inactive and cannot sign in.");
            }

            if (PasswordHasher.VerifyPassword(existingUser.Password, user.Password))
            {
                return Ok(existingUser);
            }
            return BadRequest("Incorrect password.");
        }

        /*
 * Creates a new traveller user.
 */
        [HttpPost]
        [Route("addTraveluser")]
        public IHttpActionResult CreateTraveller(User user)
        {
            var newUser = new User
            {
                ID = ObjectId.GenerateNewId().ToString(),
                NIC = user.NIC,
                FName = user.FName,
                LName = user.LName,
                Email = user.Email,
                UName = user.UName,
                Password = user.Password,
                RePassword = user.RePassword,
                CNumber = user.CNumber,
                UserType = "Traveller",
                UserStatus = "Active"
            };
            _usersCollection.InsertOne(newUser);
            return Ok(newUser);
        }

        /*
 * Retrieves a user by their ID.
 */
        [HttpGet]
        [Route("get/{id}")]
        public IHttpActionResult GetUserByUserID(string id)
        {
            var user = _usersCollection.Find(u => u.ID == id).FirstOrDefault();

            if (user == null)
            {
                return NotFound();
            }
            return Ok(user);
        }

        /*
 * Updates a user's information by their ID.
 */
        [HttpPut]
        [Route("updatebyid/{id}")]
        public IHttpActionResult updatetravelagentbyId(string id, User updatedUser)
        {
            var filter = Builders<User>.Filter.Eq(u => u.ID, id);
            var update = Builders<User>.Update
                .Set(u => u.UName, updatedUser.UName)
                .Set(u => u.FName, updatedUser.FName)
                .Set(u => u.LName, updatedUser.LName)
                .Set(u => u.Email, updatedUser.Email)
                .Set(u => u.CNumber, updatedUser.CNumber);

            var result = _usersCollection.UpdateOne(filter, update);

            if (result.ModifiedCount == 0)
            {
                return NotFound();
            }
            return Ok();
        }

        /*
 * Retrieves all travel agents.
 */
        [HttpGet]
        [Route("getall")]
        public IHttpActionResult GetAllTravelAgents()
        {
            var filter = Builders<User>.Filter.Eq(u => u.UserType, "Traveller");
            var travelAgents = _usersCollection.Find(filter).ToList();
            return Ok(travelAgents);
        }

        /*
 * Updates a user's status by their ID.
 */
        [HttpPut]
        [Route("updatestatusbyid/{id}")]
        public IHttpActionResult updatetravelagentStatusById(string id, [FromBody] UserStatusUpdateByIdModel updateModel)
        {
            var filter = Builders<User>.Filter.And(
                Builders<User>.Filter.Eq(u => u.ID, id)
            );
            var update = Builders<User>.Update.Set(u => u.UserStatus, updateModel.UserStatus);

            var result = _usersCollection.UpdateOne(filter, update);

            if (result.ModifiedCount == 0)
            {
                return NotFound();
            }
            return Ok();
        }

        public class UserStatusUpdateByIdModel
        {
            public string UserStatus { get; set; }
        }

        /*
 * Deletes a user by their ID.
 */
        [HttpDelete]
        [Route("delete/{id}")]
        public IHttpActionResult DeleteUser(string id)
        {
            var result = _usersCollection.DeleteOne(u => u.ID == id);

            if (result.DeletedCount == 0)
            {
                return NotFound();
            }
            return Ok();
        }
    }
}