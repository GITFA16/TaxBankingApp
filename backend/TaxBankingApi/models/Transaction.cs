namespace TaxBankingApi.Models; //Group of the class Models

public class Transaction
{
    public int Id { get; set; }

    public int BankAccountId { get; set; }

    public DateTime BookingDate { get; set; }

    public string Description { get; set; } = string.Empty; // here the tax catagory will be suggested

    public decimal Amount { get; set; }

    public string Currency { get; set; } = "CHF";

    public string SuggestedTaxCategory { get; set; } = string.Empty;

    public BankAccount? BankAccount { get; set; }
    // Navigation property
    // This Transaction belongs to one BankAccount
}