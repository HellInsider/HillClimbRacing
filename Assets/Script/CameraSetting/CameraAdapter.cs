using UnityEngine;

public class CameraAdapter : MonoBehaviour
{
    public float defaultAspect = 16f / 9f;
    private Camera cam;

    void Start()
    {
        cam = Camera.main;
        AdjustCamera();
    }

    void AdjustCamera()
    {
        float currentAspect = (float)Screen.width / (float)Screen.height;

        if (currentAspect < defaultAspect)
        {
            cam.orthographicSize *= defaultAspect / currentAspect;
        }
    }
}
