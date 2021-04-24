using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityWorldRenderer : MonoBehaviour
{
    public GameObject EntityParticlePrefab;

    private void Start()
    {
        Render();
    }
    public void Render()
    {
        Instantiate(EntityParticlePrefab);
    }

    public void Attack()
    {

    }
}
