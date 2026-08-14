namespace TaxBankingApi.Models; 

public class BankAccount
{
    public int Id { get; set; } // Unique identifier for the bank account

    public int UserId { get; set; } // Identifier for the user who owns the bank account

    public string AccountName { get; set; } = string.Empty; //string.Empty: avoid null value

    public string Iban { get; set; } = string.Empty;

    public string Currency { get; set; } = "CHF";

    public decimal Balance { get; set; } 
    //double : binary floating point, exp: 0.300000004
    //decimal : decimal floating point, exp: 0.3

    public User? User { get; set; }
    // This BankAccount belongs to one User

    public List<Transaction> Transactions { get; set; } = new();
     // One BankAccount can have many Transactions



}