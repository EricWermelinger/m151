using m151_backend.DTOs;
using m151_backend.Entities;
using m151_backend.ErrorHandling;
using m151_backend.Framework;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Routing.Constraints;

namespace m151_backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class GpxFileController : Controller
    {
        private readonly DataContext _context;
        private readonly IUserService _userService;
        private ErrorhandlingM151<GpxFile> _errorHandling = new();
        private ValidationM151 validation = new();
        private CalculationM151 calculation = new();

        public GpxFileController(DataContext context, IUserService userService)
        {
            _context = context;
            _userService = userService;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<GpxFileDTO>> GetDownloadGpxFile(Guid id)
        {
            var gpxFile = await _context.GpxFiles.FindAsync(id);
            var run = await _context.Runs.Where(run => run.GpxFileId == id).FirstOrDefaultAsync();
            if (gpxFile == null || run == null)
            {
                return BadRequest(_errorHandling.ErrorNotFound());
            }
            
            var nodes = await _context.GpxNodes.Where(node => node.GpxFileId == id)
                .OrderBy(node => node.OrderInFile)
                .Select(node => new GpxNodeDTO
                {
                    Elevation = node.Elevation,
                    Latitude = node.Latitude,
                    Longitude = node.Longitude,
                    OrderInFile = node.OrderInFile,
                    Time = node.Time
                })
                .ToListAsync();

            return Ok(new GpxFileDTO
            {
                Filename = gpxFile.Filename,
                Nodes = nodes,
                RunId = run.Id
            });
        }

        [HttpPost]
        public async Task<ActionResult<RunDTO>> UploadGpxFile(GpxFileDTO request)
        {
            var user = await _userService.GetUser();
            if (user == null)
            {
                return Unauthorized(_errorHandling.Unauthorized());
            }

            var run = await _context.Runs.FindAsync(request.RunId);
            
            if (run != null && run.UserId != user.Id)
            {
                return BadRequest(_errorHandling.DataNotValid());
            }
            if (request.Nodes.Count <= 1)
            {
                return BadRequest(_errorHandling.GetCustomError(ErrorKeys.GpxFile_FileNotValid));
            }

            var validNodes = request.Nodes
                .Where(nod => validation.ValidateGeoNode(nod.Latitude, nod.Longitude, nod.Elevation))
                .OrderBy(nod => nod.Time)
                .Select((nod, index) => new
                {
                    Id = Guid.NewGuid(),
                    nod.OrderInFile,
                    nod.Elevation,
                    nod.Latitude,
                    nod.Longitude,
                    nod.Time,
                    OrderByTime = index
                })
                .Where(x => x.OrderInFile == x.OrderByTime);

            if (validNodes.Count() != request.Nodes.Count)
            {
                return BadRequest(_errorHandling.GetCustomError(ErrorKeys.GpxFile_FileNotValid));
            }

            Guid gpxFileId = Guid.NewGuid();
            _context.GpxFiles.Add(new GpxFile
            {
                Id = gpxFileId,
                Filename = request.Filename
            });

            _context.AddRange(validNodes.Select(vnd => new GpxNode
            {
                Id = vnd.Id,
                OrderInFile = vnd.OrderByTime,
                Elevation = vnd.Elevation,
                Latitude = vnd.Latitude,
                Longitude = vnd.Longitude,
                Time = vnd.Time,
                GpxFileId = gpxFileId
            }));

            var points = validNodes.Select(vnd => new PointDTO
            {
                Latitude = vnd.Latitude,
                Longitude = vnd.Longitude
            }).ToList();

            if (run != null)
            {
                run.Altitude = calculation.CalculateRouteAltitudeUp(validNodes.Select(vnd => vnd.Elevation).ToList());
                run.Duration = (decimal)((validNodes.Last().Time - validNodes.First().Time).TotalSeconds);
                run.GpxFileId = gpxFileId;
                run.Length = calculation.CalculateRouteDistance(points);
                run.StartTime = validNodes.First().Time;
                run.Title = request.Filename;
            }
            else
            {
                var newRun = new Run
                {
                    Altitude = calculation.CalculateRouteAltitudeUp(validNodes.Select(vnd => vnd.Elevation).ToList()),
                    Duration = (decimal) ((validNodes.Last().Time - validNodes.First().Time).TotalSeconds),
                    GpxFileId = gpxFileId,
                    Length = calculation.CalculateRouteDistance(points),
                    StartTime = validNodes.First().Time,
                    Title = request.Filename,
                    UserId = user.Id,
                    Id = request.RunId
                };
                _context.Runs.Add(newRun);
                run = newRun;
            }
            

            await _context.SaveChangesAsync();

            return Ok(new RunDTO
            {
                Id = run.Id,
                GpxFileId = run.GpxFileId,
                Altitude = run.Altitude,
                Length = run.Length,
                StartTime = run.StartTime,
                Duration = run.Duration,
                Title = run.Title
            });
        }

        [HttpDelete]
        public async Task<ActionResult> DeleteGpxFile(Guid id)
        {
            var user = await _userService.GetUser();
            if (user == null)
            {
                return Unauthorized(_errorHandling.Unauthorized());
            }

            var gpxFile = await _context.GpxFiles.FindAsync(id);
            var run = await _context.Runs.Where(run => run.GpxFileId == id && run.UserId == user.Id).FirstOrDefaultAsync();
            if (gpxFile == null || run == null)
            {
                return BadRequest(_errorHandling.ErrorNotFound());
            }

            var nodesToRemove = await _context.GpxNodes.Where(node => node.GpxFileId == id)
                .ToListAsync();
            _context.GpxNodes.RemoveRange(nodesToRemove);
            _context.GpxFiles.Remove(gpxFile);

            _context.SaveChanges();
            return Ok();
        }
    }
}
