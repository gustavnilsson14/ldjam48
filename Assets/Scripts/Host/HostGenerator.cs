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
    public List<IChallenge> challenges = new List<IChallenge>();
    public List<IPickup> pickups = new List<IPickup>();

    private Host host;
    private int levelIndex;

    private void Start()
    {
        Register();
    }

    private void Register()
    {
        host = GetComponent<Host>();
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
        children = children.OrderBy(x => RandomHandler.random.Next()).ToList();
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
        float totalChallengeRating = challengeRating * (float)levelIndex;
        while (totalChallengeRating > 0)
        {
            totalChallengeRating--;
        }
    }
    private void AddPickups()
    {
        float totalLootValue = lootValue * (float)levelIndex;
    }
    private void SelectPriorityDirectories() {
        priorityDirectories = host.GetLeafDirectories();
        priorityDirectories = RandomHandler.Shuffle<Directory>(priorityDirectories);
        Debug.Log($"priorityDirectories: {string.Join("-", priorityDirectories)}");
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
