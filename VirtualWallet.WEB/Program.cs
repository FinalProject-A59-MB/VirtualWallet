using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.EntityFrameworkCore;
using VirtualWallet.DATA;
using VirtualWallet.WEB.Middlewares;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using VirtualWallet.BUSINESS.Services.Contracts;
using VirtualWallet.DATA.Repositories.Contracts;
using VirtualWallet.DATA.Repositories;
using VirtualWallet.DATA.Services.Contracts;
using VirtualWallet.DATA.Services;
using VirtualWallet.BUSINESS.Services;
using VirtualWallet.DATA.Context;
using System.Net.Mail;
using System.Net;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// EF
builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    string connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
    options.UseSqlServer(connectionString);
    options.EnableSensitiveDataLogging();
});

// Session configuration
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

builder.Services.AddSingleton<ITempDataProvider, CookieTempDataProvider>();
builder.Services.AddSingleton<ITempDataDictionaryFactory, TempDataDictionaryFactory>();

// Repositories
//builder.Services.AddScoped<IBlockedRecordRepository, BlockedRecordRepository>(); TODO
builder.Services.AddScoped<ICardRepository, CardRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<ICardTransactionRepository, CardTransactionRepository>();
builder.Services.AddScoped<IRealCardRepository, RealCardRepository>();
//builder.Services.AddScoped<IRecurringPaymentRepository, RecurringPaymentRepository>(); TODO
//builder.Services.AddScoped<IUserContactRepository, UserContactRepository>(); TODO
builder.Services.AddScoped<IUserWalletRepository, UserWalletRepository>();
builder.Services.AddScoped<IWalletRepository, WalletRepository>();
builder.Services.AddScoped<IWalletTransactionRepository, WalletTransactionRepository>();

// Services
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<ICardService, CardService>();
builder.Services.AddScoped<ICardTransactionService, CardTransactionService>();
builder.Services.AddScoped<IEmailService, SmtpEmailService>();
builder.Services.AddScoped<IPaymentProcessorService, PaymentProcessorService>();
builder.Services.AddScoped<ITransactionHandlingService, TransactionHandlingService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IWalletService, WalletService>();
builder.Services.AddScoped<IWalletTransactionService, WalletTransactionService>();

builder.Services.AddScoped<ICloudinaryService, CloudinaryService>();

builder.Services.AddSingleton<IConfiguration>(builder.Configuration);
builder.Services.AddHttpContextAccessor();

// Helpers
builder.Services.AddScoped<IViewModelMapper, ViewModelMapper>();
builder.Services.AddScoped<IDtoMapper, DtoMapper>();

// Attributes
builder.Services.AddScoped<RequireAuthorizationAttribute>();


builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo { Title = "ForumProject API", Version = "v1" });
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter 'Bearer' [space] and then your token in the text input below.\n\nExample: \"Bearer abcdef12345\""
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
                        new string[] {}
                    }
                });
});




var app = builder.Build();
// Custom middlewares
app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseMiddleware<CurrentUserMiddleware>();

// Data Seeding
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<ApplicationDbContext>();
    InitializeData.Initialize(context);
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

// JWT authentication
app.UseAuthentication();
app.UseAuthorization();

// troubleshooting
//app.UseDeveloperExceptionPage();

app.UseSwagger();
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "AspNetCoreDemo API V1");
    options.RoutePrefix = "api/swagger";
});

app.UseEndpoints(endpoints =>
{
    endpoints.MapDefaultControllerRoute();
    endpoints.MapControllerRoute(
        name: "error",
        pattern: "Error",
        defaults: new { controller = "Error", action = "Index" });
});

app.Run();
