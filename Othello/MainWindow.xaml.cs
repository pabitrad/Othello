﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Othello.ViewModel;
using Othello.Logic.Interfaces;
using Othello.Logic.Common;
using System.IO;

namespace Othello
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        const int MOVE_COUNT = 40;
        MainViewModel _main = null;
        string defaultPlayerNameA = string.Empty;
        string defaultPlayerNameB = string.Empty;

        BitmapImage activeBlackPlayer = new BitmapImage(new Uri("C:\\png\\turn_black.png"));
        BitmapImage activeWhitePlayer = new BitmapImage(new Uri("C:\\png\\turn_white.png"));

        BitmapImage inactiveBlackPlayer = new BitmapImage(new Uri("C:\\png\\black_noback.png"));
        BitmapImage inactiveWhitePlayer = new BitmapImage(new Uri("C:\\png\\white_noback.png"));

        bool noAvailableMessageDisplayedForBlack = false;
        bool noAvailableMessageDisplayedForWhite = false;
        bool resultShown = false;

        private PlayerKind? _lastDecrementedMoveOwnerPlayer = null;

        public MainWindow()
        {
            InitializeComponent();
            populateStackPanels();

            Loaded+=(_,__)=>SubscribeEvents();
        }

        private void populateStackPanels()
        {
            addMoves();

            defaultPlayerNameA = PlayerA.Content.ToString().Trim();
        }

        private void addMoves()
        {
            for (int i = 0; i < MOVE_COUNT; i++)
            {
                addOneMove(PlayerKind.Black);
                addOneMove(PlayerKind.White);
            }

            WhiteTotalCount.Text = WhiteMoves.Children.Count.ToString();
            BlackTotalCount.Text = BlackMoves.Children.Count.ToString();
        }

        private void addOneMove(PlayerKind playerKind)
        {
            StackPanel movesPanel = playerKind == PlayerKind.White ? WhiteMoves : BlackMoves;
            Color borderColor = playerKind == PlayerKind.White ? Colors.Black : Colors.White;
            Color backgroundColor = playerKind == PlayerKind.White ? Colors.White : Colors.Black;
            movesPanel.Children.Add(new Label
            {
                BorderThickness = new Thickness(1, 1, 1, 1),
                BorderBrush = new SolidColorBrush(borderColor),
                Height = 12,
                Background = new SolidColorBrush(backgroundColor)
            });
        }

        private void SubscribeEvents()
        {
            _main = DataContext as MainViewModel;
            if (_main == null)
                return;
            _main.PropertyChanged += (_, e) =>
                                      {
                                          if (e.PropertyName != "WhitePlaysNow") return;
                                          if (_main.WhitePlaysNow == null)
                                              VisualStateManager.GoToState(turnControl, "NonePlaying", false);
                                          else if (_main.WhitePlaysNow.Value)
                                              VisualStateManager.GoToState(turnControl, "PlayingWhite", false);
                                          else
                                              VisualStateManager.GoToState(turnControl, "PlayingBlack", false);
                                      };
        }

        public bool BeginGame()
        {
            if (defaultPlayerNameA == PlayerA.Content.ToString().Trim() || defaultPlayerNameB == PlayerB.Content.ToString().Trim())
            {
                //MessageBox.Show("Please enter name of Player A", "Othello");
                //return false;
                PlayerName playerName = null;

                playerName = new PlayerName();
                playerName.Owner = this;
                playerName.WindowStartupLocation = WindowStartupLocation.CenterOwner;
                bool? dialogResult = playerName.ShowDialog();
                if (dialogResult.HasValue && dialogResult.Value)
                {
                    this.PlayerA.Content = playerName.PlayerA;
                    this.PlayerB.Content = playerName.PlayerB;
                }
            }
            //else if (defaultPlayerNameB == PlayerB.Content.ToString().Trim())
            //{
            //    MessageBox.Show("Please enter name of Player B", "Othello");
            //    return false;
            //}

            if (_main != null)
            {
                _main.BlackAvailableMoves = BlackMoves.Children.Count;
                _main.WhiteAvailableMoves = WhiteMoves.Children.Count;

                _main.Board.InitializeBoard(false);

                BlackMoves.Children.RemoveRange(0, 2);
                WhiteMoves.Children.RemoveRange(0, 2);
                _lastDecrementedMoveOwnerPlayer = null;

                setInitialScore();
            }

            return true;
        }

        private void setInitialScore()
        {
            PlayerACount.Text = "2";
            PlayerBCount.Text = "2";

            WhiteTotalCount.Text = WhiteMoves.Children.Count.ToString();
            BlackTotalCount.Text = BlackMoves.Children.Count.ToString();
        }

        public void UpdateScore(int whiteCount, int blackCount)
        {
            PlayerACount.Text = blackCount.ToString();
            PlayerBCount.Text = whiteCount.ToString(); 
        }

        public void UpdateScore(int whiteCount, int blackCount, PlayerKind playerKind, bool isUndoneMove)
        {
            PlayerACount.Text = blackCount.ToString();
            PlayerBCount.Text = whiteCount.ToString();

            if (isUndoneMove && _lastDecrementedMoveOwnerPlayer.HasValue)
            {
                addOneMove(_lastDecrementedMoveOwnerPlayer.Value);
                if (_lastDecrementedMoveOwnerPlayer.Value == PlayerKind.White)
                {
                    _main.WhiteAvailableMoves = WhiteMoves.Children.Count;
                    WhiteTotalCount.Text = WhiteMoves.Children.Count.ToString();
                }
                else
                {
                    _main.BlackAvailableMoves = BlackMoves.Children.Count;
                    BlackTotalCount.Text = BlackMoves.Children.Count.ToString();
                }
                _lastDecrementedMoveOwnerPlayer = null;
            }

            if (playerKind == PlayerKind.White) //Black move is done
            {
                if (!isUndoneMove && BlackMoves.Children.Count > 0)
                {
                    _lastDecrementedMoveOwnerPlayer = PlayerKind.Black;
                    BlackMoves.Children.RemoveAt(0);
                    _main.BlackAvailableMoves = BlackMoves.Children.Count;
                    BlackTotalCount.Text = BlackMoves.Children.Count.ToString();
                }
                else if (!isUndoneMove)
                {
                    if (!noAvailableMessageDisplayedForBlack)
                    {
                        MessageBox.Show("No move is available for Player A.");
                        noAvailableMessageDisplayedForBlack = true;
                    }
                    if (WhiteMoves.Children.Count > 0)
                    {
                        _lastDecrementedMoveOwnerPlayer = PlayerKind.White;
                        WhiteMoves.Children.RemoveAt(0);
                        WhiteTotalCount.Text = WhiteMoves.Children.Count.ToString();

                        if (WhiteMoves.Children.Count == 0)
                        {
                            MessageBox.Show("No move is available for Player B.");
                            showResult(whiteCount, blackCount);
                            return;
                        }
                    }
                }

                if (WhiteMoves.Children.Count > 0)
                {
                    WhitePlayerPic.Source = activeWhitePlayer;
                    BlackPlayerPic.Source = inactiveBlackPlayer;
                }
            }
            else
            {
                //White move is done
                if (!isUndoneMove && WhiteMoves.Children.Count > 0)
                {
                    _lastDecrementedMoveOwnerPlayer = PlayerKind.White;
                    WhiteMoves.Children.RemoveAt(0);
                    _main.WhiteAvailableMoves = WhiteMoves.Children.Count;
                    WhiteTotalCount.Text = WhiteMoves.Children.Count.ToString();
                }
                else if (!isUndoneMove)
                {
                    if (!noAvailableMessageDisplayedForWhite)
                    {
                        MessageBox.Show("No move is available for Player B.");
                        noAvailableMessageDisplayedForWhite = true;
                    }

                    if (BlackMoves.Children.Count > 0)
                    {
                        _lastDecrementedMoveOwnerPlayer = PlayerKind.Black;
                        BlackMoves.Children.RemoveAt(0);
                        BlackTotalCount.Text = BlackMoves.Children.Count.ToString();

                        if (BlackMoves.Children.Count == 0)
                        {
                            MessageBox.Show("No move is available for Player A.");
                            showResult(whiteCount, blackCount);
                            return;
                        }
                    }
                }

                if (BlackMoves.Children.Count > 0)
                {
                    BlackPlayerPic.Source = activeBlackPlayer;
                    WhitePlayerPic.Source = inactiveWhitePlayer;
                }
            }

            if (whiteCount + blackCount == 64)
            {
                if (!resultShown)
                {
                    showResult(whiteCount, blackCount);
                    resultShown = true;
                }
            }
        }

        private void showResult(int whiteCount, int blackCount)
        {
            if (whiteCount > blackCount)
            {
                MessageBox.Show("White Wins.");
            }
            else if (blackCount > whiteCount)
            {
                MessageBox.Show("Black Wins.");
            }
            else
            {
                MessageBox.Show("Game Draw.");
            }
        }

        private void PlayerA_NameChanged(object sender, TextChangedEventArgs e)
        {
            if (_main != null)
            {
                //_main.SelectedBlackPlayer.PlayerName = NamePlayerA.Text.Trim();
            }
        }

        private void PlayerB_NameChanged(object sender, TextChangedEventArgs e)
        {
            if (_main != null)
            {
                //_main.SelectedWhitePlayer.PlayerName = NamePlayerB.Text.Trim();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            _main.Hint = !_main.Hint;

            if (_main.Hint)
            {
                btnHint.Content = "DISABLE HINT";
            }
            else
            {
                btnHint.Content = "ENABLE HINT";
            }
        }

        private void Reset_Game(object sender, RoutedEventArgs e)
        {
            BlackMoves.Children.Clear();
            WhiteMoves.Children.Clear();

            addMoves();
            setInitialScore();

            BlackPlayerPic.Source = activeBlackPlayer;
            WhitePlayerPic.Source = inactiveWhitePlayer;

            btnHint.Content = "ENABLE HINT";
            _main.Hint = false;
            _main.ResetGame();
        }

        private void New_Game(object sender, RoutedEventArgs e)
        {
            BlackMoves.Children.Clear();
            WhiteMoves.Children.Clear();

            addMoves();
            PlayerACount.Text = string.Empty;
            PlayerBCount.Text = string.Empty;

            WhiteTotalCount.Text = WhiteMoves.Children.Count.ToString();
            BlackTotalCount.Text = BlackMoves.Children.Count.ToString();

            BlackPlayerPic.Source = activeBlackPlayer;
            WhitePlayerPic.Source = inactiveWhitePlayer;

            btnHint.Content = "ENABLE HINT";
            _main.Hint = false;

            //NamePlayerA.Text = defaultPlayerName;
            //NamePlayerB.Text = defaultPlayerName;

            _main.NewGame();
        }

        private void Undo_Click(object sender, RoutedEventArgs e)
        {
            _main.Undo();
        }

        private void Save_Data(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog();
            dlg.Filter = "Text Files (*.txt)|*.txt";

            // Display OpenFileDialog by calling ShowDialog method 
            Nullable<bool> result = dlg.ShowDialog();
            if (result == true)
            {

                File.WriteAllText(dlg.FileName, _main.Board.Board.ToString());
                MessageBox.Show("Game Saved sucessfully!");
            }
        }

        private void Load_Data(object sender, RoutedEventArgs e)
        {

        }

        private void HandleManualFlipSwitch(object sender, RoutedEventArgs e)
        {
            if (btnManualFlip.IsChecked.HasValue && btnManualFlip.IsChecked.Value)
            {
                btnManualFlip.Content = "Manual Flip Off";
            }
            else
            {
                btnManualFlip.Content = "Manual Flip On";
            }
        }
    }
}
