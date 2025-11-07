using BookReviewApp.Models;

namespace BookReviewApp.Interfaces;

public interface ICategoryRepository
{
    ICollection<Category> GetCagories();
    Category GetCategory(int categoryId);
    ICollection<Book> GetBooksByCategory(int categoryId);
    bool CreateCategory(Category category);
    bool UpdateCategory(Category category);
    bool DeleteCategory(Category category);
    bool CategoryExists(int categoryId);
}