using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NameUtil : MonoBehaviour
{

    public enum HackerNameType
    {
        SUBJECT_VERB,
        SUBJECT_TITLE,
        TITLE_ADJECTIVE
    }
    public static string GetHackerName() {
        return null;
    }

    protected string[] hackerSubstance = new string[] {
        "Acid",
        "Toxic",
        "Sludge",
        "Feather",
        "Booze",
        "Razor",
        "Cpu",
        "Gpu",
        "Fan",
    };
    protected string[] hackerTitle = new string[] {
        "Doctor",
        "Mage",
        ""
    };
    protected string[] hackerVerb = new string[] {

    };
    protected string[] hackerAdjective = new string[] {

    };
}
