using System;
using Othello.Logic.Interfaces;

namespace Othello.Logic.Common
{
    public class MoveEventArgs:EventArgs
    {
        public MoveEventArgs(IMove move, bool isUndoneMove = false)
        {
            Move = move;
            IsUndoneMove = isUndoneMove;
        }
        public IMove Move { get; private set; }

        public bool IsUndoneMove { get; private set; }
    }
}
