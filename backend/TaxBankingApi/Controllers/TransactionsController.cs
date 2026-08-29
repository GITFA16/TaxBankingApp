using Microsoft.AspNetCore.Mvc;
using TaxBankingApi.Models;
using TaxBankingApi.Data;

namespace TaxBankingApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TransactionsController : ControllerBase
{
    private readonly AppDbContext _context;

    public TransactionsController(AppDbContext context)
    {
        _context = context;
    }


    // READ ALL TRANSACTIONS
    // GET /api/transactions
    [HttpGet]
    public ActionResult<IEnumerable<Transaction>> GetTransactions()
    {
        return Ok(_context.Transactions.ToList());
    }


    // READ ONE TRANSACTION BY ID
    // GET /api/transactions/{id}
    [HttpGet("{id}")]
    public ActionResult<Transaction> GetTransactionById(int id)
    {
        var transaction = _context.Transactions
            .FirstOrDefault(transaction => transaction.Id == id);

        if (transaction == null)
        {
            return NotFound();
        }

        return Ok(transaction);
    }


    // READ TRANSACTIONS FOR ONE BANK ACCOUNT
    // GET /api/bankaccounts/{bankAccountId}/transactions
    [HttpGet("/api/bankaccounts/{bankAccountId}/transactions")]
    public ActionResult<IEnumerable<Transaction>> GetTransactionsByBankAccount(
        int bankAccountId)
    {
        var accountTransactions = _context.Transactions
            .Where(transaction =>
                transaction.BankAccountId == bankAccountId)
            .ToList();

        return Ok(accountTransactions);
    }


    // CREATE TRANSACTION
    // POST /api/transactions
    [HttpPost]
    public ActionResult<Transaction> CreateTransaction(
        Transaction newTransaction)
    {
        var account = _context.BankAccounts
            .FirstOrDefault(account =>
                account.Id == newTransaction.BankAccountId);

        if (account == null)
        {
            return NotFound("Bank account not found.");
        }

        // If no tax category was selected,
        // use Uncategorized as default
        if (string.IsNullOrWhiteSpace(
            newTransaction.SuggestedTaxCategory))
        {
            newTransaction.SuggestedTaxCategory =
                "Uncategorized";
        }

        _context.Transactions.Add(newTransaction);

        _context.SaveChanges();

        return StatusCode(201, newTransaction);
    }


    // TAX SUMMARY FOR ONE BANK ACCOUNT
    // GET /api/bankaccounts/{bankAccountId}/tax-summary
    [HttpGet("/api/bankaccounts/{bankAccountId}/tax-summary")]
    public IActionResult GetTaxSummary(int bankAccountId)
    {
        var accountTransactions = _context.Transactions
            .Where(transaction =>
                transaction.BankAccountId == bankAccountId)
            .ToList();

        var taxSummary = accountTransactions
            .Where(transaction =>
                !string.IsNullOrWhiteSpace(
                    transaction.SuggestedTaxCategory) &&
                transaction.SuggestedTaxCategory !=
                    "Uncategorized")

            .GroupBy(transaction =>
                transaction.SuggestedTaxCategory)

            .ToDictionary(
                group => group.Key,
                group => group.Sum(
                    transaction =>
                        Math.Abs(transaction.Amount)
                )
            );

        return Ok(taxSummary);
    }


    // READ ONLY TAX-RELEVANT TRANSACTIONS
    // GET /api/bankaccounts/{bankAccountId}/tax-transactions
    [HttpGet("/api/bankaccounts/{bankAccountId}/tax-transactions")]
    public ActionResult<IEnumerable<Transaction>> GetTaxTransactions(
        int bankAccountId)
    {
        var taxTransactions = _context.Transactions
            .Where(transaction =>
                transaction.BankAccountId == bankAccountId &&
                !string.IsNullOrWhiteSpace(
                    transaction.SuggestedTaxCategory) &&
                transaction.SuggestedTaxCategory !=
                    "Uncategorized")
            .ToList();

        return Ok(taxTransactions);
    }


    // UPDATE TRANSACTION
    // PUT /api/transactions/{id}
    [HttpPut("{id}")]
    public IActionResult UpdateTransaction(
        int id,
        Transaction updatedTransaction)
    {
        var transaction = _context.Transactions
            .FirstOrDefault(transaction =>
                transaction.Id == id);

        if (transaction == null)
        {
            return NotFound();
        }

        var account = _context.BankAccounts
            .FirstOrDefault(account =>
                account.Id ==
                updatedTransaction.BankAccountId);

        if (account == null)
        {
            return NotFound("Bank account not found.");
        }

        transaction.BankAccountId =
            updatedTransaction.BankAccountId;

        transaction.BookingDate =
            updatedTransaction.BookingDate;

        transaction.Description =
            updatedTransaction.Description;

        transaction.Amount =
            updatedTransaction.Amount;

        transaction.Currency =
            updatedTransaction.Currency;

        transaction.SuggestedTaxCategory =
            string.IsNullOrWhiteSpace(
                updatedTransaction.SuggestedTaxCategory)
            ? "Uncategorized"
            : updatedTransaction.SuggestedTaxCategory;

        _context.SaveChanges();

        return Ok(transaction);
    }


    // DELETE TRANSACTION
    // DELETE /api/transactions/{id}
    [HttpDelete("{id}")]
    public IActionResult DeleteTransaction(int id)
    {
        var transaction = _context.Transactions
            .FirstOrDefault(transaction =>
                transaction.Id == id);

        if (transaction == null)
        {
            return NotFound();
        }

        _context.Transactions.Remove(transaction);

        _context.SaveChanges();

        return NoContent();
    }
}