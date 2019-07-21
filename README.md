# AvalonEdit Samples
Implements a sample projects to demo the [AvalonEdit](https://github.com/icsharpcode/AvalonEdit) control with extensions.

# Demo Projects

## 00 MVVMDemo

The [00 MVVMDemo](https://github.com/Dirkster99/AvalonEdit-Samples/tree/master/source/00_MVVMDemo) project gives a simple MVVM/WPF demo application that implements some simple features like:
1) Text loading
2) Editing (Copy, Paste, Cut, Undo, Redo), and
3) Highlighting.

This demo project is the base project for all other sample projects in this repository and [Dirkster99/AvalonEditHighlightingThemes](https://github.com/Dirkster99/AvalonEditHighlightingThemes).

## 10 Scale Font Demo

The [10 ScaleFontDemo](https://github.com/Dirkster99/AvalonEdit-Samples/tree/master/source/10_ScaleFontDemo) project 
shows how the display font can be scalled with interactive means like

- the Scale up/scale down (two finger) gesture on your touch-pad, or 
- Control Key and mouse wheel to zoom text size in or out.

## 88 MVVM ThemedDemo

The [88 MVVM ThemedDemo](https://github.com/Dirkster99/AvalonEdit-Samples/tree/master/source/88_MVVM%20ThemedDemo) project shows how syntax highlighting can be integrated into WPF Theming without using
custom highlighting definitions per theme.

This solution is just an intermediate step from the [00 MVVMDemo](https://github.com/Dirkster99/AvalonEdit-Samples/tree/master/source/00_MVVMDemo) solution to give you an understanding
of what might be necessary to add WPF theming to an existing MVVM application. This demo solution also
shows how bad it is to use highlighting definitions for a Light WPF theme on a Dark WPF theme and vice
versa.

Please refer to [Dirkster99/AvalonEditHighlightingThemes](https://github.com/Dirkster99/AvalonEditHighlightingThemes)
for an improved solution that coordinates:
1) Dark/Light/True Blue WPF themes
2) with an appropriate Syntax Highlighting Theme.

## Other AvalonEdit Demo Projects:

More demo projects may be listed at the [AvalonEdit's Wiki page](https://github.com/icsharpcode/AvalonEdit/wiki/Samples-and-Articles)

- [Demonstrates how to add advanced textmarkers to AvalonEdit](https://github.com/siegfriedpammer/AvalonEditSamples)
- [Dirkster99/AvalonEditHighlightingThemes](https://github.com/Dirkster99/AvalonEditHighlightingThemes) Extension implements its own highlighting manager that extends the classic way of handling highlighting definitions.
