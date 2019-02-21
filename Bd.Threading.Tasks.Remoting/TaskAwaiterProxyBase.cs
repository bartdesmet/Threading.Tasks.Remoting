using System;
using System.Runtime.CompilerServices;
using System.Runtime.Remoting.Lifetime;

namespace Bd.Threading.Tasks.Remoting
{
    internal abstract class TaskAwaiterProxyBase : INotifyCompletion
    {
        public void OnCompleted(Action action)
        {
            var sponsor = new ClientSponsor();

            Callback cb = null;

            cb = new Callback(() =>
            {
                try
                {
                    action();
                }
                finally
                {
                    sponsor.Unregister(cb);
                }
            });

            sponsor.Register(cb);

            OnCompletedCore(cb.Invoke);
        }

        protected abstract void OnCompletedCore(Action action);
    }
}
