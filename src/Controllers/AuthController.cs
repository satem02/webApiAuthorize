using System;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using webapi_authorize.Model;
using webapi_authorize.Services.Abstract;

namespace webapi_authorize.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [ApiConventionType(typeof(DefaultApiConventions))]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<CustomIdentityUser> _userManager;
        private readonly IUserSessionService _userSessionService;
        private readonly MyOptions _options;

        // public AuthController(UserManager<CustomIdentityUser> userManager, IOptions<MyOptions> options, IUserSessionService userSessionService)
        // {
        //     _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
        //     _userSessionService = userSessionService;
        //     _options = options.Value;
        // }
        public AuthController(IOptions<MyOptions> options, IUserSessionService userSessionService)
        {
            _options = options.Value;
            _userSessionService = userSessionService;

        }
        [HttpPost]
        [Route("Login")]
        public async Task<IActionResult> Post([FromBody] LoginViewModel loginViewModel)
        {
            var user = await _userManager.FindByNameAsync(loginViewModel.UserName);
            if (user != null && await _userManager.CheckPasswordAsync(user, loginViewModel.Password))
            {
                var claims = _userSessionService.GetClaims(user);

                var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_options.JwtOptions.Key));

                var token = new JwtSecurityToken(
                    issuer: _options.JwtOptions.Issuer,
                    audience: _options.JwtOptions.Audience,
                    claims: claims,
                    expires: DateTime.UtcNow.AddHours(_options.JwtOptions.ExpireHours),
                    signingCredentials: new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256)
                );

                return Ok(new
                {
                    token = new JwtSecurityTokenHandler().WriteToken(token),
                    expiration = token.ValidTo
                });
            }
            return Unauthorized();
        }


        [HttpPost]
        [Route("DummyLogin")]
        public async Task<IActionResult> Dummy([FromBody] LoginViewModel loginViewModel)
        {
            var user = new CustomIdentityUser();
            user.UserName = loginViewModel.UserName;

            var claims = _userSessionService.GetClaims(user);

            var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_options.JwtOptions.Key));

            var jwtToken = new JwtSecurityToken(
                issuer: _options.JwtOptions.Issuer,
                audience: _options.JwtOptions.Audience,
                claims: claims,
                expires: DateTime.UtcNow.AddHours(_options.JwtOptions.ExpireHours),
                signingCredentials: new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256)
            );
            return Ok(new
            {
                token = new JwtSecurityTokenHandler().WriteToken(jwtToken),
                tokenType = "Bearer",
                expiration = jwtToken.ValidTo
            });

        }
    }
}