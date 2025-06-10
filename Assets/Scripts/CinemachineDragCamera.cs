using UnityEngine;
using Unity.Cinemachine;

public class CinemachineManualOrbit : MonoBehaviour
{
    public CinemachineCamera cmCamera;
    public float orbitDistance = 10f;
    public float orbitHeight = 2f;
    public float rotationSpeed = 0;

    private float currentAngle = 0f;

    private void Update()
    {
        if (cmCamera == null || cmCamera.Follow == null)
            return;

        if (Input.GetMouseButtonDown(1))
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        if (Input.GetMouseButtonUp(1))
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }

        if (Input.GetMouseButton(1))
        {
            float mouseX = Input.GetAxis("Mouse X");
            currentAngle += mouseX * 120 * Time.deltaTime;
        }

        UpdateCameraPosition();
    }

    private void UpdateCameraPosition()
    {
        Transform target = cmCamera.Follow;
        if (target == null) return;

        // Orbit around the follow target
        Vector3 offset = new Vector3(
            Mathf.Sin(currentAngle * Mathf.Deg2Rad) * orbitDistance,
            orbitHeight,
            Mathf.Cos(currentAngle * Mathf.Deg2Rad) * orbitDistance
        );

        cmCamera.transform.position = target.position + offset;
        cmCamera.transform.LookAt(target.position + Vector3.up * orbitHeight);
    }
}
