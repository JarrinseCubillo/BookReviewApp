using BookReviewApp.Controllers;
using BookReviewApp.DTO;
using BookReviewApp.Interfaces;
using BookReviewApp.Models;
using FakeItEasy;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;

namespace BookReviewApp.Tests
{
    public class AuthorsControllerTests
    {
        private readonly IAuthorRepository _authorRepository;
        private readonly AuthorsController _controller;

        public AuthorsControllerTests()
        {
            _authorRepository = A.Fake<IAuthorRepository>();
            _controller = new AuthorsController(_authorRepository);
        }

        // ---------- GET /api/authors ----------
        [Fact]
        public void GetAuthors_ShouldReturnOk_WithAuthors()
        {
            // Arrange
            var authors = new List<Author>
            {
                new Author { Id = 1, Name = "Author 1", Bio = "Bio 1" },
                new Author { Id = 2, Name = "Author 2", Bio = "Bio 2" }
            };
            A.CallTo(() => _authorRepository.GetAllAuthors()).Returns(authors);

            // Act
            var result = _controller.GetAuthors();

            // Assert
            var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
            var returnedAuthors = okResult.Value.Should().BeAssignableTo<IEnumerable<AuthorDTO>>().Subject;
            returnedAuthors.Should().HaveCount(2);
        }

        [Fact]
        public void GetAuthors_ShouldReturnBadRequest_WhenModelStateIsInvalid()
        {
            _controller.ModelState.AddModelError("Error", "Invalid model state");

            var result = _controller.GetAuthors();

            result.Should().BeOfType<BadRequestObjectResult>();
        }

        // ---------- GET /api/authors/{authorId} ----------
        [Fact]
        public void GetAuthor_ShouldReturnBadRequest_WhenAuthorDoesNotExist()
        {
            int authorId = 1;
            A.CallTo(() => _authorRepository.AuthorExists(authorId)).Returns(false);

            var result = _controller.GetAuthor(authorId);

            var badRequest = result.Should().BeOfType<BadRequestObjectResult>().Subject;
            badRequest.Value.Should().Be("Author not found.");
        }

        [Fact]
        public void GetAuthor_ShouldReturnOk_WithAuthor()
        {
            int authorId = 1;
            var author = new Author { Id = 1, Name = "Author 1", Bio = "Bio 1" };
            A.CallTo(() => _authorRepository.AuthorExists(authorId)).Returns(true);
            A.CallTo(() => _authorRepository.GetAuthorById(authorId)).Returns(author);

            var result = _controller.GetAuthor(authorId);

            var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
            var returnedAuthor = okResult.Value.Should().BeAssignableTo<AuthorDTO>().Subject;
            returnedAuthor.Name.Should().Be("Author 1");
        }

        // ---------- GET /api/authors/book/{bookId} ----------
        [Fact]
        public void GetAuthorsOfBooks_ShouldReturnNotFound_WhenNoAuthorsFound()
        {
            int bookId = 10;
            A.CallTo(() => _authorRepository.GetAuthorsOfBooks(bookId)).Returns(new List<Author>());

            var result = _controller.GetAuthorsOfBooks(bookId);

            var notFound = result.Should().BeOfType<NotFoundObjectResult>().Subject;
            notFound.Value.Should().Be("Book not found or Author is not assigned.");
        }

        [Fact]
        public void GetAuthorsOfBooks_ShouldReturnOk_WithAuthors()
        {
            int bookId = 5;
            var authors = new List<Author>
            {
                new Author { Id = 1, Name = "Author 1", Bio = "Bio 1" }
            };
            A.CallTo(() => _authorRepository.GetAuthorsOfBooks(bookId)).Returns(authors);

            var result = _controller.GetAuthorsOfBooks(bookId);

            var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
            var returnedAuthors = okResult.Value.Should().BeAssignableTo<IEnumerable<AuthorDTO>>().Subject;
            returnedAuthors.Should().HaveCount(1);
        }

        // ---------- GET /api/authors/{authorId}/books ----------
        [Fact]
        public void GetBooksByAuthor_ShouldReturnBadRequest_WhenAuthorDoesNotExist()
        {
            int authorId = 1;
            A.CallTo(() => _authorRepository.AuthorExists(authorId)).Returns(false);

            var result = _controller.GetBooksByAuthor(authorId);

            var badRequest = result.Should().BeOfType<BadRequestObjectResult>().Subject;
            badRequest.Value.Should().Be("Author not found.");
        }

        [Fact]
        public void GetBooksByAuthor_ShouldReturnOk_WithBooks()
        {
            int authorId = 1;
            var books = new List<Book>
            {
                new Book { Id = 1, Title = "Book 1", ReleaseDate = new DateTime(2021, 1, 1) }
            };
            A.CallTo(() => _authorRepository.AuthorExists(authorId)).Returns(true);
            A.CallTo(() => _authorRepository.GetBooksByAuthor(authorId)).Returns(books);

            var result = _controller.GetBooksByAuthor(authorId);

            var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
            var returnedBooks = okResult.Value.Should().BeAssignableTo<IEnumerable<BookDTO>>().Subject;
            returnedBooks.Should().HaveCount(1);
        }
    }
}