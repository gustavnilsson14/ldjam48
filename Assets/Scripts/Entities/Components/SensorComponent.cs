using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class SensorComponent : EntityComponent
{
    public int scanDepth = 0;
    public int targetsMax = -1;
    public List<TargetData> targets = new List<TargetData>();

    protected override void Run() {
        targets.Clear();
        FindTargets();
        SortTargets();
        TriggerOutput();
    }
    public virtual bool GetCurrentTarget(out TargetData target)
    {
        target = null;
        if (targets.Count == 0)
            return false;
        target = targets[0];
        return true;
    }
    protected virtual void FindTargets()
    {
        EntityFaction myFaction = entityBody.faction;
        if (!GetEntitiesInScanRange(out List<Entity> entities))
            return;
        entities = entities.OrderBy(x => RandomUtil.random.Next()).ToList();
        foreach (Entity entity in entities)
        {
            ProcessNewTargetData(entity);
        }
    }
    protected virtual void TriggerOutput()
    {
        if (!GetCurrentTarget(out TargetData target))
            return;
        onOutput.Invoke(this, GetFormattedOutput(target));
    }
    private string GetFormattedOutput(TargetData target)
    {
        return $"{target.lastPosition.GetFullPath()}/{target.targetId}";
    }
    protected virtual void SortTargets()
    {
        targets.Sort((x,y) => y.weight.CompareTo(x.weight));
    }
    protected virtual bool GetEntitiesInScanRange(out List<Entity> targets)
    {
        List<Directory> directories = entityBody.currentDirectory.GetDirectoriesByDepth(scanDepth);
        return Directory.GetAllEntitiesInDirectories(out targets, directories);
    }
    protected virtual bool ProcessNewTargetData(Entity entity) {
        if (entity == entityBody)
            return false;
        if (!HasMaxTargets(out int max))
            return AddNewTargetData(entity);
        if (targets.Count >= max)
            return false;
        return AddNewTargetData(entity);
    }
    
    protected virtual bool AddNewTargetData(Entity entity) {
        if (!CreateNewTargetData(out TargetData newTargetData, entity))
            return false;
        targets.Add(newTargetData);
        return true;
    }
    protected virtual bool CreateNewTargetData(out TargetData newTargetData, Entity entity, float weight = 0, int ttl = 30) {
        newTargetData = null;
        if (targets.Find(target => target.targetId == entity.uniqueId) != null)
            return false;
        newTargetData = new TargetData(entity, entityBody.GetNewTargetWeight(entity), ttl);
        return true;
    }
    protected virtual bool HasMaxTargets(out int max) {
        max = targetsMax;
        if (targetsMax == -1)
            return false;
        return true;
    }

    [System.Serializable]
    /// <summary>
    /// A target discovered by a sensor
    /// </summary>
    public class TargetData {
        /// <summary>
        /// The uniqueId of the target componentwithip
        /// </summary>
        public string targetId;
        public EntityFaction targetFaction;
        /// <summary>
        /// The last known position of the target, at the time of the scan
        /// </summary>
        public Directory lastPosition;
        public float weight;
        /// <summary>
        /// Time to live, in terminal time
        /// </summary>
        private int ttl;
        private TargetDataExpireEvent onTargetDataExpire = new TargetDataExpireEvent();
        public TargetData(Entity target, float weight, int ttl)
        {
            Init(target.uniqueId, target.faction, target.currentDirectory, weight, ttl);
        }
        public TargetData(string targetId, EntityFaction targetFaction, Directory lastPosition, float weight, int ttl)
        {
            Init(targetId, targetFaction, lastPosition, weight, ttl);
        }
        private void Init(string targetId, EntityFaction targetFaction, Directory lastPosition, float weight, int ttl) {
            this.targetId = targetId;
            this.targetFaction = targetFaction;
            this.lastPosition = lastPosition;
            this.weight = weight;
            this.ttl = ttl;
            IOTerminal.I.onTerminalTimePast.AddListener(OnTerminalTimePast);
        }
        public void OnTerminalTimePast(int time) {
            ttl -= time;
            if (ttl > 0)
                return;
            onTargetDataExpire.Invoke(this);
        }
    }
    public class TargetDataExpireEvent : UnityEvent<TargetData> { }
}