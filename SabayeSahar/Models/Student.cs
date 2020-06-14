using System;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace SabayeSahar.Models
{
    /// <summary>
    /// هنرجو
    /// </summary>
    public class Student : BaseEntity
    {
        /// <summary>
        /// نام هنرجو
        /// </summary>
        public string FirstName { get; set; }

        /// <summary>
        /// نام خانوادگی هنرجو
        /// </summary>
        public string LastName { get; set; }

        /// <summary>
        /// تاریخ تولد
        /// </summary>
        public DateTime BirthDate { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// شماره تماس هنرجو
        /// </summary>
        public string PhoneNumber { get; set; }

        /// <summary>
        /// نام کامل هنرجو
        /// </summary>
        public string FullName => FirstName + " " + LastName;

        public Student(string firstName, string lastName, string phoneNumber)
        {
            FirstName = firstName ?? throw new ArgumentNullException(nameof(firstName));
            LastName = lastName ?? throw new ArgumentNullException(nameof(lastName));
            PhoneNumber = phoneNumber ?? throw new ArgumentNullException(nameof(phoneNumber));
        }
        public Student()
        {

        }
    }
    public class StudentConfiguration : IEntityTypeConfiguration<Student>
    {
        public void Configure(EntityTypeBuilder<Student> builder)
        {
            builder.ToTable(nameof(Student), "dbo");

            builder.Property(q => q.FirstName).HasMaxLength(70).IsRequired();
            builder.Property(q => q.LastName).HasMaxLength(70).IsRequired();
        }
    }
}
