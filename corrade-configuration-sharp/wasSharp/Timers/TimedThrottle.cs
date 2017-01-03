///////////////////////////////////////////////////////////////////////////
//  Copyright (C) Wizardry and Steamworks 2013 - License: GNU GPLv3      //
//  Please see: http://www.gnu.org/licenses/gpl.html for legal details,  //
//  rights of fair usage, the disclaimer and warranty conditions.        //
///////////////////////////////////////////////////////////////////////////

using System;

namespace wasSharp.Timers
{
    ///////////////////////////////////////////////////////////////////////////
    //  Copyright (C) Wizardry and Steamworks 2015 - License: GNU GPLv3      //
    ///////////////////////////////////////////////////////////////////////////
    /// <summary>
    ///     Given a number of allowed events per seconds, this class allows you
    ///     to determine via the IsSafe property whether it is safe to trigger
    ///     another lined-up event. This is mostly used to check that throttles
    ///     are being respected.
    /// </summary>
    public class TimedThrottle : IDisposable
    {
        private readonly uint EventsAllowed;
        private readonly object LockObject = new object();
        private Timer timer;
        public uint TriggeredEvents;

        public TimedThrottle(uint events, uint seconds)
        {
            EventsAllowed = events;
            if (timer == null)
            {
                timer = new Timer(() =>
                {
                    lock (LockObject)
                    {
                        TriggeredEvents = 0;
                    }
                }, seconds, seconds);
            }
        }

        public bool IsSafe
        {
            get
            {
                lock (LockObject)
                {
                    return ++TriggeredEvents <= EventsAllowed;
                }
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~TimedThrottle()
        {
            Dispose(false);
        }

        private void Dispose(bool dispose)
        {
            if (timer != null)
            {
                timer.Dispose();
                timer = null;
            }
        }
    }
}