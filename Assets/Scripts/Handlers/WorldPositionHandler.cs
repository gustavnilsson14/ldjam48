using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum WorldPositionType { 
    ENTITY, PICKUP, IMAGE
}
public class WorldPositionHandler : Handler
{
    public static WorldPositionHandler I;
    public Collider imagePositionBounds;
    public void CreateWorldPositionObject(IWorldPositionObject worldPositionObject, out GameObject instance, out Animator animator, out Renderer renderer)
    {
        Collider collider = GetCollider(worldPositionObject.GetWorldPositionType());
        instance = Instantiate(worldPositionObject.GetPrefab(), collider.transform);
        instance.transform.position = GetRandomPointInBounds(collider.bounds);
        animator = instance.GetComponentInChildren<Animator>();
        renderer = instance.GetComponentInChildren<Renderer>();
    }
    
    public Collider GetCollider(WorldPositionType worldPositionType) {
        switch (worldPositionType)
        {
            case WorldPositionType.ENTITY:
                break;
            case WorldPositionType.PICKUP:
                break;
            case WorldPositionType.IMAGE:
                break;
            default:
                break;
        }
        return imagePositionBounds;
    }
    public Vector3 GetRandomPointInBounds(Bounds bounds) {
        Vector3 result = new Vector3(
            UnityEngine.Random.Range(bounds.min.x, bounds.max.x),
            UnityEngine.Random.Range(bounds.min.y, bounds.max.y),
            UnityEngine.Random.Range(bounds.min.z, bounds.max.z)
        );
        return result;
    }
    public void ShiftImagesBackwards()
    {
        foreach (Transform child in imagePositionBounds.transform)
        {
            child.position += transform.forward*2;
        }
    }
}
public interface IWorldPositionObject
{
    WorldPositionType GetWorldPositionType();
    GameObject GetPrefab();
    Animator GetAnimator();
    Renderer GetRenderer();
    GameObject GetInstance();
}
