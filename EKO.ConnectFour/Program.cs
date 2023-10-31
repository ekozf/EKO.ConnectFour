using EKO.ConnectFour.Api.Filters;
using EKO.ConnectFour.Api.Hubs;
using EKO.ConnectFour.Api.Services;
using EKO.ConnectFour.Api.Services.Contracts;
using EKO.ConnectFour.AppLogic;
using EKO.ConnectFour.AppLogic.Contracts;
using EKO.ConnectFour.Domain;
using EKO.ConnectFour.Domain.GameDomain;
using EKO.ConnectFour.Domain.GameDomain.Contracts;
using EKO.ConnectFour.Domain.GridDomain;
using EKO.ConnectFour.Domain.GridDomain.Contracts;
using EKO.ConnectFour.Domain.PlayerDomain;
using EKO.ConnectFour.Domain.PlayerDomain.Contracts;
using EKO.ConnectFour.Infrastructure.Storage;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add SignalR services to the container.
builder.Services.AddSignalR();

builder.Services.AddSingleton(provider => new ConnectFourExceptionFilterAttribute(provider.GetRequiredService<ILogger<Program>>()));

// Add services to the container.
builder.Services.AddControllers(options =>
{
    var onlyAuthenticatedUsersPolicy = new AuthorizationPolicyBuilder(JwtBearerDefaults.AuthenticationScheme)
        .RequireAuthenticatedUser().Build();
    options.Filters.Add(new AuthorizeFilter(onlyAuthenticatedUsersPolicy));
    options.Filters.AddService<ConnectFourExceptionFilterAttribute>();
})
.AddJsonOptions(options =>
{
    options.JsonSerializerOptions.Converters.Add(new TwoDimensionalArrayJsonConverter());
});

builder.Services.AddCors(options => options.AddPolicy("CorsPolicy", policy =>
{
    policy
        .AllowAnyHeader()
        .AllowAnyMethod()
        .SetIsOriginAllowed((_) => true)
        .AllowCredentials();
}));

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Version = "v1",
        Title = "ConnectFour API",
        Description = "REST API for online Connect Four"
    });

    // Enable bearer token authentication
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Copy 'Bearer ' + valid token into field. You can retrieve a bearer token via '/api/authentication/token'"
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

var configuration = builder.Configuration as IConfiguration;


var tokenSettings = new TokenSettings();
configuration.Bind("Token", tokenSettings);

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidIssuer = tokenSettings.Issuer,
        ValidAudience = tokenSettings.Audience,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(tokenSettings.Key)),
    };
});

builder.Services.AddAuthorization();

builder.Services.AddDbContext<ConnectFourDbContext>(options =>
{
#if DEBUG
    var connectionString = configuration.GetConnectionString("ConnectFourDbConnectionDev");
#else
    var connectionString = configuration.GetConnectionString("ConnectFourDbConnectionProd");
#endif

    if (string.IsNullOrWhiteSpace(connectionString))
    {
        throw new InvalidOperationException("Connection string for database is not set.");
    }

    options.UseSqlite(connectionString).EnableSensitiveDataLogging();
});

builder.Services.AddIdentity<User, IdentityRole<Guid>>(options =>
{
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);
    options.Lockout.MaxFailedAccessAttempts = 8;
    options.Lockout.AllowedForNewUsers = true;

    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireDigit = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireLowercase = false;
    options.Password.RequiredLength = 5;

    options.SignIn.RequireConfirmedEmail = false;
    options.SignIn.RequireConfirmedPhoneNumber = false;
})
.AddEntityFrameworkStores<ConnectFourDbContext>()
.AddDefaultTokenProviders();

builder.Services.AddAutoMapper(typeof(Program));
builder.Services.AddSingleton<ITokenFactory>(new JwtTokenFactory(tokenSettings));
builder.Services.AddScoped<IWaitingPool, WaitingPool>();
builder.Services.AddSingleton<IGameCandidateFactory, GameCandidateFactory>();
builder.Services.AddSingleton<IGameCandidateRepository, InMemoryGameCandidateRepository>();
builder.Services.AddSingleton<IGameCandidateMatcher, BasicGameCandidateMatcher>();
builder.Services.AddScoped<IGameService, GameService>();
builder.Services.AddScoped(services => new GameFactory(services.GetRequiredService<IGamePlayStrategy>()) as IGameFactory);
builder.Services.AddSingleton<IGameRepository, InMemoryGameRepository>();
var miniMaxSearchDepth = builder.Configuration.GetValue<int>("MiniMaxSearchDepth");
builder.Services.AddScoped<IGamePlayStrategy, MiniMaxGamePlayStrategy>(services => new MiniMaxGamePlayStrategy(services.GetService<IGridEvaluator>()!, miniMaxSearchDepth));
builder.Services.AddScoped(_ => new GridEvaluator() as IGridEvaluator);

// Add our own services
builder.Services.AddSingleton<ILeaderboardRepository, InMemoryLeaderboardRepository>();
builder.Services.AddScoped<ILeaderboardService, LeaderboardService>();

var app = builder.Build();

//Create database (if it does not exist yet)
var scope = app.Services.CreateScope();

var context = scope.ServiceProvider.GetRequiredService<ConnectFourDbContext>();

context.Database.EnsureCreated();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("CorsPolicy");

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.MapHub<WaitingPoolChatHub>("/waitingpool-chat");
app.MapHub<GameChatHub>("/game-chat");

app.Run();
