using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Rezervacija_Avio_Karata.Models
{
    public enum Pol
    {
        Muski,
        Zenski
    };
    public enum Tip
    {
        Putnik,
        Administrator
    };

    public class Korisnik
    {
        public string Korisnicko_ime { get; set; }//unique
        public string Lozinka { get; set; }
        public string Ime {  get; set; }
        public string Prezime {  get; set; }
        public string Email { get; set; }
        public DateTime Datum_rodjenja {  get; set; }
        public Pol Pol { get; set; }
        public Tip Tip_Korisnika { get; set; }    
        public List<Rezervacija>Rezervacije { get; set; }

        public Korisnik() { }
        public Korisnik(string username,string password,string ime,string prezime)
        {
            Korisnicko_ime = username;
            Lozinka = password;
            Ime = ime;
            Prezime = prezime;
            Rezervacije = new List<Rezervacija>();
        }




    }
}