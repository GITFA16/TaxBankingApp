using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TaxBankingApi.Data;
using TaxBankingApi.Models;

namespace TaxBankingApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TaxCategoriesController : ControllerBase
{
    private readonly AppDbContext _context;

    public TaxCategoriesController(AppDbContext context)
    {
        _context = context;
    }


    // READ ALL TAX CATEGORIES
    // GET /api/taxcategories
    [HttpGet]
    public async Task<ActionResult<IEnumerable<TaxCategory>>> GetTaxCategories()
    {
        var categories = await _context.TaxCategories.ToListAsync();

        return Ok(categories);
    }


    // READ ONE TAX CATEGORY
    // GET /api/taxcategories/1
    [HttpGet("{id}")]
    public async Task<ActionResult<TaxCategory>> GetTaxCategory(int id)
    {
        var category = await _context.TaxCategories.FindAsync(id);

        if (category == null)
        {
            return NotFound();
        }

        return Ok(category);
    }


    // CREATE TAX CATEGORY
    // POST /api/taxcategories
    [HttpPost]
    public async Task<ActionResult<TaxCategory>> CreateTaxCategory(
        TaxCategory category)
    {
        _context.TaxCategories.Add(category);

        await _context.SaveChangesAsync();

        return CreatedAtAction(
            nameof(GetTaxCategory),
            new { id = category.Id },
            category
        );
    }


    // UPDATE TAX CATEGORY
    // PUT /api/taxcategories/1
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateTaxCategory(
        int id,
        TaxCategory updatedCategory)
    {
        var category = await _context.TaxCategories.FindAsync(id);

        if (category == null)
        {
            return NotFound();
        }

        category.Name = updatedCategory.Name;
        category.Description = updatedCategory.Description;

        await _context.SaveChangesAsync();

        return Ok(category);
    }


    // DELETE TAX CATEGORY
    // DELETE /api/taxcategories/1
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteTaxCategory(int id)
    {
        var category = await _context.TaxCategories.FindAsync(id);

        if (category == null)
        {
            return NotFound();
        }

        _context.TaxCategories.Remove(category);

        await _context.SaveChangesAsync();

        return NoContent();
    }
}