# waifu-2x-nccn-vulkan-gui

[![build and test](https://github.com/jkendall327/waifu2x-ncnn-vulkan-gui/actions/workflows/build-and-test.yml/badge.svg)](https://github.com/jkendall327/waifu2x-ncnn-vulkan-gui/actions/workflows/build-and-test.yml)

![Screenshot of the program](image.jpg)

- GUI frontend for [waifu-2x-nccn-vulkan](https://github.com/nihui/waifu2x-ncnn-vulkan), created by nihui
- Batch processing available
- No dependencies, download and run

You can use this to upscale images (particularly drawings, rather than photographs).

nccn-vulkan is self-contained, so you should't need to install any other dependencies to get it working.

This is a thin wrapper around the console app, so it provides a live preview of what command is going to be run.

Select multiple images to process them all, one at a time.

## Installation

The Releases tab contains a .deb installer which you can download and run to install the app.

Once it's installed, run `waifu2x-gui` in a terminal to launch the app.

Currently I've only compiled this for Linux, but it would probably work alright on Windows or OSX with minor tweaks.

## Building

This project was built with .NET 6 and Avalonia. 

If you have both of those installed, there's nothing weird with the build so you should be able to just clone and build the solution.

Step-by-step:

1. Download and install .NET. You will need a developer SDK, not just the runtime.
2. Ensure .NET is in your path. You can run `dotnet --list-sdks` to see if it works.
3. Clone the repo.
4. Navigate to the repo on your hard-drive.
5. Run `dotnet run` (to build the app and start it) or `dotnet build` to compile an executable. 

## Known issues

Due to a bug in how Avalonia handles non-Latin characters on Linux, the app may crash on startup with a message about not being able to load a default font-family.

If this happens, launch the app with a modified locale environment variable:

`LC_ALL=C [executable path here]`

If you're building from source, you can also try this:

`LANG=en_US.UTF-8 dotnet run`

## Contributions

Highly welcome!
