# Project Overview

This is a C# Windows Forms application designed to compare and analyze `.rc` (resource) files. The main purpose of the application is to help developers identify differences between two versions of a resource file, which is particularly useful in localization workflows.

## Key Features

*   **Compare Resources:** Identifies added, removed, or modified resources between two `.rc` files.
*   **Check Format Specifiers:** Verifies the consistency of format specifiers (e.g., `%s`, `%d`) in string resources.
*   **Find Untranslated Strings:** Detects strings that have not been translated.
*   **Sort Resources:** Sorts the resources in an `.rc` file according to a predefined order.

## Architecture

The application follows a simple, layered architecture:

*   **UI Layer (`UI.cs`, `UI.Designer.cs`):** The user interface is built with Windows Forms. It allows users to select the `.rc` files to compare and choose the desired analysis operation.
*   **Business Logic Layer (`CodeDirectory/LogicClass/Logic.cs`):** This layer orchestrates the analysis process. It uses a `BackgroundWorker` to perform long-running operations without blocking the UI thread.
*   **Tools Layer (`CodeDirectory/ToolsDirectory`):** This layer contains a set of specialized tools for parsing, comparing, and manipulating `.rc` files.
    *   **`Analyser`:** Reads and parses the `.rc` files.
    *   **`Comparator`:** Compares the parsed resources.
    *   **`Sorter`:** Sorts the resources.
    *   **`Extractor`:** Extracts specific information from the resources.
    *   **`Composer`:** Assembles the output file.
    *   **`Printer`:** Writes the results to a file.

# Building and Running

To build and run the project, you will need the .NET SDK installed.

1.  **Open the solution file (`ResourceCompare.sln`) in Visual Studio.**
2.  **Build the solution (Build > Build Solution).**
3.  **Run the application (Debug > Start Debugging or press F5).**

Alternatively, you can use the .NET CLI:

```bash
# Restore dependencies
dotnet restore

# Build the project
dotnet build

# Run the application
dotnet run
```

# Development Conventions

*   **Code Style:** The code generally follows the standard C# coding conventions.
*   **File Organization:** The code is organized into directories based on functionality (e.g., `LogicClass`, `ToolsDirectory`).
*   **Asynchronous Operations:** The application uses a `BackgroundWorker` to perform file analysis asynchronously, keeping the UI responsive.
*   **Error Handling:** The application includes basic error handling, such as catching `FileNotFoundException` when a specified `.rc` file does not exist.
