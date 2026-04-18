using System;
using System.Threading;
using Cysharp.Threading.Tasks;

namespace Core.Services
{
    public class Timer
    {
        private float _timeLeft;
        private readonly Action _callback;
        private readonly UniTaskCompletionSource _tcs;
        private readonly CancellationTokenRegistration _registration;

        public UpdateType UpdateType { get; }

        public Timer(float time, Action callback, UniTaskCompletionSource tcs,
            UpdateType type, CancellationToken token)
        {
            _timeLeft = time;
            _callback = callback;
            _tcs = tcs;
            UpdateType = type;

            if (token.CanBeCanceled)
            {
                _registration = token.Register(Cancel);
            }
        }

        public bool Tick(float dt)
        {
            _timeLeft -= dt;
            if (_timeLeft > 0f) return false;

            _registration.Dispose();
            _callback?.Invoke();
            _tcs?.TrySetResult();
            return true;
        }

        private void Cancel()
        {
            _registration.Dispose();
            _tcs?.TrySetCanceled();
        }
    }
}