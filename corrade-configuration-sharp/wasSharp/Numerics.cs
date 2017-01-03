﻿///////////////////////////////////////////////////////////////////////////
//  Copyright (C) Wizardry and Steamworks 2013 - License: GNU GPLv3      //
//  Please see: http://www.gnu.org/licenses/gpl.html for legal details,  //
//  rights of fair usage, the disclaimer and warranty conditions.        //
///////////////////////////////////////////////////////////////////////////

namespace wasSharp
{
    public static class Numerics
    {
        ///////////////////////////////////////////////////////////////////////////
        //  Copyright (C) Wizardry and Steamworks 2015 - License: GNU GPLv3      //
        ///////////////////////////////////////////////////////////////////////////
        /// <summary>
        ///     Given a value in a source value range and a target range, map
        ///     the value from the source range into the target range.
        /// </summary>
        /// <remarks>
        ///     value - the value to map
        ///     xMin - the lower bound of the source range
        ///     xMax - the upper bound of the source range
        ///     yMin - the lower bound of the target range
        ///     yMax - the upper bound of the target range
        /// </remarks>
        /// <returns>a value in x mapped in the range of y</returns>
        public static double MapValueToRange(double value, double xMin, double xMax, double yMin, double yMax)
        {
            return yMin + (yMax - yMin)*(value - xMin)/(xMax - xMin);
        }
    }
}