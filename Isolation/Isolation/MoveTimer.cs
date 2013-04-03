﻿using System;
using System.Diagnostics;

namespace Isolation
{
    public class MoveTimer
    {
        #region singleton

        private static readonly Lazy<MoveTimer> Singleton = new Lazy<MoveTimer>(() => new MoveTimer());
        public static MoveTimer I { get { return Singleton.Value; } }

        #endregion

        private readonly Stopwatch _sw;
        private TimeSpan _timeout;

        public MoveTimer()
        {
            _sw = new Stopwatch();
        }

        // set time allowed to calculate move
        public void SetTimeout(TimeSpan time)
        {
            _timeout = time;
        }

        public void StartTimer()
        {
            _sw.Restart();
        }

        public void StopTimer()
        {
            _sw.Stop();
        }

        public void ResetTimer()
        {
            _sw.Reset();
        }

        public TimeSpan GetTimeElapsed()
        {
            return _sw.Elapsed;
        }

        public double GetPercentOfTimeRemaining()
        {
            return (_timeout.TotalMilliseconds - _sw.ElapsedMilliseconds) / _timeout.TotalMilliseconds;
        }

        public bool Timeout()
        {
            return GetPercentOfTimeRemaining() < 0.01;
        }
    }
}