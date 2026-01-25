using UnityEngine;

public class InputHandler : MonoBehaviour
{
    [SerializeField] private GameController gameController;

    private void Update()
    {
        if (gameController == null) return;

        // Detect typing
        foreach (char c in Input.inputString)
        {
            if (char.IsLetter(c))
            {
                gameController.AddLetter(char.ToUpper(c));
            }
        }

        // Special keys
        if (Input.GetKeyDown(KeyCode.Backspace))
        {
            gameController.RemoveLetter();
        }

        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            gameController.SubmitGuess();
        }
    }
}
