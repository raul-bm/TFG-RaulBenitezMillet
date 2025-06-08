using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    public GameObject doorColliderNotEnter;

    public GameObject doorSpawnOnOtherRoom;
    public GameObject parentRoom;

    public Animator animator;

    private void Awake()
    {
        parentRoom = this.transform.parent.gameObject;
    }

    public void DoorOpened()
    {
        animator.SetTrigger("Open");
        doorColliderNotEnter.SetActive(false);
    }

    public void DoorClosed()
    {
        animator.SetTrigger("Close");
    }

    public void UnlockDoor()
    {
        doorColliderNotEnter.SetActive(false);
        animator.SetTrigger("OpenDoor");
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "Player")
        {
            other.transform.position = doorSpawnOnOtherRoom.transform.position;

            GameObject otherRoom = doorSpawnOnOtherRoom.transform.parent.GetComponent<Door>().parentRoom;

            MinimapManager.Instance.RevealRoom(otherRoom.GetComponent<Room>().thisRoomNode.position, otherRoom.GetComponent<Room>().thisRoomNode);

            other.GetComponent<PlayerController>().cameraController.ChangeCameraPosition(otherRoom);

            otherRoom.GetComponent<Room>().InitializeRoom();
        }
    }
}
