using Microsoft.AspNetCore.Mvc;
using TaxBankingApi.Models;
using TaxBankingApi.Data; // NEW: Use AppDbContext for database access

namespace TaxBankingApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BankAccountsController : ControllerBase
{
    // OLD VERSION:
    // private static readonly List<BankAccount> accounts = new() // Static list to hold bank account data
    // {
    //     new BankAccount
    //     {
    //         Id = 1,
    //         UserId = 1, // Identifier for the user who owns the bank account
    //         AccountName = "Private Account",
    //         Iban = "CH9300762011623852957",
    //         Currency = "CHF",
    //         Balance = 100400.50m
    //     },
    //
    //     new BankAccount
    //     {
    //         Id = 2,
    //         UserId = 1,
    //         AccountName = "Savings Account",
    //         Iban = "CH5600762011623852958",
    //         Currency = "CHF",
    //         Balance = 12500.00m
    //     },
    //
    //     new BankAccount
    //     {
    //         Id = 3,
    //         UserId = 2,
    //         AccountName = "Private Account",
    //         Iban = "CH1200762011623852959",
    //         Currency = "CHF",
    //         Balance = 1200.75m
    //     },
    //
    //     new BankAccount
    //     {
    //         Id = 4,
    //         UserId = 2,
    //         AccountName = "Hobby Account",
    //         Iban = "CH3400762011623852960",
    //         Currency = "CHF",
    //         Balance = 4709.51m
    //     }
    // };


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
        // OLD VERSION:
        // newAccount.Id = accounts.Max(account => account.Id) + 1;
        // .Max searches for the highest ID value and adds 1
        //
        // newAccount.UserId = userId;
        //
        // accounts.Add(newAccount);
        //
        // return Ok(newAccount);
        // Returns HTTP status code 200 OK together with the newly created bank account


        // NEW VERSION:
        // We do not manually generate newAccount.Id anymore.
        // EF Core / SQLite will generate the Id automatically.

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

        // OLD VERSION:
        // return Ok(account);
        //
        // With the static List, changing the object in memory was enough.

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
        // OLD VERSION:
        // var account = accounts.FirstOrDefault(account => account.Id == id);
        // firstOrDefault searches for the first BankAccount whose Id matches the requested id

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