using UnityEngine;
using Unity.Cinemachine;

public class MainMenu : MonoBehaviour
{
    public GameObject menuUI;
    public Transform menuCameraPoint;
    public Transform gameplayCameraTarget;
    public CinemachineCamera cmCamera;

    void Start()
    {
        Time.timeScale = 0f;

        cmCamera.Target.TrackingTarget = menuCameraPoint;

        Time.timeScale = 0f;
    }

    public void OnPlayClicked()
    {
        GameControl.hasGameStarted = true;
        PlayerPrefs.DeleteAll();

        cmCamera.Target.TrackingTarget = gameplayCameraTarget;

        menuUI.SetActive(false);
        Time.timeScale = 1f;
    }

    public void OnExitClicked()
    {
        Application.Quit();
    }

    public void OnContinueClicked()
    {
        menuUI.SetActive(false);
        Time.timeScale = 1f;

        GameControl.hasGameStarted = true;
        GameObject.Find("GameControl").GetComponent<GameControl>().LoadGame();
    }

}
