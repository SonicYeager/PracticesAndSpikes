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

# Refactoring Plan (TODO)

The current implementation has some limitations and could be improved by refactoring the code to better handle the `.rc` file structure. Here is a proposed plan:

1.  **Create a proper `.rc` file parser:**
    *   The parser should be able to read an `.rc` file and create a structured representation of its content.
    *   It should be able to handle different resource types (`STRINGTABLE`, `DIALOG`, `MENU`, etc.).
    *   For each resource type, the parser should extract the relevant information (e.g., for a `STRINGTABLE`, it should extract the string ID and the string value).

2.  **Refactor the `Analyser` class:**
    *   The `Analyser` class should use the new parser to read the `.rc` files.
    *   Instead of returning a list of strings, it should return a structured representation of the resources.

3.  **Refactor the `Comparator` class:**
    *   The `Comparator` class should be updated to work with the new structured representation of the resources.
    *   It should be able to compare two sets of resources and identify the differences (added, removed, modified).

4.  **Refactor the `Logic` class:**
    *   The `Logic` class should be updated to work with the new structured representation of the resources.
    *   It should use the refactored `Analyser` and `Comparator` classes to perform the comparison.

5.  **Refactor the `Printer` class:**
    *   The `Printer` class should be updated to generate a human-readable report of the differences based on the structured representation of the resources.