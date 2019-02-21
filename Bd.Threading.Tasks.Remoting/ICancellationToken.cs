using System;

namespace Bd.Threading.Tasks.Remoting
{
    public interface ICancellationToken
    {
        IDisposable Register(Action action);
    }
}
