using customer_support_app.CORE.DBModels;
using customer_support_app.DAL.Abstract;
using customer_support_app.DAL.Concrete;
using customer_support_app.DAL.Context.DbContext;
using customer_support_app.SERVICE.Abstract;
using customer_support_app.SERVICE.Concrete;
using Microsoft.AspNetCore.Identity;

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


#region DIs

// Data Access
builder.Services.AddScoped<ITicketDal, TicketDal>();
builder.Services.AddScoped<ICategoryDal, CategoryDal>();
builder.Services.AddScoped<ICommentDal, CommentDal>();

// Service
builder.Services.AddScoped<ICategoryService,CategoryService>();
builder.Services.AddScoped<ITicketService,TicketService>();
builder.Services.AddScoped<ICommentService,CommentService>();
#endregion



builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();


if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

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
