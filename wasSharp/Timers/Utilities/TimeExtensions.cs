///////////////////////////////////////////////////////////////////////////
//  Copyright (C) Wizardry and Steamworks 2013 - License: GNU GPLv3      //
//  Please see: http://www.gnu.org/licenses/gpl.html for legal details,  //
//  rights of fair usage, the disclaimer and warranty conditions.        //
///////////////////////////////////////////////////////////////////////////

using System;

namespace wasSharp.Timers.Utilities
{
    public static class TimeExtensions
    {
        /// <summary>
        ///     Convert an Unix timestamp to a DateTime structure.
        /// </summary>
        /// <param name="unixTimestamp">the Unix timestamp to convert</param>
        /// <returns>the DateTime structure</returns>
        /// <remarks>the function assumes UTC time</remarks>
        public static DateTime UnixTimestampToDateTime(this uint unixTimestamp)
        {
            return new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc).AddSeconds(unixTimestamp).ToUniversalTime();
        }

        /// <summary>
        ///     Convert a DateTime structure to a Unix timestamp.
        /// </summary>
        /// <param name="dateTime">the DateTime structure to convert</param>
        /// <returns>the Unix timestamp</returns>
        /// <remarks>the function assumes UTC time</remarks>
        public static uint DateTimeToUnixTimestamp(this DateTime dateTime)
        {
            return (uint) (DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc)).TotalSeconds;
        }
    }
}