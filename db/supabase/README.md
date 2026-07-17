# Dream Big Supabase migration

This folder contains the first migration from the local SQLite database to Supabase/PostgreSQL.

## Files

- `001_schema.sql`: PostgreSQL schema for Supabase.
- `export-sqlite-csv.ps1`: exports the current SQLite data to CSV files.
- `import-to-supabase.ps1`: imports the exported CSV files into Supabase using `psql`.

The generated `export/` folder is intentionally ignored by Git because it contains student and payment data.

## Steps

1. Create a Supabase project.
2. Open Supabase SQL Editor and run `001_schema.sql`.
3. Export the local SQLite data:

   ```powershell
   powershell -ExecutionPolicy Bypass -File db\supabase\export-sqlite-csv.ps1
   ```

4. Install PostgreSQL client tools if `psql` is not available.
5. Copy the Supabase connection string from Project Settings > Database.
6. Import the CSV files:

   ```powershell
   powershell -ExecutionPolicy Bypass -File db\supabase\import-to-supabase.ps1 -ConnectionString "YOUR_SUPABASE_CONNECTION_STRING"
   ```

If `psql` is not installed, use the included .NET importer instead:

```powershell
$env:DREAMBIG_SUPABASE_CONNECTION = "YOUR_SUPABASE_CONNECTION_STRING"
dotnet run --project db\supabase\DreamBig.SupabaseImporter\DreamBig.SupabaseImporter.csproj
```

If the direct Supabase host resolves only to IPv6 on your network, use the Supabase **Session pooler** connection string instead of the direct connection string.

## After Import

Run these checks in Supabase SQL Editor:

```sql
select 'students' as table_name, count(*) from students
union all select 'payments', count(*) from payments
union all select 'classes', count(*) from classes
union all select 'enrollments', count(*) from enrollments
union all select 'academic_history', count(*) from academic_history;
```

Expected counts from the current SQLite database:

- `students`: 113
- `payments`: 12
- `classes`: 16
- `enrollments`: 82
- `academic_history`: 167
