using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OpenBound_Network_Object_Library.Session;
using SharpDX.XInput;

namespace Avatar_API.Session
{
    public class SessionHandler
    {
        IHttpContextAccessor _httpContextAccessor;
    
        private const string SessionKeyEmail = "_Email";
        private const string SessionKeyNickname = "_Nickname";

        public SessionHandler(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public void StartSession(SessionUser sessionUser)
        {
            _httpContextAccessor.HttpContext.Session.SetString(SessionKeyEmail, sessionUser.Email.ToLower());
            _httpContextAccessor.HttpContext.Session.SetString(SessionKeyNickname, sessionUser.Nickname.ToLower());
        }

        public bool IsAuthenticated()
        {
            SessionUser su = GetSessionUser();
            return su.IsAuthenticated();
        }

        public SessionUser GetSessionUser()
        {
       
            string email = _httpContextAccessor.HttpContext.Session.GetString(SessionKeyEmail);
            string nickname = _httpContextAccessor.HttpContext.Session.GetString(SessionKeyNickname);

            return new SessionUser()
            {
                Email = email,
                Nickname = nickname
            };
        }

        public void EndSession()
        {
            _httpContextAccessor.HttpContext.Session.SetString(SessionKeyEmail, null);
            _httpContextAccessor.HttpContext.Session.SetString(SessionKeyNickname, null);
        }


    }
}
