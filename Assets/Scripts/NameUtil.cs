using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NameUtil : MonoBehaviour
{
    public static NameUtil I;
    public bool test = false;
    public void Awake()
    {
        NameUtil.I = this;
    }

    public enum HackerNameType
    {
        SUBJECT_VERB,
        SUBJECT_TITLE,
        TITLE_ADJECTIVE
    }

    public void Update()
    {
        if (!test)
            return;
        test = false;
        Debug.Log(GetHackerName());
    }

    public string GetHackerName() {
        Array values = Enum.GetValues(typeof(HackerNameType));
        System.Random random = new System.Random();
        HackerNameType nameType = (HackerNameType)values.GetValue(random.Next(values.Length));
        List<string> result = new List<string>();
        switch (nameType)
        {
            case HackerNameType.SUBJECT_VERB:
                result.AddRange(new List<string>() { RandomFromList(hackerSubject), RandomFromList(hackerVerb) });
                break;
            case HackerNameType.SUBJECT_TITLE:
                result.AddRange(new List<string>() { RandomFromList(hackerSubject), RandomFromList(hackerTitle) });
                break;
            case HackerNameType.TITLE_ADJECTIVE:
                result.AddRange(new List<string>() { RandomFromList(hackerTitle), RandomFromList(hackerAdjective) });
                break;
        }
        return string.Join("", result);
    }

    public string RandomFromList(string[] list)
    {
        return list[UnityEngine.Random.Range(0, list.Length)];
    }

    protected string[] hackerSubject = new string[] {
        "Acid",
        "Toxic",
        "Sludge",
        "Feather",
        "Booze",
        "Razor",
        "Cpu",
        "Gpu",
        "Fan",
        "Deck",
        "Data",
        "Deita",
        "Link",
        "Scrip",
        "Face",
    };
    protected string[] hackerTitle = new string[] {
        "Doctor",
        "Mr",
        "Ms",
        "Boy",
        "Girl",
        "Grrl",
        "Chiphead",
        "Captain",
        "Professor",
        "Master",
        "Drek",
        "Chummer",
        "Runner",
        "Go-go-go",
        "Day",
        "MrJohnsson",
    };
    protected string[] hackerVerb = new string[] {
        "Burn",
        "Bloat",
        "Ring",
        "Geek",
        "Static",
        "Pay",
        "Jack",
        "Dumpshock",
        "Breeder",
        "Frag",
        "Taser",
    };
    protected string[] hackerAdjective = new string[] {
        "Big",
        "Small",
        "Sad",
        "Happy",
        "Black",
        "White",
    };
}
