//Tiernan Raftery G00418395 CountDown  Repeat-Project 2024
using Microsoft.Maui.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Timers;

namespace CountdownWordGame
{
    public partial class MainPage : ContentPage
    {
        private List<char> selectedLetters = new List<char>();
        private static Random random = new Random();
        private const string vowels = "AEIOU";
        private const string consonants = "BCDFGHJKLMNPQRSTVWXYZ";
        private System.Timers.Timer timer;
        private int timeLeft;
        private int player1Score;
        private int player2Score;
        private bool isPlayer1Turn;

        public MainPage()
        {
            InitializeComponent();
            ResetGame();
        }

        private void ResetGame()
        {
            selectedLetters.Clear();
            UpdateLettersLabel();
            WordEntry.Text = string.Empty;
            ResultLabel.Text = string.Empty;
            timeLeft = 30;
            TimerLabel.Text = "Time: 30";
            player1Score = 0;
            player2Score = 0;
            Player1ScoreLabel.Text = "Player 1 Score: 0";
            Player2ScoreLabel.Text = "Player 2 Score: 0";
            isPlayer1Turn = true;

            timer = new System.Timers.Timer(1000); // 1 second intervals
            timer.Elapsed += OnTimerElapsed;
            WordEntry.IsEnabled = false;
            PlayAgainButton.IsVisible = false;

            UpdatePlayerTurnLabel();
        }

        private void OnTimerElapsed(object sender, ElapsedEventArgs e)
        {
            timeLeft--;

            // Update the timer label on the UI thread
            Dispatcher.Dispatch(() =>
            {
                TimerLabel.Text = "Time: " + timeLeft;
                if (timeLeft <= 0)
                {
                    timer.Stop();
                    TimerLabel.Text = "Time's up!";
                    WordEntry.IsEnabled = false;
                    PlayAgainButton.IsVisible = true; // Show the "Play Again" button
                    DetermineWinner();
                }
            });
        }

        private void OnConsonantButtonClicked(object sender, EventArgs e)//add a consconant
        {
            if (selectedLetters.Count < 9)
            {
                selectedLetters.Add(consonants[random.Next(consonants.Length)]);
                UpdateLettersLabel();
            }
        }

        private void OnVowelButtonClicked(object sender, EventArgs e)//add a vowel
        {
            if (selectedLetters.Count < 9)
            {
                selectedLetters.Add(vowels[random.Next(vowels.Length)]);
                UpdateLettersLabel();
            }
        }

        private void UpdateLettersLabel()
        {
            LettersLabel.Text = "Letters: " + string.Join(" ", selectedLetters);
            GameLettersLabel.Text = "Letters: " + string.Join(" ", selectedLetters);
        }

        private void OnCheckWordButtonClicked(object sender, EventArgs e)// The player's turn switches after entering a word, but the timer does not reset.
        {
            string enteredWord = WordEntry.Text.ToUpper();
            if (IsValidWord(enteredWord, selectedLetters))
            {
                if (isPlayer1Turn)
                {
                    player1Score++;
                    Player1ScoreLabel.Text = $"Player 1 Score: {player1Score}";//add score
                }
                else
                {
                    player2Score++;
                    Player2ScoreLabel.Text = $"Player 2 Score: {player2Score}";
                }
                ResultLabel.Text = $"Valid word: {enteredWord}";
            }
            else
            {
                ResultLabel.Text = "Invalid word or letters not used correctly!";
            }

            // Switch turns
            isPlayer1Turn = !isPlayer1Turn;
            UpdatePlayerTurnLabel();

            // Reset word entry for the next turn
            WordEntry.Text = string.Empty;
        }

        private bool IsValidWord(string word, List<char> availableLetters)// see if word is vaild 
        {
            // Check if the word can be formed with the available letters
            List<char> tempLetters = new List<char>(availableLetters);
            foreach (char c in word)
            {
                if (!tempLetters.Contains(c))
                {
                    return false;
                }
                tempLetters.Remove(c);
            }

            // You can add more validation for word existence using a dictionary or API here.
            return true;
        }

        private void OnStartGameButtonClicked(object sender, EventArgs e) // starts the game when both players enter thier namee
        {
            string player1Name = Player1NameEntry.Text;
            string player2Name = Player2NameEntry.Text;

            if (!string.IsNullOrWhiteSpace(player1Name) && !string.IsNullOrWhiteSpace(player2Name))
            {
                PlayerTurnLabel.Text = $"{player1Name}'s Turn";
                NameInputScreen.IsVisible = false;
                LetterSelectionScreen.IsVisible = true;
                GameScreen.IsVisible = false;
                ResetGame();
            }
            else
            {
                // Show an alert if either name is empty
                DisplayAlert("Error", "Please enter both player names", "OK");
            }
        }

        private void OnLettersSelectionDoneButtonClicked(object sender, EventArgs e) //triggered when you click done on the vowels and consonants
        {
            if (selectedLetters.Count == 9)
            {
                LetterSelectionScreen.IsVisible = false;
                GameScreen.IsVisible = true;
                WordEntry.IsEnabled = true;
                timer.Start();
            }
            else
            {
                DisplayAlert("Error", "Please select 9 letters", "OK");
            }
        }

        private void UpdatePlayerTurnLabel()//updates display to show whos turn it is.
        {
            string playerName = isPlayer1Turn ? Player1NameEntry.Text : Player2NameEntry.Text;
            PlayerTurnLabel.Text = $"{playerName}'s Turn";
        }

        private void DetermineWinner()//called when the timer reaches zero also determines the win on who has the most score.
        {
            string winner;
            if (player1Score > player2Score)
            {
                winner = $"{Player1NameEntry.Text} wins!";
            }
            else if (player2Score > player1Score)
            {
                winner = $"{Player2NameEntry.Text} wins!";
            }
            else
            {
                winner = "It's a tie!";
            }
            DisplayAlert("Game Over", winner, "OK");
        }

        private void OnPlayAgainButtonClicked(object sender, EventArgs e)//resets the game when button is clicked
        {
            ResetGame();
            LetterSelectionScreen.IsVisible = true;
            GameScreen.IsVisible = false;
        }
    }
}
