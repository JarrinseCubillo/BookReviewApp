using BookReviewApp.Models;
using Microsoft.EntityFrameworkCore;

namespace BookReviewApp.Data;

public class DataContext : DbContext
{
    public DataContext(DbContextOptions<DataContext> options): base(options) { }
    
    public DbSet<Author> Authors { get; set; }
    public DbSet<Book> Books { get; set; }
    public DbSet<BookAuthor> BookAuthors { get; set; }
    public DbSet<BookCategory> BookCategories { get; set; }
    public DbSet<Category> Categories { get; set; }
    public DbSet<Country> Countries { get; set; }
    public DbSet<Review> Reviews { get; set; }
    
    public DbSet<Reviewer>  Reviewers { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        
        modelBuilder.Entity<BookAuthor>()
            .HasKey(ba => new { ba.BookId, ba.AuthorId });
        modelBuilder.Entity<BookCategory>()
            .HasKey(bc => new {bc.Id, bc.CategoryId });
        base.OnModelCreating(modelBuilder);
    }
}