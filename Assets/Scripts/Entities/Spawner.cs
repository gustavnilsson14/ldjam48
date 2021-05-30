using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour, IDamageable, IGeneratedHostInhabitant, IDiscoverable, IWorldPositionObject, IEntitySpawner
{
    public GameObject worldObjectPrefab;
    public List<GameObject> spawnPrefabs;
    public int maxIP;
    public float rarity;
    public bool generatesInPriorityDirectory;
    public bool generatesInBranchDirectory;
    public bool generatesInLeafDirectory;
    public string description;
    [Range(1,99)]
    public int spawnFrequency;
    public int maxOffspring = 1;

    private void Start()
    {
        InitDamageable();
        InitDiscoverable();
        InitSpawner();
    }

    public void InitDamageable()
    {
        DamageHandler.I.InitDamageable(this);
    }
    public void InitDiscoverable()
    {
        DiscoveryHandler.I.InitDiscoverable(this);
    }
    public void InitSpawner()
    {
        SpawnerHandler.I.InitSpawner(this);
    }
    public void OnTerminalTimePast(int time)
    {
        currentMomentum -= time;
        if (currentMomentum > 0)
            return;
        currentMomentum = GetSpawnFrequency();
        SpawnerHandler.I.SpawnEntity(out List<Entity> entities, this);
    }
    public bool GetSpawnPrefabs(out List<GameObject> result) {
        result = RandomUtil.Shuffle<GameObject>(spawnPrefabs);
        if (result.Count == 0)
            return false;
        result = result.GetRange(0,1);
        return true;
    }
    public List<string> FormatCatDescription(List<string> catDescription)
    {
        catDescription.AddRange(
            new List<string> {
                $"IP: {currentIP}",
                //$"UserGroup: {faction.ToString()}",
            }
        );
        return catDescription;
    }
    public bool IsBaseComponent() => true;
    public Directory currentDirectory { get; set; }
    public bool alive { get; set; }
    public int currentIP { get; set; }
    public TakeHitEvent onTakeHit { get; set; }
    public ArmorDamageEvent onArmorDamage { get; set; }
    public BodyDamageEvent onBodyDamage { get; set; }
    public DirectDamageEvent onDirectDamage { get; set; }
    public HitTakenEvent onHitTaken { get; set; }
    public HealEvent onHeal { get; set; }
    public DeathEvent onDeath { get; set; }
    public DiscoveryEvent onDiscover { get; set; }
    public DiscoveryEvent onForget { get; set; }
    public CatEvent onCat { get; set; }
    public bool discovered { get; set; }
    public GameObject instance { get; set; }
    public EntitySpawnEvent onSpawn { get; set; }
    public List<GameObject> offspring { get; set; }
    public int currentMomentum { get; set; }

    public string GetName() => name;
    public string GetShortDescription() => description;
    public string GetFileName() => name;
    public bool GeneratesInLeafDirectory() => generatesInLeafDirectory;
    public bool GeneratesInBranchDirectory() => generatesInBranchDirectory;
    public bool GeneratesInPriorityDirectory() => generatesInPriorityDirectory;
    public float GetRarity() => rarity;
    public WorldPositionType GetWorldPositionType() => WorldPositionType.ENTITY;
    public GameObject GetWorldObjectPrefab() => worldObjectPrefab;
    public Transform GetTransform() => transform;
    public int GetMaxIP() => maxIP;
    public GameObject GetGameObject() => gameObject;
    public int GetSpawnFrequency() => spawnFrequency;
    public int GetMaxOffspring() => maxOffspring;

}
