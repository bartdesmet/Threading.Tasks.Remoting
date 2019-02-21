using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Bd.Threading.Tasks.Remoting
{
    public sealed class TaskRef<T> : MarshalByRefObject, ITask<T>
    {
        private readonly Task<T> _task;

        public TaskRef(Task<T> task) => _task = task;

        public ITaskAwaiter<T> GetAwaiter() => new Awaiter(_task);

        ITaskAwaiter ITask.GetAwaiter() => GetAwaiter();

        public override object InitializeLifetimeService() => null;

        private sealed class Awaiter : MarshalByRefObject, ITaskAwaiter<T>
        {
            private readonly TaskAwaiter<T> _awaiter;

            public Awaiter(Task<T> task) => _awaiter = task.GetAwaiter();

            public bool IsCompleted => _awaiter.IsCompleted;

            public T GetResult() => _awaiter.GetResult();

            public void OnCompleted(Action action) => _awaiter.OnCompleted(action);

            void ITaskAwaiter.GetResult() => GetResult();

            public override object InitializeLifetimeService() => null;
        }
    }
}
