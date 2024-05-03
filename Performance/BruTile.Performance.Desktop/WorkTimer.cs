// Copyright (c) BruTile developers team. All rights reserved. See License.txt in the project root for license information.

using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace BruTile.Performance.Desktop;

internal class WorkTimer(int testCount)
{
    private int _threadCompleteCount;
    private readonly int _testCount = testCount;
    private readonly object _syncRoot = new();

    public long MaxTime { get; private set; } = long.MinValue;
    public long MinTime { get; private set; } = long.MaxValue;
    public long TotalTime { get; private set; }

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
                TotalTime += stopwatch.ElapsedMilliseconds;
                MinTime = Math.Min(MinTime, stopwatch.ElapsedMilliseconds);
                MaxTime = Math.Max(MaxTime, stopwatch.ElapsedMilliseconds);
            }
            Interlocked.Increment(ref _threadCompleteCount);
        }
    }
}
