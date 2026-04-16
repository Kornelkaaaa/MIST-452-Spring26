using BooksSpring26.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace BooksSpring26.Data
{
    public class BooksDbContext : IdentityDbContext
    {
        public BooksDbContext(DbContextOptions<BooksDbContext> options) : base(options)
        {
        }

        public DbSet<Category> Categories { get; set; }  // table: Categories
        public DbSet<Book> Books { get; set; }           // table: Books

        public DbSet<CartItem> CartItems { get; set; }     // table: CartItems

        public DbSet<ApplicationUser> ApplicationUsers { get; set; } // table: ApplicationUsers

        public DbSet<Order> Order   { get; set; }
        public DbSet<OrderDetail> OrderDetail { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Book>()
                .Property(b => b.Price)
                .HasPrecision(10, 2);


            // Seed Categories
            modelBuilder.Entity<Category>().HasData(
                new Category { CategoryId = 1, Name = "Travel", Description = "Travel books" },
                new Category { CategoryId = 2, Name = "Mystery", Description = "Mystery books" },
                new Category { CategoryId = 3, Name = "Science Fiction", Description = "Science Fiction books" },
                new Category { CategoryId = 4, Name = "Romance", Description = "Romance books" },
                new Category { CategoryId = 5, Name = "Guide", Description = "Guide books" }
            );

            // Seed Books
            modelBuilder.Entity<Book>().HasData(
                new Book
                {
                    BookId = 1,
                    Title = "Into the Wild",
                    Author = "Korn",
                    Description = "wow",
                    Price = 3.00m,
                    ImgUrl = "ufhwvhsvsns",
                    CategoryId = 1
                },
                new Book
                {
                    BookId = 7,
                    Title = "Across the Desert Winds",
                    Author = "Helen Mercer",
                    Description = "A gripping travelogue through the harsh Sahara dunes.",
                    Price = 11.00m,
                    ImgUrl = "img_desert_winds",
                    CategoryId = 1
                },
                new Book
                {
                    BookId = 8,
                    Title = "The Vanishing Key",
                    Author = "Mark Holloway",
                    Description = "A detective uncovers the truth behind a mysterious locked-room disappearance.",
                    Price = 8.90m,
                    ImgUrl = "img_vanishing_key",
                    CategoryId = 2
                },
                new Book
                {
                    BookId = 9,
                    Title = "Starbound Chronicles",
                    Author = "Rivka Hale",
                    Description = "A sci-fi adventure across galaxies filled with ancient civilizations and hidden dangers.",
                    Price = 16.00m,
                    ImgUrl = "img_starbound",
                    CategoryId = 3
                }
            );


        }
    }
}
