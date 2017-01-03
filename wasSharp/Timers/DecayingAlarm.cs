///////////////////////////////////////////////////////////////////////////
//  Copyright (C) Wizardry and Steamworks 2013 - License: GNU GPLv3      //
//  Please see: http://www.gnu.org/licenses/gpl.html for legal details,  //
//  rights of fair usage, the disclaimer and warranty conditions.        //
///////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Xml.Serialization;

namespace wasSharp.Timers
{
    ///////////////////////////////////////////////////////////////////////////
    //  Copyright (C) Wizardry and Steamworks 2013 - License: GNU GPLv3      //
    ///////////////////////////////////////////////////////////////////////////
    /// <summary>
    ///     An alarm class similar to the UNIX alarm with the added benefit
    ///     of a decaying timer that tracks the time between rescheduling.
    /// </summary>
    /// <remarks>
    ///     (C) Wizardry and Steamworks 2013 - License: GNU GPLv3
    /// </remarks>
    public class DecayingAlarm : IDisposable
    {
        [Flags]
        public enum DECAY_TYPE
        {
            [Reflection.NameAttribute("none")] [XmlEnum(Name = "none")] NONE = 0,
            [Reflection.NameAttribute("arithmetic")] [XmlEnum(Name = "arithmetic")] ARITHMETIC = 1,
            [Reflection.NameAttribute("geometric")] [XmlEnum(Name = "geometric")] GEOMETRIC = 2,
            [Reflection.NameAttribute("harmonic")] [XmlEnum(Name = "harmonic")] HARMONIC = 4,
            [Reflection.NameAttribute("weighted")] [XmlEnum(Name = "weighted")] WEIGHTED = 5
        }

        private readonly DECAY_TYPE decay = DECAY_TYPE.NONE;
        private readonly Stopwatch elapsed = new Stopwatch();
        private readonly object LockObject = new object();
        private readonly HashSet<double> times = new HashSet<double>();
        private Timer alarm;

        /// <summary>
        ///     The default constructor using no decay.
        /// </summary>
        public DecayingAlarm()
        {
            Signal = new ManualResetEvent(false);
        }

        /// <summary>
        ///     The constructor for the DecayingAlarm class taking as parameter a decay type.
        /// </summary>
        /// <param name="decay">the type of decay: arithmetic, geometric, harmonic, heronian or quadratic</param>
        public DecayingAlarm(DECAY_TYPE decay)
        {
            Signal = new ManualResetEvent(false);
            this.decay = decay;
        }

        public ManualResetEvent Signal { get; set; }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~DecayingAlarm()
        {
            Dispose(false);
        }

        public void Alarm(double deadline)
        {
            lock (LockObject)
            {
                switch (alarm == null)
                {
                    case true:
                        elapsed.Start();
                        alarm = new Timer(() =>
                        {
                            lock (LockObject)
                            {
                                Signal.Set();
                                elapsed.Stop();
                                times.Clear();
                                alarm.Dispose();
                                alarm = null;
                            }
                        }, deadline, 0);
                        return;
                    case false:
                        elapsed.Stop();
                        times.Add(elapsed.ElapsedMilliseconds);
                        switch (decay)
                        {
                            case DECAY_TYPE.ARITHMETIC:
                                alarm?.Change(
                                    (int) ((deadline + times.Aggregate((a, b) => b + a))/(1f + times.Count)), 0);
                                break;
                            case DECAY_TYPE.GEOMETRIC:
                                alarm?.Change((int) Math.Pow(deadline*times.Aggregate((a, b) => b*a),
                                    1f/(1f + times.Count)), 0);
                                break;
                            case DECAY_TYPE.HARMONIC:
                                alarm?.Change((int) ((1f + times.Count)/
                                                     (1f/deadline + times.Aggregate((a, b) => 1f/b + 1f/a))), 0);
                                break;
                            case DECAY_TYPE.WEIGHTED:
                                var d = new HashSet<double>(times) {deadline};
                                var total = d.Aggregate((a, b) => b + a);
                                alarm?.Change(
                                    (int) d.Aggregate((a, b) => Math.Pow(a, 2)/total + Math.Pow(b, 2)/total), 0);
                                break;
                            default:
                                alarm?.Change((int) deadline, 0);
                                break;
                        }
                        elapsed.Reset();
                        elapsed.Start();
                        break;
                }
            }
        }

        private void Dispose(bool dispose)
        {
            if (alarm != null)
            {
                alarm.Dispose();
                alarm = null;
            }
        }

        public DecayingAlarm Clone()
        {
            return new DecayingAlarm(decay);
        }
    }
}