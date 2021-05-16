using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CommandEntity : PickupEntity
{
    public Command command;
    public override List<string> FormatCatDescription(List<string> catDescription)
    {
        catDescription = base.FormatCatDescription(catDescription);
        catDescription.AddRange(
            new List<string> {
                command.name + " added to lib! " + command.helpText,
                string.Format("You can now use the {0} command!", command.name)
            }
        );
        command.isAvailable = true;
        return catDescription;
    }
}