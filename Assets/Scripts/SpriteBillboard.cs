using Unity.Cinemachine;
using UnityEngine;

public class SpriteBillboard : MonoBehaviour
{
    private Transform camTransform;

    void Start()
    {

        GameControl gc = GameObject.Find("GameControl").GetComponent<GameControl>();
        camTransform = gc.cmCamera.transform;

    }

    void LateUpdate()
    {
        if (camTransform == null) return;

        Vector3 lookDir = transform.position - camTransform.position;
        lookDir.y = 0f;
        transform.rotation = Quaternion.LookRotation(lookDir);
    }
}