///////////////////////////////////////////////////////////////////////////
//  Copyright (C) Wizardry and Steamworks 2013 - License: GNU GPLv3      //
//  Please see: http://www.gnu.org/licenses/gpl.html for legal details,  //
//  rights of fair usage, the disclaimer and warranty conditions.        //
///////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Linq;
using wasSharp.Collections.Specialized;

namespace wasSharp.Collections.Utilities
{
    public static class CollectionExtensions
    {
        /// <summary>
        ///     Compares two dictionaries for equality.
        /// </summary>
        /// <typeparam name="TKey">key type</typeparam>
        /// <typeparam name="TValue">value type</typeparam>
        /// <param name="dictionary">dictionary to compare</param>
        /// <param name="otherDictionary">dictionary to compare to</param>
        /// <returns>true if the dictionaries contain the same elements</returns>
        public static bool ContentEquals<TKey, TValue>(this IDictionary<TKey, TValue> dictionary,
            IDictionary<TKey, TValue> otherDictionary)
        {
            return
                (dictionary ?? new Dictionary<TKey, TValue>()).Count.Equals(
                    (otherDictionary ?? new Dictionary<TKey, TValue>()).Count) &&
                (otherDictionary ?? new Dictionary<TKey, TValue>())
                    .OrderBy(kvp => kvp.Key)
                    .SequenceEqual((dictionary ?? new Dictionary<TKey, TValue>())
                        .OrderBy(kvp => kvp.Key));
        }

        public static void AddOrReplace<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, TValue value)
        {
            switch (dictionary.ContainsKey(key))
            {
                case true:
                    dictionary[key] = value;
                    break;
                default:
                    dictionary.Add(key, value);
                    break;
            }
        }

        public static void AddIfNotExists<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, TValue value)
        {
            switch (!dictionary.ContainsKey(key))
            {
                case true:
                    dictionary.Add(key, value);
                    break;
            }
        }

        public static MultiKeyDictionary<K1, K2, V> ToMultiKeyDictionary<S, K1, K2, V>(this IEnumerable<S> items, Func<S, K1> key1, Func<S, K2> key2, Func<S, V> value)
        {
            var dict = new MultiKeyDictionary<K1, K2, V>();
            foreach (S i in items)
            {
                dict.Add(key1(i), key2(i), value(i));
            }
            return dict;
        }
    }
}