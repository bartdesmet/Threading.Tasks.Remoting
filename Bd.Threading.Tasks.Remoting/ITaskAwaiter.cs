using System.Runtime.CompilerServices;

namespace Bd.Threading.Tasks.Remoting
{
    public interface ITaskAwaiter : INotifyCompletion
    {
        bool IsCompleted { get; }
        void GetResult();
    }
}
