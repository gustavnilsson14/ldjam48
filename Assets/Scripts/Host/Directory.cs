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

    public virtual bool GetEntity(out Entity entity, string id)
    {
        entity = GetEntities().Find(e => e.uniqueId == id);
        return entity != null;
    }
    public virtual bool GetEntityByName(out Entity entity, string name)
    {
        entity = GetEntities().Find(e => e.name == name);
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
        return $"/{string.Join("/", GetAllParents().Select(directory => directory.GetName()).Where(name => name != string.Empty))}";
    }
    public string GetName()
    {
        if (this == HostHandler.I.currentHost.GetRootDirectory())
            return "";
        return name;
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
        List<DirectoryModifier> result = new List<DirectoryModifier>(GetComponents<DirectoryModifier>());
        foreach (Entity entity in GetEntities())
        {
            result.AddRange(entity.GetComponents<DirectoryModifier>());
        }
        return result.FindAll(mod => !mod.IsDisabled());
    }
    public bool GetDirectoriesFromPath(out List<Directory> directories, string fullPath) {
        directories = new List<Directory>();
        
        return true;
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
[System.Serializable]
public class ParsedPath
{
    private string input;
    public List<string> pathSegments;
    public List<Directory> directories;
    public Entity entity;
    public string error;

    public bool isAbsolute = false;
    public ParsedPath(Directory directory, string path) {
        input = path;
        ParsePath(path);
        GetDirectoryListRecursive(out directories, out error, isAbsolute ? HostHandler.I.currentHost.GetRootDirectory() : directory, pathSegments);
    }

    private void ParsePath(string path)
    {
        error = "";
        //Debug.Log($"path {path}");
        if (path.Substring(0,1) == "/")
        {
            isAbsolute = true;
            path = path.Substring(1);
        }
        pathSegments = new List<string>(path.Split(new string[] { "/" }, StringSplitOptions.None));
        pathSegments = pathSegments.FindAll(segment => segment != string.Empty && segment != ".");
    }

    private bool GetDirectoryListRecursive(out List<Directory> directories, out string error, Directory currentDirectory, List<string> pathSegments)
    {
        directories = new List<Directory>() { currentDirectory };
        error = "";
        if (pathSegments.Count == 0)
            return true;
        if (!GetNextDirectoryOrEntity(out Directory nextDirectory, out Entity entity, currentDirectory, pathSegments)) {
            error = $"{input} is not a valid path";
            return false;
        }
        if (entity != null)
        {
            this.entity = entity;
            return true;
        }
        pathSegments.RemoveAt(0);
        GetDirectoryListRecursive(out List<Directory> recursiveDirectories, out error, nextDirectory, pathSegments);
        directories.AddRange(recursiveDirectories);
        return true;
    }

    private bool GetNextDirectoryOrEntity(out Directory nextDirectory, out Entity entity, Directory currentDirectory, List<string> pathSegments)
    {
        string segment = pathSegments[0];
        nextDirectory = null;
        entity = null;
        switch (segment)
        {
            case "..":
                currentDirectory.GetClosestParent(out nextDirectory);
                break;
            default:
                GetDirectoryOrEntity(out nextDirectory, out entity, currentDirectory, segment);
                break;
        }
        if (nextDirectory == null && entity == null)
            return false;
        return true;
    }
    private bool GetDirectoryOrEntity(out Directory nextDirectory, out Entity entity, Directory currentDirectory, string segment) {
        currentDirectory.GetClosestParent(out Directory parent);
        nextDirectory = currentDirectory.GetAdjacentDirectories().Find(dir => dir.name == segment && dir != parent);
        if (!currentDirectory.GetEntity(out entity, segment))
            currentDirectory.GetEntityByName(out entity, segment);
        return nextDirectory != null;
    }

    public Directory GetLastDirectory()
    {
        if (directories.Count == 0)
            return null;
        return directories[directories.Count - 1];
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