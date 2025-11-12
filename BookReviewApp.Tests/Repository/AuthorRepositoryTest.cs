using BookReviewApp.Data;
using BookReviewApp.Models;
using BookReviewApp.Repository;
using Microsoft.EntityFrameworkCore;

namespace BookReviewApp.Tests.Repository;

public class AuthorRepositoryTest
{
    private DataContext GetInMemoryContext()
    {
        var options = new DbContextOptionsBuilder<DataContext>()
            .UseInMemoryDatabase(databaseName: System.Guid.NewGuid().ToString()) 
            .Options;

        var context = new DataContext(options);

        
        var author1 = new Author { Id = 1, Name = "John", Bio = "Bio about author John." };
        var author2 = new Author { Id = 2, Name = "Jane", Bio = "Bio about author Jane." };
        var book1 = new Book { Id = 1, Title = "Book One" };
        var book2 = new Book { Id = 2, Title = "Book Two" };

        context.Authors.AddRange(author1, author2);
        context.Books.AddRange(book1, book2);

        context.BookAuthors.AddRange(
            new BookAuthor { AuthorId = 1, BookId = 1, Author = author1, Book = book1 },
            new BookAuthor { AuthorId = 2, BookId = 2, Author = author2, Book = book2 }
        );

        context.SaveChanges();
        return context;
    }
    
    [Fact]
    public void GetAllAuthors_ReturnsAllAuthors()
    {
        var context = GetInMemoryContext();
        var repo = new AuthorRepository(context);

        var result = repo.GetAllAuthors();

        Assert.Equal(2, result.Count);
    }
    
    [Fact]
    public void GetAuthorById_ReturnsCorrectAuthor()
    {
        var context = GetInMemoryContext();
        var repo = new AuthorRepository(context);

        var author = repo.GetAuthorById(1);

        Assert.NotNull(author);
        Assert.Equal("John", author.Name);
    }

    [Fact]
    public void GetAuthorsOfBooks_ReturnsAuthorsForBook()
    {
        var context = GetInMemoryContext();
        var repo = new AuthorRepository(context);

        var authors = repo.GetAuthorsOfBooks(1);

        Assert.Single(authors);
        Assert.Equal("John", authors.First().Name);
    }

    [Fact]
    public void GetBooksByAuthor_ReturnsBooksForAuthor()
    {
        var context = GetInMemoryContext();
        var repo = new AuthorRepository(context);

        var books = repo.GetBooksByAuthor(2);

        Assert.Single(books);
        Assert.Equal("Book Two", books.First().Title);
    }

    [Fact]
    public void AuthorExists_ReturnsTrueIfExists()
    {
        var context = GetInMemoryContext();
        var repo = new AuthorRepository(context);

        var exists = repo.AuthorExists(1);

        Assert.True(exists);
    }

    [Fact]
    public void AuthorExists_ReturnsFalseIfNotExists()
    {
        var context = GetInMemoryContext();
        var repo = new AuthorRepository(context);

        var exists = repo.AuthorExists(99);

        Assert.False(exists);
    }
}