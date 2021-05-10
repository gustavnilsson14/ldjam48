using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class DiscoveryHandler : Handler
{
    public static DiscoveryHandler I { get; private set; }
    public List<IDiscoverable> discovered = new List<IDiscoverable>();

    private void Awake()
    {
        DiscoveryHandler.I = this;
    }
    protected override void StartRegister()
    {
        base.StartRegister();
        Player.I.onMove.AddListener(OnPlayerMove);
    }
    /*
     
    public virtual void Discover()
    {
        isDiscovered = true;
        GetOnDiscover().Invoke(this as IDiscoverable, true);
        WorldPositionHandler.I.CreateWorldPositionObject(this, out animator, out renderer);
        renderer.material.mainTexture = image.texture;
    }

    public virtual void Forget()
    {
        isDiscovered = false;
        GetOnForget().Invoke(this as IDiscoverable, false);
        if (renderer == null)
            return;
        animator.Play("ImageOut");
        Destroy(renderer.gameObject, 0.66f);
    }
     */
    public bool Discover(IDiscoverable discoverable)
    {
        if (discovered.Contains(discoverable))
            return false;
        discoverable.Discover();
        discoverable.SetIsDiscovered(true);
        discoverable.GetOnDiscover().Invoke(discoverable, false);
        discovered.Add(discoverable);
        return true;
    }
    private void OnPlayerMove(Directory arg0, Directory arg1)
    {
        ForgetAll();
    }
    public void ForgetAll()
    {
        discovered.ForEach(d => Forget(d));
        discovered.Clear();
    }
    public void Forget(IDiscoverable discoverable)
    {
        discoverable.Forget();
        discoverable.SetIsDiscovered(false);
        discoverable.GetOnForget().Invoke(discoverable, false);
    }

}
public interface IDiscoverable
{
    string GetName();
    bool IsDiscovered();
    void SetIsDiscovered(bool isDiscovered);
    void Discover();
    void Forget();
    DiscoveryEvent GetOnDiscover();
    DiscoveryEvent GetOnForget();
}
public class DiscoveryEvent : UnityEvent<IDiscoverable, bool> { }