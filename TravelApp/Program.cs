using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Security.Principal;
using System.Text;
using TravelApp.Models;
using TravelApp.Models.Services;
using TravelApp.Models.Services.Interfaces;
using WebApplicationAPI.Middleware;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddScoped<IAdvertisementService,AdvertisementService>();
builder.Services.AddScoped<ICompanyService, CompanyService>();
builder.Services.AddScoped<IReviewService, ReviewService>();
builder.Services.AddScoped<IBookingService, BookingService>();
builder.Services.AddScoped<IAdvertisementImageService, AdvertisementImageService>();
builder.Services.AddScoped<IOfferService, OfferService>();
builder.Services.AddScoped<IPaymentService, PaymentService>();
builder.Services.AddScoped<IPermissionRequestService, PermissionRequestService>();
builder.Services.AddScoped<IEmailService, EmailService>();

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Enter JWT token",
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT"
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
            new string[]{}
        }
    });
});
builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Adding JWT And Identity
builder.Services.AddIdentity<User, IdentityRole>(options =>
{
    options.Tokens.EmailConfirmationTokenProvider = "EmailTokenProvider";
    options.Tokens.ChangeEmailTokenProvider = "EmailTokenProvider";
    options.Tokens.ProviderMap["EmailTokenProvider"] = new TokenProviderDescriptor(
    typeof(EmailTokenProvider<User>));
})
    .AddEntityFrameworkStores<ApplicationDbContext>().AddDefaultTokenProviders();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.SaveToken = true;
    options.RequireHttpsMetadata = false;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateIssuerSigningKey = true,
        ValidateLifetime = true,
        ValidAudience = builder.Configuration["JWT:ValidAudience"],
        ValidIssuer = builder.Configuration["JWT:ValidIssuer"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JWT:Secret"])),
        ClockSkew = TimeSpan.Zero
    };
})
.AddGoogle(options =>
{
    options.ClientId = builder.Configuration["Google:ClientId"];
    options.ClientSecret = builder.Configuration["Google:ClientSecret"];
});

var app = builder.Build();
app.UseCors(policy => policy
    .AllowAnyOrigin()
    .AllowAnyMethod()
    .AllowAnyHeader());

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();
//app.UseMiddleware<RateLimtingMiddleware>();
AppDbInitializer.SeedRolesAsync(app).Wait();
AppDbInitializer.SeedUsersAsync(app).Wait();
//AppDbInitializer.Seed(app).Wait();

app.MapControllers();

app.Run();

