using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    public GameObject doorColliderNotEnter;

    public GameObject doorSpawnOnOtherRoom;
    public GameObject parentRoom;

    private void Awake()
    {
        parentRoom = this.transform.parent.gameObject;
    }

    public void UnlockDoor()
    {
        doorColliderNotEnter.SetActive(false);
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
