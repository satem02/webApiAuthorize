using System.Security.Claims;
using webapi_authorize.Model;

namespace webapi_authorize.Services.Concrete
{
    public interface IUserSessionService
    {
        string GetPhoneNumber();
        string GetRole();
        string GetEmail();
        string GetUserName();
        string GetUserId();
        Claim[] GetClaims(CustomIdentityUser user);
    }
}