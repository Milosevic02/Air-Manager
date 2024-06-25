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
    }
}
