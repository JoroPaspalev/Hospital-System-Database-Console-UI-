using Microsoft.EntityFrameworkCore;
using P01_HospitalDatabase.Data.Models;
using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace P01_HospitalDatabase.Data
{
    public class HospitalContext : DbContext
    {
        public HospitalContext()
        {
        }

        public HospitalContext(DbContextOptions options)
            :base (options)
        {
        }

        public DbSet<Diagnose> Diagnoses { get; set; }

        public DbSet<Medicament> Medicaments { get; set; }

        public DbSet<Patient> Patients { get; set; }

        public DbSet<PatientMedicament> PatientMedicaments { get; set; }

        public DbSet<Visitation> Visitations { get; set; }

        public DbSet<Doctor> Doctors { get; set; }


        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer("server=.;database=HospitalDatabase;Integrated security=true");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<PatientMedicament>(entity =>
            {
                entity.HasKey(x => new { x.PatientId, x.MedicamentId });
            });

            modelBuilder.Entity<Visitation>(entity =>
            {
                entity
                .HasOne(x => x.Patient)
                .WithMany(x => x.Visitations)
                .HasForeignKey(x => x.PatientId);

                entity
                .HasOne(v => v.Doctor)
                .WithMany(d => d.Visitations)
                .HasForeignKey(v => v.DoctorId);

                entity
                .Property(x => x.Date)
                .HasDefaultValueSql("GetDate()");
            });

            modelBuilder.Entity<Diagnose>(entity =>
            {
                entity
                .HasOne(x => x.Patient)
                .WithMany(x => x.Diagnoses)
                .HasForeignKey(x => x.PatientId);
            });

            modelBuilder.Entity<PatientMedicament>(entity =>
            {
                entity
                .HasOne(x => x.Patient)
                .WithMany(x => x.Prescriptions)
                .HasForeignKey(x => x.PatientId);

            });

            modelBuilder.Entity<PatientMedicament>(entity =>
            {
                entity
                .HasOne(x => x.Medicament)
                .WithMany(x => x.Prescriptions)
                .HasForeignKey(x => x.MedicamentId);
            });
        }
    }
}
