# Code Butler

Command line tool and VS code extension for your `C#` files at your service.
## Features
 - Reorganize your source file (by type, access and name).
 - Fix code spacing:
   - Remove duplicate empty lines.
   - Add empty lines between code blocks.

## Usage
Command line:  
File mode: `dotnet CodeButler.Console.dll path/to/MyClass.cs`
Pipe mode: `type MyClass.cs | dotnet CodeButler.Console.dll > MyClass.Reorganized.cs`
