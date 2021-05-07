using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class SqlComponent : EntityComponent
{
    public List<StoredObject> storedComponents = new List<StoredObject>();
    protected override void Run()
    {
        base.Run();
        if (!GetSensorComponent(out SensorComponent sensorComponent))
        {
            HandleNoSensor();
            return;
        }
        /*
        if (!sensorComponent.GetCurrentTarget(out ComponentWithIP target))
            return;
        PickupEntity pickupEntity = target as PickupEntity;
        if (pickupEntity == null)
            return;
        AddItem(pickupEntity.pickup);
        pickupEntity.DestroyMe();*/
    }

    private void HandleNoSensor()
    {
        
    }

    public void AddItem(StoredObject item)
    {
        storedComponents.Add(item);
        SortComponents();
    }
    public bool FetchItem(out StoredObject storedObject, string id) {
        storedObject = storedComponents.Find(s => s.id["name"] == id);
        if (storedObject == null)
            return false;
        storedComponents.Remove(storedObject);
        SortComponents();
        return true;
    }
    public void SortComponents()
    {
        storedComponents.Sort((x, y) => x.id["pickupType"].CompareTo(y.id["pickupType"]));
    }
    public string GetStackedListView() { 
        string result = "";
        Dictionary<string, Dictionary<string,int>> keyValuePairs = new Dictionary<string, Dictionary<string, int>>();
        foreach (StoredObject storedObject in storedComponents)
        {
            string pickupType = storedObject.id["pickupType"];
            if (!keyValuePairs.ContainsKey(pickupType))
                keyValuePairs.Add(pickupType, new Dictionary<string, int>());
            string key = storedObject.id["name"];
            if (!keyValuePairs[pickupType].ContainsKey(key))
            {
                keyValuePairs[pickupType].Add(key,1);
                continue;
            }
            keyValuePairs[pickupType][key]++;
        }
        foreach (var item in keyValuePairs)
        {
            result += $"{item.Key}s\n";
            result += $"{string.Join("\n", item.Value.Select(x => $"    {x.Value}x {x.Key}"))}\n";
        }
        return result;
    }
}