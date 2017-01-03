///////////////////////////////////////////////////////////////////////////
//  Copyright (C) Wizardry and Steamworks 2013 - License: GNU GPLv3      //
//  Please see: http://www.gnu.org/licenses/gpl.html for legal details,  //
//  rights of fair usage, the disclaimer and warranty conditions.        //
///////////////////////////////////////////////////////////////////////////

using System;
using System.Threading;
using System.Threading.Tasks;

namespace wasSharp.Timers
{
    public class Timer : IDisposable
    {
        private readonly Task CompletedTask = Task.FromResult(false);
        private Task Delay;
        private bool Disposed;

        private CancellationTokenSource tokenSource;

        public Timer()
        {
        }

        public Timer(Action callback, TimeSpan dueTime, TimeSpan period) : this()
        {
            Callback = callback;
            DueTime = dueTime;
            Period = period;

            Start();
        }

        public Timer(Action callback, int dueTime, int period)
            : this(callback, TimeSpan.FromMilliseconds(dueTime), TimeSpan.FromMilliseconds(period))
        {
        }

        public Timer(Action callback) : this(callback, TimeSpan.Zero, TimeSpan.Zero)
        {
        }

        public Timer(Action callback, double dueTime, int period)
            : this(callback, TimeSpan.FromMilliseconds(dueTime), TimeSpan.FromMilliseconds(period))
        {
        }

        public Timer(Action callback, uint dueTime, uint period)
            : this(callback, TimeSpan.FromMilliseconds(dueTime), TimeSpan.FromMilliseconds(period))
        {
        }

        public Action Callback { set; get; }

        public TimeSpan DueTime { get; set; } = TimeSpan.Zero;

        public TimeSpan Period { get; set; } = TimeSpan.Zero;

        public void Dispose()
        {
            // Stop the timer.
            Stop();

            // Dispose the token.
            if (tokenSource != null)
            {
                tokenSource.Cancel();
                tokenSource.Dispose();
                tokenSource = null;
            }

            // Set the disposed flag.
            Disposed = true;
        }

        private void Start()
        {
            // Check if we have an installed callback and that there is at least a due time.
            if (Callback == null || DueTime.Equals(TimeSpan.Zero))
                return;

            // Dispose the previous token source.
            if (tokenSource != null)
            {
                tokenSource.Cancel();
                tokenSource.Dispose();
                tokenSource = null;
            }

            // Create a new cancellation source.
            tokenSource = new CancellationTokenSource();

            Action tick = null;

            tick = () =>
            {
                Task.Run(Callback, tokenSource.Token);
                if (Disposed)
                    return;
                Delay = !Period.Equals(TimeSpan.Zero) ? Task.Delay(Period, tokenSource.Token) : CompletedTask;
                if (Disposed || Delay.IsCompleted)
                    return;
                Delay.ContinueWith(o => tick(), tokenSource.Token);
            };

            Delay = !DueTime.Equals(TimeSpan.Zero) ? Task.Delay(DueTime, tokenSource.Token) : CompletedTask;
            if (Disposed || Delay.IsCompleted)
                return;
            Delay.ContinueWith(o => tick(), tokenSource.Token);
        }

        public void Change(int dueTime, int period)
        {
            Change(TimeSpan.FromMilliseconds(dueTime), TimeSpan.FromMilliseconds(period));
        }

        public void Change(uint dueTime, int period)
        {
            Change(TimeSpan.FromMilliseconds(dueTime), TimeSpan.FromMilliseconds(period));
        }

        public void Change(TimeSpan dueTime, TimeSpan period)
        {
            DueTime = dueTime;
            Period = period;

            Start();
        }

        public void Stop()
        {
            Change(0, 0);
        }
    }
}