using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Host))]
public class HostGenerator : MonoBehaviour
{
    public float challengeRating = 1;
    public float lootValue = 1;
    public float maxDirectoryDepth = 1;
    public float maxDirectoryChildren = 1;

    private List<Directory> priorityDirectories = new List<Directory>();

    [Header("Prefabs")]
    public List<GameObject> challengePrefabs = new List<GameObject>();
    public List<GameObject> pickupPrefabs = new List<GameObject>();
    private List<IChallenge> challenges = new List<IChallenge>();
    private List<IPickup> pickups = new List<IPickup>();



    private Host host;
    private int levelIndex;

    private void Start()
    {
        Register();
    }

    private void Register()
    {
        host = GetComponent<Host>();
        challenges.AddRange(challengePrefabs.SelectMany(c => c.GetComponents<IChallenge>()));
        Debug.Log(string.Join("--", challenges));
        pickups.AddRange(pickupPrefabs.SelectMany(p => p.GetComponents<IPickup>()));
    }

    public void Run(int levelIndex = 1)
    {
        this.levelIndex = levelIndex;
        StartCoroutine(Run());
    }
    public IEnumerator Run() {
        yield return 1;
        TrimFoldersRecursive(host.GetRootDirectory(), GetMaxDirectoryDepth());
        yield return 1;
        SelectPriorityDirectories();
        AddChallenges();
        AddPickups();
    }

    private void TrimFoldersRecursive(Directory current, int depth)
    {
        if (depth == 0)
        {
            Destroy(current.gameObject);
            return;
        }
        int currentMaxChildren = UnityEngine.Random.Range(0, GetMaxDirectoryChildren());
        if (current == host.GetRootDirectory())
            currentMaxChildren = GetMaxDirectoryChildren();
        List<Directory> children = current.GetChildren();
        children = RandomHandler.Shuffle<Directory>(children);
        int iterations = 99;
        while (children.Count > currentMaxChildren && iterations > 0 ) {
            iterations--;
            current.DestroyChild(children[children.Count - 1]);
            children.Remove(children[children.Count - 1]);
        }
        children.ForEach(next => TrimFoldersRecursive(next, depth - 1));
    }
    private void AddChallenges()
    {
        if (challenges.Count == 0)
            return;
        float totalChallengeRating = challengeRating * (float)levelIndex;
        int iterations = 9999;
        while (totalChallengeRating > 0 && iterations > 0)
        {
            iterations--;
            challenges = RandomHandler.Shuffle<IChallenge>(challenges);
            IChallenge challenge = challenges.Find(c => c.GetChallengeRating() <= totalChallengeRating);
            if (challenge == null)
                break;
            priorityDirectories = RandomHandler.Shuffle<Directory>(priorityDirectories);
            EntityHandler.I.CreateChallengeAt(priorityDirectories[0], challenge, out IChallenge newChallenge);
            totalChallengeRating -= challenge.GetChallengeRating();
        }
    }
    private void AddPickups()
    {
        if (pickups.Count == 0)
            return;
        float totalLootValue = lootValue * (float)levelIndex;
        int iterations = 9999;
        while (totalLootValue > 0 && iterations > 0)
        {
            iterations--;
            pickups = RandomHandler.Shuffle<IPickup>(pickups);
            IPickup pickup = pickups.Find(p => p.GetLootValue() <= totalLootValue);
            if (pickup == null)
                break;
            priorityDirectories = RandomHandler.Shuffle<Directory>(priorityDirectories);
            PickupHandler.I.CreatePickup(priorityDirectories[0].transform, pickup);
            totalLootValue -= pickup.GetLootValue();
        }
    }
    private void SelectPriorityDirectories() {
        priorityDirectories = host.GetLeafDirectories();
        priorityDirectories = RandomHandler.Shuffle<Directory>(priorityDirectories);
    }
    private int GetMaxDirectoryDepth()
    {
        return Mathf.FloorToInt(maxDirectoryDepth * this.levelIndex);
    }
    private int GetMaxDirectoryChildren()
    {
        return Mathf.FloorToInt(maxDirectoryDepth * this.levelIndex);
    }
}
