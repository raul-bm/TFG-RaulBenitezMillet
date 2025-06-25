using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MinimapManager : MonoBehaviour
{
    public static MinimapManager Instance { get; private set; }

    public GameObject roomIconPrefab;
    public RectTransform minimapContent;
    public RectTransform playerIcon;

    public float spacingHorizontal = 60f;
    public float spacingVertical = 35f;

    public float movingSpeedHorizontal = 0.3f;
    public float movingSpeedVertical = 0.2f;

    // [0] - visited // [1] - showed
    public List<Color> normalColors = new List<Color>() { new Color(1f, 1f, 1f, 0.7f), new Color(0.7f, 0.7f, 0.7f, 0.7f) };
    public List<Color> rewardColors = new List<Color>();
    public List<Color> bossColors = new List<Color>();

    private Dictionary<Vector2Int, GameObject> roomsShowed = new Dictionary<Vector2Int, GameObject>();
    private List<Vector2Int> roomsVisited = new List<Vector2Int>();

    private Coroutine moveCoroutine;

    private void Awake()
    {
        Instance = this;
    }

    public void RevealRoom(Vector2Int playerRoomPos, RoomNode roomActualNode)
    {
        if(!roomsVisited.Contains(playerRoomPos))
        {
            List<Color> thisColors = new List<Color>();

            switch (roomActualNode.roomType)
            {
                case RoomType.Reward:
                    thisColors = rewardColors;
                    break;
                case RoomType.Boss:
                    thisColors = bossColors;
                    break;
                default:
                    thisColors = normalColors;
                    break;
            }

            // Room icon instantiate
            if (!roomsShowed.ContainsKey(playerRoomPos))
            {
                GameObject roomIconInstance = Instantiate(roomIconPrefab, minimapContent);
                roomIconInstance.transform.localPosition = new Vector3(playerRoomPos.x * spacingHorizontal, playerRoomPos.y * spacingVertical);
                roomIconInstance.GetComponent<Image>().color = thisColors[0];
                roomsShowed.Add(playerRoomPos, roomIconInstance);
                roomsVisited.Add(playerRoomPos);
            }
            else
            {
                roomsShowed[playerRoomPos].GetComponent<Image>().color = thisColors[0];
                roomsVisited.Add(playerRoomPos);
            }
            
            foreach(RoomNode roomDescendantNode in roomActualNode.descendants)
            {
                List<Color> descendantColors = new List<Color>();

                switch (roomDescendantNode.roomType)
                {
                    case RoomType.Reward:
                        descendantColors = rewardColors;
                        break;
                    case RoomType.Boss:
                        descendantColors = bossColors;
                        break;
                    default:
                        descendantColors = normalColors;
                        break;
                }

                if (!roomsShowed.ContainsKey(roomDescendantNode.position))
                {
                    GameObject roomOtherIconInstance = Instantiate(roomIconPrefab, minimapContent);
                    roomOtherIconInstance.transform.localPosition = new Vector3(roomDescendantNode.position.x * spacingHorizontal, roomDescendantNode.position.y * spacingVertical);
                    roomOtherIconInstance.GetComponent<Image>().color = descendantColors[1];
                    roomsShowed.Add(roomDescendantNode.position, roomOtherIconInstance);
                }
            }
        }

        Vector3 targetPosition = new Vector3(-(playerRoomPos.x * spacingHorizontal), -(playerRoomPos.y * spacingVertical));

        if (moveCoroutine != null)
            StopCoroutine(moveCoroutine);

        moveCoroutine = StartCoroutine(SmoothMoveMinimap(targetPosition));
    }

    IEnumerator SmoothMoveMinimap(Vector3 targetPosition, float duration = 0.3f)
    {
        Vector3 startPosition = minimapContent.localPosition;
        float timeElapsed = 0f;

        while(timeElapsed < duration)
        {
            timeElapsed += Time.deltaTime;
            float t = Mathf.Clamp01(timeElapsed / duration);
            minimapContent.localPosition = Vector3.Lerp(startPosition, targetPosition, t);
            yield return null;
        }

        minimapContent.localPosition = targetPosition;
    }

    public void ResetMinimap()
    {
        minimapContent.localPosition = Vector3.zero;

        for(int i = minimapContent.childCount - 1; i >= 0; i--) Destroy(minimapContent.GetChild(i).gameObject);

        roomsShowed = new Dictionary<Vector2Int, GameObject>();
        roomsVisited = new List<Vector2Int>();
    }
}
