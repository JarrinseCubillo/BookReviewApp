using BookReviewApp.DTO;
using BookReviewApp.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace BookReviewApp.Controllers;
[Route("api/[controller]")]
[ApiController]
public class AuthorsController:ControllerBase
{
    private readonly IAuthorRepository _authorRepository;

    public AuthorsController(IAuthorRepository authorRepository)
    {
        _authorRepository = authorRepository;
    }

    [HttpGet]
    [ProducesResponseType(200, Type = typeof(IEnumerable<AuthorDTO>))]
    public IActionResult GetAuthors()
    {
        var authors = _authorRepository.GetAllAuthors().Select(a=>new AuthorDTO
        {
            Id=a.Id,
            Name=a.Name,
            Bio = a.Bio,
        });
        if (!ModelState.IsValid)
            return BadRequest(ModelState);
        return Ok(authors);
    }
    
    [HttpGet("{authorId}")]
    [ProducesResponseType(200, Type = typeof(IEnumerable<AuthorDTO>))]
    [ProducesResponseType(400)]
    public IActionResult GetAuthor(int authorId)
    {
        if (!_authorRepository.AuthorExists(authorId))
            return BadRequest("Author not found.");
        var author = _authorRepository.GetAuthorById(authorId) is var a
            ? new AuthorDTO() { Id = a.Id, Name = a.Name, Bio = a.Bio }
            : null; 
        if (!ModelState.IsValid)
            return BadRequest(ModelState);
        return Ok(author);

    }

    [HttpGet("book/{bookId}")]
    [ProducesResponseType(200, Type = typeof(IEnumerable<AuthorDTO>))]
    [ProducesResponseType(400)]
    public IActionResult GetAuthorsOfBooks(int bookId)
    {
        var author = _authorRepository.GetAuthorsOfBooks(bookId).Select(a=>new AuthorDTO
        {
            Id=a.Id,
            Name=a.Name,
            Bio=a.Bio,
        });
        if (author == null)
            return NotFound("Book not found or Author is not assigned.");
        if (!ModelState.IsValid)
            return BadRequest(ModelState);
        return Ok(author);
    }

    [HttpGet("{authorId}/books")]
    [ProducesResponseType(200, Type = typeof(IEnumerable<BookDTO>))]
    [ProducesResponseType(400)]
    public IActionResult GetBooksByAuthor(int authorId)
    {
        if (!_authorRepository.AuthorExists(authorId))
            return BadRequest("Author not found.");
        var books = _authorRepository.GetBooksByAuthor(authorId).Select(ba => new BookDTO
        {
            Id = ba.Id,
            Title = ba.Title,
            ReleaseDate = ba.ReleaseDate,
        });
        if (!ModelState.IsValid)
            return BadRequest(ModelState);
        return Ok(books);
    }
}