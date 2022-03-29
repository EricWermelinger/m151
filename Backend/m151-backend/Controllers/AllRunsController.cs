using System.Data.Entity.Validation;
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
    public class AllRunsController : Controller
    {
        private readonly DataContext _context;
        private ErrorhandlingM151<Run> _errorHandling = new();
        private ValidationM151 _validation = new();
        private CalculationM151 _calculation = new();

        public AllRunsController(DataContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<List<RunDTO>>> GetAllRuns(decimal altitudeMin, decimal altitudeMax, decimal lengthMin, decimal lengthMax, decimal pointLatitude, decimal pointLongitude, decimal radiusFromPoint)
        {
            RunFilterDTO request = new RunFilterDTO
            {
                AltitudeMin = altitudeMin,
                AltitudeMax = altitudeMax,
                LengthMin = lengthMin,
                LengthMax = lengthMax,
                PointLatitude = pointLatitude,
                PointLongitude = pointLongitude,
                RadiusFromPoint = radiusFromPoint
            };

            bool pointValid = (request.RadiusFromPoint >= 0  && _validation.ValidateGeoNode(request.PointLatitude, request.PointLongitude, 0));

            if (request.AltitudeMin < 0 || request.AltitudeMin > request.AltitudeMax || request.LengthMin < 0 ||
                request.LengthMin > request.LengthMax || !pointValid)
            {
                return BadRequest(_errorHandling.DataNotValid());
            }

            var runs = await _context.Runs.Where(run => run.GpxFileId != null
                       && run.Length <= request.LengthMax && run.Length >= request.LengthMin
                       && run.Altitude >= request.AltitudeMin && run.Altitude <= request.AltitudeMax)
                    .ToListAsync();


            var requestPoint = new PointDTO
            {
                Latitude = request.PointLatitude,
                Longitude = request.PointLongitude
            };
            var nodeWithDistance = await _context.GpxNodes
                    .Where(node => node.GpxFileId != Guid.Empty)
                    .Select(node =>
                        new {
                                Node = node,
                                Distance = _calculation.CalculateDistance(new PointDTO
                                {
                                    Latitude = node.Latitude,
                                    Longitude = node.Longitude
                                }, requestPoint)
                            })
                    .ToListAsync();
            var nearRoutes = nodeWithDistance
                .Where(node => node.Distance <= request.RadiusFromPoint)
                .Select(node => node.Node.GpxFileId)
                .Distinct()
                .ToList();
         
            runs = runs.Where(run => nearRoutes.Contains(run.GpxFileId ?? Guid.Empty)).ToList();

            return Ok(runs.Select(run => new RunDTO
            {
                Altitude = run.Altitude,
                Duration = run.Duration,
                GpxFileId = run.GpxFileId,
                Id = run.Id,
                Length = run.Length,
                StartTime = run.StartTime,
                Title = run.Title
            }).ToList());
        }
    }
}
