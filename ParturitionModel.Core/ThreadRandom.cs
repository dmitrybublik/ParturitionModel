using System;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace ParturitionModel.Core
{
    public sealed class ThreadRandom
    {
        private readonly Dispatcher _dispatcher;
        private readonly Random _random;

        public ThreadRandom()
        {
            _dispatcher = Dispatcher.CurrentDispatcher;
            var seed = new object().GetHashCode();
            _random = new Random(seed);
        }

        public async Task<double> NextDoubleAsync()
        {
            return await _dispatcher.InvokeAsync(() => _random.NextDouble());
        }

        public async Task<int> Next(int min, int max)
        {
            return await _dispatcher.InvokeAsync(() => _random.Next(min, max));
        }
    }
}