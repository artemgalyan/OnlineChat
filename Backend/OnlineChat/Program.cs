using BusinessLogic.Hubs.Chat;
using BusinessLogic.Queries.Chatrooms.GetChatrooms;
using BusinessLogic.Services;
using BusinessLogic.Services.UsersService;
using Database;
using Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;
using OnlineChat;
using OnlineChat.Auth;

const string myPolicy = "MyPolicy";

var config = new ConfigurationBuilder()
             .SetBasePath(Directory.GetCurrentDirectory())
             .AddJsonFile("appsettings.json")
             .Build();
var connectionString = config.GetConnectionString("SqlConnectionString");
var keyManager = new KeyManager();

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHttpContextAccessor();
builder.Services.AddAuthentication()
       .AddJwtBearer(b =>
       {
           b.TokenValidationParameters = new TokenValidationParameters {
               ValidateActor = false,
               ValidateIssuer = false,
               RequireExpirationTime = true,
               ClockSkew = TimeSpan.FromDays(31)
           };
           b.Configuration = new OpenIdConnectConfiguration { SigningKeys = { new RsaSecurityKey(keyManager.RsaKey) } };
       });
builder.Services.RegisterRepositories();
builder.Services.AddAuthorization();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSingleton<IPasswordHasher<User>, PasswordHasher>();
builder.Services.AddDbContext<ChatDatabase>(options => options.UseSqlServer(connectionString));
builder.Services.AddSingleton<IJwtProducer, JwtProducer>(_ =>
    new JwtProducer(TimeSpan.FromDays(31), keyManager.RsaKey));
builder.Services.AddScoped<IUserAccessor, UserAccessor>();
builder.Services.AddScoped<IChatHubService, ChatHubService>();
builder.Services.AddAutoMapper(Utils.GetAutoMapperProfiles());
builder.Services.AddCors(options =>
{
    options.AddPolicy(name: myPolicy,
        b =>
        {
            // b.WithOrigins(Routes.Localhost, Routes.Frontend)
            b.AllowAnyMethod()
             .AllowAnyHeader()
             .AllowCredentials()
             .SetIsOriginAllowed(o => true);
        });
});

builder.Services.AddSingleton<IUserConnectionTracker, UserConnectionTracker>();


builder.Services.AddSignalR();
builder.Services.AddMediatR(typeof(GetChatroomsHandler));

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors(myPolicy);
app.UseMiddleware<DbTokenValidator>();

app.UseAuthentication();
app.UseAuthorization();
app.MapHub<ChatHub>("/chat");
app.MapControllers();
app.Run();