namespace Bd.Threading.Tasks.Remoting
{
    public interface ICancellationTokenSource
    {
        ICancellationToken Token { get; }

        void Cancel();
    }
}
