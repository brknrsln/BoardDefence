namespace Interfaces
{
    public interface IState
    {
        /// <summary>
        /// Used this method to enter the state.
        /// </summary>
        public void EnterState();

        /// <summary>
        /// Used this method to exit the state.
        /// </summary>
        public void ExitState();

        /// <summary>
        /// Used this method to update the state.
        /// </summary>
        public void UpdateState();
    }
}