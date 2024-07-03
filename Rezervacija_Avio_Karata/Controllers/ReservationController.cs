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
                AddReservationToUserFile(reservation);
                content = JsonConvert.SerializeObject(reservations, Formatting.Indented);
                File.WriteAllText(Path.Combine(HttpRuntime.AppDomainAppPath + "App_Data/Reservations.txt"), content);

                return Ok();
            }
            else
            {
                return BadRequest("Flight dont have enough seats for you.");
            }

        }

        private void AddReservationToUserFile(Reservation reservation)
        {
            string content = File.ReadAllText(Path.Combine(HttpRuntime.AppDomainAppPath + "App_Data/Users.txt"));
            List<User> users = JsonConvert.DeserializeObject<List<User>>(content) ?? new List<User>();
            foreach(User user in users)
            {
                if(user.Username == ((User)HttpContext.Current.Session["user"]).Username)
                {
                    user.Reservations.Add(reservation);
                    HttpContext.Current.Session["user"] = user;
                }
            }
            content = JsonConvert.SerializeObject(users, Formatting.Indented);
            File.WriteAllText(Path.Combine(HttpRuntime.AppDomainAppPath + "App_Data/Users.txt"), content);

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
        [Route("LoadReservations")]
        public List<Reservation> GetReservations(string role,string status)
        {
            UpdateFinishedReservation();
            ReservationStatus resStatus = GetReservationStatus(status);
            List<Reservation>retVal = new List<Reservation>();
            string content = File.ReadAllText(Path.Combine(HttpRuntime.AppDomainAppPath + "App_Data/Reservations.txt"));
            List<Reservation> reservations = JsonConvert.DeserializeObject<List<Reservation>>(content) ?? new List<Reservation>();
            foreach (Reservation reservation in reservations)
            {
                if(reservation.ReservationStatus == resStatus)
                {
                    if(role != "Admin")
                    {
                        if(reservation.User == ((User)HttpContext.Current.Session["user"]).Username)
                        {
                            retVal.Add(reservation);
                        }
                    }
                    else
                    {
                        retVal.Add(reservation);

                    }
                }
            }
            return retVal;

        }

        private void UpdateFinishedReservation()
        {
            List<int>flightsId = GetCompletedFlights();
            string content = File.ReadAllText(Path.Combine(HttpRuntime.AppDomainAppPath + "App_Data/Reservations.txt"));
            List<Reservation> reservations = JsonConvert.DeserializeObject<List<Reservation>>(content) ?? new List<Reservation>();
            foreach (Reservation reservation in reservations)
            {
                if (flightsId.Contains(reservation.FlightId))
                {
                    reservation.ReservationStatus = ReservationStatus.Finished;
                }

            }
            content = JsonConvert.SerializeObject(reservations, Formatting.Indented);
            File.WriteAllText(Path.Combine(HttpRuntime.AppDomainAppPath + "App_Data/Reservations.txt"), content);


        }

        private List<int> GetCompletedFlights()
        {
            string content = File.ReadAllText(Path.Combine(HttpRuntime.AppDomainAppPath + "App_Data/Flights.txt"));
            List<Flight> flights = JsonConvert.DeserializeObject<List<Flight>>(content) ?? new List<Flight>();
            List<int> result = new List<int>();
            foreach(Flight flight in flights)
            {
                if(flight.FlightStatus == FlightStatus.Completed)
                {
                    result.Add(flight.Id);
                }
            }
            
            return result;
        }



        private ReservationStatus GetReservationStatus(string status)
        {
            if(status == "created") { return ReservationStatus.Created;}
            else if(status == "rejected") { return ReservationStatus.Rejected;}
            else if (status == "approved") { return ReservationStatus.Approved;}
            else{ return ReservationStatus.Finished;}
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

        [HttpPost]
        [Route("ChangeReservationStatus")]
        public IHttpActionResult ChangeReservationStatus(int id,string action)
        {
            string content = File.ReadAllText(Path.Combine(HttpRuntime.AppDomainAppPath + "App_Data/Reservations.txt"));
            List<Reservation> reservations = JsonConvert.DeserializeObject<List<Reservation>>(content) ?? new List<Reservation>();
            bool find = false;
            foreach (Reservation reservation in reservations)
            {
                if (reservation.Id == id)
                {
                    find = true;
                    if(action == "Approved")
                    {
                        reservation.ReservationStatus = ReservationStatus.Approved;
                        break;
                    }else if(action == "Rejected")
                    {
                        ChangeAvailableSeats(reservation.FlightId, reservation.CountOfPassengers, true);

                        reservation.ReservationStatus=ReservationStatus.Rejected;

                        break;
                    }
                }
            }
            if (!find) {return NotFound();}
            content = JsonConvert.SerializeObject(reservations, Formatting.Indented);
            File.WriteAllText(Path.Combine(HttpRuntime.AppDomainAppPath + "App_Data/Reservations.txt"), content);
            return Ok();
        }


    }
}
