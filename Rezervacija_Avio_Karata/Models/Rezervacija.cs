using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Rezervacija_Avio_Karata.Models
{
    public enum StatusRezervacije
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
        public StatusRezervacije StaStatusRezervacijetus { get; set; }

        public Rezervacija() { }
    }
}