using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ImageEntity : ComponentWithIP, IDiscoverable
{
    [Header("Discoverable")]
    public bool isDiscovered;
    private DiscoveryEvent onDiscover = new DiscoveryEvent();
    private DiscoveryEvent onForget = new DiscoveryEvent();

    [Header("ImageEntity")]
    public Sprite image;
    public GameObject prefab;
    private MeshRenderer meshRenderer;

    public bool IsDiscovered() => isDiscovered;

    public virtual void Discover()
    {
        isDiscovered = true;
        GetOnDiscover().Invoke(this as IDiscoverable, true);
        meshRenderer = Instantiate(prefab).GetComponent<MeshRenderer>();
        meshRenderer.material.mainTexture = image.texture;
        Debug.Log($"{image.ToString()} imageentity yo");
    }

    public virtual void Forget()
    {
        isDiscovered = false;
        if (meshRenderer != null)
            Destroy(meshRenderer.gameObject);
        GetOnForget().Invoke(this as IDiscoverable, false);
    }
    public virtual DiscoveryEvent GetOnDiscover()
    {
        return onDiscover;
    }
    public virtual DiscoveryEvent GetOnForget()
    {
        return onForget;
    }
}
