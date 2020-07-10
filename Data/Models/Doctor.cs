using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace P01_HospitalDatabase.Data.Models
{
    public class Doctor
    {
        public Doctor()
        {
            Visitations = new HashSet<Visitation>();
        }

        [Key]
        public int DoctorId { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; }

        [MaxLength(44)]
        public string Username { get; set; }

        [MaxLength(26)]
        public string Password { get; set; }

        [MaxLength(100)]
        public string Specialty { get; set; }

        public virtual ICollection<Visitation> Visitations { get; set; }
    }
}
