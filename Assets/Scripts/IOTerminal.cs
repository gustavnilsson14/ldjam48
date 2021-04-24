using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using UnityEngine.EventSystems;

public class IOTerminal : MonoBehaviour
{
    public static IOTerminal I;
    public TextMeshProUGUI outputField;
    public TextMeshProUGUI userDirField;
    public TMP_InputField commandField;

    private string baseUserDirString;

    private void Awake()
    {
        IOTerminal.I = this;
        commandField.onSubmit.AddListener(onCommandSubmit);
    }

    private void Start()
    {
        RenderUserAndDir();
        ResetCommandField();
    }

    private void RenderUserAndDir()
    {
        baseUserDirString = userDirField.text;
        userDirField.text = String.Format(baseUserDirString, "haxxor", "LDTerminal", "CSOS128", "");
    }

    private void onCommandSubmit(string commandName)
    {
        HandleStringInput(commandName);
        ResetCommandField();
    }

    private void ResetCommandField()
    {
        commandField.text = "";
        commandField.ActivateInputField();
    }

    private void HandleStringInput(string commandName)
    {
        if (!Player.GetCommand(out Command command, commandName))
        {
            HandleNoSuchCommand(commandName);
            return;
        }
        HandleCommand(command);
    }

    private void HandleCommand(Command command)
    {
        AppendDefaultCommandText();
        command.Run();
        //AppendTextLine("HELLZ YAH " + command.name);
    }

    private void HandleNoSuchCommand(string commandName)
    {
        AppendDefaultCommandText();
        AppendTextLine("No such command");
    }

    private void AppendDefaultCommandText()
    {
        AppendTextLine(userDirField.text);
        AppendTextLine("$ " + commandField.text);
    }

    public void AppendTextLine(string newText)
    {
        outputField.text += "\n" + newText;
    }
}
