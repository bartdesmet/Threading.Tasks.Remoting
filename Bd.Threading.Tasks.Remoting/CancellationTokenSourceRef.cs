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

        public override object InitializeLifetimeService() => null;

        private sealed class CancellationTokenRef : MarshalByRefObject, ICancellationToken
        {
            private readonly CancellationToken _token;

            public CancellationTokenRef(CancellationToken token) => _token = token;

            public IDisposable Register(Action action) => new CancellationTokenRegistrationRef(_token.Register(action));

            public override object InitializeLifetimeService() => null;

            private sealed class CancellationTokenRegistrationRef : MarshalByRefObject, IDisposable
            {
                private readonly CancellationTokenRegistration _ctr;

                public CancellationTokenRegistrationRef(CancellationTokenRegistration ctr) => _ctr = ctr;

                public void Dispose() => _ctr.Dispose();

                public override object InitializeLifetimeService() => null;
            }
        }
    }
}
