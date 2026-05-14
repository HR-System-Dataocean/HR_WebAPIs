using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.Configuration;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using VenusHR.Core.Login;
using VenusHR.Core.Master;
using VenusHR.Infrastructure.Presistence;
using Microsoft.EntityFrameworkCore;

namespace VenusHR.Infrastructure.Services
{
    public interface IJwtService
    {
        string GenerateToken(Hrs_Employees employee);
        ClaimsPrincipal? ValidateToken(string token);
        Task RevokeTokenAsync(string token, string? reason = null);
    }

    public class JwtService : IJwtService
    {
        private readonly string _secretKey;
        private readonly string _issuer;
        private readonly string _audience;
        private readonly int _expiryMinutes;
        private readonly ApplicationDBContext _context;

        public JwtService(IConfiguration configuration, ApplicationDBContext context)
        {
            _secretKey = configuration["JwtSettings:SecretKey"] ?? "YourDefaultSecretKeyThatShouldBeAtLeast32CharactersLong!";
            _issuer = configuration["JwtSettings:Issuer"] ?? "VenusHR";
            _audience = configuration["JwtSettings:Audience"] ?? "VenusHRUsers";
            _expiryMinutes = int.Parse(configuration["JwtSettings:ExpiryMinutes"] ?? "60");
            _context = context;
        }

        public string GenerateToken(Hrs_Employees employee)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_secretKey);

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, employee.Code?.ToString() ?? ""),
                new Claim(ClaimTypes.Name, employee.ArbName ?? ""),
                new Claim(ClaimTypes.Email, employee.E_Mail ?? ""),
                new Claim("EmployeeId", employee.id.ToString()),
                new Claim("CompanyId", employee.CompanyId.ToString()),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddMinutes(_expiryMinutes),
                Issuer = _issuer,
                Audience = _audience,
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        public async Task RevokeTokenAsync(string token, string? reason = null)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var jwtToken = tokenHandler.ReadJwtToken(token);
            
            var revokedToken = new RevokedToken
            {
                Token = token,
                RevokedAt = DateTime.UtcNow,
                ExpiresAt = jwtToken.ValidTo,
                Reason = reason ?? "User logout"
            };
            
            _context.RevokedTokens.Add(revokedToken);
            await _context.SaveChangesAsync();
        }

        public ClaimsPrincipal? ValidateToken(string token)
        {
            try
            {
                // Check if token is revoked
                var isRevoked = _context.RevokedTokens
                    .Any(rt => rt.Token == token && rt.ExpiresAt > DateTime.UtcNow);
                
                if (isRevoked)
                {
                    return null;
                }

                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.ASCII.GetBytes(_secretKey);

                var validationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = true,
                    ValidIssuer = _issuer,
                    ValidateAudience = true,
                    ValidAudience = _audience,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero
                };

                var principal = tokenHandler.ValidateToken(token, validationParameters, out SecurityToken validatedToken);
                return principal;
            }
            catch
            {
                return null;
            }
        }
    }
}
