using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Avatar_API.Data.Models;
using Avatar_API.Data.Services;
using Avatar_API.Session;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OpenBound_Network_Object_Library.Database.Controller;
using OpenBound_Network_Object_Library.Models;
using OpenBound_Network_Object_Library.Session;

namespace Avatar_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthorizationController : ControllerBase
    {
        private readonly SessionHandler _sessionHandler;
        private readonly UserService _userService;
        public AuthorizationController(UserService userService, SessionHandler sessionHandler)
        {
            _sessionHandler = sessionHandler;
            _userService = userService;
        }

        /// <summary>
        /// Authenticates a player.
        /// </summary>
        /// <remarks>
        /// </remarks>
        [HttpPost("login")]
        [ProducesResponseType(200)]
        public ActionResult<SessionUser> Login([FromBody] AuthenticateModel authenticateModel)
        {
            if (_sessionHandler.IsAuthenticated())
                return BadRequest("You are already logged in.");

            SessionUser sessionUser = _userService.UserLogin(authenticateModel.Email, authenticateModel.Password);

            if (sessionUser == null)
                return BadRequest("Invalid Credentials.");

            _sessionHandler.StartSession(sessionUser);
            Console.WriteLine($"User {sessionUser.Nickname} has logged in.");
            return Ok(_sessionHandler.GetSessionUser());
        }

        /// <summary>
        /// Ends the session.
        /// </summary>
        /// <remarks>
        /// </remarks>
        [HttpPost("logout")]
        [ProducesResponseType(200)]
        public ActionResult Logout()
        {
            if (!_sessionHandler.IsAuthenticated())
                return BadRequest("You are not logged in.");

            _sessionHandler.EndSession();
            return Ok($"Your session has been terminated.");
        }
    }
}
