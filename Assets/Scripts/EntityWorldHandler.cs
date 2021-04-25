using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityWorldHandler : MonoBehaviour
{
    public static EntityWorldHandler I;

    public List<Transform> spawnPoints = new List<Transform>();

    private void Awake()
    {
        EntityWorldHandler.I = this;
    }

    public bool RenderEntity(GameObject entityParticlePrefab, out GameObject instantiatedParticleEntity)
    {
        instantiatedParticleEntity = null;

        if (!FindFirstFreeSpawnPoint(out Transform spawnPoint))
            return false;

        instantiatedParticleEntity = Instantiate(entityParticlePrefab, spawnPoint);

        return true;
    }

    public bool FindFirstFreeSpawnPoint(out Transform t)
    {
        t = null;
        foreach(Transform spawnPoint in spawnPoints)
        {
            if (spawnPoint.childCount > 0)
                continue;

            t = spawnPoint;
            return true; 
        }

        return false;
    }
}
