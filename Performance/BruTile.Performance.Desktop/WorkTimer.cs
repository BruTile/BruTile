using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace BruTile.Performance.Desktop
{
    class WorkTimer
    {
        private long _maxTime = long.MinValue;
        private long _minTime = long.MaxValue;
        private long _totalTime;
        private int _threadCompleteCount;
        private readonly int _testCount;
        private readonly object _syncRoot = new object();

        public long MaxTime { get { return _maxTime; } }
        public long MinTime { get { return _minTime; } }
        public long TotalTime { get { return _totalTime; } }

        public WorkTimer(int testCount)
        {
            _testCount = testCount;
        }

        public void TimeWork(Action work)
        {
            for (int i = 0; i < _testCount; i++)
            {
                Task.Run(() => TimeSingleWork(work));
            }
        }

        public void TimeWork<T>(Func<int, T> argFactory, Action<T> work)
        {
            for (var i = 0; i < _testCount; i++)
            {
                var arg = argFactory(i);
                Task.Run(() => TimeSingleWork(() => work(arg)));
            }
        }

        public void WaitForTestsToComplete()
        {
            while (Interlocked.CompareExchange(ref _threadCompleteCount, 0, _testCount) != _testCount)
            {
                Thread.Sleep(50);
            }
        }

        private void TimeSingleWork(Action action)
        {
            var stopwatch = Stopwatch.StartNew();
            try
            {
                action();
            }
            finally
            {
                stopwatch.Stop();
                lock (_syncRoot)
                {
                    _totalTime += stopwatch.ElapsedMilliseconds;
                    _minTime = Math.Min(_minTime, stopwatch.ElapsedMilliseconds);
                    _maxTime = Math.Max(_maxTime, stopwatch.ElapsedMilliseconds);
                }
                Interlocked.Increment(ref _threadCompleteCount);
            }
        }
    }
}
