using System;
using System.Runtime.CompilerServices;
using System.Runtime.Remoting.Lifetime;
using System.Threading.Tasks;

namespace Bd.Threading.Tasks.Remoting
{
    public sealed class TaskRef : MarshalByRefObject, ITask
    {
        private readonly Task _task;

        public TaskRef(Task task) => _task = task;

        public ITaskAwaiter GetAwaiter()
        {
            var awaiter = new TaskAwaiterRef(_task);

            if (!awaiter.IsCompleted)
            {
                var sponsor = new ClientSponsor();

                sponsor.Register(awaiter);

                awaiter.OnCompleted(() => sponsor.Unregister(awaiter));
            }

            return awaiter;
        }

        // NB: Default lifetime for task references. You better await them soon after getting a proxy to them.

        private sealed class TaskAwaiterRef : MarshalByRefObject, ITaskAwaiter
        {
            private readonly TaskAwaiter _awaiter;

            public TaskAwaiterRef(Task task) => _awaiter = task.GetAwaiter();

            public bool IsCompleted => _awaiter.IsCompleted;

            public void OnCompleted(Action action) => _awaiter.OnCompleted(action);

            public void GetResult() => _awaiter.GetResult();

            // NB: Got a sponsor until the task completes.
        }
    }
}
