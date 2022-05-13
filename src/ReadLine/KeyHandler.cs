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

using Internal.ReadLine.Abstractions;
using System;
using System.Collections.Generic;
using System.Text;

namespace Internal.ReadLine
{
    /// <summary>
    /// The keyhandler class
    /// </summary>
    internal class KeyHandler
    {
        // Public Variables
        public string Text => _text.ToString();

        // Private Variables
        private int _cursorPos;
        private int _cursorLimit;
        private StringBuilder _text;
        private List<string> _history;
        private int _historyIndex;
        private ConsoleKeyInfo _keyInfo;
        private Dictionary<string, Action> _keyActions;
        private string[] _completions;
        private int _completionStart;
        private int _completionsIndex;
        private IConsole Console2;

        /// <summary>
        /// Whether we're at the beginning of the line
        /// </summary>
        private bool IsStartOfLine() => _cursorPos == 0;

        /// <summary>
        /// Whether we're at the end of the line
        /// </summary>
        private bool IsEndOfLine() => _cursorPos == _cursorLimit;

        /// <summary>
        /// Whether we're at the start of the console buffer
        /// </summary>
        private bool IsStartOfBuffer() => Console2.CursorLeft == 0;

        /// <summary>
        /// Whether we're at the end of the console buffer
        /// </summary>
        private bool IsEndOfBuffer() => Console2.CursorLeft == Console2.BufferWidth - 1;

        /// <summary>
        /// Whether we're at the auto-completion mode
        /// </summary>
        private bool IsInAutoCompleteMode() => _completions != null;

        // --> Cursor movement

        /// <summary>
        /// Moves the cursor to the left
        /// </summary>
        private void MoveCursorLeft()
        {
            // If we're in the beginning of the line, do absolutely nothing
            if (IsStartOfLine())
                return;

            // Check to see if we're at the beginning of the console buffer
            if (IsStartOfBuffer())
                // We're at the beginning of the buffer. Move up and to the rightmost position
                Console2.SetCursorPosition(Console2.BufferWidth - 1, Console2.CursorTop - 1);
            else
                // We're not at the beginning of the console buffer. Move the cursor one step backwards
                Console2.SetCursorPosition(Console2.CursorLeft - 1, Console2.CursorTop);

            _cursorPos--;
        }

        /// <summary>
        /// Moves the cursor to the right
        /// </summary>
        private void MoveCursorRight()
        {
            // If we're in the end of the line, do absolutely nothing
            if (IsEndOfLine())
                return;

            // Check to see if we're at the end of the console buffer
            if (IsEndOfBuffer())
                // We're at the end of the buffer. Move down and to the leftmost position
                Console2.SetCursorPosition(0, Console2.CursorTop + 1);
            else
                // We're not at the end of the console buffer. Move the cursor one step forward
                Console2.SetCursorPosition(Console2.CursorLeft + 1, Console2.CursorTop);

            _cursorPos++;
        }

        /// <summary>
        /// Moves the cursor to the beginning of the line
        /// </summary>
        private void MoveCursorHome()
        {
            while (!IsStartOfLine())
                MoveCursorLeft();
        }

        /// <summary>
        /// Moves the cursor to the end of the line
        /// </summary>
        private void MoveCursorEnd()
        {
            while (!IsEndOfLine())
                MoveCursorRight();
        }

        // --> Writing

        /// <summary>
        /// Writes the string to the console, clearing the line beforehand
        /// </summary>
        /// <param name="str">The text to be printed</param>
        private void WriteNewString(string str)
        {
            ClearLine();
            WriteString(str);
        }

        /// <summary>
        /// Writes the string to the console without clearing the line
        /// </summary>
        /// <param name="str">The text to be printed</param>
        private void WriteString(string str)
        {
            foreach (char character in str)
                WriteChar(character);
        }

        /// <summary>
        /// Writes a single character to the console
        /// </summary>
        private void WriteChar() => WriteChar(_keyInfo.KeyChar);

        /// <summary>
        /// Writes a specific character to the console
        /// </summary>
        /// <param name="c">A character to be printed</param>
        private void WriteChar(char c)
        {
            // If we're at the end of the line, just write
            if (IsEndOfLine())
            {
                // Just append the character and write it to the console
                _text.Append(c);
                Console2.Write(c.ToString());
                _cursorPos++;
            }
            else
            {
                // Get a part of the string from the cursor position
                int left = Console2.CursorLeft;
                int top = Console2.CursorTop;
                string str = _text.ToString()[_cursorPos..];

                // Inject a character to the main text in the cursor position
                _text.Insert(_cursorPos, c);

                // Write the result and set the correct console cursor position
                Console2.Write(c.ToString() + str);
                Console2.SetCursorPosition(left, top);

                // Move the cursor to the right
                MoveCursorRight();
            }

            _cursorLimit++;
        }

        // --> Clearing

        /// <summary>
        /// Erases the last letter. Simulates the backspace key.
        /// </summary>
        private void Backspace()
        {
            if (IsStartOfLine())
                return;
            MoveCursorLeft();
            DeleteChar();
        }

        /// <summary>
        /// Deletes the letter in the current position
        /// </summary>
        private void Delete()
        {
            if (IsEndOfLine())
                return;
            DeleteChar();
        }

        /// <summary>
        /// Deletes the character in the current position. Invoked by <see cref="Delete"/> and <see cref="Backspace"/>
        /// </summary>
        private void DeleteChar()
        {
            // Remove a character from the main text
            int index = _cursorPos;
            _text.Remove(index, 1);

            // Form the result
            string replacement = _text.ToString()[index..];
            
            // Write the resulting string and set the appropriate cursor position
            int left = Console2.CursorLeft;
            int top = Console2.CursorTop;
            Console2.Write(string.Format("{0} ", replacement));
            Console2.SetCursorPosition(left, top);

            // Sets the cursor limit appropriately
            _cursorLimit--;
        }

        /// <summary>
        /// Clears the entire line
        /// </summary>
        private void ClearLine()
        {
            MoveCursorEnd();
            while (!IsStartOfLine())
                Backspace();
        }

        /// <summary>
        /// Clears the line to the left
        /// </summary>
        private void ClearLineToLeft()
        {
            while (!IsStartOfLine())
                Backspace();
        }

        /// <summary>
        /// Clears the line to the right
        /// </summary>
        private void ClearLineToRight()
        {
            int pos = _cursorPos;
            MoveCursorEnd();
            while (_cursorPos > pos)
                Backspace();
        }

        /// <summary>
        /// Clears all characters until the space is spotted
        /// </summary>
        private void ClearLineUntilSpace()
        {
            while (!IsStartOfLine() && _text[_cursorPos - 1] != ' ')
                Backspace();
        }

        // --> Manipulating

        /// <summary>
        /// Transposes the two characters in the current position
        /// </summary>
        private void TransposeChars()
        {
            // Local helper functions to make life easier
            bool almostEndOfLine() => (_cursorLimit - _cursorPos) == 1;
            int incrementIf(Func<bool> expression, int index) => expression() ? index + 1 : index;
            int decrementIf(Func<bool> expression, int index) => expression() ? index - 1 : index;

            // We can't transpose the characters at the start of the line
            if (IsStartOfLine()) { return; }

            // Get the two character indexes
            int firstIdx = decrementIf(IsEndOfLine, _cursorPos - 1);
            int secondIdx = decrementIf(IsEndOfLine, _cursorPos);

            // Actually swap the two characters with each other
            (_text[firstIdx], _text[secondIdx]) = (_text[secondIdx], _text[firstIdx]);

            // Get the cursor position of the console
            int left = incrementIf(almostEndOfLine, Console2.CursorLeft);
            int cursorPosition = incrementIf(almostEndOfLine, _cursorPos);

            // Write the resulting string
            WriteNewString(_text.ToString());

            // Set the cursor position to the appropriate values
            Console2.SetCursorPosition(left, Console2.CursorTop);
            _cursorPos = cursorPosition;

            // Move the cursor to the right
            MoveCursorRight();
        }

        // --> Auto-completion

        /// <summary>
        /// Starts the auto-completion, showing the first suggestion
        /// </summary>
        private void StartAutoComplete()
        {
            while (_cursorPos > _completionStart)
                Backspace();

            // We usually start at index 0
            _completionsIndex = 0;

            // Write the suggestion
            WriteString(_completions[_completionsIndex]);
        }

        /// <summary>
        /// Goes to the next suggestion, or to the first suggestion if we reached the last suggestion
        /// </summary>
        private void NextAutoComplete()
        {
            while (_cursorPos > _completionStart)
                Backspace();

            // Increment the completion index
            _completionsIndex++;

            // If we got the last of the suggestions, go to the beginning
            if (_completionsIndex == _completions.Length)
                _completionsIndex = 0;

            // Write the suggestion
            WriteString(_completions[_completionsIndex]);
        }

        /// <summary>
        /// Goes to the previous suggestion, or to the last suggestion if we reached the first suggestion
        /// </summary>
        private void PreviousAutoComplete()
        {
            while (_cursorPos > _completionStart)
                Backspace();

            // Decrement the completion index
            _completionsIndex--;

            // If we got the first suggestion, go to the last one
            if (_completionsIndex == -1)
                _completionsIndex = _completions.Length - 1;

            // Write the suggestion
            WriteString(_completions[_completionsIndex]);
        }

        /// <summary>
        /// Resets the auto-completion
        /// </summary>
        private void ResetAutoComplete()
        {
            _completions = null;
            _completionsIndex = 0;
        }

        // --> Command history

        /// <summary>
        /// Shows the previous history
        /// </summary>
        private void PrevHistory()
        {
            if (_historyIndex > 0)
            {
                _historyIndex--;
                WriteNewString(_history[_historyIndex]);
            }
        }

        /// <summary>
        /// Shows the next history
        /// </summary>
        private void NextHistory()
        {
            if (_historyIndex < _history.Count)
            {
                _historyIndex++;
                if (_historyIndex == _history.Count)
                    // We're at the end of the history. Clear the line.
                    ClearLine();
                else
                    WriteNewString(_history[_historyIndex]);
            }
        }

        /// <summary>
        /// Builds the key input string
        /// </summary>
        /// <returns>The key (for ex. B), or the pressed modifier and the key (for ex. ControlB)</returns>
        private string BuildKeyInput()
        {
            return (_keyInfo.Modifiers != ConsoleModifiers.Control && _keyInfo.Modifiers != ConsoleModifiers.Shift) ?
                _keyInfo.Key.ToString() : _keyInfo.Modifiers.ToString() + _keyInfo.Key.ToString();
        }

        /// <summary>
        /// Initializes the new instance of the key handler class
        /// </summary>
        /// <param name="console">Console instance</param>
        /// <param name="history">History of written inputs</param>
        /// <param name="autoCompleteHandler">Auto completion handler</param>
        public KeyHandler(IConsole console, List<string> history, IAutoCompleteHandler autoCompleteHandler)
        {
            Console2 = console;

            // Initialize history and text
            _history = history ?? new List<string>();
            _historyIndex = _history.Count;
            _text = new StringBuilder();

            // Assign the key actions
            _keyActions = new Dictionary<string, Action>
            {
                // Cursor movement (left and right)
                ["LeftArrow"] = MoveCursorLeft,
                ["ControlB"] = MoveCursorLeft,
                ["RightArrow"] = MoveCursorRight,
                ["ControlF"] = MoveCursorRight,

                // Cursor movement (home and end)
                ["Home"] = MoveCursorHome,
                ["ControlA"] = MoveCursorHome,
                ["End"] = MoveCursorEnd,
                ["ControlE"] = MoveCursorEnd,

                // Deletion of one character
                ["Backspace"] = Backspace,
                ["ControlH"] = Backspace,
                ["Delete"] = Delete,
                ["ControlD"] = Delete,

                // Deletion of whole line
                ["Escape"] = ClearLine,
                ["ControlL"] = ClearLine,
                ["ControlU"] = ClearLineToLeft,
                ["ControlK"] = ClearLineToRight,
                ["ControlW"] = ClearLineUntilSpace,

                // History manipulation
                ["UpArrow"] = PrevHistory,
                ["ControlP"] = PrevHistory,
                ["DownArrow"] = NextHistory,
                ["ControlN"] = NextHistory,

                // Character transposition
                ["ControlT"] = TransposeChars,

                // Auto-completion initialization
                ["Tab"] = () =>
                {
                    if (IsInAutoCompleteMode())
                    {
                        NextAutoComplete();
                    }
                    else
                    {
                        if (autoCompleteHandler == null || !IsEndOfLine())
                            return;

                        string text = _text.ToString();

                        _completionStart = text.LastIndexOfAny(autoCompleteHandler.Separators);
                        _completionStart = _completionStart == -1 ? 0 : _completionStart + 1;

                        _completions = autoCompleteHandler.GetSuggestions(text, _completionStart);
                        _completions = _completions?.Length == 0 ? null : _completions;

                        if (_completions == null)
                            return;

                        StartAutoComplete();
                    }
                },
                ["ShiftTab"] = () =>
                {
                    if (IsInAutoCompleteMode())
                    {
                        PreviousAutoComplete();
                    }
                }
            };
        }

        /// <summary>
        /// Handles the pressed key
        /// </summary>
        /// <param name="keyInfo">Key information</param>
        public void Handle(ConsoleKeyInfo keyInfo)
        {
            _keyInfo = keyInfo;

            // Reset the auto completion if we didn't press TAB
            if (IsInAutoCompleteMode() && _keyInfo.Key != ConsoleKey.Tab)
                ResetAutoComplete();

            // Get the key input and assign it to the action defined in the actions list. Otherwise, write the character.
            _keyActions.TryGetValue(BuildKeyInput(), out Action action);
            action ??= WriteChar;
            action.Invoke();
        }
    }
}
