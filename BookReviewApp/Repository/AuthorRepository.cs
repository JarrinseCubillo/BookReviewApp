using BookReviewApp.Data;
using BookReviewApp.Interfaces;
using BookReviewApp.Models;

namespace BookReviewApp.Repository;

public class AuthorRepository:IAuthorRepository
{
    private readonly DataContext _context;

    public AuthorRepository(DataContext context)
    {
        _context = context;
    }

    public ICollection<Author> GetAllAuthors()
    {
        return _context.Authors.ToList();
    }

    public Author GetAuthorById(int authorId)
    {
        return _context.Authors.Where(a => a.Id == authorId).FirstOrDefault();
    }

    public ICollection<Author> GetAuthorsOfBooks(int bookId)
    {
        return _context.BookAuthors.Where(ba => ba.BookId == bookId).Select(ba => ba.Author).ToList();
    }
    
    public ICollection<Book> GetBooksByAuthor(int authorId)
    {
        return _context.BookAuthors.Where(ba => ba.AuthorId == authorId).Select(ba => ba.Book).ToList();
    }

    public bool AuthorExists(int authorId)
    {
        return _context.Authors.Any(a => a.Id == authorId);
    }
}