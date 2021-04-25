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

    public enum HostNameType 
    {
        OS_USAGE_OWNER,
        OWNER_OS,
        OWNER_USAGE,
        USAGE_OS,
    }

    public void Update()
    {
        if (!test)
            return;
        test = false;
        Debug.Log(GetHostName());
    }

    public string GetHackerName()
    {
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
    public string GetHostName()
    {
        Array values = Enum.GetValues(typeof(HostNameType));
        System.Random random = new System.Random();
        HostNameType nameType = (HostNameType)values.GetValue(random.Next(values.Length));
        List<string> result = new List<string>();
        switch (nameType)
        {
            case HostNameType.OS_USAGE_OWNER:
                result.AddRange(new List<string>() { RandomFromList(hostOs), RandomFromList(hostUsage), RandomFromList(hostOwner) });
                break;
            case HostNameType.OWNER_OS:
                result.AddRange(new List<string>() { RandomFromList(hostOwner), RandomFromList(hostOs) });
                break;
            case HostNameType.OWNER_USAGE:
                result.AddRange(new List<string>() { RandomFromList(hostOwner), RandomFromList(hostUsage) });
                break;
            case HostNameType.USAGE_OS:
                result.AddRange(new List<string>() { RandomFromList(hostUsage), RandomFromList(hostOs) });
                break;
            default:
                break;
        }
        return string.Join("-", result);
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
    protected string[] hostOs = new string[] {
        "Win",
        "Ubuntu",
        "OSX",
        "Fedora",
        "Solaris",
        "BSD",
        "Chromium",
        "CentOS",
        "Debian",
        "Deepin"
    };
    protected string[] hostUsage = new string[] {
        "test",
        "proto",
        "prod",
        "proxy",
        "store",
        "bot",
        "dev",
        "deploy",
        "registry",
    };
    protected string[] hostOwner = new string[] {
        "DevSan",
        "M0ly",
        "New3ll",
        "Gatez",
        "AlAn",
        "Zucc3r",
        "@w00d",
        "BezoPezo",
        "CarM4cc",
        "Mei3rs",
        "R0m3r0",
        "GilB",
    };
}
