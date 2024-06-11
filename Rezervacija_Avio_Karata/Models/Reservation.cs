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
        public User User { get; set; }
        public Flight Flight { get; set; }
        public int NumberOfPassenger {  get; set; }
        public double Price {  get; set; }
        public ReservationStatus ReservationStatus { get; set; }

    }
}