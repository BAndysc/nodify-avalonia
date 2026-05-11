
# NodifyAvalonia <img src="https://github.com/BAndysc/nodify-avalonia/assets/5689666/3b9fe4bd-30c8-4ac7-9b4c-5f1864b83e41" width="120px" alt="Nodify.Avalonia" align="right">

[![NuGet](https://img.shields.io/nuget/v/NodifyAvalonia?style=for-the-badge&logo=nuget&label=release)](https://www.nuget.org/packages/NodifyAvalonia/)
[![NuGet](https://img.shields.io/nuget/dt/NodifyAvalonia?label=downloads&style=for-the-badge&logo=nuget)](https://www.nuget.org/packages/NodifyAvalonia)
[![License](https://img.shields.io/github/license/bandysc/nodify-avalonia?style=for-the-badge)](https://github.com/bandysc/nodify-avalonia/blob/master/LICENSE)
[![C#](https://img.shields.io/static/v1?label=docs&message=WIP&color=orange&style=for-the-badge)](https://github.com/miroiu/nodify/wiki)

This is a direct port of [Nodify by miroiu](https://github.com/miroiu/nodify) to Avalonia. 

> A collection of highly performant controls for node-based editors designed for MVVM.

The goal of the port is to keep the codebase as similar to the original code as possible, to the point, where merges from the upstream are not a problem.

## 🚀 Examples of node-based applications

🔶 A canvas application where you can draw and connect shapes.

> [Examples/Nodify.Shapes](Examples/Nodify.Shapes)

#### ➜ Open a [WASM 🌐 browser version of the shapes demo in your browser, without downloading!](https://avalonia-port.nodify-avalonia.pages.dev/) 🚀

_(Note: C# in a browser is still much slower than standalone desktop C#, the performance is much better when used in a standalone application.)_

![Canvas](https://i.imgur.com/fIf8ACd.gif)

🎨 A playground application where you can try all the available settings.

> [Examples/Nodify.Playground](Examples/Nodify.Playground)

![Playground](https://i.imgur.com/jdAwDeh.gif)

🌓 A state machine where each state represents an executable action, and each transition represents a condition for executing the next action.

> [Examples/Nodify.StateMachine](Examples/Nodify.StateMachine)

![StateMachine](https://i.imgur.com/UU0TQxe.gif)

💻 A simple "real-time" calculator where each node represents an operation that takes input and feeds its output into other node's input.

> [Examples/Nodify.Calculator](Examples/Nodify.Calculator)

![Calculator](https://i.imgur.com/rup58xn.gif)

## 📥 Installation
Use the NuGet package manager to install `NodifyAvalonia`.

```
Install-Package NodifyAvalonia
```

And include Nodify resources:

```
<ResourceInclude Source="avares://Nodify/Theme.axaml" />
```

⚠️⚠️ **Please do not confuse with `Nodify.Avalonia` which is a different package** ⚠️⚠️

Avalonia version compatibility chart:

| Nodify version | Avalonia version |
|----------------|------------------|
| unreleased     | 12.0.3           |
| 6.6.0          | 11.1.0           |
| 6.5.0          | 11.1.0           |
| 6.2.0          | 11.1.0           |
| 6.1.0          | 11.1.0           |
| 6.0.0          | 11.1.0-beta-2    |
| 5.3.0          | 11.1.0-beta-2    |
| 5.2.0          | 11.1.0-beta-1    |

## ⭐️ Features
 
 - Designed from the start to work with **MVVM**
 - **No dependencies** other than ~~WPF~~ Avalonia
 - **Optimized** for interactions with hundreds of nodes at once
 - Built-in dark and light **themes**
 - **Selecting**, **zooming**, **panning** with **auto panning** when close to edge
 - **Select**, **move** and **connect** nodes
 - Lots of **configurable** dependency properties
 - Ready for undo/redo
 - Example applications: 🎨 [**Playground**](Examples/Nodify.Playground), 🌓 [**State machine**](Examples/Nodify.StateMachine), 💻 [**Calculator**](Examples/Nodify.Calculator), 🔶 [**Canvas**](Examples/Nodify.Shapes)

## 😿 Unsupported Features

 - Cutting Lines (blocker: https://github.com/AvaloniaUI/Avalonia/issues/16549)

## 📝 Documentation

For the wiki please refer to the original [miroiu's Wiki](https://github.com/miroiu/nodify/wiki) since the API is identical, but please report bugs here. However, if you find a bug, please try to check if it also occurs in the original WPF's version.

> [!NOTE]  
> Avalonia.Point should be used in place of System.Windows.Point (Anchor points).

## ❤️ [Contributing](CONTRIBUTING.md)

If you find a bug in Avalonia port, bug reports, PRs are more than welcome. If you think this might be not related to Avalonia, please try to reproduce it first in the original [WPF's version](https://github.com/miroiu/nodify).
