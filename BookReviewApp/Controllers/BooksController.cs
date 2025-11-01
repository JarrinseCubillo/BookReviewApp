using BookReviewApp.Interfaces;
using BookReviewApp.Models;
using Microsoft.AspNetCore.Mvc;
namespace BookReviewApp.Controllers;

[Route("api/[controller]")]
[ApiController]
public class BooksController : ControllerBase
{
    private readonly IBookRepository _bookRepository;
    public BooksController(IBookRepository bookRepository)
    {
        _bookRepository = bookRepository;
    }

    [HttpGet]
    [ProducesResponseType(200, Type = typeof(IEnumerable<Book>))]
    public IActionResult GetBooks()
    {
        var books = _bookRepository.GetAllBooks();
        if(!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        return Ok(books);
    }

    [HttpGet("{bookId}")]
    [ProducesResponseType(200, Type = typeof(Book))]
    [ProducesResponseType(400)]
    public IActionResult GetBook(int bookId)
    {
        if (!_bookRepository.BookExists(bookId))
            return BadRequest("Book Not Found.");

        var book = _bookRepository.GetBook(bookId);

        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        return Ok(book);
    }

    [HttpGet("{bookId}/rating")]
    [ProducesResponseType(200, Type = typeof(decimal))]
    [ProducesResponseType(400)]
    public IActionResult GetBookRating (int bookId)
    {
        if (!_bookRepository.BookExists(bookId))
            return BadRequest("Book Not Found");

        var rating = _bookRepository.GetBookRating(bookId);

        if (!ModelState.IsValid)
            return BadRequest(ModelState);
        
        return Ok(rating);
    }
}