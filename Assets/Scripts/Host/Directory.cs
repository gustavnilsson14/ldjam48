﻿using System.Collections;
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
}
