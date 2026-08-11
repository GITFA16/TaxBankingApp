using Microsoft.AspNetCore.Mvc;
using TaxBankingApi.Models;

namespace TaxBankingApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TransactionsController : ControllerBase
{
    private static readonly List<Transaction> transactions = new()
    {
        new Transaction
        {
            Id = 1,
            BankAccountId = 1,
            BookingDate = new DateTime(2026, 8, 1),
            Description = "Swica Krankenversicherung August",
            Amount = -420.00m,
            Currency = "CHF",
            SuggestedTaxCategory = "Krankenkasse"
        },

        new Transaction
        {
            Id = 2,
            BankAccountId = 1,
            BookingDate = new DateTime(2026, 8, 3),
            Description = "ABB TS NDS Software Engineering Course",
            Amount = -9500.17m,
            Currency = "CHF",
            SuggestedTaxCategory = "Weiterbildung"
        }
    };

    [HttpGet]
    public ActionResult<IEnumerable<Transaction>> GetTransactions()
    {
        return Ok(transactions);
    }

    [HttpGet("{id}")]
    public ActionResult<Transaction> GetTransactionById(int id)
    {
        var transaction = transactions
            .FirstOrDefault(transaction => transaction.Id == id);

        if (transaction == null)
        {
            return NotFound();
        }

        return Ok(transaction);
    }

    [HttpGet("/api/bankaccounts/{bankAccountId}/transactions")]
    public ActionResult<IEnumerable<Transaction>> GetTransactionsByBankAccount(
        int bankAccountId)
    {
        var accountTransactions = transactions
            .Where(transaction => transaction.BankAccountId == bankAccountId)
            .ToList();

        return Ok(accountTransactions);
    }
}