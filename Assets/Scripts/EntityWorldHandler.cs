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

    public void RemoveChildFromSpawnPointDelay(GameObject entityParticle, float delay)
    {
        StartCoroutine(RemoveRenderWithDelay(entityParticle, delay));
    }

    public IEnumerator RemoveRenderWithDelay(GameObject entityParticle, float delay)
    {
        yield return new WaitForSeconds(3);
        RemoveChildFromSpawnPoint(entityParticle);

    }
    public bool RemoveChildFromSpawnPoint(GameObject entityParticle)
    {
        foreach(Transform spawnPoint in spawnPoints)
        {
            if (spawnPoint.childCount == 0)
                continue;

            if (spawnPoint.GetChild(0).gameObject == entityParticle)
            {
                Destroy(spawnPoint.GetChild(0).gameObject);
                return true;
            }
        }

        return false;
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
