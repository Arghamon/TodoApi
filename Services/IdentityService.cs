using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using TodoApi.Contracts.V1.Requests;
using TodoApi.Domains;
using TodoApi.Options;

namespace TodoApi.Services
{
    public class IdentityService : IIdentityService
    {

        private readonly UserManager<IdentityUser> _userManager;
        private readonly JwtSettings _jwtSettings;

        public IdentityService(UserManager<IdentityUser> userManager, JwtSettings jwtSettings)
        {
            _userManager = userManager;
            _jwtSettings = jwtSettings;
        }

        private AuthenticationResult GenerateAuthenticationResult(IdentityUser user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_jwtSettings.Secret);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(JwtRegisteredClaimNames.Sub, user.Email),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                    new Claim(JwtRegisteredClaimNames.Email, user.Email),
                    new Claim("id", user.Id)
                }),

                Expires = DateTime.UtcNow.AddHours(2),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);

            return new AuthenticationResult
            {
                Success = true,
                Token = tokenHandler.WriteToken(token)
            };
        }


        public async Task<AuthenticationResult> LoginAsync(UserLoginRequest request)
        {
            var user = await _userManager.FindByEmailAsync(request.email);

            if (user == null)
            {
                return new AuthenticationResult
                {
                    Success = false,
                    Errors = new[] { "Email/Password combination doesn't exist" }
                };
            }

            var userHasValidPassword = await _userManager.CheckPasswordAsync(user, request.password);

            if (!userHasValidPassword)
            {
                return new AuthenticationResult
                {
                    Success = false,
                    Errors = new[] { "Email/Password combination doesn't exist" }
                };
            }

            return GenerateAuthenticationResult(user);
        }

        public async Task<AuthenticationResult> RegisterAsync(UserRegisterRequest request)
        {
            var user = await _userManager.FindByEmailAsync(request.email);

            if (user != null)
            {
                return new AuthenticationResult
                {
                    Success = false,
                    Errors = new[] { "User with this email address already exists" }
                };
            }

            var newUser = new IdentityUser
            {
                Email = request.email,
                UserName = request.email,
            };

            var createdUser = await _userManager.CreateAsync(newUser, request.password);

            if (!createdUser.Succeeded)
            {
                return new AuthenticationResult
                {
                    Success = false,
                    Errors = createdUser.Errors.Select(error => error.Description)
                };
            }

            return GenerateAuthenticationResult(newUser);

        }

    }
}
