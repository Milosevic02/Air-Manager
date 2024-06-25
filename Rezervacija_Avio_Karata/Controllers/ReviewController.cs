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

    public class ReviewController : ApiController
    {
        [HttpPost]
        [Route("AddReview")]
        public IHttpActionResult AddReview(Review review,string name)
        {

            if(review == null)
            {
                return BadRequest();
            }
            string content = File.ReadAllText(Path.Combine(HttpRuntime.AppDomainAppPath + "App_Data/Reviews.txt"));
            List<Review> reviews = JsonConvert.DeserializeObject<List<Review>>(content) ?? new List<Review>();
            review.Id = IdGenerator.GenerateReviewId();
            review.Reviewer = ((User)HttpContext.Current.Session["user"]).Username;
            review.ReviewStatus = ReviewStatus.Created;
            review.Airline = name;
            if (!AddReviewToAirline(review))
            {
                return BadRequest("Airline with name " + review.Airline + "doesnt exists");
            }
            reviews.Add(review);

            content = JsonConvert.SerializeObject(reviews, Formatting.Indented);
            File.WriteAllText(Path.Combine(HttpRuntime.AppDomainAppPath + "App_Data/Reviews.txt"), content);
            return Ok();

        }

        private bool AddReviewToAirline(Review review)
        {
            string content = File.ReadAllText(Path.Combine(HttpRuntime.AppDomainAppPath + "App_Data/Airllines.txt"));
            List<Airlline> airllines = JsonConvert.DeserializeObject<List<Airlline>>(content) ?? new List<Airlline>();
            foreach(Airlline air in airllines)
            {
                if(air.Name == review.Airline)
                {
                    air.Reviews.Add(review);
                    content = JsonConvert.SerializeObject(airllines, Formatting.Indented);
                    File.WriteAllText(Path.Combine(HttpRuntime.AppDomainAppPath + "App_Data/Airllines.txt"), content);
                    return true;
                }
            }
            return false;
        }

        [HttpGet]
        [Route("GetCreatedReview")]
        public List<Review> GetCreatedReview()
        {
            List<Review> res = new List<Review>();
            string content = File.ReadAllText(Path.Combine(HttpRuntime.AppDomainAppPath + "App_Data/Reviews.txt"));
            List<Review> reviews = JsonConvert.DeserializeObject<List<Review>>(content) ?? new List<Review>();
            foreach(Review review in reviews)
            {
                if (review.ReviewStatus == ReviewStatus.Created)
                {
                    res.Add(review);
                }
            }
            return res;
        }

        [HttpPost]
        [Route("ChangeReviewStatus")]
        public IHttpActionResult ChangeReservationStatus(int id, string action)
        {
            string content = File.ReadAllText(Path.Combine(HttpRuntime.AppDomainAppPath + "App_Data/Reviews.txt"));
            List<Review> reviews = JsonConvert.DeserializeObject<List<Review>>(content) ?? new List<Review>();
            bool find = false;
            foreach (Review review in reviews)
            {
                if (review.Id == id)
                {
                    find = true;
                    if (action == "Approved")
                    {
                        review.ReviewStatus = ReviewStatus.Approved;
                        LoadStatusInAirlineFile(id,review.Airline,true);
                        break;
                    }
                    else if (action == "Rejected")
                    {
                        review.ReviewStatus = ReviewStatus.Rejected;
                        LoadStatusInAirlineFile(id, review.Airline, false);

                        break;
                    }
                }
            }
            if (!find) { return NotFound(); }
            content = JsonConvert.SerializeObject(reviews, Formatting.Indented);
            File.WriteAllText(Path.Combine(HttpRuntime.AppDomainAppPath + "App_Data/Reviews.txt"), content);
            return Ok();
        }

        private void LoadStatusInAirlineFile(int id, string airlineName,bool approved)
        {
            string content = File.ReadAllText(Path.Combine(HttpRuntime.AppDomainAppPath + "App_Data/Airllines.txt"));
            List<Airlline> airllines = JsonConvert.DeserializeObject<List<Airlline>>(content) ?? new List<Airlline>();

            foreach(Airlline air in airllines)
            {
                if (air.Name == airlineName)
                {
                    for (int i = 0; i < air.Reviews.Count; i++)
                    {
                        if (air.Reviews[i].Id == id)
                        {
                            if (approved)
                            {
                                air.Reviews[i].ReviewStatus = ReviewStatus.Approved;

                            }
                            else
                            {
                                air.Reviews[i].ReviewStatus = ReviewStatus.Rejected;
                            }
                        }
                    }
                }
            }

            content = JsonConvert.SerializeObject(airllines, Formatting.Indented);
            File.WriteAllText(Path.Combine(HttpRuntime.AppDomainAppPath + "App_Data/Airllines.txt"), content);
        }




    }
}
