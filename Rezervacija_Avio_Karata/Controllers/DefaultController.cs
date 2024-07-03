using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Results;

namespace Rezervacija_Avio_Karata.Controllers
{
    public class DefaultController : ApiController
    {
        [HttpGet,Route("")]
        public RedirectResult Index()
        {
            var requestUri = Request.RequestUri;
            return Redirect(requestUri.AbsoluteUri + "Pages/Index.html");
        }
    }
}
