using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace CI.PowerConsole.Core
{
    public class ConsoleController : MonoBehaviour, IDragHandler
    {
        private const int _maxCommandHistory = 50;

        private bool _isVisible;
        public bool IsVisible
        {
            get { return _isVisible; }
            set { _isVisible = value; UpdateVisibility(); }
        }

        public string Title
        {
            get => _title.text;
            set => _title.text = value;
        }

        public bool IsEnabled { get; set; }
        public LogLevel LogLevel { get; set; } = LogLevel.Trace;
        public List<KeyCode> OpenCloseHotkeys { get; set; } = new List<KeyCode>() { KeyCode.LeftControl, KeyCode.I };

        public event EventHandler<CommandEnteredEventArgs> CommandEntered;

        private TMP_InputField _text;
        private TMP_InputField _input;
        private TextMeshProUGUI _title;
        private Button _submitButton;
        private Button _closeButton;
        private Scrollbar _scrollbar;

        private ConsoleConfig _config;
        private bool _isFollowingTail = true;
        private int _commandHistoryIndex = -1;
        private Queue<string> _buffer;

        private readonly List<string> _commandHistory = new List<string>(_maxCommandHistory);
        private readonly Dictionary<string, CustomCommand> _commands = new Dictionary<string, CustomCommand>();

        public void Awake()
        {
            _text = GameObject.Find("ConsoleFeed").GetComponent<TMP_InputField>();
            _input = GameObject.Find("ConsoleInput").GetComponent<TMP_InputField>();
            _title = GameObject.Find("ConsoleTitle").GetComponent<TextMeshProUGUI>();
            _submitButton = GameObject.Find("ConsoleSubmitButton").GetComponent<Button>();
            _closeButton = GameObject.Find("ConsoleCloseButton").GetComponent<Button>();
            _scrollbar = GameObject.Find("FeedScrollbarVertical").GetComponent<Scrollbar>();

            _submitButton.onClick.AddListener(() =>
            {
                OnEnterPressed();
            });

            _closeButton.onClick.AddListener(() =>
            {
                IsVisible = false;
            });

            _scrollbar.onValueChanged.AddListener(x =>
            {
                _isFollowingTail = x > 0.9;
            });

            RegisterCommand(new CustomCommand()
            {
                Command = "help",
                Description = "Displays help content",
                Callback = e => ShowHelpContent()
            });
            RegisterCommand(new CustomCommand()
            {
                Command = "clear",
                Description = "Clears the console",
                Callback = e => ClearDisplay()
            });
            RegisterCommand(new CustomCommand()
            {
                Command = "exit",
                Description = "Closes the console",
                Callback = e => IsVisible = false
            });
            RegisterCommand(new CustomCommand()
            {
                Command = "position fullscreen",
                Description = "Fullscreen the console",
                Callback = e =>
                {
                    _config.Position = ConsolePosition.Fullscreen;
                    SetPosition();
                }
            });
            RegisterCommand(new CustomCommand()
            {
                Command = "position top",
                Description = "Position the console at the top of the screen",
                Callback = e =>
                {
                    _config.Position = ConsolePosition.Top;
                    SetPosition();
                }
            });
            RegisterCommand(new CustomCommand()
            {
                Command = "position bottom",
                Description = "Position the console at the bottom of the screen",
                Callback = e =>
                {
                    _config.Position = ConsolePosition.Bottom;
                    SetPosition();
                }
            });

            UpdateVisibility();
        }

        public void Update()
        {
            if (OpenCloseHotkeys.Any() &&
                OpenCloseHotkeys.All(x => Input.GetKey(x)) && 
                Input.GetKeyDown(OpenCloseHotkeys.Last()))
            {
                IsVisible = !IsVisible;
            }

            if (IsVisible)
            {
                if (Input.GetKeyDown(KeyCode.Return))
                {
                    OnEnterPressed();
                }
                if (Input.GetKeyDown(KeyCode.UpArrow))
                {
                    IncrementCommandHistory(-1);
                }
                if (Input.GetKeyDown(KeyCode.DownArrow))
                {
                    IncrementCommandHistory(1);
                }
            }
        }

        public void Initialise(ConsoleConfig config)
        {
            _config = config;

            _buffer = new Queue<string>(_config.MaxBufferSize);

            SetPosition();

            IsEnabled = true;
        }

        public void Log(LogLevel logLevel, string message, bool forceScroll)
        {
            if (_buffer.Count >= _config.MaxBufferSize)
            {
                _buffer.Dequeue();
            }

            var colour = GetColour(logLevel);

            if (logLevel == LogLevel.None) 
            {
                _buffer.Enqueue($"<color=#{colour}>{DateTime.Now:HH:mm:ss} > {message}</color>");
            }
            else
            {
                _buffer.Enqueue($"<color=#{colour}>{DateTime.Now:HH:mm:ss} [{logLevel}] {message}</color>");
            }

            if (IsVisible)
            {
                RefreshDisplay();

                if (forceScroll || _isFollowingTail)
                {
                    ScrollToEnd();
                }
            }
        }

        public void ClearDisplay()
        {
            _text.text = string.Empty;
            _buffer.Clear();
        }

        public void RegisterCommand(CustomCommand command)
        {
            _commands[command.Command] = command;
        }

        public void UnregisterCommand(string command)
        {
            if (CommandExists(command))
            {
                _commands.Remove(command);
            }
        }

        public bool CommandExists(string command) => _commands.ContainsKey(command);

        private void UpdateVisibility()
        {
            transform.localScale = IsVisible ? Vector3.one : Vector3.zero;

            if (IsVisible)
            {
                RefreshDisplay();
                ScrollToEnd();
                FocusInput();
            }
        }

        private void OnEnterPressed()
        {
            Log(LogLevel.None, _input.text, true);

            HandleCommands();
            UpdateCommandHistory();
            CommandEntered?.Invoke(this, new CommandEnteredEventArgs() { Command = _input.text });

            ClearInput();
            FocusInput();
        }

        private void ClearInput()
        {
            _input.text = string.Empty;
        }

        private void FocusInput()
        {
            _input.ActivateInputField();
        }

        private void ShowHelpContent()
        {
            foreach (var command in _commands)
            {
                var descriptionString = string.IsNullOrWhiteSpace(command.Value.Description) ? string.Empty : $" - {command.Value.Description}";
                var argString = command.Value.Args != null && command.Value.Args.Any() ? $" | {string.Join(" ", command.Value.Args.Select(x => $"[{x.Name}]{(string.IsNullOrWhiteSpace(x.Description) ? string.Empty : $" {x.Description}")}"))}" : string.Empty;

                Log(LogLevel.None, $"{command.Key}{descriptionString}{argString}", true);
            }
        }

        private string GetColour(LogLevel logLevel)
        {
            switch (logLevel)
            {
                case LogLevel.Trace:
                    return ColorUtility.ToHtmlStringRGB(_config.Colours.TraceColour);
                case LogLevel.Debug:
                    return ColorUtility.ToHtmlStringRGB(_config.Colours.DebugColor);
                case LogLevel.Information:
                    return ColorUtility.ToHtmlStringRGB(_config.Colours.InformationColor);
                case LogLevel.Warning:
                    return ColorUtility.ToHtmlStringRGB(_config.Colours.WarningColor);
                case LogLevel.Error:
                    return ColorUtility.ToHtmlStringRGB(_config.Colours.ErrorColor);
                case LogLevel.Critical:
                    return ColorUtility.ToHtmlStringRGB(_config.Colours.CriticalColor);
                default:
                    return ColorUtility.ToHtmlStringRGB(_config.Colours.NoneColour);
            }
        }

        private void RefreshDisplay()
        {
            _text.text = string.Join(Environment.NewLine, _buffer);
        }

        private void ScrollToEnd()
        {
            _scrollbar.value = 1;
        }

        private void UpdateCommandHistory()
        {
            if (_commandHistory.Count >= _maxCommandHistory)
            {
                _commandHistory.RemoveAt(0);
            }

            _commandHistory.Add(_input.text);
        }

        private void IncrementCommandHistory(int value)
        {
            _commandHistoryIndex += value;

            if (_commandHistoryIndex < -1)
            {
                _commandHistoryIndex = _commandHistory.Count - 1;
            }
            else if (_commandHistoryIndex > _commandHistory.Count -1)
            {
                _commandHistoryIndex = -1;
            }

            if (_commandHistoryIndex >= 0)
            {
                _input.text = _commandHistory[_commandHistoryIndex];
                _input.MoveTextEnd(false);
            }
            else
            {
                _input.text = string.Empty;
            }
        }

        private void HandleCommands()
        {
            string command;
            var args = new Dictionary<string, string>();
            var indexOfFirstArg = _input.text.IndexOf(" -");

            if (indexOfFirstArg >= 0)
            {
                command = _input.text.Substring(0, indexOfFirstArg).Trim();

                var argString = _input.text.Substring(indexOfFirstArg + 1).Trim();
                var argsList = new List<string>();

                var index = 0;
                var ignoreSpaces = false;
                var builder = new StringBuilder();
                while (index < argString.Length)
                {
                    if (!ignoreSpaces && argString[index] == '\"')
                    {
                        ignoreSpaces = true;
                    }
                    else if (ignoreSpaces && argString[index] == '\"')
                    {
                        ignoreSpaces = false;
                    }

                    if ((argString[index] != ' ' || (argString[index] == ' ' && ignoreSpaces)) && argString[index] != '\"')
                    {
                        builder.Append(argString[index]);
                    }
                    if ((argString[index] == ' ' && !ignoreSpaces) || index == argString.Length - 1)
                    {
                        argsList.Add(builder.ToString());
                        builder.Clear();
                    }
                    index++;
                }

                index = 0;
                while (index < argsList.Count)
                {
                    var noArg = false;
                    var option = argsList[index];
                    var arg = index + 1 < argsList.Count ? argsList[index + 1]: string.Empty;

                    if (arg.StartsWith("-"))
                    {
                        arg = string.Empty;
                        noArg = true;
                    }

                    args[option] = arg;

                    index += noArg ? 1 : 2;
                }
            }
            else
            {
                command = _input.text.Trim();
            }

            if (_commands.ContainsKey(command))
            {
                _commands[command].Callback?.Invoke(new CommandCallback()
                {
                    Command = command,
                    Args = args
                });
            }
        }

        private void SetPosition()
        {
            var rectTransform = GetComponent<RectTransform>();

            if (_config.Position == ConsolePosition.Bottom)
            {
                if (_config.Width == null)
                {
                    rectTransform.anchorMin = new Vector2(0, 0);
                    rectTransform.anchorMax = new Vector2(1, 0);
                    rectTransform.pivot = new Vector2(0.5f, 0);
                    rectTransform.offsetMin = new Vector2(10, 10);
                    rectTransform.offsetMax = new Vector2(-10, 0);
                    rectTransform.sizeDelta = new Vector2(rectTransform.sizeDelta.x, _config.Height);
                }
                else
                {
                    rectTransform.anchorMin = new Vector2(0, 0);
                    rectTransform.anchorMax = new Vector2(0, 0);
                    rectTransform.pivot = new Vector2(0, 0);
                    rectTransform.offsetMin = new Vector2(10, 10);
                    rectTransform.sizeDelta = new Vector2((float)_config.Width, _config.Height);
                }
            }
            else if (_config.Position == ConsolePosition.Top)
            {
                if (_config.Width == null)
                {
                    rectTransform.anchorMin = new Vector2(0, 1);
                    rectTransform.anchorMax = new Vector2(1, 1);
                    rectTransform.pivot = new Vector2(0.5f, 1);
                    rectTransform.offsetMin = new Vector2(10, 0);
                    rectTransform.offsetMax = new Vector2(-10, -10);
                    rectTransform.sizeDelta = new Vector2(rectTransform.sizeDelta.x, _config.Height);
                }
                else
                {
                    rectTransform.anchorMin = new Vector2(0, 1);
                    rectTransform.anchorMax = new Vector2(0, 1);
                    rectTransform.pivot = new Vector2(0, 1);
                    rectTransform.offsetMin = new Vector2(10, rectTransform.offsetMin.y);
                    rectTransform.offsetMax = new Vector2(rectTransform.offsetMax.x, -10);
                    rectTransform.sizeDelta = new Vector2((float)_config.Width, _config.Height);
                }
            }
            else
            {
                rectTransform.anchorMin = new Vector2(0, 0);
                rectTransform.anchorMax = new Vector2(1, 1);
                rectTransform.pivot = new Vector2(0.5f, 0.5f);
                rectTransform.offsetMin = new Vector2(10, 10);
                rectTransform.offsetMax = new Vector2(-10, -10);
            }

            ScrollToEnd();
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (!_config.IsFixed)
            {
                transform.position = eventData.position;
            }
        }
    }
}