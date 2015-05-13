using System;
using System.Windows;

namespace Othello
{
    public partial class PlayerName : Window
    {
        //private readonly ViewModelClass _viewModel;

        public PlayerName()
        {
            //if (viewModel == null)
            //{
            //    throw new ArgumentNullException("viewModel");
            //}
            
            //_viewModel = viewModel;

            InitializeComponent();
            txtPlayerNameA.Focus();
        }

        public string PlayerA { get; private set; }
        public string PlayerB { get; private set; }

        private void btnOk_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(txtPlayerNameA.Text))
                {
                    MessageBox.Show("Name of Player A can not be empty.");
                    return;
                }
                else if (string.IsNullOrWhiteSpace(txtPlayerNameB.Text))
                {
                    MessageBox.Show("Name of Player B can not be empty.");
                    return;
                }
                else
                {
                    PlayerA = txtPlayerNameA.Text.Trim();
                    PlayerB = txtPlayerNameB.Text.Trim();

                    DialogResult = true;
                    Close();
                }
            }
            catch (Exception ex)
            {
                DialogResult = false;
                MessageBox.Show(ex.Message);
            }
            finally
            {
                
            }
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            this.Close();
        }

        //private bool IsUserAcceptedPlayAnotherSet(int gameSetPlayedCount)
        //{
        //    string message = string.Format("You have already played this set {0} number of times’.  Do you want to play new set?", gameSetPlayedCount);
        //    var answer = MessageBox.Show(message, "Confirm please:", MessageBoxButton.YesNo, MessageBoxImage.Question);
        //    return answer == MessageBoxResult.Yes;
        //} 
    }
}
