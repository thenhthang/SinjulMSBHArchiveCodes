using System;
using DotvvmApplication1.DAL;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Threading.Tasks;
using System.Security.Claims;

namespace DotvvmApplication1.Services
{
    public class UserService
    {
        public async Task<ClaimsIdentity> SignInAsync(string userName, string password)
        {
            using (var dbContext = CreateDbContext())
            {
                using (var userManager = CreateUserManager(dbContext))
                {
                    var user = await userManager.FindAsync(userName, password);
                    if (user != null)
                    {
                        return await userManager.CreateIdentityAsync(user, DefaultAuthenticationTypes.ApplicationCookie);
                    }
                    return null;
                }
            }
        }

        public async Task<IdentityResult> RegisterAsync(string userName, string password)
        {
            using (var dbContext = CreateDbContext())
            {
                using (var userManager = CreateUserManager(dbContext))
                {
                    var user = new IdentityUser { UserName = userName };
                    return await userManager.CreateAsync(user, password);
                }
            }
        }

        private UserManager<IdentityUser, string> CreateUserManager(StudentDbContext dbContext)
        {
            return new UserManager<IdentityUser, string>(new UserStore<IdentityUser>(dbContext))
            {
                UserTokenProvider = new TotpSecurityStampBasedTokenProvider<IdentityUser, string>()
            };
        }

        private StudentDbContext CreateDbContext()
        {
            return new StudentDbContext();
        }
    }
}
