using System;
using System.Threading;
using System.Threading.Tasks;

namespace Bd.Threading.Tasks.Remoting
{
    class Program
    {
        static async Task Main()
        {
            var ad = AppDomain.CreateDomain("test");

            //
            // Instantiate a service in the target app domain.
            //

            ad.DoCallBack(() =>
            {
                AppDomain.CurrentDomain.SetData("svc", new BarService(new Bar()));
            });

            var svc = (IBarService)ad.GetData("svc");


            //
            // Create a proxy to the service.
            //

            var bar = new BarProxy(svc);


            //
            // Invoke without and with cancellation.
            //

            Console.WriteLine(await bar.FooAsync(21, CancellationToken.None));

            var cts = new CancellationTokenSource();
            cts.CancelAfter(500);

            try
            {
                await bar.FooAsync(21, cts.Token);

                Console.WriteLine("Didn't fail!");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }

    interface IBarService
    {
        ITask<int> FooAsync(int x, ICancellationToken token);
        ICancellationTokenSource GetCancellationTokenSource();
    }

    class BarProxy : IBar
    {
        private readonly IBarService _bar;

        public BarProxy(IBarService bar) => _bar = bar;

        public async Task<int> FooAsync(int x, CancellationToken token)
        {
            token.ThrowIfCancellationRequested();

            var cancel = _bar.GetCancellationTokenSource();

            using (token.Register(cancel.Cancel))
            {
                return await new TaskProxy<int>(_bar.FooAsync(x, cancel.Token));
            }
        }
    }

    class BarService : MarshalByRefObject, IBarService
    {
        private readonly IBar _bar;

        public BarService(IBar bar) => _bar = bar;

        public ITask<int> FooAsync(int x, ICancellationToken token)
        {
            var cancel = new CancellationTokenSource();

            var task = _bar.FooAsync(x, cancel.Token);

            var ctr = token.Register(cancel.Cancel);
            task.ContinueWith(_ => ctr.Dispose());

            return new TaskRef<int>(task);
        }

        public ICancellationTokenSource GetCancellationTokenSource() => new CancellationTokenSourceRef(new CancellationTokenSource());

        public override object InitializeLifetimeService() => null;
    }

    interface IBar
    {
        Task<int> FooAsync(int x, CancellationToken token);
    }

    class Bar : IBar
    {
        public async Task<int> FooAsync(int x, CancellationToken token)
        {
            await Task.Delay(5000, token);
            token.ThrowIfCancellationRequested();
            return x * 2;
        }
    }
}
