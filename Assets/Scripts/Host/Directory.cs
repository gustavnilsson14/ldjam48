using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Directory : MonoBehaviour
{
    public enum DirectoryPrivilege
    {
        DANGER,
        FRIENDLY
    }

    public DirectoryPrivilege privilege = DirectoryPrivilege.DANGER;

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
    public List<Directory> GetDirectoriesByDepth(int scanDepth = 0) {
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
    public string GetFullPath()
    {
        return "/" + string.Join("/", GetAllParents().Select(directory => directory.name));
    }
    public List<PathNode> FindPath(Directory target) {

        int iterations = 99;
        List<PathNode> result = new List<PathNode>();
        List<PathNode> openNodes = new List<PathNode>();
        List<PathNode> closedNodes = new List<PathNode>();
        PathNode currentNode = new PathNode(0, this, null);
        while (currentNode.directory != target && iterations > 0) {
            iterations--;
            Debug.Log(" iterations1 " + iterations);
            foreach (Directory neighbor in currentNode.directory.GetAdjacentDirectories())
            {
                if (openNodes.Find(node => node.directory == neighbor) != null)
                    continue;
                if (closedNodes.Find(node => node.directory == neighbor) != null)
                    continue;
                openNodes.Add(new PathNode(currentNode.distance + 1, neighbor, currentNode));
            }
            openNodes.Sort((a, b) => a.distance.CompareTo(b.distance));
            currentNode = openNodes[0];
            openNodes.Remove(currentNode);
            closedNodes.Add(currentNode);
        }

        while (currentNode.directory != this && iterations > 0)
        {
            iterations--;
            Debug.Log(" iterations2 " + iterations);
            result.Add(currentNode);
            currentNode = currentNode.parent;
        }
        result.Reverse();

        return result;
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
