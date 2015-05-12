using Othello.Logic.Common;

namespace Othello.Logic.Interfaces
{
    public interface IPlayer
    {
        PlayerKind PlayerKind { get; set; }
        IMove GetNextMove(IBoard board);
        string PlayerName { get; set; }
        void CancelNextMove();
    }
}
