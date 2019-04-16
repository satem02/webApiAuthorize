using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Logging;
using Microsoft.IdentityModel.Tokens;

namespace InvoicePayment.WebAPI.Controllers {
    [Route ("api/[controller]")]
    public class AuthController : ControllerBase {
        private readonly UserManager<CustomIdentityUser> _userManager;
        private readonly MyOptions _options;

        public AuthController (UserManager<CustomIdentityUser> userManager, IOptions<MyOptions> options) {
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
            _options = options.Value;
        }

        [HttpPost]
        [Route ("login")]
        public async Task<IActionResult> Post ([FromBody] LoginViewModel loginViewModel) {
            var user = await _userManager.FindByNameAsync (loginViewModel.Mail);
            if (user != null && await _userManager.CheckPasswordAsync (user, loginViewModel.Password)) {
                var claims = new [] {
                new Claim (JwtRegisteredClaimNames.Sub, user.UserName),
                new Claim (JwtRegisteredClaimNames.Jti, new Guid ().ToString ()),
                new Claim (ClaimTypes.Name, user.UserName)
                };

                var signingKey = new SymmetricSecurityKey (Encoding.UTF8.GetBytes (_options.Jwt.Key));

                var token = new JwtSecurityToken (
                    issuer: _options.Jwt.Issuer,
                    audience: _options.Jwt.Audience,
                    claims: claims,
                    expires: DateTime.UtcNow.AddHours (_options.Jwt.ExpireHours),
                    signingCredentials: new SigningCredentials (signingKey, SecurityAlgorithms.HmacSha256)
                );
                IdentityModelEventSource.ShowPII = true;
                return Ok (new {
                    token = new JwtSecurityTokenHandler ().WriteToken (token),
                        expiration = token.ValidTo
                });
            }
            return Unauthorized ();
        }
    }
}