using BookReviewApp.DTO;
using BookReviewApp.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace BookReviewApp.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ReviewersController:ControllerBase
{
    private readonly IReviewerRepository _reviewerRepository;

    public ReviewersController(IReviewerRepository reviewerRepository)
    {
        _reviewerRepository = reviewerRepository;
    }

    [HttpGet]
    [ProducesResponseType(200, Type = typeof(IEnumerable<ReviewerDTO>))]
    public IActionResult GetReviewers()
    {
        var reviewers = _reviewerRepository.GetReviewers().Select(rw=>new ReviewerDTO
        {
            Id=rw.Id,
            FirstName = rw.FirstName,
            LastName = rw.LastName,
        });
        if (!ModelState.IsValid)
            return BadRequest(ModelState);
        return Ok(reviewers);
    }

    [HttpGet("{reviewerId}")]
    [ProducesResponseType(200, Type = typeof(ReviewerDTO))]
    [ProducesResponseType(400)]
    public IActionResult GetReviewer(int reviewerId)
    {
        if (!_reviewerRepository.ReviewerExists(reviewerId))
            return BadRequest("Reviewer Not Found.");
        var reviewer = _reviewerRepository.GetReviewer(reviewerId)is var rw?new ReviewerDTO(){Id=rw.Id,FirstName = rw.FirstName,LastName = rw.LastName,}:null;
        if (!ModelState.IsValid)
            return BadRequest(ModelState);
        return Ok(reviewer);
    }

    [HttpGet("{reviewerId}/reviews")]
    [ProducesResponseType(200,Type = typeof(IEnumerable<ReviewDTO>))]
    [ProducesResponseType(400)]
    public IActionResult GetReviewsByReviewer(int reviewerId)
    {
        if (!_reviewerRepository.ReviewerExists(reviewerId))
            return BadRequest("Reviewer Not Found.");
        var reviews = _reviewerRepository.GetReviewsByReviewer(reviewerId).Select(r => new ReviewDTO
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
}