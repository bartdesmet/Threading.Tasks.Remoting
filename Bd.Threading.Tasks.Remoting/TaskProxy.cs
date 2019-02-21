using System;

namespace Bd.Threading.Tasks.Remoting
{
    public sealed class TaskProxy : ITask
    {
        private readonly ITask _task;

        public TaskProxy(ITask task) => _task = task;

        public ITaskAwaiter GetAwaiter() => new Awaiter(_task);

        private sealed class Awaiter : ITaskAwaiter
        {
            private readonly ITaskAwaiter _awaiter;

            public Awaiter(ITask task) => _awaiter = task.GetAwaiter();

            public bool IsCompleted => _awaiter.IsCompleted;

            public void GetResult() => _awaiter.GetResult();

            public void OnCompleted(Action action) => _awaiter.OnCompleted(new Callback(action).Invoke);
        }
    }
}
