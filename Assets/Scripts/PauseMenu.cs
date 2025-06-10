using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public GameObject pauseUI;

    private bool isPaused = false;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePause();
        }
    }

    public void TogglePause()
    {
        isPaused = !isPaused;

        pauseUI.SetActive(isPaused);
        Time.timeScale = isPaused ? 0f : 1f;
    }

    public void OnResumeClicked()
    {
        TogglePause();
    }

    public GameObject optionsPanel; // Assign this in Inspector

    public void OnOptionClicked()
    {
        optionsPanel.SetActive(true);
    }

    public void OnSaveAndQuitClicked()
    {
        // Save game
        GameControl gc = GameObject.Find("GameControl").GetComponent<GameControl>();
        gc.SaveGame();

        // Optional: reset flag if you're returning to menu
        GameControl.hasGameStarted = false;

        Time.timeScale = 1f; // unpause before quitting or loading scene

        // Option 1: Return to main menu scene
        SceneManager.LoadScene("MainMenuScene");

        // Option 2: Or just quit game:
        // Application.Quit();
    }
}
