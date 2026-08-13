using Microsoft.EntityFrameworkCore;
using TaxBankingApi.Models;

namespace TaxBankingApi.Data;

public class AppDbContext : DbContext      //database context class that inherits from DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public DbSet<User> Users { get; set; } //database set for the User entity, representing a collection of User objects in the database

    public DbSet<BankAccount> BankAccounts { get; set; } //database set for the BankAccount entity

    public DbSet<Transaction> Transactions { get; set; } //database set for the Transaction entity

}