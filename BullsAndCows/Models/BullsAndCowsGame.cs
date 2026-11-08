using System;
using System.Collections.Generic;
using System.Text;

namespace BullsAndCows.Models
{
    public class BullsAndCowsGame
    {
        private const int CodeLength = 4;
        private Random _random = new Random();
        private string _secretNumber = string.Empty;

        public int AttemptCount { get; private set; }

        public bool IsGameOver { get; private set; }

        public int RandomNumber { get; private set; }
        public void GenerateRandomNumber()
        {
            RandomNumber = _random.Next(1000, 10000);
        }
        public int BestScore { get; private set; } = 0; //in the bonus part
        public BullsAndCowsGame()
        {
            StartNewGame();
        }
        public void StartNewGame()
        {
            _secretNumber = GenerateSecretNumber();
            AttemptCount = 0;
            IsGameOver = false;
        }

        private string GenerateSecretNumber()
        {
            string secret = "";
            int FirstD = _random.Next(1, 10);
            secret += FirstD.ToString();
            while (secret.Length < CodeLength)
            {
                int nextDigit = _random.Next(0, 10);
                if (!Contains(secret, nextDigit))
                {
                    secret += nextDigit.ToString();
                }
            }
            return secret;
        }

        // פעולת עזר לפעולה למעלה הבודקת האם המספר מופיע במחרוזת
        private bool Contains(string number, int num)
        {
            for (int i = 0; i < number.Length; i++)
            {
                if (number[i] - '0' == num)
                {
                    return true;
                }
            }
            return false;
        }


        public static bool IsValidGuess(string guess, out string errorMessage)
        {
            errorMessage = string.Empty;
            if (guess.Length != CodeLength)
            {
                errorMessage = "The guess must have four digits";
                return false;
            }

            for (int i = 0; i < CodeLength; i++)
            {
                bool isNumber = false;
                for (int j = 0; j <= 9; j++)
                {
                    int digitValue = guess[i] - '0';   //'5' - '0' = 5
                    if (digitValue == j)
                    {
                        isNumber = true;
                    }
                }
                if (!isNumber)
                {
                    errorMessage = "The guess should contain only numbers";
                    return false;
                }
            }
            for (int i = 0; i < CodeLength; i++)
            {
                for (int j = 0; j < CodeLength; j++)
                {
                    if (i != j)
                    {
                        if (guess[i] == guess[j])
                        {
                            errorMessage = "All digits must be different from each other";
                            return false;
                        }
                    }
                }
            }
            return true;
        }

        public GuessResult SubmitGuess(string guess)
        {
            if (IsGameOver)
            {
                throw new InvalidOperationException("The game is already over."); //
            }

            AttemptCount++;
            int bulls = 0;
            int cows = 0;
            for (int i = 0; i < guess.Length; i++)
            {
                if (guess[i] == _secretNumber[i])
                {
                    bulls++;
                }
            }
            for (int i = 0; i < guess.Length; i++)
            {
                if (guess[i] != _secretNumber[i])
                {
                    for (int j = 0; j < guess.Length; j++)
                    {
                        if (j != i && guess[i] == _secretNumber[j])
                        {
                            cows++;
                            break;
                        }
                    }
                }
            }
            GuessResult result = new GuessResult();
            result.Bulls = bulls;
            result.Cows = cows;

            if (result.IsWinningGuess) // אם הנסיון הוא הנכון המשחק נגמר
            {
                IsGameOver = true;
            }
            if (result.IsWinningGuess) //כדי למצוא את הנסיון הכי טוב   
            {
                IsGameOver = true;

                if (BestScore == 0 || AttemptCount < BestScore) //bonus
                {
                    BestScore = AttemptCount;
                }
            }
            return result;
        }


    }
}


