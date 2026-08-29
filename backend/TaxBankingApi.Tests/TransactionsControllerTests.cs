using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TaxBankingApi.Controllers;
using TaxBankingApi.Data;
using TaxBankingApi.Models;

namespace TaxBankingApi.Tests;

public class TransactionsControllerTests
{
    private AppDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(
                databaseName: Guid.NewGuid().ToString())
            .Options;

        return new AppDbContext(options);
    }


    [Fact]
    public void CreateTransaction_WithValidBankAccount_Returns201()
    {
        using var context = CreateContext();

        var bankAccount = new BankAccount
        {
            Id = 1,
            UserId = 1,
            AccountName = "Private Account",
            Currency = "CHF",
            Balance = 1000
        };

        context.BankAccounts.Add(bankAccount);
        context.SaveChanges();

        var controller =
            new TransactionsController(context);

        var transaction = new Transaction
        {
            BankAccountId = 1,
            BookingDate = DateTime.Now,
            Description = "Migros Einkauf",
            Amount = -50,
            Currency = "CHF"
        };

        var result =
            controller.CreateTransaction(transaction);

        var objectResult =
            Assert.IsType<ObjectResult>(result.Result);

        Assert.Equal(201, objectResult.StatusCode);
    }


    [Fact]
    public void CreateTransaction_WithInvalidBankAccount_Returns404()
    {
        using var context = CreateContext();

        var controller =
            new TransactionsController(context);

        var transaction = new Transaction
        {
            BankAccountId = 999,
            BookingDate = DateTime.Now,
            Description = "Test Transaction",
            Amount = -100,
            Currency = "CHF"
        };

        var result =
            controller.CreateTransaction(transaction);

        var objectResult =
            Assert.IsType<NotFoundObjectResult>(
                result.Result);

        Assert.Equal(404, objectResult.StatusCode);
    }


    [Fact]
    public void CreateTransaction_WithoutTaxCategory_SetsUncategorized()
    {
        using var context = CreateContext();

        var bankAccount = new BankAccount
        {
            Id = 1,
            UserId = 1,
            AccountName = "Private Account",
            Currency = "CHF",
            Balance = 1000
        };

        context.BankAccounts.Add(bankAccount);
        context.SaveChanges();

        var controller =
            new TransactionsController(context);

        var transaction = new Transaction
        {
            BankAccountId = 1,
            BookingDate = DateTime.Now,
            Description = "Unknown Payment",
            Amount = -25,
            Currency = "CHF",
            SuggestedTaxCategory = ""
        };

        controller.CreateTransaction(transaction);

        Assert.Equal(
            "Uncategorized",
            transaction.SuggestedTaxCategory);
    }


    [Fact]
    public void GetTransactionById_WithUnknownId_Returns404()
    {
        using var context = CreateContext();

        var controller =
            new TransactionsController(context);

        var result =
            controller.GetTransactionById(999);

        Assert.IsType<NotFoundResult>(
            result.Result);
    }


    [Fact]
    public void DeleteTransaction_WithUnknownId_Returns404()
    {
        using var context = CreateContext();

        var controller =
            new TransactionsController(context);

        var result =
            controller.DeleteTransaction(999);

        Assert.IsType<NotFoundResult>(result);
    }
}