using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using DotVVM.Framework.ViewModel;
using DotVVM.Framework.Hosting;
using DotVVM.Framework.ViewModel.Validation;
using Microsoft.AspNet.Identity;
using System.ComponentModel.DataAnnotations;
using DotvvmApplication1.Services;
using DotvvmApplication1.Resources;

namespace DotvvmApplication1.ViewModels.Authentication
{
    public class SignInViewModel : MasterPageViewModel
    {
        private readonly UserService userService;
        
        [Required]
        public string UserName { get; set; }

        [Required]
        [Bind(Direction.ClientToServer)]
        public string Password { get; set; }

        public string ReturnUrl { get; set; }
        
        public SignInViewModel(UserService userService)
        {
            this.userService = userService;
        }

        public override Task Load()
        {
            ReturnUrl = Context.GetOwinContext().Request.Query["ReturnUrl"];
            return base.Load();
        }

        public async Task SignIn()
        {
            Context.FailOnInvalidModelState();

            var identity = await userService.SignInAsync(UserName, Password);
            if (identity == null)
            {
                Context.ModelState.Errors.Add(new ViewModelValidationError{ErrorMessage = "Incorrect login", PropertyPath = nameof(Password) });
                Context.FailOnInvalidModelState();
            }
            else
            {
                var authenticationManager = Context.GetAuthentication();
                authenticationManager.SignIn(identity);

                if (ReturnUrl == null)
                {
                    Context.RedirectToRoute("Default");
                }
                else
                {
                    Context.RedirectToUrl(ReturnUrl);
                }
            }
        }
    }
}
