using Microsoft.AspNetCore.Identity;

namespace webapi_authorize.Model
{
    public class CustomIdentityUser : IdentityUser
    {
        public CustomIdentityUser()
        {
        }

        public CustomIdentityUser(string userName) : base(userName)
        {
        }
    }
}