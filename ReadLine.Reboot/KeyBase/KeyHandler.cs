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

using Internal.ReadLineReboot.Abstractions;
using ReadLineReboot;
using System;
using System.Collections.Generic;
using System.Text;

namespace Internal.ReadLineReboot
{
    /// <summary>
    /// The keyhandler class
    /// </summary>
    internal class KeyHandler
    {
        // Public Variables
        /// <summary>
        /// The current text
        /// </summary>
        public string Text => _text.ToString();

        /// <summary>
        /// The current line
        /// </summary>
        public string CurrentLine => _currentLine.ToString();

        /// <summary>
        /// The current console clipboard content for use with CTRL + Y (Yank)
        /// </summary>
        public string KillBuffer => _killBuffer.ToString();

        // Private Variables
        private int _argDigit;
        private int _cursorPos;
        private int _cursorLimit;
        private ConsoleKeyInfo _keyInfo;
        private string[] _completions;
        private int _completionStart;
        private int _completionsIndex;
        private string _lastHandler;
        private string _currentHandler;
        private bool _updateCurrentLine = true;
        private bool _updateCurrentLineHistory = true;
        private bool _middleOfWriteNewString;
        private bool _middleOfUndoAll;
        private bool _middleOfUndo;
        private bool _middleOfArgInsert;
        private readonly StringBuilder _text;
        private readonly StringBuilder _killBuffer;
        private readonly StringBuilder _currentLine;
        private readonly Dictionary<string, Action> _keyActions;
        private readonly IAutoCompleteHandler _autoCompleteHandler;
        private readonly IConsole ConsoleWrapper;
        private readonly char Escape = Convert.ToChar(27);
        internal int _historyIndex;
        internal string _cachedPrompt;
        internal string _initialPrompt;
        internal int _prePromptCursorLeft;
        internal int _prePromptCursorTop;
        internal readonly List<string> _history;
        internal readonly List<string> _currentLineEditHistory;

        // Private Properties
        private bool IsStartOfLine => _cursorPos == 0;
        private bool IsEndOfLine => _cursorPos == _cursorLimit;
        private bool IsStartOfBuffer => ConsoleWrapper.CursorLeft == 0;
        private bool IsEndOfBuffer => ConsoleWrapper.CursorLeft == ConsoleWrapper.BufferWidth - 1;
        private bool IsInAutoCompleteMode => _completions != null;
        private bool IsKillBufferEmpty => _killBuffer.Length == 0;
        internal static ConsoleKeyInfo SimulatedEnter => new ConsoleKeyInfo('\u000A', ConsoleKey.J, false, false, true);
        internal static ConsoleKeyInfo SimulatedEnterAlt => new ConsoleKeyInfo('\u0004', ConsoleKey.D, false, false, true);
        internal static ConsoleKeyInfo SimulatedEnterCtrlC => new ConsoleKeyInfo('\u0003', ConsoleKey.C, false, false, true);

        #region Cursor Movement

        /// <summary>
        /// Moves the cursor to the left once
        /// </summary>
        private void MoveCursorLeft()
        {
            MoveCursorLeft(1);
        }

        /// <summary>
        /// Moves the cursor to the left n times
        /// </summary>
        private void MoveCursorLeft(int count)
        {
            int CursorLeft = ConsoleWrapper.CursorLeft;
            int CursorTop = ConsoleWrapper.CursorTop;

            for (int i = 0; i < count; i++)
            {
                // If we're in the beginning of the line, do absolutely nothing
                if (IsStartOfLine)
                    return;

                // Check to see if we're at the beginning of the console buffer
                if (CursorLeft == 0)
                {
                    // We're at the beginning of the buffer. Move up and to the rightmost position
                    CursorLeft = ConsoleWrapper.BufferWidth - 1;
                    CursorTop -= 1;
                }
                else
                    // We're not at the beginning of the console buffer. Move the cursor one step backwards
                    CursorLeft -= 1;

                _cursorPos--;
            }

            // Now, go to the final position
            ConsoleWrapper.SetCursorPosition(CursorLeft, CursorTop);
        }

        /// <summary>
        /// Moves the cursor to the right once
        /// </summary>
        private void MoveCursorRight()
        {
            MoveCursorRight(1);
        }

        /// <summary>
        /// Moves the cursor to the right n times
        /// </summary>
        private void MoveCursorRight(int count)
        {
            int CursorLeft = ConsoleWrapper.CursorLeft;
            int CursorTop = ConsoleWrapper.CursorTop;

            for (int i = 0; i < count; i++)
            { 
                // If we're in the end of the line, do absolutely nothing
                if (IsEndOfLine)
                    return;

                // Check to see if we're at the end of the console buffer
                if (CursorLeft == ConsoleWrapper.BufferWidth - 1)
                {
                    // We're at the end of the buffer. Move down and to the leftmost position
                    CursorLeft = 0;
                    CursorTop += 1;
                }
                else
                    // We're not at the end of the console buffer. Move the cursor one step forward
                    CursorLeft += 1;

                _cursorPos++;
            }

            // Now, go to the final position
            ConsoleWrapper.SetCursorPosition(CursorLeft, CursorTop);
        }

        /// <summary>
        /// Moves the cursor to the beginning of the line
        /// </summary>
        private void MoveCursorHome()
        {
            MoveCursorLeft(_currentLine.Length);
        }

        /// <summary>
        /// Moves the cursor to the end of the line
        /// </summary>
        private void MoveCursorEnd()
        {
            MoveCursorRight(_currentLine.Length);
        }

        /// <summary>
        /// Moves the cursor to the left one word
        /// </summary>
        private void MoveCursorWordLeft()
        {
            while (!IsStartOfLine && _text[_cursorPos - 1] == ' ')
                MoveCursorLeft();
            while (!IsStartOfLine && _text[_cursorPos - 1] != ' ')
                MoveCursorLeft();
        }

        /// <summary>
        /// Moves the cursor to the right one word
        /// </summary>
        private void MoveCursorWordRight()
        {
            while (!IsEndOfLine && _text[_cursorPos] == ' ')
                MoveCursorRight();
            while (!IsEndOfLine && _text[_cursorPos] != ' ')
                MoveCursorRight();
        }
        #endregion

        #region Writing
        /// <summary>
        /// Writes the string to the console, clearing the line beforehand
        /// </summary>
        /// <param name="str">The text to be printed</param>
        internal void WriteNewString(string str)
        {
            // We're in the middle of the job
            _middleOfWriteNewString = true;

            // Write the text after clearing the line
            ClearLine();
            WriteString(str);

            // Update the current line history
            _middleOfWriteNewString = false;
            UpdateCurrentLine();
        }

        /// <summary>
        /// Writes the string to the console without clearing the line
        /// </summary>
        /// <param name="str">The text to be printed</param>
        private void WriteString(string str)
        {
            // We're in the middle of the job
            _updateCurrentLineHistory = false;

            foreach (char character in str)
                WriteChar(character);

            // Update the current line history
            _updateCurrentLineHistory = true;
            UpdateCurrentLine();
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
            // If we have tabs, convert the character to eight spaces
            string finalChar = c.ToString();
            int tabLength = 8 - ConsoleWrapper.CursorLeft % 8;
            if (c == '\t')
                finalChar = new string(' ', tabLength);

            // If the character isn't a null character, go on
            if (c != default)
            {
                // If we're at the end of the line, just write
                if (IsEndOfLine)
                {
                    // Just append the character and write it to the console
                    _text.Append(finalChar);
                    UpdateCurrentLine();
                    ConsoleWrapper.Write(finalChar);
                    _cursorPos += finalChar.Length;

                    // Increase the cursor limit
                    _cursorLimit += finalChar.Length;
                }
                else
                {
                    // Get a part of the string from the cursor position
                    int left = ConsoleWrapper.CursorLeft;
                    int top = ConsoleWrapper.CursorTop;
#if NETCOREAPP
                    string str = _text.ToString()[_cursorPos..];
#else
                    string str = _text.ToString().Substring(_cursorPos);
#endif

                    // Inject a character to the main text in the cursor position
                    _text.Insert(_cursorPos, finalChar);
                    UpdateCurrentLine();

                    // Write the result and set the correct console cursor position
                    ConsoleWrapper.Write(finalChar + str);
                    ConsoleWrapper.SetCursorPosition(left, top);

                    // Increase the cursor limit
                    _cursorLimit += finalChar.Length;

                    // Move the cursor to the right
                    for (int i = 0; i < finalChar.Length; i++)
                        MoveCursorRight();
                }
            }
        }

        /// <summary>
        /// Inserts the comment to the current command
        /// </summary>
        private void InsertComment()
        {
            int initialConsoleLeft = ConsoleWrapper.CursorLeft + 1;
            int initialCursorLeft = _cursorPos + 1;

            // Drag the cursor to the beginning of the line
            MoveCursorHome();

            // Add the comment
            WriteChar('#');

            // Restore the current position
            ConsoleWrapper.SetCursorPosition(initialConsoleLeft, ConsoleWrapper.CursorTop);
            _cursorPos = initialCursorLeft;
        }

        /// <summary>
        /// Inserts the home directory by replacing the tilde
        /// </summary>
        private void InsertHomeDirectory()
        {
            // We can't do this when the text is empty
            if (_text.Length == 0)
                return;

            // Get the characters on and behind the cursor
            char onCursor = _cursorPos != _cursorLimit ? _text[_cursorPos] : ' ';
            char behindCursor = _cursorPos == 0 ? _text[_cursorPos] : _text[_cursorPos - 1];
            bool canBackspace = _cursorPos != 0;

            // Get the home directory based on platform
            bool isOnWindows = Environment.OSVersion.Platform == PlatformID.Win32NT;
            string homeDir = isOnWindows ? Environment.GetEnvironmentVariable("USERPROFILE") : Environment.GetEnvironmentVariable("HOME");
            bool writeHomeDir = false;

            // Delete the tilde to replace it with the home directory
            if (onCursor == '~')
            {
                DeleteChar();
                writeHomeDir = true;
            }
            else if (behindCursor == '~')
            {
                if (canBackspace)
                    Backspace();
                else
                    DeleteChar();
                writeHomeDir = true;
            }

            // Now, do the job!
            if (writeHomeDir)
                WriteString(homeDir);
        }
        #endregion

        #region Clearing
        /// <summary>
        /// Erases the last letter. Simulates the backspace key.
        /// </summary>
        private void Backspace()
        {
            Backspace(1);
        }

        /// <summary>
        /// Erases the last letter. Simulates the backspace key.
        /// </summary>
        private void Backspace(int count)
        {
            if (IsStartOfLine)
                return;
            MoveCursorLeft(count);
            DeleteChar(count);
        }

        /// <summary>
        /// Deletes the letter in the current position
        /// </summary>
        private void Delete()
        {
            if (IsEndOfLine)
                return;
            DeleteChar();
        }

        /// <summary>
        /// Deletes the character in the current position. Invoked by <see cref="Delete"/> and <see cref="Backspace()"/>
        /// </summary>
        private void DeleteChar()
        {
            DeleteChar(1);
        }

        /// <summary>
        /// Deletes the character in the current position. Invoked by <see cref="Delete"/> and <see cref="Backspace()"/>
        /// </summary>
        private void DeleteChar(int count)
        {
            // Remove a character from the main text
            int index = _cursorPos;
            _text.Remove(index, count);
            UpdateCurrentLine();

            // Form the result
#if NETCOREAPP
            string replacement = _text.ToString()[index..];
#else
            string replacement = _text.ToString().Substring(index);
#endif

            // Write the resulting string and set the appropriate cursor position
            int left = ConsoleWrapper.CursorLeft;
            int top = ConsoleWrapper.CursorTop;
            string spaces = new string(' ', count);
            if (ConsoleWrapper.PasswordMode && ConsoleWrapper.PasswordMaskChar != default)
            {
                // Write the replacement, but use Console.Write to write the space, because we need to ensure that it really got deleted on render.
                ConsoleWrapper.Write(replacement);
                Console.Write(spaces);
            }
            else
            {
                ConsoleWrapper.Write($"{replacement}{spaces}");
            }
            ConsoleWrapper.SetCursorPosition(left, top);

            // Sets the cursor limit appropriately
            _cursorLimit -= count;
        }

        /// <summary>
        /// Clears the entire line
        /// </summary>
        private void ClearLine()
        {
            // We're in the middle of the job
            _updateCurrentLineHistory = false;

            // Clear the line
            MoveCursorEnd();
            while (!IsStartOfLine)
                Backspace();

            // Update the current line history
            _updateCurrentLineHistory = true;
            UpdateCurrentLine();
        }

        /// <summary>
        /// Clears the line to the left
        /// </summary>
        private void ClearLineToLeft()
        {
            // We're in the middle of the job
            _updateCurrentLineHistory = false;

            // Clear the kill buffer if the last handler is not this command
            List<char> chars = new List<char>();
            if (_lastHandler != nameof(ClearLineToLeft))
                _killBuffer.Clear();

            // Now, do the job
            while (!IsStartOfLine)
            {
                chars.Add(_text[_cursorPos - 1]);
                Backspace();
            }

            // Append the wiped characters to the kill buffer
            chars.Reverse();
            _killBuffer.Append(string.Join("", chars));

            // Update the current line history
            _updateCurrentLineHistory = true;
            UpdateCurrentLine();
        }

        /// <summary>
        /// Clears the line to the right
        /// </summary>
        private void ClearLineToRight()
        {
            // We're in the middle of the job
            _updateCurrentLineHistory = false;

            // Clear the kill buffer if the last handler is not this command
            List<char> chars = new List<char>();
            if (_lastHandler != nameof(ClearLineToRight))
                _killBuffer.Clear();

            // Now, do the job
            int pos = _cursorPos;
            MoveCursorEnd();
            while (_cursorPos > pos)
            {
                chars.Add(_text[_cursorPos - 1]);
                Backspace();
            }

            // Append the wiped characters to the kill buffer
            chars.Reverse();
            _killBuffer.Append(string.Join("", chars));

            // Update the current line history
            _updateCurrentLineHistory = true;
            UpdateCurrentLine();
        }

        /// <summary>
        /// Clears all characters until the space is spotted
        /// </summary>
        private void ClearLineUntilSpace()
        {
            // We're in the middle of the job
            _updateCurrentLineHistory = false;

            // Clear the kill buffer if the last handler is not this command
            List<char> chars = new List<char>();
            if (_lastHandler != nameof(ClearLineUntilSpace))
                _killBuffer.Clear();

            // Clear all whitespaces found
            while (!IsStartOfLine && char.IsWhiteSpace(_text[_cursorPos - 1]))
            {
                chars.Add(_text[_cursorPos - 1]);
                Backspace();
            }

            // Now, clear all the letters until we've found a whitespace
            while (!IsStartOfLine && !char.IsWhiteSpace(_text[_cursorPos - 1]))
            {
                chars.Add(_text[_cursorPos - 1]);
                Backspace();
            }

            // Append the wiped characters to the kill buffer
            chars.Reverse();
            _killBuffer.Append(string.Join("", chars));

            // Update the current line history
            _updateCurrentLineHistory = true;
            UpdateCurrentLine();
        }

        /// <summary>
        /// Clears all horizontal space
        /// </summary>
        private void ClearHorizontalSpace()
        {
            // We're in the middle of the job
            _updateCurrentLineHistory = false;

            // Clear all whitespaces found to the right
            while (!IsEndOfLine && char.IsWhiteSpace(_text[_cursorPos]))
                Delete();

            // Clear all whitespaces found to the left
            while (!IsStartOfLine && char.IsWhiteSpace(_text[_cursorPos - 1]))
                Backspace();

            // Update the current line history
            _updateCurrentLineHistory = true;
            UpdateCurrentLine();
        }

        /// <summary>
        /// Clears all characters after the space is spotted
        /// </summary>
        private void ClearLineAfterSpace()
        {
            // We're in the middle of the job
            _updateCurrentLineHistory = false;

            // Clear the kill buffer if the last handler is not this command
            List<char> chars = new List<char>();
            if (_lastHandler != nameof(ClearLineAfterSpace))
                _killBuffer.Clear();

            // Clear all whitespaces found
            while (!IsEndOfLine && char.IsWhiteSpace(_text[_cursorPos]))
            {
                chars.Add(_text[_cursorPos]);
                Delete();
            }

            // Now, clear all the letters until we've found a whitespace
            while (!IsEndOfLine && !char.IsWhiteSpace(_text[_cursorPos]))
            {
                chars.Add(_text[_cursorPos]);
                Delete();
            }

            // Append the wiped characters to the kill buffer
            _killBuffer.Append(string.Join("", chars));

            // Update the current line history
            _updateCurrentLineHistory = true;
            UpdateCurrentLine();
        }
        #endregion

        #region Manipulating
        /// <summary>
        /// Transposes the two characters in the current position
        /// </summary>
        private void TransposeChars()
        {
            // Local helper functions to make life easier
            bool almostEndOfLine() => (_cursorLimit - _cursorPos) == 1;
            int incrementIf(bool expression, int index) => expression ? index + 1 : index;
            int decrementIf(bool expression, int index) => expression ? index - 1 : index;

            // We can't transpose the characters at the start of the line
            if (IsStartOfLine) 
                return;

            // Get the two character indexes
            int firstIdx = decrementIf(IsEndOfLine, _cursorPos - 1);
            int secondIdx = decrementIf(IsEndOfLine, _cursorPos);

            // Actually swap the two characters with each other
            (_text[firstIdx], _text[secondIdx]) = (_text[secondIdx], _text[firstIdx]);

            // Get the cursor position of the console
            int left = incrementIf(almostEndOfLine(), ConsoleWrapper.CursorLeft);
            int cursorPosition = incrementIf(almostEndOfLine(), _cursorPos);

            // Write the resulting string
            WriteNewString(_text.ToString());

            // Set the cursor position to the appropriate values
            ConsoleWrapper.SetCursorPosition(left, ConsoleWrapper.CursorTop);
            _cursorPos = cursorPosition;

            // Move the cursor to the right
            MoveCursorRight();
        }

        /// <summary>
        /// Transposes the two words in the current position
        /// </summary>
        private void TransposeWords()
        {
            // We can't do this at the end of the line
            if (IsEndOfLine)
                return;

            // We can't transpose the words in the middle of the words
            if (_text[_cursorPos] != ' ')
                return;

            // Build the two words required
            List<char> wordChars = new List<char>();
            StringBuilder firstWord = new StringBuilder();
            StringBuilder secondWord = new StringBuilder();
            int currentCursorPosition = _cursorPos - 1;

            // Build the first word
            while (currentCursorPosition >= 0)
            {
                if (_text[currentCursorPosition] != ' ')
                {
                    // Add the word characters in reverse order
                    wordChars.Add(_text[currentCursorPosition]);
                    currentCursorPosition -= 1;
                }
                else
                {
                    // We've reached the end of word.
                    break;
                }
            }
            wordChars.Reverse();
            firstWord.Append(string.Join("", wordChars));
            wordChars.Clear();

            // Build the second word
            currentCursorPosition = _cursorPos + 1;
            while (currentCursorPosition <= _cursorLimit - 1)
            {
                if (_text[currentCursorPosition] != ' ')
                {
                    // Add the word characters in order
                    wordChars.Add(_text[currentCursorPosition]);
                    currentCursorPosition += 1;
                }
                else
                {
                    // We've reached the end of word.
                    break;
                }
            }
            secondWord.Append(string.Join("", wordChars));
            wordChars.Clear();

            // If we managed to get the two words, continue
            if (firstWord.Length > 0 && secondWord.Length > 0)
            {
                // Wipe the two words (the + 1 is to indicate whitespace between the two words)
                int initialPosition = ConsoleWrapper.CursorLeft;
                int initialCursorPos = _cursorPos;
                int charsToDelete = firstWord.Length + secondWord.Length + 1;
                    MoveCursorLeft(firstWord.Length);
                for (int i = 0; i < charsToDelete; i++)
                    DeleteChar();

                // Actually swap the two words with each other
                WriteString($"{secondWord} {firstWord}");

                // Set the cursor position to the appropriate values
                ConsoleWrapper.SetCursorPosition(initialPosition, ConsoleWrapper.CursorTop);
                _cursorPos = initialCursorPos;
            }
        }
        #endregion

        #region Auto-completion
        /// <summary>
        /// Initialize auto-complete initially
        /// </summary>
        private void DoAutoComplete()
        {
            if (ReadLine.AutoCompletionEnabled)
            {
                if (IsInAutoCompleteMode)
                {
                    // We're in the middle of auto-completion. Get the next suggestion
                    NextAutoComplete();
                }
                else
                {
                    // If there is no handler installed or if we're in the middle of the line
                    if (_autoCompleteHandler == null || !IsEndOfLine)
                        return;

                    // Get the initial text
                    string text = _text.ToString();

                    // Get the completion start index (get the last index of any of the separators found in the text)
                    _completionStart = text.LastIndexOfAny(_autoCompleteHandler.Separators);
                    _completionStart = _completionStart == -1 ? 0 : _completionStart + 1;

                    // Get the suggestions based on the current text and the completion start index
                    _completions = _autoCompleteHandler.GetSuggestions(text, _completionStart);
                    _completions = _completions?.Length == 0 ? null : _completions;

                    // If we have no completions, bail
                    if (_completions == null)
                        return;

                    // Start the auto completion
                    if (_currentHandler != nameof(InsertCompletions))
                        StartAutoComplete();
                }
            }
            else
            {
                WriteChar();
            }
        }

        /// <summary>
        /// Does the reverse auto complete
        /// </summary>
        private void DoReverseAutoComplete()
        {
            if (ReadLine.AutoCompletionEnabled)
            {
                if (IsInAutoCompleteMode)
                {
                    // We're in the middle of auto-completion. Get the previous suggestion
                    PreviousAutoComplete();
                }
            }
            else
            {
                WriteChar();
            }
        }

        /// <summary>
        /// Starts the auto-completion, showing the first suggestion
        /// </summary>
        private void StartAutoComplete()
        {
            Backspace(_cursorPos - _completionStart);

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
            Backspace(_cursorPos - _completionStart);

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
            Backspace(_cursorPos - _completionStart);

            // Decrement the completion index
            _completionsIndex--;

            // If we got the first suggestion, go to the last one
            if (_completionsIndex == -1)
                _completionsIndex = _completions.Length - 1;

            // Write the suggestion
            WriteString(_completions[_completionsIndex]);
        }

        /// <summary>
        /// Inserts available completions
        /// </summary>
        private void InsertCompletions()
        {
            if (ReadLine.AutoCompletionEnabled)
            {
                // Initialize suggestions
                DoAutoComplete();

                // Dump all the suggestions
                WriteString(string.Join(" ", _completions));

                // Finalize suggestions
                ResetAutoComplete();
            }
        }

        /// <summary>
        /// Resets the auto-completion
        /// </summary>
        private void ResetAutoComplete()
        {
            _completions = null;
            _completionsIndex = 0;
        }
        #endregion

        #region Command history
        /// <summary>
        /// Shows the previous history
        /// </summary>
        private void PrevHistory()
        {
            _updateCurrentLine = false;
            _updateCurrentLineHistory = false;
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
            _updateCurrentLine = false;
            _updateCurrentLineHistory = false;
            if (_historyIndex < _history.Count)
            {
                _historyIndex++;
                if (_historyIndex == _history.Count)
                {
                    _historyIndex = _history.Count;
                    WriteNewString(_currentLine.ToString());
                }
                else
                    WriteNewString(_history[_historyIndex]);
            }
        }

        /// <summary>
        /// Adds last argument to the current input
        /// </summary>
        private void AddLastArgument()
        {
            if (_history.Count > 0)
            {
                string[] lastHistoryArgs = _history[_history.Count - 1].Split(' ');
                if (lastHistoryArgs.Length > 0)
                    WriteString(lastHistoryArgs[lastHistoryArgs.Length - 1]);
            }
        }

        /// <summary>
        /// Gets the first history
        /// </summary>
        private void FirstHistory()
        {
            _updateCurrentLine = false;
            _updateCurrentLineHistory = false;
            if (_history.Count > 0)
            {
                _historyIndex = 0;
                WriteNewString(_history[_historyIndex]);
            }
        }

        /// <summary>
        /// Goes back to the current line
        /// </summary>
        private void GoBackToCurrentLine()
        {
            if (_history.Count > 0)
            {
                _historyIndex = _history.Count;
                WriteNewString(_currentLine.ToString());
            }
        }
        #endregion

        #region Case manipulation
        /// <summary>
        /// Makes the word lowercase
        /// </summary>
        private void LowercaseWord()
        {
            // Skip all whitespaces found
            while (!IsEndOfLine && char.IsWhiteSpace(_text[_cursorPos]))
                MoveCursorRight();

            // Now, lowercase the entire word
            while (!IsEndOfLine && _text[_cursorPos] != ' ')
            {
                char Result = char.ToLower(_text[_cursorPos]);
                DeleteChar();
                WriteChar(Result);
            }
        }

        /// <summary>
        /// Makes the word UPPERCASE
        /// </summary>
        private void UppercaseWord()
        {
            // Skip all whitespaces found
            while (!IsEndOfLine && char.IsWhiteSpace(_text[_cursorPos]))
                MoveCursorRight();

            // Now, UPPERCASE the entire word
            while (!IsEndOfLine && _text[_cursorPos] != ' ')
            {
                char Result = char.ToUpper(_text[_cursorPos]);
                DeleteChar();
                WriteChar(Result);
            }
        }

        /// <summary>
        /// Makes the character UPPERCASE and move to the end of the word
        /// </summary>
        private void UppercaseCharMoveToEndOfWord()
        {
            char Result = char.ToUpper(_text[_cursorPos]);
            DeleteChar();
            WriteChar(Result);
            MoveCursorWordRight();
        }

        /// <summary>
        /// Makes the character lowercase and move to the end of the word
        /// </summary>
        private void LowercaseCharMoveToEndOfWord()
        {
            char Result = char.ToLower(_text[_cursorPos]);
            DeleteChar();
            WriteChar(Result);
            MoveCursorWordRight();
        }
        #endregion

        #region Clipboard manipulation
        /// <summary>
        /// Pastes the content of console clipboard (kill buffer)
        /// </summary>
        private void Yank()
        {
            if (ReadLine.ClipboardEnabled)
            {
                // Write the kill buffer content
                if (!IsKillBufferEmpty)
                    WriteString(_killBuffer.ToString());
            }
            else
            {
                WriteChar();
            }
        }
        #endregion

        #region Current line history
        /// <summary>
        /// Undos the last edit done to the current line
        /// </summary>
        private void Undo()
        {
            if (ReadLine.UndoEnabled)
            {
                _middleOfUndo = true;

                if (_currentLineEditHistory.Count > 1)
                {
                    WriteNewString(_currentLineEditHistory[_currentLineEditHistory.Count - 2]);
                    _currentLineEditHistory.RemoveAt(_currentLineEditHistory.Count - 1);
                }
                else
                {
                    ClearLine();
                    if (_currentLineEditHistory.Count == 1)
                        _currentLineEditHistory.RemoveAt(0);
                }

                _middleOfUndo = false;
            }
        }

        /// <summary>
        /// Undos all the edits done to the current line
        /// </summary>
        private void UndoAll()
        {
            if (ReadLine.UndoEnabled)
            {
                _middleOfUndoAll = true;
                while (_currentLineEditHistory.Count > 0)
                    Undo();
                _middleOfUndoAll = false;
            }
        }
        #endregion

        #region Argument manipulation
        /// <summary>
        /// Sets the argument
        /// </summary>
        /// <param name="arg">Argument digit to add</param>
        private void SetArgument(int arg)
        {
            // Make a string builder to add the digit to the number
            StringBuilder tempArg = new StringBuilder(_argDigit.ToString());
            tempArg.Append(arg);

            // Update digit
            _argDigit = int.Parse(tempArg.ToString());
            UpdatePrompt($"(arg: {_argDigit}) ");
            _middleOfArgInsert = true;
        }

        /// <summary>
        /// Add a minus to the argument number or write the minuses if an argument is already specified
        /// </summary>
        private void MinusArgumentOrWrite()
        {
            if (_middleOfArgInsert)
            {
                WriteChar();
                _middleOfArgInsert = false;
                _argDigit = 0;
            }
            else
            {
                _argDigit = -1;
                UpdatePrompt($"(arg: {_argDigit}) ");
                _middleOfArgInsert = true;
            }
        }
        #endregion

        #region Other logic
        /// <summary>
        /// Updates the current line variable
        /// </summary>
        private void UpdateCurrentLine()
        {
            if (_updateCurrentLine)
            {
                _currentLine.Clear();
                _currentLine.Append(_text.ToString());
                if (_updateCurrentLineHistory && !_middleOfWriteNewString && !_middleOfUndo && !_middleOfUndoAll)
                    _currentLineEditHistory.Add(_currentLine.ToString());
            }
        }

        /// <summary>
        /// Updates the prompt
        /// </summary>
        /// <param name="newPrompt">Prompt to be updated</param>
        /// <param name="rewriteCurrentLine">Whether to rewrite current line</param>
        private void UpdatePrompt(string newPrompt, bool rewriteCurrentLine = true)
        {
            // Get number of newlines
            int newLines = _cachedPrompt.Replace("\r", "").Split(new char[] { '\n' }).Length - 1;
            int lastTop = _prePromptCursorTop == Console.BufferHeight - 1 ? _prePromptCursorTop - newLines : _prePromptCursorTop;

            // Clear current line to destroy current prompt (prompt may have newlines)
            Console.Write($"{Escape}[2K");
            for (int i = Console.CursorTop; i >= lastTop; i--)
            {
                Console.SetCursorPosition(_prePromptCursorLeft, i);
                Console.Write($"{Escape}[2K");
            }

            // Write the new prompt
            ReadLine.WritePrompt.Invoke(newPrompt);
            _cachedPrompt = newPrompt;

            // Re-write current line since we destroyed all the lines
            if (rewriteCurrentLine)
            {
                _cursorPos = 0;
                WriteNewString(CurrentLine);
            }
            else
            {
                _text.Clear();
                _cursorLimit = 0;
                _cursorPos = 0;
            }

        }
        #endregion

        #region Main logic
        /// <summary>
        /// Initializes the new instance of the key handler class
        /// </summary>
        /// <param name="console">Console instance</param>
        /// <param name="history">History of written inputs</param>
        /// <param name="autoCompleteHandler">Auto completion handler</param>
        public KeyHandler(IConsole console, List<string> history, IAutoCompleteHandler autoCompleteHandler)
        {
            // Set the console wrapper
            ConsoleWrapper = console;

            // Initialize history and text
            _history = history ?? new List<string>();
            _currentLineEditHistory = new List<string>();
            _historyIndex = _history.Count;
            _text = new StringBuilder();
            _currentLine = new StringBuilder();
            _killBuffer = new StringBuilder();
            _autoCompleteHandler = autoCompleteHandler;

            // Assign the key actions
            _keyActions = new Dictionary<string, Action>
            {
                // Cursor movement (left and right)
                ["LeftArrow"] =                 MoveCursorLeft,
                ["ControlB"] =                  MoveCursorLeft,
                ["AltB"] =                      MoveCursorWordLeft,
                ["RightArrow"] =                MoveCursorRight,
                ["ControlF"] =                  MoveCursorRight,
                ["AltF"] =                      MoveCursorWordRight,

                // Cursor movement (home and end)
                ["Home"] =                      MoveCursorHome,
                ["ControlA"] =                  MoveCursorHome,
                ["End"] =                       MoveCursorEnd,
                ["ControlE"] =                  MoveCursorEnd,

                // Deletion of one character
                ["Backspace"] =                 Backspace,
                ["ControlH"] =                  Backspace,
                ["Delete"] =                    Delete,
                ["ControlD"] =                  Delete,

                // Deletion of whole line
                ["Escape"] =                    ClearLine,
                ["ControlL"] =                  ClearLine,
                ["ControlU"] =                  ClearLineToLeft,
                ["ControlK"] =                  ClearLineToRight,
                ["ControlW"] =                  ClearLineUntilSpace,
                ["AltBackspace"] =              ClearLineUntilSpace,
                ["AltD"] =                      ClearLineAfterSpace,
                ["AltOem5"] =                   ClearHorizontalSpace,

                // History manipulation
                ["UpArrow"] =                   PrevHistory,
                ["ControlP"] =                  PrevHistory,
                ["DownArrow"] =                 NextHistory,
                ["ControlN"] =                  NextHistory,
                ["AltOemPeriod"] =              AddLastArgument,
                ["Alt, ShiftOemComma"] =        FirstHistory,
                ["Alt, ShiftOemPeriod"] =       GoBackToCurrentLine,

                // Substitution
                ["ControlT"] =                  TransposeChars,
                ["AltT"] =                      TransposeWords,

                // Auto-completion initialization
                ["Tab"] =                       DoAutoComplete,
                ["ControlI"] =                  DoAutoComplete,
                ["ShiftTab"] =                  DoReverseAutoComplete,
                ["Shift, ControlI"] =           DoReverseAutoComplete,
                ["Alt, ShiftD8"] =              InsertCompletions,

                // Case manipulation
                ["AltL"] =                      LowercaseWord,
                ["AltU"] =                      UppercaseWord,
                ["AltV"] =                      LowercaseCharMoveToEndOfWord,
                ["AltC"] =                      UppercaseCharMoveToEndOfWord,

                // Clipboard manipulation
                ["ControlY"] =                  Yank,

                // Insertion
                ["Alt, ShiftD3"] =              InsertComment,
                ["Alt, ShiftD7"] =              InsertHomeDirectory,
                ["AltTab"] =                    WriteChar,

                // Undoing
                ["Shift, ControlOemMinus"] =    Undo,
                ["AltR"] =                      UndoAll,

                // Argument support (0-9, -)
                ["AltD0"] =                     () => SetArgument(0),
                ["AltD1"] =                     () => SetArgument(1),
                ["AltD2"] =                     () => SetArgument(2),
                ["AltD3"] =                     () => SetArgument(3),
                ["AltD4"] =                     () => SetArgument(4),
                ["AltD5"] =                     () => SetArgument(5),
                ["AltD6"] =                     () => SetArgument(6),
                ["AltD7"] =                     () => SetArgument(7),
                ["AltD8"] =                     () => SetArgument(8),
                ["AltD9"] =                     () => SetArgument(9),
                ["AltSubtract"] =                  MinusArgumentOrWrite
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
            if (ReadLine.AutoCompletionEnabled)
            {
                if (IsInAutoCompleteMode && _keyInfo.Key != ConsoleKey.Tab &&
                                            _keyInfo.Key != ConsoleKey.I && (!_keyInfo.Modifiers.HasFlag(ConsoleModifiers.Control) ||
                                                                             !_keyInfo.Modifiers.HasFlag(ConsoleModifiers.Shift)))
                    ResetAutoComplete();
            }

            // Get the key input and assign it to the action defined in the actions list. Otherwise, write the character.
            string KeyInputName = KeyTools.BuildKeyInput(keyInfo);
            _keyActions.TryGetValue(KeyInputName, out Action action);
            action ??= WriteChar;

            // Because SetArgument is getting called from the lambda (we don't want to duplicate code for each number),
            // the current handler is being set to some trash name with "lambda" in it, so we need to replace this gibberish
            // name with the "SetArgument" to make it easy for KeyHandler to detect if we're not setting an argument on the
            // next keypress.
            _currentHandler = action.Method.Name.Contains("<.ctor>b__99_") ? nameof(SetArgument) : action.Method.Name;

            // Invoke it!
            if (_middleOfArgInsert && _currentHandler != nameof(SetArgument))
            {
                UpdatePrompt(_initialPrompt);
                for (int i = 1; i <= _argDigit; i++)
                    action.Invoke();
            }
            else
                action.Invoke();

            // Set the last handler
            _lastHandler = _currentHandler;
            _updateCurrentLine = true;
            _updateCurrentLineHistory = true;

            // If not setting argument, reset flag
            if (_currentHandler != nameof(SetArgument) && _currentHandler != nameof(MinusArgumentOrWrite))
            {
                _middleOfArgInsert = false;
                _argDigit = 0;
            }
        }
        #endregion
    }
}
