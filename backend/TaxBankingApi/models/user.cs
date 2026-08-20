using System.ComponentModel.DataAnnotations;
namespace TaxBankingApi.Models //use for the group name of the class


{
    public class User  //class name
    {
        public int Id { get; set; }  //integer for ID ; get: Read and set: change
        [Required]
        public string FirstName { get; set; } = string.Empty; //string.empty : avoid null value
        [Required]
        public string LastName { get; set; } = string.Empty; 
        [Required]
        public string Email { get; set; } = string.Empty; 
      
        public List<BankAccount> BankAccounts { get; set; } = new();
        // One User can have many BankAccounts
    }
}