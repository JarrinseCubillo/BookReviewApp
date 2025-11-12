using BookReviewApp.Data;
using BookReviewApp.Models;
using BookReviewApp.Repository;
using Microsoft.EntityFrameworkCore;

namespace BookReviewApp.Tests.Repository;

public class BookRepositoryTest
{
      private DataContext GetInMemoryContext()
        {
            var options = new DbContextOptionsBuilder<DataContext>()
                .UseInMemoryDatabase(databaseName: System.Guid.NewGuid().ToString())
                .Options;

            var context = new DataContext(options);

            var book1 = new Book
            {
                Id = 1,
                Title = "C# Fundamentals",
                ReleaseDate = new DateTime(2022, 1, 1),
                Reviews = new List<Review>()
            };

            var book2 = new Book
            {
                Id = 2,
                Title = "ASP.NET Core Guide",
                ReleaseDate = new DateTime(2023, 5, 10),
                Reviews = new List<Review>()
            };

            var book3 = new Book
            {
                Id = 3,
                Title = "Entity Framework Deep Dive",
                ReleaseDate = new DateTime(2024, 3, 15),
                Reviews = new List<Review>()
            };

            
            var review1 = new Review
            {
                Id = 1,
                Title = "Good",
                Text = "Solid intro",
                Rating = 4,
                ReviewDate = DateTime.Now,
                Book = book1
            };

            var review2 = new Review
            {
                Id = 2,
                Title = "Excellent",
                Text = "Very detailed",
                Rating = 5,
                ReviewDate = DateTime.Now,
                Book = book1
            };

            var review3 = new Review
            {
                Id = 3,
                Title = "Average",
                Text = "Ok book",
                Rating = 3,
                ReviewDate = DateTime.Now,
                Book = book2
            };

            
            book1.Reviews.Add(review1);
            book1.Reviews.Add(review2);
            book2.Reviews.Add(review3);

            
            context.Books.AddRange(book1, book2, book3);
            context.Reviews.AddRange(review1, review2, review3);

            context.SaveChanges();

            return context;
        }

        [Fact]
        public void GetAllBooks_ReturnsAllBooksOrderedById()
        {
            var context = GetInMemoryContext();
            var repo = new BookRepository(context);

            var result = repo.GetAllBooks();

            Assert.Equal(3, result.Count);
            Assert.True(result.First().Id < result.Last().Id);
        }

        [Fact]
        public void GetBook_ReturnsCorrectBook()
        {
            var context = GetInMemoryContext();
            var repo = new BookRepository(context);

            var book = repo.GetBook(2);

            Assert.NotNull(book);
            Assert.Equal("ASP.NET Core Guide", book.Title);
        }

        [Fact]
        public void GetBookByTitle_IgnoresCaseAndSpaces()
        {
            var context = GetInMemoryContext();
            var repo = new BookRepository(context);

            var book = repo.GetBookByTitle("   c# fundamentals  "); 
            
            Assert.NotNull(book);
            Assert.Equal(1, book.Id);
        }

        [Fact]
        public void GetBookRating_ReturnsAverageRating()
        {
            var context = GetInMemoryContext();
            var repo = new BookRepository(context);

            var rating = repo.GetBookRating(1);

            Assert.Equal(4.5m, rating);
        }

        [Fact]
        public void GetBookRating_ReturnsZero_WhenNoReviews()
        {
            var context = GetInMemoryContext();
            var repo = new BookRepository(context);

            var rating = repo.GetBookRating(3);

            Assert.Equal(0, rating);
        }

        [Fact]
        public void BookExists_ReturnsTrue_WhenBookExists()
        {
            var context = GetInMemoryContext();
            var repo = new BookRepository(context);

            var exists = repo.BookExists(2);

            Assert.True(exists);
        }

        [Fact]
        public void BookExists_ReturnsFalse_WhenBookDoesNotExist()
        {
            var context = GetInMemoryContext();
            var repo = new BookRepository(context);

            var exists = repo.BookExists(999);

            Assert.False(exists);
        }
}