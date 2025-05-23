using System;

namespace Interfaces
{
    public interface IStateController<T> : IDisposable where T : IState
    {
        /// <summary>
        /// Put the current state here.
        /// </summary>
        public T CurrentState { get; }

        /// <summary>
        /// Used this method to initialize the state controller.
        /// </summary>
        /// <param name="state"></param>
        public void Initialize(T state);

        /// <summary>
        /// Used this method to calling update method of the current state.
        /// </summary>
        public void UpdateState();

        /// <summary>
        /// Used this method to change the current state.
        /// </summary>
        /// <param name="newState"></param>
        public void ChangeState(T newState);
    }
}