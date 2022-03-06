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
    [Authorize]
    public class UserDataController : Controller
    {
        private readonly DataContext _context;
        private ErrorhandlingM151<User> _errorHandling = new();
        private AuthorizationM151 authorization = new();

        public UserDataController(DataContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<UserDTO>> GetUserData()
        {
            Guid jwtUserId = authorization.JwtUserId();
            var user = await _context.Users.FindAsync(jwtUserId);
            if (user == null)
            {
                return BadRequest(_errorHandling.ErrorNotFound());
            }

            return Ok(new UserDTO
            {
                Birthdate = user.Birthdate,
                Firstname = user.Firstname,
                Height = user.Height,
                Lastname = user.Lastname,
                SexId = user.SexId,
                Weight = user.Weight
            });
        }

        [HttpPut]
        public async Task<ActionResult> UpdateUserData(UserDTO request)
        {
            Guid jwtUserId = authorization.JwtUserId();
            var user = await _context.Users.FindAsync(jwtUserId);
            if (user == null)
            {
                return BadRequest(_errorHandling.ErrorNotFound());
            }

            if (request.Birthdate < new DateTime(1900, 1, 1) || request.Birthdate > DateTime.Today ||
                request.Height < 100 || request.Height > 250 || request.Weight < 40 || request.Weight > 200)
            {
                return BadRequest(_errorHandling.DataNotValid());
            }

            user.Birthdate = request.Birthdate;
            user.Firstname = request.Firstname;
            user.Height = request.Height;
            user.Lastname = request.Lastname;
            user.SexId = request.SexId;
            user.Weight = request.Weight;

            await _context.SaveChangesAsync();
            return Ok();
        }
    }
}
