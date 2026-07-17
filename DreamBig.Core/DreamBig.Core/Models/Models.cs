using System;

namespace DreamBigManagement.Models
{
    public class Level
    {
        public int id_level { get; set; }
        public string level_name { get; set; }
        public double price { get; set; }
    }

    public class Room
    {
        public int id_room { get; set; }
        public string room_name { get; set; }
    }

    public class Schedule
    {
        public int id_schedule { get; set; }
        public string start_time { get; set; }
        public string end_time { get; set; }

        public string DisplayTime => $"{start_time} - {end_time}";
    }

    public class Tutor
    {
        public int id_tutor { get; set; }
        public string first_name { get; set; }
        public string last_name { get; set; }
        public string phone_number { get; set; }
        public string address { get; set; }

        public string FullName => $"{last_name}, {first_name}";
    }

    public class Student
    {
        public int id_student { get; set; }
        public string DNI { get; set; }
        public string first_name { get; set; }
        public string last_name { get; set; }
        public string phone { get; set; }
        public int status { get; set; }

        public double price { get; set; }
        public string FullName => $"{last_name}, {first_name}";

        public int? id_current_level { get; set; }
        public string current_level_name { get; set; }

        public override string ToString()
        {
            return $"{last_name.ToUpper()}, {first_name}";
        }
    }

    public class StudentTutor
    {
        public int id_student { get; set; }
        public int id_tutor { get; set; }
    }

    public class ClassGroup
    {
        public int id_class { get; set; }
        public int id_level { get; set; }
        public int id_teacher { get; set; }
        public string modality { get; set; }

        public string level_name { get; set; }
        public string teacher_name { get; set; }

        public string start_time { get; set; }
        public string room_name { get; set; }

        public string DisplayInfo => $"{level_name} - {teacher_name} ({modality})";
    }

    public class ClassAssignment
    {
        public int id_assignment { get; set; }
        public int id_class { get; set; }
        public int id_room { get; set; }
        public int id_schedule { get; set; }
        public int day_of_week { get; set; }

        public string room_name { get; set; }
        public string start_time { get; set; }
        public string end_time { get; set; }

        public string DayName => day_of_week switch
        {
            0 => "Lunes",
            1 => "Martes",
            2 => "Miércoles",
            3 => "Jueves",
            4 => "Viernes",
            5 => "Sábado",
            _ => "Desconocido"
        };
    }

    public class Enrollment
    {
        public int id_enrollment { get; set; }
        public int id_student { get; set; }
        public int id_class { get; set; }
        public int school_year { get; set; }

        public string student_name { get; set; }
    }

    public class AcademicHistory
    {
        public int id_grade { get; set; }
        public int id_student { get; set; }
        public int school_year { get; set; }
        public int id_level_taken { get; set; }
        public double written_score { get; set; }
        public double oral_score { get; set; }
        public string intl_exam_info { get; set; }
        public string exam_date { get; set; }
        public string LevelName { get; set; }
    }

    public class Payment
    {
        public int id_payment { get; set; }
        public int id_student { get; set; }
        public int year { get; set; }
        public int month { get; set; }
        public double amount { get; set; }
        public int is_paid { get; set; }
        public string payment_date { get; set; }
    }

    public class Teacher
    {
        public int id_teacher { get; set; }
        public string first_name { get; set; }
        public string last_name { get; set; }
        public string phone { get; set; }

        public string full_name => $"{first_name} {last_name}";
    }

    public class WeeklyScheduleItem
    {
        public int id_assignment { get; set; }
        public int day_of_week { get; set; }
        public int id_schedule { get; set; }
        public int id_class { get; set; }
        public string start_time { get; set; }
        public string end_time { get; set; }

        public string level_name { get; set; } = string.Empty;
        public string room_name { get; set; } = string.Empty;
        public string teacher_name { get; set; } = string.Empty;
    }
}