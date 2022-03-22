using System.Security.Cryptography;
using m151_backend.DTOs;
using m151_backend.Entities;
using m151_backend.ErrorHandling;
using m151_backend.Framework;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace m151_backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [AllowAnonymous]
    public class RegisterController : Controller
    {
        private readonly DataContext _context;
        private ErrorhandlingM151<User> _errorHandling = new();
        private AuthorizationM151 _authorization = new();

        public RegisterController(DataContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<ActionResult> ResiterUser(LoginDTO request)
        {
            if (request.Password == null || request.Username == null)
            {
                return BadRequest(_errorHandling.DataNotValid());
            }

            var usernameExists = await _context.Users.AnyAsync(usr => usr.Username == request.Username);
            if (usernameExists)
            {
                return BadRequest(_errorHandling.GetCustomError(ErrorKeys.Register_UsernameExists));
            }

            var user = new User
            {
                Id = Guid.NewGuid(),
                Username = request.Username
            };

            using var hmac = new HMACSHA512();
            user.PasswordSalt = hmac.Key;
            user.PasswordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(request.Password));

            var token = _authorization.CreateToken(user.Id);

            user.RefreshToken = token.RefreshToken;
            user.RefreshExpires = token.RefreshExpires;

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return Ok(token);
        }
    }
}
