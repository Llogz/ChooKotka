using System;
using System.Collections.Generic;
using System.Threading;

namespace Core
{
    public interface ILifetime
    {
        Lifetime Child();
        CancellationToken GetToken();
        void Dispose();
    }
    
    public sealed class Lifetime : IDisposable, ILifetime
    {
        private readonly CancellationTokenSource _cts = new();
        private readonly List<Lifetime> _children = new();
        private bool _disposed;

        public Lifetime Child()
        {
            if (_disposed)
                throw new ObjectDisposedException(nameof(Lifetime));

            var child = new Lifetime();
            _children.Add(child);
            return child;
        }
        
        public CancellationToken GetToken() => _cts.Token;

        public void Dispose()
        {
            if (_disposed)
                return;

            _disposed = true;

            foreach (var child in _children)
            {
                child.Dispose();
            }
            _children.Clear();

            _cts.Cancel();
            _cts.Dispose();
        }
    }
}