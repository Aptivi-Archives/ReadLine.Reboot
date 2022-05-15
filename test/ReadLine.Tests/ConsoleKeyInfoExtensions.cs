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

namespace ReadLine.Tests
{
    public static class ConsoleKeyInfoExtensions
    {
        // Normal control characters used in ReadLine.Reboot
        public static readonly ConsoleKeyInfo Backspace = new('\0', ConsoleKey.Backspace, false, false, false);
        public static readonly ConsoleKeyInfo Delete = new('\0', ConsoleKey.Delete, false, false, false);
        public static readonly ConsoleKeyInfo Home = new('\0', ConsoleKey.Home, false, false, false);
        public static readonly ConsoleKeyInfo End = new('\0', ConsoleKey.End, false, false, false);
        public static readonly ConsoleKeyInfo LeftArrow = new('\0', ConsoleKey.LeftArrow, false, false, false);
        public static readonly ConsoleKeyInfo RightArrow = new('\0', ConsoleKey.RightArrow, false, false, false);
        public static readonly ConsoleKeyInfo UpArrow = new('\0', ConsoleKey.UpArrow, false, false, false);
        public static readonly ConsoleKeyInfo DownArrow = new('\0', ConsoleKey.DownArrow, false, false, false);
        public static readonly ConsoleKeyInfo Tab = new('\0', ConsoleKey.Tab, false, false, false);
        public static readonly ConsoleKeyInfo ShiftTab = new('\0', ConsoleKey.Tab, true, false, false);
        public static readonly ConsoleKeyInfo AltD = new('d', ConsoleKey.D, false, true, false);
        public static readonly ConsoleKeyInfo AltB = new('b', ConsoleKey.B, false, true, false);
        public static readonly ConsoleKeyInfo AltF = new('f', ConsoleKey.F, false, true, false);
        public static readonly ConsoleKeyInfo AltL = new('l', ConsoleKey.L, false, true, false);
        public static readonly ConsoleKeyInfo AltU = new('u', ConsoleKey.U, false, true, false);
        public static readonly ConsoleKeyInfo AltC = new('c', ConsoleKey.C, false, true, false);
        public static readonly ConsoleKeyInfo AltV = new('v', ConsoleKey.V, false, true, false);
        public static readonly ConsoleKeyInfo AltT = new('t', ConsoleKey.T, false, true, false);
        public static readonly ConsoleKeyInfo AltOemPeriod = new('.', ConsoleKey.OemPeriod, false, true, false);
        public static readonly ConsoleKeyInfo AltBackspace = new('\0', ConsoleKey.Backspace, false, true, false);

        // The actual characters used in test
        public static readonly ConsoleKeyInfo ExclamationPoint = CharExtensions.ExclamationPoint.ToConsoleKeyInfo();
        public static readonly ConsoleKeyInfo Space = CharExtensions.Space.ToConsoleKeyInfo();

        // The control sequence used in ReadLine.Reboot
        public static readonly ConsoleKeyInfo CtrlA = CharExtensions.CtrlA.ToConsoleKeyInfo();
        public static readonly ConsoleKeyInfo CtrlB = CharExtensions.CtrlB.ToConsoleKeyInfo();
        public static readonly ConsoleKeyInfo CtrlD = CharExtensions.CtrlD.ToConsoleKeyInfo();
        public static readonly ConsoleKeyInfo CtrlE = CharExtensions.CtrlE.ToConsoleKeyInfo();
        public static readonly ConsoleKeyInfo CtrlF = CharExtensions.CtrlF.ToConsoleKeyInfo();
        public static readonly ConsoleKeyInfo CtrlH = CharExtensions.CtrlH.ToConsoleKeyInfo();
        public static readonly ConsoleKeyInfo CtrlI = CharExtensions.CtrlI.ToConsoleKeyInfo();
        public static readonly ConsoleKeyInfo CtrlK = CharExtensions.CtrlK.ToConsoleKeyInfo();
        public static readonly ConsoleKeyInfo CtrlL = CharExtensions.CtrlL.ToConsoleKeyInfo();
        public static readonly ConsoleKeyInfo CtrlN = CharExtensions.CtrlN.ToConsoleKeyInfo();
        public static readonly ConsoleKeyInfo CtrlP = CharExtensions.CtrlP.ToConsoleKeyInfo();
        public static readonly ConsoleKeyInfo CtrlT = CharExtensions.CtrlT.ToConsoleKeyInfo();
        public static readonly ConsoleKeyInfo CtrlU = CharExtensions.CtrlU.ToConsoleKeyInfo();
        public static readonly ConsoleKeyInfo CtrlW = CharExtensions.CtrlW.ToConsoleKeyInfo();
    }
}