# .NET 10.0 Upgrade Plan

## Execution Steps

Execute steps below sequentially one by one in the order they are listed.

1. Validate that a .NET 10.0 SDK required for this upgrade is installed on the machine and if not, help to get it installed.
2. Ensure that the SDK version specified in global.json files is compatible with the .NET 10.0 upgrade.
3. Upgrade newsApi\newsApi.csproj
4. Upgrade NewsApi.Tests\NewsApi.Tests.csproj
5. Run unit tests to validate upgrade in the projects listed below:
   - NewsApi.Tests\NewsApi.Tests.csproj

## Settings

This section contains settings and data used by execution steps.

### Aggregate NuGet packages modifications across all projects

NuGet packages used across all selected projects or their dependencies that need version update in projects that reference them.

| Package Name          | Current Version | New Version           | Description       |
|:----------------------------------------------------------|:---------------:|:---------------------:|:---------------------------------------------------------------------------|
| FluentValidation.AspNetCore            | 11.3.0       | 11.3.1    | Package is deprecated, should be replaced              |
| Microsoft.AspNetCore.Authentication.JwtBearer             | 8.0.8         | 10.0.0-rc.2.25502.107 | Recommended for .NET 10.0  |
| Microsoft.Extensions.Configuration.Json              | 9.0.9 | 10.0.0-rc.2.25502.107 | Recommended for .NET 10.0      |
| Microsoft.VisualStudio.Azure.Containers.Tools.Targets     | 1.19.6        |         | Package is incompatible, no supported version found - should be removed    |

### Project upgrade details

This section contains details about each project upgrade and modifications that need to be done in the project.

#### newsApi\newsApi.csproj modifications

Project properties changes:
  - Target framework should be changed from `net8.0` to `net10.0`

NuGet packages changes:
  - FluentValidation.AspNetCore should be updated from `11.3.0` to `11.3.1` (*package is deprecated*)
  - Microsoft.AspNetCore.Authentication.JwtBearer should be updated from `8.0.8` to `10.0.0-rc.2.25502.107` (*recommended for .NET 10.0*)
  - Microsoft.VisualStudio.Azure.Containers.Tools.Targets should be removed (*package is incompatible, no supported version found*)

#### NewsApi.Tests\NewsApi.Tests.csproj modifications

Project properties changes:
  - Target framework should be changed from `net8.0` to `net10.0`

NuGet packages changes:
  - Microsoft.Extensions.Configuration.Json should be updated from `9.0.9` to `10.0.0-rc.2.25502.107` (*recommended for .NET 10.0*)
