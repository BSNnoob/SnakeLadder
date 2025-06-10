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

    public GameObject optionsPanel;

    public void OnOptionClicked()
    {
        optionsPanel.SetActive(true);
    }

    public void OnSaveAndQuitClicked()
    {
        GameControl gc = GameObject.Find("GameControl").GetComponent<GameControl>();
        gc.SaveGame();

        GameControl.hasGameStarted = false;

        Time.timeScale = 1f;

        SceneManager.LoadScene("MainMenuScene");
    }
}
