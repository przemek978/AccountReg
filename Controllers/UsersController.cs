using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Swashbuckle.AspNetCore.Annotations;
using AccountReg.Models;

namespace AccountReg.Controllers
{
    /// <summary>
    /// API controller for user support.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly UserContext _context;

        public UsersController(UserContext context)
        {
            _context = context;
        }

        // GET: api/Users
        /// <summary>
        /// Gets a list of all users.
        /// </summary>
        /// <returns>A list of all users.</returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<User>>> GetUsers()
        {
            try
            {
                var users = await _context.Users.ToListAsync();
                if (_context.Users == null)
                {
                    return NotFound();
                }
                return await _context.Users.ToListAsync();
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }
        /// <summary>
        /// Logs in a user based on email address and password.
        /// </summary>
        /// <param name="user">Login details.</param>
        /// <returns>Returns the user object from the server.</returns>
        [HttpPost("login")]
        [SwaggerOperation(Summary = "Logowanie użytkownika")]
        public async Task<ActionResult<User>> Login([FromForm] UserLogin user)
        {
            var userFromDb = await _context.Users.FirstOrDefaultAsync(u => u.Email == user.Email);

            if (userFromDb == null)
            {
                return BadRequest("Invalid email or password");
            }

            var passwordValid = userFromDb.Password == user.Password;

            if (!passwordValid)
            {
                return BadRequest("Invalid email or password");
            }

            return Ok(userFromDb);
        }
        /// <summary>
        /// Registers a new user.
        /// </summary>
        /// <param name="user">New user details.</param>
        /// <returns>Returns the registered user object from the server.</returns>

        [HttpPost("register")]
        [SwaggerOperation(Summary = "Rejestracja użytkownika")]
        public async Task<ActionResult<User>> Register([FromForm] User user)
        {
            if (ModelState.IsValid)
            {
                var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.Email == user.Email);

                if (existingUser != null)
                {
                    return BadRequest("Email address already in use");
                }

                _context.Users.Add(user);
                await _context.SaveChangesAsync();

                return Ok(user);
            }
            else
            {
                return BadRequest(ModelState);
            }
        }

        //// GET: api/Users/5
        //[HttpGet("{id}")]
        //public async Task<ActionResult<User>> GetUser(string id)
        //{
        //    if (_context.Users == null)
        //    {
        //        return NotFound();
        //    }
        //    var user = await _context.Users.FindAsync(id);

        //    if (user == null)
        //    {
        //        return NotFound();
        //    }

        //    return user;
        //}

        //// PUT: api/Users/5
        //// To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        //[HttpPut("{id}")]
        //public async Task<IActionResult> PutUser(string id, User user)
        //{
        //    if (id != user.Pesel)
        //    {
        //        return BadRequest();
        //    }

        //    _context.Entry(user).State = EntityState.Modified;

        //    try
        //    {
        //        await _context.SaveChangesAsync();
        //    }
        //    catch (DbUpdateConcurrencyException)
        //    {
        //        if (!UserExists(id))
        //        {
        //            return NotFound();
        //        }
        //        else
        //        {
        //            throw;
        //        }
        //    }

        //    return NoContent();
        //}

        //// POST: api/Users
        //// To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        //[HttpPost]
        //public async Task<ActionResult<User>> PostUser(User user)
        //{
        //  if (_context.Users == null)
        //  {
        //      return Problem("Entity set 'UserContext.Users'  is null.");
        //  }
        //    _context.Users.Add(user);
        //    try
        //    {
        //        await _context.SaveChangesAsync();
        //    }
        //    catch (DbUpdateException)
        //    {
        //        if (UserExists(user.Pesel))
        //        {
        //            return Conflict();
        //        }
        //        else
        //        {
        //            throw;
        //        }
        //    }

        //    return CreatedAtAction("GetUser", new { id = user.Pesel }, user);
        //}

        //// DELETE: api/Users/5
        //[HttpDelete("{id}")]
        //public async Task<IActionResult> DeleteUser(string id)
        //{
        //    if (_context.Users == null)
        //    {
        //        return NotFound();
        //    }
        //    var user = await _context.Users.FindAsync(id);
        //    if (user == null)
        //    {
        //        return NotFound();
        //    }

        //    _context.Users.Remove(user);
        //    await _context.SaveChangesAsync();

        //    return NoContent();
        //}

        //private bool UserExists(string id)
        //{
        //    return (_context.Users?.Any(e => e.Pesel == id)).GetValueOrDefault();
        //}
    }
}
