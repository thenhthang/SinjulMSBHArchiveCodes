using System;
using System.ComponentModel.DataAnnotations.Schema;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace SabayeSahar.Models
{
    public class Order : BaseEntity
    {
        /// <summary>
        /// زمان ثبت نام
        /// </summary>
        public DateTime DateTime { get; set; }

        /// <summary>
        /// هنرجوی ثبت نام شده
        /// </summary>
        public virtual Student Student { get; private set; }
        private int _studentId;    

        /// <summary>
        /// کد پیگیری دوره ثبت نام شده
        /// </summary>
        public string TrackingCode { get; private set; }

        /// <summary>
        /// مبلغ پرداخت شده
        /// </summary>
        [Column(TypeName = "decimal(18,2)")]
        public decimal AmountPayed { get; set; }

        /// <summary>
        /// وضعیت پرداخت
        /// </summary>
        public bool IsPayed { get; set; }

        public string Authority { get; set; }

        public Order(Student student, int payTypeId, decimal amountPayed)
        {
            DateTime = DateTime.Now;
            Student = student ?? throw new ArgumentNullException(nameof(student));
            AmountPayed = amountPayed;
            TrackingCode = "123456";
        }

        public Order()
        {
        }

    }
    public class OrderConfiguration : IEntityTypeConfiguration<Order>
    {
        public void Configure(EntityTypeBuilder<Order> builder)
        {
            builder.ToTable(nameof(Order), "dbo");

            builder.Metadata.FindNavigation(nameof(Order.Student))
            .SetPropertyAccessMode(PropertyAccessMode.FieldDuringConstruction);

            builder.HasOne(o => o.Student)
                .WithMany()
                .HasForeignKey("StudentId")
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired()
            ;

            builder.Property(q => q.AmountPayed)
                .IsRequired()
                .HasColumnType("decimal(18,2)")
                .HasDefaultValue(0)
            ;
        }
    }
}
