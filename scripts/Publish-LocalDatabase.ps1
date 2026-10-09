<#
.SYNOPSIS
    Creates or upgrades YOUR OWN local development database from the SQL project.

.DESCRIPTION
    For a personal machine with its own SQL Server, LocalDB or Docker container (ADR 0011).
    It refuses any server that is not on this machine, so it cannot touch a shared, office or
    production server. Never run it on an office laptop.

    -ScriptOnly writes the upgrade script to artifacts/sql for review instead of applying it.
    Changes that would lose data are blocked; fix them with a pre-deployment script instead.

.EXAMPLE
    ./scripts/Publish-LocalDatabase.ps1                                   # LocalDB, database FastXGenErp_Dev
    ./scripts/Publish-LocalDatabase.ps1 -Server "localhost,1433" -SqlLogin sa
    ./scripts/Publish-LocalDatabase.ps1 -ScriptOnly                       # review the SQL first
#>
[CmdletBinding()]
param(
    [string] $Server = '(localdb)\MSSQLLocalDB',
    [string] $Database = 'FastXGenErp_Dev',
    [string] $SqlLogin,
    [switch] $ScriptOnly
)

$ErrorActionPreference = 'Stop'

$localServer = '^(\(localdb\)\\[\w.-]+|localhost|127\.0\.0\.1|\.)(\\[\w.-]+)?(,\d+)?$'
if ($Server -notmatch $localServer) {
    throw "Refusing '$Server': this script only targets a SQL Server on this machine."
}
if ($Database -notmatch '^FastXGenErp_[A-Za-z0-9]+$') {
    throw "Database name must look like 'FastXGenErp_Dev'."
}

$root = Split-Path -Parent $PSScriptRoot
$project = Join-Path $root 'database/FastXGen.Erp.Database/FastXGen.Erp.Database.sqlproj'
$dacpac = Join-Path $root 'database/FastXGen.Erp.Database/bin/Debug/FastXGen.Erp.Database.dacpac'

dotnet build $project -c Debug
if ($LASTEXITCODE -ne 0) { throw 'SQL project build failed.' }

dotnet tool restore | Out-Null

$connection = "Server=$Server;Database=$Database;TrustServerCertificate=True;"
if ($SqlLogin) {
    $password = Read-Host "Password for $SqlLogin" -AsSecureString
    $plain = [System.Net.NetworkCredential]::new('', $password).Password
    $connection += "User Id=$SqlLogin;Password=$plain;"
}
else {
    $connection += 'Integrated Security=True;'
}

$common = @(
    "/SourceFile:$dacpac",
    "/TargetConnectionString:$connection",
    '/p:BlockOnPossibleDataLoss=true',
    '/p:DropObjectsNotInSource=false'
)

if ($ScriptOnly) {
    $outDir = Join-Path $root 'artifacts/sql'
    New-Item -ItemType Directory -Force -Path $outDir | Out-Null
    $outFile = Join-Path $outDir ("{0}_Upgrade_{1}.sql" -f $Database, (Get-Date -Format 'yyyyMMdd-HHmm'))
    dotnet sqlpackage /Action:Script @common "/OutputPath:$outFile"
    if ($LASTEXITCODE -ne 0) { throw 'Script generation failed.' }
    Write-Host "Upgrade script written to $outFile"
}
else {
    dotnet sqlpackage /Action:Publish @common
    if ($LASTEXITCODE -ne 0) { throw 'Publish failed.' }
    Write-Host "Database $Database on $Server is up to date."
    Write-Host "For database tests:  `$env:Database__ConnectionString = '<the connection string above>'"
}
