using Newtonsoft.Json;
using Rezervacija_Avio_Karata.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Web;
using System.Web.Http;

namespace Rezervacija_Avio_Karata.Controllers
{
    [RoutePrefix("api")]
    public class FlightController : ApiController
    {
        [HttpPost]
        [Route("GetAllFlights")]
        public List<Flight> GetAllFlights([FromBody] FlightFilter filter)
        {
            UpdateCompleatedFlights();

            string content = File.ReadAllText(Path.Combine(HttpRuntime.AppDomainAppPath + "App_Data/Flights.txt"));
            List<Flight> flights = JsonConvert.DeserializeObject<List<Flight>>(content) ?? new List<Flight>();
            List<Flight> result = new List<Flight>();
            foreach (Flight f in flights)
            {
                if (HttpContext.Current.Session["user"] == null)
                {
                    if (f.FlightStatus == FlightStatus.Active && !f.IsDeleted) result.Add(f);
                }
                else
                {
                    if (((User)HttpContext.Current.Session["user"]).Role == "Admin")
                    {
                        if (!f.IsDeleted) result.Add(f);
                    }
                    else
                    {
                        if (f.FlightStatus == FlightStatus.Active && !f.IsDeleted) result.Add(f);
                    }
                }
            }

            List<Flight> retVal = FilterFlights(result, filter);
            return retVal;
        }

        private void UpdateCompleatedFlights()
        {
            string content = File.ReadAllText(Path.Combine(HttpRuntime.AppDomainAppPath + "App_Data/Flights.txt"));
            List<Flight> flights = JsonConvert.DeserializeObject<List<Flight>>(content) ?? new List<Flight>();
            foreach(Flight f in flights)
            {
                if (Expired(f.DepartureDateAndTime))
                {
                    f.FlightStatus = FlightStatus.Completed;
                }
            }
            content = JsonConvert.SerializeObject(flights, Formatting.Indented);
            File.WriteAllText(Path.Combine(HttpRuntime.AppDomainAppPath + "App_Data/Flights.txt"), content);
        }

        private bool Expired(string date)
        {
            DateTime departureDateTime;
            if (DateTime.TryParseExact(date, "yyyy-MM-dd HH:mm", null, System.Globalization.DateTimeStyles.None, out departureDateTime))
            {
                return DateTime.Now > departureDateTime;
            }
            return false;
        }


        private List<Flight> FilterFlights(List<Flight> flights, FlightFilter filter)
        {
            IEnumerable<Flight> filteredFlights = flights;
            if (!string.IsNullOrEmpty(filter.Status) && filter.Status != "active")
            {
                filteredFlights = StatusFilter(filter.Status);
            }
            if (!string.IsNullOrEmpty(filter.DepartureDestination))
            {
                filteredFlights = filteredFlights.Where(f => f.DepartureDestination.ToLower().Contains(filter.DepartureDestination.ToLower()));
            }
            if (!string.IsNullOrEmpty(filter.DepartureDateFrom))
            {
                DateTime departureDateFrom = DateTime.Parse(filter.DepartureDateFrom);
                filteredFlights = filteredFlights.Where(f => DateTime.Parse(f.DepartureDateAndTime).Date >= departureDateFrom.Date);
            }
            if (!string.IsNullOrEmpty(filter.DepartureDateTo))
            {
                DateTime departureDateTo = DateTime.Parse(filter.DepartureDateTo);
                filteredFlights = filteredFlights.Where(f => DateTime.Parse(f.DepartureDateAndTime).Date <= departureDateTo.Date);
            }
            if (!string.IsNullOrEmpty(filter.Airline))
            {
                filteredFlights = filteredFlights.Where(f => f.Airline.ToLower().Contains(filter.Airline.ToLower()));
            }
            if (!string.IsNullOrEmpty(filter.ArrivalDestination))
            {
                filteredFlights = filteredFlights.Where(f => f.ArrivalDestination.ToLower().Contains(filter.ArrivalDestination.ToLower()));
            }
            if (!string.IsNullOrEmpty(filter.ArrivalDateFrom))
            {
                DateTime arrivalDateFrom = DateTime.Parse(filter.ArrivalDateFrom);
                filteredFlights = filteredFlights.Where(f => DateTime.Parse(f.ArrivalDateAndTime).Date >= arrivalDateFrom.Date);
            }
            if (!string.IsNullOrEmpty(filter.ArrivalDateTo))
            {
                DateTime arrivalDateTo = DateTime.Parse(filter.ArrivalDateTo);
                filteredFlights = filteredFlights.Where(f => DateTime.Parse(f.ArrivalDateAndTime).Date <= arrivalDateTo.Date);
            }

            if (filter.SortByPrice == "asc")
            {
                filteredFlights = filteredFlights.OrderBy(f => f.Price);
            }
            else if (filter.SortByPrice == "desc")
            {
                filteredFlights = filteredFlights.OrderByDescending(f => f.Price);
            }

            return filteredFlights.ToList();
        }


        private List<Flight> StatusFilter(string status)
        {
            List<Flight> filteredFlights = new List<Flight>();
            List<int> flightsId = GetFlightsIdFromReservation();
            FlightStatus fs = GetFlightStatus(status);
            string content = File.ReadAllText(Path.Combine(HttpRuntime.AppDomainAppPath + "App_Data/Flights.txt"));
            List<Flight> flights = JsonConvert.DeserializeObject<List<Flight>>(content) ?? new List<Flight>();
            foreach (Flight flight in flights)
            {
                if (flightsId.Contains(flight.Id))
                {
                    if(flight.FlightStatus == fs)
                    {
                        filteredFlights.Add(flight);
                    }
                }
            }
            return filteredFlights;
        }
        private FlightStatus GetFlightStatus(string status) {
            if(status == "completed")
            {
                return FlightStatus.Completed;
            }else
            {
                return FlightStatus.Cancelled;
            }
        }


        private List<int> GetFlightsIdFromReservation()
        {
            List<int> retVal = new List<int>();
            string content = File.ReadAllText(Path.Combine(HttpRuntime.AppDomainAppPath + "App_Data/Reservations.txt"));
            List<Reservation> reservations = JsonConvert.DeserializeObject<List<Reservation>>(content) ?? new List<Reservation>();
            foreach (Reservation reservation in reservations)
            {
                if (reservation.ReservationStatus != ReservationStatus.Rejected)
                {
                    if (reservation.User == ((User)HttpContext.Current.Session["user"]).Username)
                    {
                        retVal.Add(reservation.FlightId);
                    }

                   
                }
            }
            return retVal;
        }

        public class FlightFilter
        {
            public string DepartureDestination { get; set; }
            public string DepartureDateFrom { get; set; }

            public string DepartureDateTo { get; set; }
            public string Airline { get; set; }
            public string ArrivalDestination { get; set; }
            public string ArrivalDateFrom { get; set; }

            public string ArrivalDateTo { get; set; }
            public string Status { get; set; }
            public string SortByPrice { get; set; }
            public string SortByStatus { get; set; }
        }


        [HttpPost]
        [Route("AddFlight")]
        public IHttpActionResult AddFlight(Flight flight)
        {
            if (flight == null)
                return null;
            string airllineContent = File.ReadAllText(Path.Combine(HttpRuntime.AppDomainAppPath + "App_Data/Airllines.txt"));
            List<Airlline> airllines = JsonConvert.DeserializeObject<List<Airlline>>(airllineContent) ?? new List<Airlline>();

            int index = -1;

            for(int i = 0; i < airllines.Count; i++)
            {
                if(flight.Airline == airllines[i].Name)
                {
                    index = i;
                }
            }
            if (index != -1)
            {
                string[] dateA = flight.ArrivalDateAndTime.ToString().Split('T');
                string[] dateD = flight.DepartureDateAndTime.ToString().Split('T');

                flight.DepartureDateAndTime = dateD[0] + ' ' + dateD[1];
                flight.ArrivalDateAndTime = dateA[0] + ' ' + dateA[1];
                flight.FlightStatus = FlightStatus.Active;
                flight.OccupiedSeats = 0;
                flight.Id = IdGenerator.GenerateFlightId();

                var airlline = airllines[index];
                airllines.RemoveAt(index);
                airlline.Flights.Add(flight);
                airllines.Insert(index,airlline);
            }
            else
            {
                return BadRequest("Airlline with name \"" + flight.Airline + "\" doesnt exist!");
            }

            airllineContent = JsonConvert.SerializeObject(airllines, Formatting.Indented);
            File.WriteAllText(Path.Combine(HttpRuntime.AppDomainAppPath + "App_Data/Airllines.txt"), airllineContent);



            string content = File.ReadAllText(Path.Combine(HttpRuntime.AppDomainAppPath + "App_Data/Flights.txt"));
            List<Flight> flights = JsonConvert.DeserializeObject<List<Flight>>(content) ?? new List<Flight>();
            flights.Add(flight);
            content = JsonConvert.SerializeObject(flights, Formatting.Indented);
            File.WriteAllText(Path.Combine(HttpRuntime.AppDomainAppPath + "App_Data/Flights.txt"), content);
            
            return Ok();
        }

        

        [HttpGet]
        [Route("GetFlightDetails")]
        public IHttpActionResult GetFlightDetails(int id)
        {
            var flight = GetFlightById(id);

            if (flight == null)
            {
                return NotFound();
            }

            return Ok(flight);
        }

        private Flight GetFlightById(int id)
        {
            string content = File.ReadAllText(Path.Combine(HttpRuntime.AppDomainAppPath + "App_Data/Flights.txt"));
            List<Flight> flights = JsonConvert.DeserializeObject<List<Flight>>(content) ?? new List<Flight>();

            var flight = flights.FirstOrDefault(f => f.Id == id);
            return flight;
        }

        [HttpDelete]
        [Route("DeleteFlight")]
        public IHttpActionResult DeleteFlight(int id) {
            string content = File.ReadAllText(Path.Combine(HttpRuntime.AppDomainAppPath + "App_Data/Flights.txt"));
            List<Flight> flights = JsonConvert.DeserializeObject<List<Flight>>(content) ?? new List<Flight>();
            
            
            bool success = false;
            foreach (Flight flight in flights)
            {
                if (flight.Id == id)
                {
                    flight.IsDeleted = true;
                    success = true;
                    LogicalDeleteFlightFromAirllineFile(flight);

                }
            }
            if(!success) return BadRequest("Flight doesnt exist!");
            content = JsonConvert.SerializeObject(flights, Formatting.Indented);
            File.WriteAllText(Path.Combine(HttpRuntime.AppDomainAppPath + "App_Data/Flights.txt"), content);


            return Ok();
        }

        private void LogicalDeleteFlightFromAirllineFile(Flight flight) {
            string content = File.ReadAllText(Path.Combine(HttpRuntime.AppDomainAppPath + "App_Data/Airllines.txt"));
            List<Airlline> airllines = JsonConvert.DeserializeObject<List<Airlline>>(content) ?? new List<Airlline>();
            foreach (Airlline air in airllines)
            {
                if(air.Name == flight.Airline)
                {
                    for (int i = 0; i < air.Flights.Count; i++)
                    {
                        if(air.Flights[i].Id == flight.Id)
                        {
                            air.Flights[i].IsDeleted = true;
                        }
                    }
                }
            }
            content = JsonConvert.SerializeObject(airllines, Formatting.Indented);
            File.WriteAllText(Path.Combine(HttpRuntime.AppDomainAppPath + "App_Data/Airllines.txt"), content);
        }

        [HttpPut]
        [Route("EditFlight")]
        public IHttpActionResult EditFlight(Flight flight,int id) {
            string content = File.ReadAllText(Path.Combine(HttpRuntime.AppDomainAppPath + "App_Data/Flights.txt"));
            List<Flight> flights = JsonConvert.DeserializeObject<List<Flight>>(content) ?? new List<Flight>();
            if (IsAirlineExists(flight.Airline))
            {
                for(int i = 0; i < flights.Count; i++)
                {
                    if (flights[i].Id == id)
                    {
                        string oldAirline = flights[i].Airline;
                        flights[i].Airline = flight.Airline;
                        string[] dateA = flight.ArrivalDateAndTime.ToString().Split('T');
                        string[] dateD = flight.DepartureDateAndTime.ToString().Split('T');

          
                        flights[i].ArrivalDateAndTime = dateD[0] + ' ' + dateD[1];
                        flights[i].DepartureDateAndTime = dateA[0] + ' ' + dateA[1];
                        flights[i].OccupiedSeats = flight.OccupiedSeats;
                        flights[i].AvailableSeats = flight.AvailableSeats;
                        flights[i].Price = flight.Price;
                        flights[i].FlightStatus = flight.FlightStatus;
                        FileChangeFlightForAirline(oldAirline, flights[i]);
                        break;
                    }
                }
            }
            else
            {
                return BadRequest("Airline with name " + flight.Airline + "doesnt exists");
            }
            content = JsonConvert.SerializeObject(flights, Formatting.Indented);
            File.WriteAllText(Path.Combine(HttpRuntime.AppDomainAppPath + "App_Data/Flights.txt"), content);

            return Ok();
        }

        private void FileChangeFlightForAirline(string oldAirline,Flight flight) 
        {
            
            if (flight.Airline != oldAirline)
            {
                DeleteFlightFromAirllineFile(oldAirline, flight.Id);
                string content = File.ReadAllText(Path.Combine(HttpRuntime.AppDomainAppPath + "App_Data/Airllines.txt"));
                List<Airlline> airllines = JsonConvert.DeserializeObject<List<Airlline>>(content) ?? new List<Airlline>();

                foreach (Airlline air in airllines)
                {
                    if(air.Name == flight.Airline)
                    {
                        air.Flights.Add(flight);
                        break;
                    }
                }
                content = JsonConvert.SerializeObject(airllines, Formatting.Indented);
                File.WriteAllText(Path.Combine(HttpRuntime.AppDomainAppPath + "App_Data/Airllines.txt"), content);
            }
            else
            {
                string content = File.ReadAllText(Path.Combine(HttpRuntime.AppDomainAppPath + "App_Data/Airllines.txt"));
                List<Airlline> airllines = JsonConvert.DeserializeObject<List<Airlline>>(content) ?? new List<Airlline>();

                foreach (Airlline air in airllines){
                    if(air.Name == flight.Airline)
                    {
                       for(int i = 0; i < air.Flights.Count; i++)
                        {
                            if (air.Flights[i].Id == flight.Id)
                            {
                                air.Flights[i] = flight;
                                break;
                            }
                        }
                    }
                }
                content = JsonConvert.SerializeObject(airllines, Formatting.Indented);
                File.WriteAllText(Path.Combine(HttpRuntime.AppDomainAppPath + "App_Data/Airllines.txt"), content);

            }

        }


    

        private bool IsAirlineExists(string AirlineName)
        {
            string content = File.ReadAllText(Path.Combine(HttpRuntime.AppDomainAppPath + "App_Data/Airllines.txt"));
            List<Airlline> airllines = JsonConvert.DeserializeObject<List<Airlline>>(content) ?? new List<Airlline>();
            foreach (Airlline air in airllines)
            {
                if (air.Name == AirlineName)
                {
                    return true;
                }
            }
            return false;
        }

        private void DeleteFlightFromAirllineFile(string airlineName,int flightId)
        {
            string content = File.ReadAllText(Path.Combine(HttpRuntime.AppDomainAppPath + "App_Data/Airllines.txt"));
            List<Airlline> airllines = JsonConvert.DeserializeObject<List<Airlline>>(content) ?? new List<Airlline>();
            foreach (Airlline air in airllines)
            {
                if (air.Name == airlineName)
                {
                    for (int i = 0; i < air.Flights.Count; i++)
                    {
                        if (air.Flights[i].Id == flightId)
                        {
                            air.Flights.RemoveAt(i);
                        }
                    }
                }
            }
            content = JsonConvert.SerializeObject(airllines, Formatting.Indented);
            File.WriteAllText(Path.Combine(HttpRuntime.AppDomainAppPath + "App_Data/Airllines.txt"), content);
        }


    }
}
