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
using System.Collections.Generic;

namespace System
{
    /// <summary>
    /// The initial read line module
    /// </summary>
    public static class ReadLine
    {
        // Variables
        /// <summary>
        /// Whether the history is enabled. Currently false.
        /// </summary>
        public static bool HistoryEnabled { get; set; }

        /// <summary>
        /// The auto completion handler. You need to make a class that implements <see cref="IAutoCompleteHandler"/>
        /// </summary>
        public static IAutoCompleteHandler AutoCompletionHandler { private get; set; }

        private static readonly List<string> _history = new List<string>();

        /// <summary>
        /// Adds a text or an array of texts to the history
        /// </summary>
        /// <param name="texts">The strings to add to the history</param>
        public static void AddHistory(params string[] texts) => _history.AddRange(texts);

        /// <summary>
        /// Gets the current history
        /// </summary>
        public static List<string> GetHistory() => _history;

        /// <summary>
        /// Clears the history
        /// </summary>
        public static void ClearHistory() => _history.Clear();

        /// <summary>
        /// Writes the prompt and reads the input
        /// </summary>
        /// <param name="prompt">The prompt to write</param>
        /// <param name="defaultText">The default text to write if nothing is written</param>
        /// <returns>The written text if anything is input from the user, or the default text if nothing if printed</returns>
        public static string Read(string prompt = "", string defaultText = "")
        {
            // Prepare the prompt
            Console.Write(prompt);
            KeyHandler keyHandler = new KeyHandler(new ConsoleWrapper(), _history, AutoCompletionHandler);

            // Get the written text
            string text = GetText(keyHandler);

            // Add the text to the history if the text is written
            if (string.IsNullOrWhiteSpace(text) && !string.IsNullOrWhiteSpace(defaultText))
            {
                // User didn't input any text. Therefore, set the text to the default value
                text = defaultText;
            }
            else
            {
                // If the history is enabled, add the text to the history
                if (HistoryEnabled)
                    AddHistory(text);
            }

            return text;
        }

        /// <summary>
        /// Writes the prompt and reads the input while masking the written input
        /// </summary>
        /// <param name="prompt">The prompt to write</param>
        /// <returns>The written text</returns>
        public static string ReadPassword(string prompt = "")
        {
            // Prepare the prompt
            Console.Write(prompt);
            KeyHandler keyHandler = new KeyHandler(new ConsoleWrapper() { PasswordMode = true }, null, null);

            // Get the written text
            return GetText(keyHandler);
        }

        private static string GetText(KeyHandler keyHandler)
        {
            // Get the key
            ConsoleKeyInfo keyInfo = Console.ReadKey(true);

            // Stop handling keys if Enter is pressed
            while (keyInfo.Key != ConsoleKey.Enter)
            {
                // Handle the key as appropriate
                keyHandler.Handle(keyInfo);
                keyInfo = Console.ReadKey(true);
            }

            // Write a new line and get the text
            Console.WriteLine();
            return keyHandler.Text;
        }
    }
}
