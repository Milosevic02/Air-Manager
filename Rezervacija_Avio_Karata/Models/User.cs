using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Rezervacija_Avio_Karata.Models
{
    public enum Gender {
        Male,
        Female

    }
    public class User
    {
        public string Username {  get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public string Name { get; set; }    
        public string Surname {  get; set; }
        public DateTime DateOfBirth { get; set; }
        public Gender Gender { get; set; }
        public List<Reservation> Reservations { get; set; } = new List<Reservation>();

    }
}