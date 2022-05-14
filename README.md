# ReadLine.Reboot

[![Build status](https://ci.appveyor.com/api/projects/status/twc6ovqb6cc8s184?svg=true)](https://ci.appveyor.com/project/EoflaOE/readline-reboot)

ReadLine.Reboot is a reboot of Toni Solarin-Sodara's original ReadLine that is discontinued. It's a [GNU Readline](https://en.wikipedia.org/wiki/GNU_Readline) like library built in pure C#. It can serve as a drop in replacement for the inbuilt `Console.ReadLine()` and brings along with it some of the terminal goodness you get from Unix shells, like command history navigation and tab auto completion.

It is cross platform and runs anywhere .NET is supported, targeting `netstandard2.1` means that it can be used with the modern .NET and the classic .NET Framework.

## Shortcut Guide

| Shortcut                       | Comment                           |
|:-------------------------------|:----------------------------------|
| `Ctrl`+`A` / `HOME`            | Beginning of line                 |
| `Ctrl`+`B` / `←`               | Backward one character            |
| `Ctrl`+`C`                     | Send EOF                          |
| `Ctrl`+`E` / `END`             | End of line                       |
| `Ctrl`+`F` / `→`               | Forward one character             |
| `Ctrl`+`H` / `Backspace`       | Delete previous character         |
| `Tab`                          | Command line completion           |
| `Shift`+`Tab`                  | Backwards command line completion |
| `Ctrl`+`J` / `Enter`           | Line feed                         |
| `Ctrl`+`K`                     | Cut text to the end of line       |
| `Ctrl`+`L` / `Esc`             | Clear line                        |
| `Ctrl`+`M`                     | Same as Enter key                 |
| `Ctrl`+`N` / `↓`               | Forward in history                |
| `Ctrl`+`P` / `↑`               | Backward in history               |
| `Ctrl`+`U`                     | Cut text to the start of line     |
| `Ctrl`+`W`                     | Cut previous word                 |
| `Alt`+`D`                      | Cut next word                     |
| `Backspace`                    | Delete previous character         |
| `Ctrl` + `D` / `Delete`        | Delete succeeding character       |

## Installation

Available on [NuGet](https://www.nuget.org/packages/ReadLine.Reboot/)

Visual Studio:

```powershell
PM> Install-Package ReadLine.Reboot
```

.NET Core CLI:

```bash
dotnet add package ReadLine.Reboot
```

## Usage

This section shows you how to use this library to read the lines and manage history. Just add this to the top of the source file you want to use ReadLine.Reboot on:

```csharp
using ReadLineReboot;
```

### Input

_Note: The `(prompt>)` is  optional_

#### Read input

```csharp
string input = ReadLine.Read("(prompt)> ");
```

#### Read password

```csharp
string password = ReadLine.ReadPassword("(prompt)> ");
```

#### Read password with password mask

```csharp
string password = ReadLine.ReadPassword("(prompt)> ", '*');
```

### History management

_Note: History information is persisted for an entire application session. Also, calls to `ReadLine.Read()` automatically adds the console input to history_

```csharp
// Get command history
ReadLine.GetHistory();

// Add command to history
ReadLine.AddHistory("dotnet run");

// Clear history
ReadLine.ClearHistory();

// Set history
List<string> newHistory = new() { "apt update", "apt dist-upgrade" };
ReadLine.SetHistory(newHistory);

// Disable history (default)
ReadLine.HistoryEnabled = false;

// Enable history
ReadLine.HistoryEnabled = true;
```

### Auto-Completion

_Note: If no "AutoCompletionHandler" is set, tab autocompletion will be disabled_

```csharp
class AutoCompletionHandler : IAutoCompleteHandler
{
    // characters to start completion from
    public char[] Separators { get; set; } = new char[] { ' ', '.', '/' };

    // text - The current text entered in the console
    // index - The index of the terminal cursor within {text}
    public string[] GetSuggestions(string text, int index)
    {
        if (text.StartsWith("git "))
            return new string[] { "init", "clone", "pull", "push" };
        else
            return null;
    }
}

ReadLine.AutoCompletionHandler = new AutoCompletionHandler();
```

## Credits

| Author              | For                                                      |
|:--------------------|:---------------------------------------------------------|
| Toni Solarin-Sodara | [Original ReadLine](https://github.com/tonerdo/readline) |
| EoflaOE             | Rebooting the ReadLine project                           |

## License

```
MIT License

Copyright (c) 2017 Toni Solarin-Sodara
Copyright (c) 2022 EoflaOE and its companies

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
```
