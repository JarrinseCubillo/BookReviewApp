using BookReviewApp.Data;
using BookReviewApp.Models;
using BookReviewApp.Repository;
using Microsoft.EntityFrameworkCore;

namespace BookReviewApp.Tests.Repository;

public class CountryRepositoryTest
{
    private DataContext GetInMemoryContext()
        {
            var options = new DbContextOptionsBuilder<DataContext>()
                .UseInMemoryDatabase(databaseName: System.Guid.NewGuid().ToString()) 
                .Options;

            var context = new DataContext(options);

            var country1 = new Country { Id = 1, Name = "USA", Authors = new List<Author>() };
            var country2 = new Country { Id = 2, Name = "UK", Authors = new List<Author>() };
            var country3 = new Country { Id = 3, Name = "Canada", Authors = new List<Author>() };

            var author1 = new Author { Id = 1, Name = "John", Bio = "This is a biography by author John.", Country = country1 };
            var author2 = new Author { Id = 2, Name = "Jane", Bio = "This is a biography by author Jane.", Country = country2 };
            var author3 = new Author { Id = 3, Name = "Alex", Bio = "This is a biography by author Alex.", Country = country1 };
            
            country1.Authors.Add(author1);
            country1.Authors.Add(author3);
            country2.Authors.Add(author2);

            context.Countries.AddRange(country1, country2, country3);
            context.Authors.AddRange(author1, author2, author3);

            context.SaveChanges();
            return context;
        }

        [Fact]
        public void GetCountries_ReturnsAllCountriesOrderedById()
        {
            var context = GetInMemoryContext();
            var repo = new CountryRepository(context);

            var result = repo.GetCountries();

            Assert.Equal(3, result.Count);
            Assert.Equal("USA", result.First().Name);
        }

        [Fact]
        public void GetCountry_ReturnsCorrectCountry()
        {
            var context = GetInMemoryContext();
            var repo = new CountryRepository(context);

            var country = repo.GetCountry(2);

            Assert.NotNull(country);
            Assert.Equal("UK", country.Name);
        }

        [Fact]
        public void GetCountryByAuthor_ReturnsAuthorsCountry()
        {
            var context = GetInMemoryContext();
            var repo = new CountryRepository(context);

            var country = repo.GetCountryByAuthor(3);

            Assert.NotNull(country);
            Assert.Equal("USA", country.Name);
        }

        [Fact]
        public void GetAuthorsFromCountry_ReturnsAuthorsInThatCountry()
        {
            var context = GetInMemoryContext();
            var repo = new CountryRepository(context);

            var authors = repo.GetAuthorsFromCountry(1);

            Assert.Equal(2, authors.Count);
            Assert.Contains(authors, a => a.Name == "John");
            Assert.Contains(authors, a => a.Name == "Alex");
        }

        [Fact]
        public void UpdateCountry_ChangesCountrySuccessfully()
        {
            var context = GetInMemoryContext();
            var repo = new CountryRepository(context);

            var country = context.Countries.First(c => c.Id == 3);
            country.Name = "New Canada";

            var result = repo.UpdateCountry(country);

            Assert.True(result);

            var updated = context.Countries.First(c => c.Id == 3);
            Assert.Equal("New Canada", updated.Name);
        }

        [Fact]
        public void CountryExists_ReturnsTrueIfExists()
        {
            var context = GetInMemoryContext();
            var repo = new CountryRepository(context);

            var exists = repo.CountryExists(1);

            Assert.True(exists);
        }

        [Fact]
        public void CountryExists_ReturnsFalseIfNotExists()
        {
            var context = GetInMemoryContext();
            var repo = new CountryRepository(context);

            var exists = repo.CountryExists(99);

            Assert.False(exists);
        }
}