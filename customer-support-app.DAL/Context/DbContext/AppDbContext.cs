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
using customer_support_app.CORE.Constants;

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
        public DbSet<ActivityLog> ActivityLogs { get; set; }
        public DbSet<FileAttachment> FileAttechments { get; set; }
        public DbSet<SystemNotification> SystemNotifications { get; set; }
        public DbSet<TicketNotification> TicketNotifications { get; set; }

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
                .HasMany(user => user.UsersTickets)
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

            modelBuilder.Entity<AppUser>()
                .HasMany(user => user.AssignedTickets)
                .WithOne(ticket => ticket.AssignedTo)
                .HasForeignKey(ticket => ticket.AssignedUserId);

            modelBuilder.Entity<AppUser>()
                .HasMany(user => user.TicketActivities)
                .WithOne(activity => activity.User)
                .HasForeignKey(activity => activity.UserId);

            modelBuilder.Entity<Ticket>()
                .HasMany(ticket => ticket.Activities)
                .WithOne(activity => activity.Ticket)
                .HasForeignKey(activity => activity.TicketId);

            modelBuilder.Entity<Ticket>()
                .HasMany(ticket => ticket.Attachments)
                .WithOne(attachment => attachment.Ticket)
                .HasForeignKey(attachment => attachment.TicketId);

            modelBuilder.Entity<Ticket>()
                .HasMany(ticket => ticket.Notifications)
                .WithOne(notification => notification.Ticket)
                .HasForeignKey(notification => notification.TicketId);

            #endregion

            #region Query Filter
            modelBuilder.Entity<Ticket>().HasQueryFilter(t => !t.IsDeleted);
            modelBuilder.Entity<Category>().HasQueryFilter(t => !t.IsDeleted);
            modelBuilder.Entity<Comment>().HasQueryFilter(t => !t.IsDeleted);
            #endregion


        }

        #region Seeder Functions

        #region Profile Image Paths
        static string baseDirectory = Directory.GetCurrentDirectory();

        static string adminImage = Path.Combine(baseDirectory, "Assets", "Images", "admin-image.png");
        static string helpDeskImage = Path.Combine(baseDirectory, "Assets", "Images", "helpdesk-image.png");
        static string customerImage = Path.Combine(baseDirectory, "Assets", "Images", "customer-image.png");

        private static byte[] GetImageBytesFromPath(string imagePath)
        {
            if (File.Exists(imagePath))
            {
                return File.ReadAllBytes(imagePath);
            }
            return null;
        }

        #endregion



        public static async Task SeedUsersAndRolesAsync(IServiceProvider serviceProvider)
        {
            var roleManager = serviceProvider.GetRequiredService<RoleManager<AppRole>>();
            var userManager = serviceProvider.GetRequiredService<UserManager<AppUser>>();

            byte[] adminImageByte = GetImageBytesFromPath(adminImage);

            string[] roleNames = { RoleTypes.Admin, RoleTypes.Helpdesk, RoleTypes.Customer };

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
                Adress = "Admin Address",
                ProfileImage = adminImageByte
            };

            if (userManager.Users.All(u => u.UserName != adminUser.UserName))
            {
                await userManager.CreateAsync(adminUser, "Password123.");
                await userManager.AddToRoleAsync(adminUser, RoleTypes.Admin);
            }

            byte[] hdImageByte = GetImageBytesFromPath(helpDeskImage);

            for (int i = 1; i <= 3; i++)
            {
                var helpdeskUser = new AppUser
                {
                    UserName = $"helpdesk{i}",
                    Email = $"helpdesk{i}@example.com",
                    Name = $"Helpdesk{i}",
                    Surname = "User",
                    Adress = $"Helpdesk Address {i}",
                    IsApproved = true,
                    ProfileImage = hdImageByte
                };

                if (userManager.Users.All(u => u.UserName != helpdeskUser.UserName))
                {
                    await userManager.CreateAsync(helpdeskUser, "Password123.");
                    await userManager.AddToRoleAsync(helpdeskUser, RoleTypes.Helpdesk);
                }
            }

            byte[] csImageByte = GetImageBytesFromPath(customerImage);
            for (int i = 1; i <= 6; i++)
            {
                var customerUser = new AppUser
                {
                    UserName = $"customer{i}",
                    Email = $"customer{i}@example.com",
                    Name = $"Customer{i}",
                    Surname = "User",
                    Adress = $"Customer Address {i}",
                    ProfileImage = csImageByte
                };

                if (userManager.Users.All(u => u.UserName != customerUser.UserName))
                {
                    await userManager.CreateAsync(customerUser, "Password123.");
                    await userManager.AddToRoleAsync(customerUser, RoleTypes.Customer);
                }
            }
        }
        public static List<Category> GetCategories()
        {
            var categories = new List<Category>()
            {
                new Category {Id = 1,  Name = "Home Electronics", Creator = "Seeder" },
                new Category {Id = 2,  Name = "Personal Devices", Creator = "Seeder" },
                new Category {Id = 3,  Name = "Fashion", Creator = "Seeder" },
                new Category {Id = 4,  Name = "Sports Equipment", Creator = "Seeder" },
                new Category {Id = 5,  Name = "Books", Creator = "Seeder" },
                new Category {Id = 6,  Name = "Toys & Games", Creator = "Seeder" },
                new Category {Id = 7,  Name = "Automotive", Creator = "Seeder" },
                new Category {Id = 8,  Name = "Beauty & Health", Creator = "Seeder" },
                new Category {Id = 9,  Name = "Furniture", Creator = "Seeder" },
                new Category {Id = 10, Name = "Kitchen Appliances", Creator = "Seeder" }
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
            Status= TicketStatus.Completed,
            ClosedAt= DateTime.Now.AddDays(-31),
            CategoryId = 1,
            IsPublished = true,
            CreatorId = 5,
            AssignedUserId = 2,
        },
        new Ticket{
            Id =2,
            Title = "Device malfunction",
            Content= "My personal device is not working.",
            Status= TicketStatus.Waiting,
            CategoryId = 6,
            IsPublished = true,
            CreatorId = 7,
            AssignedUserId = 4,
        },
        new Ticket{
            Id =3,
            Title = "Fashion query",
            Content= "I have a question about fashion.",
            Status= TicketStatus.Completed,
            ClosedAt= DateTime.Now.AddDays(-21),
            CategoryId = 3,
            IsPublished = true,
            CreatorId = 6,
            AssignedUserId= 4,
            UpdatedAt = DateTime.Now.AddDays(-5),
        },
        new Ticket{
            Id = 4,
            Title = "Sports equipment issue",
            Content= "Need assistance with sports gear.",
            Status= TicketStatus.Waiting,
            CategoryId = 4,
            IsPublished = true,
            CreatorId = 1,
            AssignedUserId = 2,
        },

        // Creator 2
        new Ticket{
            Id =5,
            Title = "Book availability",
            Content= "Looking for a specific book.",
            Status= TicketStatus.Waiting,
            CategoryId = 5,
            IsPublished = true,
            CreatorId = 7,
            AssignedUserId = 2,
        },
        new Ticket{
            Id =6,
            Title = "Toy malfunction",
            Content= "Toy is not working properly.",
            Status= TicketStatus.Completed,
            ClosedAt= DateTime.Now.AddDays(-13),
            CategoryId = 6,
            IsPublished = true,
            CreatorId = 7,
            AssignedUserId = 6,
            UpdatedAt = DateTime.Now.AddDays(-2),
        },
        new Ticket{
            Id =7,
            Title = "Car issue",
            Content= "Problem with my car.",
            Status= TicketStatus.Waiting,
            CategoryId = 7,
            IsPublished = true,
            CreatorId = 7,
            AssignedUserId = 3,
        },
        new Ticket{
            Id =8,
            Title = "Beauty product inquiry",
            Content= "Question about beauty products.",
            Status= TicketStatus.Waiting,
            CategoryId = 8,
            IsPublished = true,
            CreatorId = 7,
            AssignedUserId = 2,
        },

        // Creator 3
        new Ticket
        {
            Id =9,
            Title = "Furniture damage",
            Content= "My furniture is damaged.",
            Status= TicketStatus.Waiting,
            CategoryId = 9,
            IsPublished = true,
            CreatorId = 3,
            AssignedUserId = 3,
        },
        new Ticket
        {
            Id = 10,
            Title = "Kitchen appliance malfunction",
            Content= "Problem with my kitchen appliance.",
            Status= TicketStatus.Waiting,
            CategoryId = 10,
            IsPublished = true,
            CreatorId = 9,
            AssignedUserId = 2,
        },
        new Ticket{
            Id = 11,
            Title = "Electronics warranty",
            Content= "Need warranty details for electronics.",
            Status= TicketStatus.Waiting,
            CategoryId = 1,
            IsPublished = true,
            CreatorId = 9,
            AssignedUserId = 2,
        },
        new Ticket
        {
            Id = 12,
            Title = "Device return request",
            Content= "Want to return my device.",
            Status= TicketStatus.Waiting,
            CategoryId = 2,
            IsPublished = true,
            CreatorId = 5,
            AssignedUserId = 2,
        },

        // Creator 4
        new Ticket
        {
            Id =13,
            Title = "Fashion product exchange",
            Content= "Want to exchange a fashion item.",
            Status= TicketStatus.Waiting,
            CategoryId = 3,
            IsPublished = true,
            CreatorId = 9,
            AssignedUserId = 2,
        },
        new Ticket
        {
            Id =14,
            Title = "Sports equipment refund",
            Content= "Requesting refund for sports gear.",
            Status= TicketStatus.Waiting,
            CategoryId = 4,
            IsPublished = true,
            CreatorId = 5,
            AssignedUserId = 2,
        },
        new Ticket
        {
            Id =15,
            Title = "Book delivery delay",
            Content= "My book delivery is delayed.",
            Status= TicketStatus.Completed,
            ClosedAt= DateTime.Now.AddDays(-41),
            CategoryId = 5,
            IsPublished = true,
            CreatorId = 5,
            AssignedUserId = 4,
            UpdatedAt = DateTime.Now.AddDays(-5),
        },
        new Ticket
        {
            Id =16,
            Title = "Toy quality issue",
            Content= "Concern about toy quality.",
            Status= TicketStatus.Pending,
            CategoryId = 6,
            IsPublished = true,
            CreatorId = 6
        },

        // Creator 5
        new Ticket
        {
            Id =17,
            Title = "Car service inquiry",
            Content= "Question about car servicing.",
            Status= TicketStatus.Waiting,
            CategoryId = 7,
            IsPublished = true,
            CreatorId = 5,
            AssignedUserId = 3,
        },
        new Ticket
        {
            Id =18,
            Title = "Beauty product refund",
            Content= "Requesting refund for beauty product.",
            Status= TicketStatus.Waiting,
            CategoryId = 8,
            IsPublished = true,
            CreatorId = 5,
            AssignedUserId = 4,
        },
        new Ticket
        {
            Id =19,
            Title = "Furniture assembly help",
            Content= "Need help assembling furniture.",
            Status= TicketStatus.Completed,
            ClosedAt= DateTime.Now.AddDays(-31),
            CategoryId = 9,
            IsPublished = true,
            CreatorId = 5,
            AssignedUserId = 4,
            UpdatedAt= DateTime.Now.AddDays(-19),
        },
        new Ticket
        {
            Id =20,
            Title = "Kitchen appliance return",
            Content= "Want to return kitchen appliance.",
            Status= TicketStatus.Pending,
            CategoryId = 10,
            IsPublished = true,
            CreatorId = 5
        },

        // Creator 6
        new Ticket
        {
            Id =21,
            Title = "Electronics installation",
            Content= "Need help installing electronics.",
            Status= TicketStatus.Waiting,
            CategoryId = 1,
            IsPublished = true,
            CreatorId = 6,
            AssignedUserId = 2,
        },
        new Ticket
        {
            Id =22,
            Title = "Device repair request",
            Content= "Requesting repair for my device.",
            Status= TicketStatus.Waiting,
            CategoryId = 2,
            IsPublished = true,
            CreatorId = 6,
            AssignedUserId = 2,
        },
        new Ticket
        {
            Id =23,
            Title = "Fashion trend inquiry",
            Content= "Question about latest fashion trends.",
            Status= TicketStatus.Waiting,
            CategoryId = 3,
            IsPublished = true,
            CreatorId = 6,
            AssignedUserId = 2,
        },
        new Ticket
        {
            Id =24,
            Title = "Sports gear replacement",
            Content= "Need replacement for sports gear.",
            Status= TicketStatus.Waiting,
            CategoryId = 4,
            IsPublished = true,
            CreatorId = 6,
            AssignedUserId = 2,
        },

        // Creator 7
        new Ticket
        {
            Id =25,
            Title = "Book recommendation",
            Content= "Looking for book recommendations.",
            Status= TicketStatus.Waiting,
            CategoryId = 5,
            IsPublished = true,
            CreatorId = 7,
            AssignedUserId = 2,
        },
        new Ticket
        {
            Id =26,
            Title = "Toy return request",
            Content= "Want to return a toy.",
            Status= TicketStatus.Waiting,
            CategoryId = 6,
            IsPublished = true,
            CreatorId = 7,
            AssignedUserId = 2,
        },
        new Ticket
        {
            Id =27,
            Title = "Car parts inquiry",
            Content= "Question about car parts.",
            Status= TicketStatus.Waiting,
            CategoryId = 7,
            IsPublished = true,
            CreatorId = 7,
            AssignedUserId = 3,
        },
        new Ticket
        {
            Id =28,
            Title = "Beauty product query",
            Content= "Inquiry about beauty products.",
            Status= TicketStatus.Waiting,
            CategoryId = 8,
            IsPublished = true,
            CreatorId = 7,
            AssignedUserId = 3,
                                    CreatedAt = DateTime.Now.AddDays(-100),
        },

        // Creator 8
        new Ticket
        {
            Id =29,
            Title = "Furniture return",
            Content= "Want to return furniture.",
            Status= TicketStatus.Waiting,
            CategoryId = 9,
            IsPublished = true,
            CreatorId = 8,
            AssignedUserId = 3,
                                    CreatedAt = DateTime.Now.AddDays(-100),
        },
        new Ticket
        {
            Id =30,
            Title = "Kitchen appliance issue",
            Content= "Kitchen appliance is not functioning.",
            Status= TicketStatus.Waiting,
            CategoryId = 10,
            IsPublished = true,
            CreatorId = 8,
            AssignedUserId = 3,
                                    CreatedAt = DateTime.Now.AddDays(-100),
        },
        new Ticket
        {
            Id =31,
            Title = "Electronics technical help",
            Content= "Need technical help for electronics.",
            Status= TicketStatus.Waiting,
            CategoryId = 1,
            IsPublished = true,
            CreatorId = 8,
            AssignedUserId = 3,
                        CreatedAt = DateTime.Now.AddDays(-100),
        },
        new Ticket
        {
            Id =32,
            Title = "Device warranty extension",
            Content= "Requesting warranty extension.",
            Status= TicketStatus.Waiting,
            CategoryId = 2,
            IsPublished = true,
            CreatorId = 8,
            AssignedUserId = 4,
        },

        // Creator 9
        new Ticket
        {
            Id =33,
            Title = "Fashion item return",
            Content= "Want to return a fashion item.",
            Status= TicketStatus.Waiting,
            CategoryId = 3,
            IsPublished = true,
            CreatorId = 9,
            AssignedUserId = 4,
        },
        new Ticket
        {
            Id =34,
            Title = "Sports gear repair",
            Content= "Requesting repair for sports gear.",
            Status= TicketStatus.Waiting,
            CategoryId = 4,
            IsPublished = true,
            CreatorId = 9,
            AssignedUserId = 4,
        },
        new Ticket
        {
            Id =35,
            Title = "Book replacement",
            Content= "Need a replacement for my book.",
            Status= TicketStatus.Waiting,
            CategoryId = 5,
            IsPublished = true,
            CreatorId = 9,
            AssignedUserId = 4,
        },
        new Ticket
        {
            Id =36,
            Title = "Toy assembly issue",
            Content= "Need help with toy assembly.",
            Status= TicketStatus.Waiting,
            CategoryId = 6,
            IsPublished = true,
            CreatorId = 9,
            AssignedUserId = 4,
            CreatedAt = DateTime.Now.AddDays(-180),
        },

        // Creator 10
        new Ticket
        {
            Id =37,
            Title = "Car warranty",
            Content= "Question about car warranty.",
            Status= TicketStatus.Waiting,
            CategoryId = 7,
            IsPublished = true,
            CreatorId = 10,
            AssignedUserId = 4,
                        CreatedAt = DateTime.Now.AddDays(-180),
        },
        new Ticket
        {
            Id =38,
            Title = "Beauty product exchange",
            Content= "Want to exchange beauty products.",
            Status= TicketStatus.Waiting,
            CategoryId = 8,
            IsPublished = true,
            CreatorId = 10,
            AssignedUserId = 4,
                        CreatedAt = DateTime.Now.AddDays(-180),
        },
        new Ticket
        {
            Id =39,
            Title = "Furniture cleaning advice",
            Content= "Need advice on cleaning furniture.",
            Status= TicketStatus.Waiting, CategoryId = 9,
            IsPublished = true,
            CreatorId = 10,
            AssignedUserId = 4,
        },
        new Ticket
        {
            Id =40,
            Title = "Kitchen appliance inquiry",
            Content= "Inquiry about kitchen appliance features.",
            Status= TicketStatus.Waiting,
            CategoryId = 10,
            IsPublished = true,
            CreatorId = 10,
            AssignedUserId = 4,
        },
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
                    CreatedAt = DateTime.Now.AddDays(-13).AddMinutes(53),
                },
                 new Comment
                {
                    Id = 2,
                    Message = "We are working on it. It will be fixed soon.",
                    CreatorId = 2,
                    TicketId= 1,
                    CreatedAt = DateTime.Now.AddDays(-14).AddMinutes(-120),
                },
                  new Comment
                {
                    Id = 3,
                    Message = "Thank you for your efforts sir!",
                    CreatorId = 1,
                    TicketId= 1,
                    CreatedAt = DateTime.Now.AddDays(-15).AddMinutes(31),
                },
                  new Comment
                {
                    Id = 4,
                    Message = "Still waiting to solve ? Realy?",
                    CreatorId = 5,
                    TicketId= 4,
                    CreatedAt = DateTime.Now.AddDays(-13),
                },
            };

            return comments;
        }

        #endregion
    }
}
