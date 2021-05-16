using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Host))]
[RequireComponent(typeof(DirectoryKey))]
public class HostGenerator : MonoBehaviour
{
    public float challengeRating = 1;
    public float lootValue = 1;
    public float maxDirectories = 5;

    public List<Directory> leafDirectories;
    public List<Directory> branchDirectories;
    public List<Directory> priorityDirectories;
    private List<Directory> allDirectories;

    [Header("Prefabs")]
    public List<GameObject> challengePrefabs = new List<GameObject>();
    public List<GameObject> pickupPrefabs = new List<GameObject>();
    private List<IChallenge> challenges = new List<IChallenge>();
    private List<IPickup> pickups = new List<IPickup>();

    private Host host;
    private int levelIndex;
    private float levelIndexMultiplier;

    private DirectoryKey directoryKeyTemplate;
    private SshKey sshKey;

    private void Start()
    {
        Register();
    }

    private void Register()
    {
        host = GetComponent<Host>();
        directoryKeyTemplate = GetComponent<DirectoryKey>();
        challenges.AddRange(challengePrefabs.SelectMany(c => c.GetComponents<IChallenge>()));
        pickups.AddRange(pickupPrefabs.SelectMany(p => p.GetComponents<IPickup>()));
    }

    public void Run(int levelIndex = 1)
    {
        this.levelIndex = levelIndex;
        this.levelIndexMultiplier = ((float)levelIndex / 3) +1;
        StartCoroutine(Run());
    }
    public IEnumerator Run()
    {
        sshKey = gameObject.AddComponent<SshKey>();
        yield return 1;
        TrimFolders();
        yield return 1;
        RegisterDirectoryTypes();
        AddChallenges();
        AddPickups();
    }

    private void RegisterDirectoryTypes()
    {
        leafDirectories = host.GetLeafDirectories();
        leafDirectories = RandomUtil.Shuffle<Directory>(leafDirectories);
        branchDirectories = host.GetBranchDirectories();
        branchDirectories = RandomUtil.Shuffle<Directory>(branchDirectories);
        RegisterPriorityDirectories();
        allDirectories = new List<Directory>();
        allDirectories.AddRange(leafDirectories);
        allDirectories.AddRange(branchDirectories);
        allDirectories.AddRange(priorityDirectories);
    }

    private void RegisterPriorityDirectories()
    {
        int iterations = 99;
        List<Directory> availableDirectories = new List<Directory>();
        List<Directory> sshLocations = new List<Directory>();
        availableDirectories.AddRange(leafDirectories);
        availableDirectories.AddRange(branchDirectories);
        while (availableDirectories.Count > 2 && priorityDirectories.Count < levelIndex && iterations > 0)
        {
            iterations--;
            availableDirectories = RandomUtil.Shuffle<Directory>(availableDirectories);
            
            if (availableDirectories.Count == 0)
                return;
            Directory randomDirToLock = availableDirectories[0];
            randomDirToLock.GetComponentsInChildren<Directory>().ToList().ForEach(d => availableDirectories.Remove(d));
            if (availableDirectories.Count == 0)
                return;

            List<Directory> lockedLeaves = HostHandler.I.currentHost.GetLeafDirectories(randomDirToLock);
            if (lockedLeaves.Count == 0)
                continue;
            lockedLeaves = RandomUtil.Shuffle<Directory>(lockedLeaves);
            sshLocations.AddRange(lockedLeaves);

            Directory priorityDir = lockedLeaves[0];
            priorityDirectories.Add(priorityDir);
            Directory keyDirectory = availableDirectories[0];
            priorityDirectories.Add(keyDirectory);

            directoryKeyTemplate.path = randomDirToLock.GetFullPath();
            PickupEntity directoryKey = PickupHandler.I.CreatePickup(keyDirectory.transform, directoryKeyTemplate, true);
            randomDirToLock.bannedFactions.Add(EntityFaction.HACKER);
        }
        sshLocations = RandomUtil.Shuffle<Directory>(sshLocations);
        PickupEntity sshKey = PickupHandler.I.CreatePickup(sshLocations[0].transform, this.sshKey, true);
    }
    private void TrimFolders() {
        List<Directory> allDirectories = new List<Directory>(host.GetLeafDirectories());
        allDirectories.AddRange(host.GetBranchDirectories());
        allDirectories = RandomUtil.Shuffle<Directory>(allDirectories);
        int iterations = 9999;
        int currentMax = Mathf.FloorToInt(maxDirectories * levelIndexMultiplier);
        while (allDirectories.Count > currentMax && iterations > 0) {
            iterations--;
            int diff = allDirectories.Count - currentMax;
            Directory directory = allDirectories.Find(d => d.GetComponentsInChildren<Directory>().Length <= diff);
            directory.GetComponentsInChildren<Directory>().ToList().ForEach(d => allDirectories.Remove(d));
            Destroy(directory.gameObject);
        }
    }
    private void AddChallenges()
    {
        if (challenges.Count == 0)
            return;
        float totalChallengeRating = challengeRating * levelIndexMultiplier;
        int iterations = 9999;
        while (totalChallengeRating > 0 && iterations > 0)
        {
            iterations--;
            challenges = RandomUtil.ShuffleHostList(challenges.Cast<IGeneratedHostInhabitant>().ToList()).Cast<IChallenge>().ToList();
            IChallenge challenge = challenges.Find(c => c.GetChallengeRating() <= totalChallengeRating);
            if (challenge == null)
                break;
            if (!GetSpawnDirectory(out Directory directory, challenge as IGeneratedHostInhabitant))
                continue;
            EntityHandler.I.CreateChallengeAt(directory, challenge, out IChallenge newChallenge);
            totalChallengeRating -= challenge.GetChallengeRating();
        }
    }
    private void AddPickups()
    {
        if (pickups.Count == 0)
            return;
        float totalLootValue = lootValue * levelIndexMultiplier;
        int iterations = 9999;
        while (totalLootValue > 0 && iterations > 0)
        {
            iterations--;
            pickups = RandomUtil.ShuffleHostList(pickups.Cast<IGeneratedHostInhabitant>().ToList()).Cast<IPickup>().ToList();
            IPickup pickup = pickups.Find(p => p.GetLootValue() <= totalLootValue);
            if (pickup == null)
                break;
            if (!GetSpawnDirectory(out Directory directory, pickup as IGeneratedHostInhabitant))
                continue;
            PickupHandler.I.CreatePickup(directory.transform, pickup);
            totalLootValue -= pickup.GetLootValue();
        }
    }
    private bool GetSpawnDirectory(out Directory directory, IGeneratedHostInhabitant generatedHostInhabitant) {
        List<Directory> result = new List<Directory>();
        if (generatedHostInhabitant.GeneratesInBranchDirectory())
            result.AddRange(branchDirectories);
        if (generatedHostInhabitant.GeneratesInLeafDirectory())
            result.AddRange(leafDirectories);
        if (generatedHostInhabitant.GeneratesInPriorityDirectory())
            result.AddRange(priorityDirectories);
        result = result.OrderBy(d => GetDirectoryWeight(d) * RandomUtil.random.Next()).ToList();
        directory = result[0];
        return directory != null;
    }
    private float GetDirectoryWeight(Directory directory) {
        if (priorityDirectories.Contains(directory))
            return 3;
        if (leafDirectories.Contains(directory))
            return 2;
        if (branchDirectories.Contains(directory))
            return 1;
        return 0;
    }
}
