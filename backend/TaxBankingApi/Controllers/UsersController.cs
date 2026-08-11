using Microsoft.AspNetCore.Mvc; // Use ASP.NET Core MVC classes
using TaxBankingApi.Models; // Use the User model

namespace TaxBankingApi.Controllers; // Namespace for controller classes

[ApiController] // Tells ASP.NET Core that this class is an API controller
[Route("api/[controller]")] // Base route becomes: /api/users
public class UsersController : ControllerBase
{
    // private = only accessible inside this class
    // static = all instances info for this controller
    // readonly = the list reference cannot be replaced
    private static readonly List<User> users = new()
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


    // READ ALL USERS
    // GET /api/users
    [HttpGet]
    public ActionResult<IEnumerable<User>> GetUsers()
    {
        return Ok(users);
        // Returns HTTP 200 OK with all users
    }


    // READ ONE USER
    // GET /api/users/1
    [HttpGet("{id}")]
    public ActionResult<User> GetUserById(int id)
    {
        var user = users.FirstOrDefault(user => user.Id == id);

        // FirstOrDefault searches for the first user
        // whose Id matches the id from the URL

        if (user == null)
        {
            return NotFound();
            // HTTP 404 Not Found
        }

        return Ok(user);
        // HTTP 200 OK with the selected user
    }


    // UPDATE USER
    // PUT /api/users/1
    [HttpPut("{id}")]
    public IActionResult UpdateUser(int id, User updatedUser)
    {
        var user = users.FirstOrDefault(user => user.Id == id);

        // Search for the user that should be updated

        if (user == null)
        {
            return NotFound();
        }

        // Update the editable properties
        user.FirstName = updatedUser.FirstName;
        user.LastName = updatedUser.LastName;
        user.Email = updatedUser.Email;

        // We do not change user.Id
        // because the Id identifies the user

        return Ok(user);
        // HTTP 200 OK with the updated user
    }


    // DELETE USER
    // DELETE /api/users/1
    [HttpDelete("{id}")]
    public IActionResult DeleteUser(int id)
    {
        var user = users.FirstOrDefault(user => user.Id == id);

        // Search for the user that should be deleted

        if (user == null)
        {
            return NotFound();
        }

        users.Remove(user);
        // Remove the user from the shared list

        return NoContent();
        // HTTP 204 No Content
        // Delete was successful
    }
}