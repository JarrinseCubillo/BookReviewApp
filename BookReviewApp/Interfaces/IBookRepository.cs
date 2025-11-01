using BookReviewApp.Models;
namespace BookReviewApp.Interfaces;

public interface IBookRepository
{
    ICollection<Book> GetAllBooks();
    Book GetBook(int id);
    Book GetBookByTitle(string title);
    decimal GetBookRating(int bookId);
    bool BookExists(int bookId);
}