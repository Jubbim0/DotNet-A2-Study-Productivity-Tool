# Study Productivity App

A Windows desktop productivity application built with C#, WPF, .NET, Entity Framework Core, SQLite and NUnit.

## Prerequisites

- Windows 10/11
- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)
- Visual Studio 2022 or later (optional; .NET 10 development requires Visual Studio 2026 or a compatible .NET CLI environment)

## Install Dependencies

From the repository root:

```powershell
cd src
dotnet restore StudyProductivityApp.slnx
```

NuGet dependencies, including Entity Framework Core, SQLite, LiveCharts and NUnit, are restored automatically.

## Build

```powershell
dotnet build StudyProductivityApp.slnx
```

## Run

```powershell
dotnet run --project StudyProductivityApp/StudyProductivityApp.csproj
```

The SQLite database is automatically created and migrated on startup. Application data is stored at:

```text
%LOCALAPPDATA%\StudyProductivityApp\studyapp.db
```

## Test

Run all NUnit tests:

```powershell
dotnet test StudyProductivityApp.slnx
```

Alternatively, use **Test Explorer** in a compatible version of Visual Studio.

## Notes

- The application requires Windows because it uses WPF.
- No external database server is required; SQLite runs locally.
- The application and test projects must target compatible .NET versions.
