using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Bd.Threading.Tasks.Remoting
{
    public sealed class TaskRef : MarshalByRefObject, ITask
    {
        private readonly Task _task;

        public TaskRef(Task task) => _task = task;

        public ITaskAwaiter GetAwaiter() => new Awaiter(_task);

        public override object InitializeLifetimeService() => null;

        private sealed class Awaiter : MarshalByRefObject, ITaskAwaiter
        {
            private readonly TaskAwaiter _awaiter;

            public Awaiter(Task task) => _awaiter = task.GetAwaiter();

            public bool IsCompleted => _awaiter.IsCompleted;

            public void OnCompleted(Action action) => _awaiter.OnCompleted(action);

            public void GetResult() => _awaiter.GetResult();

            public override object InitializeLifetimeService() => null;
        }
    }
}
