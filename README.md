[![Build status](https://ci.appveyor.com/api/projects/status/s19eint5cqhjxh5h/branch/master?svg=true)](https://ci.appveyor.com/project/Dirkster99/avalonedithighlightingthemes/branch/master) [![Release](https://img.shields.io/github/release/Dirkster99/AvalonEditHighlightingThemes.svg)](https://github.com/Dirkster99/AvalonEditHighlightingThemes/releases/latest) [![NuGet](https://img.shields.io/nuget/dt/Dirkster.HL.svg)](http://nuget.org/packages/Dirkster.HL)

# AvalonEditSamples
Implements a sample projects to demo the [AvalonEdit](https://github.com/icsharpcode/AvalonEdit) control with MVVM (extension) scenarious

# Demo Projects

## 00 MVVMDemo

The [00 MVVMDemo]() project gives a simple MVVM/WPF demo application that implements some simple features like:
1) Text loading
2) Editing (Copy, Paste, Cut, Undo, Redo), and
3) Highlighting.

This demo project is the base project for all other sample projects in this repository and [Dirkster99/AvalonEditHighlightingThemes](https://github.com/Dirkster99/AvalonEditHighlightingThemes).

## 88 MVVM ThemedDemo

The 88 MVVM ThemedDemo project shows how syntax highlighting can be integrated into WPF Theming without using
custom highlighting definitions per theme.

This solution is just an intermediate step from the [00 MVVMDemo]() solution to give you an understanding
of what might be necessary to add WPF theming to an existing MVVM application. This demo solution also
shows how bad it is to use highlighting definitions for a Light WPF theme on a Dark WPF theme and vice
versa.

Please refer to [Dirkster99/AvalonEditHighlightingThemes](https://github.com/Dirkster99/AvalonEditHighlightingThemes)
for an improved solution that coordinates:
1) Dark/Light/True Blue WPF themes
2) with an appropriate Syntax Highlighting Theme.

## Other AvalonEdit Demo Projects:

More demo projects may be listed at the AvalonEdit's Wiki page
- [AvalonEdit](https://github.com/icsharpcode/AvalonEdit/wiki/Samples-and-Articles)

- [https://github.com/siegfriedpammer/AvalonEditSamples](Demonstrates how to add advanced textmarkers to AvalonEdit)
- [Dirkster99/AvalonEditHighlightingThemes](https://github.com/Dirkster99/AvalonEditHighlightingThemes) Extension implements its own highlighting manager that extends the classic way of handling highlighting definitions.
