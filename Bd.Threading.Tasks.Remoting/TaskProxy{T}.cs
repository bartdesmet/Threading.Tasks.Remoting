using System;

namespace Bd.Threading.Tasks.Remoting
{
    public sealed class TaskProxy<T> : ITask<T>
    {
        private readonly ITask<T> _task;

        public TaskProxy(ITask<T> task) => _task = task;

        public ITaskAwaiter<T> GetAwaiter() => new TaskAwaiterProxy(_task);

        ITaskAwaiter ITask.GetAwaiter() => GetAwaiter();

        private sealed class TaskAwaiterProxy : TaskAwaiterProxyBase, ITaskAwaiter<T>
        {
            private readonly ITaskAwaiter<T> _awaiter;

            public TaskAwaiterProxy(ITask<T> task) => _awaiter = task.GetAwaiter();

            public bool IsCompleted => _awaiter.IsCompleted;

            public T GetResult() => _awaiter.GetResult();

            protected override void OnCompletedCore(Action action) => _awaiter.OnCompleted(action);

            void ITaskAwaiter.GetResult() => GetResult();
        }
    }
}
