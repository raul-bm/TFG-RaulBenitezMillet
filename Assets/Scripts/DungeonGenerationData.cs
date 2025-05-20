using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum LevelDifficulty
{
    Early,
    Middle,
    Final,
    Postgame
}

[CreateAssetMenu(fileName = "DungeonGenerationData", menuName = "DungeonScriptableObjects/DungeonGenerationData")]
public class DungeonGenerationData : ScriptableObject
{
    public LevelDifficulty difficulty;
    public int minRooms;
    public int maxRooms;
    public int rewardRooms;
    public List<SwordPartScripteable> poolObjectsRewardRoom;
}
