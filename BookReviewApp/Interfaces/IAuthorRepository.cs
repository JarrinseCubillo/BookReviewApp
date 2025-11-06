using BookReviewApp.Models;

namespace BookReviewApp.Interfaces;

public interface IAuthorRepository
{
    ICollection<Author> GetAllAuthors();

    Author GetAuthorById(int authorId);

    ICollection<Author> GetAuthorsOfBooks(int bookId);
    
    ICollection<Book> GetBooksByAuthor(int authorId);

    bool AuthorExists(int authorId);
}