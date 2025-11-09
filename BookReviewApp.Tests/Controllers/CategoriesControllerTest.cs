using BookReviewApp.Controllers;
using BookReviewApp.DTO;
using BookReviewApp.Interfaces;
using BookReviewApp.Models;
using FakeItEasy;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;

namespace BookReviewApp.Tests.Controllers;

public class CategoriesControllerTest
{
     private readonly ICategoryRepository _categoryRepository;
     private readonly CategoriesController _controller;

    public CategoriesControllerTest()
    {
        _categoryRepository = A.Fake<ICategoryRepository>();
        _controller = new CategoriesController(_categoryRepository);
    }

    // ---------- GET /api/categories ----------
    [Fact]
    public void GetCategories_ShouldReturnOk_WithCategories()
    {
        var categories = new List<Category>
        {
            new Category { Id = 1, Name = "Fiction" },
            new Category { Id = 2, Name = "Science" }
        };
        A.CallTo(() => _categoryRepository.GetCagories()).Returns(categories);

        var result = _controller.GetCategories();

        var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
        var returnedCategories = okResult.Value.Should().BeAssignableTo<IEnumerable<CategoryDTO>>().Subject;
        returnedCategories.Should().HaveCount(2);
    }

    [Fact]
    public void GetCategories_ShouldReturnBadRequest_WhenModelStateIsInvalid()
    {
        _controller.ModelState.AddModelError("Error", "Invalid model state");

        var result = _controller.GetCategories();

        result.Should().BeOfType<BadRequestObjectResult>();
    }

    // ---------- GET /api/categories/{categoryId} ----------
    [Fact]
    public void GetCategory_ShouldReturnBadRequest_WhenCategoryDoesNotExist()
    {
        int categoryId = 1;
        A.CallTo(() => _categoryRepository.CategoryExists(categoryId)).Returns(false);

        var result = _controller.GetCategory(categoryId);

        var badRequest = result.Should().BeOfType<BadRequestObjectResult>().Subject;
        badRequest.Value.Should().Be("Category Not Found.");
    }

    [Fact]
    public void GetCategory_ShouldReturnOk_WithCategory()
    {
        int categoryId = 1;
        var category = new Category { Id = 1, Name = "Fiction" };
        A.CallTo(() => _categoryRepository.CategoryExists(categoryId)).Returns(true);
        A.CallTo(() => _categoryRepository.GetCategory(categoryId)).Returns(category);

        var result = _controller.GetCategory(categoryId);

        var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
        var returnedCategory = okResult.Value.Should().BeAssignableTo<CategoryDTO>().Subject;
        returnedCategory.Name.Should().Be("Fiction");
    }

    // ---------- GET /api/categories/{categoryId}/books ----------
    [Fact]
    public void GetBooksByCategory_ShouldReturnBadRequest_WhenCategoryDoesNotExist()
    {
        int categoryId = 1;
        A.CallTo(() => _categoryRepository.CategoryExists(categoryId)).Returns(false);

        var result = _controller.GetBooksByCategory(categoryId);

        var badRequest = result.Should().BeOfType<BadRequestObjectResult>().Subject;
        badRequest.Value.Should().Be("Category not found.");
    }

    [Fact]
    public void GetBooksByCategory_ShouldReturnOk_WithBooks()
    {
        int categoryId = 1;
        var books = new List<Book>
        {
            new Book { Id = 1, Title = "Book 1", ReleaseDate = new DateTime(2022,1,1) }
        };
        A.CallTo(() => _categoryRepository.CategoryExists(categoryId)).Returns(true);
        A.CallTo(() => _categoryRepository.GetBooksByCategory(categoryId)).Returns(books);

        var result = _controller.GetBooksByCategory(categoryId);

        var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
        var returnedBooks = okResult.Value.Should().BeAssignableTo<IEnumerable<Book>>().Subject;
        returnedBooks.Should().HaveCount(1);
    }

    // ---------- POST /api/categories ----------
    [Fact]
    public void CreateCategory_ShouldReturnBadRequest_WhenCategoryIsNull()
    {
        var result = _controller.CreateCategory(null);

        var badRequest = result.Should().BeOfType<BadRequestObjectResult>().Subject;
        badRequest.Value.Should().Be("Invalid Category Data.");
    }

    [Fact]
    public void CreateCategory_ShouldReturn422_WhenCategoryAlreadyExists()
    {
        var categoryDto = new CategoryDTO { Name = "Fiction" };
        var existingCategories = new List<Category> { new Category { Id = 1, Name = "Fiction" } };
        A.CallTo(() => _categoryRepository.GetCagories()).Returns(existingCategories);

        var result = _controller.CreateCategory(categoryDto);

        result.Should().BeOfType<StatusCodeResult>().Which.StatusCode.Should().Be(422);
    }

    [Fact]
    public void CreateCategory_ShouldReturnOk_WhenCategoryCreated()
    {
        var categoryDto = new CategoryDTO { Name = "NewCategory" };
        A.CallTo(() => _categoryRepository.GetCagories()).Returns(new List<Category>());
        A.CallTo(() => _categoryRepository.CreateCategory(A<Category>.Ignored)).Returns(true);

        var result = _controller.CreateCategory(categoryDto);

        var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
        okResult.Value.Should().Be("Category successfully created!");
    }

    [Fact]
    public void CreateCategory_ShouldReturn500_WhenCreateFails()
    {
        var categoryDto = new CategoryDTO { Name = "NewCategory" };
        A.CallTo(() => _categoryRepository.GetCagories()).Returns(new List<Category>());
        A.CallTo(() => _categoryRepository.CreateCategory(A<Category>.Ignored)).Returns(false);

        var result = _controller.CreateCategory(categoryDto);

        var statusResult = result.Should().BeOfType<ObjectResult>().Subject;
        statusResult.StatusCode.Should().Be(500);
    }

    // ---------- PUT /api/categories/{categoryId} ----------
    [Fact]
    public void UpdateCategory_ShouldReturnBadRequest_WhenCategoryInvalid()
    {
        var categoryDto = new CategoryDTO { Id = 1, Name = "Updated" };
        var result = _controller.UpdateCategory(2, categoryDto);

        var badRequest = result.Should().BeOfType<BadRequestObjectResult>().Subject;
        badRequest.Value.Should().Be("Invalid category data.");
    }

    [Fact]
    public void UpdateCategory_ShouldReturnBadRequest_WhenCategoryDoesNotExist()
    {
        int categoryId = 1;
        var categoryDto = new CategoryDTO { Id = 1, Name = "Updated" };
        A.CallTo(() => _categoryRepository.CategoryExists(categoryId)).Returns(false);

        var result = _controller.UpdateCategory(categoryId, categoryDto);

        var badRequest = result.Should().BeOfType<BadRequestObjectResult>().Subject;
        badRequest.Value.Should().Be("Category not found");
    }

    [Fact]
    public void UpdateCategory_ShouldReturnOk_WhenUpdated()
    {
        int categoryId = 1;
        var categoryDto = new CategoryDTO { Id = 1, Name = "Updated" };
        var category = new Category { Id = 1, Name = "OldName" };
        A.CallTo(() => _categoryRepository.CategoryExists(categoryId)).Returns(true);
        A.CallTo(() => _categoryRepository.GetCategory(categoryId)).Returns(category);

        var result = _controller.UpdateCategory(categoryId, categoryDto);

        var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
        okResult.Value.Should().Be("Category successfully updated!");
        category.Name.Should().Be("Updated");
    }

    // ---------- DELETE /api/categories/{categoryId} ----------
    [Fact]
    public void DeleteCategory_ShouldReturnBadRequest_WhenCategoryDoesNotExist()
    {
        int categoryId = 1;
        A.CallTo(() => _categoryRepository.CategoryExists(categoryId)).Returns(false);

        var result = _controller.DeleteCategory(categoryId);

        var badRequest = result.Should().BeOfType<BadRequestObjectResult>().Subject;
        badRequest.Value.Should().Be("Category not found.");
    }

    [Fact]
    public void DeleteCategory_ShouldReturnOk_WhenDeleted()
    {
        int categoryId = 1;
        var category = new Category { Id = 1, Name = "Test" };
        A.CallTo(() => _categoryRepository.CategoryExists(categoryId)).Returns(true);
        A.CallTo(() => _categoryRepository.GetCategory(categoryId)).Returns(category);
        A.CallTo(() => _categoryRepository.DeleteCategory(category)).Returns(true);

        var result = _controller.DeleteCategory(categoryId);

        var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
        okResult.Value.Should().Be("Category successfully deleted!");
    }

    [Fact]
    public void DeleteCategory_ShouldReturn500_WhenDeleteFails()
    {
        int categoryId = 1;
        var category = new Category { Id = 1, Name = "Test" };
        A.CallTo(() => _categoryRepository.CategoryExists(categoryId)).Returns(true);
        A.CallTo(() => _categoryRepository.GetCategory(categoryId)).Returns(category);
        A.CallTo(() => _categoryRepository.DeleteCategory(category)).Returns(false);

        var result = _controller.DeleteCategory(categoryId);

        var statusResult = result.Should().BeOfType<ObjectResult>().Subject;
        statusResult.StatusCode.Should().Be(500);
    }
}