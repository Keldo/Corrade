﻿///////////////////////////////////////////////////////////////////////////
//  Copyright (C) Wizardry and Steamworks 2013 - License: GNU GPLv3      //
//  Please see: http://www.gnu.org/licenses/gpl.html for legal details,  //
//  rights of fair usage, the disclaimer and warranty conditions.        //
///////////////////////////////////////////////////////////////////////////

using System;
using System.IO;
using System.Linq;

namespace wasSharp
{
    public static class Strings
    {
        ///////////////////////////////////////////////////////////////////////////
        //    Copyright (C) 2015 Wizardry and Steamworks - License: GNU GPLv3    //
        ///////////////////////////////////////////////////////////////////////////
        /// <summary>
        ///     Combine multiple paths.
        /// </summary>
        /// <param name="paths">an array of paths</param>
        /// <returns>a combined path</returns>
        public static string PathCombine(params string[] paths)
        {
            return paths.Aggregate((x, y) => Path.Combine(x, y));
        }

        ///////////////////////////////////////////////////////////////////////////
        //    Copyright (C) 2016 Wizardry and Steamworks - License: GNU GPLv3    //
        ///////////////////////////////////////////////////////////////////////////
        /// <summary>
        ///     Determines if two strings are equal.
        /// </summary>
        /// <param name="a">first string</param>
        /// <param name="b">second string</param>
        /// <param name="comparison">string comparison to use</param>
        /// <returns>true if the strings are equal</returns>
        public static bool StringEquals(string a, string b, StringComparison comparison)
        {
            return (a == null && b == null) || (a.Length == b.Length && string.Equals(a, b, comparison));
        }

        ///////////////////////////////////////////////////////////////////////////
        //    Copyright (C) 2016 Wizardry and Steamworks - License: GNU GPLv3    //
        ///////////////////////////////////////////////////////////////////////////
        /// <summary>
        ///     Determines if two strings are equal.
        /// </summary>
        /// <param name="a">first string</param>
        /// <param name="b">second string</param>
        /// <returns>true if the strings are equal</returns>
        public static bool StringEquals(string a, string b)
        {
            return (a == null && b == null) || (a != null && b != null && a.Length == b.Length && string.Equals(a, b));
        }
    }
}