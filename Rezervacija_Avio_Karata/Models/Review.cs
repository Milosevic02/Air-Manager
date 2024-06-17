using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Rezervacija_Avio_Karata.Models
{
    public enum ReviewStatus
    {
        Created,
        Approved,
        Rejected
    }
    public class Review
    {
        public User Reviewer { get; set; }
        public string Airline { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Image {  get; set; } = String.Empty;
        public ReviewStatus ReviewStatus { get; set; }
    }
}