namespace Bd.Threading.Tasks.Remoting
{
    public interface ITask<out T> : ITask
    {
        new ITaskAwaiter<T> GetAwaiter();
    }
}
