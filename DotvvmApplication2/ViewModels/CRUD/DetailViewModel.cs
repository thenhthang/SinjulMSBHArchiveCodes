using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using DotVVM.Framework.ViewModel;
using DotVVM.Framework.Runtime.Filters;
using DotvvmApplication2.Models;
using DotvvmApplication2.Services;


namespace DotvvmApplication2.ViewModels.CRUD
{
    public class DetailViewModel : MasterPageViewModel
    {
        private readonly StudentService studentService;

        public DetailViewModel(StudentService studentService)
        {
            this.studentService = studentService;
        }

        public StudentDetailModel Student { get; set; }

        public override async Task PreRender()
        {
            int id = Convert.ToInt32(Context.Parameters["Id"]);
            Student = await studentService.GetStudentByIdAsync(id);
            await base.PreRender();
        }
		[Authorize]
        public async Task DeleteStudent()
        {
            await studentService.DeleteStudentAsync(Student.Id);
            Context.RedirectToRoutePermanent("Default", replaceInHistory: true);
        }
    }
}
