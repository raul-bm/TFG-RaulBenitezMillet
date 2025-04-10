using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraAdjustments : MonoBehaviour
{
    private Camera m_camera;

    void Start()
    {
        m_camera = GetComponent<Camera>();

        AdjustCameraOrtographicSize();
    }

    void AdjustCameraOrtographicSize()
    {
        float baseSize = 6f;
        float targetAspect = 16 / 9f;
        float currentAspect = (float)Screen.width / Screen.height;

        m_camera.orthographicSize = baseSize * (targetAspect / currentAspect);
    }
}
 