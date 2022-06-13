/*
 * MIT License
 *
 * Copyright (c) 2017 Toni Solarin-Sodara
 * Copyright (c) 2022 EoflaOE and its companies
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

using Internal.ReadLineReboot;
using Internal.ReadLineReboot.Abstractions;
using System;
using System.Collections.Generic;
using System.Threading;

namespace ReadLineReboot
{
    /// <summary>
    /// The initial read line module
    /// </summary>
    public static class ReadLine
    {
        // Internal variables
        private static readonly List<string> _history = new List<string>();
        private static bool _readInterrupt;
        internal static bool _pressedEnterOnHistoryEntry;

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
        /// Whether the CTRL + C to EOL feature is enabled. Currently false. If false, exits program upon pressing CTRL + C.
        /// </summary>
        public static bool CtrlCEnabled { get; set; }

        /// <summary>
        /// Whether the last <see cref="Read(string, string)"/> or <see cref="ReadPassword(string, char)"/> request ran to completion.
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
        public static Action<string> WritePrompt { private get; set; } = (prompt) => Console.Write(prompt);

        /// <summary>
        /// Adds a text or an array of texts to the history
        /// </summary>
        /// <param name="texts">The strings to add to the history</param>
        public static void AddHistory(params string[] texts) => _history.AddRange(texts);

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
        /// Writes the prompt and reads the input
        /// </summary>
        /// <param name="prompt">The prompt to write</param>
        /// <param name="defaultText">The default text to write if nothing is written</param>
        /// <returns>The written text if anything is input from the user, or the default text if nothing if printed</returns>
        public static string Read(string prompt = "", string defaultText = "")
        {
            // Reset the flag
            ReadRanToCompletion = false;

            // Prepare the prompt
            WritePrompt.Invoke(prompt);
            KeyHandler keyHandler = new KeyHandler(new ConsoleWrapper(), _history, AutoCompletionHandler);

            // Pre-write default value if enabled
            if (PrewriteDefaultValue && !string.IsNullOrWhiteSpace(defaultText))
                keyHandler.WriteNewString(defaultText);

            // Get the written text
            string text = GetText(keyHandler);

            // Add the text to the history if the text is written
            if (string.IsNullOrWhiteSpace(text) && !string.IsNullOrWhiteSpace(defaultText))
            {
                // User didn't input any text. Therefore, set the text to the default value
                text = defaultText;
            }
            else
            {
                // If the history is enabled, add the text to the history
                if (HistoryEnabled && !_pressedEnterOnHistoryEntry)
                    AddHistory(text);
            }

            // Reset the flag and return the text
            _pressedEnterOnHistoryEntry = false;
            return text;
        }

        /// <summary>
        /// Writes the prompt and reads the input while masking the written input
        /// </summary>
        /// <param name="prompt">The prompt to write</param>
        /// <param name="mask">Character to use to mask password</param>
        /// <returns>The written text</returns>
        public static string ReadPassword(string prompt = "", char mask = default)
        {
            // Reset the flag
            ReadRanToCompletion = false;

            // Prepare the prompt
            WritePrompt.Invoke(prompt);
            KeyHandler keyHandler = new KeyHandler(new ConsoleWrapper() { PasswordMode = true, PasswordMaskChar = mask }, null, null);

            // Get the written text
            return GetText(keyHandler);
        }

        /// <summary>
        /// Interrupts reading from the console
        /// </summary>
        public static void InterruptRead() => _readInterrupt = true;

        private static string GetText(KeyHandler keyHandler)
        {
            bool _ctrlCPressed = false;
            string _output = "";

            // Check to see if we're going to treat CTRL + C as actual input
            if (CtrlCEnabled)
                Console.TreatControlCAsInput = true;

            // Stop handling keys if Enter is pressed
            if (Interruptible)
            {
                // For some weird reason, keys will be missed if we're sleeping for 1 millisecond, but when we remove the sleep, the CPU will hog.
                // Something has to be done about it.
                while (!_readInterrupt)
                {
                    if (Console.KeyAvailable)
                    {
                        ConsoleKeyInfo keyInfo = Console.ReadKey(true);
                        if (keyInfo.Key != ConsoleKey.Enter &&
                            !keyInfo.Equals(KeyHandler.SimulatedEnter) &&
                            !(keyInfo.Equals(KeyHandler.SimulatedEnterAlt) && keyHandler.Text.Length == 0) &&
                            !keyInfo.Equals(KeyHandler.SimulatedEnterCtrlC))
                        {
                            // Handle the key as appropriate
                            keyHandler.Handle(keyInfo);
                        } 
                        else
                        {
                            // Handle CTRL + C
                            if (keyInfo.Equals(KeyHandler.SimulatedEnterCtrlC))
                                _ctrlCPressed = true;
                            break;
                        }
                    }
                }
            }
            else
            {
                // Get the initial key
                ConsoleKeyInfo keyInfo = Console.ReadKey(true);

                // Stop handling keys if Enter is pressed
                while (keyInfo.Key != ConsoleKey.Enter &&
                       !keyInfo.Equals(KeyHandler.SimulatedEnter) &&
                       !(keyInfo.Equals(KeyHandler.SimulatedEnterAlt) && keyHandler.Text.Length == 0) &&
                       !keyInfo.Equals(KeyHandler.SimulatedEnterCtrlC))
                {
                    // Handle the key as appropriate
                    keyHandler.Handle(keyInfo);
                    keyInfo = Console.ReadKey(true);
                }

                // Handle CTRL + C
                if (keyInfo.Equals(KeyHandler.SimulatedEnterCtrlC))
                    _ctrlCPressed = true;
            }

            // Restore CTRL + C state
            if (CtrlCEnabled)
                Console.TreatControlCAsInput = false;

            // Check to see if ENTER is pressed in the middle of the history
            if (keyHandler._historyIndex != keyHandler._history.Count && keyHandler._currentLineEditHistory.Count == 0)
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
                _output = keyHandler.Text;
            } 
            else
            {
                // Read is interrupted. Print a new line.
                _readInterrupt = false;
                Console.WriteLine();
            }
            return _output;
        }
    }
}
