using BookReviewApp.Controllers;
using BookReviewApp.DTO;
using BookReviewApp.Interfaces;
using BookReviewApp.Models;
using FakeItEasy;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;

namespace BookReviewApp.Tests.Controllers;

public class AuthorsControllerTests
{
    private readonly IAuthorRepository _authorRepository;
    private readonly AuthorsController _controller;

    public AuthorsControllerTests()
    {
        _authorRepository = A.Fake<IAuthorRepository>();
        _controller = new AuthorsController(_authorRepository);
    }

    [Fact]
    public void GetAuthors_ShouldReturnOk_WithListOfAuthors()
    {
        // Arrange
        var authors = new List<Author>
        {
            new Author { Id = 1, Name = "Author One", Bio = "Bio1" },
            new Author { Id = 2, Name = "Author Two", Bio = "Bio2" }
        };
        A.CallTo(() => _authorRepository.GetAllAuthors()).Returns(authors);

        // Act
        var result = _controller.GetAuthors();

        // Assert
        var okResult = result as OkObjectResult;
        okResult.Should().NotBeNull();
        okResult.StatusCode.Should().Be(200);

        var returnedAuthors = okResult.Value as IEnumerable<AuthorDTO>;
        returnedAuthors.Should().HaveCount(2);
        returnedAuthors.First().Name.Should().Be("Author One");
    }

    [Fact]
    public void GetAuthor_ShouldReturnBadRequest_WhenAuthorDoesNotExist()
    {
        // Arrange
        int authorId = 1;
        A.CallTo(() => _authorRepository.AuthorExists(authorId)).Returns(false);

        // Act
        var result = _controller.GetAuthor(authorId);

        // Assert
        var badRequest = result as BadRequestObjectResult;
        badRequest.Should().NotBeNull();
        badRequest.Value.Should().Be("Author not found.");
    }

    [Fact]
    public void GetAuthor_ShouldReturnOk_WithAuthor()
    {
        // Arrange
        int authorId = 1;
        var author = new Author { Id = 1, Name = "Author Benjamin", Bio = "This is a normal author." };
        A.CallTo(() => _authorRepository.AuthorExists(authorId)).Returns(true);
        A.CallTo(() => _authorRepository.GetAuthorById(authorId)).Returns(author);

        // Act
        var result = _controller.GetAuthor(authorId);

        // Assert
        var okResult = result as OkObjectResult;
        okResult.Should().NotBeNull();

        var dto = okResult.Value as AuthorDTO;
        dto.Should().NotBeNull();
        dto.Name.Should().Be("Author Benjamin");
    }

    [Fact]
    public void GetAuthorsOfBooks_ShouldReturnNotFound_WhenNoAuthorsFound()
    {
        // Arrange
        int bookId = 10;
        A.CallTo(() => _authorRepository.GetAuthorsOfBooks(bookId)).Returns(null);

        // Act
        var result = _controller.GetAuthorsOfBooks(bookId);

        // Assert
        var objectResult = result.Should().BeOfType<ObjectResult>().Subject;
        objectResult.StatusCode.Should().Be(500);
        objectResult.Value.Should().BeOfType<string>()
            .Which.Should().StartWith("An unexpected error occurred:");
    }

    [Fact]
    public void GetAuthorsOfBooks_ShouldReturnOk_WithAuthors()
    {
        // Arrange
        int bookId = 10;
        var authors = new List<Author>
        {
            new Author { Id = 1, Name = "Author Test", Bio = "Biography of Author Test" }
        };
        A.CallTo(() => _authorRepository.GetAuthorsOfBooks(bookId)).Returns(authors);

        // Act
        var result = _controller.GetAuthorsOfBooks(bookId);

        // Assert
        result.Should().BeOfType<OkObjectResult>();
        var okResult = result as OkObjectResult;
        var returnedAuthors = okResult.Value as IEnumerable<AuthorDTO>;
        returnedAuthors.Should().ContainSingle();
        returnedAuthors.First().Name.Should().Be("Author Test");
    }

    [Fact]
    public void GetBooksByAuthor_ShouldReturnBadRequest_WhenAuthorDoesNotExist()
    {
        // Arrange
        int authorId = 2;
        A.CallTo(() => _authorRepository.AuthorExists(authorId)).Returns(false);

        // Act
        var result = _controller.GetBooksByAuthor(authorId);

        // Assert
        result.Should().BeOfType<BadRequestObjectResult>()
            .Which.Value.Should().Be("Author not found.");
    }

    [Fact]
    public void GetBooksByAuthor_ShouldReturnOk_WithBooks()
    {
        // Arrange
        int authorId = 2;
        var books = new List<Book>
        {
            new Book { Id = 1, Title = "Book BestSeller", ReleaseDate = new DateTime(2025, 1, 1) }
        };
        A.CallTo(() => _authorRepository.AuthorExists(authorId)).Returns(true);
        A.CallTo(() => _authorRepository.GetBooksByAuthor(authorId)).Returns(books);

        // Act
        var result = _controller.GetBooksByAuthor(authorId);

        // Assert
        var okResult = result as OkObjectResult;
        okResult.Should().NotBeNull();
        var returnedBooks = okResult.Value as IEnumerable<BookDTO>;
        returnedBooks.Should().ContainSingle();
        returnedBooks.First().Title.Should().Be("Book BestSeller");
    }
}