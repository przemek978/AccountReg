using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Swashbuckle.AspNetCore.Annotations;
using AccountReg.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using System.Security.Cryptography;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Identity;

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
        private readonly IConfiguration _configuration;
        private readonly IPasswordHasher<string> passwordHasher;

        public UsersController(UserContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
            passwordHasher = new PasswordHasher<string>();
        }
        /// <summary>
        /// Logs in a user based on email address and password.
        /// </summary>
        /// <param name="user">Login details.</param>
        /// <returns>Returns an authentication token.</returns>
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

                    var passwordValid = passwordHasher.VerifyHashedPassword(null,userFromDb.Password,user.Password) == PasswordVerificationResult.Success;
                    if (!passwordValid)
                    {
                        return Unauthorized("Invalid email or password");
                    }

                    var key = Encoding.ASCII.GetBytes(_configuration["JWT:SecretKey"]);
                    var tokenHandler = new JwtSecurityTokenHandler();

                    var claims = new[]
                    {
                        new Claim(ClaimTypes.Name, user.Email) 
                    };

                    var token = new JwtSecurityToken(
                        issuer: _configuration["JWT:Issuer"],
                        audience: _configuration["JWT:Audience"], 
                        claims: claims,
                        expires: DateTime.UtcNow.AddMinutes(30),
                        signingCredentials: new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256) // Uwierzytelnianie z użyciem HMACSHA256
                    );
                    var tokenString = tokenHandler.WriteToken(token);
                    return Ok("Bearer "+tokenString);
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
                    if (UserPesel != null)
                    {
                        return Conflict("User with this PESEL already exists");
                    }

                    if (user.Password.Equals(user.RePassword))
                    {
                        string hashedPassword = passwordHasher.HashPassword(null, user.Password);
                        user.Password = hashedPassword;
                    }
                    else
                    {
                        return BadRequest("Passwords aren't the same");
                    }

                    if (!user.ValidatePesel())
                    {
                        return BadRequest("Pesel is not valid");
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
        /// <summary>
        /// Retrieves user information
        /// </summary>
        /// <returns>Returns the  user object from the server.</returns>
        [HttpGet("User")]
        [Authorize]
        [SwaggerOperation(Summary = "Informacje o zalogowanym użytkowniku")]
        public async Task<ActionResult<User>> GetUser()
        {
            try
            {
                if (ModelState.IsValid)
                {

                    var authHeader = Request.Headers["Authorization"];
                    var jwtToken = authHeader.ToString().Replace("Bearer ", "");

                    var handler = new JwtSecurityTokenHandler();
                    var token = handler.ReadJwtToken(jwtToken);


                    var emailClaim=token.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value;
                    var userFromDb = await _context.Users.FirstOrDefaultAsync(u => u.Email == emailClaim);

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
        // GET: api/Users
        /// <summary>
        /// Gets a list of all users.
        /// </summary>
        /// <returns>A list of all users.</returns>
        [SwaggerOperation(Summary = "Wszyscy użytkownicy")]
        [Authorize]
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

    }
}
