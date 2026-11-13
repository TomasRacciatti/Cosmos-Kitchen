using System;
using System.Collections.Generic;
using Managers;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Cheats
{
    public class CheatConsole : MonoBehaviour
    {
        private delegate string CommandHandler(string[] args);

        private readonly struct Command
        {
            public readonly string Usage;
            public readonly string Description;
            public readonly CommandHandler Run;

            public Command(string usage, string description, CommandHandler run)
            {
                Usage = usage;
                Description = description;
                Run = run;
            }
        }

        private static readonly char[] Separators = { ' ', '\t' };
        private const string InputControlName = "CheatConsoleInput";
        private const int MaxLogLines = 10;

        [SerializeField] private bool cheatsEnabled = true;
        [SerializeField] private Key toggleKey = Key.Backquote;
        [SerializeField, Min(10)] private int fontSize = 18;

        private Dictionary<string, Command> _commands;
        private readonly List<string> _log = new();
        private string _logText = string.Empty;
        private string _input = string.Empty;
        private bool _open;
        private int _openedFrame;
        private GUIStyle _logStyle;
        private GUIStyle _inputStyle;

        private bool _savedInputActive;
        private bool _savedCanMove;
        private CursorLockMode _savedLockState;
        private bool _savedCursorVisible;

        public bool CheatsEnabled => cheatsEnabled;

        private void Awake()
        {
            _commands = new Dictionary<string, Command>(StringComparer.OrdinalIgnoreCase)
            {
                ["help"] = new Command("help", "Lists all commands.", Help),
                ["set_novas"] = new Command("set_novas <amount>", "Sets how many Golden Novas you have.", SetNovas),
            };
        }

        private void OnDisable()
        {
            if (_open) Close();
        }

        private void Update()
        {
            var keyboard = Keyboard.current;
            if (keyboard == null || toggleKey == Key.None || !keyboard[toggleKey].wasPressedThisFrame) return;

            if (_open) Close();
            else if (cheatsEnabled) Open();
        }

        public bool TryExecute(string commandLine, out string result)
        {
            if (!cheatsEnabled)
            {
                result = "Cheats are disabled.";
                return false;
            }

            var parts = commandLine.Split(Separators, StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length == 0)
            {
                result = string.Empty;
                return false;
            }

            if (!_commands.TryGetValue(parts[0], out var command))
            {
                result = $"Unknown command '{parts[0]}'. Type help.";
                return false;
            }

            var args = new string[parts.Length - 1];
            Array.Copy(parts, 1, args, 0, args.Length);
            result = command.Run(args);
            return true;
        }

        #region Commands

        private string Help(string[] args)
        {
            var lines = new List<string>(_commands.Count);
            foreach (var command in _commands.Values) lines.Add($"{command.Usage} - {command.Description}");
            return string.Join("\n", lines);
        }

        private static string SetNovas(string[] args)
        {
            if (args.Length != 1 || !int.TryParse(args[0], out var amount) || amount < 0)
                return "Usage: set_novas <amount> (0 or more)";
            if (GameManager.Novas == null) return "No Nova progress (is there a GameManager?).";

            GameManager.Novas.SetDebugNovas(amount);
            return $"Golden Novas: {GameManager.Novas.Novas}";
        }

        #endregion

        #region Console UI

        private void Open()
        {
            _open = true;
            _openedFrame = Time.frameCount;
            _input = string.Empty;

            _savedLockState = Cursor.lockState;
            _savedCursorVisible = Cursor.visible;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

            var player = GameManager.Player;
            if (player == null) return;
            _savedInputActive = player.Input.active;
            _savedCanMove = player.Input.canMove;
            player.SetInputActive(false);
            player.SetMoveActive(false);
        }

        private void Close()
        {
            _open = false;

            Cursor.lockState = _savedLockState;
            Cursor.visible = _savedCursorVisible;

            var player = GameManager.Player;
            if (player == null) return;
            player.SetInputActive(_savedInputActive);
            player.SetMoveActive(_savedCanMove);
        }

        private void Submit()
        {
            if (string.IsNullOrWhiteSpace(_input)) return;

            AddLog("> " + _input);
            TryExecute(_input, out var result);
            if (!string.IsNullOrEmpty(result)) AddLog(result);
            _input = string.Empty;
        }

        private void AddLog(string line)
        {
            _log.AddRange(line.Split('\n'));
            while (_log.Count > MaxLogLines) _log.RemoveAt(0);
            _logText = string.Join("\n", _log);
        }

        // IMGUI keeps the console free of scene/prefab setup; it only draws (and allocates) while open.
        private void OnGUI()
        {
            if (!_open) return;

            _logStyle ??= new GUIStyle(GUI.skin.label) { fontSize = fontSize, wordWrap = false, richText = false };
            _inputStyle ??= new GUIStyle(GUI.skin.textField) { fontSize = fontSize };
            var current = Event.current;

            // The keypress that opened the console also arrives as a typed character this frame.
            if (current.type == EventType.KeyDown && Time.frameCount == _openedFrame)
            {
                current.Use();
                return;
            }

            if (current.type == EventType.KeyDown &&
                (current.keyCode == KeyCode.Return || current.keyCode == KeyCode.KeypadEnter))
            {
                Submit();
                current.Use();
            }

            var lineHeight = fontSize + 8f;
            var logHeight = lineHeight * MaxLogLines;
            var area = new Rect(10f, Screen.height - logHeight - lineHeight - 30f, Screen.width - 20f, logHeight + lineHeight + 20f);

            GUI.Box(area, GUIContent.none);
            GUI.Label(new Rect(area.x + 8f, area.y + 4f, area.width - 16f, logHeight), _logText, _logStyle);

            GUI.SetNextControlName(InputControlName);
            _input = GUI.TextField(new Rect(area.x + 8f, area.yMax - lineHeight - 8f, area.width - 16f, lineHeight), _input, _inputStyle);
            GUI.FocusControl(InputControlName);
        }

        #endregion
    }
}
