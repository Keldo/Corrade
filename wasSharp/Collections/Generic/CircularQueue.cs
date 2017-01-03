///////////////////////////////////////////////////////////////////////////
//  Copyright (C) Wizardry and Steamworks 2013 - License: GNU GPLv3      //
//  Please see: http://www.gnu.org/licenses/gpl.html for legal details,  //
//  rights of fair usage, the disclaimer and warranty conditions.        //
///////////////////////////////////////////////////////////////////////////

using System.Collections.Generic;

namespace wasSharp.Collections.Generic
{
    ///////////////////////////////////////////////////////////////////////////
    //    Copyright (C) 2016 Wizardry and Steamworks - License: GNU GPLv3    //
    ///////////////////////////////////////////////////////////////////////////
    /// <summary>
    ///     A circular queue implementation based on linked lists.
    /// </summary>
    /// <typeparam name="T">the type of value to store</typeparam>
    public class CircularQueue<T>
    {
        private readonly LinkedList<T> Store = new LinkedList<T>();

        private readonly object SyncRoot = new object();
        private LinkedListNode<T> CurrentNode;

        public CircularQueue()
        {
        }

        public CircularQueue(IEnumerable<T> items)
        {
            Enqueue(items);
        }

        public CircularQueue(CircularQueue<T> queue)
        {
            lock (SyncRoot)
            {
                lock (queue.SyncRoot)
                {
                    foreach (var item in queue.Items)
                    {
                        Store.AddLast(item);
                    }

                    if (CurrentNode == null)
                        CurrentNode = Store.First;
                }
            }
        }

        public int Count
        {
            get
            {
                lock (SyncRoot)
                {
                    return Store.Count;
                }
            }
        }

        private T GetNext
        {
            get
            {
                lock (SyncRoot)
                {
                    if (CurrentNode == null)
                        return default(T);

                    var value = CurrentNode.Value;

                    switch (CurrentNode.Next != null)
                    {
                        case true:
                            CurrentNode = CurrentNode.Next;
                            break;
                        default:
                            CurrentNode = Store.First;
                            break;
                    }

                    return value;
                }
            }
        }

        public IEnumerable<T> Items
        {
            get
            {
                lock (SyncRoot)
                {
                    if (CurrentNode == null)
                        yield break;

                    var node = CurrentNode;
                    do
                    {
                        yield return node.Value;
                        node = node.Next;
                    } while (node != null);
                }
            }
        }

        public void Enqueue(IEnumerable<T> items)
        {
            lock (SyncRoot)
            {
                foreach (var i in items)
                    Store.AddLast(i);

                if (CurrentNode == null)
                    CurrentNode = Store.First;
            }
        }

        public void Enqueue(T item)
        {
            lock (SyncRoot)
            {
                Store.AddLast(item);

                if (CurrentNode == null)
                    CurrentNode = Store.First;
            }
        }

        public T Dequeue()
        {
            lock (SyncRoot)
            {
                return GetNext;
            }
        }

        public IEnumerable<T> Dequeue(int count = 1)
        {
            if (count <= 0)
                yield break;

            lock (SyncRoot)
            {
                if (CurrentNode == null)
                    yield break;

                do
                {
                    yield return GetNext;
                } while (--count != 0);
            }
        }

        public void Clear()
        {
            lock (SyncRoot)
            {
                Store.Clear();

                CurrentNode = null;
            }
        }

        public bool Contains(T item)
        {
            lock (SyncRoot)
            {
                return Store.Contains(item);
            }
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            lock (SyncRoot)
            {
                Store.CopyTo(array, arrayIndex);
            }
        }

        public bool Remove(T item)
        {
            lock (SyncRoot)
            {
                var node = Store.Find(item);
                if (node == null)
                    return false;
                if (CurrentNode.Equals(node))
                {
                    switch (node.Next != null)
                    {
                        case true:
                            CurrentNode = node.Next;
                            break;
                        default:
                            CurrentNode = Store.First;
                            break;
                    }
                }
                Store.Remove(node);
                return true;
            }
        }

        public void RemoveAll(IEnumerable<T> items)
        {
            var itemSet = new HashSet<T>(items);
            lock (SyncRoot)
            {
                var node = CurrentNode;
                do
                {
                    var next = node.Next;
                    if (itemSet.Contains(node.Value))
                    {
                        switch (next != null)
                        {
                            case true:
                                CurrentNode = next;
                                break;
                            default:
                                CurrentNode = Store.First;
                                break;
                        }
                        Store.Remove(node);
                    }
                    node = next;
                } while (node != null);
            }
        }
    }
}