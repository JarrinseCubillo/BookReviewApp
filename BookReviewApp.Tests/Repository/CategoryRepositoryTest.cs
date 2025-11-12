using BookReviewApp.Data;
using BookReviewApp.Models;
using BookReviewApp.Repository;
using Microsoft.EntityFrameworkCore;

namespace BookReviewApp.Tests.Repository;

public class CategoryRepositoryTest
{
     private DataContext GetInMemoryContext()
        {
            var options = new DbContextOptionsBuilder<DataContext>()
                .UseInMemoryDatabase(databaseName: System.Guid.NewGuid().ToString()) 
                .Options;

            var context = new DataContext(options);

            var category1 = new Category { Id = 1, Name = "Programming" };
            var category2 = new Category { Id = 2, Name = "Databases" };
            var category3 = new Category { Id = 3, Name = "Web Development" };

            var book1 = new Book { Id = 1, Title = "C# Fundamentals" };
            var book2 = new Book { Id = 2, Title = "SQL Mastery" };
            var book3 = new Book { Id = 3, Title = "ASP.NET Core Guide" };

            var bc1 = new BookCategory { Id = 1, CategoryId = 1, Category = category1, Book = book1 };
            var bc2 = new BookCategory { Id = 2, CategoryId = 2, Category = category2, Book = book2 };
            var bc3 = new BookCategory { Id = 3, CategoryId = 3, Category = category3, Book = book3 };

            context.Categories.AddRange(category1, category2, category3);
            context.Books.AddRange(book1, book2, book3);
            context.BookCategories.AddRange(bc1, bc2, bc3);

            context.SaveChanges();

            return context;
        }

        [Fact]
        public void GetCategories_ReturnsAllCategoriesOrderedById()
        {
            var context = GetInMemoryContext();
            var repo = new CategoryRepository(context);

            var result = repo.GetCagories();

            Assert.Equal(3, result.Count);
            Assert.True(result.First().Id < result.Last().Id);
        }

        [Fact]
        public void GetCategory_ReturnsCorrectCategory()
        {
            var context = GetInMemoryContext();
            var repo = new CategoryRepository(context);

            var category = repo.GetCategory(2);

            Assert.NotNull(category);
            Assert.Equal("Databases", category.Name);
        }

        [Fact]
        public void GetBooksByCategory_ReturnsBooksInThatCategory()
        {
            var context = GetInMemoryContext();
            var repo = new CategoryRepository(context);

            var books = repo.GetBooksByCategory(3);

            Assert.Single(books);
            Assert.Equal("ASP.NET Core Guide", books.First().Title);
        }

        [Fact]
        public void CreateCategory_AddsCategorySuccessfully()
        {
            var context = GetInMemoryContext();
            var repo = new CategoryRepository(context);

            var newCategory = new Category { Id = 10, Name = "Machine Learning" };

            var result = repo.CreateCategory(newCategory);

            Assert.True(result);
            Assert.True(context.Categories.Any(c => c.Name == "Machine Learning"));
        }

        [Fact]
        public void UpdateCategory_ChangesCategorySuccessfully()
        {
            var context = GetInMemoryContext();
            var repo = new CategoryRepository(context);

            var category = context.Categories.First(c => c.Id == 1);
            category.Name = "Programming Updated";

            var result = repo.UpdateCategory(category);

            Assert.True(result);

            var updated = context.Categories.First(c => c.Id == 1);
            Assert.Equal("Programming Updated", updated.Name);
        }

        [Fact]
        public void DeleteCategory_RemovesCategorySuccessfully()
        {
            var context = GetInMemoryContext();
            var repo = new CategoryRepository(context);

            var category = context.Categories.First(c => c.Id == 2);

            var result = repo.DeleteCategory(category);

            Assert.True(result);
            Assert.False(context.Categories.Any(c => c.Id == 2));
        }

        [Fact]
        public void CategoryExists_ReturnsTrueIfExists()
        {
            var context = GetInMemoryContext();
            var repo = new CategoryRepository(context);

            var exists = repo.CategoryExists(1);

            Assert.True(exists);
        }

        [Fact]
        public void CategoryExists_ReturnsFalseIfNotExists()
        {
            var context = GetInMemoryContext();
            var repo = new CategoryRepository(context);

            var exists = repo.CategoryExists(999);

            Assert.False(exists);
        }
}