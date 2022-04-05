﻿using System.Security.Cryptography;
using m151_backend.DTOs;
using m151_backend.Entities;
using m151_backend.ErrorHandling;
using m151_backend.Framework;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Extensions;

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
            var users = await _context.Users.ToListAsync();
            var error = LoginError(users, request);
            if (error != null)
            {
                switch (error)
                {
                    case "DataNotValid":
                        return BadRequest(_errorHandling.DataNotValid());
                    case "Login_WrongUser":
                        return Ok(_errorHandling.GetCustomError(ErrorKeys.Login_WrongUser));
                    case "Login_WrongPassword":
                        return Ok(_errorHandling.GetCustomError(ErrorKeys.Login_WrongPassword));
                }
            }

            var user = await _context.Users.FirstOrDefaultAsync(usr => usr.Username == request.Username);
            var token = _authorization.CreateToken(user.Id);
            user.RefreshToken = token.RefreshToken;
            user.RefreshExpires = token.RefreshExpires;
            await _context.SaveChangesAsync();

            return Ok(token);
        }

        private string LoginError(List<User> users, LoginDTO request)
        {
            if (request.Password == null || request.Username == null)
            {
                return "DataNotValid";
            }

            var user = users.FirstOrDefault(usr => usr.Username == request.Username);
            if (user == null)
            {
                return "Login_WrongUser";
            }

            using var hmac = new HMACSHA512(user.PasswordSalt);
            var computeHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(request.Password));
            if (!computeHash.SequenceEqual(user.PasswordHash))
            {
                return "Login_WrongPassword";
            }

            return null;
        }
    }
}
