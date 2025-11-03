using BookReviewApp.Data;
using BookReviewApp.Interfaces;
using BookReviewApp.Models;

namespace BookReviewApp.Repository;

public class CategoryRepository : ICategoryRepository
{
    private readonly DataContext _context;

    public CategoryRepository(DataContext context)
    {
        _context = context;
    }
    
    public ICollection<Category> GetCagories()
    {
        return _context.Categories.OrderBy(c => c.Id).ToList();
    }

    public Category GetCategory(int categoryId)
    {
        return _context.Categories.FirstOrDefault(c => c.Id == categoryId);
    }

    public ICollection<Book> GetBooksByCategory(int categoryId)
    {
        return _context.BookCategories.Where(bc => bc.CategoryId == categoryId)
            .Select(bc => bc.Book)
            .ToList();
    }

    public bool CategoryExists(int categoryId)
    {
        return _context.Categories.Any(c => c.Id == categoryId);
    }
}