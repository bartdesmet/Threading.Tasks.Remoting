using System;
using System.Threading;

namespace Bd.Threading.Tasks.Remoting
{
    public sealed class CancellationTokenSourceRef : MarshalByRefObject, ICancellationTokenSource
    {
        private readonly CancellationTokenSource _cts;

        public CancellationTokenSourceRef(CancellationTokenSource cts) => _cts = cts;

        public ICancellationToken Token => new CancellationTokenRef(_cts.Token);

        public void Cancel() => _cts.Cancel();

        // NB: Default lifetime for cancellation token sources. Use them immediately to obtain a token reference.

        private sealed class CancellationTokenRef : MarshalByRefObject, ICancellationToken
        {
            private readonly CancellationToken _token;

            public CancellationTokenRef(CancellationToken token) => _token = token;

            public IDisposable Register(Action action) => _token.Register(action); // NB: Register is called service-side, so doesn't need MBRO.

            // NB: Default lifetime for cancellation tokens. Pass them immediately back to the service.
        }
    }
}
