using System;

namespace Bd.Threading.Tasks.Remoting
{
    public sealed class TaskProxy : ITask
    {
        private readonly ITask _task;

        public TaskProxy(ITask task) => _task = task;

        public ITaskAwaiter GetAwaiter() => new TaskAwaiterProxy(_task);

        private sealed class TaskAwaiterProxy : TaskAwaiterProxyBase, ITaskAwaiter
        {
            private readonly ITaskAwaiter _awaiter;

            public TaskAwaiterProxy(ITask task) => _awaiter = task.GetAwaiter();

            public bool IsCompleted => _awaiter.IsCompleted;

            public void GetResult() => _awaiter.GetResult();

            protected override void OnCompletedCore(Action action) => _awaiter.OnCompleted(action);
        }
    }
}
