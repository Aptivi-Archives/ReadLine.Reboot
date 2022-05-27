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
using ReadLineReboot;
using ReadLineDemo.Demonstration.Data;
using ReadLineDemo.Demonstration.Fixtures;

namespace ReadLineDemo
{
    public class EntryPoint
    {
        // All fixtures here
        private static readonly Dictionary<string, BaseFixture> fixtures = new()
        { 
            { "NormalPrompt",               new NormalPrompt() },
            { "NormalPromptHistoryEnabled", new NormalPromptHistoryEnabled() } ,
            { "NormalPromptNoAutoComplete", new NormalPromptNoAutoComplete() } ,
            { "NormalPromptDefault",        new NormalPromptDefault() } ,
            { "NormalPromptCustomPrompt",   new NormalPromptCustomPrompt() } ,
            { "NormalPromptNoYank",         new NormalPromptNoYank() } ,
            { "NormalPromptCtrlCAsInput",   new NormalPromptCtrlCAsInput() } ,
            { "MaskedPrompt",               new MaskedPrompt() } ,
            { "MaskedPromptCustomMask",     new MaskedPromptCustomMask() } ,
            { "CharDebug",                  new CharDebug() } 
        };

        public static void Main(string[] args)
        {
            // Show the header
            Console.WriteLine("ReadLine Reboot Demo");
            Console.WriteLine("====================\n");
            Console.WriteLine("Run this program with \"help\" argument to list fixtures. Write \"exit\" to exit program.\n");

            // Initialize the history
            string[] history = new string[] { "ls -a", "dotnet run", "git init" };
            ReadLine.AddHistory(history);

            // Initialize the auto completion handler
            ReadLine.AutoCompletionHandler = new AutoCompletionHandler();

            // Check to see if there are arguments
            string currentFixture = "NormalPrompt";
            if (args.Length > 0)
            {
                if (args[0] == "help" || !fixtures.ContainsKey(args[0]))
                {
                    Console.WriteLine("Available fixtures:");
                    Console.WriteLine("  - " + string.Join(", ", fixtures.Keys));
                    return;
                }
                else
                {
                    currentFixture = args[0];
                }
            }
            Console.WriteLine($"Current fixture: {currentFixture}");

            // Execute based on fixture
            fixtures[currentFixture].RunFixture();
        }
    }
}
