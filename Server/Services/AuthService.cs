using SmartMonitoring.Server.Entities;
using SmartMonitoring.Shared.Extensions;
using SmartMonitoring.Shared.Models;
using SmartMonitoring.Shared.ViewModels;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using AutoMapper;
using Microsoft.IdentityModel.Tokens;

namespace SmartMonitoring.Server.Services;

public class AuthService
{
    private AdminService AdminService;
    private TelegramUserService TelegramUserService;
    private IMapper Mapper;

    public AuthService(AdminService adminService, IMapper mapper, TelegramUserService telegramUserService)
    {
        AdminService = adminService;
        Mapper = mapper;
        TelegramUserService = telegramUserService;
    }

    public async Task<ServiceResponse<AuthHelperModel<AdminViewModel>>> Authenticate(AuthModel model)
    {
        var res = new ServiceResponse<AuthHelperModel<AdminViewModel>>();
        res.Name = "Проверьте введённые данные: номер телефона и/или пароль.";

        var user = AdminService.GetByLogin(model.Login);
        if (user == null)
        {
            return res;
        }

        if (user.PasswordHash != model.Password.ToHashSHA256())
        {
            return res;
        }

        res.Data = new AuthHelperModel<AdminViewModel>
        {
            Entity = "Admin",
            EntityJSON = Mapper.Map<AdminViewModel>(user),
            EntityHash = Mapper.Map<AdminViewModel>(user).ToHashSHA256(),
            EntityID = user.ID.ToString(),
            PlatformID = null,
            Token = GetTokenByUser(user)
        };
        res.Status = true;

        return res;
    }


    #region ASPNET_IDENTIFY

    public const string ISSUER = "SM.Server";
    public const string AUDIENCE = "SM.Client";

    /// <summary>
    /// Ключ шифрования.
    /// </summary>
    private const string KEY = "S2390-dJPOSd()SUYdhi*YDSAhou;iD()@@*UOINJ$KNSA";

    private static SymmetricSecurityKey GetSymmetricSecurityKey() =>
        new(Encoding.UTF8.GetBytes(KEY));

    /// <summary>
    /// Get Bearer token by User entity.
    /// </summary>
    /// <param name="user">User entity.</param>
    /// <returns>Bearer token for User entity.</returns>
    static string GetTokenByUser(AdminEntity user)
    {
        var time = TimeSpan.FromDays(365);

        // создаем JWT-токен
        var jwt = new JwtSecurityToken(
            issuer: ISSUER,
            audience: AUDIENCE,
            claims: GetIdentityByUser(user).Claims,
            expires: DateTime.UtcNow.Add(time),
            signingCredentials: new SigningCredentials(GetSymmetricSecurityKey(), SecurityAlgorithms.HmacSha256));

        var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);
        return encodedJwt;
    }

    /// <summary>
    /// Generate Claims by User entity.
    /// </summary>
    /// <param name="user">User entity.</param>
    /// <returns>Claims.</returns>
    static ClaimsIdentity GetIdentityByUser(AdminEntity user)
    {
        var claims = new List<Claim>
        {
            new("ID", user.ID.ToString()),
            new("Login", user.Login),
            new("Entity", "User"),
            new(ClaimTypes.NameIdentifier, user.ID.ToString()),
            new(ClaimsIdentity.DefaultNameClaimType, user.ID.ToString()),
        };

        ClaimsIdentity claimsIdentity =
            new ClaimsIdentity(claims, "Token", "ID",
                "Admin");
        return claimsIdentity;
    }

    /// <summary>
    /// Builder paramethers.
    /// </summary>
    public static TokenValidationParameters BuilderParamethers => new TokenValidationParameters
    {
        // указывает, будет ли валидироваться издатель при валидации токена
        ValidateIssuer = true,
        // строка, представляющая издателя
        ValidIssuer = ISSUER,
        // будет ли валидироваться потребитель токена
        ValidateAudience = true,
        // установка потребителя токена
        ValidAudience = AUDIENCE,
        // будет ли валидироваться время существования
        ValidateLifetime = true,
        // установка ключа безопасности
        IssuerSigningKey = GetSymmetricSecurityKey(),
        // валидация ключа безопасности
        ValidateIssuerSigningKey = true,
    };

    #endregion
}