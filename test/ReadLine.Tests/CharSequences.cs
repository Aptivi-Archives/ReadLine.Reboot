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

using Extensification.CharExts;
using System;
using System.Collections.Generic;

namespace ReadLine.Tests
{
    public static class CharSequences
    {
        // The actual characters used in test
        public const char ExclamationPointChar = '!';
        public const char SpaceChar            = ' ';

        // The control sequence used in ReadLine.Reboot
        public const char CtrlAChar = '\u0001';
        public const char CtrlBChar = '\u0002';
        public const char CtrlDChar = '\u0004';
        public const char CtrlEChar = '\u0005';
        public const char CtrlFChar = '\u0006';
        public const char CtrlHChar = '\u0008';
        public const char CtrlIChar = '\u0009';
        public const char CtrlKChar = '\u000B';
        public const char CtrlLChar = '\u000C';
        public const char CtrlNChar = '\u000E';
        public const char CtrlPChar = '\u0010';
        public const char CtrlTChar = '\u0014';
        public const char CtrlUChar = '\u0015';
        public const char CtrlWChar = '\u0017';
        public const char CtrlYChar = '\u0019';

        // No modifiers
        private const ConsoleModifiers NoModifiers = 0;

        // The special key character
        internal static readonly Dictionary<char, Tuple<ConsoleKey, ConsoleModifiers>> specialKeyCharMap = new()
        {
            // The actual characters used in test
            { ExclamationPointChar, Tuple.Create(ConsoleKey.D0,        NoModifiers) },
            { SpaceChar,            Tuple.Create(ConsoleKey.Spacebar,  NoModifiers) },

            // The control sequence used in ReadLine.Reboot
            { CtrlAChar, Tuple.Create(ConsoleKey.A, ConsoleModifiers.Control) },
            { CtrlBChar, Tuple.Create(ConsoleKey.B, ConsoleModifiers.Control) },
            { CtrlDChar, Tuple.Create(ConsoleKey.D, ConsoleModifiers.Control) },
            { CtrlEChar, Tuple.Create(ConsoleKey.E, ConsoleModifiers.Control) },
            { CtrlFChar, Tuple.Create(ConsoleKey.F, ConsoleModifiers.Control) },
            { CtrlHChar, Tuple.Create(ConsoleKey.H, ConsoleModifiers.Control) },
            { CtrlIChar, Tuple.Create(ConsoleKey.I, ConsoleModifiers.Control) },
            { CtrlKChar, Tuple.Create(ConsoleKey.K, ConsoleModifiers.Control) },
            { CtrlLChar, Tuple.Create(ConsoleKey.L, ConsoleModifiers.Control) },
            { CtrlNChar, Tuple.Create(ConsoleKey.N, ConsoleModifiers.Control) },
            { CtrlPChar, Tuple.Create(ConsoleKey.P, ConsoleModifiers.Control) },
            { CtrlTChar, Tuple.Create(ConsoleKey.T, ConsoleModifiers.Control) },
            { CtrlUChar, Tuple.Create(ConsoleKey.U, ConsoleModifiers.Control) },
            { CtrlWChar, Tuple.Create(ConsoleKey.W, ConsoleModifiers.Control) },
            { CtrlYChar, Tuple.Create(ConsoleKey.Y, ConsoleModifiers.Control) }
        };

        // Normal control characters used in ReadLine.Reboot
        public static readonly ConsoleKeyInfo Backspace =           new('\0', ConsoleKey.Backspace,  false, false, false);
        public static readonly ConsoleKeyInfo Delete =              new('\0', ConsoleKey.Delete,     false, false, false);
        public static readonly ConsoleKeyInfo Home =                new('\0', ConsoleKey.Home,       false, false, false);
        public static readonly ConsoleKeyInfo End =                 new('\0', ConsoleKey.End,        false, false, false);
        public static readonly ConsoleKeyInfo LeftArrow =           new('\0', ConsoleKey.LeftArrow,  false, false, false);
        public static readonly ConsoleKeyInfo RightArrow =          new('\0', ConsoleKey.RightArrow, false, false, false);
        public static readonly ConsoleKeyInfo UpArrow =             new('\0', ConsoleKey.UpArrow,    false, false, false);
        public static readonly ConsoleKeyInfo DownArrow =           new('\0', ConsoleKey.DownArrow,  false, false, false);
        public static readonly ConsoleKeyInfo Tab =                 new('\0', ConsoleKey.Tab,        false, false, false);
        public static readonly ConsoleKeyInfo ShiftTab =            new('\0', ConsoleKey.Tab,        true,  false, false);
        public static readonly ConsoleKeyInfo AltD =                new('d',  ConsoleKey.D,          false, true,  false);
        public static readonly ConsoleKeyInfo AltB =                new('b',  ConsoleKey.B,          false, true,  false);
        public static readonly ConsoleKeyInfo AltF =                new('f',  ConsoleKey.F,          false, true,  false);
        public static readonly ConsoleKeyInfo AltL =                new('l',  ConsoleKey.L,          false, true,  false);
        public static readonly ConsoleKeyInfo AltU =                new('u',  ConsoleKey.U,          false, true,  false);
        public static readonly ConsoleKeyInfo AltC =                new('c',  ConsoleKey.C,          false, true,  false);
        public static readonly ConsoleKeyInfo AltV =                new('v',  ConsoleKey.V,          false, true,  false);
        public static readonly ConsoleKeyInfo AltT =                new('t',  ConsoleKey.T,          false, true,  false);
        public static readonly ConsoleKeyInfo AltOemPeriod =        new('.',  ConsoleKey.OemPeriod,  false, true,  false);
        public static readonly ConsoleKeyInfo AltShiftOemComma =    new('<',  ConsoleKey.OemComma,   true,  true,  false);
        public static readonly ConsoleKeyInfo AltOem5 =             new('\\', ConsoleKey.Oem5,       false, true,  false);
        public static readonly ConsoleKeyInfo AltBackspace =        new('\0', ConsoleKey.Backspace,  false, true,  false);

        // The actual characters used in test
        public static readonly ConsoleKeyInfo ExclamationPoint = ExclamationPointChar.ToConsoleKeyInfo(specialKeyCharMap);
        public static readonly ConsoleKeyInfo Space =            SpaceChar.ToConsoleKeyInfo(specialKeyCharMap);

        // The control sequence used in ReadLine.Reboot
        public static readonly ConsoleKeyInfo CtrlA = CtrlAChar.ToConsoleKeyInfo(specialKeyCharMap);
        public static readonly ConsoleKeyInfo CtrlB = CtrlBChar.ToConsoleKeyInfo(specialKeyCharMap);
        public static readonly ConsoleKeyInfo CtrlD = CtrlDChar.ToConsoleKeyInfo(specialKeyCharMap);
        public static readonly ConsoleKeyInfo CtrlE = CtrlEChar.ToConsoleKeyInfo(specialKeyCharMap);
        public static readonly ConsoleKeyInfo CtrlF = CtrlFChar.ToConsoleKeyInfo(specialKeyCharMap);
        public static readonly ConsoleKeyInfo CtrlH = CtrlHChar.ToConsoleKeyInfo(specialKeyCharMap);
        public static readonly ConsoleKeyInfo CtrlI = CtrlIChar.ToConsoleKeyInfo(specialKeyCharMap);
        public static readonly ConsoleKeyInfo CtrlK = CtrlKChar.ToConsoleKeyInfo(specialKeyCharMap);
        public static readonly ConsoleKeyInfo CtrlL = CtrlLChar.ToConsoleKeyInfo(specialKeyCharMap);
        public static readonly ConsoleKeyInfo CtrlN = CtrlNChar.ToConsoleKeyInfo(specialKeyCharMap);
        public static readonly ConsoleKeyInfo CtrlP = CtrlPChar.ToConsoleKeyInfo(specialKeyCharMap);
        public static readonly ConsoleKeyInfo CtrlT = CtrlTChar.ToConsoleKeyInfo(specialKeyCharMap);
        public static readonly ConsoleKeyInfo CtrlU = CtrlUChar.ToConsoleKeyInfo(specialKeyCharMap);
        public static readonly ConsoleKeyInfo CtrlW = CtrlWChar.ToConsoleKeyInfo(specialKeyCharMap);
        public static readonly ConsoleKeyInfo CtrlY = CtrlYChar.ToConsoleKeyInfo(specialKeyCharMap);
    }
}
