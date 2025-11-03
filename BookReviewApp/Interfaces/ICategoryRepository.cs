using BookReviewApp.Models;

namespace BookReviewApp.Interfaces;

public interface ICategoryRepository
{
    ICollection<Category> GetCagories();
    Category GetCategory(int categoryId);
    ICollection<Book> GetBooksByCategory(int categoryId);
    bool CategoryExists(int categoryId);
}