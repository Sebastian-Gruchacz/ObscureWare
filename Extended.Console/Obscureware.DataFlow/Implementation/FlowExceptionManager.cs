namespace Obscureware.DataFlow.Implementation
{
    using System;
    using System.Threading;

    /// <summary>
    /// Manages exceptions that occur during flow processing / execution. This is very simple and naive implementation of ExceptionHandler strategy.
    /// </summary>
    public class FlowExceptionManager
    {
        private int _maxExceptions;

        private int _totalExceptionCount;

        public FlowExceptionManager(int maxExceptions = 0)
        {
            this._maxExceptions = maxExceptions;
        }

        public void IncrementExceptionCount()
        {
            Interlocked.Increment(ref this._totalExceptionCount);
        }

        private long GetExceptionCount()
        {
            return Thread.VolatileRead(ref this._totalExceptionCount);
        }

        public virtual bool IsCritical(Exception ex)
        {
            return false;
        }

        public virtual bool IsExceptionCountExceeded()
        {
            return this.GetExceptionCount() > this._maxExceptions;
        }

        public void SetMaxException(int maxException)
        {
            this._maxExceptions = maxException;
        }

        public void ResetTotalExceptionCount()
        {
            this._totalExceptionCount = 0;
        }
    }
}