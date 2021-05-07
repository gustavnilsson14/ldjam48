using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Directory : MonoBehaviour
{
    public List<EntityFaction> bannedFactions = new List<EntityFaction>();
    public DirectoryMoveEvent onEntityEnter = new DirectoryMoveEvent();
    public DirectoryMoveEvent onEntityExit = new DirectoryMoveEvent();
    public List<Directory> GetAdjacentDirectories() {
        List<Directory> result = new List<Directory>();
        foreach (Transform child in transform)
        {
            Directory neighbor = child.GetComponent<Directory>();
            if (neighbor == null)
                continue;
            result.Add(neighbor);
        }
        Directory parent = transform.parent.GetComponent<Directory>();
        if (parent == null)
            return result;
        result.Add(parent);
        return result;
    }

    public virtual bool GetEntity(out Entity entity, int id)
    {
        entity = GetEntities().Find(e => e.uniqueId == id);
        return entity != null;
    }

    public void EntityEnter(Directory from, Entity entity) {
        onEntityEnter.Invoke(from, this, entity);
    }
    public void EntityExit(Directory to, Entity entity) {
        onEntityExit.Invoke(this, to, entity);
    }

    public bool GetClosestParent(out Directory parentDirectory)
    {
        parentDirectory = transform.parent.GetComponent<Directory>();
        if (parentDirectory == null)
            return false;
        return true;
    }

    public List<Entity> GetEntities() {
        List<Entity> result = new List<Entity>();
        foreach (Transform child in transform)
        {
            Entity childEntity = child.GetComponent<Entity>();
            if (childEntity == null)
                continue;
            result.Add(childEntity);
        }
        return result;
    }
    public List<Directory> GetAllParents()
    {
        List<Directory> directories = new List<Directory>();
        Directory parentDirectory = transform.parent.GetComponent<Directory>();
        if (parentDirectory != null)
            directories.AddRange(parentDirectory.GetAllParents());
        directories.Add(this);
        return directories;
    }
    public List<Directory> GetDirectoriesByDepth(int scanDepth = 0)
    {
        List<Directory> result = new List<Directory>();
        if (scanDepth != 0)
        {
            foreach (Directory directory in GetAdjacentDirectories())
            {
                result.AddRange(directory.GetDirectoriesByDepth(scanDepth - 1));
            }
        }
        if (!result.Contains(this))
            result.Add(this);
        return result;
    }
    public List<Directory> GetDirectoriesAtDepth(int scanDepth = 0)
    {
        int iterations = 99;
        if (scanDepth == 0)
            return new List<Directory>() { this };
        List<Directory> result = new List<Directory>();
        List<PathNode> openNodes = new List<PathNode>() { new PathNode(0, this, null) };
        List<PathNode> closedNodes = new List<PathNode>();
        PathNode currentNode = openNodes[0];
        while (openNodes.Count > 0 && iterations > 0)
        {
            iterations--;
            foreach (Directory neighbor in currentNode.directory.GetAdjacentDirectories())
            {
                if (openNodes.Find(node => node.directory == neighbor) != null)
                    continue;
                if (closedNodes.Find(node => node.directory == neighbor) != null)
                    continue;
                PathNode newNode = new PathNode(currentNode.distance + 1, neighbor, currentNode);
                openNodes.Add(newNode);
                if (newNode.distance != scanDepth)
                    continue;
                closedNodes.Add(newNode);
                openNodes.Remove(newNode);
                result.Add(newNode.directory);
            }
            openNodes.Remove(currentNode);
            closedNodes.Add(currentNode);
            if (openNodes.Count == 0)
                continue;
            openNodes.Sort((a, b) => a.distance.CompareTo(b.distance));
            currentNode = openNodes[0];
        }
        return result;
    }
    public string GetFullPath()
    {
        return "/" + string.Join("/", GetAllParents().Select(directory => directory.name));
    }

    public bool GetFullPathWithoutRoot(out string path)
    {
        List<Directory> paths = GetAllParents();
        path = "";
        if (paths.Count < 2)
            return false;
        paths.RemoveAt(0);
            
        path = string.Join("/", paths.Select(directory => directory.name));
        return true;
    }
    public List<DirectoryModifier> GetModifiers() {
        List<DirectoryModifier> result = new List<DirectoryModifier>();
        if (GetClosestParent(out Directory parent))
            result.AddRange(parent.GetModifiers());
        result.AddRange(GetLocalModifiers());
        return result;
    }

    private List<DirectoryModifier> GetLocalModifiers()
    {
        List<DirectoryModifier> result = new List<DirectoryModifier>();
        result.AddRange(GetComponents<DirectoryModifier>());
        foreach (Entity entity in GetEntities())
        {
            result.AddRange(entity.GetComponents<DirectoryModifier>());
        }
        return result.FindAll(mod => !mod.IsDisabled());
    }

    public int GetDepth()
    {
        return GetAllParents().Count;
    }
    public List<PathNode> FindPath(Directory target) {
        //Debug.Log("public List<PathNode> FindPath(Directory " + target.name + ") {" + this.name);
        int iterations = 99;
        List<PathNode> result = new List<PathNode>();
        List<PathNode> openNodes = new List<PathNode>();
        List<PathNode> closedNodes = new List<PathNode>();
        PathNode currentNode = new PathNode(0, this, null);
        while (currentNode.directory != target && iterations > 0) {
            iterations--;
            //Debug.Log(" iterations1 " + iterations);
            foreach (Directory neighbor in currentNode.directory.GetAdjacentDirectories())
            {
                if (openNodes.Find(node => node.directory == neighbor) != null)
                    continue;
                if (closedNodes.Find(node => node.directory == neighbor) != null)
                    continue;
                openNodes.Add(new PathNode(currentNode.distance + 1, neighbor, currentNode));
            }
            openNodes.Remove(currentNode);
            closedNodes.Add(currentNode);
            if (openNodes.Count == 0)
                continue;
            openNodes.Sort((a, b) => a.distance.CompareTo(b.distance));
            currentNode = openNodes[0];
        }

        while (currentNode.directory != this && iterations > 0)
        {
            iterations--;
            //Debug.Log(" iterations2 " + iterations);
            result.Add(currentNode);
            currentNode = currentNode.parent;
        }
        result.Reverse();
        //Debug.Log("FindPath " + string.Join("-", result.Select(s => s.directory.name)) + " iterations " + iterations);
        return result;
    }

    public bool test = false;
    private void Update()
    {
        if (!test)
            return;
        test = false;
        Debug.Log($" --------- TESTING DEPTH 1 {string.Join(", ", GetDirectoriesAtDepth(1).Select(dir => dir.GetFullPath()))}---------");
        Debug.Log($" --------- TESTING DEPTH 2 {string.Join(", ", GetDirectoriesAtDepth(2).Select(dir => dir.GetFullPath()))}---------");
        Debug.Log($" --------- TESTING DEPTH 3 {string.Join(", ", GetDirectoriesAtDepth(3).Select(dir => dir.GetFullPath()))}---------");
    }

    public static bool GetAllEntitiesInDirectories(out List<Entity> targets, List<Directory> directories)
    {
        targets = new List<Entity>();
        List<Entity> result = new List<Entity>();
        if (directories.Count == 0)
            return false;
        foreach (Directory directory in directories)
        {
            result.AddRange(directory.GetEntities().FindAll(e => !result.Contains(e)));
        }
        targets.AddRange(result);
        if (targets.Count == 0)
            return false;
        return true;
    }
}
public class PathNode {
    public int distance;
    public PathNode parent;
    public Directory directory;
    public PathNode(int distance, Directory directory, PathNode parent) {
        this.distance = distance;
        this.directory = directory;
        this.parent = parent;
    }
}
public class DirectoryMoveEvent : UnityEvent<Directory, Directory, Entity> { }