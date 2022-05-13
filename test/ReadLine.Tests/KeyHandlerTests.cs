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

using Internal.ReadLine;
using Internal.ReadLine.Abstractions;
using ReadLine.Tests.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;
using static ReadLine.Tests.ConsoleKeyInfoExtensions;

namespace ReadLine.Tests
{
    public class KeyHandlerTests
    {
        // Variables
        private KeyHandler _keyHandler;
        private readonly List<string> _history;
        private readonly AutoCompletionHandler _autoCompleteHandler;
        private readonly string[] _completions;
        private readonly IConsole _console;

        /// <summary>
        /// Initialize the variables
        /// </summary>
        public KeyHandlerTests()
        {
            // Initialize the auto completion
            _autoCompleteHandler = new AutoCompletionHandler();
            _completions = _autoCompleteHandler.GetSuggestions("", 0);

            // Initialize the history
            _history = new List<string>(new string[] { "dotnet run", "git init", "clear" });

            // Initialize the key handler
            _console = new DumbConsole();
            _keyHandler = new KeyHandler(_console, _history, null);

            // Initial writing
            "Hello".Select(c => c.ToConsoleKeyInfo())
                   .ToList()
                   .ForEach(_keyHandler.Handle);
        }

        /// <summary>
        /// Tests writing the characters
        /// </summary>
        [Fact]
        public void TestWriteChar()
        {
            // Ensure that we have initialized this class
            Assert.Equal("Hello", _keyHandler.Text);
            
            // Write this
            " World".Select(c => c.ToConsoleKeyInfo())
                    .ToList()
                    .ForEach(_keyHandler.Handle);
            
            // Confirm that everything works
            Assert.Equal("Hello World", _keyHandler.Text);
        }

        /// <summary>
        /// Tests deleting a character
        /// </summary>
        [Fact]
        public void TestBackspace()
        {
            // Simulate the user pressing the BACKSPACE key
            _keyHandler.Handle(Backspace);

            // Ensure that we've erased the last character
            Assert.Equal("Hell", _keyHandler.Text);
        }

        /// <summary>
        /// Tests deleting a character
        /// </summary>
        [Fact]
        public void TestBackspaceWithControlH()
        {
            // Simulate the user pressing the CTRL + H key
            _keyHandler.Handle(CtrlH);

            // Ensure that we've erased the last character
            Assert.Equal("Hell", _keyHandler.Text);
        }

        /// <summary>
        /// Tests deleting a character
        /// </summary>
        [Fact]
        public void TestDelete()
        {
            // Simulate the user pressing the LEFT ARROW + DELETE keys
            new List<ConsoleKeyInfo>() { LeftArrow, Delete }
                .ForEach(_keyHandler.Handle);

            // Ensure that we've erased the last character
            Assert.Equal("Hell", _keyHandler.Text);
        }

        /// <summary>
        /// Tests deleting a character
        /// </summary>
        [Fact]
        public void TestDeleteWithControlD()
        {
            // Simulate the user pressing the LEFT ARROW (4x) + CTRL + D keys
            Enumerable.Repeat(LeftArrow, 4)
                      .Append(CtrlD)
                      .ToList()
                      .ForEach(_keyHandler.Handle);

            // Ensure that we've erased the second character
            Assert.Equal("Hllo", _keyHandler.Text);
        }

        /// <summary>
        /// Tries to delete the character from the end of the line
        /// </summary>
        [Fact]
        public void TestDeleteAtTheEndOfLine()
        {
            // Simulate the user pressing the DELETE key
            _keyHandler.Handle(Delete);

            // Nothing should be deleted. Or, we've got a serious bug
            Assert.Equal("Hello", _keyHandler.Text);
        }

        /// <summary>
        /// Tests clearing the line
        /// </summary>
        [Fact]
        public void TestClearLineWithControlL()
        {
            // Simulate the user pressing the CTRL + L key
            _keyHandler.Handle(CtrlL);

            // Nothing should be there
            Assert.Equal(string.Empty, _keyHandler.Text);
        }

        /// <summary>
        /// Tests clearing the line to the left
        /// </summary>
        [Fact]
        public void TestClearLineToLeftWithControlU()
        {
            // Simulate the user pressing the LEFT ARROW + CTRL + U keys
            new List<ConsoleKeyInfo>() { LeftArrow, CtrlU }
                .ForEach(_keyHandler.Handle);

            // Ensure that we wiped all the characters from the position before the "o" character
            Assert.Equal("o", _keyHandler.Text);

            // Simulate the user pressing the END + CTRL + U keys
            new List<ConsoleKeyInfo>() { End, CtrlU }
                .ForEach(_keyHandler.Handle);

            // Ensure that nothing is there
            Assert.Equal(string.Empty, _keyHandler.Text);
        }

        /// <summary>
        /// Tests clearing the line to the current position
        /// </summary>
        [Fact]
        public void TestClearLineToRightWithControlK()
        {
            // Simulate the user pressing the LEFT ARROW and CTRL + K keys
            new List<ConsoleKeyInfo>() { LeftArrow, CtrlK }
                .ForEach(_keyHandler.Handle);

            // Ensure that we've removed all the characters to the current position
            Assert.Equal("Hell", _keyHandler.Text);

            // Simulate the user pressing the HOME and CTRL + K keys
            new List<ConsoleKeyInfo>() { Home, CtrlK }
                .ForEach(_keyHandler.Handle);

            // Ensure that nothing is there
            Assert.Equal(string.Empty, _keyHandler.Text);
        }

        /// <summary>
        /// Tests clearing the line until space is spotted
        /// </summary>
        [Fact]
        public void TestClearLineUntilSpaceWithControlW()
        {
            // Simulate the user pressing the CTRL + W key while writing the " World" string
            " World".Select(c => c.ToConsoleKeyInfo())
                    .Append(CtrlW)
                    .ToList()
                    .ForEach(_keyHandler.Handle);

            // Ensure that we've erased everything until the space is spotted
            Assert.Equal("Hello ", _keyHandler.Text);

            // Simulate the user pressing the BACKSPACE and CTRL + W keys
            new List<ConsoleKeyInfo>() { Backspace, CtrlW }
                .ForEach(_keyHandler.Handle);

            // Ensure that nothing is there
            Assert.Equal(string.Empty, _keyHandler.Text);
        }

        /// <summary>
        /// Tries to swap the two characters
        /// </summary>
        [Fact]
        public void TestTransposeWithControlT()
        {
            // Initial console cursor position
            var initialCursorCol = _console.CursorLeft;

            // Simulate the user pressing the CTRL + T keys
            _keyHandler.Handle(CtrlT);

            // Ensure that the O and the L at the end of the string are swapped
            Assert.Equal("Helol", _keyHandler.Text);
            Assert.Equal(initialCursorCol, _console.CursorLeft);
        }

        /// <summary>
        /// Tries to swap the two characters
        /// </summary>
        [Fact]
        public void TestTransposeWithMovement()
        {
            // Initial console cursor position
            var initialCursorCol = _console.CursorLeft;

            // Simulate the user pressing the LEFT ARROW and CTRL + T keys
            new List<ConsoleKeyInfo>() { LeftArrow, CtrlT }
                .ForEach(_keyHandler.Handle);

            // Ensure that the O and the L at the end of the string are swapped
            Assert.Equal("Helol", _keyHandler.Text);
            Assert.Equal(initialCursorCol, _console.CursorLeft);
        }

        /// <summary>
        /// Tries to swap the two characters in the middle
        /// </summary>
        [Fact]
        public void TestTransposeInMiddle()
        {
            // Simulate the user triple-pressing the LEFT ARROW key
            Enumerable
                .Repeat(LeftArrow, 3)
                .ToList()
                .ForEach(_keyHandler.Handle);

            // Initial console cursor position
            var initialCursorCol = _console.CursorLeft;

            // Simulate the user pressing the CTRL + T keys
            _keyHandler.Handle(CtrlT);

            // Ensure that the O and the L at the middle of the string are swapped
            Assert.Equal("Hlelo", _keyHandler.Text);
            Assert.Equal(initialCursorCol + 1, _console.CursorLeft);
        }

        /// <summary>
        /// Tries to swap the two characters in the very beginning. This should do nothing to the string
        /// </summary>
        [Fact]
        public void TestTransposeAtVeryBeginning()
        {
            // Simulate the user pressing the CTRL + A keys
            _keyHandler.Handle(CtrlA);

            // Initial console cursor position
            var initialCursorCol = _console.CursorLeft;

            // Simulate the user pressing the CTRL + T keys
            _keyHandler.Handle(CtrlT);

            // Ensure that nothing is swapped
            Assert.Equal("Hello", _keyHandler.Text);
            Assert.Equal(initialCursorCol, _console.CursorLeft);
        }

        /// <summary>
        /// Tries to go to the beginning of the line and write a character
        /// </summary>
        [Fact]
        public void TestHome()
        {
            // Simulate the user pressing the HOME and S keys
            new List<ConsoleKeyInfo>() { Home, 'S'.ToConsoleKeyInfo() }
                .ForEach(_keyHandler.Handle);

            // Ensure that the S is at the beginning
            Assert.Equal("SHello", _keyHandler.Text);
        }

        /// <summary>
        /// Tries to go to the beginning of the line and write a character
        /// </summary>
        [Fact]
        public void TestHomeWithControlA()
        {
            // Simulate the user pressing the CTRL + A and S keys
            new List<ConsoleKeyInfo>() { CtrlA, 'S'.ToConsoleKeyInfo() }
                .ForEach(_keyHandler.Handle);

            // Ensure that the S is at the beginning
            Assert.Equal("SHello", _keyHandler.Text);
        }

        /// <summary>
        /// Tries to go to the end of the line and writing a character
        /// </summary>
        [Fact]
        public void TestEnd()
        {
            // Simulate the user pressing the HOME, END, and ! keys
            new List<ConsoleKeyInfo>() { Home, End, ExclamationPoint }
                .ForEach(_keyHandler.Handle);

            // Ensure that the exclamation mark is there
            Assert.Equal("Hello!", _keyHandler.Text);
        }

        /// <summary>
        /// Tries to go to the end of the line and writing a character
        /// </summary>
        [Fact]
        public void TestEndWithControlE()
        {
            // Simulate the user pressing the CTRL + A, CTRL + E, and ! keys
            new List<ConsoleKeyInfo>() { CtrlA, CtrlE, ExclamationPoint }
                .ForEach(_keyHandler.Handle);

            // Ensure that the exclamation mark is there
            Assert.Equal("Hello!", _keyHandler.Text);
        }

        /// <summary>
        /// Tests going to the left
        /// </summary>
        [Fact]
        public void TestLeftArrow()
        {
            // Simulate the user pressing the LEFT ARROW key while writing the " N" string
            " N".Select(c => c.ToConsoleKeyInfo())
                .Prepend(LeftArrow)
                .ToList()
                .ForEach(_keyHandler.Handle);

            // Ensure that we've put the " N" before the "o"
            Assert.Equal("Hell No", _keyHandler.Text);
        }

        /// <summary>
        /// Tests going to the left
        /// </summary>
        [Fact]
        public void TestLeftArrowWithControlB()
        {
            // Simulate the user pressing the CTRL + B key while writing the " N" string
            " N".Select(c => c.ToConsoleKeyInfo())
                .Prepend(CtrlB)
                .ToList()
                .ForEach(_keyHandler.Handle);

            // Ensure that we've put the " N" before the "o"
            Assert.Equal("Hell No", _keyHandler.Text);
        }

        /// <summary>
        /// Tests going to the right
        /// </summary>
        [Fact]
        public void TestRightArrow()
        {
            // Simulate the user pressing the LEFT ARROW, RIGHT ARROW, and ! key
            new List<ConsoleKeyInfo>() { LeftArrow, RightArrow, ExclamationPoint }
                .ForEach(_keyHandler.Handle);

            // Ensure that the exclamation mark is there
            Assert.Equal("Hello!", _keyHandler.Text);
        }

        /// <summary>
        /// Tests going to the right
        /// </summary>
        [Fact]
        public void TestRightArrowWithControlF()
        {
            // Simulate the user pressing the LEFT ARROW, CTRL + F, and ! key
            new List<ConsoleKeyInfo>() { LeftArrow, CtrlF, ExclamationPoint }
                .ForEach(_keyHandler.Handle);

            // Ensure that the exclamation mark is there
            Assert.Equal("Hello!", _keyHandler.Text);
        }

        /// <summary>
        /// Tests going up to reveal the previous history
        /// </summary>
        [Fact]
        public void TestPreviousHistoryWithUpArrow()
        {
            // Ensure that we got the right history once UP ARROW is simulated
            _history.AsEnumerable().Reverse().ToList().ForEach((history) => {
                _keyHandler.Handle(UpArrow);
                Assert.Equal(history, _keyHandler.Text);
            });
        }

        /// <summary>
        /// Tests going up to reveal the previous history
        /// </summary>
        [Fact]
        public void TestPreviousHistoryWithControlP()
        {
            // Ensure that we got the right history once CTRL + P is simulated
            _history.AsEnumerable().Reverse().ToList().ForEach((history) => {
                _keyHandler.Handle(CtrlP);
                Assert.Equal(history, _keyHandler.Text);
            });
        }

        /// <summary>
        /// Tests going down to reveal the next history
        /// </summary>
        [Fact]
        public void TestNextHistoryWithDownArrow()
        {
            // Simulate the user pressing the UP ARROW key the number of times as the count of history entries
            Enumerable.Repeat(UpArrow, _history.Count)
                      .ToList()
                      .ForEach(_keyHandler.Handle);

            // Ensure that we got the right history once DOWN ARROW is simulated
            _history.ForEach(history => {
                Assert.Equal(history, _keyHandler.Text);
                _keyHandler.Handle(DownArrow);
            });
        }

        /// <summary>
        /// Tests going down to reveal the next history
        /// </summary>
        [Fact]
        public void TestNextHistoryWithControlN()
        {
            // Simulate the user pressing the UP ARROW key the number of times as the count of history entries
            Enumerable.Repeat(UpArrow, _history.Count)
                      .ToList()
                      .ForEach(_keyHandler.Handle);

            // Ensure that we got the right history once CTRL + N is simulated
            _history.ForEach(history => {
                Assert.Equal(history, _keyHandler.Text);
                _keyHandler.Handle(CtrlN);
            });
        }

        /// <summary>
        /// Tests moving cursor and going to previous history
        /// </summary>
        [Fact]
        public void MoveCursorThenPreviousHistory()
        {
            // Simulate the user pressing the LEFT ARROW and UP ARROW key
            new List<ConsoleKeyInfo>() { LeftArrow, UpArrow }
                .ForEach(_keyHandler.Handle);

            Assert.Equal("clear", _keyHandler.Text);
        }

        /// <summary>
        /// Tests auto completion
        /// </summary>
        [Fact]
        public void TestTab()
        {
            // Simulate the user pressing the TAB key
            _keyHandler.Handle(Tab);

            // Nothing happens when no auto complete handler is set
            Assert.Equal("Hello", _keyHandler.Text);

            // Let's set one up
            _keyHandler = new KeyHandler(new DumbConsole(), _history, _autoCompleteHandler);

            // Write this
            "Hi ".Select(c => c.ToConsoleKeyInfo())
                 .ToList()
                 .ForEach(_keyHandler.Handle);

            // Simulate TAB to ensure that auto completion works
            _completions.ToList().ForEach(completion => {
                _keyHandler.Handle(Tab);
                Assert.Equal($"Hi {completion}", _keyHandler.Text);
            });
        }

        /// <summary>
        /// Tests reverse auto completion
        /// </summary>
        [Fact]
        public void TestBackwardsTab()
        {
            // Simulate the user pressing the TAB key
            _keyHandler.Handle(Tab);

            // Nothing happens when no auto complete handler is set
            Assert.Equal("Hello", _keyHandler.Text);

            // Let's set one up
            _keyHandler = new KeyHandler(new DumbConsole(), _history, _autoCompleteHandler);

            // Write this
            "Hi ".Select(c => c.ToConsoleKeyInfo())
                 .ToList()
                 .ForEach(_keyHandler.Handle);

            // Bring up the first suggestion
            _keyHandler.Handle(Tab);

            // Simulate SHIFT + TAB to ensure that auto completion works in reverse
            _completions.Reverse().ToList().ForEach(completion => {
                _keyHandler.Handle(ShiftTab);
                Assert.Equal($"Hi {completion}", _keyHandler.Text);
            });
        }
    }
}
