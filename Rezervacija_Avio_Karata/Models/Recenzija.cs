using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Rezervacija_Avio_Karata.Models
{
    public enum StatusRecenzije
    {
        Kreirana,
        Odobrena,
        Odbijena
    };
    public class Recenzija
    {
        public Korisnik Recezent {  get; set; }
        public Aviokompanija Aviokompanija { get; set; }
        public string Naslov {  get; set; }
        public string Sadrzaj {  get; set; }
        public string Slika { get; set; }
        public StatusRecenzije StatusRecenzije { get; set; }
    }
}