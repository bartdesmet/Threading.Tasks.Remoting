namespace Bd.Threading.Tasks.Remoting
{
    public interface ITaskAwaiter<out T> : ITaskAwaiter
    {
        new T GetResult();
    }
}
