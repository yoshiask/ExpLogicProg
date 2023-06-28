namespace Guan;

public class SyncEnumerable<T> : IAsyncEnumerable<T>
{
    private IEnumerable<T> _enumerable;

    public SyncEnumerable(IEnumerable<T> enumerable)
    {
        _enumerable = enumerable;
    }

    public IAsyncEnumerator<T> GetAsyncEnumerator(CancellationToken cancellationToken = default)
        => new SyncEnumerator<T>(_enumerable.GetEnumerator());

    private class SyncEnumerator<T> : IAsyncEnumerator<T>
    {
        private IEnumerator<T> _enumerator;

        public SyncEnumerator(IEnumerator<T> enumerator)
        {
            _enumerator = enumerator;
        }
        
        public ValueTask DisposeAsync() => ValueTask.CompletedTask;

        public ValueTask<bool> MoveNextAsync() => ValueTask.FromResult(_enumerator.MoveNext());

        public T Current => _enumerator.Current;
    }
}