using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Rezervacija_Avio_Karata.Models
{
    public class Airline
    {
        public string Name {  get; set; }
        public string Address {  get; set; }
        public string ContactInfo {  get; set; }
        public List<Flight> Flights { get; set; } = new List<Flight>();
        public List<Review> Reviews { get; set; } = new List<Review>();
    }
}