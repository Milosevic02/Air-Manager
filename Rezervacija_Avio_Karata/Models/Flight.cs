using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Rezervacija_Avio_Karata.Models
{
    public enum FlightStatus
    {
        Active,
        Cancelled,
        Completed
    }
    public class Flight
    {

        public int Id { get; set;}
        public string Airline { get; set; }
        public bool IsDeleted {  get; set; } = false;
        public string DepartureDestination { get; set; }
        public string ArrivalDestination { get; set; }
        public string DepartureDateAndTime { get; set; }
        public string ArrivalDateAndTime { get; set; }
        public int AvailableSeats { get; set; }
        public int OccupiedSeats { get; set; }
        public double Price { get; set; }
        public FlightStatus FlightStatus {get;set;}


    }
}