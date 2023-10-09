/* 
 * File: Reservation.cs
 * Description: This file contains the Reservation class which is used for modeling ticket reservation data.
 */

using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;

namespace WebSevice.Models
{
    public class Reservation
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string ID { get; set; }
        public string TravelerName { get; set; }
        public string NIC { get; set; }
        public string UserId { get; set; }
        public DateTime ReservationDate
        {
            get { return _reservationDate; }
            set
            {
                _reservationDate = value;
                FormattedReservationDate = value.ToString("yyyy-MM-dd");
            }
        }
        private DateTime _reservationDate;

        public DateTime BookingDate
        {
            get { return _bookingDate; }
            set
            {
                _bookingDate = value;
                FormattedBookingDate = value.ToString("yyyy-MM-dd");
            }
        }
        private DateTime _bookingDate;
        public string FormattedReservationDate { get; set; }
        public string FormattedBookingDate { get; set; }
        public string TrainID { get; set; }
        public string BookingStatus { get; set; }
        public string DepartureLocation { get; set; }
        public string DestinationLocation { get; set; }
        public int NumPassengers { get; set; }
        public int Age { get; set; }
        public string TicketClass { get; set; }
        public string SeatSelection { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
    }

}