using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Othello;
using Othello.Logic.Classes.Players;
using Othello.Logic.Common;
using Othello.Logic.Interfaces;

namespace Othello.Logic.Classes
{
    public class GameManager:IGameManager
    {
        #region Implementation of IGameManager
        
        public IBoard Board { get; set; }
        public IPlayer WhitePlayer { get; set; }
        public IPlayer BlackPlayer { get; set; }
        public IPlayer CurrentPlayerAtTurn { get; set; }
        public bool GameStarted { get; set; }

        private IMove _lastMove = null;

        public int BlackAvailableMoves
        {
            get;
            set;
        }

        public IMove LastMove
        {
            get
            {
                return _lastMove;
            }
        }

        public int WhiteAvailableMoves { get; set; }

        public void StartGame()
        {
            ResetGame();
            GameStarted = true;
            CurrentPlayerAtTurn = BlackPlayer;
        }

        public void ResetGame()
        {
            Board.ConfigureNewGame(false);
            GameStarted = false;
            IsPlaying = false;
            IsEndGame = false;
        }

        public bool IsPlaying { get; set; }
        public bool IsEndGame { get; set; }
        private bool HasPlayerPassed { get; set; }

        public void MoveNext()
        {
            if (IsEndGame || IsPlaying || !GameStarted)
                return;
            
            IsPlaying = true;
            var nextMove = CurrentPlayerAtTurn.GetNextMove(Board);

            if (nextMove == null) //if can't move 
            {
                IsPlaying = false;
                return;
            } 

            if (nextMove.IsPassMove && HasPlayerPassed) //finish case
            {
                IsEndGame = true;
                
                var whites = Board.WhitePoints.Count;
                var blacks = Board.BlackPoints.Count;
                var gameFinishedEventArgs = new GameFinishedEventArgs {WhiteCount = whites, BlackCount = blacks};
                if (whites - blacks != 0)
                    gameFinishedEventArgs.WhiteWins = whites - blacks > 0;
                RaiseGameFinished(gameFinishedEventArgs);

                GameStarted = false;
                IsPlaying = false;
                return;
            }
            HasPlayerPassed = nextMove.IsPassMove;
            
            MakeMove(nextMove);
            IsPlaying = false;
        }

        private void MakeMove(IMove nextMove)
        {
            Board.MakeMove(nextMove);

            _lastMove = nextMove;

            //CurrentPlayerAtTurn = CurrentPlayerAtTurn == WhitePlayer ? BlackPlayer : WhitePlayer;

            if (CurrentPlayerAtTurn == WhitePlayer)
            {
                if (BlackAvailableMoves > 0)
                {
                    CurrentPlayerAtTurn = BlackPlayer;                    
                }
            }
            else if (CurrentPlayerAtTurn == BlackPlayer)
            {
                if (WhiteAvailableMoves > 0)
                {
                    CurrentPlayerAtTurn = WhitePlayer;
                }
            }

            RaiseMoveDone(new MoveEventArgs(nextMove));
        }

        public void UndoLastMove() {
            if (_lastMove == null || _lastMove.IsPassMove)
            {
                return;
            }
            Board.UnDoMove(_lastMove);

            PlayerKind lastPlayerKind = _lastMove.Player.PlayerKind;
            var previosPlayer = CurrentPlayerAtTurn;
            if (lastPlayerKind == PlayerKind.White)
            {
                CurrentPlayerAtTurn = WhitePlayer;
            }
            else if (lastPlayerKind == PlayerKind.Black)
            {
                CurrentPlayerAtTurn = BlackPlayer;
            }
            else
            {
                throw new InvalidOperationException(string.Format("Unknown PlayerKind: {0}", lastPlayerKind));
            }
            if (previosPlayer != null)
            {
                previosPlayer.CancelNextMove();
            }
            RaiseMoveDone(new MoveEventArgs(_lastMove, true));
            _lastMove = null;
        }

        #region MoveDone

        public event EventHandler<MoveEventArgs> MoveDone;
        
        private void RaiseMoveDone(MoveEventArgs e)
        {
            EventHandler<MoveEventArgs> handler = MoveDone;
            if (handler != null) handler(this, e);
        }

        #endregion

        #region GameFinished

        public event EventHandler<GameFinishedEventArgs> GameFinished;

        private void RaiseGameFinished(GameFinishedEventArgs e)
        {
            EventHandler<GameFinishedEventArgs> handler = GameFinished;
            if (handler != null) handler(this, e);
        }

        #endregion

        #endregion
    }
}
