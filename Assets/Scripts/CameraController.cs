using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public float cameraVerticalAdjust;
    private Vector3 targetPosition;
    private bool isMoving;
    private bool isVertical; // True -> moving in vertical, False -> moving in horizontal
    [SerializeField] public float movingSpeedHorizontal;
    [SerializeField] public float movingSpeedVertical;

    private void Update()
    {
        if(isMoving)
        {
            float movingSpeed = isVertical ? movingSpeedVertical : movingSpeedHorizontal;

            transform.position = Vector3.MoveTowards(transform.position, targetPosition, movingSpeed);

            if (transform.position == targetPosition) isMoving = false;
        }
    }

    public void ChangeCameraPosition(GameObject room)
    {
        targetPosition = room.transform.position + new Vector3(0, cameraVerticalAdjust, this.transform.position.z);

        if (transform.position.x == room.transform.position.x) isVertical = true;
        else isVertical = false;
        
        isMoving = true;
    }

    public void ResetCameraLevel()
    {
        transform.position = new Vector3(0, cameraVerticalAdjust, this.transform.position.z);
    }
}
