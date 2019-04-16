using Microsoft.AspNetCore.Identity;

namespace InvoicePayment.WebAPI.Controllers
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