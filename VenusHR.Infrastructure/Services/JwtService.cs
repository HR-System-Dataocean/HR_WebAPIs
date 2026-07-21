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
        bool IsTokenActive(int employeeId, string jti);
    }

    public class JwtService : IJwtService
    {
        private readonly string _secretKey;
        private readonly string _issuer;
        private readonly string _audience;
        private readonly ApplicationDBContext _context;

        public JwtService(IConfiguration configuration, ApplicationDBContext context)
        {
            _secretKey = configuration["JwtSettings:SecretKey"] ?? "YourDefaultSecretKeyThatShouldBeAtLeast32CharactersLong!";
            _issuer = configuration["JwtSettings:Issuer"] ?? "VenusHR";
            _audience = configuration["JwtSettings:Audience"] ?? "VenusHRUsers";
            _context = context;
        }

        public string GenerateToken(Hrs_Employees employee)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_secretKey);
            var jti = Guid.NewGuid().ToString();
            var now = DateTime.UtcNow;
            // Include a real exp claim (required by JWT middleware). Actual session end is logout / new login.
            var expires = now.AddYears(10);

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, employee.Code?.ToString() ?? ""),
                new Claim(ClaimTypes.Name, employee.ArbName ?? ""),
                new Claim(ClaimTypes.Email, employee.E_Mail ?? ""),
                new Claim("EmployeeId", employee.id.ToString()),
                new Claim("CompanyId", employee.CompanyId.ToString()),
                new Claim(JwtRegisteredClaimNames.Jti, jti)
            };

            var credentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature);
            var jwt = new JwtSecurityToken(
                issuer: _issuer,
                audience: _audience,
                claims: claims,
                notBefore: now,
                expires: expires,
                signingCredentials: credentials);

            // Logging in again replaces the active token, invalidating any previous one.
            var previousTokens = _context.UserActiveTokens.Where(t => t.EmployeeId == employee.id);
            _context.UserActiveTokens.RemoveRange(previousTokens);
            _context.UserActiveTokens.Add(new UserActiveToken
            {
                EmployeeId = employee.id,
                Jti = jti,
                IssuedAt = now
            });
            _context.SaveChanges();

            return tokenHandler.WriteToken(jwt);
        }

        public bool IsTokenActive(int employeeId, string jti)
        {
            return _context.UserActiveTokens.Any(t => t.EmployeeId == employeeId && t.Jti == jti);
        }

        public async Task RevokeTokenAsync(string token, string? reason = null)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var jwtToken = tokenHandler.ReadJwtToken(token);

            // Remove the active-token record so the token is immediately invalid.
            var employeeIdClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == "EmployeeId")?.Value;
            if (int.TryParse(employeeIdClaim, out var employeeId))
            {
                var activeTokens = _context.UserActiveTokens.Where(t => t.EmployeeId == employeeId);
                _context.UserActiveTokens.RemoveRange(activeTokens);
            }

            // Kept as an audit trail of revocations.
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
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.UTF8.GetBytes(_secretKey);

                var validationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = true,
                    ValidIssuer = _issuer,
                    ValidateAudience = true,
                    ValidAudience = _audience,
                    // Lifetime is controlled by UserActiveTokens (logout / new login), not by exp time.
                    ValidateLifetime = false,
                    ClockSkew = TimeSpan.Zero
                };

                var principal = tokenHandler.ValidateToken(token, validationParameters, out SecurityToken validatedToken);

                // Token is only valid while it is the user's current active token
                // (i.e. the user has not logged out or logged in again since it was issued).
                var jti = principal.FindFirst(JwtRegisteredClaimNames.Jti)?.Value;
                var employeeIdClaim = principal.FindFirst("EmployeeId")?.Value;
                if (jti == null || !int.TryParse(employeeIdClaim, out var employeeId) || !IsTokenActive(employeeId, jti))
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
    }
}
