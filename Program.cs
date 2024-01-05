using System.Data.Common;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.IdentityModel.Tokens;

namespace Labb_3
{
    public class LocalDB : DbContext
    {
        public DbSet<Betyg> Betyg { get; set; }
        public DbSet<Klass> Klasser { get; set; }
        public DbSet<Kurs> Kurser { get; set; }
        public DbSet<Personal> Personal { get; set; }
        public DbSet<Roll> Roller { get; set; }
        public DbSet<RollerPersonal> RollerPersonal { get; set; }
        public DbSet<Student> Studenter { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Server=localhost;Database=Skola;User Id=sa;Password=password;Encrypt=False;");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<RollerPersonal>()
                .HasKey(e => new { e.RollID, e.PersonalID });
        }

    }

    public class Betyg
    {
        public int BetygID { get; set; }
        public string? Resultat { get; set; }
        public DateTime Datum { get; set; }
        public int PersonalID { get; set; }
        public int StudentID { get; set; }
        public int KursID { get; set; }
    }

    public class Klass
    {
        public int KlassID { get; set; }
        public int StudentID { get; set; }
    }

    public class Kurs
    {
        public int KursID { get; set; }
        public int StudentID { get; set; }
    }

    public class Personal
    {
        public int PersonalID { get; set; }
        public string? Personnummer { get; set; }
        public string? Förnamn { get; set; }
        public string? Efternamn { get; set; }
        public decimal Lön { get; set; }
        public DateTime Anställningsdatum { get; set; }
    }

    public class Roll
    {
        public int RollID { get; set; }
        public string? RollNamn { get; set; }
    }

    public class RollerPersonal
    {
        public int RollID { get; set; }
        public int PersonalID { get; set; }
    }

    public class Student
    {
        public int StudentID { get; set; }
        public string? Personnummer { get; set; }
        public string? Förnamn { get; set; }
        public string? Efternamn { get; set; }
    }

    internal class Program
    {
        private static void Main(string[] args)
        {
            string connectionString = "Server=localhost;Database=Skola;User Id=sa;Password=password;Encrypt=False;";
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                }
                catch (Exception)
                {
                    Console.WriteLine("Kunde inte ansluta till servern.");
                    return;
                }

                bool exit = false;

                while (!exit)
                {
                    PrintMenu();

                    char option = Console.ReadKey(true).KeyChar;

                    switch (option)
                    {
                        case '1':
                            Console.WriteLine("\nVisa personal med specifik roll? y/n");
                            option = Console.ReadKey(true).KeyChar;
                            switch (option)
                            {
                                case 'Y':
                                case 'y':
                                    using (SqlCommand cmd = new SqlCommand("SELECT * FROM Roller ORDER BY RollID", connection))
                                    {
                                        using (SqlDataReader reader = cmd.ExecuteReader())
                                        {
                                            Console.Write("\n");

                                            while (reader.Read())
                                            {
                                                Console.WriteLine($"{reader[0]} {reader[1]}");
                                            }
                                        }
                                    }

                                    Console.Write("\nVälj roll: ");
                                    string? input = Console.ReadLine();
                                    int roleChoice;
                                    Int32.TryParse(input, out roleChoice);

                                    using (SqlCommand cmd = new SqlCommand($"EXEC SelectAllPersonal @Roll = {roleChoice}", connection))
                                    {
                                        using (SqlDataReader reader = cmd.ExecuteReader())
                                        {
                                            Console.Write("\n");

                                            while (reader.Read())
                                            {
                                                Console.WriteLine($"{reader[0]} {reader[1]} {reader[2]} {reader[3]} Lön: {Math.Round((decimal)reader[4])}kr Anställd: {Math.Round((DateTime.Now - (DateTime)reader[5]).Days / 365.0)} år");
                                            }

                                            Console.WriteLine("\nTryck på valfri tangent för att gå vidare...");
                                            Console.ReadKey(true);                             
                                        }
                                    }
                                    break;
                                case 'N':
                                case 'n':
                                    using (SqlCommand cmd = new SqlCommand($"EXEC SelectAllPersonal", connection))
                                    {
                                        using (SqlDataReader reader = cmd.ExecuteReader())
                                        {
                                            Console.Write("\n");

                                            while (reader.Read())
                                            {
                                                Console.WriteLine($"{reader[0]} {reader[1]} {reader[2]} {reader[3]} Lön: {Math.Round((decimal)reader[4])}kr Anställd: {Math.Round((DateTime.Now - (DateTime)reader[5]).Days / 365.0)} år");
                                            }

                                            Console.WriteLine("\nTryck på valfri tangent för att gå vidare...");
                                            Console.ReadKey(true);                             
                                        }
                                    }
                                    break;
                                default:
                                    Console.WriteLine($"\n'{option}' är inte ett alternativ.");
                                    break;
                            }
                            break;
                        case '2':
                            using (var context = new LocalDB())
                            {
                                var studenter = context.Studenter.ToList();

                                Console.WriteLine("Sortera efter:\n1) Förnamn.\n2) Efternamn.");
                                option = Console.ReadKey(true).KeyChar;

                                bool lastName;
                                switch (option)
                                {
                                    case '2':
                                        lastName = true;
                                        break;
                                    case '1':
                                    default:
                                        lastName = false;
                                        break;
                                }
                                Console.WriteLine("Sortera efter:\n1) Stigande ordning.\n2) Fallande ordning.");
                                option = Console.ReadKey(true).KeyChar;

                                switch (option)
                                {
                                    case '2':
                                        studenter = lastName
                                            ? studenter.OrderByDescending(e => e.Efternamn).ToList()
                                            : studenter.OrderByDescending(e => e.Förnamn).ToList();
                                        break;
                                    case '1':
                                    default:
                                        studenter = lastName
                                            ? studenter.OrderBy(e => e.Efternamn).ToList()
                                            : studenter.OrderBy(e => e.Förnamn).ToList();
                                        break;
                                }
                                Console.Write("\n");

                                foreach (var student in studenter)
                                {
                                    Console.WriteLine($"{student.Personnummer} {student.Förnamn} {student.Efternamn}");
                                }

                                Console.WriteLine("\nTryck på valfri tangent för att gå vidare...");
                                Console.ReadKey(true);     
                            }
                            break;
                        case '3':
                            Console.WriteLine("\nVälj en klass:");
                            using (var context = new LocalDB())
                            {
                                var klasser = context.Klasser.GroupBy(klass => klass.KlassID).Select(group => group.First()).ToList();

                                foreach (var klass in klasser)
                                {
                                    Console.WriteLine($"Klass: {klass.KlassID}");
                                }

                                string? input = Console.ReadLine();
                                int classChoice;
                                Int32.TryParse(input, out classChoice);

                                var query = from student in context.Studenter
                                            join klass in context.Klasser on student.StudentID equals klass.StudentID
                                            where klass.KlassID == classChoice
                                            select new
                                            {
                                                student.Personnummer,
                                                student.Förnamn,
                                                student.Efternamn,
                                                klass.KlassID
                                            };
                                var students = query.ToList();

                                foreach (var student in students)
                                {
                                    Console.WriteLine($"Klass {student.KlassID}: {student.Personnummer} {student.Förnamn} {student.Efternamn}");
                                }

                                Console.WriteLine("\nTryck på valfri tangent för att gå vidare...");
                                Console.ReadKey(true);    
                            }
                            break;
                        case '4':
                            Console.Write("Ange elevens personnummer (YYYYMMDD-XXXX): ");
                            using (var context = new LocalDB())
                            {
                                string? input = Console.ReadLine();
                                var query = from student in context.Studenter
                                            join grade in context.Betyg on student.StudentID equals grade.StudentID
                                            join teacher in context.Personal on grade.PersonalID equals teacher.PersonalID
                                            where student.Personnummer == input
                                            select new
                                            {
                                                Student = student.Personnummer,
                                                grade.KursID,
                                                grade.Resultat,
                                                grade.Datum,
                                                Lärare = teacher.Personnummer
                                            };

                                var students = query.ToList();

                                Console.WriteLine("Student  Kurs  Resultat  Datum  Lärare");

                                foreach (var student in students)
                                {
                                    Console.WriteLine($"{student.Student} {student.KursID} {student.Resultat} {student.Datum} {student.Lärare}");
                                }

                                Console.WriteLine("\nTryck på valfri tangent för att gå vidare...");
                                Console.ReadKey(true);   
                                
                            }
                            break;
                        case '5':
                            using (var context = new LocalDB())
                            {
                                DateTime oneMonthAgo = DateTime.Now.AddMonths(-1);

                                var query = from betyg in context.Betyg
                                            join student in context.Studenter on betyg.StudentID equals student.StudentID
                                            where betyg.Datum >= oneMonthAgo
                                            select new
                                            {
                                                student.Förnamn,
                                                student.Efternamn,
                                                betyg.Resultat,
                                                betyg.Datum,
                                                betyg.KursID
                                            };

                                var pastMonthGrades = query.ToList();

                                Console.Write("\n");
                            
                                foreach (var grade in pastMonthGrades)
                                {
                                    Console.WriteLine($"{grade.Förnamn} {grade.Efternamn} Kurs: {grade.KursID} {grade.Resultat} {grade.Datum}");
                                }

                                Console.WriteLine("\nTryck på valfri tangent för att gå vidare...");
                                Console.ReadKey(true);    

                            }
                            break;       
                        case '6':
                            using (var context = new LocalDB())
                            {
                                var result = context.Betyg
                                    .GroupBy(betyg => betyg.KursID)
                                    .Select(grade => new
                                    {
                                        // Ge varje betyg ett motsvarande numeriskt så som U = 0, G = 1 och VG = 2
                                        KursID = grade.Key,
                                        AverageGrade = grade.Average(betyg => betyg.Resultat == "G" ? 1.0 : (betyg.Resultat == "VG" ? 2.0 : 0.0)),
                                        MinGrade = grade.Min(betyg => betyg.Resultat == "G" ? 1 : (betyg.Resultat == "VG" ? 2 : 0)),
                                        MaxGrade = grade.Max(betyg => betyg.Resultat == "G" ? 1 : (betyg.Resultat == "VG" ? 2 : 0))
                                    })
                                    .ToList();

                                foreach (var statistic in result)
                                {
                                    Console.WriteLine($"Kurs: {statistic.KursID} Min: {statistic.MinGrade} Max: {statistic.MaxGrade} Genomsnitt: {statistic.AverageGrade}");
                                }

                                Console.WriteLine("\nTryck på valfri tangent för att gå vidare...");
                                Console.ReadKey(true);
                            }
                            break;  
                        case '7':
                            using (var context = new LocalDB())
                            {
                                string? firstName = "", lastName = "", socialSecurityNumber = "";

                                while (string.IsNullOrEmpty(firstName))
                                {
                                    Console.WriteLine("Ange förnamn:");
                                    firstName = Console.ReadLine();
                                }

                                while (string.IsNullOrEmpty(lastName))
                                {
                                    Console.WriteLine("Ange efternamn:");
                                    lastName = Console.ReadLine();
                                }

                                while (string.IsNullOrEmpty(socialSecurityNumber))
                                {
                                    Console.WriteLine("Ange personnummer:");
                                    socialSecurityNumber = Console.ReadLine();
                                }

                                var newStudent = new Student
                                {
                                    Förnamn = firstName,
                                    Efternamn = lastName,
                                    Personnummer = socialSecurityNumber
                                };

                                context.Studenter.Add(newStudent);
                                context.SaveChanges();
                            }
                            break;  
                        case '8':
                            using (var context = new LocalDB())
                            {
                                string? firstName = "", lastName = "", socialSecurityNumber = "";
                                decimal? salary = null;
                                DateTime? hiredDate = null;

                                while (string.IsNullOrEmpty(firstName))
                                {
                                    Console.WriteLine("Ange förnamn:");
                                    firstName = Console.ReadLine();
                                }

                                while (string.IsNullOrEmpty(lastName))
                                {
                                    Console.WriteLine("Ange efternamn:");
                                    lastName = Console.ReadLine();
                                }

                                while (string.IsNullOrEmpty(socialSecurityNumber))
                                {
                                    Console.WriteLine("Ange personnummer:");
                                    socialSecurityNumber = Console.ReadLine();
                                }

                                while (salary == null)
                                {
                                    Console.WriteLine("Ange lön:");
                                    salary = Decimal.Parse(Console.ReadLine());
                                }

                                while (hiredDate == null)
                                {
                                    Console.WriteLine("Ange anställningsdatum (yyyy-MM-dd):");
                                    hiredDate = DateTime.Parse(Console.ReadLine());
                                }

                                var newStaff = new Personal
                                {
                                    Förnamn = firstName,
                                    Efternamn = lastName,
                                    Personnummer = socialSecurityNumber,
                                    Lön = (decimal)salary,
                                    Anställningsdatum = (DateTime)hiredDate
                                };

                                context.Personal.Add(newStaff);
                                context.SaveChanges();
                            }
                            break;  
                        case '9':
                            using (var context = new LocalDB())
                            {
                                int employeeID, studentID, courseID;
                                string result = "";

                            
                                Console.Write("Ange elevens personnummer (YYYYMMDD-XXXX): ");
                                string studentInput = Console.ReadLine();

                                var resultStudent = context.Studenter
                                    .Where(student => student.Personnummer == studentInput)                                    
                                    .Select(student => new
                                    {
                                        student.StudentID,
                                        student.Personnummer
                                    })
                                    .ToList();

                                if (resultStudent.Count > 0)
                                {
                                    studentID = resultStudent[0].StudentID;
                                }
                                else
                                {
                                    Console.WriteLine($"Ingen elev med personummret '{studentInput}' hittades.");

                                    Console.WriteLine("\nTryck på valfri tangent för att gå vidare...");
                                    Console.ReadKey(true);     
                                    break;
                                }
                               
                                var resultCourse = context.Kurser
                                    .Where(kurs => kurs.StudentID == studentID)
                                    .Select(kurs => kurs.KursID)
                                    .ToList();

                                if (resultCourse.Count > 0)
                                {
                                    Console.WriteLine("Välj kurs:");

                                    foreach (var kurs in resultCourse)
                                    {
                                        Console.WriteLine(kurs);
                                    }
                                }
                                else
                                {
                                    Console.WriteLine("Eleven har inga kurser.");

                                    Console.WriteLine("\nTryck på valfri tangent för att gå vidare...");
                                    Console.ReadKey(true);     
                                    break;
                                }

                                int.TryParse(Console.ReadLine(), out courseID);

                                resultCourse = context.Kurser
                                    .Where(kurs => kurs.KursID == courseID && kurs.StudentID == studentID)
                                    .Select(kurs => kurs.KursID)
                                    .ToList();


                                if (resultCourse.Count > 0)
                                {
                                    courseID = resultCourse[0];
                                }
                                else
                                {
                                    Console.WriteLine($"'{courseID}' är inte en giltig kurs.");
                                    Console.WriteLine("\nTryck på valfri tangent för att gå vidare...");
                                    Console.ReadKey(true);     
                                    break;
                                }

                                Console.WriteLine("Ange personnummer för läraren som har satt betyget (YYYYMMDD-XXXX): ");
                                string employeeInput = Console.ReadLine();

                                var resultEmployee = context.Personal
                                    .Where(personal => personal.Personnummer == employeeInput)                                    
                                    .Select(personal => new
                                    {
                                        personal.PersonalID,
                                        personal.Personnummer
                                    })
                                    .ToList();

                                if (resultEmployee.Count > 0)
                                {
                                    employeeID = resultEmployee[0].PersonalID;
                                }
                                else
                                {

                                    Console.WriteLine($"Ingen personal med personummret '{employeeInput}' hittades.");

                                    Console.WriteLine("\nTryck på valfri tangent för att gå vidare...");
                                    Console.ReadKey(true);     
                                    break;
                                }

                                while (result.IsNullOrEmpty())
                                {
                                    Console.WriteLine("Välj betyg:\n0) U\n1) G\n2) VG");
                                    option = Console.ReadKey(true).KeyChar;
                                    switch (option)
                                    {
                                        case '0':
                                            result = "U";
                                            break;
                                        case '1':
                                            result = "G";
                                            break;
                                        case '2':
                                            result = "VG";
                                            break;
                                        default:
                                            Console.WriteLine($"'{option}' är inte ett alternativ.");
                                            break;
                                    }
                                }

                                using (var transaction = context.Database.BeginTransaction())
                                {
                                    try
                                    {
                                        var newGrade = new Betyg
                                        {
                                            Resultat = result,
                                            Datum = DateTime.Now,
                                            PersonalID = employeeID,
                                            StudentID = studentID,
                                            KursID = courseID
                                        };

                                        context.Betyg.Add(newGrade);
                                        context.SaveChanges();

                                        transaction.Commit();
                                    }
                                    catch (System.Exception)
                                    {
                                        transaction.Rollback();

                                        Console.WriteLine("Kunde inte sätta betyget, återställer ändringar.");
                                    }
                                }
                            }
                            break;
                        case 'a':
                            using (var context = new LocalDB())
                            {
                                var departments = from personal in context.Personal
                                    join rollerPersonal in context.RollerPersonal on personal.PersonalID equals rollerPersonal.PersonalID
                                    join roller in context.Roller on rollerPersonal.RollID equals roller.RollID
                                    group new { personal, roller } by new { roller.RollNamn } into grouped
                                    select new
                                    {
                                        grouped.Key.RollNamn,
                                        Antal = grouped.Count(),
                                        TotalLön = grouped.Sum(x => x.personal.Lön),
                                        GenomsnittligLön = grouped.Average(x => x.personal.Lön)
                                    };

                                Console.Write("\n");

                                foreach (var department in departments)
                                {
                                    Console.WriteLine($"RollNamn: {department.RollNamn}, Antal: {department.Antal}, Total lön: {department.TotalLön}, Genomsnittlig lön: {department.GenomsnittligLön}");
                                }     

                                Console.WriteLine("\nTryck på valfri tangent för att gå vidare...");
                                Console.ReadKey(true);                                   
                            }
                            break;
                        case 'b':
                            using (var context = new LocalDB())
                            {
                                var courses = from kurser in context.Kurser
                                    group new { kurser } by new { kurser.KursID } into grouped
                                    select new
                                    {
                                        grouped.Key.KursID
                                    };
                                
                                Console.WriteLine("\nAlla kurser: ");

                                foreach (var course in courses)
                                {
                                    Console.WriteLine($"KursID: {course.KursID}");
                                }     

                                Console.WriteLine("\nTryck på valfri tangent för att gå vidare...");
                                Console.ReadKey(true);  

                            }
                            break;
                        case 'q':
                            exit = true;
                            break;         
                        default:
                            Console.WriteLine($"\n'{option}' är inte ett alternativ.");
                            break;
                    }
                }
            }
        }

        static void PrintMenu()
        {
            Console.WriteLine("\nMeny för Skola");
            Console.WriteLine("--------------");
            Console.WriteLine("1) Visa personal.\n2) Visa alla elever.\n3) Visa alla elever i en klass.\n4) Visa alla betyg för en elev.\n5) Visa alla betyg som satts den senaste månaden.\n6) Visa genomsnittligt, högsta och lägsta betyg i alla kurser.\n7) Lägg till ny elev.\n8) Lägg till ny personal.\n9) Sätt betyg på elev.\na) Visa information om avdelningarna.\nb) Visa kurser.\nq) Avsluta.");
        }
    }
}