using Npgsql;

var rawConnectionString = Environment.GetEnvironmentVariable("DREAMBIG_SUPABASE_CONNECTION");
if (string.IsNullOrWhiteSpace(rawConnectionString))
{
    Console.Error.WriteLine("Missing DREAMBIG_SUPABASE_CONNECTION environment variable.");
    return 1;
}

var connectionString = NormalizeConnectionString(rawConnectionString);

var root = FindRoot();
var schemaPath = Path.Combine(root, "db", "supabase", "001_schema.sql");
var exportDir = Path.Combine(root, "db", "supabase", "export");

await using var connection = new NpgsqlConnection(connectionString);
await connection.OpenAsync();

Console.WriteLine("Connected to Supabase.");

if (args.Length == 2 && args[0] == "--apply-sql")
{
    var sqlPath = Path.GetFullPath(args[1]);
    await ExecuteAsync(connection, File.ReadAllText(sqlPath));
    Console.WriteLine($"Applied SQL file: {sqlPath}");
    return 0;
}

await ExecuteAsync(connection, File.ReadAllText(schemaPath));
Console.WriteLine("Schema applied.");

await ExecuteAsync(connection, """
truncate table
    payments,
    academic_history,
    enrollments,
    class_assignments,
    classes,
    student_tutor,
    teachers,
    students,
    tutors,
    schedules,
    rooms,
    levels
restart identity cascade;
""");
Console.WriteLine("Existing data cleared.");

var imports = new[]
{
    new ImportSpec("levels", "id_level, level_name, price", "levels.csv"),
    new ImportSpec("rooms", "id_room, room_name", "rooms.csv"),
    new ImportSpec("schedules", "id_schedule, start_time, end_time", "schedules.csv"),
    new ImportSpec("tutors", "id_tutor, first_name, last_name, phone_number, address", "tutors.csv"),
    new ImportSpec("students", "id_student, first_name, last_name, status, phone, dni", "students.csv"),
    new ImportSpec("teachers", "id_teacher, first_name, last_name, phone", "teachers.csv"),
    new ImportSpec("student_tutor", "id_student, id_tutor", "student_tutor.csv"),
    new ImportSpec("classes", "id_class, id_level, modality, id_teacher", "classes.csv"),
    new ImportSpec("class_assignments", "id_assignment, id_class, id_room, id_schedule, day_of_week", "class_assignments.csv"),
    new ImportSpec("enrollments", "id_enrollment, id_student, id_class, school_year", "enrollments.csv"),
    new ImportSpec("academic_history", "id_grade, id_student, school_year, id_level_taken, written_score, oral_score, intl_exam_info, exam_date", "academic_history.csv"),
    new ImportSpec("payments", "id_payment, id_student, year, month, amount, is_paid, payment_date", "payments.csv")
};

foreach (var import in imports)
{
    var csvPath = Path.Combine(exportDir, import.FileName);
    var sql = $"copy {import.TableName} ({import.Columns}) from stdin with (format csv, header true, null '')";

    await using var writer = await connection.BeginTextImportAsync(sql);
    using var reader = File.OpenText(csvPath);
    while (await reader.ReadLineAsync() is { } line)
        await writer.WriteLineAsync(line);

    Console.WriteLine($"Imported {import.TableName}.");
}

await ExecuteAsync(connection, """
select setval(pg_get_serial_sequence('levels', 'id_level'), coalesce(max(id_level), 1), max(id_level) is not null) from levels;
select setval(pg_get_serial_sequence('rooms', 'id_room'), coalesce(max(id_room), 1), max(id_room) is not null) from rooms;
select setval(pg_get_serial_sequence('schedules', 'id_schedule'), coalesce(max(id_schedule), 1), max(id_schedule) is not null) from schedules;
select setval(pg_get_serial_sequence('tutors', 'id_tutor'), coalesce(max(id_tutor), 1), max(id_tutor) is not null) from tutors;
select setval(pg_get_serial_sequence('students', 'id_student'), coalesce(max(id_student), 1), max(id_student) is not null) from students;
select setval(pg_get_serial_sequence('teachers', 'id_teacher'), coalesce(max(id_teacher), 1), max(id_teacher) is not null) from teachers;
select setval(pg_get_serial_sequence('classes', 'id_class'), coalesce(max(id_class), 1), max(id_class) is not null) from classes;
select setval(pg_get_serial_sequence('class_assignments', 'id_assignment'), coalesce(max(id_assignment), 1), max(id_assignment) is not null) from class_assignments;
select setval(pg_get_serial_sequence('enrollments', 'id_enrollment'), coalesce(max(id_enrollment), 1), max(id_enrollment) is not null) from enrollments;
select setval(pg_get_serial_sequence('academic_history', 'id_grade'), coalesce(max(id_grade), 1), max(id_grade) is not null) from academic_history;
select setval(pg_get_serial_sequence('payments', 'id_payment'), coalesce(max(id_payment), 1), max(id_payment) is not null) from payments;
""");

await using var countCommand = new NpgsqlCommand("""
select 'students' as table_name, count(*) from students
union all select 'academic_history', count(*) from academic_history
union all select 'enrollments', count(*) from enrollments
union all select 'classes', count(*) from classes
union all select 'payments', count(*) from payments
union all select 'levels', count(*) from levels
order by table_name;
""", connection);

await using var counts = await countCommand.ExecuteReaderAsync();
while (await counts.ReadAsync())
    Console.WriteLine($"{counts.GetString(0)}: {counts.GetInt64(1)}");

Console.WriteLine("Done.");
return 0;

static async Task ExecuteAsync(NpgsqlConnection connection, string sql)
{
    await using var command = new NpgsqlCommand(sql, connection);
    await command.ExecuteNonQueryAsync();
}

static string FindRoot()
{
    var directory = new DirectoryInfo(AppContext.BaseDirectory);
    while (directory is not null)
    {
        if (File.Exists(Path.Combine(directory.FullName, "DreamBigManagement.sln")))
            return directory.FullName;

        directory = directory.Parent;
    }

    throw new InvalidOperationException("Could not find repository root.");
}

static string NormalizeConnectionString(string value)
{
    if (!value.StartsWith("postgres://", StringComparison.OrdinalIgnoreCase) &&
        !value.StartsWith("postgresql://", StringComparison.OrdinalIgnoreCase))
    {
        return value;
    }

    var uri = new Uri(value);
    var userInfo = uri.UserInfo.Split(':', 2);
    var builder = new NpgsqlConnectionStringBuilder
    {
        Host = uri.Host,
        Port = uri.Port,
        Database = uri.AbsolutePath.TrimStart('/'),
        Username = Uri.UnescapeDataString(userInfo[0]),
        Password = userInfo.Length > 1 ? Uri.UnescapeDataString(userInfo[1]) : string.Empty,
        SslMode = SslMode.Require
    };

    return builder.ConnectionString;
}

internal sealed record ImportSpec(string TableName, string Columns, string FileName);
