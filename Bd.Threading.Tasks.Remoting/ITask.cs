namespace Bd.Threading.Tasks.Remoting
{
    public interface ITask
    {
        ITaskAwaiter GetAwaiter();
    }
}
