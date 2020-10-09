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
    public class RegisterViewModel : MasterPageViewModel, IValidatableObject
    {
        private readonly UserService userService;


        [Required]
        public string UserName { get; set; }

        [Required]
        public string Password { get; set; }
        [Required]
        public string ConfirmPassword { get; set; }

        public RegisterViewModel(UserService userService)
        {
            this.userService = userService;
        }
        

        public async Task Register()
        {

            var identityResult = await userService.RegisterAsync(UserName, Password);
            if (identityResult.Succeeded)
            {
                await SignIn();
            }
            else
            {
                var modelErrors = ConvertIdentityErrorsToModelErrors(identityResult);
                Context.ModelState.Errors.AddRange(modelErrors);
                Context.FailOnInvalidModelState();
            }

            Context.RedirectToRoute("Default");
        }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (Password != ConfirmPassword)
            {
                yield return new ValidationResult(Texts.Error_PasswordsMatch, new[] { nameof(ConfirmPassword) });
            }
        }

        private async Task SignIn()
        {
            var claimsIdentity = await userService.SignInAsync(UserName, Password);
            Context.GetAuthentication().SignIn(claimsIdentity);
        }

        private IEnumerable<ViewModelValidationError> ConvertIdentityErrorsToModelErrors(IdentityResult identityResult)
        {
            return identityResult.Errors.Select(identityError => new ViewModelValidationError
            {
                ErrorMessage = identityError,
                PropertyPath = nameof(UserName)
            });
        }
    }
}
