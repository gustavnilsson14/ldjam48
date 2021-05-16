using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ImageEntity : MonoBehaviour, IDiscoverable, IWorldPositionObject
{
    [Header("WorldPositionObject")]
    public GameObject prefab;

    [Header("ImageEntity")]
    public Sprite image;
    public string description;

    public void Start()
    {
        InitDiscoverable();
    }
    public void InitDiscoverable()
    {
        DiscoveryHandler.I.InitDiscoverable(this);
        onDiscover.AddListener(OnDiscover);
        onForget.AddListener(OnForget);
    }
    private void OnDiscover(IDiscoverable arg0, bool arg1)
    {
        WorldPositionHandler.I.ShiftImagesBackwards();
        WorldPositionHandler.I.CreateWorldPositionObject(this, out GameObject worldObjectInstance);
        instance = worldObjectInstance;
        if (!WorldPositionHandler.I.GetRenderer(this, out Renderer renderer))
            return;
        renderer.material.SetTexture("_BaseMap", image.texture);
    }
    private void OnForget(IDiscoverable arg0, bool arg1)
    {
        WorldPositionHandler.I.PlayAnimation(this, "ImageOut");
        Destroy(instance, 0.66f);
    }
    public WorldPositionType GetWorldPositionType() => WorldPositionType.IMAGE;
    public GameObject GetWorldObjectPrefab() => prefab;
    public string GetName() => name;
    public GameObject GetGameObject() => gameObject;
    public string GetFileName() => name;

    public List<string> FormatCatDescription(List<string> catDescription)
    {
        return catDescription;
    }
    public string GetShortDescription() => description;
    public GameObject instance { get; set; }
    public DiscoveryEvent onDiscover { get; set; }
    public DiscoveryEvent onForget { get; set; }
    public bool discovered { get; set; }
    public Directory currentDirectory { get; set; }
    public CatEvent onCat { get; set; }
}
