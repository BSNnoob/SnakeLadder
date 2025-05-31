using Unity.Cinemachine;
using UnityEngine;

public class SpriteBillboard : MonoBehaviour
{
    private Transform camTransform;

    void Start()
    {
        // Find the CinemachineCamera's transform (from GameControl or by tag)
        GameControl gc = GameObject.Find("GameControl").GetComponent<GameControl>();
        camTransform = gc.cmCamera.transform;

        // Alternatively, if the CinemachineCamera has a tag, you can do:
        // camTransform = GameObject.FindGameObjectWithTag("MainCamera").transform;
    }

    void LateUpdate()
    {
        if (camTransform == null) return;

        // Make the billboard face the camera
        Vector3 lookDir = transform.position - camTransform.position;
        lookDir.y = 0f; // Optional: ignore vertical angle to only rotate horizontally
        transform.rotation = Quaternion.LookRotation(lookDir);
    }
}