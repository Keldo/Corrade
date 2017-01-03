///////////////////////////////////////////////////////////////////////////
//  Copyright (C) Wizardry and Steamworks 2016 - License: GNU GPLv3      //
//  Please see: http://www.gnu.org/licenses/gpl.html for legal details,  //
//  rights of fair usage, the disclaimer and warranty conditions.        //
///////////////////////////////////////////////////////////////////////////

using System;
using System.Text;

namespace wasSharpNET.Cryptography
{
    public static class SHA1
    {
        /// <summary>
        ///     Return a 40 character hex representation of a SHA1 hash.
        /// </summary>
        /// <param name="sha1">the SHA1 hash object</param>
        public static string ToHex(this System.Security.Cryptography.SHA1 sha1, byte[] data)
        {
            return BitConverter.ToString(sha1.ComputeHash(data)).Replace("-", "");
        }

        public static string ToHex(this System.Security.Cryptography.SHA1 sha1, string data)
        {
            return BitConverter.ToString(sha1.ComputeHash(Encoding.UTF8.GetBytes(data))).Replace("-", "");
        }
    }
}