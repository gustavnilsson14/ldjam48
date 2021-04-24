using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Host : MonoBehaviour
{
    public Directory GetRootDirectory() {
        return GetComponentInChildren<Directory>();
    }
}
