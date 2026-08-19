using Microsoft.AspNetCore.Mvc;
using TaxBankingApi.Models;
using TaxBankingApi.Data; // NEW: Use AppDbContext for database access

namespace TaxBankingApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BankAccountsController : ControllerBase
{
    // NEW VERSION:
    // _context gives this controller access to the database through Entity Framework Core
    private readonly AppDbContext _context;

    // Constructor
    // ASP.NET Core provides AppDbContext automatically through Dependency Injection
    public BankAccountsController(AppDbContext context)
    {
        _context = context;
    }

    // READ ALL BANK ACCOUNTS
    // GET /api/bankaccounts
    [HttpGet]
    public ActionResult<IEnumerable<BankAccount>> GetBankAccounts() //<..> group of BankAccount objects
    {
        // OLD VERSION:
        // return Ok(accounts);

        // NEW VERSION:
        // Read all BankAccounts from the database
        var accounts = _context.BankAccounts.ToList();

        return Ok(accounts);
    }

    // READ ONE BANK ACCOUNT BY ID
    // GET /api/bankaccounts/1
    [HttpGet("{id}")]
    public ActionResult<BankAccount> GetBankAccountById(int id)
    {
        // OLD VERSION:
        // var account = accounts.FirstOrDefault(a => a.Id == id);

        // NEW VERSION:
        // Search the database for the first BankAccount whose Id matches the requested id
        var account = _context.BankAccounts
            .FirstOrDefault(account => account.Id == id);

        if (account == null)
        {
            return NotFound();
        }

        return Ok(account);
    }

    // READ ALL BANK ACCOUNTS OF ONE USER
    // GET /api/users/1/accounts
    [HttpGet("/api/users/{userId}/accounts")]
    // Handles HTTP GET requests for all bank accounts belonging to a specific user
    // Example: GET /api/users/1/accounts
    public ActionResult<IEnumerable<BankAccount>> GetAccountsByUser(int userId)
    {
        // OLD VERSION:
        // var userAccounts = accounts
        //     // account : list of all bank accounts
        //     // (result) userAccounts : list of bank accounts belonging to the requested user
        //     .Where(a => a.UserId == userId)
        //     // a.UserId == userId checks whether the account belongs to the requested user
        //     .ToList();
        //
        // Where(...) filters the list of bank accounts
        // ToList() converts the filtered result into a List<BankAccount>

        // NEW VERSION:
        var userAccounts = _context.BankAccounts
            .Where(account => account.UserId == userId)
            .ToList();

        return Ok(userAccounts);
        // Returns HTTP status code 200 OK
        // together with all bank accounts belonging to the requested user
    }

    // CREATE NEW BANK ACCOUNT FOR ONE USER
    // POST /api/users/1/accounts
    [HttpPost("/api/users/{userId}/accounts")]
    public ActionResult<BankAccount> CreateBankAccount(
        int userId,
        BankAccount newAccount)
    {
        var user = _context.Users
        .FirstOrDefault(user => user.Id == userId);

        if (user == null)
        {
        return NotFound("User not found.");
        }

        newAccount.UserId = userId;

        // Add the new bank account to the database
        _context.BankAccounts.Add(newAccount);

        // SaveChanges() writes the new bank account permanently to SQLite
        _context.SaveChanges();

        return StatusCode(201, newAccount);
        // HTTP 201 Created
    }


    // UPDATE BANK ACCOUNT
    // PUT /api/bankaccounts/1
    [HttpPut("{id}")]
    public IActionResult UpdateBankAccount(
        int id,
        BankAccount updatedAccount)
    {
        // OLD VERSION:
        // var account = accounts.FirstOrDefault(account => account.Id == id);

        // NEW VERSION:
        var account = _context.BankAccounts
            .FirstOrDefault(account => account.Id == id);

        if (account == null)
        {
            return NotFound();
        }

        account.AccountName = updatedAccount.AccountName;
        account.Iban = updatedAccount.Iban;
        account.Currency = updatedAccount.Currency;
        account.Balance = updatedAccount.Balance;

        // NEW VERSION:
        // Save the updated values permanently to the database
        _context.SaveChanges();

        return Ok(account);
    }

    // DELETE BANK ACCOUNT
    // DELETE /api/bankaccounts/1
    [HttpDelete("{id}")]
    public IActionResult DeleteBankAccount(int id)
    {

        // NEW VERSION:
        var account = _context.BankAccounts
            .FirstOrDefault(account => account.Id == id);

        if (account == null)
        {
            return NotFound();
        }

        // OLD VERSION:
        // accounts.Remove(account);

        // NEW VERSION:
        // Mark the bank account for deletion from the database
        _context.BankAccounts.Remove(account);

        // Save the deletion permanently to SQLite
        _context.SaveChanges();

        return NoContent();
    }
}