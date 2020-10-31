using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Avatar_API.Controllers
{
    [Route("api/[controller]")]
    [Produces("application/json")]
    [ApiController]
    public class AvatarController : ControllerBase
    {

        /// <summary>
        /// Returns if avatar api is online.
        /// </summary>
        /// <remarks>
        /// </remarks>
        [HttpGet("status")]
        [ProducesResponseType(200)]
        public ActionResult GetStatus()
        {
            return Ok("ok");
        }
    }
}
