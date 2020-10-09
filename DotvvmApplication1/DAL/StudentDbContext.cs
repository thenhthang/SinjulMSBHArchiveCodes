using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using Microsoft.AspNet.Identity.EntityFramework;
using DotvvmApplication1.DAL.Entities;
using System.Data.Entity;

namespace DotvvmApplication1.DAL
{
    public class StudentDbContext : IdentityDbContext
    {
        public StudentDbContext() : base("DefaultConnection") { }
        public DbSet<Student> Students { get; set; }
    }
}
