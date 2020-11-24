using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Planner.Repository.SqLite
{
    // disposes of another asyncdisposable when the first enumerator is disposed.
    // useful to dispose of a dbcontext once the query had been enumerated.
    public class DisposeWithAsyncEnumerable<T> : IAsyncEnumerable<T>
    {
        private IAsyncEnumerable<T> inner;
        private IAsyncDisposable disposable;

        public DisposeWithAsyncEnumerable(IAsyncEnumerable<T> inner, IAsyncDisposable disposable)
        {
            this.inner = inner;
            this.disposable = disposable;
        }

        public IAsyncEnumerator<T> GetAsyncEnumerator(CancellationToken cancellationToken = new()) => 
            new DisposingEnumerator(inner.GetAsyncEnumerator(cancellationToken), disposable);

        private class DisposingEnumerator : IAsyncEnumerator<T>
        {
            private IAsyncEnumerator<T> innerEnumerator;
            private IAsyncDisposable disposable;

            public DisposingEnumerator(IAsyncEnumerator<T> innerEnumerator, IAsyncDisposable disposable)
            {
                this.innerEnumerator = innerEnumerator;
                this.disposable = disposable;
            }

            public ValueTask DisposeAsync()
            {
                var t1 = disposable.DisposeAsync();
                var t2 = innerEnumerator.DisposeAsync();
                if (t1.IsCompletedSuccessfully && t2.IsCompletedSuccessfully)
                    return t1;
                return new ValueTask(Task.WhenAll(t1.AsTask(), t2.AsTask()));
            }

            public ValueTask<bool> MoveNextAsync() => innerEnumerator.MoveNextAsync();
            public T Current => innerEnumerator.Current;
        }

    }
}