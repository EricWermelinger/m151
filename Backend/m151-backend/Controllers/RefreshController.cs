﻿using System.Security.Cryptography;
using System.Text.Json;
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
    public class RefreshController : Controller
    {
        private readonly DataContext _context;
        private ErrorhandlingM151<User> _errorHandling = new();
        private AuthorizationM151 _authorization = new();

        public RefreshController(DataContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<ActionResult<TokenDTO>> RefreshUser(RefreshTokenDTO command)
        {
            if (command == null)
            {
                return Ok(_errorHandling.GetCustomError(ErrorKeys.Refresh_UserNotFound));
            }

            var user = await _context.Users.FirstOrDefaultAsync(usr => usr.RefreshToken == command.RefreshToken
                                                                       && usr.RefreshExpires >= DateTime.UtcNow);
            if (user == null)
            {
                return Ok(_errorHandling.GetCustomError(ErrorKeys.Refresh_UserNotFound));
            }
   
            var token = _authorization.CreateToken(user.Id);

            user.RefreshToken = token.RefreshToken;
            user.RefreshExpires = token.RefreshExpires;
            await _context.SaveChangesAsync();

            return Ok(token);
        }
    }
}
