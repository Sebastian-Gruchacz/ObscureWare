namespace Obscureware.DataFlow.Logging
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Threading;

    /// <summary>
    /// Simple performance monitoring routines, good for remote measurement of deployed code.
    /// </summary>
    public static class PerformanceMonitor
    {
        private static readonly ConcurrentDictionary<string, PerfInfo> _results = new ConcurrentDictionary<string, PerfInfo>();

        /// <summary>
        /// Register execution time under specific counter key
        /// </summary>
        /// <param name="key"></param>
        /// <param name="elapsedTicks"></param>
        public static void RegisterExecution(string key, long elapsedTicks)
        {
            PerfInfo info;
            if (!_results.TryGetValue(key, out info))
            {
                info = new PerfInfo(key);
                if (!_results.TryAdd(key, info))
                {
                    _results.TryGetValue(key, out info); // receive already stored object
                }
            }

            if (info != null)
            //lock (info)
            {
                Interlocked.Add(ref info.Events, 1);
                Interlocked.Add(ref info.TotalTicks, elapsedTicks);
            }
        }

        /// <summary>
        /// Measures execution time of given action under specific counter key
        /// </summary>
        /// <param name="key"></param>
        /// <param name="action"></param>
        public static void MeasureBlock(string key, Action action)
        {
            Stopwatch sw = Stopwatch.StartNew();
            try
            {
                action.Invoke();
            }
            finally
            {
                sw.Stop();
                RegisterExecution(key, sw.ElapsedTicks);
            }
        }

        /// <summary>
        /// Measures execution time of given function under specific counter key
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="func"></param>
        /// <returns></returns>
        public static T MeasureBlock<T>(string key, Func<T> func)
        {
            Stopwatch sw = Stopwatch.StartNew();
            try
            {
                return func.Invoke();
            }
            finally
            {
                sw.Stop();
                RegisterExecution(key, sw.ElapsedTicks);
            }
        }

        /// <summary>
        /// Measures execution time of given action under automatically generated key - by stack frame history
        /// </summary>
        /// <param name="action"></param>
        public static void MeasureBlock(Action action)
        {
            Stopwatch sw = Stopwatch.StartNew();
            try
            {
                action.Invoke();
            }
            finally
            {
                sw.Stop();
                StackFrame frame = new StackFrame(1);
                var callingMethod = frame.GetMethod();
                var key = callingMethod.DeclaringType.Name + "."
                          + callingMethod.Name + "; Line: " + frame.GetFileLineNumber();
                RegisterExecution(key, sw.ElapsedTicks);
            }
        }

        /// <summary>
        /// Measures execution time of given function under automatically generated key - by stack frame history
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="func"></param>
        /// <returns></returns>
        public static T MeasureBlock<T>(Func<T> func)
        {
            Stopwatch sw = Stopwatch.StartNew();
            try
            {
                return func.Invoke();
            }
            finally
            {
                sw.Stop();
                StackFrame frame = new StackFrame(1);
                var callingMethod = frame.GetMethod();
                var key = callingMethod.DeclaringType.Name + "."
                          + callingMethod.Name + "; Line: " + frame.GetFileLineNumber();
                RegisterExecution(key, sw.ElapsedTicks);
            }
        }

        /// <summary>
        /// Writes performance results in formated layout to the given stream.
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="orderMode"></param>
        /// <param name="vector"></param>
        public static void ExportPerformanceResults(StreamWriter stream, OrderMode orderMode, OrderVector vector)
        {
            var comparator = new PerfInfoComparator(orderMode);
            PerfInfo[] dataSet;
            if (vector == OrderVector.Ascending)
            {
                dataSet = _results.Values.Select(i => i).OrderBy(i => i, comparator).ToArray();
            }
            else
            {
                dataSet = _results.Values.Select(i => i).OrderByDescending(i => i, comparator).ToArray();
            }

            stream.WriteLine(@"| EventName | EventCount | TotalTicks | AverageTicks | Average Time (ms) |");
            foreach (PerfInfo perfInfo in dataSet)
            {
                stream.WriteLine(@"| " + string.Join(@" | ", new[]
                {
                    perfInfo.Name,
                    perfInfo.Events.ToString(@"D"),
                    perfInfo.TotalTicks.ToString(@"D"),
                    (perfInfo.TotalTicks/perfInfo.Events).ToString(@"D"),
                    ((decimal) perfInfo.TotalTicks/perfInfo.Events/TimeSpan.TicksPerMillisecond)
                        .ToString(@"F4")
                }) + @" |");
            }
        }

        /// <summary>
        /// Clears performance results registered so far.
        /// </summary>
        public static void Clear()
        {
            _results.Clear();
        }
    }


    internal class PerfInfoComparator : IComparer<PerfInfo>
    {
        private readonly OrderMode _orderMode;

        public PerfInfoComparator(OrderMode orderMode)
        {
            this._orderMode = orderMode;
        }

        public int Compare(PerfInfo x, PerfInfo y)
        {
            switch (this._orderMode)
            {
                case OrderMode.Name:
                    return x.Name.CompareTo(y.Name);
                case OrderMode.TotalTime:
                    return x.TotalTicks.CompareTo(y.TotalTicks);
                case OrderMode.AverageTime:
                    return (x.TotalTicks / x.Events).CompareTo(y.TotalTicks / y.Events); // will always be at least 1 :-)
                case OrderMode.EventCount:
                    return x.Events.CompareTo(y.TotalTicks);
                default:
                    throw new ArgumentOutOfRangeException("orderMode");
            }
        }
    }

    public enum OrderMode
    {
        Name,
        TotalTime,
        AverageTime,
        EventCount
    }

    public enum OrderVector
    {
        Ascending,
        Descending
    }

    public class PerfInfo
    {
        private readonly string _name;
        public int Events;
        public long TotalTicks;

        public PerfInfo(string name)
        {
            this._name = name;
        }

        public string Name
        {
            get { return this._name; }
        }
    }
}
