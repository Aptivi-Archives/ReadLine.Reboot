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

using ReadLineReboot.Abstractions;

namespace ReadLine.Tests.Abstractions
{
    internal class DumbConsole : IConsole
    {
        private int _cursorLeft;
        private int _cursorTop;
        private readonly int _bufferWidth;

        public int CursorLeft => _cursorLeft;

        public int CursorTop => _cursorTop;

        public int BufferWidth => _bufferWidth;

        public bool PasswordMode { get; set; } = false;

        public char PasswordMaskChar { get; set; } = default;

        public DumbConsole()
        {
            _cursorLeft = 0;
            _cursorTop = 0;
            _bufferWidth = 100;
        }

        public void SetCursorPosition(int left, int top)
        {
            if (!PasswordMode || PasswordMaskChar != default)
            {
                _cursorLeft = left;
                _cursorTop = top;
            }
        }

        public void Write(string value)
        {
            _cursorLeft += value.Length;
        }
    }
}