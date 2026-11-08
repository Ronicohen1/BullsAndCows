using BullsAndCows.Models;
using BullsAndCows.ViewModels;
using System;
using System.ComponentModel;
using System.Windows.Input;

namespace BullsAndCows.ViewModels
{
    // The ViewModel sits between the page (View) and the game rules (Model).
    // INotifyPropertyChanged lets the page know when a property changes,
    // so the screen updates by itself.
    public class GameViewModel : INotifyPropertyChanged
    {
        // ---------- The Model (all the game rules live here) ----------
        private BullsAndCowsGame _game;

        // ---------- Fields: they store the values of the properties ----------
        private string _playerName = "";
        private string _currentGuess = "";
        private string _statusMessage = "";
        private string _errorMessage = "";
        private bool _hasError = false;
        private bool _canGuess = true;
        private int _attemptCount = 0;
        private int _bestScore = 0;

        // The two commands the buttons use
        private ICommand _submitGuessCommand;
        private ICommand _newGameCommand;
        private ICommand _generateRandomNumberCommand;
        public ICommand GenerateRandomNumberCommand
        {
            get { return _generateRandomNumberCommand; }
        }
        public void GenerateRandomNumber()
        {
            _game.GenerateRandomNumber();
            RandomNumberText = _game.RandomNumber.ToString();
        }
        private string _randomNumberText = "No number generated yet";
        // The page listens to this event
        public event PropertyChangedEventHandler? PropertyChanged;

        // ---------- Constructor ----------
        public GameViewModel()
        {
            _game = new BullsAndCowsGame();                      // the game picks a secret number
            _submitGuessCommand = new SubmitGuessCommandClass(this);
            _newGameCommand = new NewGameCommandClass(this);
            _statusMessage = "0 Bulls, 0 Cows";                  // first message on the screen
            _generateRandomNumberCommand = new GenerateRandomNumberCommandClass(this);

        }

        // Tells the page: "this property changed, read it again"
        private void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        // ---------- Properties (the page binds to these) ----------

        // The player's name (typed in the name box)
        public string PlayerName
        {
            get { return _playerName; }
            set
            {
                if (_playerName != value)
                {
                    _playerName = value;
                    OnPropertyChanged(nameof(PlayerName));
                }
            }
        }

        // The guess the player is typing
        public string CurrentGuess
        {
            get { return _currentGuess; }
            set
            {
                if (_currentGuess != value)
                {
                    _currentGuess = value;
                    OnPropertyChanged(nameof(CurrentGuess));
                }
            }
        }

        // Feedback shown in the message box (bulls/cows, win message...)
        public string StatusMessage
        {
            get { return _statusMessage; }
            set
            {
                if (_statusMessage != value)
                {
                    _statusMessage = value;
                    OnPropertyChanged(nameof(StatusMessage));
                }
            }
        }

        // The text of the error (for an invalid guess)
        public string ErrorMessage
        {
            get { return _errorMessage; }
            set
            {
                if (_errorMessage != value)
                {
                    _errorMessage = value;
                    OnPropertyChanged(nameof(ErrorMessage));
                }
            }
        }

        // true = the error label is visible
        public bool HasError
        {
            get { return _hasError; }
            set
            {
                if (_hasError != value)
                {
                    _hasError = value;
                    OnPropertyChanged(nameof(HasError));
                }
            }
        }

        // true = the player can guess. false = the game is over (after a win)
        public bool CanGuess
        {
            get { return _canGuess; }
            set
            {
                if (_canGuess != value)
                {
                    _canGuess = value;
                    OnPropertyChanged(nameof(CanGuess));
                }
            }
        }

        // How many valid guesses in the current game
        public int AttemptCount
        {
            get { return _attemptCount; }
            set
            {
                if (_attemptCount != value)
                {
                    _attemptCount = value;
                    OnPropertyChanged(nameof(AttemptCount));
                }
            }
        }

        // The lowest number of attempts in a win (0 = no win yet)
        public int BestScore
        {
            get { return _bestScore; }
            set
            {
                if (_bestScore != value)
                {
                    _bestScore = value;
                    OnPropertyChanged(nameof(BestScore));
                }
            }
        }
        public string RandomNumberText
        {
            get { return _randomNumberText; }
            set
            {
                if (_randomNumberText != value)
                {
                    _randomNumberText = value;
                    OnPropertyChanged(nameof(RandomNumberText));
                }
            }
        }
        // ---------- Commands (the buttons bind to these) ----------
        public ICommand SubmitGuessCommand
        {
            get { return _submitGuessCommand; }
        }

        public ICommand NewGameCommand
        {
            get { return _newGameCommand; }
        }

        // ---------- Methods (called by the commands) ----------

        // Runs when the player presses "Enter"
        public void SubmitGuess()
        {
            // The Entry can give null when it is empty, so use "" instead
            string guess = CurrentGuess ?? "";

            // Game is over: no more guesses until a new game
            if (_game.IsGameOver)
            {
                StatusMessage = "Game is over! Do you want to play again?";
                return;
            }

            // Check the guess with the same rules as the console app
            string errorText;
            if (!BullsAndCowsGame.IsValidGuess(guess, out errorText))
            {
                ErrorMessage = errorText;
                HasError = true;
                return;
            }

            // The guess is valid: hide the error and let the Model count bulls and cows
            HasError = false;
            ErrorMessage = "";
            GuessResult result = _game.SubmitGuess(guess);

            // Copy the numbers from the Model to the screen
            AttemptCount = _game.AttemptCount;
            BestScore = _game.BestScore;

            string message = result.Bulls + " Bulls, " + result.Cows + " Cows";
            if (result.IsWinningGuess)
            {
                message += "\n*** Correct " + PlayerName + "! You guessed it in " + _game.AttemptCount + " attempts. ***";
                message += "\nBest score: " + _game.BestScore + " attempts.";
                message += "\nPress 'New game' to play again.";
                CanGuess = false;       // no guesses after a win
            }
            StatusMessage = message;
            CurrentGuess = "";          // empty the guess box for the next guess
        }

        // Runs when the player presses "New game"
        public void NewGame()
        {
            _game.StartNewGame();       // new secret number, attempts back to 0
            AttemptCount = _game.AttemptCount;
            BestScore = _game.BestScore;    // the best score stays, like in the console app
            CanGuess = true;
            HasError = false;
            ErrorMessage = "";
            CurrentGuess = "";
            StatusMessage = "Starting a new game! I've picked a new secret number.";
        }
    }

    // A command is a small class that the button calls.
    // This one calls SubmitGuess() of the ViewModel.
    public class SubmitGuessCommandClass : ICommand
    {
        private GameViewModel _viewModel;

        public SubmitGuessCommandClass(GameViewModel viewModel)
        {
            _viewModel = viewModel;
        }

        // The page needs this event, but we never change it (the button is
        // enabled or disabled by the CanGuess property instead)
        public event EventHandler? CanExecuteChanged
        {
            add { }
            remove { }
        }

        public bool CanExecute(object? parameter)
        {
            return true;
        }

        public void Execute(object? parameter)
        {
            _viewModel.SubmitGuess();
        }
    }

    // This one calls NewGame() of the ViewModel.
    public class NewGameCommandClass : ICommand
    {
        private GameViewModel _viewModel;

        public NewGameCommandClass(GameViewModel viewModel)
        {
            _viewModel = viewModel;
        }

        public event EventHandler? CanExecuteChanged
        {
            add { }
            remove { }
        }

        public bool CanExecute(object? parameter)
        {
            return true;
        }

        public void Execute(object? parameter)
        {
            _viewModel.NewGame();
        }
    }

}
public class GenerateRandomNumberCommandClass : ICommand
{
    private GameViewModel _viewModel;

    public GenerateRandomNumberCommandClass(GameViewModel viewModel)
    {
        _viewModel = viewModel;
    }

    public event EventHandler CanExecuteChanged
    {
        add { }
        remove { }
    }

    public bool CanExecute(object parameter)
    {
        return true;
    }

    public void Execute(object parameter)
    {
        _viewModel.GenerateRandomNumber();
    }
}