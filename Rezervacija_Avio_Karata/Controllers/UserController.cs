﻿using Newtonsoft.Json;
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
                return BadRequest();
            }
            
            string[] dateTime = user.DateOfBirth.Split(' ');
            user.DateOfBirth = dateTime[0];

            users.Add(user);
            content = JsonConvert.SerializeObject(users, Formatting.Indented);
            File.WriteAllText(Path.Combine(HttpRuntime.AppDomainAppPath + "App_Data/Users.txt"), content);
            return Ok();
        }

        [HttpGet]
        [Route("LoginUser")]
        public IHttpActionResult LoginUser(string username, string password)
        {
            string content = File.ReadAllText(Path.Combine(HttpRuntime.AppDomainAppPath + "App_Data/Users.txt"));
            List<User> users = JsonConvert.DeserializeObject<List<User>>(content) ?? new List<User>();
            var exsists = users.Find(x => x.Username.Equals(username));
            if (exsists == null)
            {
                return BadRequest("User with username \"" + username + "\" doesn't exist!");

            }

            if (exsists.Password != password)
            {
                return BadRequest("Incorrect password");
            }

            User current = (User)HttpContext.Current.Session["user"];
            if (current != null && current.Username == username)
            {
                return BadRequest("User already logged in");
            }

            HttpContext.Current.Session["user"] = exsists;
            return Ok(username + " successfully logged in");

        }
    }


}
