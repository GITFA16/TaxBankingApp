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
            Balance = 5400.50m
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
            Balance = 3200.75m
        }
    };

    [HttpGet]
    public ActionResult<IEnumerable<BankAccount>> GetBankAccounts()
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
}