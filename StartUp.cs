using P01_HospitalDatabase.Data;
using P01_HospitalDatabase.Data.Models;
using System;
using System.Linq;

namespace P01_HospitalDatabase
{
    public class StartUp
    {
        static void Main(string[] args)
        {
            var context = new HospitalContext();

            context.Database.EnsureDeleted();
            context.Database.EnsureCreated();

            while (true)
            {
                DoctorLogIn(context);
            }
        }

        private static void DoctorLogIn(HospitalContext context)
        {
            Console.WriteLine("=======================================================");
            Console.WriteLine("||                                                   ||");
            Console.WriteLine("============= Вход за доктори в сисметата =============");
            Console.WriteLine("||                                                   ||");
            Console.WriteLine("=======================================================");
            Console.Write("Моля въведете Username: ");
            string doctorUsername = Console.ReadLine();
            Console.Write("Въведете парола: ");
            string doctorPass = Console.ReadLine();
            Console.WriteLine();

            var currentDoctor = context.Doctors
                 .SingleOrDefault(d => d.Username == doctorUsername && d.Password == doctorPass);

            if (currentDoctor == null)
            {
                Console.WriteLine("Грешно потребителско име или парола!");
                return;
            }
            else
            {
                while (true)
                {
                    WelcomeInfo();

                    int inputChoice;

                    bool isInputCorrect = int.TryParse(Console.ReadLine(), out inputChoice);

                    if (!isInputCorrect)
                    {
                        Console.WriteLine("<==== Несъществуваща позиция ====>");
                        Console.WriteLine();
                        continue;
                    }

                    if (inputChoice == 1)
                    {
                        while (true)
                        {
                            Patient currentPatient = GetPatient(context);

                            if (currentPatient == null)
                            {
                                Console.WriteLine("Не е открит пациент с това име. Ще бъде създаден нов пациент!");
                                currentPatient = AddNewPatient(context, currentPatient);
                            }

                            //имаме зараден пациент с който да работим

                            // Мога да създам Visitation вече
                            AddVisitation(context, currentDoctor, currentPatient);

                            //Търси лекарство по име
                            Medicament currentMedicament = GetMedicament(context);

                            //Имаме създаден и зареден обекта Лекарство
                            // Сега трябва да го добавим към този обект с този доктор и тази визитация
                            // Създавам за целта нов PatientMedicament

                            AddMedicamentToPatient(context, currentPatient, currentMedicament);

                            Diagnose currentDiagnose = SetDiagnose(context, currentPatient);

                            PrintPatientDiagnoseAndMedicine(currentPatient, currentMedicament, currentDiagnose);
                        }
                    }

                    if (inputChoice == 9)
                    {
                        break;
                    }
                }
            }
        }

        private static void PrintPatientDiagnoseAndMedicine(Patient currentPatient, Medicament currentMedicament, Diagnose currentDiagnose)
        {
            Console.WriteLine($"Вие успешно зададохте");
            Console.WriteLine($"                    Диагноза: {currentDiagnose.Name}, ");
            Console.WriteLine($"                    на Пациент: {currentPatient.FirstName} {currentPatient.LastName}");
            Console.WriteLine($"                    и лекарство: {currentMedicament.Name}");
            Console.WriteLine();
        }

        private static void WelcomeInfo()
        {            
            Console.WriteLine("=================================================================");
            Console.WriteLine("||    Hospital System Jp&Co - Успешно влизане в системата      ||");
            Console.WriteLine("=================================================================");
            Console.WriteLine("За търсене на пациент натиснете 1");
            Console.WriteLine("За изход натиснете 9");
        }

        private static Diagnose SetDiagnose(HospitalContext context, Patient currentPatient)
        {
            Console.Write("Въведете име на диагнозата: ");
            string diagnoseName = Console.ReadLine();

            var currentDiagnose = context.Diagnoses
                 .FirstOrDefault(x => x.Name == diagnoseName);

            if (currentDiagnose == null)
            {
                currentDiagnose = AddDiagnose(context, currentPatient, diagnoseName);
            }

            return currentDiagnose;
        }

        private static Diagnose AddDiagnose(HospitalContext context, Patient currentPatient, string diagnoseName)
        {
            string diagnoseComment;
            while (true)
            {
                Console.Write(@"Не е открита диагноза по зададените критерии. Ще бъде създадена нова с това име. Желаете ли да добавите коментар към новата диагноза? ""Yes""/""No""");
                diagnoseComment = Console.ReadLine().ToLower();

                if (diagnoseComment == "yes")
                {
                    diagnoseComment = Console.ReadLine();
                    break;
                }
                else if (diagnoseComment == "no")
                {
                    diagnoseComment = " ";
                    break;
                }
                else
                {
                    continue;
                }
            }

            context.Diagnoses
            .Add(new Diagnose { Name = diagnoseName, Comments = diagnoseComment, PatientId = currentPatient.PatientId });

            context.SaveChanges();

            Diagnose addedDiagnose = context.Diagnoses.First(d => d.Name == diagnoseName);

            Console.WriteLine("Успешно зададена диагноза!");

            return addedDiagnose;
        }

        private static void AddMedicamentToPatient(HospitalContext context, Patient currentPatient, Medicament currentMedicament)
        {
            context.PatientMedicaments
                .Add(new PatientMedicament { PatientId = currentPatient.PatientId, MedicamentId = currentMedicament.MedicamentId });

            context.SaveChanges();
            Console.WriteLine("Успешно регистрирано лекарство на текущ пациент!");
        }

        private static void AddVisitation(HospitalContext context, Doctor currentDoctor, Patient currentPatient)
        {
            string visitationComment = AddVisitationComment();

            var currentVisitation = context.Visitations
                .Add(new Visitation { DoctorId = currentDoctor.DoctorId, PatientId = currentPatient.PatientId, Comments = visitationComment });

            context.SaveChanges();
            Console.WriteLine("Успешно създадена Визитация!");
        }

        private static Patient GetPatient(HospitalContext context)
        {
            Console.Write("Въведете Email на пациента: ");
            string patientEmail = Console.ReadLine();

            Patient currentPatient = context.Patients
                .SingleOrDefault(p => p.Email == patientEmail);
            return currentPatient;
        }

        private static Medicament GetMedicament(HospitalContext context)
        {
            Console.Write("Въведете име на лекарството, което ще предпишете на пациента: ");
            string medicamentName = Console.ReadLine();

            var currentMedicament = context.Medicaments
                .SingleOrDefault(m => m.Name == medicamentName);

            // Ако го има зареди го и ми го върни като обект, ако не го създай и ми го върни
            if (currentMedicament == null)
            {
                Console.WriteLine("Не е открито лекарство въведените критерии. Ще бъде създадено ново!");
                currentMedicament = AddNewMedicament(context, medicamentName);
            }

            return currentMedicament;
        }

        private static string AddVisitationComment()
        {
            Console.Write("Създава се Визитация, въведете коментар: ");
            string visitationComment = Console.ReadLine();
            
            return visitationComment;
        }

        private static Medicament AddNewMedicament(HospitalContext context, string medicamentName)
        {       
            context.Medicaments
                .Add(new Medicament { Name = medicamentName });

            context.SaveChanges();

            Console.WriteLine("Вие успешно въведохте ново лекарство в базата данни!");

            Medicament addedMedicament = context.Medicaments.First(m => m.Name == medicamentName);

            return addedMedicament;
        }

        private static Patient AddNewPatient(HospitalContext context, Patient currentPatient)
        {
            Console.WriteLine("Моля въведете първото име на пациента");
            string patientFirstName = Console.ReadLine();
            Console.WriteLine("Моля въведете Фамилия на пациента");
            string patientLastname = Console.ReadLine();
            Console.WriteLine("Моля въведете адрес на пациента");
            string patientAddress = Console.ReadLine();
            string patientEmail;

            while (true)
            {
                Console.WriteLine("Моля въведете Email на пациента");
                patientEmail = Console.ReadLine();
                //Провери дали има пациент с такъв мейл

                var dublicateEmail = context.Patients
                    .FirstOrDefault(p => p.Email == patientEmail);

                if (dublicateEmail != null)
                {
                    Console.WriteLine("Вече има създаден пациент с този Email!");
                }
                else
                {
                    break;
                }
            }

            bool isHealthInsured;

            while (true)
            {
                Console.WriteLine(@"Здравно осигурен ли е пациента: ""Yes""/ ""No""");
                string insurance = Console.ReadLine().ToLower();

                if (insurance == "yes")
                {
                    isHealthInsured = true;
                    break;
                }
                else if (insurance == "no")
                {
                    isHealthInsured = false;
                    break;
                }
            }

            context.Patients
                .Add(new Patient
                {
                    FirstName = patientFirstName,
                    LastName = patientLastname,
                    Address = patientAddress,
                    Email = patientEmail,
                    HasInsurance = isHealthInsured
                });
            context.SaveChanges();
            Console.WriteLine("Вие успешно създадохте нов пациент!");
            currentPatient = context.Patients.First(p => p.Email == patientEmail);
            return currentPatient;
        }

        private static void AddNewDoctor(HospitalContext context)
        {
            Console.WriteLine("Моля въведете име и фамилия на Доктора");
            string doctorName = Console.ReadLine();
            Console.WriteLine("Моля въведете специалността на доктора");
            string doctorSpecialty = Console.ReadLine();

            context.Doctors
                .Add(new Doctor { Name = doctorName, Specialty = doctorSpecialty });

            context.SaveChanges();
            Console.WriteLine("Въведеният от вас Доктор е запаметен в базата данни!");
        }
    }
}
