using Newtonsoft.Json;
using Rezervacija_Avio_Karata.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;

namespace Rezervacija_Avio_Karata.Controllers
{
    [RoutePrefix("api")]

    public class ReservationController : ApiController
    {
        [HttpPost]
        [Route("CreateReservation")]
        public IHttpActionResult CreateReservation(Reservation reservation)
        {
            string content = File.ReadAllText(Path.Combine(HttpRuntime.AppDomainAppPath + "App_Data/Reservations.txt"));
            List<Reservation> reservations = JsonConvert.DeserializeObject<List<Reservation>>(content) ?? new List<Reservation>();
            if (HasFreeSeats(reservation.FlightId, reservation.CountOfPassengers))
            {
                reservation.ReservationStatus = ReservationStatus.Created;
                reservation.Id = IdGenerator.GenerateReservationId();
                reservation.User = ((User)HttpContext.Current.Session["user"]).Username;
                reservations.Add(reservation);
                DecreaseAvailableSeats(reservation.FlightId, reservation.CountOfPassengers);
                content = JsonConvert.SerializeObject(reservations, Formatting.Indented);
                File.WriteAllText(Path.Combine(HttpRuntime.AppDomainAppPath + "App_Data/Reservations.txt"), content);

                return Ok();
            }
            else
            {
                return BadRequest("Flight dont have enough seats for you.");
            }

        }

        private bool HasFreeSeats(int id, int seats)
        {
            string content = File.ReadAllText(Path.Combine(HttpRuntime.AppDomainAppPath + "App_Data/Flights.txt"));
            List<Flight> flights = JsonConvert.DeserializeObject<List<Flight>>(content) ?? new List<Flight>();
            foreach (Flight flight in flights)
            {
                if (flight.Id == id)
                {
                    if (flight.AvailableSeats > seats)
                    {
                        return true;
                    }
                    return false;
                }
            }
            return false;
        }

        private void DecreaseAvailableSeats(int id, int seats)
        {
            string content = File.ReadAllText(Path.Combine(HttpRuntime.AppDomainAppPath + "App_Data/Flights.txt"));
            List<Flight> flights = JsonConvert.DeserializeObject<List<Flight>>(content) ?? new List<Flight>();
            foreach (Flight flight in flights)
            {
                if (flight.Id == id)
                {
                    flight.AvailableSeats -= seats;
                    flight.OccupiedSeats += seats;
                }
            }
            content = JsonConvert.SerializeObject(flights, Formatting.Indented);
            File.WriteAllText(Path.Combine(HttpRuntime.AppDomainAppPath + "App_Data/Flights.txt"), content);

        }

        [HttpGet]
        [Route("LoadCreatedReservations")]
        public List<Reservation> GetCreatedReservation()
        {
            List<Reservation>retVal = new List<Reservation>();
            string content = File.ReadAllText(Path.Combine(HttpRuntime.AppDomainAppPath + "App_Data/Reservations.txt"));
            List<Reservation> reservations = JsonConvert.DeserializeObject<List<Reservation>>(content) ?? new List<Reservation>();
            foreach (Reservation reservation in reservations)
            {
                if(reservation.ReservationStatus == ReservationStatus.Created && reservation.User == ((User)HttpContext.Current.Session["user"]).Username)
                {
                    retVal.Add(reservation);
                }
            }
            return retVal;

        }

    }
}
