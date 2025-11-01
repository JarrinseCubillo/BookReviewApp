using BookReviewApp.Data;
using BookReviewApp.Interfaces;
using BookReviewApp.Models;

namespace BookReviewApp.Repository;

public class BookRepository : IBookRepository
{
    private readonly DataContext _context;
    public BookRepository(DataContext context)
    {
        _context = context;
    }

    public ICollection<Book> GetAllBooks()
    {
        return _context.Books.OrderBy(b=>b.Id).ToList();
    }

    public Book GetBook(int id)
    {
        return _context.Books.Where(b=>b.Id==id).FirstOrDefault();
    }

    public Book GetBookByTitle(string title)
    {
        return _context.Books.Where(b => b.Title.Trim().ToUpper() == title.Trim().ToUpper()).FirstOrDefault();
    }

    public decimal GetBookRating(int bookId)
    {
        var reviews = _context.Reviews.Where(r => r.Book.Id == bookId);
        if (!reviews.Any())
            return 0;
        return (decimal)reviews.Average(r => r.Rating);
    }

    public bool BookExists(int bookId)
    {
        return _context.Books.Any(b => b.Id == bookId);
    }
}