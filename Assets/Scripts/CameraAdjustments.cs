using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraAdjustments : MonoBehaviour
{
    private Camera m_camera;
    private float baseOrtographicSize;

    void Start()
    {
        m_camera = GetComponent<Camera>();
        baseOrtographicSize = m_camera.orthographicSize;

        AdjustCameraOrtographicSize();
    }

    void AdjustCameraOrtographicSize()
    {
        float targetAspect = 16 / 9f;
        float currentAspect = (float)Screen.width / Screen.height;

        m_camera.orthographicSize = baseOrtographicSize * (targetAspect / currentAspect);
    }
}
 