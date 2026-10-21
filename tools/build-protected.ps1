# Run from a VS Developer PowerShell with Python, NuGet and Windows SDK signtool.
param(
    [Parameter(Mandatory=$true)][string]$VendorSdkDirectory,
    [Parameter(Mandatory=$true)][ValidatePattern('^[0-9a-fA-F]{40}$')][string]$CodeSigningThumbprint,
    [string]$TimestampUrl = 'http://timestamp.digicert.com'
)
$ErrorActionPreference = 'Stop'
$repo = Split-Path $PSScriptRoot -Parent
function RequireSuccess([string]$step) { if ($LASTEXITCODE -ne 0) { throw "$step failed ($LASTEXITCODE)" } }
Push-Location $repo
try {
    python tools/generate-web-integrity.py --check
    RequireSuccess 'Reviewed UI hashes'
    nuget restore ElecTest2026.sln -NonInteractive
    RequireSuccess 'Dependency restore'
    nuget install Obfuscar -Version 2.2.50 -OutputDirectory packages -NonInteractive
    RequireSuccess 'Obfuscator restore'
    MSBuild.exe ElecTest2026.sln /t:Rebuild /p:Configuration=Release "/p:VendorSdkDirectory=$VendorSdkDirectory"
    RequireSuccess 'Production build'
    # Unique staging directory; never publish files from a failed invocation.
    $stage = Join-Path $repo ('artifacts/build-' + [Guid]::NewGuid().ToString('N'))
    $obfuscated = Join-Path $stage 'private-obfuscation'
    $package = Join-Path $stage 'package'
    New-Item -ItemType Directory -Path $obfuscated,$package -Force | Out-Null
    [xml]$config = Get-Content security/obfuscar.xml
    ($config.Obfuscator.Var | Where-Object name -eq 'InPath').value = Join-Path $repo 'Testplan/bin/Release'
    ($config.Obfuscator.Var | Where-Object name -eq 'OutPath').value = $obfuscated
    $configPath = Join-Path $stage 'obfuscar.xml'
    $config.Save($configPath)
    & packages/Obfuscar.2.2.50/tools/Obfuscar.Console.exe $configPath
    RequireSuccess 'Obfuscation'
    $release = Join-Path $repo 'Testplan/bin/Release'
    Get-ChildItem $release -File | Where-Object { $_.Extension -in '.dll','.config' } | Copy-Item -Destination $package
    Copy-Item (Join-Path $release 'Web') $package -Recurse
    Copy-Item (Join-Path $obfuscated 'ElecTest2026.exe') $package
    # No local settings or issued lists copied; configure authorization after installation.
    $exe = Join-Path $package 'ElecTest2026.exe'
    signtool sign /sha1 $CodeSigningThumbprint /fd SHA256 /tr $TimestampUrl /td SHA256 $exe
    RequireSuccess 'Publisher signing'
    signtool verify /pa $exe
    RequireSuccess 'Publisher signature verification'
    Write-Host "Signed candidate: $package"
    Write-Host 'Validate this exact obfuscated package on Windows and instruments before distributing.'
    Write-Host "Private mapping (do not distribute): $obfuscated"
} finally { Pop-Location }
