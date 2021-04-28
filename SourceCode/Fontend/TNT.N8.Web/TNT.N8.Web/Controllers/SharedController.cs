using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace TNT.N8.Web.Controllers
{
    public class SharedController : Controller
    {
        private IConfiguration configuration;
        public SharedController(IConfiguration _configuration)
        {
            configuration = _configuration;
        }

        [HttpGet]
        [Route("/api/shared/getkey")]
        public JsonResult GetAppSettingByKey([FromQuery]string key)
        {
            return new JsonResult(new { Key = key, Value = configuration[key]});
        }
    }
}