namespace Obscureware.DataFlow
{
    using System.Threading.Tasks;
    using System.Threading.Tasks.Dataflow;

    /// <summary>
    /// This <see langword="interface"/> is required because of Generics covariance problems
    /// </summary>
    public interface IBlock
    {
        bool Post(object token);

        object Receive();

        void Complete();

        bool IsCompleted { get; }

        Task Completion { get; }

        void OnReceived(ITargetBlock<object> onReceiveBlock);
    }
}
