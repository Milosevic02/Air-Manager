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
using System.Xml.Linq;

namespace Rezervacija_Avio_Karata.Controllers
{
    [RoutePrefix("api")]
    public class AirllineController : ApiController
    {
        [HttpGet]
        [Route("GetAllAirllines")]
        public List<Airlline> GetAllAirllines()
        {
            string content = File.ReadAllText(Path.Combine(HttpRuntime.AppDomainAppPath + "App_Data/Airllines.txt"));
            List<Airlline> airllines = JsonConvert.DeserializeObject<List<Airlline>>(content) ?? new List<Airlline>();
            return airllines;
        }

        [HttpPost]
        [Route("FilterAirlines")]
        public List<Airlline> FilterAirlines([FromBody] AirlineFilter filter)
        {
            string content = File.ReadAllText(Path.Combine(HttpRuntime.AppDomainAppPath, "App_Data/Airllines.txt"));
            List<Airlline> airlines = JsonConvert.DeserializeObject<List<Airlline>>(content) ?? new List<Airlline>();

            IEnumerable<Airlline> filteredAirlines = airlines;

            if (!string.IsNullOrEmpty(filter.SearchValue))
            {
                string searchLower = filter.SearchValue.ToLower();
                switch (filter.SortBy.ToLower())
                {
                    case "name":
                        filteredAirlines = filteredAirlines.Where(a =>
                            a.Name.ToLower().Contains(searchLower));
                        break;
                    case "address":
                        filteredAirlines = filteredAirlines.Where(a =>
                            a.Address.ToLower().Contains(searchLower));
                        break;
                    case "contactinfo":
                        filteredAirlines = filteredAirlines.Where(a =>
                            a.ContactInfo.ToLower().Contains(searchLower));
                        break;
                    default:
                        filteredAirlines = filteredAirlines.Where(a =>
                            a.Name.ToLower().Contains(searchLower) ||
                            a.Address.ToLower().Contains(searchLower) ||
                            a.ContactInfo.ToLower().Contains(searchLower));
                        break;
                }
            }

            return filteredAirlines.ToList();
        }

        public class AirlineFilter
        {
            public string SearchValue { get; set; }
            public string SortBy { get; set; }
        }


        [HttpPost]
        [Route("AddAirlline")]
        public IHttpActionResult AddAirlline(Airlline airlline)
        {
            if (airlline == null)
                return null;

            string content = File.ReadAllText(Path.Combine(HttpRuntime.AppDomainAppPath + "App_Data/Airllines.txt"));
            List<Airlline> airllines = JsonConvert.DeserializeObject<List<Airlline>>(content) ?? new List<Airlline>();
            if (IsAirlineExists(airlline.Name))
            {
                return BadRequest("Airline with Name " +  airlline.Name + " already exists");
            }
            airllines.Add(airlline);
            content = JsonConvert.SerializeObject(airllines, Formatting.Indented);
            File.WriteAllText(Path.Combine(HttpRuntime.AppDomainAppPath + "App_Data/Airllines.txt"), content);
            return Ok();
        }

        [HttpGet]
        [Route("GetAirlineDetails")]
        public IHttpActionResult GetAirlineDetails(string name,string role)
        {
            string content = File.ReadAllText(Path.Combine(HttpRuntime.AppDomainAppPath + "App_Data/Airllines.txt"));
            List<Airlline> airllines = JsonConvert.DeserializeObject<List<Airlline>>(content) ?? new List<Airlline>();
            var airline = airllines.FirstOrDefault(a => a.Name == name);
            if (airline == null)
            {
                return NotFound();
            }
            if(role != "Admin")
            {
                airline.Reviews = airline.Reviews.Where(r => r.ReviewStatus == ReviewStatus.Approved).ToList();
            }
            return Ok(airline);
        }

        [HttpPut]
        [Route("EditAirline")]
        public IHttpActionResult EditAirline(Airlline airline,string oldName)
        {
       
            if (airline.Name != oldName)
            {
                if (IsAirlineExists(airline.Name))
                {
                    return BadRequest("Airline with name " + airline.Name + " already exists");
                }
                FileChangeAirlineForFlights(airline.Name, oldName);
            }
            string content = File.ReadAllText(Path.Combine(HttpRuntime.AppDomainAppPath + "App_Data/Airllines.txt"));
            List<Airlline> airllines = JsonConvert.DeserializeObject<List<Airlline>>(content) ?? new List<Airlline>();
            for (int i = 0; i < airllines.Count; i++)
            {
                if (airllines[i].Name == oldName)
                {
                    airllines[i].Name = airline.Name;
                    airllines[i].Address = airline.Address;
                    airllines[i].ContactInfo = airline.ContactInfo;
                    break;
                }
            }
            content = JsonConvert.SerializeObject(airllines, Formatting.Indented);
            File.WriteAllText(Path.Combine(HttpRuntime.AppDomainAppPath + "App_Data/Airllines.txt"), content);
            return Ok();
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

        private void FileChangeAirlineForFlights(string airlineName, string oldName)
        {
            string content = File.ReadAllText(Path.Combine(HttpRuntime.AppDomainAppPath + "App_Data/Airllines.txt"));
            List<Airlline> airllines = JsonConvert.DeserializeObject<List<Airlline>>(content) ?? new List<Airlline>();
            foreach(Airlline air in airllines)
            {
                if(air.Name == oldName)
                {
                    for (int i = 0; i < air.Flights.Count; i++) {
                        air.Flights[i].Airline = airlineName;
                    }
                    break;
                }
            }
            content = JsonConvert.SerializeObject(airllines, Formatting.Indented);
            File.WriteAllText(Path.Combine(HttpRuntime.AppDomainAppPath + "App_Data/Airllines.txt"), content);

            UpdateFlights(oldName, airlineName);
        
        }

        private void UpdateFlights(string oldName, string airlineName)
        {
            string content = File.ReadAllText(Path.Combine(HttpRuntime.AppDomainAppPath + "App_Data/Flights.txt"));
            List<Flight> flights = JsonConvert.DeserializeObject<List<Flight>>(content) ?? new List<Flight>();
            foreach (Flight f in flights)
            {
                if(f.Airline == oldName)
                {
                    f.Airline = airlineName;
                }
            }
            content = JsonConvert.SerializeObject(flights, Formatting.Indented);
            File.WriteAllText(Path.Combine(HttpRuntime.AppDomainAppPath + "App_Data/Flights.txt"), content);
       
        }

        [HttpDelete]
        [Route("DeleteAirline")]
        public IHttpActionResult DeleteAirline(string name)
        {
            string content = File.ReadAllText(Path.Combine(HttpRuntime.AppDomainAppPath + "App_Data/Airllines.txt"));
            List<Airlline> airllines = JsonConvert.DeserializeObject<List<Airlline>>(content) ?? new List<Airlline>();
            bool find = false;
            for(int i = 0;i<airllines.Count;i++) {
                if (airllines[i].Name == name)
                {
                    if (HaveActiveFlights(airllines[i]))
                    {
                        return BadRequest("Airline with name " + airllines[i].Name + " has active flights and cant delete!");
                    }
                    else
                    {
                        find = true;
                        DeleteFlights(airllines[i]);
                        airllines.RemoveAt(i);
                        break;
                    }
                }
            }
            if (!find) { return NotFound(); }
        
            content = JsonConvert.SerializeObject(airllines, Formatting.Indented);
            File.WriteAllText(Path.Combine(HttpRuntime.AppDomainAppPath + "App_Data/Airllines.txt"), content);


            return Ok();
        }

        private bool HaveActiveFlights(Airlline air) {
            string content = File.ReadAllText(Path.Combine(HttpRuntime.AppDomainAppPath + "App_Data/Flights.txt"));
            List<Flight> flights = JsonConvert.DeserializeObject<List<Flight>>(content) ?? new List<Flight>();
            foreach (Flight f in flights)
            {
                if(f.Airline == air.Name)
                {
                    if (f.FlightStatus == FlightStatus.Active)
                    {
                        return true;
                    }
                }
                
            }
            return false;
        }

        private void DeleteFlights(Airlline air) {
            List<int> flightsId = new List<int>();
            foreach (Flight flight in air.Flights)
            {
                flightsId.Add(flight.Id);
            }
            string content = File.ReadAllText(Path.Combine(HttpRuntime.AppDomainAppPath + "App_Data/Flights.txt"));
            List<Flight> flights = JsonConvert.DeserializeObject<List<Flight>>(content) ?? new List<Flight>();
            flights = flights.Where(f => !flightsId.Contains(f.Id)).ToList();

            content = JsonConvert.SerializeObject(flights, Formatting.Indented);
            File.WriteAllText(Path.Combine(HttpRuntime.AppDomainAppPath + "App_Data/Flights.txt"), content);
        }
    }
}


