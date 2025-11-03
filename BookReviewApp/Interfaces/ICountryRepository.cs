using BookReviewApp.Models;

namespace BookReviewApp.Interfaces;

public interface ICountryRepository
{
    ICollection<Country> GetCountries();
    Country GetCountry(int countryId);
    Country GetCountryByAuthor(int authorId);
    bool CountryExists(int countryId);
    ICollection<Author> GetAuthorsFromCountry(int countryId);
}