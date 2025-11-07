using BookReviewApp.Data;
using BookReviewApp.Interfaces;
using BookReviewApp.Models;

namespace BookReviewApp.Repository;

public class CountryRepository : ICountryRepository
{
    private readonly DataContext _context;
    
    public CountryRepository (DataContext context)
    {
        _context = context;
    }
    public ICollection<Country> GetCountries()
    {
        return _context.Countries.OrderBy(c => c.Id).ToList();
    }

    public Country GetCountry(int countryId)
    {
        return _context.Countries.Where(c => c.Id==countryId).FirstOrDefault();
    }

    public Country GetCountryByAuthor(int authorId)
    {
        return _context.Authors.Where(a => a.Id == authorId).Select(a => a.Country)
            .FirstOrDefault();
    }

    public bool UpdateCountry(Country country)
    {
        _context.Countries.Update(country);
        return _context.SaveChanges() > 0;
    }

    public bool CountryExists(int countryId)
    {
        return _context.Countries.Any(c => c.Id == countryId);
    }
    
    public ICollection<Author> GetAuthorsFromCountry(int countryId)
    {
        return _context.Authors.Where(a => a.Country.Id == countryId).ToList();
    }
}