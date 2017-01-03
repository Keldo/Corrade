///////////////////////////////////////////////////////////////////////////
//  Copyright (C) Wizardry and Steamworks 2016 - License: GNU GPLv3      //
//  Please see: http://www.gnu.org/licenses/gpl.html for legal details,  //
//  rights of fair usage, the disclaimer and warranty conditions.        //
///////////////////////////////////////////////////////////////////////////

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.ServiceModel.Syndication;
using System.Xml;
using wasSharp.Collections.Specialized;
using wasSharp.Timers;

namespace wasSharpNET.Syndication
{
    ///////////////////////////////////////////////////////////////////////////
    //    Copyright (C) 2016 Wizardry and Steamworks - License: GNU GPLv3    //
    ///////////////////////////////////////////////////////////////////////////
    /// <summary>
    ///     A Syndication implementation as an Observable collection.
    /// </summary>
    public class ObservableSyndication : ICollection<SyndicationItem>, INotifyCollectionChanged
    {
        private readonly ObservableDictionary<string, SyndicationItem> syndicationItems =
            new ObservableDictionary<string, SyndicationItem>();

        private readonly Timer syndicationPoll;
        private TimeSpan _defaultUpdateTime;
        private string URL;

        public ObservableSyndication(string URL, TimeSpan defaultUpdateTime)
        {
            // Assign update variables.
            _defaultUpdateTime = defaultUpdateTime;
            this.URL = URL;

            // Forward the collection change event.
            syndicationItems.CollectionChanged += (o, p) => { CollectionChanged?.Invoke(this, p); };

            // Poll the feed.
            syndicationPoll = new Timer(() =>
            {
                using (var reader = XmlReader.Create(URL))
                {
                    SyndicationFeed.Load(reader)?
                        .Items
                        .AsParallel()
                        .Where(o => o.PublishDate.CompareTo(
                            DateTimeOffset.UtcNow.Subtract(TimeSpan.FromDays(30)).Subtract(defaultUpdateTime)) > 0)
                        .ForAll(o =>
                        {
                            if (!syndicationItems.ContainsKey(o.Id))
                            {
                                syndicationItems.Add(o.Id, o);
                            }
                        });
                }
            }, defaultUpdateTime, defaultUpdateTime);
        }

        public ObservableSyndication(string URL, uint milliseconds) : this(URL, TimeSpan.FromMilliseconds(milliseconds))
        {
        }

        public TimeSpan Refresh
        {
            get { return _defaultUpdateTime; }
            set
            {
                _defaultUpdateTime = value;
                syndicationPoll.Change(_defaultUpdateTime, _defaultUpdateTime);
            }
        }

        public int Count => syndicationItems.Count;

        public bool IsReadOnly => false;

        public void Add(SyndicationItem item)
        {
            syndicationItems.Add(item.Id, item);
        }

        public void Clear()
        {
            syndicationItems.Clear();
        }

        public bool Contains(SyndicationItem item)
        {
            return syndicationItems.ContainsKey(item.Id) && syndicationItems[item.Id].Equals(item);
        }

        public void CopyTo(SyndicationItem[] array, int arrayIndex)
        {
            syndicationItems.Values.CopyTo(array, arrayIndex);
        }

        public bool Remove(SyndicationItem item)
        {
            return syndicationItems.Remove(item.Id);
        }

        public IEnumerator<SyndicationItem> GetEnumerator()
        {
            return syndicationItems.Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return syndicationItems.GetEnumerator();
        }

        public event NotifyCollectionChangedEventHandler CollectionChanged;
    }
}