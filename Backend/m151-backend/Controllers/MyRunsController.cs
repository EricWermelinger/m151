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
    public class MyRunsController : Controller
    {
        private readonly DataContext _context;
        private readonly IUserService _userService;
        private ErrorhandlingM151<Run> _errorHandling = new();

        public MyRunsController(DataContext context, IUserService userService)
        {
            _context = context;
            _userService = userService;
        }

        [HttpGet]
        public async Task<ActionResult<List<RunDTO>>> GetMyRuns()
        {
            var user = await _userService.GetUser();
            if (user == null)
            {
                return Unauthorized(_errorHandling.Unauthorized());
            }

            var runs = await _context.Runs.Where(run => run.UserId == user.Id)
                .Select(run => new RunDTO
                {
                    Altitude = run.Altitude,
                    Duration = run.Duration,
                    GpxFileId = run.GpxFileId,
                    Id = run.Id,
                    Length = run.Length,
                    StartTime = run.StartTime,
                    Title = run.Title
                })
                .ToListAsync();

            return Ok(runs);
        }

        [HttpPost]
        public async Task<ActionResult> UpdateRun(RunDTO request)
        {
            var user = await _userService.GetUser();
            if (user == null)
            {
                return Unauthorized(_errorHandling.Unauthorized());
            }

            if (request.Duration <= 0 || request.Length <= 0 || request.StartTime < new DateTime(1900, 1, 1) ||
                request.StartTime > DateTime.Now)
            {
                return BadRequest(_errorHandling.DataNotValid());
            }

            var existingRun = await _context.Runs.FindAsync(request.Id);
            if (existingRun == null)
            {
                _context.Runs.Add(new Run
                {
                    UserId = user.Id,
                    Altitude = request.Altitude,
                    Duration = request.Duration,
                    GpxFileId = request.GpxFileId,
                    Id = request.Id,
                    Length = request.Length,
                    StartTime = request.StartTime,
                    Title = request.Title
                });
            }
            else
            {
                existingRun.Altitude = request.Altitude;
                existingRun.Duration = request.Duration;
                existingRun.GpxFileId = request.GpxFileId;
                existingRun.Length = request.Length;
                existingRun.StartTime = request.StartTime;
                existingRun.Title = request.Title;
            }
            
            await _context.SaveChangesAsync();
            return Ok();
        }

        [HttpDelete]
        public async Task<ActionResult> DeleteMyRun(Guid id)
        {
            var user = await _userService.GetUser();
            if (user == null)
            {
                return Unauthorized(_errorHandling.Unauthorized());
            }

            var run = await _context.Runs.FindAsync(id);
            if (run == null || run.UserId != user.Id)
            {
                return BadRequest(_errorHandling.ErrorNotFound());
            }

            _context.Runs.Remove(run);
            if (run.GpxFileId != null)
            {
                var gpxFile = await _context.GpxFiles.FindAsync(run.GpxFileId);
                if (gpxFile != null)
                {
                    _context.GpxFiles.Remove(gpxFile);
                    var nodesToRemove = await _context.GpxNodes.Where(node => node.GpxFileId == id)
                        .ToListAsync();
                    _context.GpxNodes.RemoveRange(nodesToRemove);
                }
            }

            var notesToRemove = await _context.RunNotes.Where(note => note.RunId == id)
                .ToListAsync();
            _context.RunNotes.RemoveRange(notesToRemove);

            await _context.SaveChangesAsync();
            
            return Ok();
        }
    }
}
