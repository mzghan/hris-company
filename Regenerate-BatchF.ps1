$ErrorActionPreference = 'Stop'

Write-Host 'Regenerating Batch F migration metadata...' -ForegroundColor Cyan

$project = Join-Path $PSScriptRoot 'HRIS.Api.csproj'
if (-not (Test-Path $project)) {
    throw 'Run this script from the extracted patch after copying the patch into the HRIS project root.'
}

# The migration file already exists in the patch, but its Designer/Snapshot were not generated.
# Remove only the Batch F migration, then let EF Core scaffold it from the current model.
$migration = Join-Path $PSScriptRoot 'Migrations/20261003160000_BatchF_ManpowerAndJobDescription.cs'
if (Test-Path $migration) { Remove-Item $migration -Force }

$designer = Join-Path $PSScriptRoot 'Migrations/20261003160000_BatchF_ManpowerAndJobDescription.Designer.cs'
if (Test-Path $designer) { Remove-Item $designer -Force }

Push-Location $PSScriptRoot
try {
    dotnet build
    if ($LASTEXITCODE -ne 0) { throw 'dotnet build failed. Fix the compile error before generating the migration.' }

    dotnet ef migrations add BatchF_ManpowerAndJobDescription --output-dir Migrations --no-build
    if ($LASTEXITCODE -ne 0) { throw 'dotnet ef migrations add failed.' }

    Write-Host ''
    Write-Host 'Batch F migration, Designer.cs, and ModelSnapshot have been generated.' -ForegroundColor Green
}
finally {
    Pop-Location
}
