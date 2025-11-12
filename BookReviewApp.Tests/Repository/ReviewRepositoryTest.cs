using BookReviewApp.Data;
using BookReviewApp.Models;
using BookReviewApp.Repository;
using Microsoft.EntityFrameworkCore;

namespace BookReviewApp.Tests.Repository;

public class ReviewRepositoryTest
{
     private DataContext GetInMemoryContext()
        {
            var options = new DbContextOptionsBuilder<DataContext>()
                .UseInMemoryDatabase(databaseName: System.Guid.NewGuid().ToString()) 
                .Options;

            var context = new DataContext(options);

            var reviewer1 = new Reviewer { Id = 1, FirstName = "Alice", LastName = "Johnson" };
            var reviewer2 = new Reviewer { Id = 2, FirstName = "Bob", LastName = "Smith" };

            var book1 = new Book { Id = 1, Title = "C# Fundamentals" };
            var book2 = new Book { Id = 2, Title = "ASP.NET Core Guide" };

            var review1 = new Review
            {
                Id = 1,
                Title = "Excellent",
                Text = "Really helpful.",
                Rating = 5,
                ReviewDate = DateTime.Now,
                Reviewer = reviewer1,
                Book = book1
            };

            var review2 = new Review
            {
                Id = 2,
                Title = "Good",
                Text = "Well written.",
                Rating = 4,
                ReviewDate = DateTime.Now,
                Reviewer = reviewer1,
                Book = book2
            };

            var review3 = new Review
            {
                Id = 3,
                Title = "Average",
                Text = "Could be better.",
                Rating = 3,
                ReviewDate = DateTime.Now,
                Reviewer = reviewer2,
                Book = book1
            };

            context.Reviewers.AddRange(reviewer1, reviewer2);
            context.Books.AddRange(book1, book2);
            context.Reviews.AddRange(review1, review2, review3);

            context.SaveChanges();

            return context;
        }

        [Fact]
        public void GetReviews_ReturnsAllReviews()
        {
            var context = GetInMemoryContext();
            var repo = new ReviewRepository(context);

            var result = repo.GetReviews();

            Assert.Equal(3, result.Count);
            Assert.Contains(result, r => r.Title == "Excellent");
            Assert.Contains(result, r => r.Title == "Average");
        }

        [Fact]
        public void GetReview_ReturnsCorrectReview()
        {
            var context = GetInMemoryContext();
            var repo = new ReviewRepository(context);

            var review = repo.GetReview(2);

            Assert.NotNull(review);
            Assert.Equal("Good", review.Title);
            Assert.Equal(4, review.Rating);
        }

        [Fact]
        public void GetReviewsOfBook_ReturnsReviewsForSpecificBook()
        {
            var context = GetInMemoryContext();
            var repo = new ReviewRepository(context);

            var reviews = repo.GetReviewsOfBook(1);

            Assert.Equal(2, reviews.Count);
            Assert.All(reviews, r => Assert.Equal(1, r.Book.Id));
        }

        [Fact]
        public void ReviewExists_ReturnsTrueIfExists()
        {
            var context = GetInMemoryContext();
            var repo = new ReviewRepository(context);

            var exists = repo.ReviewExists(3);

            Assert.True(exists);
        }

        [Fact]
        public void ReviewExists_ReturnsFalseIfNotExists()
        {
            var context = GetInMemoryContext();
            var repo = new ReviewRepository(context);

            var exists = repo.ReviewExists(999);

            Assert.False(exists);
        }
}