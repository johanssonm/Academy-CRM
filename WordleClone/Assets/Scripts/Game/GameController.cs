using UnityEngine;
using WordleCore;
using System;

// Placeholder interfaces/classes for View components
// In a real project, these would be separate MonoBehaviours attached to UI objects
public class BoardView : MonoBehaviour
{
    public void SetupGrid() { }
    public void UpdateCurrentGuess(int row, string guess) { }
    public void ShowResult(int row, GuessResult result) { }
}

public class KeyboardView : MonoBehaviour
{
    public void ResetKeys() { }
    public void UpdateKeys(GuessResult result) { }
}

public class UIManager : MonoBehaviour
{
    public void ShowMessage(string message) { }
    public void ShowError(string error) { }
    public void ShowStats(PlayerStats stats) { }
}

public class GameController : MonoBehaviour
{
    [Header("Dependencies")]
    [SerializeField] private BoardView boardView;
    [SerializeField] private KeyboardView keyboardView;
    [SerializeField] private UIManager uiManager;

    private WordleGame _game;
    private WordGenerator _wordGenerator;
    private StatisticsManager _statsManager;
    private string _currentGuess = "";
    private DateTime _gameDate;

    private void Start()
    {
        _wordGenerator = new WordGenerator();
        _statsManager = LoadStats();

        StartNewGame();
    }

    private void StartNewGame()
    {
        // Daily Word Logic: Use DateTime.Today and cache it
        _gameDate = DateTime.Today;
        string targetWord = _wordGenerator.GetDailyWord(_gameDate);

        // Initialize Core Game Logic
        _game = new WordleGame(targetWord);
        _currentGuess = "";

        // Reset View
        if (boardView) boardView.SetupGrid();
        if (keyboardView) keyboardView.ResetKeys();
        if (uiManager) uiManager.ShowMessage("Guess the Word!");
    }

    private StatisticsManager LoadStats()
    {
        // Simple PlayerPrefs implementation for persistence
        if (!PlayerPrefs.HasKey("WordleStats"))
        {
            return new StatisticsManager();
        }

        string json = PlayerPrefs.GetString("WordleStats");
        try
        {
            var stats = JsonUtility.FromJson<PlayerStats>(json);
            return new StatisticsManager(stats);
        }
        catch
        {
            return new StatisticsManager();
        }
    }

    private void SaveStats()
    {
        string json = JsonUtility.ToJson(_statsManager.Stats);
        PlayerPrefs.SetString("WordleStats", json);
        PlayerPrefs.Save();
    }

    // Called by InputHandler or UI Buttons
    public void AddLetter(char letter)
    {
        if (_game == null || _game.Status != GameStatus.Playing) return;

        if (_currentGuess.Length < _game.TargetWord.Length)
        {
            _currentGuess += letter;
            if (boardView) boardView.UpdateCurrentGuess(_game.AttemptsUsed, _currentGuess);
        }
    }

    public void RemoveLetter()
    {
        if (_game == null || _game.Status != GameStatus.Playing) return;

        if (_currentGuess.Length > 0)
        {
            _currentGuess = _currentGuess.Substring(0, _currentGuess.Length - 1);
            if (boardView) boardView.UpdateCurrentGuess(_game.AttemptsUsed, _currentGuess);
        }
    }

    public void SubmitGuess()
    {
        if (_game == null || _game.Status != GameStatus.Playing) return;

        var result = _game.SubmitGuess(_currentGuess);

        if (!result.IsValid)
        {
            if (uiManager) uiManager.ShowError(result.ErrorMessage);
            return; // Don't clear guess, let them fix it (e.g., incomplete word)
        }

        // Visualize results
        // Note: AttemptsUsed is incremented inside SubmitGuess, so the index for the row we just filled is AttemptsUsed - 1
        if (boardView) boardView.ShowResult(_game.AttemptsUsed - 1, result);
        if (keyboardView) keyboardView.UpdateKeys(result);

        // Check Game Over
        if (_game.Status == GameStatus.Won)
        {
            if (uiManager) uiManager.ShowMessage("You Won!");
            _statsManager.RecordGame(true, _game.AttemptsUsed, _gameDate);
            SaveStats();
            if (uiManager) uiManager.ShowStats(_statsManager.Stats);
        }
        else if (_game.Status == GameStatus.Lost)
        {
            if (uiManager) uiManager.ShowMessage($"Game Over. Word was: {_game.TargetWord}");
            _statsManager.RecordGame(false, _game.AttemptsUsed, _gameDate);
            SaveStats();
            if (uiManager) uiManager.ShowStats(_statsManager.Stats);
        }

        _currentGuess = "";
    }
}
