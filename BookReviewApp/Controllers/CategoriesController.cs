using BookReviewApp.DTO;
using BookReviewApp.Interfaces;
using BookReviewApp.Models;
using Microsoft.AspNetCore.Mvc;

namespace BookReviewApp.Controllers;

[Route("api/[controller]")]
[ApiController]

public class CategoriesController:ControllerBase
{
   private readonly ICategoryRepository _categoryRepository;

   public CategoriesController(ICategoryRepository categoryRepository)
   {
      _categoryRepository = categoryRepository;
   }

   [HttpGet]
   [ProducesResponseType(200, Type = typeof(IEnumerable<CategoryDTO>))]
   public IActionResult GetCategories()
   {
      var categoriesDtos = _categoryRepository.GetCagories().Select(c=> new CategoryDTO
      {
         Id=c.Id,
         Name =c.Name
      }).ToList();
      if (!ModelState.IsValid)
         return BadRequest(ModelState);
      return Ok(categoriesDtos);
   }

   [HttpGet("{categoryId}")]
   [ProducesResponseType(200, Type=typeof(CategoryDTO))]
   [ProducesResponseType(400)]
   public IActionResult GetCategory(int id)
   {
      if (!_categoryRepository.CategoryExists(id))
         return BadRequest("Category Not Found.");
      var categoryDto = _categoryRepository.GetCategory(id) is var c ? new CategoryDTO { Id = c.Id, Name = c.Name } : null;
      if (!ModelState.IsValid)
         return BadRequest(ModelState);
      return Ok(categoryDto);
   }

   [HttpGet("{categoryId}/books")]
   [ProducesResponseType(200, Type = typeof(Book))]
   [ProducesResponseType(400)]
   public IActionResult GetBooksByCategory(int categoryId)
   {
      if (!_categoryRepository.CategoryExists(categoryId))
         return BadRequest("Category not found.");
      var books = _categoryRepository.GetBooksByCategory(categoryId);
      if (!ModelState.IsValid)
         return BadRequest(ModelState);
      return Ok(books);
   }

   [HttpPost]
   [ProducesResponseType(204)]
   [ProducesResponseType(400)]
   public IActionResult CreateCategory([FromBody] CategoryDTO newCategory)
   {
      if (newCategory == null)
         return BadRequest("Invalid Category Data.");
      var existingCategory = _categoryRepository.GetCagories()
         .FirstOrDefault(c => c.Name.Trim().ToUpper() == newCategory.Name.Trim().ToUpper());
      if (existingCategory != null)
      {
         ModelState.AddModelError("","Category Already Exists.");
         return StatusCode(422);
      }

      var createdCategory = new Category { Name = newCategory.Name };
      if (!_categoryRepository.CreateCategory(createdCategory))
      {
         ModelState.AddModelError("","Something went wrong while saving the category.");
         return StatusCode(500,ModelState);
      }
      return Ok("Category successfully created!");
   }
   
   [HttpPut("{categoryId}")]
   [ProducesResponseType(204)]
   [ProducesResponseType(400)]
   public IActionResult UpdateCategory(int categoryId,[FromBody] CategoryDTO updatedCategory)
   {
      if (updatedCategory == null || categoryId != updatedCategory.Id)
         return BadRequest("Invalid category data.");
      if (!_categoryRepository.CategoryExists(categoryId))
         return BadRequest("Category not found");
      var categoryToUpdate = _categoryRepository.GetCategory(categoryId);
      categoryToUpdate.Name = updatedCategory.Name;
      if (!ModelState.IsValid)
         return BadRequest(ModelState);
      _categoryRepository.UpdateCategory(categoryToUpdate);
      return Ok("Category successfully updated!");
   }

   [HttpDelete("{categoryId}")]
   [ProducesResponseType(204)]
   [ProducesResponseType(400)]
   public IActionResult DeleteCategory(int categoryId)
   {
      if (!_categoryRepository.CategoryExists(categoryId))
         return BadRequest("Category not found.");
      var categoryToDelete = _categoryRepository.GetCategory(categoryId);
      if (!ModelState.IsValid)
         return BadRequest(ModelState);
      if (!_categoryRepository.DeleteCategory(categoryToDelete))
      {
         ModelState.AddModelError("","Something went wrong while deleting the category.");
         return StatusCode(500, ModelState);
      }
      return Ok("Category successfully deleted!");
   }
   
}