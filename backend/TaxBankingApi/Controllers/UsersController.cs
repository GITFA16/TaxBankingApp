using Microsoft.AspNetCore.Mvc; // Use ASP.NET Core MVC classes such as ControllerBase, ApiController, HttpGet, HttpDelete, Route, ActionResult
using TaxBankingApi.Models; // Use the User model from TaxBankingApi.Models

namespace TaxBankingApi.Controllers; // Namespace for grouping controller classes

[ApiController] // Attribute: tells ASP.NET Core that this class is an API controller
[Route("api/[controller]")] // Defines the URL route. [controller] becomes "users"
public class UsersController : ControllerBase // UsersController inherits from ControllerBase
{
    [HttpGet] // This method handles HTTP GET requests
    public ActionResult<IEnumerable<User>> GetUsers()
    // public = accessible by ASP.NET Core
    // ActionResult<IEnumerable<User>> = HTTP response containing a collection of User objects
    // GetUsers = name of the method
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

        return Ok(users);
        // Ok() returns HTTP status code 200 OK
    }


    [HttpGet("{id}")]
    // Handles HTTP GET requests with an ID parameter
    public ActionResult<User> GetUserById(int id)
    // ActionResult<User> = HTTP response that can contain one User object
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

        var user = users.FirstOrDefault(u => u.Id == id);
        // FirstOrDefault searches for the first User whose Id matches the requested id
        // If no matching user exists, the result is null

        if (user == null)
        {
            return NotFound();
            // Returns HTTP status code 404 Not Found
        }

        return Ok(user);
        // Returns HTTP status code 200 OK together with the User object
    }


    [HttpDelete("{id}")]
    // Handles HTTP DELETE requests
    public IActionResult DeleteUser(int id)
    // IActionResult = the method returns an HTTP response
    // int id = ID of the user that should be deleted
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

        var user = users.FirstOrDefault(u => u.Id == id);
        // Search for the User whose Id matches the requested id

        if (user == null)
        {
            return NotFound();
            // If the user does not exist, return HTTP 404 Not Found
        }

        users.Remove(user);
        // Remove the User object from the list

        return NoContent();
        // Returns HTTP status code 204 No Content
        // 204 means the delete operation was successful
        // No response body is returned
    }
}