using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading;
using System.Windows;
using System.Windows.Threading;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Othello.Logic.Classes;
using Othello.Logic.Classes.Players;
using Othello.Logic.Common;
using Othello.Logic.Interfaces;
using System.Linq;

namespace Othello.ViewModel
{
    /// <summary>
    /// This class contains properties that the main View can data bind to.
    /// <para>
    /// Use the <strong>mvvminpc</strong> snippet to add bindable properties to this ViewModel.
    /// </para>
    /// <para>
    /// You can also use Blend to data bind with the tool's support.
    /// </para>
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    public class MainViewModel : ViewModelBase
    {
        private MainWindow _mainWindow = null;
        private bool _hint = false;

        /// <summary>
        /// Initializes a new instance of the MainViewModel class.
        /// </summary>
        public MainViewModel()
        {
            ////if (IsInDesignMode)
            ////{
            ////    // Code runs in Blend --> create design time data.
            ////}
            ////else
            ////{
            ////    // Code runs "for real"
            ////}
            var tSync = SyncContext;
        }

        #region Properties

        public MainWindow MainWindow
        {
            get
            {
                if (_mainWindow == null)
                {
                    _mainWindow = Application.Current.MainWindow as MainWindow;
                }
                return _mainWindow;
            }
        }

        #region IsManualFlipEnabled

        private bool _isManualFlipEnabled;

        public bool IsManualFlipEnabled
        {
            get { return _isManualFlipEnabled; }
            set
            {
                _isManualFlipEnabled = value;
                RaisePropertyChanged("IsManualFlipEnabled");
            }
        }

        #endregion

        #region GameManager

        private IGameManager _GameManager;
        private const string GameManagerName = "GameManager";
        private int _blackAvailableMoves = 0;
        private int _whiteAvailableMoves = 0;

        public IGameManager GameManager
        {
            get { return _GameManager??(_GameManager = GetNewGameManager()); }
            set
            {
                if (_GameManager != value)
                {
                    _GameManager = value;
                    _GameManager.BlackAvailableMoves = _blackAvailableMoves;
                    _GameManager.WhiteAvailableMoves = _whiteAvailableMoves;

                    RaisePropertyChanged(GameManagerName);
                }
            }
        }

        public int BlackAvailableMoves
        {
            set
            {
                _blackAvailableMoves = value;
                GameManager.BlackAvailableMoves = value;
            }
        }

        public int WhiteAvailableMoves
        {
            set
            {
                _whiteAvailableMoves = value;
                GameManager.WhiteAvailableMoves = value;
            }
        }

        public bool Hint
        {
            get
            {
                return _hint;
            }

            set
            {
                _hint = value;
                _Board.RefreshCells();
            }
        }
        private IGameManager GetNewGameManager()
        {
            return new GameManager() { Board = Board.Board, WhitePlayer = SelectedWhitePlayer, BlackPlayer = SelectedBlackPlayer };
        }

        #endregion

        #region Board

        private BoardViewModel _Board;
        private const string BoardName = "Board";

        public BoardViewModel Board
        {
            get { return _Board ?? (_Board = new BoardViewModel()); }
            set
            {
                if (_Board != value)
                {
                    _Board = value;
                    RaisePropertyChanged(BoardName);
                }
            }
        }

        #endregion

        #region CurrentState

        private string _CurrentState = "Create a new game for start playing.";

        public string CurrentState
        {
            get { return _CurrentState; }
            set
            {
                if (_CurrentState != value)
                {
                    _CurrentState = value;
                    RaisePropertyChanged("CurrentState");
                }
            }
        }

        #endregion

        #region AviableWhitePlayers

        private IEnumerable<IPlayer> _AviableWhitePlayers;
        private const string AviablePlayersName = "AviableWhitePlayers";

        public IEnumerable<IPlayer> AviableWhitePlayers
        {
            get
            {
                return _AviableWhitePlayers ?? (_AviableWhitePlayers = new List<IPlayer>
                                                                           {
                                                                               new HummanPlayer(
                                                                                   PlayerKind.White),
                                                                               new RandomPlayer(
                                                                                   PlayerKind.White),
                                                                               new MiniMaxAlphaBetaPlayer(
                                                                                   PlayerKind.White),
                                                                                new MiniMaxAlphaBetaPlayerHard(
                                                                                   PlayerKind.White),
                                                                                new MiniMaxAlphaBetaEvenHarder(
                                                                                   PlayerKind.White)
                                                                           });
            }
        }

        #endregion

        #region AviableBlackPlayers

        private IEnumerable<IPlayer> _AviableBlackPlayers;
        private const string AviableBlackPlayersName = "AviableBlackPlayers";

        public IEnumerable<IPlayer> AviableBlackPlayers
        {
            get { return _AviableBlackPlayers??(_AviableBlackPlayers = new List<IPlayer>
                                                                           {
                                                                               new HummanPlayer(PlayerKind.Black),
                                                                               new RandomPlayer(PlayerKind.Black),
                                                                               new MiniMaxAlphaBetaPlayer(
                                                                                   PlayerKind.Black),
                                                                               new MiniMaxAlphaBetaPlayerHard(
                                                                                   PlayerKind.Black),
                                                                               new MiniMaxAlphaBetaEvenHarder(
                                                                                   PlayerKind.Black)
                                                                           }); }
           
        }

        #endregion

        #region SelectedWhitePlayer

        private IPlayer _SelectedWhitePlayer;
        private const string SelectedWhitePlayerName = "SelectedWhitePlayer";

        public IPlayer SelectedWhitePlayer
        {
            get { return _SelectedWhitePlayer??(_SelectedWhitePlayer = AviableWhitePlayers.FirstOrDefault()); }
            set
            {
                if (_SelectedWhitePlayer != value)
                {
                    _SelectedWhitePlayer = value;
                    RaisePropertyChanged(SelectedWhitePlayerName);
                }
            }
        }

        #endregion

        #region SelectedBlackPlayer

        private IPlayer _SelectedBlackPlayer;
        private const string SelectedBlackPlayerName = "SelectedBlackPlayer";

        public IPlayer SelectedBlackPlayer
        {
            get { return _SelectedBlackPlayer??(_SelectedBlackPlayer = AviableBlackPlayers.FirstOrDefault()); }
            set
            {
                if (_SelectedBlackPlayer != value)
                {
                    _SelectedBlackPlayer = value;
                    RaisePropertyChanged(SelectedBlackPlayerName);
                }
            }
        }

        #endregion

        #region PlayerThatPlayNow

        private IPlayer _PlayerThatPlayNow;
        private const string PlayerThatPlayNowName = "PlayerThatPlayNow";

        public IPlayer PlayerThatPlayNow
        {
            get { return _PlayerThatPlayNow; }
            set
            {
                if (_PlayerThatPlayNow != value)
                {
                    _PlayerThatPlayNow = value;
                    RaisePropertyChanged(PlayerThatPlayNowName);
                    CurrentState = PlayerThatPlayNow == SelectedWhitePlayer ? "White player playing..." : "Black player playing...";
                    RaisePropertyChanged(IsAHummanPlayerName);

                    _PlausiblesMoves = null;
                    if (IsAHummanPlayer)
                        Board.RefreshCells();   //Refresh Cells for Posibles moves
                    RaisePropertyChanged(WhitePlaysNowName);
                }
            }
        }

        #endregion

        #region IsAHummanPlayer

        private bool _IsAHummanPlayer;
        private const string IsAHummanPlayerName = "IsAHummanPlayer";

        public bool IsAHummanPlayer
        {
            get { return PlayerThatPlayNow is HummanPlayer; }
        }

        #endregion

        public void NewGame()
        {
            ExecuteEndPlayCommand();
            _Board.InitializeBoard(true);
        }

        #region PlausiblesMoves

        private IEnumerable<IMove> _PlausiblesMoves;
        private const string PlausiblesMovesName = "PlausiblesMoves";

        public IEnumerable<IMove> PlausiblesMoves
        {
            get 
            { 
                if (_hint)
                {
                    return _PlausiblesMoves??(_PlausiblesMoves = Board.Board.GetPlausibleMoves(PlayerThatPlayNow));
                }
                else
                {
                    return null;
                }
            }
            //get { return null; }
            
        }

        #endregion
        
        #region HasIsPlayerPassed

        private bool _HasIsPlayerPassed;
        private const string HasIsPlayerPassedName = "HasIsPlayerPassed";

        public bool HasIsPlayerPassed
        {
            get { return _HasIsPlayerPassed; }
            set
            {
                if (_HasIsPlayerPassed != value)
                {
                    _HasIsPlayerPassed = value;
                    RaisePropertyChanged(HasIsPlayerPassedName);
                }
            }
        }

        #endregion

        #region IsPlaying

        private bool _IsPlaying;
        private const string IsPlayingName = "IsPlaying";

        public bool IsPlaying
        {
            get { return _IsPlaying; }
            set
            {
                if (_IsPlaying != value)
                {
                    _IsPlaying = value;
                    RaisePropertyChanged(IsPlayingName);
                    RefreshCommands();
                }
            }
        }

        #endregion

        #region IsGameOver

        private bool _IsGameOver;
        private const string IsGameOverName = "IsGameOver";

        public bool IsGameOver
        {
            get { return _IsGameOver; }
            set
            {
                if (_IsGameOver != value)
                {
                    _IsGameOver = value;
                    RaisePropertyChanged(IsGameOverName);
                }
            }
        }

        #endregion

        #region PlayWorker

        private BackgroundWorker _PlayWorker;
        private const string PlayWorkerName = "PlayWorker";

        public BackgroundWorker PlayWorker
        {
            get { return _PlayWorker; }
            set
            {
                if (_PlayWorker != value)
                {
                    _PlayWorker = value;
                    RaisePropertyChanged(PlayWorkerName);
                }
            }
        }

        #endregion

        #region SyncContext

        private SynchronizationContext _SyncContext;

        public SynchronizationContext SyncContext
        {
            get { return _SyncContext??(_SyncContext=SynchronizationContext.Current); }
            
        }

        #endregion

        #region WhiteCount

        private int _WhiteCount;
        private const string WhiteCountName = "WhiteCount";

        public int WhiteCount
        {
            get { return _WhiteCount; }
            set
            {
                if (_WhiteCount != value)
                {
                    _WhiteCount = value;
                    RaisePropertyChanged(WhiteCountName);
                }
            }
        }

        #endregion

        #region BlackCount

        private int _BlackCount;
        private const string BlackCountName = "BlackCount";

        public int BlackCount
        {
            get { return _BlackCount; }
            set
            {
                if (_BlackCount != value)
                {
                    _BlackCount = value;
                    RaisePropertyChanged(BlackCountName);
                }
            }
        }

        #endregion

        #region WinString

        private string _WinString;
        private const string WinStringName = "WinString";

        public string WinString
        {
            get { return _WinString; }
            set
            {
                if (_WinString != value)
                {
                    _WinString = value;
                    RaisePropertyChanged(WinStringName);
                }
            }
        }

        #endregion

        #region MovesString

        private string _MovesString;
        private const string MovesStringName = "MovesString";

        public string MovesString
        {
            get { return _MovesString; }
            set
            {
                if (_MovesString != value)
                {
                    _MovesString = value;
                    RaisePropertyChanged(MovesStringName);
                }
            }
        }

        #endregion

        #region MovesCount

        private int _MovesCount;
        private const string MovesCountName = "MovesCount";

        public int MovesCount
        {
            get { return _MovesCount; }
            set
            {
                if (_MovesCount != value)
                {
                    _MovesCount = value;
                    RaisePropertyChanged(MovesCountName);
                }
            }
        }

        #endregion

        #region WhitePlaysNow

        private const string WhitePlaysNowName = "WhitePlaysNow";

        public bool? WhitePlaysNow
        {
            get
            {
                if (!IsPlaying)
                    return null;
                return PlayerThatPlayNow.PlayerKind == PlayerKind.White;
            }
        }

        #endregion

        #region BlackTimer

        private DispatcherTimer _BlackTimer;
        private const string BlackTimerName = "BlackTimer";

        public DispatcherTimer BlackTimer
        {
            get { return _BlackTimer ?? (_BlackTimer = CreateBlackTimer()); }
            set
            {
                if (_BlackTimer != value)
                {
                    _BlackTimer = value;
                    RaisePropertyChanged(BlackTimerName);
                }
            }
        }

        private DispatcherTimer CreateBlackTimer()
        {   
            var toRet = new DispatcherTimer();
            toRet.Tick += (_, __) => BlackEllapsedSeconds++;
            toRet.Interval = new TimeSpan(0, 0, 0, 1);
            return toRet;
        }

        #endregion

        #region WhiteTimer

        private DispatcherTimer _WhiteTimer;
        private const string WhiteTimerName = "WhiteTimer";

        public DispatcherTimer WhiteTimer
        {
            get { return _WhiteTimer??(_WhiteTimer = CreateWhiteTimer()); }
            set
            {
                if (_WhiteTimer != value)
                {
                    _WhiteTimer = value;
                    RaisePropertyChanged(WhiteTimerName);
                    
                }
            }
        }

        private DispatcherTimer CreateWhiteTimer()
        {
            var toRet = new DispatcherTimer();
            toRet.Tick += (_, __) => WhiteEllapsedSeconds++;
            toRet.Interval = new TimeSpan(0,0,0,1);
            return toRet;
        }

        #endregion

        #region WhiteEllapsedSeconds

        private int _WhiteEllapsedSeconds = 0;
        private const string WhiteEllapsedSecondsName = "WhiteEllapsedSeconds";

        public int WhiteEllapsedSeconds
        {
            get { return _WhiteEllapsedSeconds; }
            set
            {
                if (_WhiteEllapsedSeconds != value)
                {
                    _WhiteEllapsedSeconds = value;
                    RaisePropertyChanged(WhiteEllapsedSecondsName);
                }
            }
        }

        #endregion

        #region BlackEllapsedSeconds

        private int _BlackEllapsedSeconds = 0;
        private const string BlackEllapsedSecondsName = "BlackEllapsedSeconds";

        public int BlackEllapsedSeconds
        {
            get { return _BlackEllapsedSeconds; }
            set
            {
                if (_BlackEllapsedSeconds != value)
                {
                    _BlackEllapsedSeconds = value;
                    RaisePropertyChanged(BlackEllapsedSecondsName);
                }
            }
        }

        #endregion

        #region LastTimeMoved

        private DateTime _LastTimeMoved;
        private const string LastTimeMovedName = "LastTimeMoved";

        public DateTime LastTimeMoved
        {
            get { return _LastTimeMoved; }
            set
            {
                if (_LastTimeMoved != value)
                {
                    _LastTimeMoved = value;
                    RaisePropertyChanged(LastTimeMovedName);
                }
            }
        }

        #endregion

        #region WhiteMaxEllapsedTime

        private int _WhiteMaxEllapsedTime = 0;
        private const string WhiteMaxEllapsedTimeName = "WhiteMaxEllapsedTime";

        public int WhiteMaxEllapsedTime
        {
            get { return _WhiteMaxEllapsedTime; }
            set
            {
                if (_WhiteMaxEllapsedTime != value)
                {
                    _WhiteMaxEllapsedTime = value;
                    RaisePropertyChanged(WhiteMaxEllapsedTimeName);
                }
            }
        }

        #endregion

        #region BlackMaxEllapsedTime

        private int _BlackMaxEllapsedTime = 0;
        private const string BlackMaxEllapsedTimeName = "BlackMaxEllapsedTime";

        public int BlackMaxEllapsedTime
        {
            get { return _BlackMaxEllapsedTime; }
            set
            {
                if (_BlackMaxEllapsedTime != value)
                {
                    _BlackMaxEllapsedTime = value;
                    RaisePropertyChanged(BlackMaxEllapsedTimeName);
                }
            }
        }

        #endregion

        #endregion

        #region Commands

        private void StartPlay()
        {
            MainWindow _mainWindow = Application.Current.MainWindow as MainWindow;
            if (_mainWindow != null)
            {
                if (_mainWindow.BeginGame() == false)
                {
                    return;
                }
            }

            IsPlaying = true;
            PlayerThatPlayNow = SelectedBlackPlayer;
            IsGameOver = false;
            WinString = "";
            WhiteCount = 2;
            BlackCount = 2;
            WhiteEllapsedSeconds = 0;
            BlackEllapsedSeconds = 0;
            BlackTimer.Start();

            WhiteMaxEllapsedTime = 0;
            BlackMaxEllapsedTime = 0;

            PlayWorker = new BackgroundWorker();
            PlayWorker.DoWork+=PlayLogic;

            GameManager = GetNewGameManager();
            GameManager.StartGame();
            Board.InitializeBoard(false);
            _PlausiblesMoves = null;
            
            GameManager.MoveDone+=GameManagerOnMoveDone;
            GameManager.GameFinished+=GameManagerOnGameFinished;

            MovesString = "";
            MovesCount = 0;

            LastTimeMoved = DateTime.Now;

            PlayWorker.RunWorkerAsync();
        }

        private void GameManagerOnGameFinished(object sender, GameFinishedEventArgs gameFinishedEventArgs)
        {
            SyncContext.Send((_) =>
                                 {
                                     IsPlaying = false;
                                     IsGameOver = true;
                                     WhiteCount = gameFinishedEventArgs.WhiteCount;
                                     BlackCount = gameFinishedEventArgs.BlackCount;
                                     var draftString = "The game is draft";
                                     var noDraftString = gameFinishedEventArgs.WhiteWins == null
                                                             ? null
                                                             : string.Format("{0} Wins!!!",
                                                                             gameFinishedEventArgs.WhiteWins.Value
                                                                                 ? "Whites"
                                                                                 : "Blacks");
                                     WinString = gameFinishedEventArgs.WhiteWins == null ? draftString : noDraftString;
                                     CurrentState = WinString;
                                     BlackTimer.Stop();
                                     WhiteTimer.Stop();
                                 },null);
        }

        private void GameManagerOnMoveDone(object sender, MoveEventArgs moveEventArgs)
        {
            SyncContext.Send(_ =>
            {
                if (!IsPlaying)
                    return;

                if (!moveEventArgs.IsUndoneMove)
                {
                    MovesCount++;
                    MovesString = string.Format("{0}\n{1}.{2}-{3}", MovesString, MovesCount.ToString("00"),
                        PlayerThatPlayNow.PlayerKind == PlayerKind.White
                            ? "White"
                            : "Black",
                        moveEventArgs.Move.IsPassMove
                            ? "Pass"
                            : string.Format("{0}{1}",
                                (char) ('a' +
                                        moveEventArgs.Move.MovePosition.Item2),
                                moveEventArgs.Move.MovePosition.Item1 + 1));

                    Board.DoMove(moveEventArgs.Move);
                }
                else
                {
                    MovesCount--;
                    MovesString = string.Format("{0}\n{1}", MovesString, "Move was undone.");
                    Board.ReverseMove(moveEventArgs.Move);
                }

                PlayerThatPlayNow = GameManager.CurrentPlayerAtTurn;
                Board.RefreshCells();
                WhiteCount = Board.Board.WhitePoints.Count;
                BlackCount = Board.Board.BlackPoints.Count;

                MainWindow.UpdateScore(WhiteCount, BlackCount, PlayerThatPlayNow.PlayerKind, moveEventArgs.IsUndoneMove);
                var seconds = (DateTime.Now - LastTimeMoved).TotalSeconds;
                LastTimeMoved = DateTime.Now;
                if (PlayerThatPlayNow.PlayerKind == PlayerKind.Black)
                {
                    WhiteTimer.Stop();
                    BlackTimer.Start();
                    //While was playing
                    if (WhiteMaxEllapsedTime < seconds)
                        WhiteMaxEllapsedTime = (int) seconds;


                }
                else
                {
                    WhiteTimer.Start();
                    BlackTimer.Stop();
                    //Black was playing
                    if (BlackMaxEllapsedTime < seconds)
                        BlackMaxEllapsedTime = (int) seconds;
                }
            }, null);
        }

        private void PlayLogic(object sender, DoWorkEventArgs doWorkEventArgs)
        {
            //SyncContext.Send((_) => Board.RefreshCells(), null);
            while(GameManager.GameStarted)
            {
                GameManager.MoveNext();
            }
        }

        #region StartPlay

        private RelayCommand _StartPlayCommand;

        public RelayCommand StartPlayCommand
        {
            get { return _StartPlayCommand ?? (_StartPlayCommand = new RelayCommand(ExecuteStartPlayCommand, CanExecuteStartPlayCommand)); }
        }

        private void ExecuteStartPlayCommand()
        {
            //ComandCode
            StartPlay();
        }

        private bool CanExecuteStartPlayCommand()
        {
            return !IsPlaying;
        }

        #endregion

        #region EndPlay

        private RelayCommand _EndPlayCommand;

        public RelayCommand EndPlayCommand
        {
            get { return _EndPlayCommand ?? (_EndPlayCommand = new RelayCommand(ExecuteEndPlayCommand, CanExecuteEndPlayCommand)); }
        }

        private void ExecuteEndPlayCommand()
        {
            //ComandCode
            IsPlaying = false;
            IsGameOver = true;
            GameManager.GameStarted = false;

            //Force to Humman Play for end the endless loop
            if (IsAHummanPlayer && PlausiblesMoves != null && PlausiblesMoves.Any())
            {
                (PlayerThatPlayNow as HummanPlayer).HummanPositionToMove = PlausiblesMoves.First().MovePosition;
            }
            CurrentState = "Stop Game";
            WhiteTimer.Stop();
            BlackTimer.Stop();
        }

        private bool CanExecuteEndPlayCommand()
        {
            return IsPlaying;
        }

        #endregion
        
        #region Exit

        private RelayCommand _ExitCommand;

        public RelayCommand ExitCommand
        {
            get { return _ExitCommand ?? (_ExitCommand = new RelayCommand(ExecuteExitCommand, CanExecuteExitCommand)); }
        }

        public void ResetGame()
        {
            ExecuteEndPlayCommand();
            _Board.InitializeBoard(false);
            _Board.RefreshCells();
            //Hint = false;
            StartPlay();
        }

        private void ExecuteExitCommand()
        {
            //ComandCode
            Application.Current.Shutdown();
        }

        private bool CanExecuteExitCommand()
        {
            return true;
        }

        #endregion

        #region HummanMove

        private RelayCommand<CellViewModel> _HummanMoveCommand;
        
        public RelayCommand<CellViewModel> HummanMoveCommand
        {
            get { return _HummanMoveCommand ?? (_HummanMoveCommand = new RelayCommand<CellViewModel>(ExecuteHummanMoveCommand, CanExecuteHummanMoveCommand)); }
        }

        public void Undo()
        {
            _GameManager.UndoLastMove();
        }

        private void ExecuteHummanMoveCommand(CellViewModel cell)
        {
            if (!IsManualFlipEnabled)
            {
                if (IsAHummanPlayer && cell != null)
                {
                    (PlayerThatPlayNow as HummanPlayer).HummanPositionToMove = new MyTuple<int, int>(cell.X, cell.Y);
                }
            }
            else
            {
                Board.FlipCellManually(cell);
                WhiteCount = Board.Board.WhitePoints.Count;
                BlackCount = Board.Board.BlackPoints.Count;
                MainWindow.UpdateScore(WhiteCount, BlackCount);
            }
        }

        private bool CanExecuteHummanMoveCommand(CellViewModel cell)
        {
            if (!IsManualFlipEnabled)
            {
                IEnumerable<IMove> possibleMoves = Board.Board.GetPlausibleMoves(PlayerThatPlayNow);

                foreach (Move move in possibleMoves)
                {
                    if (move.MovePosition.Item1 == cell.X && move.MovePosition.Item2 == cell.Y)
                    {
                        return true;
                    }
                }
            }
            else if (Board.CanFlipCellManualy(cell))
            {
                return true;
            }

            MessageBox.Show("Illegal Move");
            return false;
        }

        #endregion


        private void RefreshCommands()
        {
            StartPlayCommand.RaiseCanExecuteChanged();
            EndPlayCommand.RaiseCanExecuteChanged();
        }
        #endregion
    }
}