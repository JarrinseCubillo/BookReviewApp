using BookReviewApp.DTO;
using BookReviewApp.Interfaces;
using BookReviewApp.Models;
using Microsoft.AspNetCore.Mvc;

namespace BookReviewApp.Controllers;

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
        var countries = _countryRepository.GetCountries();
        if (!ModelState.IsValid)
            return BadRequest(ModelState);
        return Ok(countries);
    }

    [HttpGet]
    [ProducesResponseType(200, Type = typeof(CountryDTO))]
    [ProducesResponseType(400)]
    public IActionResult GetCountry(int countryId)
    {
        if (!_countryRepository.CountryExists(countryId))
            return BadRequest("Country not found.");
        var country = _countryRepository.GetCountry(countryId);
        if (!ModelState.IsValid)
            return BadRequest(ModelState);
        return Ok(country);
    }

    [HttpGet("author/{authorId}")]
    [ProducesResponseType(200, Type = typeof(CountryDTO))]
    [ProducesResponseType(400)]
    public IActionResult GetCountryByAuthor(int authorId)
    {
        var country = _countryRepository.GetCountryByAuthor(authorId);
        if (country == null)
            return NotFound("Author not found or has no country assigned.");
        if (!ModelState.IsValid)
            return BadRequest(ModelState);
        return Ok(country);

    }

    [HttpGet("{countryId}/authors")]
    [ProducesResponseType(200, Type = typeof(CountryDTO))]
    [ProducesResponseType(400)]
    public IActionResult GetAuthorsFromCountry(int countryId)
    {
        if (!_countryRepository.CountryExists(countryId))
            return BadRequest("Country not found");
        var authors = _countryRepository.GetAuthorsFromCountry(countryId);
        if (!ModelState.IsValid)
            return BadRequest(ModelState);
        return Ok(authors);
    }
}