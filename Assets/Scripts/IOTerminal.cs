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
    public bool levelUpCommand = false;

    [Header("Events")]
    public CommandEvent onCommand = new CommandEvent();

    public TerminalTimeEvent onTerminalTimePast = new TerminalTimeEvent();
    public UnityEvent onEnter = new UnityEvent();

    private string baseUserDirString;
    private List<ParsedCommand> commandHistory = new List<ParsedCommand>();
    private int commandHistoryCurrentIndex;
    private int currentTerminalTime;
    private int totalTerminalTime;

    public List<string> destroyedEntities = new List<string>();

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
            AutoCompleteSsh();
            AutoCompletePath();
            AutoCompleteEntityName();
        }
    }

    private void AutoCompleteSsh()
    {
        ParsedCommand parsedCommand = new ParsedCommand(commandField.text);
        if (parsedCommand.name != "ssh")
            return;

        if (parsedCommand.arguments.Count != 1)
            return;

        if (parsedCommand.arguments[0].Length < 1)
            return;

        if (parsedCommand.flags.Count > 0)
            return;

        foreach (PublicKey key in HostHandler.I.currentHost.keys)
        {
            if (!(key is SshKey))
                continue;

            if (!key.isAvailable)
                return;

            if (!(key as SshKey).GetName().StartsWith(parsedCommand.arguments[0]))
                continue;

            commandField.text = $"{parsedCommand.name} {(key as SshKey).GetName()}";
        }
        commandField.caretPosition = commandField.text.Length;

    }

    private void AutoCompleteCommand()
    {
        ParsedCommand parsedCommand = new ParsedCommand(commandField.text);
        if (parsedCommand.arguments.Count > 0)
            return;

        if (parsedCommand.name.Length < 1)
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

        if (parsedCommand.arguments[0].Length < 1)
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

        if (parsedCommand.arguments[0].Length < 1)
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

        if (levelUpCommand)
        {
            Debug.Log("Chose command");
            LevelUpCommand(commandName);
            ResetCommandField();
            return;
        }
        HandleStringInput(commandName);
        ResetCommandField();
    }

    public void DisplayLevelUp()
    {
        levelUpCommand = true;
        List<string> resultStrings = new List<string>();
        foreach (Command command in Player.GetCommands())
        {
            if (!command.isAvailable)
                continue;
            resultStrings.Add(command.name);
        }
        string result = string.Join(" ", resultStrings);

        AppendTextLine(StringUtil.ColorWrap($"LEVEL UP! Type a command to level it up", Palette.YELLOW));
        AppendTextLine($"Your commands: {result}");


    }
    private void LevelUpCommand(string commandName)
    {
        ParsedCommand parsedCommand = new ParsedCommand(commandName);
        if (!Player.GetCommand(out Command command, parsedCommand.name))
        {
            AppendTextLine(StringUtil.ColorWrap("Command not found, try another one to level up!", Palette.MAGENTA));
            return;
        }

        levelUpCommand = false;
        command.LevelUp();
        AppendTextLine(StringUtil.ColorWrap($"{command.name} is now level {command.level}, speed increased and can use {command.maxFlags} flags", Palette.YELLOW));
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
            AppendTextLine($"{StringUtil.ColorWrap("ERROR:", Palette.RED)} {result}");
            return;
        }
        CommandPassed(command, parsedCommand);
        if (result == "")
            return;
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

    public void AppendTextLine(string newText, bool clear = false)
    {
        if (clear)
            ClearOutput();
        outputField.text += "\n" + newText;
    }

    public void ClearOutput()
    {
        outputField.text = "";
    }
    private IEnumerator DisplayEpiloge(List<string> commandStrings)
    {
        yield return new WaitForSeconds(1f);
        string epilog = "<color=red>TERMINAL ERROR:</color>";
        AppendTextLine(epilog, true);
        yield return new WaitForSeconds(2f);
        epilog += "\n<color=red>Uplink terminated...</color>";
        AppendTextLine(epilog, true);
        yield return new WaitForSeconds(2f);
        epilog += "\nWorking";
        AppendTextLine(epilog, true);
        yield return new WaitForSeconds(1f);
        for (int i = 0; i < 10; i++)
        {
            yield return new WaitForSeconds(0.05f);
            epilog += ".";
            AppendTextLine(epilog, true);
        }
        yield return new WaitForSeconds(0.05f);
        epilog += $"\nTotal hosts visited: {HostHandler.I.exploredHosts.Count}";
        AppendTextLine(epilog, true);
        yield return new WaitForSeconds(1f);
        foreach (Host host in HostHandler.I.exploredHosts)
        {
            yield return new WaitForSeconds(0.1f);
            epilog += $"\n{host.name} as {host.userName}";
            AppendTextLine(epilog, true);
        }
        yield return new WaitForSeconds(1f);
        epilog += $"\nTotal commands: {commandStrings.Count}\n";
        AppendTextLine(epilog, true);
        yield return new WaitForSeconds(0.5f);
        foreach (string command in commandStrings)
        {
            yield return new WaitForSeconds(0.1f);
            epilog += $"{command}, ";
            AppendTextLine(epilog, true);
        }
        yield return new WaitForSeconds(1f);
        epilog += $"\nDestroyed files: {destroyedEntities.Count}\n";
        AppendTextLine(epilog, true);
        yield return new WaitForSeconds(0.5f);
        foreach (string entity in destroyedEntities)
        {
            yield return new WaitForSeconds(0.05f);
            epilog += $"{entity}, ";
            AppendTextLine(epilog, true);
        }
        yield return new WaitForSeconds(1f);
        epilog += "\nPress R to reboot";
        AppendTextLine(epilog, true);
        yield return new WaitForSeconds(1f);
        epilog += "\nAnd thank you for playing!";
        AppendTextLine(epilog, true);
        yield return new WaitForSeconds(1f);
        epilog += "\n" + NameUtil.RandomizeStringColors("By");
        AppendTextLine(epilog, true);
        yield return new WaitForSeconds(0.3f);
        epilog += NameUtil.RandomizeStringColors(" Red");
        AppendTextLine(epilog, true);
        yield return new WaitForSeconds(0.3f);
        epilog += NameUtil.RandomizeStringColors(" Pentagram");
        AppendTextLine(epilog, true);
        yield return new WaitForSeconds(0.3f);
        epilog += NameUtil.RandomizeStringColors(" Studios");
        AppendTextLine(epilog, true);
        onEnter.AddListener(Restart);
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