using System;
using System.Collections.Generic;
using System.Linq;

namespace WordleCore
{
    public enum LetterState
    {
        Absent,
        WrongPosition,
        Correct
    }

    public enum GameStatus
    {
        Playing,
        Won,
        Lost
    }

    public struct LetterResult
    {
        public char Letter;
        public LetterState State;

        public LetterResult(char letter, LetterState state)
        {
            Letter = letter;
            State = state;
        }
    }

    public class GuessResult
    {
        public LetterResult[]? LetterResults { get; }
        public bool IsValid { get; }
        public string? ErrorMessage { get; }

        public GuessResult(LetterResult[] results)
        {
            LetterResults = results;
            IsValid = true;
            ErrorMessage = null;
        }

        public GuessResult(string errorMessage)
        {
            LetterResults = null;
            IsValid = false;
            ErrorMessage = errorMessage;
        }
    }

    public class WordleGame
    {
        public string TargetWord { get; private set; }
        public int MaxAttempts { get; private set; }
        public int AttemptsUsed { get; private set; }
        public GameStatus Status { get; private set; }

        public WordleGame(string targetWord, int maxAttempts = 6)
        {
            if (string.IsNullOrEmpty(targetWord))
                throw new ArgumentException("Target word cannot be null or empty.");

            TargetWord = targetWord.ToUpper();
            MaxAttempts = maxAttempts;
            AttemptsUsed = 0;
            Status = GameStatus.Playing;
        }

        public GuessResult SubmitGuess(string guess)
        {
            if (Status != GameStatus.Playing)
            {
                return new GuessResult("Game is already over.");
            }

            if (string.IsNullOrEmpty(guess) || guess.Length != TargetWord.Length)
            {
                return new GuessResult($"Guess must be {TargetWord.Length} letters long.");
            }

            guess = guess.ToUpper();

            // Note: Dictionary validation (IsWordValid) would typically be injected here.

            AttemptsUsed++;

            var results = new LetterResult[TargetWord.Length];
            var targetCharCounts = new Dictionary<char, int>();

            // Count frequencies in target word
            foreach (var c in TargetWord)
            {
                if (!targetCharCounts.ContainsKey(c))
                    targetCharCounts[c] = 0;
                targetCharCounts[c]++;
            }

            // First pass: Find correct letters (Green)
            for (int i = 0; i < guess.Length; i++)
            {
                char guessChar = guess[i];
                if (TargetWord[i] == guessChar)
                {
                    results[i] = new LetterResult(guessChar, LetterState.Correct);
                    targetCharCounts[guessChar]--;
                }
            }

            // Second pass: Find wrong position (Yellow) or absent (Gray)
            for (int i = 0; i < guess.Length; i++)
            {
                // Skip if already marked correct
                if (results[i].State == LetterState.Correct)
                    continue;

                char guessChar = guess[i];
                if (targetCharCounts.ContainsKey(guessChar) && targetCharCounts[guessChar] > 0)
                {
                    results[i] = new LetterResult(guessChar, LetterState.WrongPosition);
                    targetCharCounts[guessChar]--;
                }
                else
                {
                    results[i] = new LetterResult(guessChar, LetterState.Absent);
                }
            }

            if (guess == TargetWord)
            {
                Status = GameStatus.Won;
            }
            else if (AttemptsUsed >= MaxAttempts)
            {
                Status = GameStatus.Lost;
            }

            return new GuessResult(results);
        }
    }
}
