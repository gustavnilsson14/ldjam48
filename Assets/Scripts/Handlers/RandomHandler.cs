using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomHandler
{
    public static System.Random random = new System.Random();
    public static List<T> Shuffle<T>(List<T> list) {
        return list.OrderBy(x => RandomHandler.random.Next()).ToList();
    }
}
