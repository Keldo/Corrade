///////////////////////////////////////////////////////////////////////////
//  Copyright (C) Wizardry and Steamworks 2016 - License: GNU GPLv3      //
//  Please see: http://www.gnu.org/licenses/gpl.html for legal details,  //
//  rights of fair usage, the disclaimer and warranty conditions.        //
///////////////////////////////////////////////////////////////////////////

using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;

namespace wasSharp.Collections.Specialized
{
    ///////////////////////////////////////////////////////////////////////////
    //    Copyright (C) 2016 Wizardry and Steamworks - License: GNU GPLv3    //
    ///////////////////////////////////////////////////////////////////////////
    /// <summary>
    ///     An implementation of an observable Dictionary.
    /// </summary>
    /// <typeparam name="K">the key type</typeparam>
    /// <typeparam name="V">the value type</typeparam>
    public class ObservableDictionary<K, V> : IDictionary<K, V>, INotifyCollectionChanged
    {
        private readonly Dictionary<K, V> store = new Dictionary<K, V>();

        public bool IsVirgin { get; private set; } = true;

        public V this[K key]
        {
            get { return store[key]; }

            set { store[key] = value; }
        }

        public int Count => store.Count;

        public bool IsReadOnly => false;

        public ICollection<K> Keys => store.Keys;

        public ICollection<V> Values => store.Values;

        public void Add(KeyValuePair<K, V> item)
        {
            ((IDictionary<K, V>) store).Add(item);
            IsVirgin = false;
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item));
        }

        public void Add(K key, V value)
        {
            store.Add(key, value);
            IsVirgin = false;
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add,
                new KeyValuePair<K, V>(key, value)));
        }

        public void Clear()
        {
            store.Clear();
            if (!IsVirgin)
                OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
            IsVirgin = false;
        }

        public bool Contains(KeyValuePair<K, V> item)
        {
            return ((IDictionary<K, V>) store).Contains(item);
        }

        public bool ContainsKey(K key)
        {
            return store.ContainsKey(key);
        }

        public void CopyTo(KeyValuePair<K, V>[] array, int arrayIndex)
        {
            ((IDictionary<K, V>) store).CopyTo(array, arrayIndex);
        }

        public IEnumerator<KeyValuePair<K, V>> GetEnumerator()
        {
            return ((IDictionary<K, V>) store).GetEnumerator();
        }

        public bool Remove(KeyValuePair<K, V> item)
        {
            var removed = ((IDictionary<K, V>) store).Remove(item);
            IsVirgin = false;
            if (removed)
                OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, item));
            return removed;
        }

        public bool Remove(K key)
        {
            KeyValuePair<K, V> item;
            if (store.ContainsKey(key))
                item = new KeyValuePair<K, V>(key, store[key]);

            var removed = store.Remove(key);
            IsVirgin = false;
            if (removed)
                OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, item));
            return removed;
        }

        public bool TryGetValue(K key, out V value)
        {
            return store.TryGetValue(key, out value);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IDictionary<K, V>) store).GetEnumerator();
        }

        public event NotifyCollectionChangedEventHandler CollectionChanged;

        private void OnCollectionChanged(NotifyCollectionChangedEventArgs args)
        {
            CollectionChanged?.Invoke(this, args);
        }
    }
}