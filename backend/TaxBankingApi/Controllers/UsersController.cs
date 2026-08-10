using Microsoft.AspNetCore.Mvc; // using class/fuction from MS-Aspenet- ControllerBase,Apicontroller,HttpGet,Route,ActionResult ModelViewController
using TaxBankingApi.Models; //using class/fuction from TaxBankingApi.Models-User

namespace TaxBankingApi.Controllers; //namespace for the group name of the class

[ApiController] //attribute ; this class is an API controller
[Route("api/[controller]")] //url controller 
public class UsersController : ControllerBase //usersController inhetrits from ControllerBase class
{
    [HttpGet] // this method handles HTTP GET requests
    public ActionResult<IEnumerable<User>> GetUsers() //public for ASP.NET; GetUsers name of the method; ActionResult<IEnumerable<User>> return type for a collection of User objects
    {
        var users = new List<User>
        {
            new User
            {
                Id = 1,
                FirstName = "Faizal",
                LastName = "Alamudi",
                Email = "faizal.alamudi@example.com"
            },

            new User
            {
                Id = 2,
                FirstName = "Simon",
                LastName = "Ammann",
                Email = "simon.ammann@example.com"
            }
        };

        return Ok(users); //return HTTP 200 OK response with the list of users
    }
}