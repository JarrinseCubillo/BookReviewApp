using BookReviewApp.Controllers;
using BookReviewApp.DTO;
using BookReviewApp.Interfaces;
using BookReviewApp.Models;
using FakeItEasy;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;

namespace BookReviewApp.Tests.Controllers;

public class CountriesControllerTest
{
 private readonly ICountryRepository _countryRepository;
    private readonly CountriesController _controller;

    public CountriesControllerTest()
    {
        _countryRepository = A.Fake<ICountryRepository>();
        _controller = new CountriesController(_countryRepository);
    }

    // ---------- GET /api/countries ----------
    [Fact]
    public void GetCountries_ShouldReturnOk_WithCountries()
    {
        var countries = new List<Country>
        {
            new Country { Id = 1, Name = "USA" },
            new Country { Id = 2, Name = "UK" }
        };
        A.CallTo(() => _countryRepository.GetCountries()).Returns(countries);

        var result = _controller.GetCountries();

        var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
        var returnedCountries = okResult.Value.Should().BeAssignableTo<IEnumerable<CountryDTO>>().Subject;
        returnedCountries.Should().HaveCount(2);
    }

    [Fact]
    public void GetCountries_ShouldReturnBadRequest_WhenModelStateInvalid()
    {
        _controller.ModelState.AddModelError("Error", "Invalid model state");

        var result = _controller.GetCountries();

        result.Should().BeOfType<BadRequestObjectResult>();
    }

    // ---------- GET /api/countries/{countryId} ----------
    [Fact]
    public void GetCountry_ShouldReturnBadRequest_WhenCountryDoesNotExist()
    {
        int countryId = 1;
        A.CallTo(() => _countryRepository.CountryExists(countryId)).Returns(false);

        var result = _controller.GetCountry(countryId);

        var badRequest = result.Should().BeOfType<BadRequestObjectResult>().Subject;
        badRequest.Value.Should().Be("Country not found.");
    }

    [Fact]
    public void GetCountry_ShouldReturnOk_WithCountry()
    {
        int countryId = 1;
        var country = new Country { Id = 1, Name = "USA" };
        A.CallTo(() => _countryRepository.CountryExists(countryId)).Returns(true);
        A.CallTo(() => _countryRepository.GetCountry(countryId)).Returns(country);

        var result = _controller.GetCountry(countryId);

        var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
        var returnedCountry = okResult.Value.Should().BeAssignableTo<CountryDTO>().Subject;
        returnedCountry.Name.Should().Be("USA");
    }

    // ---------- PUT /api/countries/{countryId} ----------
    [Fact]
    public void UpdateCountry_ShouldReturnBadRequest_WhenInvalidData()
    {
        var updatedCountry = new CountryDTO { Id = 2, Name = "Canada" };

        var result = _controller.UpdateCountry(1, updatedCountry);

        var badRequest = result.Should().BeOfType<BadRequestObjectResult>().Subject;
        badRequest.Value.Should().Be("Invalid country data.");
    }

    [Fact]
    public void UpdateCountry_ShouldReturnBadRequest_WhenCountryDoesNotExist()
    {
        int countryId = 1;
        var updatedCountry = new CountryDTO { Id = 1, Name = "Canada" };
        A.CallTo(() => _countryRepository.CountryExists(countryId)).Returns(false);

        var result = _controller.UpdateCountry(countryId, updatedCountry);

        var badRequest = result.Should().BeOfType<BadRequestObjectResult>().Subject;
        badRequest.Value.Should().Be("Country Not Found.");
    }

    [Fact]
    public void UpdateCountry_ShouldReturnOk_WhenUpdateSucceeds()
    {
        int countryId = 1;
        var updatedCountry = new CountryDTO { Id = 1, Name = "Canada" };
        var country = new Country { Id = 1, Name = "USA" };
        A.CallTo(() => _countryRepository.CountryExists(countryId)).Returns(true);
        A.CallTo(() => _countryRepository.GetCountry(countryId)).Returns(country);

        var result = _controller.UpdateCountry(countryId, updatedCountry);

        var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
        okResult.Value.Should().Be("Country successfully updated!");
        country.Name.Should().Be("Canada");
    }

    // ---------- GET /api/countries/author/{authorId} ----------
    [Fact]
    public void GetCountryByAuthor_ShouldReturnOk_WithCountry()
    {
        int authorId = 1;
        var country = new Country { Id = 1, Name = "USA" };
        A.CallTo(() => _countryRepository.GetCountryByAuthor(authorId)).Returns(country);

        var result = _controller.GetCountryByAuthor(authorId);

        var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
        var returnedCountry = okResult.Value.Should().BeAssignableTo<CountryDTO>().Subject;
        returnedCountry.Name.Should().Be("USA");
    }

    [Fact]
    public void GetCountryByAuthor_ShouldReturnStatusCode500_WhenRepositoryThrows()
    {
        int authorId = 1;
        A.CallTo(() => _countryRepository.GetCountryByAuthor(authorId)).Throws(new Exception("DB error"));

        var result = _controller.GetCountryByAuthor(authorId);

        var statusCodeResult = result.Should().BeOfType<ObjectResult>().Subject;
        statusCodeResult.StatusCode.Should().Be(500);
        statusCodeResult.Value.Should().Be("An unexpected error occurred: DB error");
    }

    // ---------- GET /api/countries/{countryId}/authors ----------
    [Fact]
    public void GetAuthorsFromCountry_ShouldReturnBadRequest_WhenCountryDoesNotExist()
    {
        int countryId = 1;
        A.CallTo(() => _countryRepository.CountryExists(countryId)).Returns(false);

        var result = _controller.GetAuthorsFromCountry(countryId);

        var badRequest = result.Should().BeOfType<BadRequestObjectResult>().Subject;
        badRequest.Value.Should().Be("Country not found");
    }

    [Fact]
    public void GetAuthorsFromCountry_ShouldReturnOk_WithAuthors()
    {
        int countryId = 1;
        var authors = new List<Author>
        {
            new Author { Id = 1, Name = "Author 1", Bio = "Bio 1" }
        };
        A.CallTo(() => _countryRepository.CountryExists(countryId)).Returns(true);
        A.CallTo(() => _countryRepository.GetAuthorsFromCountry(countryId)).Returns(authors);

        var result = _controller.GetAuthorsFromCountry(countryId);

        var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
        var returnedAuthors = okResult.Value.Should().BeAssignableTo<IEnumerable<AuthorDTO>>().Subject;
        returnedAuthors.Should().HaveCount(1);
    }
}