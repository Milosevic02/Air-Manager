using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Rezervacija_Avio_Karata.Models
{
    public enum Status
    {
        Kreirana,
        Odobrena,
        Otkazana,
        Zavrsena
    };
    public class Rezervacija
    {
        public Korisnik Korisnik { get; set; }  
        public Let Let { get; set; }
        public int BrojPutnika {  get; set; }
        public double UkupnaCena { get; set; }
        public Status Status { get; set; }

        public Rezervacija() { }
    }
}