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
using System.Collections.Generic;

namespace ReadLineReboot
{
    internal class KeyBindings
    {
        internal static Dictionary<string, Action> _baseKeyBindings;
        internal static Dictionary<string, Action> _customKeyBindings = new Dictionary<string, Action>();

        internal static void InitializeBindings()
        {
            _baseKeyBindings = new Dictionary<string, Action>()
            {
                // Cursor movement (left and right)
                ["LeftArrow"]               = ReadLine._keyHandler.MoveCursorLeft,
                ["ControlB"]                = ReadLine._keyHandler.MoveCursorLeft,
                ["AltB"]                    = ReadLine._keyHandler.MoveCursorWordLeft,
                ["RightArrow"]              = ReadLine._keyHandler.MoveCursorRight,
                ["ControlF"]                = ReadLine._keyHandler.MoveCursorRight,
                ["AltF"]                    = ReadLine._keyHandler.MoveCursorWordRight,

                // Cursor movement (home and end)
                ["Home"]                    = ReadLine._keyHandler.MoveCursorHome,
                ["ControlA"]                = ReadLine._keyHandler.MoveCursorHome,
                ["End"]                     = ReadLine._keyHandler.MoveCursorEnd,
                ["ControlE"]                = ReadLine._keyHandler.MoveCursorEnd,

                // Deletion of one character
                ["Backspace"]               = ReadLine._keyHandler.Backspace,
                ["ControlH"]                = ReadLine._keyHandler.Backspace,
                ["Delete"]                  = ReadLine._keyHandler.Delete,
                ["ControlD"]                = ReadLine._keyHandler.Delete,

                // Deletion of whole line
                ["Escape"]                  = ReadLine._keyHandler.ClearLine,
                ["ControlL"]                = ReadLine._keyHandler.ClearScreenAndRewrite,
                ["ControlU"]                = ReadLine._keyHandler.ClearLineToLeft,
                ["ControlK"]                = ReadLine._keyHandler.ClearLineToRight,
                ["ControlW"]                = ReadLine._keyHandler.ClearLineUntilSpace,
                ["AltBackspace"]            = ReadLine._keyHandler.ClearLineUntilSpace,
                ["AltD"]                    = ReadLine._keyHandler.ClearLineAfterSpace,
                ["AltOem5"]                 = ReadLine._keyHandler.ClearHorizontalSpace,

                // History manipulation
                ["UpArrow"]                 = ReadLine._keyHandler.PrevHistory,
                ["ControlP"]                = ReadLine._keyHandler.PrevHistory,
                ["DownArrow"]               = ReadLine._keyHandler.NextHistory,
                ["ControlN"]                = ReadLine._keyHandler.NextHistory,
                ["AltOemPeriod"]            = ReadLine._keyHandler.AddLastArgument,
                ["Alt, ShiftOemComma"]      = ReadLine._keyHandler.FirstHistory,
                ["Alt, ShiftOemPeriod"]     = ReadLine._keyHandler.GoBackToCurrentLine,

                // Substitution
                ["ControlT"]                = ReadLine._keyHandler.TransposeChars,
                ["AltT"]                    = ReadLine._keyHandler.TransposeWords,

                // Auto-completion initialization
                ["Tab"]                     = ReadLine._keyHandler.DoAutoComplete,
                ["ControlI"]                = ReadLine._keyHandler.DoAutoComplete,
                ["ShiftTab"]                = ReadLine._keyHandler.DoReverseAutoComplete,
                ["Shift, ControlI"]          = ReadLine._keyHandler.DoReverseAutoComplete,
                ["Alt, ShiftD8"]            = ReadLine._keyHandler.InsertCompletions,

                // Case manipulation
                ["AltL"]                    = ReadLine._keyHandler.LowercaseWord,
                ["AltU"]                    = ReadLine._keyHandler.UppercaseWord,
                ["AltV"]                    = ReadLine._keyHandler.LowercaseCharMoveToEndOfWord,
                ["AltC"]                    = ReadLine._keyHandler.UppercaseCharMoveToEndOfWord,

                // Clipboard manipulation
                ["ControlY"]                = ReadLine._keyHandler.Yank,

                // Insertion
                ["Alt, ShiftD3"]            = ReadLine._keyHandler.InsertComment,
                ["Alt, ShiftD7"]            = ReadLine._keyHandler.InsertHomeDirectory,
                ["AltTab"]                  = ReadLine._keyHandler.WriteChar,

                // Undoing
                ["Shift, ControlOemMinus"]  = ReadLine._keyHandler.Undo,
                ["AltR"]                    = ReadLine._keyHandler.UndoAll,

                // Argument support (0-9, -)
                ["AltD0"]                   = () => ReadLine._keyHandler.SetArgument(0),
                ["AltD1"]                   = () => ReadLine._keyHandler.SetArgument(1),
                ["AltD2"]                   = () => ReadLine._keyHandler.SetArgument(2),
                ["AltD3"]                   = () => ReadLine._keyHandler.SetArgument(3),
                ["AltD4"]                   = () => ReadLine._keyHandler.SetArgument(4),
                ["AltD5"]                   = () => ReadLine._keyHandler.SetArgument(5),
                ["AltD6"]                   = () => ReadLine._keyHandler.SetArgument(6),
                ["AltD7"]                   = () => ReadLine._keyHandler.SetArgument(7),
                ["AltD8"]                   = () => ReadLine._keyHandler.SetArgument(8),
                ["AltD9"]                   = () => ReadLine._keyHandler.SetArgument(9),
                ["AltSubtract"]             = ReadLine._keyHandler.MinusArgumentOrWrite
                };
        }
    }
}
