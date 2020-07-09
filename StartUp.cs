using P01_HospitalDatabase.Data;
using P01_HospitalDatabase.Data.Models;
using System;

namespace P01_HospitalDatabase
{
    public class StartUp
    {
        static void Main(string[] args)
        {
            var context = new HospitalContext();

            //context.Database.EnsureDeleted();
            //context.Database.EnsureCreated();

            while (true)
            {
                Console.WriteLine("Hospital System Jp&Co");
                Console.WriteLine("=================================================================");
                Console.WriteLine("За регистринане на нов Доктор натиснете 1");
                Console.WriteLine("За регистринане на нов Пациент натиснете 2");
                Console.WriteLine("За регистринане на нов Медикамент натиснете 3");
                Console.WriteLine("За изход натиснете 9");

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
                    AddNewDoctor(context);
                }

                if (inputChoice == 2)
                {
                    AddNewPatient(context);
                }

                if (inputChoice == 3)
                {
                    AddNewMedicament(context);
                }

                //TODO: Как да обвържа всички таблици в базата данни? Кое ми е главното:
                //- Визитацията ли? Една визитация има доктор има пациент, пациента си има в него диагноза, както си има и лекарство
                // - Пациента ли? Той си има всичко в себе си - Визитации които водят към докторите, диагнози, лекартва

                if (inputChoice == 9)
                {
                    break;
                }

            }
        }

        private static void AddNewMedicament(HospitalContext context)
        {
            Console.WriteLine("Моля въведете име на медикамента");
            string medicamentName = Console.ReadLine();

            context.Medicaments
                .Add(new Medicament { Name = medicamentName });

            context.SaveChanges();

            Console.WriteLine("Вие успешно въведохте нов медикамент в базата данни!");
        }

        private static void AddNewPatient(HospitalContext context)
        {
            Console.WriteLine("Моля въведете първото име на пациента");
            string patientFirstName = Console.ReadLine();
            Console.WriteLine("Моля въведете Фамилия на пациента");
            string patientLastname = Console.ReadLine();
            Console.WriteLine("Моля въведете адрес на пациента");
            string patientAddress = Console.ReadLine();
            Console.WriteLine("Моля въведете Email на пациента");
            string patientEmail = Console.ReadLine();
            bool isHealthInsured;

            while (true)
            {
                Console.WriteLine(@"Здравно осигурен ли е пациента: ""Да""/ ""Не""");
                string insurance = Console.ReadLine().ToLower();

                if (insurance == "да")
                {
                    isHealthInsured = true;
                    break;
                }
                else if (insurance == "не")
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
