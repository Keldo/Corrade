///////////////////////////////////////////////////////////////////////////
//  Copyright (C) Wizardry and Steamworks 2013 - License: GNU GPLv3      //
//  Please see: http://www.gnu.org/licenses/gpl.html for legal details,  //
//  rights of fair usage, the disclaimer and warranty conditions.        //
///////////////////////////////////////////////////////////////////////////

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;

namespace wasSharp.Collections.Specialized
{
    ///////////////////////////////////////////////////////////////////////////
    //    Copyright (C) 2014 Wizardry and Steamworks - License: GNU GPLv3    //
    ///////////////////////////////////////////////////////////////////////////
    /// <summary>
    ///     An implementation of an observable HashSet.
    /// </summary>
    /// <typeparam name="T">the object type</typeparam>
    public class ObservableHashSet<T> : ICollection<T>, INotifyCollectionChanged, IEnumerable<T>
    {
        private readonly HashSet<T> store = new HashSet<T>();

        public ObservableHashSet(HashSet<T> set)
        {
            UnionWith(set);
        }

        public ObservableHashSet()
        {
        }

        public ObservableHashSet(T item)
        {
            Add(item);
        }

        public ObservableHashSet(ObservableHashSet<T> other)
        {
            UnionWith(other);
        }

        public ObservableHashSet(IEnumerable<T> list)
        {
            UnionWith(list);
        }

        public bool IsVirgin { get; private set; } = true;

        public IEnumerator<T> GetEnumerator()
        {
            return store.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Add(T item)
        {
            store.Add(item);
            IsVirgin = false;
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item));
        }

        public void Clear()
        {
            store.Clear();
            if (!IsVirgin)
                OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
            IsVirgin = false;
        }

        public bool Contains(T item)
        {
            return store.Contains(item);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            store.CopyTo(array, arrayIndex);
        }

        public bool Remove(T item)
        {
            var removed = store.Remove(item);
            IsVirgin = false;
            if (removed)
                OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, item));
            return removed;
        }

        public int Count => store.Count;

        public bool IsReadOnly => false;

        public event NotifyCollectionChangedEventHandler CollectionChanged;

        public void UnionWith(IEnumerable<T> list)
        {
            var added = new List<T>(list.Except(store));
            store.UnionWith(added);
            if (!IsVirgin && added.Any())
                OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, added));
            IsVirgin = false;
        }

        private void OnCollectionChanged(NotifyCollectionChangedEventArgs args)
        {
            CollectionChanged?.Invoke(this, args);
        }

        public void ExceptWith(IEnumerable<T> list)
        {
            var removed = new List<T>(list.Intersect(store));
            store.ExceptWith(removed);
            if (!IsVirgin && removed.Any())
                OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove,
                    removed));
            IsVirgin = false;
        }

        public void RemoveWhere(Func<T, bool> func)
        {
            var removed = new List<T>(store.Where(func));
            store.ExceptWith(removed);
            if (!IsVirgin && removed.Any())
                OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove,
                    removed));
            IsVirgin = false;
        }

        public IEnumerable<T> AsEnumerable()
        {
            return store.AsEnumerable();
        }
    }
}