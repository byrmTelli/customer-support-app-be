using customer_support_app.CORE.DBModels;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using customer_support_app.CORE.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace customer_support_app.DAL.Context.DbContext
{
    public class AppDbContext : IdentityDbContext<AppUser, AppRole, int>
    {
        public AppDbContext()
        {
            
        }
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {

        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data Source=CustomerSupport.db");
        }

        public DbSet<Category> Categories { get; set; }
        public DbSet<Ticket> Tickets { get; set; }
        public DbSet<Comment> Comments { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfigurationsFromAssembly(GetType().Assembly);

            #region DB Relations

            modelBuilder.Entity<Ticket>()
                .HasOne(ticket => ticket.Category)
                .WithMany(category => category.Tickets)
                .HasForeignKey(ticket => ticket.CategoryId);

            modelBuilder.Entity<AppUser>()
                .HasMany(user => user.Tickets)
                .WithOne(ticket => ticket.Creator)
                .HasForeignKey(ticket => ticket.CreatorId);

            modelBuilder.Entity<Comment>()
                .HasOne(comment => comment.Ticket)
                .WithMany(ticket => ticket.Comments)
                .HasForeignKey(comment => comment.TicketId);

            modelBuilder.Entity<AppUser>()
                .HasMany(user => user.Comments)
                .WithOne(comment => comment.Creator)
                .HasForeignKey(comment => comment.CreatorId);

            #endregion

            #region Query Filter
            modelBuilder.Entity<Ticket>().HasQueryFilter(t => !t.IsDeleted);
            modelBuilder.Entity<Category>().HasQueryFilter(t => !t.IsDeleted);
            modelBuilder.Entity<Comment>().HasQueryFilter(t => !t.IsDeleted);
            #endregion


        }

        #region Seeder Functions
        public static async Task SeedUsersAndRolesAsync(IServiceProvider serviceProvider)
        {
            var roleManager = serviceProvider.GetRequiredService<RoleManager<AppRole>>();
            var userManager = serviceProvider.GetRequiredService<UserManager<AppUser>>();

            string[] roleNames = { "admin", "helpdesk", "customer" };

            foreach (var roleName in roleNames)
            {
                if (!await roleManager.RoleExistsAsync(roleName))
                {
                    await roleManager.CreateAsync(new AppRole { Name = roleName });
                }
            }

            var adminUser = new AppUser
            {
                UserName = "adminuser",
                Email = "admin@example.com",
                Name = "Admin",
                Surname = "User",
                Adress = "Admin Address"
            };

            if (userManager.Users.All(u => u.UserName != adminUser.UserName))
            {
                await userManager.CreateAsync(adminUser, "Password123.");
                await userManager.AddToRoleAsync(adminUser, "admin");
            }

            for (int i = 1; i <= 3; i++)
            {
                var helpdeskUser = new AppUser
                {
                    UserName = $"helpdesk{i}",
                    Email = $"helpdesk{i}@example.com",
                    Name = $"Helpdesk{i}",
                    Surname = "User",
                    Adress = $"Helpdesk Address {i}"
                };

                if (userManager.Users.All(u => u.UserName != helpdeskUser.UserName))
                {
                    await userManager.CreateAsync(helpdeskUser, "Password123.");
                    await userManager.AddToRoleAsync(helpdeskUser, "helpdesk");
                }
            }

            for (int i = 1; i <= 6; i++)
            {
                var customerUser = new AppUser
                {
                    UserName = $"customer{i}",
                    Email = $"customer{i}@example.com",
                    Name = $"Customer{i}",
                    Surname = "User",
                    Adress = $"Customer Address {i}"
                };

                if (userManager.Users.All(u => u.UserName != customerUser.UserName))
                {
                    await userManager.CreateAsync(customerUser, "Password123.");
                    await userManager.AddToRoleAsync(customerUser, "customer");
                }
            }
        }
        public static List<Category> GetCategories()
        {
            var categories = new List<Category>()
            {
                new Category {Id =1, Name = "Home Electronics", Creator = "Seeder" },
                new Category {Id =2,  Name = "Personal Devices", Creator = "Seeder" },
                new Category {Id =3,  Name = "Fashion", Creator = "Seeder" },
                new Category {Id =4,  Name = "Sports Equipment", Creator = "Seeder" },
                new Category {Id =5,  Name = "Books", Creator = "Seeder" },
                new Category {Id =6,  Name = "Toys & Games", Creator = "Seeder" },
                new Category {Id =7,  Name = "Automotive", Creator = "Seeder" },
                new Category {Id =8,  Name = "Beauty & Health", Creator = "Seeder" },
                new Category {Id =9,  Name = "Furniture", Creator = "Seeder" },
                new Category {Id =10,  Name = "Kitchen Appliances", Creator = "Seeder" }
            };

            return categories;
        }
        public static List<Ticket> GetTickets()
        {
            var tickets = new List<Ticket>()
    {
        // Creator 1
        new Ticket{
            Id =1, 
            Title = "Issue with Electronics", 
            Content= "Problem with my home electronics.", 
            Status= TicketStatus.Pending, 
            CategoryId = 1, 
            IsPublished = true, 
            CreatorId = 1
        },
        new Ticket{
            Id =2, 
            Title = "Device malfunction", 
            Content= "My personal device is not working.", 
            Status= TicketStatus.Pending, 
            CategoryId = 2, 
            IsPublished = true, 
            CreatorId = 1
        },
        new Ticket{
            Id =3, 
            Title = "Fashion query", 
            Content= "I have a question about fashion.", 
            Status= TicketStatus.Completed, 
            CategoryId = 3, 
            IsPublished = true, 
            CreatorId = 1
        },
        new Ticket{
            Id = 4, 
            Title = "Sports equipment issue", 
            Content= "Need assistance with sports gear.", 
            Status= TicketStatus.Pending, 
            CategoryId = 4, 
            IsPublished = true,
            CreatorId = 1
        },

        // Creator 2
        new Ticket{
            Id =5, 
            Title = "Book availability", 
            Content= "Looking for a specific book.", 
            Status= TicketStatus.Pending, 
            CategoryId = 5, 
            IsPublished = true, 
            CreatorId = 2
        },
        new Ticket{
            Id =6, 
            Title = "Toy malfunction", 
            Content= "Toy is not working properly.", 
            Status= TicketStatus.Completed, 
            CategoryId = 6, 
            IsPublished = true, 
            CreatorId = 2
        },
        new Ticket{Id =7, Title = "Car issue", Content= "Problem with my car.", Status= TicketStatus.Pending, CategoryId = 7, IsPublished = true, CreatorId = 2},
        new Ticket{Id =8, Title = "Beauty product inquiry", Content= "Question about beauty products.", Status= TicketStatus.Pending, CategoryId = 8, IsPublished = true, CreatorId = 2},

        // Creator 3
        new Ticket{Id =9, Title = "Furniture damage", Content= "My furniture is damaged.", Status= TicketStatus.Pending, CategoryId = 9, IsPublished = true, CreatorId = 3},
        new Ticket{Id = 10, Title = "Kitchen appliance malfunction", Content= "Problem with my kitchen appliance.", Status= TicketStatus.Pending, CategoryId = 10, IsPublished = true, CreatorId = 3},
        new Ticket{Id = 11, Title = "Electronics warranty", Content= "Need warranty details for electronics.", Status= TicketStatus.Pending, CategoryId = 1, IsPublished = true, CreatorId = 3},
        new Ticket{Id = 12, Title = "Device return request", Content= "Want to return my device.", Status= TicketStatus.Pending, CategoryId = 2, IsPublished = true, CreatorId = 3},

        // Creator 4
        new Ticket{Id =13, Title = "Fashion product exchange", Content= "Want to exchange a fashion item.", Status= TicketStatus.Pending, CategoryId = 3, IsPublished = true, CreatorId = 4},
        new Ticket{Id =14, Title = "Sports equipment refund", Content= "Requesting refund for sports gear.", Status= TicketStatus.Pending, CategoryId = 4, IsPublished = true, CreatorId = 4},
        new Ticket{Id =15, Title = "Book delivery delay", Content= "My book delivery is delayed.", Status= TicketStatus.Pending, CategoryId = 5, IsPublished = true, CreatorId = 4},
        new Ticket{Id =16, Title = "Toy quality issue", Content= "Concern about toy quality.", Status= TicketStatus.Pending, CategoryId = 6, IsPublished = true, CreatorId = 4},

        // Creator 5
        new Ticket{Id =17, Title = "Car service inquiry", Content= "Question about car servicing.", Status= TicketStatus.Pending, CategoryId = 7, IsPublished = true, CreatorId = 5},
        new Ticket{Id =18, Title = "Beauty product refund", Content= "Requesting refund for beauty product.", Status= TicketStatus.Pending, CategoryId = 8, IsPublished = true, CreatorId = 5},
        new Ticket{Id =19, Title = "Furniture assembly help", Content= "Need help assembling furniture.", Status= TicketStatus.Pending, CategoryId = 9, IsPublished = true, CreatorId = 5},
        new Ticket{Id =20, Title = "Kitchen appliance return", Content= "Want to return kitchen appliance.", Status= TicketStatus.Pending, CategoryId = 10, IsPublished = true, CreatorId = 5},

        // Creator 6
        new Ticket{Id =21, Title = "Electronics installation", Content= "Need help installing electronics.", Status= TicketStatus.Pending, CategoryId = 1, IsPublished = true, CreatorId = 6},
        new Ticket{Id =22, Title = "Device repair request", Content= "Requesting repair for my device.", Status= TicketStatus.Pending, CategoryId = 2, IsPublished = true, CreatorId = 6},
        new Ticket{Id =23, Title = "Fashion trend inquiry", Content= "Question about latest fashion trends.", Status= TicketStatus.Pending, CategoryId = 3, IsPublished = true, CreatorId = 6},
        new Ticket{Id =24, Title = "Sports gear replacement", Content= "Need replacement for sports gear.", Status= TicketStatus.Pending, CategoryId = 4, IsPublished = true, CreatorId = 6},

        // Creator 7
        new Ticket{Id =25, Title = "Book recommendation", Content= "Looking for book recommendations.", Status= TicketStatus.Pending, CategoryId = 5, IsPublished = true, CreatorId = 7},
        new Ticket{Id =26, Title = "Toy return request", Content= "Want to return a toy.", Status= TicketStatus.Pending, CategoryId = 6, IsPublished = true, CreatorId = 7},
        new Ticket{Id =27, Title = "Car parts inquiry", Content= "Question about car parts.", Status= TicketStatus.Pending, CategoryId = 7, IsPublished = true, CreatorId = 7},
        new Ticket{Id =28, Title = "Beauty product query", Content= "Inquiry about beauty products.", Status= TicketStatus.Pending, CategoryId = 8, IsPublished = true, CreatorId = 7},

        // Creator 8
        new Ticket{Id =29, Title = "Furniture return", Content= "Want to return furniture.", Status= TicketStatus.Pending, CategoryId = 9, IsPublished = true, CreatorId = 8},
        new Ticket{Id =30, Title = "Kitchen appliance issue", Content= "Kitchen appliance is not functioning.", Status= TicketStatus.Pending, CategoryId = 10, IsPublished = true, CreatorId = 8},
        new Ticket{Id =31, Title = "Electronics technical help", Content= "Need technical help for electronics.", Status= TicketStatus.Pending, CategoryId = 1, IsPublished = true, CreatorId = 8},
        new Ticket{Id =32, Title = "Device warranty extension", Content= "Requesting warranty extension.", Status= TicketStatus.Pending, CategoryId = 2, IsPublished = true, CreatorId = 8},

        // Creator 9
        new Ticket{Id =33, Title = "Fashion item return", Content= "Want to return a fashion item.", Status= TicketStatus.Pending, CategoryId = 3, IsPublished = true, CreatorId = 9},
        new Ticket{Id =34, Title = "Sports gear repair", Content= "Requesting repair for sports gear.", Status= TicketStatus.Pending, CategoryId = 4, IsPublished = true, CreatorId = 9},
        new Ticket{Id =35, Title = "Book replacement", Content= "Need a replacement for my book.", Status= TicketStatus.Pending, CategoryId = 5, IsPublished = true, CreatorId = 9},
        new Ticket{Id =36, Title = "Toy assembly issue", Content= "Need help with toy assembly.", Status= TicketStatus.Pending, CategoryId = 6, IsPublished = true, CreatorId = 9},

        // Creator 10
        new Ticket{Id =37, Title = "Car warranty", Content= "Question about car warranty.", Status= TicketStatus.Pending, CategoryId = 7, IsPublished = true, CreatorId = 10},
        new Ticket{Id =38, Title = "Beauty product exchange", Content= "Want to exchange beauty products.", Status= TicketStatus.Pending, CategoryId = 8, IsPublished = true, CreatorId = 10},
        new Ticket{Id =39,Title = "Furniture cleaning advice", Content= "Need advice on cleaning furniture.", Status= TicketStatus.Pending, CategoryId = 9, IsPublished = true, CreatorId = 10},
        new Ticket{Id =40, Title = "Kitchen appliance inquiry", Content= "Inquiry about kitchen appliance features.", Status= TicketStatus.Pending, CategoryId = 10, IsPublished = true, CreatorId = 10},
    };

            return tickets;
        }
        public static List<Comment> GetComments()
        {
            var comments = new List<Comment>
            {
                new Comment 
                {
                    Id = 1,
                    Message = "I have an issiue about my electronic device.",
                    CreatorId = 1,
                    TicketId= 1,                    
                },
                 new Comment
                {
                    Id = 2,
                    Message = "We are working on it. It will be fixed soon.",
                    CreatorId = 2,
                    TicketId= 1
                },
                  new Comment
                {
                    Id = 3,
                    Message = "Thank you for your efforts sir!",
                    CreatorId = 1,
                    TicketId= 1
                },
            };

            return comments;   
        }

        #endregion
    }
}
