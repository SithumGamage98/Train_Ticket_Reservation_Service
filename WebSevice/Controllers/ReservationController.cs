/*
 * File: BookingsController.cs
 * Description: This file contains the ReservationController class which handles ticket  operations.
 */

using MongoDB.Bson;
using WebSevice.Models;
using System;
using System.Linq;
using System.Web.Http;
using MongoDB.Driver;

namespace WebSevice.Controllers
{
    [RoutePrefix("api/reservations")]
    public class bookingscontroller : ApiController
    {
        private readonly IMongoCollection<Reservation> _bookingsCollection;

        public bookingscontroller()
        {
            var connectionString = "mongodb+srv://teamparadox199:TeamPara123@cluster0.x2dsxn6.mongodb.net";
            var collectionName = "reservations";
            var client = new MongoClient(connectionString);
            var database = client.GetDatabase("EAD");
            _bookingsCollection = database.GetCollection<Reservation>(collectionName);
        }

        // Create a new booking reservation.
        [HttpPost]
        [Route("create")]
        public IHttpActionResult CreateBooking(Reservation reservation)
        {
            if (reservation.NumPassengers > 4)
            {
                return BadRequest("Maximum 4 passengers allowed per reservation.");
            }
            reservation.BookingDate = DateTime.Now.Date;

            if ((reservation.ReservationDate - reservation.BookingDate).Days > 30)
            {
                return BadRequest("Reservation date must be within 30 days from the reservation date.");
            }
            reservation.ID = ObjectId.GenerateNewId().ToString();
            reservation.BookingStatus = "Active";

            var userId = reservation.UserId;

            _bookingsCollection.InsertOne(reservation);

            return Ok(reservation);
        }

        // Update an existing booking reservation by ID.
        [HttpPut]
        [Route("update/{id}")]
        public IHttpActionResult UpdateBooking(string id, Reservation updatedBooking)
        {
            var filter = Builders<Reservation>.Filter.Eq(b => b.ID, id);
            var existingBooking = _bookingsCollection.Find(filter).FirstOrDefault();

            if (existingBooking == null)
            {
                return NotFound();
            }
            var today = DateTime.Now;
            var reservationDate = existingBooking.ReservationDate;

            if (reservationDate > today && reservationDate.Subtract(today).Days >= 5)
            {
                existingBooking.TravelerName = updatedBooking.TravelerName;
                existingBooking.NIC = updatedBooking.NIC;
                existingBooking.TrainID = updatedBooking.TrainID;
                existingBooking.ReservationDate = updatedBooking.ReservationDate;
                existingBooking.BookingStatus = "Active";
                existingBooking.DepartureLocation = updatedBooking.DepartureLocation;
                existingBooking.DestinationLocation = updatedBooking.DestinationLocation;
                existingBooking.NumPassengers = updatedBooking.NumPassengers;
                existingBooking.Age = updatedBooking.Age;
                existingBooking.TicketClass = updatedBooking.TicketClass;
                existingBooking.SeatSelection = updatedBooking.SeatSelection;
                existingBooking.Email = updatedBooking.Email;
                existingBooking.Phone = updatedBooking.Phone;

                existingBooking.FormattedReservationDate = updatedBooking.ReservationDate.ToString("yyyy-MM-dd");

                var result = _bookingsCollection.ReplaceOne(filter, existingBooking);

                if (result.ModifiedCount == 0)
                {
                    return NotFound();
                }

                return Ok();
            }

            return BadRequest("Reservation can only be updated at least 5 days before the reservation date.");
        }

        // Cancel an existing booking reservation by ID.
        [HttpPut]
        [Route("cancel/{id}")]
        public IHttpActionResult CancelBooking(string id)
        {
            var filter = Builders<Reservation>.Filter.Eq(b => b.ID, id);
            var existingBooking = _bookingsCollection.Find(filter).FirstOrDefault();

            if (existingBooking == null)
            {
                return NotFound();
            }

            var today = DateTime.Now;
            var reservationDate = existingBooking.ReservationDate;

            if (reservationDate > today && reservationDate.Subtract(today).Days >= 5)
            {
                var update = Builders<Reservation>.Update
                    .Set(b => b.BookingStatus, "Cancelled");

                var result = _bookingsCollection.UpdateOne(filter, update);

                if (result.ModifiedCount == 0)
                {
                    return NotFound();
                }

                var deleteResult = _bookingsCollection.DeleteOne(filter);

                if (deleteResult.DeletedCount == 0)
                {
                    return NotFound();
                }
                return Ok();
            }
            return BadRequest("Reservation can only be canceled at least 5 days before the reservation date.");
        }

        // Get an existing booking reservation by ID.
        [HttpGet]
        [Route("get/{id}")]
        public IHttpActionResult GetBookingById(string id)
        {
            var reservation = _bookingsCollection.Find(b => b.ID == id).FirstOrDefault();

            if (reservation == null)
            {
                return NotFound();
            }
            return Ok(reservation);
        }

        // Update an existing booking reservation by NIC.
        [HttpGet]
        [Route("getbynic/{nic}")]
        public IHttpActionResult GetBookingByNic(string nic)
        {
            var reservations = _bookingsCollection.Find(b => b.NIC == nic).ToList();

            if (reservations.Count == 0)
            {
                return NotFound();
            }

            return Ok(reservations);
        }

        // Get all booking reservations for a user by user ID.
        [HttpGet]
        [Route("getall/{userId}")]
        public IHttpActionResult GetAllBookings(string userId)
        {
            var reservations = _bookingsCollection.Find(b => b.UserId == userId).ToList();
            return Ok(reservations);
        }
    }
}