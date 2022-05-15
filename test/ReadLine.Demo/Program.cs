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
using ReadLineReboot;

namespace ReadLineDemo
{
    public class Program
    {
        public static void Main()
        {
            // Show the header
            Console.WriteLine("ReadLine Reboot Demo");
            Console.WriteLine("====================");
            Console.WriteLine();

            // Initialize the history
            string[] history = new string[] { "ls -a", "dotnet run", "git init" };
            ReadLine.AddHistory(history);

            // Initialize the auto completion handler
            ReadLine.AutoCompletionHandler = new AutoCompletionHandler();

            // Enter the prompt
            string input = ReadLine.Read("(prompt)> ");
            Console.WriteLine(input);

            // Enter the prompt with no autocomplete
            ReadLine.AutoCompletionEnabled = false;
            input = ReadLine.Read("(promptnoac)> ");
            Console.WriteLine(input);
            ReadLine.AutoCompletionEnabled = true;

            // Enter the prompt with default
            input = ReadLine.Read("(prompt2)> [def] ", "def");
            Console.WriteLine(input);

            // Enter the prompt with custom prompt handler
            ReadLine.WritePrompt = (prompt) => Console.Write($">> {prompt}");
            input = ReadLine.Read("(prompt3)> ");
            Console.WriteLine(input);
            ReadLine.WritePrompt = (prompt) => Console.Write(prompt);

            // Enter the masked prompt
            input = ReadLine.ReadPassword("Enter Password> ");
            Console.WriteLine(input);

            // Enter the masked prompt with password mask
            input = ReadLine.ReadPassword("Enter Password> ", '*');
            Console.WriteLine(input);
        }
    }
}
