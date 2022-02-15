namespace Rockstodons.Web.Controllers
{
    using System;
    using System.IdentityModel.Tokens.Jwt;
    using System.Security.Claims;
    using System.Text;
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Options;
    using Microsoft.IdentityModel.Tokens;
    using Rockstodons.Data.Models;
    using Rockstodons.Web.DTOs.Identity;
    using Rockstodons.Web.Infrastructure;

    public class IdentityController : ApiController
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly ApplicationSettings applicationSettings;

        public IdentityController(UserManager<ApplicationUser> userManager, IOptions<ApplicationSettings> applicationSettings)
        {
            this.userManager = userManager;
            this.applicationSettings = applicationSettings.Value;
        }

        [HttpPost]
        [AllowAnonymous]
        [Route(nameof(Register))]
        public async Task<ActionResult> Register(RegisterRequestModel registerRequestModel) 
        {
            var user = new ApplicationUser
            {
                Email = registerRequestModel.Email,
                UserName = registerRequestModel.UserName,
            };

            var result = await this.userManager.CreateAsync(user, registerRequestModel.Password);

            if (result.Succeeded)
            {
                return this.Ok();
            }

            return this.BadRequest(result.Errors);
        }

        [HttpPost]
        [AllowAnonymous]
        [Route(nameof(Login))]
        public async Task<ActionResult<string>> Login(LoginRequestModel loginRequestModel)
        {
            var user = await this.userManager.FindByNameAsync(loginRequestModel.UserName);

            if (user == null)
            {
                return this.Unauthorized();
            }

            var isPasswordValid = await this.userManager.CheckPasswordAsync(user, loginRequestModel.Password);

            if (!isPasswordValid)
            {
                return this.Unauthorized();
            }

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(this.applicationSettings.Secret);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, user.Id.ToString()),
                }),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            var encryptedToken = tokenHandler.WriteToken(token);

            return encryptedToken;
        }
    }
}
