using BookReviewApp.Models;

namespace BookReviewApp.Interfaces;

public interface IReviewRepository
{
    ICollection<Review> GetReviews();
    Review GetReview(int reviewId);
    ICollection<Review> GetReviewsOfBook(int bookId);
    bool ReviewExists(int reviewId);
}