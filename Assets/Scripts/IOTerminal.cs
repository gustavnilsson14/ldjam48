using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using UnityEngine.EventSystems;
using UnityEngine.Events;

public class IOTerminal : MonoBehaviour
{
    public static IOTerminal I;
    public TextMeshProUGUI outputField;
    public TextMeshProUGUI userDirField;
    public TMP_InputField commandField;

    [Header("Events")]
    public TimeEvent onTerminalTimePast = new TimeEvent();
    public TimeEvent onTimePast = new TimeEvent();

    private string baseUserDirString;
    private List<ParsedCommand> commandHistory = new List<ParsedCommand>();
    private int commandHistoryCurrentIndex;
    private int currentTerminalTime;
    private int totalTerminalTime;

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
        HandleTimePast();
    }

    private void HandleTimePast()
    {
        
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
    private void TerminalTimePast(int time)
    {
        currentTerminalTime += time;
        totalTerminalTime += time;
        onTerminalTimePast.Invoke(time);
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
        if (!command.Run(out string result, parsedCommand))
        {
            TerminalTimePast(parsedCommand.GetCommandString().Length*2);
            AppendTextLine("<color=red>ERROR:</color> " + result);
            return;
        }
        TerminalTimePast(command.GetTerminalTimePast(parsedCommand));
        AppendTextLine(result);
    }
    private void HandleNoSuchCommand(string commandName)
    {
        AppendDefaultCommandText();
        TerminalTimePast(commandName.Length);
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

    public void ClearOutput() {
        outputField.text = "";
    }
}
public class TimeEvent : UnityEvent<int> { }