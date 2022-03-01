using Microsoft.AspNetCore.Mvc;

namespace m151_backend.Controllers
{
    public class RepositoryM151<T> : ControllerBase where T : class
    {
        private readonly DataContext _context;

        public RepositoryM151(DataContext context)
        {
            _context = context;
        }

        public async Task<ActionResult<List<T>>> GetEntities()
        {
            var results = await _context.Set<T>().ToListAsync();
            return Ok(results);
        }

        public async Task<ActionResult<T>> GetEntity(Guid id)
        {
            var result = await _context.Set<T>().FindAsync(id);
            if (result == null)
            {
                return BadRequest(ErrorMessage());
            }
            return Ok(result);
        }

        public async Task<ActionResult<List<T>>> AddEntity(T request)
        {
            _context.Set<T>().Add(request);
            await _context.SaveChangesAsync();
            var results = await _context.Set<T>().ToListAsync();
            return Ok(results);
        }

        public async Task<ActionResult<List<T>>> UpdateEntity(Guid id, T request)
        {
            var result = await _context.Set<T>().FindAsync(id);
            if (result == null)
            {
                return BadRequest(ErrorMessage());
            }

            _context.Set<T>().Remove(result);
            _context.Set<T>().Add(request);
            await _context.SaveChangesAsync();

            var results = await _context.Set<T>().ToListAsync();
            return Ok(results);
        }

        public async Task<ActionResult<List<T>>> DeleteEntity(Guid id)
        {
            var result = await _context.Set<T>().FindAsync(id);
            if (result == null)
            {
                return BadRequest(ErrorMessage());
            }

            _context.Set<T>().Remove(result);
            await _context.SaveChangesAsync();

            var results = await _context.Set<T>().ToListAsync();
            return Ok(results);
        }

        private string ErrorMessage()
        {
            return typeof(T).Name + " not found.";
        }
    }
}