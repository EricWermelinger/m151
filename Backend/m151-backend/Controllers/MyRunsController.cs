using m151_backend.DTOs;
using m151_backend.Entities;
using m151_backend.ErrorHandling;
using m151_backend.Framework;
using Microsoft.AspNetCore.Mvc;

namespace m151_backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MyRunsController : Controller
    {
        private readonly DataContext _context;
        private ErrorhandlingM151<Run> _errorHandling = new();
        private AuthorizationM151 authorization = new();

        public MyRunsController(DataContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<List<RunDTO>>> GetMyRuns()
        {
            Guid jwtUserId = authorization.JwtUserId();

            var runs = await _context.Runs.Where(run => run.UserId == jwtUserId)
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
            Guid jwtUserId = authorization.JwtUserId();

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
                    UserId = jwtUserId,
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
        public async Task<ActionResult> DeleteMyRun(Guid runId)
        {
            Guid jwtUserId = authorization.JwtUserId();

            var run = await _context.Runs.FindAsync(runId);
            if (run == null || run.UserId != jwtUserId)
            {
                return BadRequest(_errorHandling.ErrorNotFound());
            }

            _context.Runs.Remove(run);
            await _context.SaveChangesAsync();
            return Ok();
        }
    }
}
