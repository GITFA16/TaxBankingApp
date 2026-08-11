using Microsoft.AspNetCore.Mvc;
using TaxBankingApi.Models;

namespace TaxBankingApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BankAccountsController : ControllerBase
{
    private static readonly List<BankAccount> accounts = new() // Static list to hold bank account data
    {
        new BankAccount
        {
            Id = 1,
            UserId = 1, // Identifier for the user who owns the bank account
            AccountName = "Private Account",
            Iban = "CH9300762011623852957",
            Currency = "CHF",
            Balance = 100400.50m
        },

        new BankAccount
        {
            Id = 2,
            UserId = 1,
            AccountName = "Savings Account",
            Iban = "CH5600762011623852958",
            Currency = "CHF",
            Balance = 12500.00m
        },

        new BankAccount
        {
            Id = 3,
            UserId = 2,
            AccountName = "Private Account",
            Iban = "CH1200762011623852959",
            Currency = "CHF",
            Balance = 1200.75m
        }
    };

    [HttpGet]
    public ActionResult<IEnumerable<BankAccount>> GetBankAccounts() //<..> group of BankAccount objects
    {
        return Ok(accounts);
    }

    [HttpGet("{id}")]
    public ActionResult<BankAccount> GetBankAccountById(int id)
    {
        var account = accounts.FirstOrDefault(a => a.Id == id);

        if (account == null)
        {
            return NotFound();
        }

        return Ok(account);
    }

    [HttpGet("/api/users/{userId}/accounts")]
// Handles HTTP GET requests for all bank accounts belonging to a specific user
// Example: GET /api/users/1/accounts

    public ActionResult<IEnumerable<BankAccount>> GetAccountsByUser(int userId)
    {
        var userAccounts = accounts //account : list of all bank accounts ; (result)userAccounts : list of bank accounts belonging to the requested user
            .Where(a => a.UserId == userId) // a.UserId == userId checks whether the account belongs to the requested user
            .ToList();
        // Where(...) filters the list of bank accounts
        // ToList() converts the filtered result into a List<BankAccount>

        return Ok(userAccounts);
        // Returns HTTP status code 200 OK
        // together with all bank accounts belonging to the requested user
    }

    [HttpPost("/api/users/{userId}/accounts")]
    public ActionResult<BankAccount> CreateBankAccount(
        int userId,
        BankAccount newAccount)
    {
        newAccount.Id = accounts.Max(account => account.Id) + 1; //.max search for highest ID value and adds 1
        newAccount.UserId = userId;

        accounts.Add(newAccount);

       return Ok(newAccount);
       // Returns HTTP status code 200 OK together with the newly created bank account
    }

    [HttpPut("{id}")]
    public IActionResult UpdateBankAccount(int id, BankAccount updatedAccount)
    {
        var account = accounts.FirstOrDefault(account => account.Id == id);

        if (account == null)
        {
            return NotFound();
        }

        account.AccountName = updatedAccount.AccountName;
        account.Iban = updatedAccount.Iban;
        account.Currency = updatedAccount.Currency;
        account.Balance = updatedAccount.Balance;

        return Ok(account);
    }

    [HttpDelete("{id}")]
    public IActionResult DeleteBankAccount(int id)
    {
        var account = accounts.FirstOrDefault(account => account.Id == id); 
        //firstOrDefault searches for the first BankAccount whose Id matches the requested id 
        if (account == null)
        {
            return NotFound();
        }

        accounts.Remove(account);

        return NoContent();
    }
}