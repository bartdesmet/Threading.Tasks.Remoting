using System;

namespace Bd.Threading.Tasks.Remoting
{
    public sealed class TaskProxy<T> : ITask<T>
    {
        private readonly ITask<T> _task;

        public TaskProxy(ITask<T> task) => _task = task;

        public ITaskAwaiter<T> GetAwaiter() => new Awaiter(_task);

        ITaskAwaiter ITask.GetAwaiter() => GetAwaiter();

        private sealed class Awaiter : ITaskAwaiter<T>
        {
            private readonly ITaskAwaiter<T> _awaiter;

            public Awaiter(ITask<T> task) => _awaiter = task.GetAwaiter();

            public bool IsCompleted => _awaiter.IsCompleted;

            public T GetResult() => _awaiter.GetResult();

            public void OnCompleted(Action action) => _awaiter.OnCompleted(new Callback(action).Invoke);

            void ITaskAwaiter.GetResult() => GetResult();
        }
    }
}
