using Microsoft.AspNetCore.Mvc;
using TaxBankingApi.Models;

namespace TaxBankingApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TaxCategoriesController : ControllerBase
{
    // Tax categories are standard reference data.
    // They are read-only and cannot be created, updated, or deleted by the user.
    // static = one shared list for the application
    // readonly = the list reference cannot be replaced
    private static readonly List<TaxCategory> categories = new()
    {
        new TaxCategory
        {
            Id = 1,
            Name = "Health Insurance / Krankenkasse",
            Description = "Health insurance expenses"
        },

        new TaxCategory
        {
            Id = 2,
            Name = "Education / Weiterbildung",
            Description = "Education and professional training expenses"
        },

        new TaxCategory
        {
            Id = 3,
            Name = "Charity / Spenden",
            Description = "Charitable donations"
        },

        new TaxCategory
        {
            Id = 4,
            Name = "Pension / Vorsorge 3a",
            Description = "Pillar 3a retirement contributions"
        },

        new TaxCategory
        {
            Id = 5,
            Name = "Mortgage Interest / Hypothekenzinsen",
            Description = "Interest paid on mortgage loans"
        },

        new TaxCategory
        {
            Id = 6,
            Name = "Childcare / Kinderbetreuung",
            Description = "Expenses related to childcare services"
        },

        new TaxCategory
        {
            Id = 7,
            Name = "Public Transportation / Öffentlicher Verkehr",
            Description = "Expenses related to public transportation"
        },

        new TaxCategory
        {
            Id = 8,
            Name = "Professional Expenses / Professionelle Auslagen",
            Description = "Expenses related to professional work"
        }
    };


    // READ ALL STANDARD TAX CATEGORIES
    // GET /api/taxcategories
    [HttpGet]
    public ActionResult<IEnumerable<TaxCategory>> GetTaxCategories()
    {
        return Ok(categories);
    }
}