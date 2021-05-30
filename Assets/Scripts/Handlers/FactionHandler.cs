using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public enum FactionType
{
    NONE,
    HACKER,
    SECURITY,
    VIRUS
}

public class FactionHandler : Handler
{
    public static FactionHandler I;
    public void InitFactionMember(IFactionMember factionMember) {
        factionMember.onFactionChange = new FactionChangeEvent();
        factionMember.currentDirectory = GetComponentInParent<Directory>();
    }
    public bool UserMod(IFactionMember origin, IFactionMember target, FactionType newFaction) {

        return true;
    }
    public void ChangeFaction(IFactionMember factionMember, FactionType newFaction) {
        factionMember.onFactionChange.Invoke(factionMember, factionMember.faction, newFaction);
        factionMember.faction = newFaction;
    }
}

public interface IFactionMember {
    void InitFactionMember();
    FactionType faction { get; set; }
    FactionChangeEvent onFactionChange { get; set; }
    Directory currentDirectory { get; set; }
}
public class FactionChangeEvent : UnityEvent<IFactionMember, FactionType, FactionType> { }