using Microsoft.AspNetCore.Mvc;
using TaxBankingApi.Models;
using TaxBankingApi.Services;

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

    [HttpPost]
    public ActionResult<Transaction> CreateTransaction(Transaction newTransaction)
    {
        if (transactions.Count == 0)
        {
            newTransaction.Id = 1;
        }
        else
        {
            newTransaction.Id =
                transactions.Max(transaction => transaction.Id) + 1;
        }

        var taxCategoryService = new TaxCategoryService();

        newTransaction.SuggestedTaxCategory =
            taxCategoryService.GetSuggestedCategory(newTransaction.Description);

        transactions.Add(newTransaction);

        return StatusCode(201, newTransaction);
    }

    [HttpGet("/api/bankaccounts/{bankAccountId}/tax-summary")]
    public IActionResult GetTaxSummary(int bankAccountId)
    {
        var accountTransactions = transactions
            .Where(transaction => transaction.BankAccountId == bankAccountId)
            .ToList();

        var taxSummary = accountTransactions
            .Where(transaction => transaction.SuggestedTaxCategory != "Uncategorized") // Exclude transactions that are not categorized
            .GroupBy(transaction => transaction.SuggestedTaxCategory)  // Group transactions by their suggested tax category
            .ToDictionary(
                group => group.Key, // Key: Tax category
                group => group.Sum(transaction => Math.Abs(transaction.Amount)) 
                // Value: Total amount for each tax category math.abs : negativ to positiv
            );

        return Ok(taxSummary);
    }
    [HttpGet("/api/bankaccounts/{bankAccountId}/tax-transactions")]
    public ActionResult<IEnumerable<Transaction>> GetTaxTransactions(int bankAccountId)
    {
        var taxTransactions = transactions
            .Where(transaction =>
                transaction.BankAccountId == bankAccountId &&   //transaction belongs to the requested bank account
                transaction.SuggestedTaxCategory != "Uncategorized") // Exclude transactions that are not categorized
            .ToList();

        return Ok(taxTransactions);

        // exp : Transaction 1
        // BankAccountId = 1
        // Category = Krankenkasse
        // 1 == 1 ✅
        // Category != Uncategorized ✅
    }
}