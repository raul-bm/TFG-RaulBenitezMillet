using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sword : MonoBehaviour
{
    //private SwordPart pommelPart;
    //private SwordPart gripPart;
    //private SwordPart crossguardPart;
    //private SwordPart bladePart;

    [SerializeField] private GameObject player;
    [SerializeField] private float distanceFromPlayer;
    [SerializeField] private float rotationSpeed;

    private void Update()
    {
        // Position of the mouse on the screen
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0f;

        // Direction for the sword
        Vector3 direction = (mousePos - player.transform.position).normalized;

        // Distance from the player
        transform.position = player.transform.position + direction * distanceFromPlayer;

        // Rotation of the sword
        float angle = Mathf.Atan2(direction.x, direction.y) * Mathf.Rad2Deg;
        Quaternion targetRotation = Quaternion.Euler(0f, 0f, -angle);

        transform.rotation = Quaternion.RotateTowards(
            transform.rotation,
            targetRotation,
            rotationSpeed * Time.deltaTime);
    }
}
