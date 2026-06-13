param(
    [string]$Command = "dotnet run --launch-profile http"
)

$envFile = Join-Path $PSScriptRoot ".env"
if (-not (Test-Path $envFile)) {
    Write-Error ".env not found at $envFile"
    exit 1
}

Get-Content $envFile | ForEach-Object {
    $line = $_.Trim()
    if ($line -and $line -notlike "#*" -and $line -like "*=*") {
        $eq = $line.IndexOf("=")
        $key = $line.Substring(0, $eq).Trim()
        $value = $line.Substring($eq + 1).Trim()
        [Environment]::SetEnvironmentVariable($key, $value)
    }
}

Write-Host "Loaded .env — running: $Command"
Write-Host ""

$projDir = Join-Path $PSScriptRoot "backend\src\TicketAlert.Api"
Push-Location $projDir
try {
    Invoke-Expression $Command
} finally {
    Pop-Location
}
