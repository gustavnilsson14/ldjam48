using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum SpawnType
{
    ENEMY,
    FILE
}

public class WorldSpawnPoint : MonoBehaviour
{
    public SpawnType spawnType = SpawnType.ENEMY;
}
