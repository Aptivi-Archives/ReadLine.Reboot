﻿/*
 * MIT License
 *
 * Copyright (c) 2017 Toni Solarin-Sodara
 * Copyright (c) 2022 Aptivi
 *
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 * 
 * The above copyright notice and this permission notice shall be included in all
 * copies or substantial portions of the Software.
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
 * SOFTWARE.
 * 
 */

using ReadLineReboot.Abstractions;
using System;
using System.Collections.Generic;
using System.Threading;

// TODO: Rewrite entire project as base itself is bad.
namespace ReadLineReboot
{
    /// <summary>
    /// The initial read line module
    /// </summary>
    public static class ReadLine
    {
        // Internal variables
        private static readonly List<string> _history = new();
        private static bool _readInterrupt;
        private static int _historySize = -1;
        private static int _numChars = -1;
        internal static bool _pressedEnterOnHistoryEntry;
        internal static KeyHandler _keyHandler;
        internal static object _lock = new();
        internal static char _escapeChar = Convert.ToChar(0x1B);
        internal static bool _prependAlt = false;
        internal static bool _extendedMode = false;

        // Variables
        /// <summary>
        /// Whether the history is enabled. Currently false.
        /// </summary>
        public static bool HistoryEnabled { get; set; }

        /// <summary>
        /// Whether the auto completion is enabled. Currently true.
        /// </summary>
        public static bool AutoCompletionEnabled { get; set; } = true;

        /// <summary>
        /// Whether the clipboard (kill buffer) is enabled. Currently true.
        /// </summary>
        public static bool ClipboardEnabled { get; set; } = true;

        /// <summary>
        /// Whether the undo feature is enabled. Currently true.
        /// </summary>
        public static bool UndoEnabled { get; set; } = true;

        /// <summary>
        /// Whether the default value should be pre-written or not. Currently false.
        /// </summary>
        public static bool PrewriteDefaultValue { get; set; }

        /// <summary>
        /// Whether the input is interruptible. Currently false.
        /// </summary>
        public static bool Interruptible { get; set; }

        /// <summary>
        /// If <see cref="Interruptible"/> is enabled, sets how responsive the interruption listener while waiting for keypress.
        /// <para>If zero, this means instant, but causes 100% CPU usage.</para>
        /// <para>If more than zero, this means that the listener will sleep for assigned number of milliseconds before detecting the next keys.</para>
        /// <para>Use this carefully, because it can cause missed keys depending on the application.</para>
        /// </summary>
        public static int InterruptionResponsiveness { get; set; } = 10;

        /// <summary>
        /// Whether the CTRL + C to EOL feature is enabled. Currently false. If false, exits program upon pressing CTRL + C.
        /// </summary>
        public static bool CtrlCEnabled { get; set; }

        /// <summary>
        /// Whether the last <see cref="Read(string, string, bool)"/> or <see cref="ReadPassword(string, char, bool)"/> request ran to completion.
        /// If the request is either cancelled or interrupted, this is false.
        /// </summary>
        public static bool ReadRanToCompletion { get; private set; }

        /// <summary>
        /// The auto completion handler. You need to make a class that implements <see cref="IAutoCompleteHandler"/>
        /// </summary>
        public static IAutoCompleteHandler AutoCompletionHandler { private get; set; }

        /// <summary>
        /// The prompt writing handler.
        /// </summary>
        public static Action<string> WritePrompt { internal get; set; } = (prompt) => Console.Write(prompt);

        /// <summary>
        /// The key handler.
        /// </summary>
        public static KeyHandler KeyHandler => _keyHandler;

        /// <summary>
        /// The bell style.
        /// </summary>
        public static BellType BellStyle { get; set; } = BellType.Audible;

        /// <summary>
        /// Specifies the history size. If set to 0, all entries are removed and nothing will be added to the history.
        /// If set to a value less than 0, the history size is unlimited.
        /// </summary>
        public static int HistorySize
        {
            get
            {
                return _historySize;
            }
            set
            {
                if (value == 0)
                    ClearHistory();
                _historySize = value;
            }
        }

        /// <summary>
        /// Number of characters to read. If set to -1, ReadLine will read all characters as long as ENTER is pressed.
        /// </summary>
        public static int NumberOfCharsToRead { get => _numChars; set => _numChars = value; }

        /// <summary>
        /// Adds a text or an array of texts to the history
        /// </summary>
        /// <param name="texts">The strings to add to the history</param>
        public static void AddHistory(params string[] texts)
        {
            // If we have nothing to add, return.
            if (texts == null)
                return;

            // Iterate through texts
            if (HistorySize > 0)
            {
                // We have limited history.
                foreach (string text in texts)
                {
                    // Add the text
                    _history.Add(text);

                    // Check to see if we've exceeded the history limit
                    if (_history.Count > HistorySize)
                    {
                        // We may have more than one history entry added during the unlimited history size, so remove
                        // entries until we reach the limit
                        while (_history.Count > HistorySize)
                            _history.RemoveAt(0);
                    }
                }
            }
            else if (HistorySize < 0)
            {
                // We have unlimited history.
                _history.AddRange(texts);
            }
        }

        /// <summary>
        /// Gets the current history
        /// </summary>
        public static List<string> GetHistory() => _history;

        /// <summary>
        /// Clears the history
        /// </summary>
        public static void ClearHistory() => _history.Clear();

        /// <summary>
        /// Sets the history
        /// </summary>
        public static void SetHistory(List<string> history)
        {
            _history.Clear();
            _history.AddRange(history);
        }

        /// <summary>
        /// Adds your own custom binding. If a custom binding already exists, updates the custom binding.
        /// </summary>
        /// <param name="key">Console key to press</param>
        /// <param name="action">Action to be done when key is pressed</param>
        public static void AddCustomBinding(ConsoleKeyInfo key, Action action)
        {
            string ConsoleKeyName = KeyTools.BuildKeyInput(key);
            if (!KeyBindings._customKeyBindings.ContainsKey(ConsoleKeyName))
                KeyBindings._customKeyBindings.Add(ConsoleKeyName, action);
            else
                ChangeCustomBinding(key, action);
        }

        /// <summary>
        /// Changes your own custom binding
        /// </summary>
        /// <param name="key">Console key to press</param>
        /// <param name="action">Action to be done when key is pressed</param>
        public static void ChangeCustomBinding(ConsoleKeyInfo key, Action action)
        {
            string ConsoleKeyName = KeyTools.BuildKeyInput(key);
            if (KeyBindings._customKeyBindings.ContainsKey(ConsoleKeyName))
                KeyBindings._customKeyBindings[ConsoleKeyName] = action;
        }

        /// <summary>
        /// Removes your own custom binding
        /// </summary>
        /// <param name="key">Console key to press</param>
        public static void RemoveCustomBinding(ConsoleKeyInfo key)
        {
            string ConsoleKeyName = KeyTools.BuildKeyInput(key);
            KeyBindings._customKeyBindings.Remove(ConsoleKeyName);
        }

        /// <summary>
        /// Writes the prompt and reads the input
        /// </summary>
        /// <param name="prompt">The prompt to write</param>
        /// <param name="defaultText">The default text to write if nothing is written</param>
        /// <param name="force">Force read the input (may cause instability)</param>
        /// <returns>The written text if anything is input from the user, or the default text if nothing if printed</returns>
        public static string Read(string prompt = "", string defaultText = "", bool force = true)
        {
            if (force)
                return ReadInternal(prompt, defaultText);
            else
            {
                lock (_lock)
                {
                    return ReadInternal(prompt, defaultText);
                }
            }
        }

        /// <summary>
        /// Writes the prompt and reads the input while masking the written input
        /// </summary>
        /// <param name="prompt">The prompt to write</param>
        /// <param name="mask">Character to use to mask password</param>
        /// <param name="force">Force read the input (may cause instability)</param>
        /// <returns>The written text</returns>
        public static string ReadPassword(string prompt = "", char mask = default, bool force = true)
        {
            if (force)
                return ReadPasswordInternal(prompt, mask);
            else
            {
                lock (_lock)
                {
                    return ReadPasswordInternal(prompt, mask);
                }
            }
        }

        /// <summary>
        /// Interrupts reading from the console
        /// </summary>
        public static void InterruptRead() => _readInterrupt = true;

        private static string ReadInternal(string prompt = "", string defaultText = "")
        {
            // Reset the flag
            ReadRanToCompletion = false;

            // Get initial cursor left and top positions
            int _prePromptCursorLeft = Console.CursorLeft;
            int _prePromptCursorTop = Console.CursorTop;

            // Prepare the prompt
            WritePrompt.Invoke(prompt);
            _keyHandler = new KeyHandler(new ConsoleWrapper(), _history, AutoCompletionHandler)
            {
                // Prepare initial variables
                _initialPrompt = prompt,
                _cachedPrompt = prompt,
                _prePromptCursorLeft = _prePromptCursorLeft,
                _prePromptCursorTop = _prePromptCursorTop
            };

            // Initialize bindings
            KeyBindings.InitializeBindings();

            // Pre-write default value if enabled
            if (PrewriteDefaultValue && !string.IsNullOrWhiteSpace(defaultText))
                _keyHandler.WriteNewString(defaultText);

            // Get the written text
            string text = GetText();

            // Add the text to the history if the text is written
            if (string.IsNullOrWhiteSpace(text) && !string.IsNullOrWhiteSpace(defaultText))
            {
                // User didn't input any text. Therefore, set the text to the default value
                text = defaultText;
            }
            else
            {
                // If the history is enabled and the text was written, add the text to the history
                if (HistoryEnabled && !_pressedEnterOnHistoryEntry && !string.IsNullOrWhiteSpace(text))
                    AddHistory(text);
            }

            // Reset the flag and return the text
            _pressedEnterOnHistoryEntry = false;
            return text;
        }

        private static string ReadPasswordInternal(string prompt = "", char mask = default)
        {
            // Reset the flag
            ReadRanToCompletion = false;

            // Get initial cursor left and top positions
            int _prePromptCursorLeft = Console.CursorLeft;
            int _prePromptCursorTop = Console.CursorTop;

            // Prepare the prompt
            WritePrompt.Invoke(prompt);
            _keyHandler = new KeyHandler(new ConsoleWrapper() { PasswordMode = true, PasswordMaskChar = mask }, null, null)
            {
                // Prepare initial variables
                _initialPrompt = prompt,
                _cachedPrompt = prompt,
                _prePromptCursorLeft = _prePromptCursorLeft,
                _prePromptCursorTop = _prePromptCursorTop
            };

            // Initialize bindings
            KeyBindings.InitializeBindings();

            // Get the written text
            return GetText();
        }

        private static bool TryGetConsoleKey(out ConsoleKeyInfo key)
        {
            if (Console.KeyAvailable)
            {
                key = Console.ReadKey(true);
                return true;
            }
            else
            {
                key = new ConsoleKeyInfo();
                return false;
            }
        }

        private static string GetText()
        {
            bool _ctrlCPressed = false;
            string _output = "";
            int _chars = 0;

            // Check to see if we're going to treat CTRL + C as actual input
            if (CtrlCEnabled)
                Console.TreatControlCAsInput = true;

            // Stop handling keys if Enter is pressed
            if (Interruptible)
            {
                while (!_readInterrupt)
                {
                    while (TryGetConsoleKey(out ConsoleKeyInfo keyInfo))
                    {
                        if (keyInfo.Key != ConsoleKey.Enter &&
                            !keyInfo.Equals(KeyHandler.SimulatedEnter) &&
                            !(keyInfo.Equals(KeyHandler.SimulatedEnterAlt) && _keyHandler.Text.Length == 0) &&
                            !keyInfo.Equals(KeyHandler.SimulatedEnterCtrlC) &&
                            _chars != _numChars)
                        {
                            // Handle the key as appropriate
                            if (_prependAlt)
                            {
                                keyInfo = new ConsoleKeyInfo(keyInfo.KeyChar, keyInfo.Key,
                                                             keyInfo.Modifiers.HasFlag(ConsoleModifiers.Shift), 
                                                             true, 
                                                             keyInfo.Modifiers.HasFlag(ConsoleModifiers.Control));
                                _prependAlt = false;
                            }
                            _keyHandler.Handle(keyInfo);
                            _chars += 1;
                        }
                        else
                        {
                            // Handle CTRL + C
                            if (keyInfo.Equals(KeyHandler.SimulatedEnterCtrlC))
                                _ctrlCPressed = true;
                            break;
                        }
                    }
                    Thread.Sleep(InterruptionResponsiveness);
                }
            }
            else
            {
                // Get the initial key
                ConsoleKeyInfo keyInfo = Console.ReadKey(true);

                // Stop handling keys if Enter is pressed
                while (keyInfo.Key != ConsoleKey.Enter &&
                       !keyInfo.Equals(KeyHandler.SimulatedEnter) &&
                       !(keyInfo.Equals(KeyHandler.SimulatedEnterAlt) && _keyHandler.Text.Length == 0) &&
                       !keyInfo.Equals(KeyHandler.SimulatedEnterCtrlC) &&
                       _chars != _numChars)
                {
                    // Handle the key as appropriate
                    if (_prependAlt)
                    {
                        keyInfo = new ConsoleKeyInfo(keyInfo.KeyChar, keyInfo.Key,
                                                     keyInfo.Modifiers.HasFlag(ConsoleModifiers.Shift),
                                                     true,
                                                     keyInfo.Modifiers.HasFlag(ConsoleModifiers.Control));
                        _prependAlt = false;
                    }
                    _keyHandler.Handle(keyInfo);
                    keyInfo = Console.ReadKey(true);
                    _chars += 1;
                }

                // Handle CTRL + C
                if (keyInfo.Equals(KeyHandler.SimulatedEnterCtrlC))
                    _ctrlCPressed = true;
            }

            // Restore CTRL + C state
            if (CtrlCEnabled)
                Console.TreatControlCAsInput = false;

            // Check to see if ENTER is pressed in the middle of the history
            if (_keyHandler._historyIndex != _keyHandler._history.Count && _keyHandler._currentLineEditHistory.Count == 0)
                _pressedEnterOnHistoryEntry = true;

            // Check to see if we're aborting
            if (_ctrlCPressed)
            {
                // We're aborting. Return nothing.
                Console.WriteLine("^C");
            }
            else if (!_readInterrupt || !Interruptible)
            {
                // Write a new line and get the text
                Console.WriteLine();
                ReadRanToCompletion = true;
                _output = _keyHandler.Text;
            }
            else
            {
                // Read is interrupted. Print a new line.
                _readInterrupt = false;
                Console.WriteLine();
            }
            return _output;
        }

        internal static void RingBell()
        {
            switch (BellStyle)
            {
                case BellType.None:
                    break;
                case BellType.Audible:
                    // Write the bell character to the console
                    Console.Write("\a");
                    break;
                case BellType.Visible:
                    // Try to emit the visible bell
                    Console.Write($"{_escapeChar}[?5h");
                    Thread.Sleep(100);
                    Console.Write($"{_escapeChar}[?5l");
                    break;
                default:
                    break;
            }
        }
    }
}
