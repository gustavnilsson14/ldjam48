using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Host : MonoBehaviour
{
    public bool isAvailable = false;
    public List<string> users = new List<string>();

    public Directory GetRootDirectory() {
        return GetComponentInChildren<Directory>();
    }
}
