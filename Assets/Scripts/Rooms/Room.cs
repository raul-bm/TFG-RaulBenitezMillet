using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public abstract class Room : MonoBehaviour
{
    public RoomNode thisRoomNode;
    public GameObject[] doors; // 0: up - 1: left - 2: down - 3: right
    [SerializeField] protected TextMeshPro textMPRoom;
 
    protected bool isCleared = false;

    public void SetDoors()
    {
        if (thisRoomNode.parent != null) SetDoors(thisRoomNode.parent);

        foreach(RoomNode descendant in thisRoomNode.descendants)
        {
            SetDoors(descendant);
        }

        // Disable the doors without a room to go to
        foreach(GameObject door in doors)
        {
            if(door.GetComponent<Door>().doorSpawnOnOtherRoom == null)
            {
                door.SetActive(false);
            }
        }
    }

    private void SetDoors(RoomNode roomNode)
    {
        // UP
        if (roomNode.position.y > thisRoomNode.position.y)
        {
            SetSpecificDoor(roomNode, 0, 2);
        } // DOWN 
        else if (roomNode.position.y < thisRoomNode.position.y)
        {
            SetSpecificDoor(roomNode, 2, 0);
        } // LEFT 
        else if (roomNode.position.x < thisRoomNode.position.x)
        {
            SetSpecificDoor(roomNode, 1, 3);
        } // RIGHT
        else if (roomNode.position.x > thisRoomNode.position.x)
        {
            SetSpecificDoor(roomNode, 3, 1);
        }
    }

    private void SetSpecificDoor(RoomNode roomNode, int doorThisObject, int doorRoomNodeObject)
    {
        Room roomNodeRoomScript = roomNode.roomGameObject.GetComponent<Room>();

        GameObject doorSpawn = roomNodeRoomScript.doors[doorRoomNodeObject].transform.GetChild(0).gameObject;

        doors[doorThisObject].GetComponent<Door>().doorSpawnOnOtherRoom = doorSpawn;
    }

    #region DEBUG
    public void ChangeTextRoom()
    {
        if (thisRoomNode.roomType == RoomType.Initial) textMPRoom.text = "I";
        else if (thisRoomNode.roomType == RoomType.Boss) textMPRoom.text = "B";
        else if (thisRoomNode.roomType == RoomType.Reward) textMPRoom.text = "Reward";
        else textMPRoom.text = "R" + thisRoomNode.id;
    }
    #endregion

    public abstract void InitializeRoom();
}
