using System;
using System.Collections.Generic;
using System.Text;

namespace OpenBound_Network_Object_Library.Session
{
    public class SessionUser
    {
        public string Email { get; set; }
        public string Nickname { get; set; }

        public bool IsAuthenticated()
        {
            return (!string.IsNullOrEmpty(Email) && !string.IsNullOrEmpty(Nickname));
        }
    }
}
