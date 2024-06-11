using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Rezervacija_Avio_Karata.Models
{
    public enum StatusLeta
    {
        Aktivan,
        Otkazan,
        Zavrsen
    }
    public class Let
    {
        public Aviokompanija Aviokompanija { get; set; }
        public string PolaznaDestinacija {  get; set; }
        public string OdredisnaDestinacija { get; set; }
        public DateTime DatumVremePolaska {  get; set; }
        public DateTime DatumVremeDolaska { get; set; }
        public int BrojSlobodnihMesta {  get; set; }
        public int BrojZauzetihMesta { get; set; }
        public double Cena {  get; set; }
        public StatusLeta StatusLeta { get; set; }
        public Let() { }



    }
}