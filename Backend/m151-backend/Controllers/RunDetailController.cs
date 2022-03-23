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
    public class RunDetailController : Controller
    {
        private readonly DataContext _context;
        private readonly IUserService _userService;
        private ErrorhandlingM151<Run> _errorHandling = new();

        public RunDetailController(DataContext context, IUserService userService)
        {
            _context = context;
            _userService = userService;
        }

        [HttpGet]
        public async Task<ActionResult<RunDTO>> GetRunById(Guid id)
        {
            var user = await _userService.GetUser();
            if (user == null)
            {
                return Unauthorized(_errorHandling.Unauthorized());
            }

            var run = await _context.Runs.FindAsync(id);
            if (run == null)
            {
                return Ok(null);
            }

            if (run.UserId != user.Id)
            {
                return BadRequest(_errorHandling.ErrorNotFound());
            }

            return Ok(new RunDTO{
                Altitude = run.Altitude,
                Duration = run.Duration,
                GpxFileId = run.GpxFileId,
                Id = run.Id,
                Length = run.Length,
                StartTime = run.StartTime,
                Title = run.Title
            });
        }
    }
}
