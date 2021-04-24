using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CommandEntity : Entity
{
    public Command command;

    public override string GetCatDescription()
    {
        List<string> result = new List<string> {
            GetBinaryStatic(),
            command.helpText,
            string.Format("You can now use the {0} command!", command.name)
        };
        onCat.Invoke();
        command.isAvailable = true;
        Die();
        return string.Join("\n", result);
    }
}
