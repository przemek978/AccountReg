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
        [SwaggerOperation(Summary = "Wszyscy użytkownicy")]
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
        [HttpPost("Login")]
        [SwaggerOperation(Summary = "Logowanie użytkownika")]
        public async Task<ActionResult<User>> Login([FromForm] UserLogin user)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var userFromDb = await _context.Users.FirstOrDefaultAsync(u => u.Email == user.Email);

                    if (userFromDb == null)
                    {
                        return Unauthorized("Invalid email or password");
                    }

                    var passwordValid = userFromDb.Password == user.Password;
                    if (!passwordValid)
                    {
                        return Unauthorized("Invalid email or password");
                    }

                    return Ok(userFromDb);
                }
                else
                {
                    return BadRequest(ModelState);
                }
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }
        /// <summary>
        /// Registers a new user.
        /// </summary>
        /// <param name="user">New user details.</param>
        /// <returns>Returns the registered user object from the server.</returns>
        [HttpPost("Register")]
        [SwaggerOperation(Summary = "Rejestracja użytkownika")]
        public async Task<ActionResult<User>> Register([FromForm] User user)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.Email == user.Email);

                    if (existingUser != null)
                    {
                        return Conflict("Email address already in use");
                    }
                    var UserPesel = await _context.Users.FirstOrDefaultAsync(u => u.Pesel == user.Pesel);
                    if (UserPesel!=null)
                    {
                        return Conflict("User with this PESEL already exists");
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
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }
    }
}
