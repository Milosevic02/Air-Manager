using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Rezervacija_Avio_Karata.Models
{
    public class Aviokompanija
    {
        public string Naziv {  get; set; }
        public string Adresa {  get; set; }
        public string Kontakt_informacije {  get; set; }
        public List<Let>Letovi { get; set; }
        public List<Recenzija> Recenzije { get;set; }

        public Aviokompanija() { }  


    }
}