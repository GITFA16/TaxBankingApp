using Microsoft.AspNetCore.Mvc; // Use ASP.NET Core MVC classes
using TaxBankingApi.Models; // Use the User model
using TaxBankingApi.Data; // NEW: Use AppDbContext for database access

namespace TaxBankingApi.Controllers; // Namespace for controller classes

[ApiController] // Tells ASP.NET Core that this class is an API controller
[Route("api/[controller]")] // Base route becomes: /api/users
public class UsersController : ControllerBase
{
    // OLD VERSION:
    // private = only accessible inside this class
    // static = all instances info for this controller
    // readonly = the list reference cannot be replaced
    //
    // private static readonly List<User> users = new()
    // {
    //     new User
    //     {
    //         Id = 1,
    //         FirstName = "Faizal",
    //         LastName = "Alamudi",
    //         Email = "faizal.alamudi@example.com"
    //     },
    //
    //     new User
    //     {
    //         Id = 2,
    //         FirstName = "Simon",
    //         LastName = "Ammann",
    //         Email = "simon.ammann@example.com"
    //     }
    // };


    // NEW VERSION:
    // _context gives this controller access to the database through Entity Framework Core.
    // readonly means the _context reference cannot be replaced after the constructor has assigned it.
    // Important: readonly does NOT make the database read-only.
    // We can still add, update, and delete data through _context.
    private readonly AppDbContext _context;


    // Constructor
    // ASP.NET Core provides AppDbContext automatically through Dependency Injection.
    public UsersController(AppDbContext context)
    {
        // Store the AppDbContext provided by ASP.NET Core in _context
        // so that all methods in this controller can access the database.
        _context = context;
    }


    // READ ALL USERS
    // GET /api/users
    [HttpGet]
    public ActionResult<IEnumerable<User>> GetUsers()
    {
        // NEW VERSION:
        // Read all users from the Users table in the database
        var users = _context.Users.ToList();

        return Ok(users);
        // Returns HTTP 200 OK with all users
    }


    // READ ONE USER
    // GET /api/users/1
    [HttpGet("{id}")]
    public ActionResult<User> GetUserById(int id)
    {
        // OLD VERSION:
        // var user = users.FirstOrDefault(user => user.Id == id);

         var user = _context.Users
            .FirstOrDefault(user => user.Id == id);

        // FirstOrDefault searches for the first user
        // whose Id matches the id from the URL
        if (user == null)
        {
            return NotFound();
            // HTTP 404 Not Found
        }

        return Ok(user);
        // HTTP 200 OK with the selected user
    }


    // CREATE USER
    // POST /api/users
    [HttpPost]
    public ActionResult<User> CreateUser(User newUser)
    {
        // Add the new user to the Users table
        _context.Users.Add(newUser);

        // SaveChanges writes the new user permanently to SQLite
        _context.SaveChanges();

        return StatusCode(201, newUser);
        // HTTP 201 Created
    }


    // UPDATE USER
    // PUT /api/users/1
    [HttpPut("{id}")]
    public IActionResult UpdateUser(int id, User updatedUser)
    {
        // OLD VERSION:
        // var user = users.FirstOrDefault(user => user.Id == id);

        // NEW VERSION:
        var user = _context.Users
            .FirstOrDefault(user => user.Id == id);

        // Search for the user that should be updated
        if (user == null)
        {
            return NotFound();
        }

        // Update the editable properties
        user.FirstName = updatedUser.FirstName;
        user.LastName = updatedUser.LastName;
        user.Email = updatedUser.Email;

        // We do not change user.Id, because the Id identifies the user
        // With the static List, changing the object in memory was enough.

        // Save the updated values permanently to the database
        _context.SaveChanges();

        return Ok(user);
        // HTTP 200 OK with the updated user
    }


    // DELETE USER
    // DELETE /api/users/1
    [HttpDelete("{id}")]
    public IActionResult DeleteUser(int id)
    {
        // OLD VERSION:
        // var user = users.FirstOrDefault(user => user.Id == id);

        // NEW VERSION:
        var user = _context.Users
            .FirstOrDefault(user => user.Id == id);

        // Search for the user that should be deleted

        if (user == null)
        {
            return NotFound();
        }

        // OLD VERSION:
        // users.Remove(user);
        // Remove the user from the shared list

        // NEW VERSION:
        // Mark the user for deletion from the database
        _context.Users.Remove(user);

        // Save the deletion permanently to SQLite
        _context.SaveChanges();

        return NoContent();
        // HTTP 204 No Content
        // Delete was successful
    }

    // TAX SUMMARY FOR ALL BANK ACCOUNTS OF ONE USER
    // GET /api/users/1/tax-summary
    [HttpGet("{userId}/tax-summary")]
    public IActionResult GetUserTaxSummary(int userId)
    {
    // Get all BankAccount IDs belonging to the requested user
    var bankAccountIds = _context.BankAccounts
        .Where(account => account.UserId == userId)
        .Select(account => account.Id) //get only the Ids of the user's bank accounts
        .ToList();

    // Get all transactions belonging to those bank accounts
    var userTransactions = _context.Transactions
        .Where(transaction =>
            bankAccountIds.Contains(transaction.BankAccountId))
        .ToList();

    // Exclude uncategorized transactions
    // Group by tax category
    // Calculate total amount for each category
    var taxSummary = userTransactions
        .Where(transaction =>
            transaction.SuggestedTaxCategory != "Uncategorized")
        .GroupBy(transaction =>
            transaction.SuggestedTaxCategory)
        .ToDictionary(
            group => group.Key,
            group => group.Sum(
                transaction => Math.Abs(transaction.Amount)
            )
        );

    return Ok(taxSummary);
    }

    // TAX-RELEVANT TRANSACTIONS FOR ALL BANK ACCOUNTS OF ONE USER
    // GET /api/users/1/tax-transactions
    [HttpGet("{userId}/tax-transactions")]
    public ActionResult<IEnumerable<Transaction>> GetUserTaxTransactions(int userId)
    {
        // Get all BankAccount IDs belonging to the requested user
        var bankAccountIds = _context.BankAccounts
            .Where(account => account.UserId == userId)
            .Select(account => account.Id)
            .ToList();

        // Get all tax-relevant transactions from all accounts of the user
        var taxTransactions = _context.Transactions
            .Where(transaction =>
                bankAccountIds.Contains(transaction.BankAccountId) &&
                transaction.SuggestedTaxCategory != "Uncategorized")
            .ToList();

        return Ok(taxTransactions);
    }
}