using BookReviewApp.Data;
using BookReviewApp.Models;
using BookReviewApp.Repository;
using Microsoft.EntityFrameworkCore;

namespace BookReviewApp.Tests.Repository;

public class ReviewerRepositoryTest
{
    private DataContext GetInMemoryContext()
        {
            var options = new DbContextOptionsBuilder<DataContext>()
                .UseInMemoryDatabase(databaseName: System.Guid.NewGuid().ToString()) 
                .Options;

            var context = new DataContext(options);
            
            var reviewer1 = new Reviewer
            {
                Id = 1,
                FirstName = "Alice",
                LastName = "Johnson",
                Reviews = new List<Review>()
            };

            var reviewer2 = new Reviewer
            {
                Id = 2,
                FirstName = "Bob",
                LastName = "Smith",
                Reviews = new List<Review>()
            };

            var book1 = new Book { Id = 1, Title = "C# Fundamentals" };
            var book2 = new Book { Id = 2, Title = "ASP.NET Core Guide" };

            var review1 = new Review
            {
                Id = 1,
                Title = "Great Book",
                Text = "Very helpful for beginners.",
                Rating = 5,
                ReviewDate = DateTime.Now,
                Reviewer = reviewer1,
                Book = book1
            };

            var review2 = new Review
            {
                Id = 2,
                Title = "Good reference",
                Text = "Nice examples.",
                Rating = 4,
                ReviewDate = DateTime.Now,
                Reviewer = reviewer1,
                Book = book2
            };

            var review3 = new Review
            {
                Id = 3,
                Title = "Not bad",
                Text = "Could be better organized.",
                Rating = 3,
                ReviewDate = DateTime.Now,
                Reviewer = reviewer2,
                Book = book1
            };

            reviewer1.Reviews.Add(review1);
            reviewer1.Reviews.Add(review2);
            reviewer2.Reviews.Add(review3);

            context.Reviewers.AddRange(reviewer1, reviewer2);
            context.Reviews.AddRange(review1, review2, review3);
            context.Books.AddRange(book1, book2);

            context.SaveChanges();
            return context;
        }

        [Fact]
        public void GetReviewers_ReturnsAllReviewers()
        {
            var context = GetInMemoryContext();
            var repo = new ReviewerRepository(context);

            var reviewers = repo.GetReviewers();

            Assert.Equal(2, reviewers.Count);
            Assert.Contains(reviewers, r => r.FirstName == "Alice");
            Assert.Contains(reviewers, r => r.FirstName == "Bob");
        }

        [Fact]
        public void GetReviewer_ReturnsCorrectReviewer()
        {
            var context = GetInMemoryContext();
            var repo = new ReviewerRepository(context);

            var reviewer = repo.GetReviewer(2);

            Assert.NotNull(reviewer);
            Assert.Equal("Bob", reviewer.FirstName);
        }

        [Fact]
        public void GetReviewsByReviewer_ReturnsAllReviewsForThatReviewer()
        {
            var context = GetInMemoryContext();
            var repo = new ReviewerRepository(context);

            var reviews = repo.GetReviewsByReviewer(1);

            Assert.Equal(2, reviews.Count);
            Assert.All(reviews, r => Assert.Equal(1, r.Reviewer.Id));
        }

        [Fact]
        public void ReviewerExists_ReturnsTrueIfExists()
        {
            var context = GetInMemoryContext();
            var repo = new ReviewerRepository(context);

            var exists = repo.ReviewerExists(1);

            Assert.True(exists);
        }

        [Fact]
        public void ReviewerExists_ReturnsFalseIfNotExists()
        {
            var context = GetInMemoryContext();
            var repo = new ReviewerRepository(context);

            var exists = repo.ReviewerExists(99);

            Assert.False(exists);
        }
}