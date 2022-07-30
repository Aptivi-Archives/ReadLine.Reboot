/*
 * MIT License
 *
 * Copyright (c) 2017 Toni Solarin-Sodara
 * Copyright (c) 2022 Aptivi
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

namespace ReadLineReboot
{
    /// <summary>
    /// The auto completion handler interface. You must make a base class that implements this interface in order to enable auto completion.
    /// </summary>
    public interface IAutoCompleteHandler
    {
        /// <summary>
        /// Separator characters. If any of these are encountered, the auto-completion will trigger
        /// </summary>
        char[] Separators { get; set; }

        /// <summary>
        /// Gets the suggestions based on current text and current index
        /// </summary>
        /// <param name="text">The current text entered in the console</param>
        /// <param name="index">The index of the terminal cursor where the auto-completion is triggered</param>
        /// <returns></returns>
        string[] GetSuggestions(string text, int index);
    }
}