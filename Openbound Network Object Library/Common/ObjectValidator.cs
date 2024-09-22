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

using OpenBound_Network_Object_Library.Entity.Text;
using System;
using System.Collections.Generic;

namespace OpenBound_Network_Object_Library.Common
{
    public class ObjectValidator
    {
        public static bool ValidateString(string Param)
        {
            return Param != null && Param.Length > 0;
        }

        public static bool WasBeforeDate(DateTime Param, DateTime ComparingDate)
        {
            return Param.CompareTo(ComparingDate) == -1;
        }

        public static bool WasAfterDate(DateTime Param, DateTime ComparingDate)
        {
            return Param.CompareTo(ComparingDate) == 1;
        }

        public static bool WasOnSameDate(DateTime Param, DateTime ComparingDate)
        {
            return Param.Day == ComparingDate.Day &&
                Param.Month == ComparingDate.Month &&
                Param.Year == ComparingDate.Year;
        }

        public static bool ValidateList<T>(List<T> Param)
        {
            return Param != null && Param.Count > 0;
        }

        public static bool ValidateAndModify(PlayerMessage playerMessage)
        {
            if (playerMessage.Text == null) return false;

            playerMessage.Text = playerMessage.Text.Trim();

            if (playerMessage.Text.Length == 0) return false;

            return true;
        }
    }
}