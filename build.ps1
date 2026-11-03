#!/usr/bin/env pwsh
<#
.SYNOPSIS
    Build helper for ElternAnker – mirrors Makefile targets for Windows-native CI.
.EXAMPLE
    .\build.ps1 -Target test
#>
param(
    [ValidateSet("restore","build","test","format","format-check","run","clean","setup-hooks")]
    [string]$Target = "build"
)

$ErrorActionPreference = "Stop"
$sln = "ElternAnker.slnx"

switch ($Target) {
    "setup-hooks"   { git config core.hooksPath .githooks; Write-Host "Git hooks set to .githooks" }
    "restore"       { dotnet restore $sln }
    "build"         { dotnet restore $sln; dotnet build $sln --no-restore --configuration Release }
    "test"          {
        dotnet restore $sln
        dotnet build $sln --no-restore --configuration Release
        dotnet test $sln --no-build --configuration Release `
            --logger "console;verbosity=normal" `
            --collect:"XPlat Code Coverage"
    }
    "format"        { dotnet format $sln }
    "format-check"  { dotnet format $sln --verify-no-changes }
    "run"           { dotnet run --project src/ElternAnker.Api --environment Development --configuration Debug }
    "clean"         {
        dotnet clean $sln
        Get-ChildItem -Recurse -Directory -Filter "bin" | Remove-Item -Recurse -Force
        Get-ChildItem -Recurse -Directory -Filter "obj" | Remove-Item -Recurse -Force
    }
}

