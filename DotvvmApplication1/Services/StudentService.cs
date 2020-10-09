using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Data.Entity;
using DotvvmApplication1.Models;
using DotvvmApplication1.DAL;
using DotvvmApplication1.DAL.Entities;


namespace DotvvmApplication1.Services
{
    public class StudentService
    {
        public async Task<List<StudentListModel>> GetAllStudentsAsync()
        {
            using (var dbContext = CreateDbContext())
            {
                return await dbContext.Students.Select(
                    s => new StudentListModel
                    {
                        Id = s.Id,
                        FirstName = s.FirstName,
                        LastName = s.LastName
                    }
                    ).ToListAsync();
            }
        }

        public async Task<StudentDetailModel> GetStudentByIdAsync(int studentId)
        {
            using (var dbContext = CreateDbContext())
            {
                return await dbContext.Students.Select(
                    s => new StudentDetailModel
                    {
                        Id = s.Id,
                        FirstName = s.FirstName,
                        LastName = s.LastName,
                        About = s.About,
                        EnrollmentDate = s.EnrollmentDate
                    })
                    .FirstOrDefaultAsync(s => s.Id == studentId);
            }
        }

        public async Task UpdateStudentAsync(StudentDetailModel student)
        {
            using (var dbContext = CreateDbContext())
            {
                var entity = await dbContext.Students.FirstOrDefaultAsync(s => s.Id == student.Id);

                entity.FirstName = student.FirstName;
                entity.LastName = student.LastName;
                entity.About = student.About;
                entity.EnrollmentDate = student.EnrollmentDate;
                
                await dbContext.SaveChangesAsync();
            }
        }

        public async Task InsertStudentAsync(StudentDetailModel student)
        {
            using (var dbContext = CreateDbContext())
            {
                var entity = new Student()
                {
                    FirstName = student.FirstName,
                    LastName = student.LastName,
                    About = student.About,
                    EnrollmentDate = student.EnrollmentDate
                };

                dbContext.Students.Add(entity);
                await dbContext.SaveChangesAsync();
            }
        }

        public async Task DeleteStudentAsync(int studentId)
        {
            using(var dbContext = CreateDbContext())
            {
                var entity = new Student()
                {
                    Id = studentId
                };
                dbContext.Students.Attach(entity);
                dbContext.Students.Remove(entity);
                await dbContext.SaveChangesAsync();
            }
        }

        protected StudentDbContext CreateDbContext()
        {
            return new StudentDbContext();
        }

    }
}
