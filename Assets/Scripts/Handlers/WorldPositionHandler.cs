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
    public Collider entityPositionBounds;
    public Collider pickupPositionBounds;

    public void CreateWorldPositionObject(IWorldPositionObject worldPositionObject, out GameObject instance)
    {
        Collider collider = GetCollider(worldPositionObject.GetWorldPositionType());
        instance = Instantiate(worldPositionObject.GetWorldObjectPrefab(), collider.transform);
        instance.transform.position = GetRandomPointInBounds(collider.bounds);
    }
    
    public Collider GetCollider(WorldPositionType worldPositionType) {
        switch (worldPositionType)
        {
            case WorldPositionType.ENTITY:
                return entityPositionBounds;
            case WorldPositionType.PICKUP:
                return pickupPositionBounds;
            case WorldPositionType.IMAGE:
                return imagePositionBounds;
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

    public void PlayAnimation(IWorldPositionObject worldPositionObject, string animation)
    {
        if (!GetAnimator(worldPositionObject, out Animator animator))
            return;
        animator.Play(animation);
    }
    public bool GetAnimator(IWorldPositionObject worldPositionObject, out Animator animator)
    {
        animator = null;
        if (worldPositionObject.instance == null)
            return false;
        animator = worldPositionObject.instance.GetComponentInChildren<Animator>();
        return animator != null;
    }
    public bool GetRenderer(IWorldPositionObject worldPositionObject, out Renderer renderer)
    {
        renderer = null;
        if (worldPositionObject.instance == null)
            return false;
        renderer = worldPositionObject.instance.GetComponentInChildren<Renderer>();
        return renderer != null;
    }
}
public interface IWorldPositionObject
{
    GameObject instance { get; set; }
    GameObject GetWorldObjectPrefab();
    WorldPositionType GetWorldPositionType();
}
