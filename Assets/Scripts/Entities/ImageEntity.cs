using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ImageEntity : ComponentWithIP, IDiscoverable, IWorldPositionObject
{
    [Header("Discoverable")]
    public bool isDiscovered;
    private DiscoveryEvent onDiscover = new DiscoveryEvent();
    private DiscoveryEvent onForget = new DiscoveryEvent();

    [Header("WorldPositionObject")]
    public GameObject prefab;
    public WorldPositionType positionType;
    private Animator animator;
    private Renderer renderer;
    private GameObject instance;

    [Header("ImageEntity")]
    public Sprite image;

    public override void StartRegister()
    {
        base.StartRegister();
    }

    public virtual void Discover()
    {
        WorldPositionHandler.I.ShiftImagesBackwards();
        WorldPositionHandler.I.CreateWorldPositionObject(this, out instance, out animator, out renderer);
        renderer.material.mainTexture = image.texture;
    }
    public virtual void Forget() 
    {
        if (renderer == null)
            return;
        animator.Play("ImageOut");
        Destroy(instance, 0.66f);
    }
    public bool IsDiscovered() => isDiscovered;
    public void SetIsDiscovered(bool isDiscovered) => this.isDiscovered = isDiscovered;
    public virtual DiscoveryEvent GetOnDiscover() => onDiscover;
    public virtual DiscoveryEvent GetOnForget() => onForget;
    public WorldPositionType GetWorldPositionType() => positionType;
    public Renderer GetRenderer() => renderer;
    public Animator GetAnimator() => animator;
    public GameObject GetInstance() => instance;
    public GameObject GetPrefab() => prefab;

}
