using BookReviewApp.DTO;
using BookReviewApp.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace BookReviewApp.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ReviewsController:ControllerBase
{
    private readonly IReviewRepository _reviewRepository;

    public ReviewsController(IReviewRepository reviewRepository)
    {
        _reviewRepository = reviewRepository;
    }

    [HttpGet]
    [ProducesResponseType(200, Type = typeof(IEnumerable<ReviewDTO>))]
    public IActionResult GetReviews()
    {
        var reviews = _reviewRepository.GetReviews().Select(r=> new ReviewDTO
        {
            Id=r.Id,
            Title = r.Title,
            Text = r.Text,
            Rating = r.Rating,
        });
        if (!ModelState.IsValid)
            return BadRequest(ModelState);
        return Ok(reviews);
    }

    [HttpGet("{reviewId}")]
    [ProducesResponseType(200, Type = typeof(ReviewDTO))]
    public IActionResult GetReview(int reviewId)
    {
        if (!_reviewRepository.ReviewExists(reviewId))
            return BadRequest("Review not found");
        var review = _reviewRepository.GetReview(reviewId) is var r ? new ReviewDTO()
        { 
            Id = r.Id,
            Title = r.Title,
            Text = r.Text,
            Rating = r.Rating,
        } : null;
        if (!ModelState.IsValid)
            return BadRequest(ModelState);
        return Ok(review);
    }

    [HttpGet("book/{bookId}")]
    [ProducesResponseType(200, Type = typeof(IEnumerable<ReviewDTO>))]
    [ProducesResponseType(400)]
    public IActionResult GetReviewsOfBook(int bookId)
    {
        var reviews = _reviewRepository.GetReviewsOfBook(bookId)
            .Select(r => new ReviewDTO
        {
          Id=r.Id,
          Title = r.Title,
          Text = r.Text,
          Rating = r.Rating
        });
        
        if (reviews == null || !reviews.Any())
            return BadRequest("Book not found or has no reviews assigned");
        if (!ModelState.IsValid)
            return BadRequest(ModelState);
        return Ok(reviews);
    }
}