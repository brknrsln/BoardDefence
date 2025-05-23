using System;
using Interfaces;

namespace DefaultNamespace
{
    public abstract class StateController<T> : IStateController<T> where T : class, IState
    {
        public T CurrentState { get; protected set; }

        private bool _initialized;


        public abstract void Dispose();

        public void Initialize(T state)
        {
            if (_initialized)
            {
                throw new Exception("State controller is already initialized!");
            }
            CurrentState = state;
            CurrentState.EnterState();
            _initialized = true;
        }

        public void UpdateState()
        {
            if (!_initialized)
            {
                throw new Exception("StateController not initialized!");
            }
            CurrentState.UpdateState();
        }

        public virtual void ChangeState(T newState)
        {
            // TODO: add available change state rolls, QG-1409
            if (CurrentState is null || newState == CurrentState)
            {
                return;
            }

            CurrentState.ExitState();
            CurrentState = newState;
            CurrentState.EnterState();
        }

        public bool CurrentStateIs<TState>() where TState : class, IState
        {
            return CurrentState?.GetType() == typeof(TState);
        }
    }
}