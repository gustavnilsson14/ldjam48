using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyEntity : Entity
{
    

    public override string GetCatDescription()
    {
        List<string> result = new List<string> {
            GetBinaryStatic(),
            " added to lib! ",
            string.Format("You can now use the {0} command!", "asdasd")
        };
        onCat.Invoke();
        //command.isAvailable = true;
        Die();
        return string.Join("\n", result);
    }
}
