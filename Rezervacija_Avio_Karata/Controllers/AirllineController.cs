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
        [Route("AddAirlline")]
        public Airlline AddAirlline(Airlline airlline)
        {
            if (airlline == null)
                return null;

            string content = File.ReadAllText(Path.Combine(HttpRuntime.AppDomainAppPath + "App_Data/Airllines.txt"));
            List<Airlline> airllines = JsonConvert.DeserializeObject<List<Airlline>>(content) ?? new List<Airlline>();
            airllines.Add(airlline);
            content = JsonConvert.SerializeObject(airllines, Formatting.Indented);
            File.WriteAllText(Path.Combine(HttpRuntime.AppDomainAppPath + "App_Data/Airllines.txt"), content);
            return airlline;
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
                    return BadRequest("Airline with name " + airline.Name + "already exists");
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
        }
    }
}


