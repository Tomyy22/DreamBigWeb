param(
    [Parameter(Mandatory = $true)]
    [string]$ConnectionString,

    [string]$InputDir = "db\supabase\export"
)

$ErrorActionPreference = "Stop"

if (-not (Get-Command psql -ErrorAction SilentlyContinue)) {
    throw "psql was not found. Install PostgreSQL client tools before running this script."
}

if (-not (Test-Path -LiteralPath $InputDir)) {
    throw "CSV input directory not found: $InputDir"
}

$copySql = @"
\set ON_ERROR_STOP on

\copy levels(id_level, level_name, price) from '$InputDir/levels.csv' with (format csv, header true);
\copy rooms(id_room, room_name) from '$InputDir/rooms.csv' with (format csv, header true);
\copy schedules(id_schedule, start_time, end_time) from '$InputDir/schedules.csv' with (format csv, header true);
\copy tutors(id_tutor, first_name, last_name, phone_number, address) from '$InputDir/tutors.csv' with (format csv, header true);
\copy students(id_student, first_name, last_name, status, phone, dni) from '$InputDir/students.csv' with (format csv, header true);
\copy teachers(id_teacher, first_name, last_name, phone) from '$InputDir/teachers.csv' with (format csv, header true);
\copy student_tutor(id_student, id_tutor) from '$InputDir/student_tutor.csv' with (format csv, header true);
\copy classes(id_class, id_level, modality, id_teacher) from '$InputDir/classes.csv' with (format csv, header true);
\copy class_assignments(id_assignment, id_class, id_room, id_schedule, day_of_week) from '$InputDir/class_assignments.csv' with (format csv, header true);
\copy enrollments(id_enrollment, id_student, id_class, school_year) from '$InputDir/enrollments.csv' with (format csv, header true);
\copy academic_history(id_grade, id_student, school_year, id_level_taken, written_score, oral_score, intl_exam_info, exam_date) from '$InputDir/academic_history.csv' with (format csv, header true);
\copy payments(id_payment, id_student, year, month, amount, is_paid, payment_date) from '$InputDir/payments.csv' with (format csv, header true);

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
"@

$tempSql = Join-Path ([System.IO.Path]::GetTempPath()) "dreambig-supabase-import.sql"
Set-Content -Path $tempSql -Value $copySql -Encoding UTF8

psql $ConnectionString -f $tempSql

