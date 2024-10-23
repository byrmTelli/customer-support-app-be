using customer_support_app.CORE.DBModels;
using customer_support_app.DAL.Abstract;
using customer_support_app.DAL.Concrete;
using customer_support_app.DAL.Context.DbContext;
using customer_support_app.SERVICE.Abstract;
using customer_support_app.SERVICE.Concrete;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);



builder.Services.AddDbContext<AppDbContext>();
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
#region Password Settings
builder.Services.AddIdentity<AppUser, AppRole>(options =>
{
    options.Password.RequireDigit = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireLowercase = false;
    options.Password.RequiredLength = 6;
})
    .AddEntityFrameworkStores<AppDbContext>()
.AddDefaultTokenProviders();
#endregion

#region Jwt Settings

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options => 
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
    };
});

#endregion

#region DIs

// Data Access
builder.Services.AddScoped<ITicketDal, TicketDal>();
builder.Services.AddScoped<ICategoryDal, CategoryDal>();
builder.Services.AddScoped<ICommentDal, CommentDal>();
builder.Services.AddScoped<IUserDal, UserDal>();
builder.Services.AddScoped<ILogActivityDal, LogActivityDal>();

// Service
builder.Services.AddScoped<ICategoryService,CategoryService>();
builder.Services.AddScoped<ITicketService,TicketService>();
builder.Services.AddScoped<ICommentService,CommentService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IAuthService, AuthService>();
#endregion


#region CORS

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend",
        builder => builder.WithOrigins("https://localhost:3000", "http://localhost:3000", "https://www.customersupportapp.com")
                          .AllowAnyMethod()
                          .AllowAnyHeader());
});

#endregion

builder.Services.AddAuthorization();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();


if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowFrontend");

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();


#region Data Seeding
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var dbContext = services.GetRequiredService<AppDbContext>();
    try
    {
        await AppDbContext.SeedUsersAndRolesAsync(services);


        if (!dbContext.Categories.Any())
        {
            var categories = AppDbContext.GetCategories();
            dbContext.Categories.AddRange(categories);
            await dbContext.SaveChangesAsync();
        }

        if (!dbContext.Tickets.Any())
        {
            var tickets = AppDbContext.GetTickets();
            dbContext.Tickets.AddRange(tickets);
            await dbContext.SaveChangesAsync();
        }

        if (!dbContext.Comments.Any())
        { 
           var comments = AppDbContext.GetComments();
            dbContext.Comments.AddRange(comments);
            await dbContext.SaveChangesAsync();
        }

        await dbContext.SaveChangesAsync();

    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred during seeding roles and users.");
    }
}
#endregion

app.Run();
