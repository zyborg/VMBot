$dotnet = Get-Command dotnet -ErrorAction SilentlyContinue
if (-not $dotnet) {
    throw "dotnet CLI could not be found"
}

$tfmod = Join-Path -Path $PSScriptRoot -ChildPath "../../deploy/tf/guacbot" -Resolve
$tfres = Join-Path -Path $tfmod -ChildPath "res"
$tfpkg = Join-Path -Path $tfres -ChildPath "Zyborg.GuacBot.zip"

if (-not (Test-Path $tfres)) {
    mkdir $tfres
}

& $dotnet lambda package --project-location $PSScriptRoot --output-package $tfpkg
if (-not $?) {
    Write-Error "==================================================================================="
    Write-Error "Failed to invoke Lambda Package tool"
    Write-Error "Make sure the you have the Lambda extensions for dotnet CLI installed:"
    Write-Error "  https://github.com/aws/aws-extensions-for-dotnet-cli#aws-lambda-amazonlambdatools"
    exit
}
