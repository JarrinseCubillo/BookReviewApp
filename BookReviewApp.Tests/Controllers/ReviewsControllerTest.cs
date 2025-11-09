using BookReviewApp.Controllers;
using BookReviewApp.DTO;
using BookReviewApp.Interfaces;
using BookReviewApp.Models;
using FakeItEasy;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;

namespace BookReviewApp.Tests.Controllers;

public class ReviewsControllerTest
{
    private readonly IReviewRepository _reviewRepository;
    private readonly ReviewsController _controller;

    public ReviewsControllerTest()
    {
        _reviewRepository = A.Fake<IReviewRepository>();
        _controller = new ReviewsController(_reviewRepository);
    }

    // ---------- GET /api/reviews ----------
    [Fact]
    public void GetReviews_ShouldReturnOk_WithReviews()
    {
        var reviews = new List<Review>
        {
            new Review { Id = 1, Title = "Book 1", Text = "Excellent", Rating = 5 },
            new Review { Id = 2, Title = "Book 2", Text = "Good", Rating = 4 }
        };
        A.CallTo(() => _reviewRepository.GetReviews()).Returns(reviews);

        var result = _controller.GetReviews();

        var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
        var returnedReviews = okResult.Value.Should().BeAssignableTo<IEnumerable<ReviewDTO>>().Subject;
        returnedReviews.Should().HaveCount(2);
    }

    [Fact]
    public void GetReviews_ShouldReturnBadRequest_WhenModelStateInvalid()
    {
        _controller.ModelState.AddModelError("Error", "Invalid ModelState");

        var result = _controller.GetReviews();

        result.Should().BeOfType<BadRequestObjectResult>();
    }

    // ---------- GET /api/reviews/{reviewId} ----------
    [Fact]
    public void GetReview_ShouldReturnBadRequest_WhenReviewDoesNotExist()
    {
        int reviewId = 1;
        A.CallTo(() => _reviewRepository.ReviewExists(reviewId)).Returns(false);

        var result = _controller.GetReview(reviewId);

        var badRequest = result.Should().BeOfType<BadRequestObjectResult>().Subject;
        badRequest.Value.Should().Be("Review not found");
    }

    [Fact]
    public void GetReview_ShouldReturnOk_WithReview()
    {
        int reviewId = 1;
        var review = new Review { Id = 1, Title = "Book 1", Text = "Excellent", Rating = 5 };
        A.CallTo(() => _reviewRepository.ReviewExists(reviewId)).Returns(true);
        A.CallTo(() => _reviewRepository.GetReview(reviewId)).Returns(review);

        var result = _controller.GetReview(reviewId);

        var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
        var returnedReview = okResult.Value.Should().BeAssignableTo<ReviewDTO>().Subject;
        returnedReview.Title.Should().Be("Book 1");
        returnedReview.Rating.Should().Be(5);
    }

    [Fact]
    public void GetReview_ShouldReturnBadRequest_WhenModelStateInvalid()
    {
        _controller.ModelState.AddModelError("Error", "Invalid ModelState");

        var result = _controller.GetReview(1);

        result.Should().BeOfType<BadRequestObjectResult>();
    }

    // ---------- GET /api/reviews/book/{bookId} ----------
    [Fact]
    public void GetReviewsOfBook_ShouldReturnBadRequest_WhenNoReviews()
    {
        int bookId = 1;
        A.CallTo(() => _reviewRepository.GetReviewsOfBook(bookId)).Returns(new List<Review>());

        var result = _controller.GetReviewsOfBook(bookId);

        var badRequest = result.Should().BeOfType<BadRequestObjectResult>().Subject;
        badRequest.Value.Should().Be("Book not found or has no reviews assigned");
    }

    [Fact]
    public void GetReviewsOfBook_ShouldReturnOk_WithReviews()
    {
        int bookId = 1;
        var reviews = new List<Review>
        {
            new Review { Id = 1, Title = "Book 1", Text = "Excellent", Rating = 5 },
            new Review { Id = 2, Title = "Book 1 Part 2", Text = "Good", Rating = 4 }
        };
        A.CallTo(() => _reviewRepository.GetReviewsOfBook(bookId)).Returns(reviews);

        var result = _controller.GetReviewsOfBook(bookId);

        var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
        var returnedReviews = okResult.Value.Should().BeAssignableTo<IEnumerable<ReviewDTO>>().Subject;
        returnedReviews.Should().HaveCount(2);
    }

    [Fact]
    public void GetReviewsOfBook_ShouldReturnBadRequest_WhenModelStateInvalid()
    {
        _controller.ModelState.AddModelError("Error", "Invalid ModelState");

        var result = _controller.GetReviewsOfBook(1);

        result.Should().BeOfType<BadRequestObjectResult>();
    }
}