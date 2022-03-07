## 2.0.0 (March 07, 2022)
 - Adds support for C# 10 record struct ([#13](https://github.com/Projektanker/code-butler/issues/13))
 - ⚠️ Breaking change: Required runtime changed from .NET 5 to .NET 6 (LTS)

## 1.4.0 (September 29, 2021)
 - New features by [pmahend1](https://github.com/pmahend1)
   - Added same keybinding as Code Maid
     - Ctrl+M Ctrl+Space 🪟 Windows and 📺 Linux
     - Cmd+M Cmd+Space 🍎 Mac
   - Added editor context menu for right click ➔ choose Clean up command
 - Available on [open-vsx.org](https://open-vsx.org/extension/projektanker/code-butler)

## 1.3.0 (March 22, 2021)
 - Improvements by [loreggia](https://github.com/loreggia)
   - Added the ability to run code cleanup on save
   - Added a configuration switch for the automatic cleanup (disabled by default)

## 1.2.1 (January 31, 2021)
Fixes README typo 😅

## 1.2.0 (January 31, 2021)
- Bug fixes:
  - Removes buggy member padding fix feature. It is replaced by a simpler approach due to lack of time:
    - Removes trailing whitespace.
    - Removes consecutive blank lines. between the members. Adds a blank line between members if there is none.

## 1.1.0 (December 25, 2020)

- Bug fixes:
  - Do not add leading blank line before namespace declaration if no using directives exists.
  - README typo
- Improvements:
  - [VS code extension](https://marketplace.visualstudio.com/items?itemName=projektanker.code-butler): Execute `Format Document` after cleaning C# file ([#1](https://github.com/Projektanker/code-butler/issues/1))
  - [VS code extension](https://marketplace.visualstudio.com/items?itemName=projektanker.code-butler): Better error and information messages.