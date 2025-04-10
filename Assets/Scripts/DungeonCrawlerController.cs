using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Direction
{
    up,
    left,
    down,
    right
}

public class DungeonCrawlerController : MonoBehaviour
{
    public static List<Vector2Int> positionsVisited = new List<Vector2Int>();
    private static readonly Dictionary<Direction, Vector2Int> directionMovementMap = new Dictionary<Direction, Vector2Int>
    {
        {Direction.up, Vector2Int.up },
        {Direction.left, Vector2Int.left },
        {Direction.down, Vector2Int.down },
        {Direction.right, Vector2Int.right }
    };

    public static List<Vector2Int> GenerateDungeon(DungeonGenerationData dungeonData)
    {
        DungeonCrawler dungeonCrawler = new DungeonCrawler(Vector2Int.zero);

        int positionsOnDungeon = Random.Range(dungeonData.salasMin, dungeonData.salasMax);

        while(positionsVisited.Count < positionsOnDungeon)
        {
            Vector2Int newPos = dungeonCrawler.Move(directionMovementMap);
            positionsVisited.Add(newPos);
        }

        return positionsVisited;
    }
}
