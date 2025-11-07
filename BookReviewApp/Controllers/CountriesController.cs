using BookReviewApp.DTO;
using BookReviewApp.Interfaces;
using BookReviewApp.Models;
using Microsoft.AspNetCore.Mvc;

namespace BookReviewApp.Controllers;

[Route("api/[controller]")]
[ApiController]
public class CountriesController: ControllerBase
{
    private readonly ICountryRepository _countryRepository;

    public CountriesController(ICountryRepository countryRepository)
    {
        _countryRepository = countryRepository;
    }

    [HttpGet]
    [ProducesResponseType(200, Type = typeof(IEnumerable<CountryDTO>))]
    public IActionResult GetCountries()
    {
        var countries = _countryRepository.GetCountries().Select(c=>new CountryDTO
        {
            Id=c.Id,
            Name = c.Name,
        }).ToList();
        if (!ModelState.IsValid)
            return BadRequest(ModelState);
        return Ok(countries);
    }

    [HttpGet("{countryId}")]
    [ProducesResponseType(200, Type = typeof(CountryDTO))]
    [ProducesResponseType(400)]
    public IActionResult GetCountry(int countryId)
    {
        if (!_countryRepository.CountryExists(countryId))
            return BadRequest("Country not found.");
        var country = _countryRepository.GetCountry(countryId)is var c?new CountryDTO()
        {
         Id=c.Id,
         Name = c.Name,
        }:null;
        if (!ModelState.IsValid)
            return BadRequest(ModelState);
        return Ok(country);
    }

    [HttpPut("{countryId}")]
    [ProducesResponseType(204)]
    [ProducesResponseType(400)]
    public IActionResult UpdateCountry(int countryId, [FromBody] CountryDTO updatedCountry)
    {
        if (updatedCountry == null || countryId != updatedCountry.Id)
            return BadRequest("Invalid country data.");
        if (!_countryRepository.CountryExists(countryId))
            return BadRequest("Country Not Found.");
        var countryToUpdate = _countryRepository.GetCountry(countryId);
        countryToUpdate.Name = updatedCountry.Name;
        if (!ModelState.IsValid)
            return BadRequest(ModelState);
        _countryRepository.UpdateCountry(countryToUpdate);
        return Ok("Country successfully updated!");
    }
    
    
    [HttpGet("author/{authorId}")]
    [ProducesResponseType(200, Type = typeof(CountryDTO))]
    [ProducesResponseType(400)]
    public IActionResult GetCountryByAuthor(int authorId)
    {
        var country = _countryRepository.GetCountryByAuthor(authorId) is var c?new CountryDTO()
        {
            Id = c.Id,
            Name=c.Name,
        }:null;
        if (country == null)
            return NotFound("Author not found or has no country assigned.");
        if (!ModelState.IsValid)
            return BadRequest(ModelState);
        return Ok(country);

    }

    [HttpGet("{countryId}/authors")]
    [ProducesResponseType(200, Type = typeof(IEnumerable<AuthorDTO>))]
    [ProducesResponseType(400)]
    public IActionResult GetAuthorsFromCountry(int countryId)
    {
        if (!_countryRepository.CountryExists(countryId))
            return BadRequest("Country not found");
        var authors = _countryRepository.GetAuthorsFromCountry(countryId).Select(a=>new AuthorDTO
        {
            Id=a.Id,
            Name = a.Name,
            Bio = a.Bio,
        }).ToList();
        if (!ModelState.IsValid)
            return BadRequest(ModelState);
        return Ok(authors);
    }
}