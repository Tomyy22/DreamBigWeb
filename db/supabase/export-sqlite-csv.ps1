param(
    [string]$SqliteDb = "DreamBig.Web\institute.db",
    [string]$OutputDir = "db\supabase\export"
)

$ErrorActionPreference = "Stop"

if (-not (Test-Path -LiteralPath $SqliteDb)) {
    throw "SQLite database not found: $SqliteDb"
}

New-Item -ItemType Directory -Force -Path $OutputDir | Out-Null

$tables = @(
    "Levels",
    "Rooms",
    "Schedules",
    "Tutors",
    "Students",
    "Teachers",
    "Student_Tutor",
    "Classes",
    "Class_Assignments",
    "Enrollments",
    "Academic_History",
    "Payments"
)

foreach ($table in $tables) {
    $csvPath = Join-Path $OutputDir "$($table.ToLowerInvariant()).csv"
    $commands = @"
.headers on
.mode csv
.once '$csvPath'
SELECT * FROM $table;
"@

    $commands | sqlite3 $SqliteDb
    Write-Host "Exported $table -> $csvPath"
}

