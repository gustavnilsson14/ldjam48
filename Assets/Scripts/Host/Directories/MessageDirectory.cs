using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class MessageDirectory : Directory
{
    [TextArea(3,99)]
    public string entryMessage;

    public override void EntityEnter(Directory from, Entity entity)
    {
        base.EntityEnter(from, entity);
        if (entity != Player.I)
            return;
        HandlePlayerEntry();
    }

    private void HandlePlayerEntry()
    {
        IOTerminal.I.ClearOutput();
        IOTerminal.I.AppendTextLine(entryMessage);
    }
}