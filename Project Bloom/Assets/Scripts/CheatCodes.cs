using UnityEngine;

public class CheatCodes : MonoBehaviour
{
    private GameManager gameManager;

    private KeyCode[] cheatKeyboard = {
        KeyCode.C, KeyCode.H, KeyCode.E, KeyCode.A, KeyCode.T
    };

    private KeyCode[] cheatGamepad = {
        KeyCode.JoystickButton5, // RB
        KeyCode.JoystickButton0, // A
        KeyCode.JoystickButton2, // X
        KeyCode.JoystickButton3, // Y
        KeyCode.JoystickButton1, // B
    };

    private int keyboardIndex = 0;
    private int gamepadIndex = 0;

    private bool cheatActive;

    void Start()
    {
        GameObject gm = GameObject.Find("Game Manager");
        if (gm != null)
            gameManager = gm.GetComponent<GameManager>();
    }

    void Update()
    {
        if ((cheatActive) || gameManager.delay) return;

        // Catch ANY key/button pressed this frame
        foreach (KeyCode key in System.Enum.GetValues(typeof(KeyCode)))
        {
            if (Input.GetKeyDown(key))
            {
                ProcessInput(key);
                break; // only process one per frame
            }
        }
    }

    void ProcessInput(KeyCode key)
    {
        bool correctKeyboard = key == cheatKeyboard[keyboardIndex];
        bool correctGamepad = key == cheatGamepad[gamepadIndex];

        if (correctKeyboard)
            keyboardIndex++;
        else
            keyboardIndex = 0;

        if (correctGamepad)
            gamepadIndex++;
        else
            gamepadIndex = 0;

        if (keyboardIndex >= cheatKeyboard.Length ||
            gamepadIndex >= cheatGamepad.Length)
        {
            ActivateCheat();
        }
    }

    void ActivateCheat()
    {
        cheatActive = true;

        if (gameManager != null)
            gameManager.wave = 4;

        Debug.Log("CHEAT ACTIVATED!");
    }
}