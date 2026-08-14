using Microsoft.AspNetCore.Mvc;
using TaxBankingApi.Models;
using TaxBankingApi.Services;
using TaxBankingApi.Data; // Use AppDbContext for database access

namespace TaxBankingApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TransactionsController : ControllerBase
{
    // NEW VERSION:
    // _context gives this controller access to the database through Entity Framework Core
    private readonly AppDbContext _context;

    // Constructor
    // ASP.NET Core automatically provides AppDbContext through Dependency Injection
    public TransactionsController(AppDbContext context)
    {
        _context = context;
    }

    // READ ALL TRANSACTIONS
    // GET /api/transactions
    [HttpGet]
    public ActionResult<IEnumerable<Transaction>> GetTransactions()
    {
        // NEW VERSION:
        // _context.Transactions = Transactions table in the database
        // ToList() reads all transactions from the database
        return Ok(_context.Transactions.ToList());
    }


    // READ ONE TRANSACTION BY ID
    // GET /api/transactions/{id}
    [HttpGet("{id}")]
    public ActionResult<Transaction> GetTransactionById(int id)
    {

        // NEW VERSION:
        // Search the Transactions table in the database
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

        // NEW VERSION:
        // Read only transactions from the database
        // where BankAccountId matches the requested bankAccountId
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
        // NEW VERSION:
        // We do not manually create the Id anymore.
        // SQLite / EF Core will generate the Id automatically.

        var taxCategoryService = new TaxCategoryService();

        newTransaction.SuggestedTaxCategory =
            taxCategoryService.GetSuggestedCategory(
                newTransaction.Description
            );

        // NEW VERSION:
        // Add the transaction to the database
        _context.Transactions.Add(newTransaction);

        // SaveChanges() writes the changes permanently to SQLite
        _context.SaveChanges();

        return StatusCode(201, newTransaction);
    }


    // TAX SUMMARY FOR ONE BANK ACCOUNT
    // GET /api/bankaccounts/{bankAccountId}/tax-summary
    [HttpGet("/api/bankaccounts/{bankAccountId}/tax-summary")]
    public IActionResult GetTaxSummary(int bankAccountId)
    {
        // NEW VERSION:
        // Read the transactions for this bank account from the database
        var accountTransactions = _context.Transactions
            .Where(transaction =>
                transaction.BankAccountId == bankAccountId)
            .ToList();


        var taxSummary = accountTransactions
            .Where(transaction =>
                transaction.SuggestedTaxCategory != "Uncategorized")
            // Exclude transactions that are not categorized

            .GroupBy(transaction =>
                transaction.SuggestedTaxCategory)
            // Group transactions by their suggested tax category

            .ToDictionary(
                group => group.Key,
                // Key: Tax category

                group => group.Sum(
                    transaction =>
                        Math.Abs(transaction.Amount)
                )
                // Value: Total amount for each tax category
                // Math.Abs: negative amount becomes positive
            );

        return Ok(taxSummary);
    }


    // READ ONLY TAX-RELEVANT TRANSACTIONS
    // GET /api/bankaccounts/{bankAccountId}/tax-transactions
    [HttpGet("/api/bankaccounts/{bankAccountId}/tax-transactions")]
    public ActionResult<IEnumerable<Transaction>> GetTaxTransactions(
        int bankAccountId)
    {
        // OLD VERSION:
        // var taxTransactions = transactions
        //     .Where(transaction =>
        //         transaction.BankAccountId == bankAccountId &&
        //         transaction.SuggestedTaxCategory != "Uncategorized")
        //     .ToList();

        // NEW VERSION:
        var taxTransactions = _context.Transactions
            .Where(transaction =>
                transaction.BankAccountId == bankAccountId &&
                // transaction belongs to the requested bank account

                transaction.SuggestedTaxCategory != "Uncategorized")
                // Exclude transactions that are not categorized

            .ToList();

        return Ok(taxTransactions);

        // exp : Transaction 1
        // BankAccountId = 1
        // Category = Krankenkasse
        // 1 == 1 ✅
        // Category != Uncategorized ✅
    }
}