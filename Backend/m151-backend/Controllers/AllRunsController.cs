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
        private ValidationM151 validation = new();
        private CalculationM151 calculation = new();

        public AllRunsController(DataContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<List<RunDTO>>> GetAllRuns(string requestSerialized)
        {
            // example: { "LengthMin":10,"LengthMax":20,"AltitudeMin":30,"AltitudeMax":40,"PointLatitude":1,"PointLongitude":2,"RadiuseFromPoint":3}
            RunFilterDTO? request = JsonSerializer.Deserialize<RunFilterDTO?>(requestSerialized);

            if (request == null)
            {
                return BadRequest(_errorHandling.DataNotValid());
            }

            bool pointFiltered = request.RadiusFromPoint != null && request.PointLatitude != null && request.PointLongitude != null;
            bool pointValid = !pointFiltered || (request.RadiusFromPoint < 0
                                                 || !validation.ValidateGeoNode(request.PointLatitude,
                                                     request.PointLongitude, 0));

            if (request.AltitudeMin < 0 || request.AltitudeMin > request.AltitudeMax || request.LengthMin < 0 ||
                request.LengthMin > request.LengthMax || !pointValid)
            {
                return BadRequest(_errorHandling.DataNotValid());
            }

            var runs = await _context.Runs.Where(run => run.GpxFileId != null
                       && run.Length <= request.LengthMax && run.Length >= request.LengthMin
                       && run.Altitude >= request.AltitudeMin && run.Altitude <= request.AltitudeMax)
                    .ToListAsync();

            if (pointFiltered)
            {
                var nearRoutes = await _context.GpxNodes.Where(node => calculation.CalculateDistance(new PointDTO
                            {
                                Latitude = node.Latitude,
                                Longitude = node.Longitude
                            }, new PointDTO
                            {
                                Latitude = request.PointLatitude ?? 0,
                                Longitude = request.PointLongitude ?? 0
                            }) <= request.RadiusFromPoint)
                    .DistinctBy(node => node.GpxFileId)
                    .Select(node => node.GpxFileId)
                    .ToListAsync();

                runs = runs.Where(run => nearRoutes.Contains(run.GpxFileId ?? Guid.Empty)).ToList();
            }

            return Ok(runs);
        }
    }
}
