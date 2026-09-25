# Builds the React frontend into the desktop host.
# Run from the repository root: powershell -File scripts/build-frontend.ps1
$ErrorActionPreference = "Stop"

$frontend = Join-Path $PSScriptRoot ".." "frontend"
Push-Location $frontend
try {
    pnpm install
    pnpm build
}
finally {
    Pop-Location
}
