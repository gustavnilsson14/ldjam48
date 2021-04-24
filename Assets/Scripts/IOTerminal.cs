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
    private List<ParsedCommand> commandHistory = new List<ParsedCommand>();
    private int commandHistoryCurrentIndex;

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

    private void Update()
    {
        HandleInput();
    }

    private void HandleInput()
    {
        if (Input.GetKeyDown(KeyCode.UpArrow))
            HistoryMove(-1);
        if (Input.GetKeyDown(KeyCode.DownArrow))
            HistoryMove(1);
    }

    private void HistoryMove(int direction)
    {
        commandHistoryCurrentIndex += direction;
        if (commandHistoryCurrentIndex < 0)
            commandHistoryCurrentIndex = 0;
        if (commandHistoryCurrentIndex >= commandHistory.Count) {
            commandHistoryCurrentIndex = commandHistory.Count;
            commandField.text = "";
            return;
        }
        commandField.text = commandHistory[commandHistoryCurrentIndex].GetCommandString();
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
        commandHistoryCurrentIndex = commandHistory.Count;
        commandField.ActivateInputField();
    }

    private void HandleStringInput(string commandName)
    {
        ParsedCommand parsedCommand = new ParsedCommand(commandName);
        commandHistory.Add(parsedCommand);
        if (!Player.GetCommand(out Command command, parsedCommand.name))
        {
            HandleNoSuchCommand(parsedCommand.name);
            return;
        }
        HandleCommand(command, parsedCommand);
    }

    private void HandleCommand(Command command, ParsedCommand parsedCommand)
    {
        AppendDefaultCommandText();
        command.Run(parsedCommand);
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
