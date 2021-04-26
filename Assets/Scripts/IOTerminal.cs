using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class IOTerminal : MonoBehaviour
{
    public static IOTerminal I;
    public TextMeshProUGUI outputField;
    public TextMeshProUGUI userDirField;
    public TMP_InputField commandField;
    public Transform inputFieldsContainer;

    [Header("Events")]
    public CommandEvent onCommand = new CommandEvent();

    public TerminalTimeEvent onTerminalTimePast = new TerminalTimeEvent();
    public UnityEvent onEnter = new UnityEvent();

    private string baseUserDirString;
    private List<ParsedCommand> commandHistory = new List<ParsedCommand>();
    private int commandHistoryCurrentIndex;
    private int currentTerminalTime;
    private int totalTerminalTime;

    private void Awake()
    {
        IOTerminal.I = this;
        baseUserDirString = userDirField.text;
        commandField.onSubmit.AddListener(onCommandSubmit);
    }
    private void Start()
    {
        RenderUserAndDir();
        ResetCommandField();
        RegisterListeners();
    }

    private void RegisterListeners()
    {
        Player.I.onMove.AddListener(OnMove);
        Player.I.onDeath.AddListener(OnPlayerDeath);
    }
    private void OnPlayerDeath()
    {
        commandField.onSubmit.RemoveListener(onCommandSubmit);
        Destroy(inputFieldsContainer.gameObject);
        Destroy(userDirField.gameObject);
        List<Command> commands = Player.I.GetCommands().FindAll(command => command.isAvailable);
        List<string> commandStrings = new List<string>();
        commandStrings.AddRange(commands.Select(command => command.name));
        StartCoroutine(DisplayEpiloge(commandStrings));
    }
    private IEnumerator DisplayEpiloge(List<string> commandStrings)
    {
        yield return new WaitForSeconds(1f);
        ClearOutput();
        AppendTextLine("<color=red>TERMINAL ERROR:</color>");
        yield return new WaitForSeconds(2f);
        AppendTextLine("<color=red>Uplink terminated...</color>");
        yield return new WaitForSeconds(2f);
        AppendTextLine("Working.");
        yield return new WaitForSeconds(1f);
        for (int i = 0; i < 4; i++)
        {
            yield return new WaitForSeconds(0.2f);
            AppendTextLine(".");
        }
        AppendTextLine($".Total hosts visited: {HostHandler.I.exploredHosts.Count}");
        yield return new WaitForSeconds(0.5f);
        foreach (Host host in HostHandler.I.exploredHosts)
        {
            yield return new WaitForSeconds(0.1f);
            AppendTextLine($"{host.name} as {host.userName}");
        }
        AppendTextLine($".Total commands: {commandStrings.Count}");
        yield return new WaitForSeconds(0.5f);
        foreach (string command in commandStrings)
        {
            yield return new WaitForSeconds(0.1f);
            AppendTextLine($"{command}");
        }
        yield return new WaitForSeconds(0.5f);
        AppendTextLine("Press R to reboot");
        yield return new WaitForSeconds(0.5f);
        AppendTextLine("And thank you for playing!");
        yield return new WaitForSeconds(0.5f);
        AppendTextLine(NameUtil.RandomizeStringColors("By Red Pentagram Studios"));
        onEnter.AddListener(Restart);
    }
    public void Restart()
    {
        SceneManager.LoadScene("Main");
    }
    private void OnMove(Directory directory, Directory origin)
    {
        RenderUserAndDir();
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
        if (Input.GetKeyDown(KeyCode.R))
            onEnter.Invoke();
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            AutoCompleteCommand();
            AutoCompletePath();
            AutoCompleteEntityName();
        }
    }

    private void AutoCompleteCommand()
    {
        ParsedCommand parsedCommand = new ParsedCommand(commandField.text);
        if (parsedCommand.arguments.Count > 0)
            return;

        foreach (Command command in Player.GetCommands())
        {
            if (!command.name.StartsWith(parsedCommand.name))
                continue;

            commandField.text = $"{command.name}";
        }
        commandField.caretPosition = commandField.text.Length;
    }
    private void AutoCompleteEntityName()
    {
        List<Entity> entities = new List<Entity>();
        entities.AddRange(Player.I.currentDirectory.GetEntities());

        if (entities.Count == 0)
            return;

        ParsedCommand parsedCommand = new ParsedCommand(commandField.text);

        if (parsedCommand.arguments.Count != 1)
            return;

        if (parsedCommand.flags.Count > 0)
            return;

        foreach (Entity entity in entities)
        {
            if (!entity.name.StartsWith(parsedCommand.arguments[0]))
                continue;

            commandField.text = $"{parsedCommand.name} {entity.name}";
        }
        commandField.caretPosition = commandField.text.Length;
    }
    private void AutoCompletePath()
    {
        ParsedCommand parsedCommand = new ParsedCommand(commandField.text);

        if (parsedCommand.arguments.Count != 1)
            return;

        if (parsedCommand.flags.Count > 0)
            return;

        foreach (Directory directory in Player.I.currentDirectory.GetAdjacentDirectories())
        {
            if (!directory.name.StartsWith(parsedCommand.arguments[0]))
                continue;

            commandField.text = $"{parsedCommand.name} {directory.name}";

        }
        commandField.caretPosition = commandField.text.Length;
    }

    private void HistoryMove(int direction)
    {
        commandHistoryCurrentIndex += direction;
        if (commandHistoryCurrentIndex < 0)
            commandHistoryCurrentIndex = 0;
        if (commandHistoryCurrentIndex >= commandHistory.Count)
        {
            commandHistoryCurrentIndex = commandHistory.Count;
            commandField.text = "";
            return;
        }
        commandField.text = commandHistory[commandHistoryCurrentIndex].GetCommandString();
        commandField.caretPosition = commandField.text.Length;
    }
    public void RenderUserAndDir()
    {
        userDirField.text = String.Format(baseUserDirString, HostHandler.I.currentHost.userName, HostHandler.I.currentHost.name, "CSOS128", Player.I.currentDirectory.GetFullPath());
    }
    private void onCommandSubmit(string commandName)
    {
        if (commandName == "")
            return;
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
            TerminalTimePast(parsedCommand.GetCommandString().Length * 2);
            AppendTextLine("<color=red>ERROR:</color> " + result);
            return;
        }
        CommandPassed(command, parsedCommand);
        AppendTextLine(result);
    }
    private void CommandPassed(Command command, ParsedCommand parsedCommand)
    {
        TerminalTimePast(command.GetTerminalTimePast(parsedCommand));
        onCommand.Invoke(command, parsedCommand);
    }
    private void TerminalTimePast(int terminalTimePassed)
    {
        currentTerminalTime += terminalTimePassed;
        totalTerminalTime += terminalTimePassed;
        onTerminalTimePast.Invoke(terminalTimePassed);
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

    public void ClearOutput()
    {
        outputField.text = "";
    }
}
public class CommandEvent : UnityEvent<Command, ParsedCommand> { }
public class TerminalTimeEvent : UnityEvent<int>
{
    internal void AddListener(object onTerminalTimePast)
    {
        throw new NotImplementedException();
    }
}