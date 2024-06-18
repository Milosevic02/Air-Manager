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
using System.Web.Routing;

namespace Rezervacija_Avio_Karata.Controllers
{
    [RoutePrefix("api")]
    public class UserController : ApiController
    {
        [HttpPost]
        [Route("RegisterUser")]
        public IHttpActionResult RegisterUser(User user)
        {
            string content = File.ReadAllText(Path.Combine(HttpRuntime.AppDomainAppPath + "App_Data/Users.txt"));
            List<User> users = JsonConvert.DeserializeObject<List<User>>(content) ?? new List<User>();
            var exsists = users.Find(x=> x.Username == user.Username);
            if (exsists != null)
            {
                return BadRequest("User with username \"" + user.Username + "\" already exist!");
            }

            string[] dateTime = user.DateOfBirth.Split(' ');
            user.DateOfBirth = dateTime[0];

            users.Add(user);
            content = JsonConvert.SerializeObject(users, Formatting.Indented);
            File.WriteAllText(Path.Combine(HttpRuntime.AppDomainAppPath + "App_Data/Users.txt"), content);
            return Ok();
        }

        [HttpPost]
        [Route("LoginUser")]
        public IHttpActionResult LoginUser(User model)
        {
            string content = File.ReadAllText(Path.Combine(HttpRuntime.AppDomainAppPath, "App_Data", "Users.txt"));
            List<User> users = JsonConvert.DeserializeObject<List<User>>(content) ?? new List<User>();
            var user = users.Find(u => u.Username.Equals(model.Username));

            if (user == null)
            {
                return BadRequest("User with username \"" + model.Username + "\" doesn't exist!");
            }

            if (user.Password != model.Password)
            {
                return BadRequest("Incorrect password");
            }

            User current = (User)HttpContext.Current.Session["user"];
            if (current != null && current.Username == model.Username)
            {
                return BadRequest("User already logged in");
            }

            HttpContext.Current.Session["user"] = user;
            return Ok(new { message = $"{model.Username} successfully logged in" });
        }

        [HttpGet]
        [Route("SignOut")]
        public IHttpActionResult SignOut()
        {
            HttpContext.Current.Session["user"] = null;
            return Ok();
        }

        [HttpGet]
        [Route("GetUserRole")]
        public string GetUserRole()
        {
            User user = (User)HttpContext.Current.Session["user"];
            if(user == null)
            {
                return "";
            }
            return user.Role;
        }


    }
}
