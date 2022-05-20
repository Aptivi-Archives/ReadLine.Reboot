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

namespace ReadLine.Tests
{
    public static class CharExtensions
    {
        // The actual characters used in test
        public const char ExclamationPoint = '!';
        public const char Space = ' ';

        // The control sequence used in ReadLine.Reboot
        public const char CtrlA = '\u0001';
        public const char CtrlB = '\u0002';
        public const char CtrlD = '\u0004';
        public const char CtrlE = '\u0005';
        public const char CtrlF = '\u0006';
        public const char CtrlH = '\u0008';
        public const char CtrlI = '\u0009';
        public const char CtrlK = '\u000B';
        public const char CtrlL = '\u000C';
        public const char CtrlN = '\u000E';
        public const char CtrlP = '\u0010';
        public const char CtrlT = '\u0014';
        public const char CtrlU = '\u0015';
        public const char CtrlW = '\u0017';
        public const char CtrlY = '\u0019';

        // No modifiers
        private const ConsoleModifiers NoModifiers = 0;

        // The special key character
        internal static readonly Dictionary<char, Tuple<ConsoleKey, ConsoleModifiers>> specialKeyCharMap = new()
        {
            // The actual characters used in test
            {ExclamationPoint, Tuple.Create(ConsoleKey.D0, NoModifiers)},
            {Space, Tuple.Create(ConsoleKey.Spacebar,  NoModifiers)},

            // The control sequence used in ReadLine.Reboot
            {CtrlA, Tuple.Create(ConsoleKey.A, ConsoleModifiers.Control)},
            {CtrlB, Tuple.Create(ConsoleKey.B, ConsoleModifiers.Control)},
            {CtrlD, Tuple.Create(ConsoleKey.D, ConsoleModifiers.Control)},
            {CtrlE, Tuple.Create(ConsoleKey.E, ConsoleModifiers.Control)},
            {CtrlF, Tuple.Create(ConsoleKey.F, ConsoleModifiers.Control)},
            {CtrlH, Tuple.Create(ConsoleKey.H, ConsoleModifiers.Control)},
            {CtrlI, Tuple.Create(ConsoleKey.I, ConsoleModifiers.Control)},
            {CtrlK, Tuple.Create(ConsoleKey.K, ConsoleModifiers.Control)},
            {CtrlL, Tuple.Create(ConsoleKey.L, ConsoleModifiers.Control)},
            {CtrlN, Tuple.Create(ConsoleKey.N, ConsoleModifiers.Control)},
            {CtrlP, Tuple.Create(ConsoleKey.P, ConsoleModifiers.Control)},
            {CtrlT, Tuple.Create(ConsoleKey.T, ConsoleModifiers.Control)},
            {CtrlU, Tuple.Create(ConsoleKey.U, ConsoleModifiers.Control)},
            {CtrlW, Tuple.Create(ConsoleKey.W, ConsoleModifiers.Control)},
            {CtrlY, Tuple.Create(ConsoleKey.Y, ConsoleModifiers.Control)}
        };
    }
}
