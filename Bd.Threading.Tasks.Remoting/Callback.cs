using System;

namespace Bd.Threading.Tasks.Remoting
{
    internal sealed class Callback : MarshalByRefObject
    {
        private readonly Action _action;

        public Callback(Action action) => _action = action;

        public void Invoke() => _action();
    }
}
