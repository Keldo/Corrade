///////////////////////////////////////////////////////////////////////////
//  Copyright (C) Wizardry and Steamworks 2013 - License: GNU GPLv3      //
//  Please see: http://www.gnu.org/licenses/gpl.html for legal details,  //
//  rights of fair usage, the disclaimer and warranty conditions.        //
///////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Linq;

namespace wasSharp.Linq
{
    public static class Extensions
    {
        ///////////////////////////////////////////////////////////////////////////
        //    Copyright (C) 2016 Wizardry and Steamworks - License: GNU GPLv3    //
        ///////////////////////////////////////////////////////////////////////////
        /// <summary>
        ///     Returns true of an enumerable contains more than one element.
        /// </summary>
        /// <typeparam name="T">the type of the enumeration</typeparam>
        /// <param name="e">the enumeration</param>
        /// <returns>true if enumeration contains more than one element</returns>
        /// <remarks>O(2) worst case</remarks>
        public static bool Some<T>(this IEnumerable<T> e)
        {
            var i = 0;
            using (var iter = e.GetEnumerator())
            {
                while (iter.MoveNext())
                {
                    if (++i > 1)
                        return true;
                }
                return false;
            }
        }

        /// <summary>
        ///     Sequentially removes all the elements from the first sequence that are in the second sequence.
        /// </summary>
        /// <typeparam name="T">the type o the collection</typeparam>
        /// <param name="o">the first sequence to remove from</param>
        /// <param name="p">the second sequence to remove</param>
        /// <returns>the first sequence excluding the second sequence</returns>
        public static IEnumerable<T> SequenceExcept<T>(this IEnumerable<T> o, IEnumerable<T> p) where T : IEquatable<T>
        {
            var l = new List<T>(o);
            var r = new List<T>(p);
            return l.Count > r.Count
                ? l.Zip(r, (x, y) => x.Equals(y) ? default(T) : y)
                    .Concat(l.Skip(r.Count()))
                    .Where(q => q != null && !q.Equals(default(T)))
                : r.Zip(l, (x, y) => x.Equals(y) ? default(T) : y)
                    .Concat(r.Skip(l.Count()))
                    .Where(q => q != null && !q.Equals(default(T)));
        }
    }
}