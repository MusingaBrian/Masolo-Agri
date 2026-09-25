# Publishes the Masolo Agro desktop app for this PC (Windows x64).
# Run from the repository root: powershell -File scripts/publish.ps1
$ErrorActionPreference = "Stop"

$root = Split-Path $PSScriptRoot -Parent

& (Join-Path $PSScriptRoot "build-frontend.ps1")

dotnet publish (Join-Path $root "src" "MasoloAgro.App" "MasoloAgro.App.csproj") `
    -c Release `
    -r win-x64 `
    --self-contained false `
    -p:PublishSingleFile=true `
    -o (Join-Path $root "publish")
