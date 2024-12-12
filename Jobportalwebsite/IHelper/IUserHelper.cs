using Jobportalwebsite.Models;
using Jobportalwebsite.Viewmodel;

namespace Jobportalwebsite.IHelper
{
    public interface IUserHelper
    {
        Task<ApplicationUser> CreateUserByAsync(RegistrationViewModel applicationUserViewModel);
        Task<ApplicationUser> CreateUserByAsync(RegistrationViewModel model, string role);
    }
}
