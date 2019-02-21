using System;
using System.Runtime.CompilerServices;
using System.Runtime.Remoting.Lifetime;
using System.Threading.Tasks;

namespace Bd.Threading.Tasks.Remoting
{
    public sealed class TaskRef<T> : MarshalByRefObject, ITask<T>
    {
        private readonly Task<T> _task;

        public TaskRef(Task<T> task) => _task = task;

        public ITaskAwaiter<T> GetAwaiter()
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

        ITaskAwaiter ITask.GetAwaiter() => GetAwaiter();

        // NB: Default lifetime for task references. You better await them soon after getting a proxy to them.

        private sealed class TaskAwaiterRef : MarshalByRefObject, ITaskAwaiter<T>
        {
            private readonly TaskAwaiter<T> _awaiter;

            public TaskAwaiterRef(Task<T> task) => _awaiter = task.GetAwaiter();

            public bool IsCompleted => _awaiter.IsCompleted;

            public T GetResult() => _awaiter.GetResult();

            public void OnCompleted(Action action) => _awaiter.OnCompleted(action);

            void ITaskAwaiter.GetResult() => GetResult();

            // NB: Got a sponsor until the task completes.
        }
    }
}
