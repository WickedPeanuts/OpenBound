using CryptSharp.Core;
using OpenBound_Network_Object_Library.Database.Context;
using OpenBound_Network_Object_Library.Models;
using OpenBound_Network_Object_Library.Session;
using System;
using System.Collections.Generic;
using System.Data.Odbc;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms.VisualStyles;

namespace Avatar_API.Data.Services
{
    public class UserService
    {
        private readonly OpenBoundDatabaseContext _odc;
        public UserService(OpenBoundDatabaseContext odc)
        {
            _odc = odc;
        }

        public SessionUser UserLogin(string email, string password)
        {
            email = email.ToLower();
            Player tmpPlayer = _odc.Players.FirstOrDefault((x) => x.Email.ToLower() == email);

            if (tmpPlayer == null || !Crypter.CheckPassword(password, tmpPlayer.Password)) return null;

            return new SessionUser()
            {
                Email = tmpPlayer.Email.ToLower(),
                Nickname = tmpPlayer.Nickname.ToLower()
            };
        }

    }
}
