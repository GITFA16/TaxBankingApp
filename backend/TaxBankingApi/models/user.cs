namespace TaxBankingApi.Models //use for the group name of the class
{
    public class User  //class name
    {
        public int Id { get; set; }  //integer for ID ; get: Read and set: change
        public string FirstName { get; set; } = string.Empty; //string.empty : avoid null value
        public string LastName { get; set; } = string.Empty; 
        public string Email { get; set; } = string.Empty; 
      
    }
}