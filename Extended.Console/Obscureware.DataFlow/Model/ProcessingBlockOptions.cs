namespace Obscureware.Console.Commands.Blocks
{
    public class ProcessingBlockOptions
    {
        public static ProcessingBlockOptions New(int maxDegreeOfParallelism = 1,
                                                 bool turnOnBroadCast = false, bool doResultCheck = true)
        {
            return new ProcessingBlockOptions
                       {
                           DoResultCheck = doResultCheck,
                           MaxDegreeOfParallelism = maxDegreeOfParallelism,
                           TurnOnBroadCast = turnOnBroadCast
                       };
        }

        public ProcessingBlockOptions()
        {
            this.MaxDegreeOfParallelism = 1;
        }

        public int MaxDegreeOfParallelism { get; set; }

        /// <summary>
        /// Only one message will be stored and will be delivered to all targets.
        /// </summary>
        public bool TurnOnBroadCast { get; set; }
        /// <summary>
        /// Set this to false if no additional checking of result should be performed.
        /// Useful for tests. 
        /// </summary>
        public bool DoResultCheck { get; set; }
    }
}