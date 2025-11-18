# Project Overview

This project is a .NET library called `PdfToolKit` for creating and manipulating PDF files. It's built on top of the MIT-licensed libraries `PDFsharp` and `SkiaSharp`.

## Main Features

*   **PDF Merging:** Combine multiple PDF files into a single document.
*   **Document Creation:** Programmatically create PDF documents from scratch.
*   **Content Elements:** Add paragraphs, tables, and images to documents.
*   **Vector Graphics:** Integrate complex, hardware-accelerated 2D graphics using SkiaSharp.
*   **Form Simulation:** Create layouts that resemble forms, although interactive fields are not yet implemented.

# Building and Running

This is a .NET library project. To use it, you would typically reference it in another .NET application (e.g., a console app, a web API).

## Building the Library

You can build the project using the `dotnet build` command:

```bash
dotnet build
```

This will compile the library into a DLL file, which can then be used by other projects.

## Running the Examples

The `PdfToolKit.cs` file includes an `Examples` class with a `DemoAll()` method. To run these examples, you would need to create a console application that references this library and calls that method.

**TODO:** Add a simple console application to the solution to demonstrate the library's features.

# Development Conventions

*   **Framework:** The project targets .NET 10.0.
*   **Dependencies:** It uses `PDFsharp` for core PDF manipulation and `SkiaSharp` for advanced graphics.
*   **Language:** The code is written in C#.
*   **Style:** The code seems to follow standard C#/.NET conventions.
*   **Licensing:** The project describes itself as "100% MIT-licensed".
