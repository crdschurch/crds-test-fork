using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Services;

namespace Controllers
{

    public class HealthController : Controller
    {
        /// <summary>
        /// Get this instance.
        /// </summary>
        /// <returns>The get.</returns>
        [HttpGet("api/health")]
        public ActionResult Get()
        {
            //TODO: Set up a singeton / interface whatever to make it easy for any part of the application to return a 400


            return StatusCode(200);
        }



        private ISettingsService settings;

        private void GetSettings(ISettingsService _settings)
        {
            settings = _settings;
        }

    }
}
