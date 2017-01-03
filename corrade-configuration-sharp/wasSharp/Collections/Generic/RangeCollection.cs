///////////////////////////////////////////////////////////////////////////
//  Copyright (C) Wizardry and Steamworks 2013 - License: GNU GPLv3      //
//  Please see: http://www.gnu.org/licenses/gpl.html for legal details,  //
//  rights of fair usage, the disclaimer and warranty conditions.        //
///////////////////////////////////////////////////////////////////////////

using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace wasSharp.Collections.Generic
{
    ///////////////////////////////////////////////////////////////////////////
    //    Copyright (C) 2016 Wizardry and Steamworks - License: GNU GPLv3    //
    ///////////////////////////////////////////////////////////////////////////
    /// <summary>
    ///     A collection that maps ranges to values with O(1) complexity
    ///     lookups and O(n) insertions.
    /// </summary>
    /// <typeparam name="T">the type of value to store</typeparam>
    public class RangeCollection<T> : IEnumerable
    {
        private readonly Dictionary<int, T> map;

        public RangeCollection(int min, int max)
        {
            map = new Dictionary<int, T>(max - min);
        }

        public T this[int x]
        {
            get
            {
                T value;
                return map.TryGetValue(x, out value) ? value : default(T);
            }
        }

        public IEnumerator GetEnumerator()
        {
            return ((IEnumerable) map).GetEnumerator();
        }

        /// <summary>
        ///     Map a value to a range.
        /// </summary>
        /// <param name="Value">the value for the range</param>
        /// <param name="min">the minimal range</param>
        /// <param name="max">the maximal range</param>
        public void Add(T Value, int min, int max)
        {
            foreach (var i in Enumerable.Range(min, max - min + 1))
            {
                map.Add(i, Value);
            }
        }
    }
}