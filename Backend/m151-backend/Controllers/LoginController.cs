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
    public class LoginController : Controller
    {
        private readonly DataContext _context;
        private ErrorhandlingM151<User> _errorHandling = new();
        private AuthorizationM151 _authorization = new();

        public LoginController(DataContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<ActionResult<TokenDTO>> LoginUser(LoginDTO request)
        {
            if (request.Password == null || request.Username == null)
            {
                return BadRequest(_errorHandling.DataNotValid());
            }

            var user = await _context.Users.FirstOrDefaultAsync(usr => usr.Username == request.Username);
            if (user == null)
            {
                return BadRequest(_errorHandling.DataNotValid());
            }
   
            using var hmac = new HMACSHA512(user.PasswordSalt);
            var computeHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(request.Password));
            if (!computeHash.SequenceEqual(user.PasswordHash))
            {
                return BadRequest(_errorHandling.DataNotValid());
            }

            return Ok(_authorization.CreateToken(user.Id));
        }
    }
}
