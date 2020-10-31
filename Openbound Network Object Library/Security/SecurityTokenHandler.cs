/* 
 * Copyright (C) 2020, Carlos H.M.S. <carlos_judo@hotmail.com>
 * This file is part of OpenBound.
 * OpenBound is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License
 * as published by the Free Software Foundation, either version 3 of the License, or(at your option) any later version.
 * 
 * OpenBound is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty
 * of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.See the GNU General Public License for more details.
 * 
 * You should have received a copy of the GNU General Public License along with OpenBound. If not, see http://www.gnu.org/licenses/.
 */

using OpenBound_Network_Object_Library.Common;
using System;
using System.Linq;

namespace OpenBound_Network_Object_Library.Security
{
    public class SecurityTokenHandler
    {
        public static SecurityToken DecodeSecurityToken(string UnifiedSecurityToken)
        {
            SecurityToken securityToken = new SecurityToken();

            byte[] data = Convert.FromBase64String(UnifiedSecurityToken);

            //Date Handling
            securityToken.DateTime = DateTime.FromBinary(BitConverter.ToInt64(data, 0));

            //Token Handling
            byte[] value = new byte[16];
            Array.Copy(data, 8, value, 0, 16);
            securityToken.Token = Convert.ToBase64String(value);

            //Unified SecurityToken
            securityToken.UnifiedSecurityToken = UnifiedSecurityToken;

            return securityToken;
        }

        public static SecurityToken GenerateSecurityToken()
        {
            SecurityToken Token = new SecurityToken();

            //Date
            Token.DateTime = DateTime.UtcNow.AddMinutes(NetworkObjectParameters.SecurityTokenExpirationInMinutes);

            byte[] base64guid = Guid.NewGuid().ToByteArray();
            byte[] date = BitConverter.GetBytes(Token.DateTime.ToBinary());

            Token.Token = Convert.ToBase64String(base64guid.ToArray());
            Token.UnifiedSecurityToken = Convert.ToBase64String(date.Concat(base64guid).ToArray());

            return Token;
        }

        public static bool IsTokenDateValid(SecurityToken SecurityToken)
        {
            return DateTime.UtcNow <= SecurityToken.DateTime &&
                SecurityToken.DateTime <= DateTime.UtcNow.AddMinutes(NetworkObjectParameters.SecurityTokenExpirationInMinutes).AddSeconds(1);
        }

        public static bool IsTokenValid(SecurityToken ClientToken, SecurityToken ServerToken)
        {
            return IsTokenDateValid(ServerToken) &&
                ClientToken.Token == ServerToken.Token;
        }
    }
}
