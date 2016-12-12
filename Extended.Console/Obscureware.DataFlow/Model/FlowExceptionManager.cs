namespace Obscureware.Console.Commands.Blocks
{
    using System;
    using System.Threading;

    public class FlowExceptionManager
    {
        private int _maxException;

        private int _totalExceptionCount;

        public FlowExceptionManager(int maxException = 0)
        {
            this._maxException = maxException;
        }

        public void IncremenetExceptionCount()
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
            return this.GetExceptionCount() > this._maxException;
        }

        public void SetMaxException(int maxException)
        {
            this._maxException = maxException;
        }

        public void ResetTotalExceptionCount()
        {
            this._totalExceptionCount = 0;
        }
    }
}