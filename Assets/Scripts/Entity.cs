using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public enum EntityFaction
{
    HACKER,
    SECURITY,
    VIRUS
}

public class Entity : ComponentWithIP
{
    public EntityFaction faction;

    [TextArea(2, 10)]
    public string description;

    [Header("Events")]
    public MoveEvent onMove = new MoveEvent();
    public CatEvent onCat = new CatEvent();
    public DiscoverEvent onDiscover = new DiscoverEvent();
    public AttackEvent onAttack = new AttackEvent();
    public PlayerEscapeEvent onPlayerEscape = new PlayerEscapeEvent();

    private bool isDiscovered = false;

    protected override void Awake()
    {
        base.Awake();
        currentDirectory = GetComponentInParent<Directory>();
    }
    public void MoveTo(Directory directory)
    {
        currentDirectory = directory;
        transform.parent = directory.transform;
        onMove.Invoke(directory);
    }
    public virtual string GetCatDescription()
    {
        List<string> result = new List<string> {
            GetBinaryStatic(),
            string.Format("IP: {0}", currentIP),
            description
        };
        onCat.Invoke();
        return string.Join("\n", result);
    }

    public virtual void Attack()
    {
        onAttack.Invoke();
    }

    public virtual void Discover()
    {
        Debug.Log($"discover entity {isDiscovered}");
        if (isDiscovered)
            return;

        isDiscovered = true;
        onDiscover.Invoke();
        Player.I.onMove.AddListener(HandlePlayerMovement);
    }

    private void HandlePlayerMovement(Directory arg0)
    {
        Debug.Log($"Player moved {this.name}");
        isDiscovered = false;
        onPlayerEscape.Invoke();
        Player.I.onMove.RemoveListener(HandlePlayerMovement);
    }

    protected string GetBinaryStatic()
    {
        List<string> result = new List<string>();
        var rand = new System.Random();
        for (int i = 0; i < 10; i++)
        {
            result.Add(Convert.ToString(rand.Next(1024), 2).PadLeft(10, '0'));
        }
        return string.Join("\n", result);
    }
    public virtual bool IsAllowedInDirectory(Directory directory) {
        return (!directory.bannedFactions.Contains(faction));
    }
}
public class CatEvent : UnityEvent { }
public class DiscoverEvent : UnityEvent { }
public class PlayerEscapeEvent : UnityEvent { }
public class AttackEvent : UnityEvent { }
