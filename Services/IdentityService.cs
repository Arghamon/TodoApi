using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using TodoApi.Contracts.V1.Requests;
using TodoApi.Data;
using TodoApi.Domains;
using TodoApi.Options;

namespace TodoApi.Services
{
    public class IdentityService : IIdentityService
    {

        private readonly UserManager<IdentityUser> _userManager;
        private readonly JwtSettings _jwtSettings;
        private readonly TokenValidationParameters _tokenValidationParameters;
        private readonly DataContext _context;

        public IdentityService(UserManager<IdentityUser> userManager, JwtSettings jwtSettings, TokenValidationParameters tokenValidationParameters, DataContext context)
        {
            _userManager = userManager;
            _jwtSettings = jwtSettings;
            _tokenValidationParameters = tokenValidationParameters;
            _context = context;
        }

        private async Task<AuthenticationResult> GenerateAuthenticationResult(IdentityUser user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_jwtSettings.Secret);

            Console.WriteLine(DateTime.UtcNow.Add(_jwtSettings.TokenLifeTime));

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(JwtRegisteredClaimNames.Sub, user.Email),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                    new Claim(JwtRegisteredClaimNames.Email, user.Email),
                    new Claim("id", user.Id)
                }),

                Expires = DateTime.UtcNow.Add(_jwtSettings.TokenLifeTime),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);

            var refreshToken = new RefreshToken
            {
                JwtId = token.Id,
                UserId = user.Id,
                CreationDate = DateTime.UtcNow,
                ExpiryDate = DateTime.UtcNow.AddMonths(6),
            };

            await _context.RefreshTokens.AddAsync(refreshToken);
            await _context.SaveChangesAsync();

            return new AuthenticationResult
            {
                Success = true,
                Token = tokenHandler.WriteToken(token),
                RefreshToken = refreshToken.Token,
            };
        }

        private ClaimsPrincipal GetClaimsPrincipal(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();

            try
            {
                var principal = tokenHandler.ValidateToken(token, _tokenValidationParameters, out var validatedToken);
                if (!IsJwtWithValidSecurityAlgorithm(validatedToken))
                {
                    return null;
                }

                return principal;
            }
            catch
            {
                return null;
            }
        }

        private bool IsJwtWithValidSecurityAlgorithm(SecurityToken validatedToken)
        {
            return (validatedToken is JwtSecurityToken jwtSecurityToken &&
                jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256,
                StringComparison.CurrentCultureIgnoreCase));
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

            return await GenerateAuthenticationResult(user);
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

            return await GenerateAuthenticationResult(newUser);

        }

        public async Task<AuthenticationResult> RefreshAsync(RefreshTokenRequest request)
        {
            var validatedToken = GetClaimsPrincipal(request.Token);
            if (validatedToken == null)
            {
                return new AuthenticationResult { Errors = new[] { "Invalid Token" } };
            }

            var expiryDateUnix = long.Parse(validatedToken.Claims.Single(x => x.Type == JwtRegisteredClaimNames.Exp).Value);
            var expiryDateUtc = new DateTime(1970, 1, 1, 0, 0, 0).AddSeconds(expiryDateUnix);

            if (expiryDateUtc > DateTime.UtcNow)
            {
                return new AuthenticationResult { Errors = new[] { "This token hasn't expired yet" } };
            }

            var jti = validatedToken.Claims.Single(x => x.Type == JwtRegisteredClaimNames.Jti).Value;
            var storedRefreshToken = await _context.RefreshTokens.SingleOrDefaultAsync(x => x.Token == request.RefreshToken);

            if (storedRefreshToken == null)
            {
                return new AuthenticationResult { Errors = new[] { "This refresh token doesn't exist" } };
            }

            if (DateTime.UtcNow > storedRefreshToken.ExpiryDate)
            {
                return new AuthenticationResult { Errors = new[] { "This refresh token has expired" } };
            }

            if (storedRefreshToken.Invalidated)
            {
                return new AuthenticationResult { Errors = new[] { "This refresh token has been invalidated" } };
            }

            if (storedRefreshToken.Used)
            {
                return new AuthenticationResult { Errors = new[] { "This refresh token has been used" } };
            }

            if (storedRefreshToken.JwtId != jti)
            {
                return new AuthenticationResult { Errors = new[] { "This refresh token doesn't march JWT" } };
            }

            storedRefreshToken.Used = true;
            _context.RefreshTokens.Update(storedRefreshToken);
            await _context.SaveChangesAsync();

            var user = await _userManager.FindByIdAsync(validatedToken.Claims.Single(x => x.Type == "id").Value);
            return await GenerateAuthenticationResult(user);

        }


    }
}
