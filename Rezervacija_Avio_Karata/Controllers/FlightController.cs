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
        [HttpGet]
        [Route("GetAllFlights")]
        public List<Flight> GetAllFlights()
        {
            string content = File.ReadAllText(Path.Combine(HttpRuntime.AppDomainAppPath + "App_Data/Flights.txt"));
            List<Flight> flights = JsonConvert.DeserializeObject<List<Flight>>(content) ?? new List<Flight>();
            List<Flight> result = new List<Flight>();
            foreach(Flight f in flights)
            {
                if (!(f.IsDeleted)) result.Add(f);
            }
            return result;
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
        [Route("GetActiveFlights")]
        public List<Flight> GetActiveFlights()
        {
            var retVal = new List<Flight>();
            string content = File.ReadAllText(Path.Combine(HttpRuntime.AppDomainAppPath + "App_Data/Flights.txt"));
            List<Flight> flights = JsonConvert.DeserializeObject<List<Flight>>(content) ?? new List<Flight>();
            foreach (Flight flight in flights)
            {
                if (flight.FlightStatus == FlightStatus.Active && !(flight.IsDeleted)) retVal.Add(flight);
            }
            return retVal;
        }

        [HttpGet]
        [Route("GetFlightDetails")]
        public IHttpActionResult GetFlightDetails(int id)
        {
            var flight = GetFlightById(id);

            if (flight == null)
            {
                return NotFound(); // Return 404 Not Found if flight with ID not found
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
                    DeleteFlightFromAirllineFile(flight);

                }
            }
            if(!success) return BadRequest("Flight doesnt exist!");
            content = JsonConvert.SerializeObject(flights, Formatting.Indented);
            File.WriteAllText(Path.Combine(HttpRuntime.AppDomainAppPath + "App_Data/Flights.txt"), content);


            return Ok();
        }

        private void DeleteFlightFromAirllineFile(Flight flight) {
            string content = File.ReadAllText(Path.Combine(HttpRuntime.AppDomainAppPath + "App_Data/Airllines.txt"));
            List<Airlline> airllines = JsonConvert.DeserializeObject<List<Airlline>>(content) ?? new List<Airlline>();
            foreach (Airlline air in airllines)
            {
                if(air.Name == flight.Airline)
                {
                    air.Flights.Remove(flight);
                }
            }
            content = JsonConvert.SerializeObject(airllines, Formatting.Indented);
            File.WriteAllText(Path.Combine(HttpRuntime.AppDomainAppPath + "App_Data/Airllines.txt"), content);
        }



    }
}
