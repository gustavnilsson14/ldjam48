using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Directory : MonoBehaviour
{
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
    public List<Directory> FindPath(Directory target)
    {
        List<Directory> result = new List<Directory>();
        return result;
    }

}
