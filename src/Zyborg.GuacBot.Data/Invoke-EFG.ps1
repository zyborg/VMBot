param(
    [ValidateSet('db-guacdb-my', 'db-guacdb-pg')]
    [Parameter(Mandatory)]
    [string]$Model
)

$efgFile = Join-Path $PSScriptRoot "$($Model).efg.yaml"

if (-not (Test-Path -PathType Leaf $efgFile)) {
    Write-Error "Could not find EFG options file at path [$efgFile]"
    return
}

Push-Location $PSScriptRoot
& efg generate -d $PSScriptRoot -f $efgFile
Pop-Location