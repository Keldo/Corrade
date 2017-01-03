///////////////////////////////////////////////////////////////////////////
//  Copyright (C) Wizardry and Steamworks 2016 - License: GNU GPLv3      //
//  Please see: http://www.gnu.org/licenses/gpl.html for legal details,  //
//  rights of fair usage, the disclaimer and warranty conditions.        //
///////////////////////////////////////////////////////////////////////////

using System;
using System.Text;
using System.Threading;
using wasSharp.Collections.Generic;

namespace wasSharpNET.Console
{
    public class ConsoleSpin : IDisposable
    {
        private static readonly CircularQueue<string> spinArt =
            new CircularQueue<string>(new[] {".oOo", "oOo.", "Oo.o", "o.oO"});

        private static readonly ManualResetEvent spinEvent = new ManualResetEvent(false);
        private static Thread spinThread;
        private static bool run = true;

        public ConsoleSpin()
        {
            spinThread = new Thread(() =>
            {
                do
                {
                    spinEvent.WaitOne();
                    Thread.Sleep(100);

                    var deco = spinArt.Dequeue();
                    System.Console.Write(deco);
                    foreach (var c in deco)
                        System.Console.Write("\b");

                } while (run);
            })
            {
                IsBackground = true
            };
            spinThread.Start();
        }

        public void Dispose()
        {
            // Stop the callback thread.
            try
            {
                run = false;
                spinEvent.Set();
                if ((!spinThread.ThreadState.Equals(ThreadState.Running) &&
                     !spinThread.ThreadState.Equals(ThreadState.WaitSleepJoin)) || spinThread.Join(1000)) return;
                spinThread.Abort();
                spinThread.Join();
            }
            catch (Exception)
            {
                /* We are going down and we do not care. */
            }
            finally
            {
                spinThread = null;
            }
        }

        public void Start()
        {
            spinEvent.Set();
        }

        public void Stop()
        {
            spinEvent.Reset();
        }
    }
}