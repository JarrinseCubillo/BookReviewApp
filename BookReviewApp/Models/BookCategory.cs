namespace BookReviewApp.Models;

public class BookCategory
{
    public int Id { get; set; }
    
    public int CategoryId { get; set; }
    
    public Book Book { get; set; }
    
    public Category Category { get; set; }
    
}