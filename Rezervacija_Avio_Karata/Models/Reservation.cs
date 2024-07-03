using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Rezervacija_Avio_Karata.Models
{
    public enum ReservationStatus
    {
        Created,
        Approved,
        Rejected,
        Finished
    }
    public class Reservation
    {
        public int Id { get; set; }
        public string User { get; set; }
        public int FlightId { get; set; }
        public int CountOfPassengers {  get; set; }
        public double Price {  get; set; }
        public ReservationStatus ReservationStatus { get; set; }

    }
}