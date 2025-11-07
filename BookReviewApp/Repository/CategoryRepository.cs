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

    public bool CreateCategory(Category category)
    {
        _context.Categories.Add(category);
        return _context.SaveChanges() > 0;
    }

    public bool UpdateCategory(Category category)
    {
        _context.Categories.Update(category);
        return _context.SaveChanges()>0;
    }

    public bool DeleteCategory(Category category)
    {
        _context.Categories.Remove(category);
        return _context.SaveChanges() > 0;
    }

    public bool CategoryExists(int categoryId)
    {
        return _context.Categories.Any(c => c.Id == categoryId);
    }
}