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

namespace ReadLineReboot.Abstractions
{
    /// <summary>
    /// Console interface
    /// </summary>
    public interface IConsole
    {
        /// <summary>
        /// Cursor left position
        /// </summary>
        int CursorLeft { get; }
        /// <summary>
        /// Cursor top position
        /// </summary>
        int CursorTop { get; }
        /// <summary>
        /// Buffer width
        /// </summary>
        int BufferWidth { get; }
        /// <summary>
        /// Password mode
        /// </summary>
        bool PasswordMode { get; }
        /// <summary>
        /// Password mask character
        /// </summary>
        char PasswordMaskChar { get; }
        /// <summary>
        /// Sets the cursor position
        /// </summary>
        /// <param name="left">Left position</param>
        /// <param name="top">Top position</param>
        void SetCursorPosition(int left, int top);
        /// <summary>
        /// Writes text to the console
        /// </summary>
        /// <param name="value">Text to be printed</param>
        void Write(string value);
    }
}