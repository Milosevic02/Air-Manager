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
                ChangeAvailableSeats(reservation.FlightId, reservation.CountOfPassengers,false);
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

        private void ChangeAvailableSeats(int id, int seats, bool inc)
        {
            int temp = inc ? 1 : -1;

            string content = File.ReadAllText(Path.Combine(HttpRuntime.AppDomainAppPath, "App_Data/Flights.txt"));

            List<Flight> flights = JsonConvert.DeserializeObject<List<Flight>>(content) ?? new List<Flight>();

            foreach (Flight flight in flights)
            {
                if (flight.Id == id)
                {
                    flight.AvailableSeats += temp * seats;
                    flight.OccupiedSeats -= temp * seats;
                }
            }

            content = JsonConvert.SerializeObject(flights, Formatting.Indented);
            File.WriteAllText(Path.Combine(HttpRuntime.AppDomainAppPath, "App_Data/Flights.txt"), content);
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

        [HttpDelete]
        [Route("CancelReservation")]
        public IHttpActionResult CancelReservation(int id)
        {
            string content = File.ReadAllText(Path.Combine(HttpRuntime.AppDomainAppPath + "App_Data/Reservations.txt"));
            List<Reservation> reservations = JsonConvert.DeserializeObject<List<Reservation>>(content) ?? new List<Reservation>();
            bool find = false;
            for(int i = 0; i < reservations.Count; i++)
            {
                if (reservations[i].Id == id)
                {
                    if (IsLessThan24Hours(reservations[i].FlightId))
                    {
                        return BadRequest("You cant cancel your reservation less than 24h before flight");
                    }
                    find = true;
                    ChangeAvailableSeats(reservations[i].FlightId, reservations[i].CountOfPassengers, true);

                    reservations.RemoveAt(i);
                    break;
                }
            }
            if (!find) { return NotFound(); }
            content = JsonConvert.SerializeObject(reservations, Formatting.Indented);
            File.WriteAllText(Path.Combine(HttpRuntime.AppDomainAppPath + "App_Data/Reservations.txt"), content);
            return Ok();
        }

        private bool IsLessThan24Hours(int flightId)
        {
            string content = File.ReadAllText(Path.Combine(HttpRuntime.AppDomainAppPath + "App_Data/Flights.txt"));
            List<Flight> flights = JsonConvert.DeserializeObject<List<Flight>>(content) ?? new List<Flight>();
            foreach (Flight flight in flights)
            {
                if(flight.Id == flightId)
                {
                    string datetimeStr = flight.DepartureDateAndTime;
                    DateTime datetime;
                    if (!DateTime.TryParseExact(datetimeStr, "yyyy-MM-dd HH:mm", null, System.Globalization.DateTimeStyles.None, out datetime))
                    {
                        throw new ArgumentException("Invalid datetime format. Expected format: yyyy-MM-dd HH:mm");
                    }

                    TimeSpan diff = datetime - DateTime.Now;
                    double diffHours = diff.TotalHours;

                    return diffHours < 24;
                }
            }
            return false;
            
        }

    }
}
