using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using webapi_authorize.Model;
using webapi_authorize.Services.Abstract;

namespace webapi_authorize.Services.Concrete
{
    public class UserSessionService : IUserSessionService
    {
        private HttpContext _httpContext;
        private readonly UserManager<CustomIdentityUser> _userManager;
        private readonly string PhoneNumber = "PhoneNumber";

        public UserSessionService(IHttpContextAccessor httpContextAccessor, UserManager<CustomIdentityUser> userManager)
        {
            _httpContext = httpContextAccessor.HttpContext;
            _userManager = userManager;
        }
        public string GetPhoneNumber()
        {
            return GetClaimValue(PhoneNumber);
        }
        public string GetRole()
        {
            return GetClaimValue(ClaimTypes.Role);
        }
        public string GetEmail()
        {
            return GetClaimValue(ClaimTypes.Email);
        }
        public string GetUserName()
        {
            return GetClaimValue(ClaimTypes.Name);
        }
        public string GetUserId()
        {
            return GetClaimValue(ClaimTypes.NameIdentifier);
        }
        private string GetClaimValue(string type)
        {
            var user = _httpContext.User.Identities.FirstOrDefault();
            if (user == null)
                return string.Empty;

            var claim = user.Claims.FirstOrDefault(x => x.Type == type);

            if (claim == null)
                return string.Empty;

            return claim.Value;

        }

        public Claim[] GetClaims(CustomIdentityUser user)
        {

            var role = _userManager.GetRolesAsync(user).Result;
            var claims = new[]{
                    new Claim (ClaimTypes.NameIdentifier,user.Id),
                    new Claim (ClaimTypes.Email,user.Email),
                    new Claim (PhoneNumber,user.PhoneNumber),
                    new Claim (ClaimTypes.Name,user.UserName),
                    new Claim (ClaimTypes.Role,role.FirstOrDefault())
            };

            return claims;
        }
    }
}