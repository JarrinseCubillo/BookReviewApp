using BookReviewApp.Controllers;
using BookReviewApp.DTO;
using BookReviewApp.Interfaces;
using BookReviewApp.Models;
using FakeItEasy;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;

namespace BookReviewApp.Tests.Controllers;

public class ReviewersControllerTest
{
     private readonly IReviewerRepository _reviewerRepository;
     private readonly ReviewersController _controller;

    public ReviewersControllerTest()
    {
        _reviewerRepository = A.Fake<IReviewerRepository>();
        _controller = new ReviewersController(_reviewerRepository);
    }

    // ---------- GET /api/reviewers ----------
    [Fact]
    public void GetReviewers_ShouldReturnOk_WithReviewers()
    {
        var reviewers = new List<Reviewer>
        {
            new Reviewer { Id = 1, FirstName = "John", LastName = "Doe" },
            new Reviewer { Id = 2, FirstName = "Jane", LastName = "Smith" }
        };
        A.CallTo(() => _reviewerRepository.GetReviewers()).Returns(reviewers);

        var result = _controller.GetReviewers();

        var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
        var returnedReviewers = okResult.Value.Should().BeAssignableTo<IEnumerable<ReviewerDTO>>().Subject;
        returnedReviewers.Should().HaveCount(2);
    }

    [Fact]
    public void GetReviewers_ShouldReturnBadRequest_WhenModelStateInvalid()
    {
        _controller.ModelState.AddModelError("Error", "Invalid model state");

        var result = _controller.GetReviewers();

        result.Should().BeOfType<BadRequestObjectResult>();
    }

    // ---------- GET /api/reviewers/{reviewerId} ----------
    [Fact]
    public void GetReviewer_ShouldReturnBadRequest_WhenReviewerDoesNotExist()
    {
        int reviewerId = 1;
        A.CallTo(() => _reviewerRepository.ReviewerExists(reviewerId)).Returns(false);

        var result = _controller.GetReviewer(reviewerId);

        var badRequest = result.Should().BeOfType<BadRequestObjectResult>().Subject;
        badRequest.Value.Should().Be("Reviewer Not Found.");
    }

    [Fact]
    public void GetReviewer_ShouldReturnOk_WithReviewer()
    {
        int reviewerId = 1;
        var reviewer = new Reviewer { Id = 1, FirstName = "John", LastName = "Doe" };
        A.CallTo(() => _reviewerRepository.ReviewerExists(reviewerId)).Returns(true);
        A.CallTo(() => _reviewerRepository.GetReviewer(reviewerId)).Returns(reviewer);

        var result = _controller.GetReviewer(reviewerId);

        var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
        var returnedReviewer = okResult.Value.Should().BeAssignableTo<ReviewerDTO>().Subject;
        returnedReviewer.FirstName.Should().Be("John");
        returnedReviewer.LastName.Should().Be("Doe");
    }

    // ---------- GET /api/reviewers/{reviewerId}/reviews ----------
    [Fact]
    public void GetReviewsByReviewer_ShouldReturnBadRequest_WhenReviewerDoesNotExist()
    {
        int reviewerId = 1;
        A.CallTo(() => _reviewerRepository.ReviewerExists(reviewerId)).Returns(false);

        var result = _controller.GetReviewsByReviewer(reviewerId);

        var badRequest = result.Should().BeOfType<BadRequestObjectResult>().Subject;
        badRequest.Value.Should().Be("Reviewer Not Found.");
    }

    [Fact]
    public void GetReviewsByReviewer_ShouldReturnOk_WithReviews()
    {
        int reviewerId = 1;
        var reviews = new List<Review>
        {
            new Review { Id = 1, Title = "Great Book", Text = "Loved it", Rating = 5 },
            new Review { Id = 2, Title = "Not bad", Text = "It was okay", Rating = 3 }
        };
        A.CallTo(() => _reviewerRepository.ReviewerExists(reviewerId)).Returns(true);
        A.CallTo(() => _reviewerRepository.GetReviewsByReviewer(reviewerId)).Returns(reviews);

        var result = _controller.GetReviewsByReviewer(reviewerId);

        var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
        var returnedReviews = okResult.Value.Should().BeAssignableTo<IEnumerable<ReviewDTO>>().Subject;
        returnedReviews.Should().HaveCount(2);
    }

    [Fact]
    public void GetReviewsByReviewer_ShouldReturnBadRequest_WhenModelStateInvalid()
    {
        _controller.ModelState.AddModelError("Error", "Invalid model state");

        var result = _controller.GetReviewsByReviewer(1);

        result.Should().BeOfType<BadRequestObjectResult>();
    }
}