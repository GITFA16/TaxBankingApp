using Microsoft.AspNetCore.Mvc;
using System.Text.RegularExpressions;
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


    // READ ONE TRANSACTION
    // GET /api/transactions/{id}
    [HttpGet("{id}")]
    public ActionResult<Transaction> GetTransactionById(int id)
    {
        var transaction = _context.Transactions
            .FirstOrDefault(transaction =>
                transaction.Id == id);

        if (transaction == null)
        {
            return NotFound();
        }

        return Ok(transaction);
    }


    // READ TRANSACTIONS FOR ONE BANK ACCOUNT
    // GET /api/bankaccounts/{bankAccountId}/transactions
    [HttpGet("/api/bankaccounts/{bankAccountId}/transactions")]
    public ActionResult<IEnumerable<Transaction>>
        GetTransactionsByBankAccount(int bankAccountId)
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

        // Automatically determine the tax category
        // from the Tax Categories stored in the database
        newTransaction.SuggestedTaxCategory =
            FindTaxCategory(newTransaction.Description);

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


    // READ TAX-RELEVANT TRANSACTIONS
    // GET /api/bankaccounts/{bankAccountId}/tax-transactions
    [HttpGet("/api/bankaccounts/{bankAccountId}/tax-transactions")]
    public ActionResult<IEnumerable<Transaction>>
        GetTaxTransactions(int bankAccountId)
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

        // Recalculate the category automatically
        // when the transaction is updated
        transaction.SuggestedTaxCategory =
            FindTaxCategory(transaction.Description);

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


    // FIND TAX CATEGORY FROM DATABASE
    private string FindTaxCategory(string description)
    {
        if (string.IsNullOrWhiteSpace(description))
        {
            return "Uncategorized";
        }

        // Convert the transaction description to lowercase.
        // This makes the comparison case-insensitive.
        var transactionText =
            description.ToLower();

        // Read all Tax Categories from the database
        var categories =
            _context.TaxCategories.ToList();

        foreach (var category in categories)
        {
            // First check the category name itself.
            //
            // Example:
            // Category Name:
            // Krankenkasse
            //
            // Transaction:
            // KRANKENKASSE Rechnung
            if (!string.IsNullOrWhiteSpace(
                category.Name))
            {
                var categoryName =
                    category.Name.ToLower();

                if (transactionText.Contains(
                    categoryName))
                {
                    return category.Name;
                }
            }


            // Then check the words stored
            // in the Tax Category Description.
            //
            // Regex allows the user to separate
            // keywords with spaces or punctuation.
            //
            // Example:
            // Health Insurance / Krankenkasse - Sick,
            // SWICA; CSS Helsana
            //
            // becomes:
            // health
            // insurance
            // krankenkasse
            // sick
            // swica
            // css
            // helsana
            if (!string.IsNullOrWhiteSpace(
                category.Description))
            {
                var keywords = Regex.Split(
                    category.Description.ToLower(),
                    @"\W+"
                )
                .Where(keyword =>
                    !string.IsNullOrWhiteSpace(keyword))
                .ToList();


                foreach (var keyword in keywords)
                {
                    if (transactionText.Contains(
                        keyword))
                    {
                        return category.Name;
                    }
                }
            }
        }

        return "Uncategorized";
    }
}