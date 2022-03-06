using m151_backend.DTOs;
using m151_backend.Entities;
using m151_backend.ErrorHandling;
using m151_backend.Framework;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Routing.Constraints;

namespace m151_backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GpxFileController : Controller
    {
        private readonly DataContext _context;
        private ErrorhandlingM151<GpxFile> _errorHandling = new();
        private AuthorizationM151 authorization = new();
        private ValidationM151 validation = new();
        private CalculationM151 calculation = new();

        public GpxFileController(DataContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<GpxFileDTO>> GetDownloadGpxFile(Guid gpxFileId)
        {
            Guid jwtUserId = authorization.JwtUserId();

            var gpxFile = await _context.GpxFiles.FindAsync(gpxFileId);
            var run = await _context.Runs.Where(run => run.GpxFileId == gpxFileId && run.UserId == jwtUserId).FirstOrDefaultAsync();
            if (gpxFile == null || run == null)
            {
                return BadRequest(_errorHandling.ErrorNotFound());
            }
            
            var nodes = await _context.GpxNodes.Where(node => node.GpxFileId == gpxFileId)
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
        public async Task<ActionResult<RunDTO>> UploadGpxFile(string requestSerialized)
        {
            // example: { "LengthMin":10,"LengthMax":20,"AltitudeMin":30,"AltitudeMax":40,"PointLatitude":1,"PointLongitude":2,"RadiuseFromPoint":3}
            GpxFileDTO? request = JsonSerializer.Deserialize<GpxFileDTO?>(requestSerialized);
            if (request == null)
            {
                return BadRequest(_errorHandling.ErrorNotFound());
            }

            Guid jwtUserId = authorization.JwtUserId();
            var user = await _context.Users.FindAsync(jwtUserId);
            if (user == null)
            {
                return BadRequest(_errorHandling.ErrorNotFound());
            }

            var run = await _context.Runs.FindAsync(request.RunId);
            if (run == null || run.UserId != jwtUserId)
            {
                return BadRequest(_errorHandling.ErrorNotFound());
            }

            request.Filename = DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss") + "_Upload.gpx";

            if (request.Nodes.Count > 1)
            {
                return BadRequest(_errorHandling.DataNotValid());
            }

            var validNodes = request.Nodes
                .Where(nod => validation.ValidateGeoNode(nod.Latitude, nod.Longitude, nod.Elevation))
                .OrderBy(nod => nod.Time)
                .Select((nod, index) => new
                {
                    Id = new Guid(),
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
                return BadRequest(_errorHandling.DataNotValid());
            }

            Guid gpxFileId = new Guid();
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

            run.Altitude = calculation.CalculateRouteAltitudeUp(validNodes.Select(vnd => vnd.Elevation).ToList());
            run.Duration = (decimal)((validNodes.Last().Time - validNodes.First().Time).TotalSeconds);
            run.GpxFileId = gpxFileId;
            run.Length = calculation.CalculateRouteDistance(points);
            run.StartTime = validNodes.First().Time;

            _context.SaveChanges();

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
        public async Task<ActionResult> DeleteGpxFile(Guid gpxFileId)
        {
            Guid jwtUserId = authorization.JwtUserId();

            var gpxFile = await _context.GpxFiles.FindAsync(gpxFileId);
            var run = await _context.Runs.Where(run => run.GpxFileId == gpxFileId && run.UserId == jwtUserId).FirstOrDefaultAsync();
            if (gpxFile == null || run == null)
            {
                return BadRequest(_errorHandling.ErrorNotFound());
            }

            var nodesToRemove = await _context.GpxNodes.Where(node => node.GpxFileId == gpxFileId)
                .ToListAsync();
            _context.GpxNodes.RemoveRange(nodesToRemove);
            _context.GpxFiles.Remove(gpxFile);

            _context.SaveChanges();
            return Ok();
        }
    }
}
