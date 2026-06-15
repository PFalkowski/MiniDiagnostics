# MiniDiagnostics

[![CI](https://github.com/PFalkowski/MiniDiagnostics/actions/workflows/ci.yml/badge.svg)](https://github.com/PFalkowski/MiniDiagnostics/actions/workflows/ci.yml)
[![Quality Gate Status](https://sonarcloud.io/api/project_badges/measure?project=PFalkowski_MiniDiagnostics&metric=alert_status)](https://sonarcloud.io/summary/new_code?id=PFalkowski_MiniDiagnostics)
[![Coverage](https://sonarcloud.io/api/project_badges/measure?project=PFalkowski_MiniDiagnostics&metric=coverage)](https://sonarcloud.io/summary/new_code?id=PFalkowski_MiniDiagnostics)
[![NuGet version](https://img.shields.io/nuget/v/MiniDiagnostics.svg)](https://www.nuget.org/packages/MiniDiagnostics/)
[![NuGet downloads](https://img.shields.io/nuget/dt/MiniDiagnostics.svg)](https://www.nuget.org/packages/MiniDiagnostics/)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](LICENSE)
[![Buy Me A Coffee](https://img.shields.io/badge/Buy%20Me%20A%20Coffee-support-yellow.svg)](https://www.buymeacoffee.com/piotrfalkowski)

Windows system diagnostics utilities for .NET 8. Get CPU and RAM usage, OS name, drive information, thread pool status, check whether the current process is elevated, and more.

> **Platform:** Windows only (`net8.0-windows`). All APIs are backed by Windows kernel calls.

## Installation

```
dotnet add package MiniDiagnostics
```

## Quick start

```csharp
using MiniDiagnostics;

// CPU and memory
float cpuPct   = Performance.CpuCurrentUsage();   // first call is always 0 (warm-up)
float ramFreeMb = Performance.RamFreeMb();
float ramUsedMb = Performance.RamUsedMb();
ulong ramTotalB = Performance.RamTotalB;

// OS and process info
string os      = Helper.OsName;
string procName = Helper.ExecutingAssemblyName;
bool elevated  = Helper.IsAdminElevated();

// Drive listing
foreach (var drive in Helper.DrivesInfo)
    Console.WriteLine(drive);
```

## API

### `Performance`

| Member | Returns | Description |
|--------|---------|-------------|
| `CpuCurrentUsage()` | `float` | % CPU used across all logical processors (first call = 0, warm-up) |
| `RamFreeB()` | `float` | Available physical RAM in bytes |
| `RamFreeKb()` | `float` | Available physical RAM in KB |
| `RamFreeMb()` | `float` | Available physical RAM in MB |
| `RamFreeGb()` | `float` | Available physical RAM in GB |
| `RamUsedB()` | `float` | Used physical RAM in bytes |
| `RamUsedKb()` | `float` | Used physical RAM in KB |
| `RamUsedMb()` | `float` | Used physical RAM in MB |
| `RamUsedGb()` | `float` | Used physical RAM in GB |
| `RamTotalB` | `ulong` | Total installed physical RAM in bytes |
| `RamTotalKb` | `float` | Total installed physical RAM in KB |
| `RamTotalMb` | `float` | Total installed physical RAM in MB |
| `RamTotalGb` | `float` | Total installed physical RAM in GB |
| `VirtualTotalB/Kb/Mb/Gb` | `ulong` | Total virtual address space |
| `VirtualFreeB/Kb/Mb/Gb` | `ulong` | Available virtual address space |
| `VirtualUsedB/Kb/Mb/Gb` | `ulong` | Used virtual address space |
| `RamPercentFree()` | `double` | % of physical RAM free |
| `RamPercentUsed()` | `double` | % of physical RAM in use |
| `VirtualPercentFree()` | `double` | % of virtual address space free |
| `VirtualPercentUsed()` | `double` | % of virtual address space in use |

### `Helper`

| Member | Returns | Description |
|--------|---------|-------------|
| `OsName` | `string` | OS friendly name (tries registry, then `Environment.OSVersion`) |
| `OsNameFromRegistry` | `string` | OS name directly from `SOFTWARE\Microsoft\Windows NT\CurrentVersion` |
| `ProcessorName` | `string` | CPU name from registry |
| `ExecutingAssemblyName` | `string` | File name of the executing assembly |
| `ExecutingAssemblyGuid` | `string` | `[Guid]` attribute value of the executing assembly, or empty string |
| `IsAdminElevated()` | `bool` | `true` if the current process runs as administrator |
| `DrivesInfo` | `IEnumerable<string>` | Formatted drive list with type, format, size, free space |
| `ThreadPoolStatus` | `string` | Human-readable thread pool min/max/available summary |
| `TimeFromStartupEnvironment` | `TimeSpan` | Time since OS boot via `Environment.TickCount` |

## License

MIT — see [LICENSE](LICENSE).
