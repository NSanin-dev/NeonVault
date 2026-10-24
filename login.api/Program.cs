using System.Text;
using login.api.data;
using login.api.services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Stripe;

var builder = WebApplication.CreateBuilder(args);

//Add Controller support
builder.Services.AddControllers();

//Add database dependency 
builder.Services.AddDbContext<AppDbContext>(o => o.UseInMemoryDatabase("DB"));

//Add auth logic service dependency
builder.Services.AddScoped<IAuthService, ImplementedAuthService>();

//Add dashboard service
builder.Services.AddScoped<IDashboardService, ImplementedDashboardService>();

//Load stripe secret key
StripeConfiguration.ApiKey = builder.Configuration["Stripe:SecretKey"];

//Add checkout service
builder.Services.AddScoped<ICheckoutService, ImplementedCheckoutService>();

//Add webhook service
builder.Services.AddScoped<IWebhookService, ImplementedWebookService>();

//Add IGDB service & Memory Cache
builder.Services.AddMemoryCache();
builder.Services.AddHttpClient<IigdbService, ImplementedIgdbService>();



//Add jwt configuration
var key = builder.Configuration["Jwt:SecretKey"];

builder.Services.AddAuthentication(o =>
{
    o.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    o.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(o =>
{
    o.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = false,
        ValidateAudience = false,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key!))
    };
});



//CORS Permission
builder.Services.AddCors(options => options.AddPolicy("AllowBlazorClient", policy =>
{
    policy.WithOrigins("http://localhost:5087")
    .AllowAnyHeader()
    .AllowAnyMethod();
}));


var app = builder.Build();

// Apply the policy!
app.UseCors("AllowBlazorClient");

//Configure HTTP Pipeline
app.UseHttpsRedirection();

//Add Authentication
app.UseAuthentication();

//Add authorization
app.UseAuthorization();

// Map Controller endpoints
app.MapControllers();

//Database seeding
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

    if (!db.AppUsers.Any(u => u.UserName == "EmperorOfMcDonal"))
    {
        string passHashed = BCrypt.Net.BCrypt.HashPassword("Password123");
        
        var admin = new AppUser
        {
            UserName = "EmperorOfMcDonal",
            Role = "Admin",
            ProfilePicture = "/images/Idk.jpg",
            PassWord = passHashed
        };

        var user = new AppUser
        {
            UserName = "SpooDer-Man",
            Role = "Free",
            ProfilePicture = "/images/images.jpeg",
            PassWord = passHashed
        };

        db.AppUsers.Add(admin);
        db.AppUsers.Add(user);
    }
    if (!db.Games.Any())
    {

        db.Games.AddRange(

            new Game
            {
                Title = "Max Payne 3",
                CoverArt = "https://images.igdb.com/igdb/image/upload/t_cover_big/co2mjo.jpg"
            },
            
            new Game
            {
                Title = "Fortnite",
                CoverArt = "https://images.igdb.com/igdb/image/upload/t_cover_big/cocxbi.jpg"
            },

            new Game
            {
                Title = "Grand Theft Auto 5",
                CoverArt = "https://images.igdb.com/igdb/image/upload/t_cover_big/co2lbd.jpg"
            },

            new Game
            {
                Title = "Red Dead Redemption 2",
                CoverArt = "https://images.igdb.com/igdb/image/upload/t_cover_big/co1q1f.jpg"
            },

            new Game
            {
                Title = "God Of War (2018)",
                CoverArt = "https://images.igdb.com/igdb/image/upload/t_cover_big/co1tmu.jpg"
            },

            new Game
            {
                Title = "The Legend of Zelda: Breath of the Wild",
                CoverArt = "https://images.igdb.com/igdb/image/upload/t_cover_big/co3p2d.jpg"
            }
        );
    }

    db.SaveChanges();
}

app.Run();