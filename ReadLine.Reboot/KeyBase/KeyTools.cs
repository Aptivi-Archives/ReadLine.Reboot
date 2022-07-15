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

using System;

namespace ReadLineReboot
{
    internal static class KeyTools
    {
        private const char CtrlSMinusChar = '\u001f';

        /// <summary>
        /// Corrects the key enumerator on some systems
        /// </summary>
        /// <param name="keyInfo">Console key information usually fetched from keyboard</param>
        /// <param name="initialKey">The key name</param>
        /// <param name="initialModifiers">The modifiers pressed at the time of check</param>
        private static void CorrectKeyChar(ConsoleKeyInfo keyInfo, out string initialKey, out ConsoleModifiers initialModifiers)
        {
            initialKey = keyInfo.Key.ToString();
            initialModifiers = keyInfo.Modifiers;

            // Correct the character if Key is 0
            if (keyInfo.Key == 0)
            {
                // Get the affected key from the key character
                switch (keyInfo.KeyChar)
                {
                    // Add only the affected keys we need to use in _keyActions.
                    case '.':
                    case '>':
                        initialKey = "OemPeriod";
                        if (keyInfo.KeyChar == '>')
                            initialModifiers |= ConsoleModifiers.Shift;
                        break;
                    case ',':
                    case '<':
                        initialKey = "OemComma";
                        if (keyInfo.KeyChar == '<')
                            initialModifiers |= ConsoleModifiers.Shift;
                        break;
                    case '_':
                        initialKey = "OemMinus";
                        initialModifiers |= ConsoleModifiers.Shift;
                        break;
                    case CtrlSMinusChar:
                        initialKey = "OemMinus";
                        initialModifiers |= ConsoleModifiers.Shift;
                        initialModifiers |= ConsoleModifiers.Control;
                        break;
                    case '\\':
                        initialKey = "Oem5";
                        break;
                    case '#':
                        initialKey = "D3";
                        initialModifiers |= ConsoleModifiers.Shift;
                        break;
                    case '&':
                        initialKey = "D7";
                        initialModifiers |= ConsoleModifiers.Shift;
                        break;
                    case '*':
                        initialKey = "D8";
                        initialModifiers |= ConsoleModifiers.Shift;
                        break;
                    default:
                        break;
                }
            }
            else
            {
                // Special cases for some characters
                switch (keyInfo.KeyChar)
                {
                    // Rename the below keys
                    case '-':
                        initialKey = "Subtract";
                        break;
                    default:
                        break;
                }
            }
        }

        /// <summary>
        /// Builds the key input string
        /// </summary>
        /// <returns>The key (for ex. B), or the pressed modifier and the key (for ex. ControlB)</returns>
        internal static string BuildKeyInput(ConsoleKeyInfo keyInfo)
        {
            // On Mono Linux, some of the characters (usually Oem*) is actually "0" according to _keyInfo.Key, screwing the shortcut up and causing
            // it to not work as defined in the below _keyActions, so give such systems special treatment so they work equally to Windows.
            CorrectKeyChar(keyInfo, out string initialKey, out ConsoleModifiers initialModifiers);

            // Get the key input name
            string inputName = (!initialModifiers.HasFlag(ConsoleModifiers.Control) && !initialModifiers.HasFlag(ConsoleModifiers.Alt) && !initialModifiers.HasFlag(ConsoleModifiers.Shift)) ?
                                 initialKey :
                                 initialModifiers.ToString() + initialKey;
            return inputName;
        }
    }
}
