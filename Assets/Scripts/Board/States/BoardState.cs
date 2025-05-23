using Interfaces;

namespace Board.States
{
    public abstract class BoardState : IState
    {
        protected readonly Board Board;

        protected BoardState(Board board)
        {
            Board = board;
        }
    
        public abstract void EnterState();
    
        public abstract void ExitState();
    
        public abstract void UpdateState();
    }
}