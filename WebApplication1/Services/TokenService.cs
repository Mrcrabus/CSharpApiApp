using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

public class TokenService
{
    private readonly JwtSetting _jwtSetting;
    private readonly SymmetricSecurityKey _securityAccessKey;
    private readonly SymmetricSecurityKey _securityRefreshKey;

    public TokenService(IOptions<JwtSetting> options)
    {
        _jwtSetting = options.Value;
        _securityAccessKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSetting.AccessSecretKey));
        _securityRefreshKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSetting.RefreshSecretKey));
    }

    public Tokens GenerateTokens(IEnumerable<Claim> claims)
    {
        var credentialsAccess = new SigningCredentials(_securityAccessKey, SecurityAlgorithms.HmacSha256);
        var credentialsRefresh = new SigningCredentials(_securityRefreshKey, SecurityAlgorithms.HmacSha256);

        var guid = Guid.NewGuid().ToString();
        var claim = new Claim("tokenId", guid);

        var accessToken = new JwtSecurityToken(
            claims: claims.Append(claim),
            issuer:  "jest",
            expires: DateTime.UtcNow.AddMinutes(12),
            signingCredentials: credentialsAccess
        );

        var refreshToken = new JwtSecurityToken(
            claims: claims.Append(claim),
            expires: DateTime.UtcNow.AddMinutes(30),
            signingCredentials: credentialsRefresh
        );

        return new Tokens
        {
            AccessToken = new JwtSecurityTokenHandler().WriteToken(accessToken),
            RefreshToken = new JwtSecurityTokenHandler().WriteToken(refreshToken)
        };
    }

    public bool TryRefreshToken(Tokens tokens, out Tokens newToken)
    {
        newToken = null;
        
        var tokenHandler = new JwtSecurityTokenHandler();
        var validationParametersAccess = new TokenValidationParameters
        {
            ValidateLifetime = false,
            IssuerSigningKey = _securityAccessKey,
            ValidateIssuerSigningKey = true, // Для декодирования не требуется проверка подписи
            ValidateIssuer = false, // Можно указать true, если требуется проверка издателя (issuer)
            ValidateAudience = false // Можно указать true, если требуется проверка аудитории (audience)
        };

        var validationParametersRefresh = new TokenValidationParameters
        {
            IssuerSigningKey = _securityRefreshKey,
            ValidateIssuerSigningKey = true, // Для декодирования не требуется проверка подписи
            ValidateIssuer = false, // Можно указать true, если требуется проверка издателя (issuer)
            ValidateAudience = false // Можно указать true, если требуется проверка аудитории (audience)
        };

        try
        {
            var claimsAccess = tokenHandler.ValidateToken(tokens.AccessToken, validationParametersAccess, out _);
            var claimsRefresh = tokenHandler.ValidateToken(tokens.RefreshToken, validationParametersRefresh, out _);

            var tokenIdAccess = claimsAccess.FindFirst("tokenId")?.Value;
            var tokenIdRefresh = claimsRefresh.FindFirst("tokenId")?.Value;


            if (tokenIdAccess == null || tokenIdRefresh == null || tokenIdAccess != tokenIdRefresh) return false;

            
            newToken = GenerateTokens(claimsAccess.Claims.Where(claim => claim.Type != "tokenId"));

            return true;
        }
        catch (SecurityTokenException)
        {
            // Недействительный токен
            return false;
        }
    }
}

public class JwtSetting
{
    public string AccessSecretKey { get; init; }
    public string RefreshSecretKey { get; init; }
}

public class Tokens
{
    public string RefreshToken { get; set; }
    public string AccessToken { get; set; }
}