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
    public class NotesController : Controller
    {
        private readonly DataContext _context;
        private readonly IUserService _userService;
        private ErrorhandlingM151<RunNote> _errorHandling = new();

        public NotesController(DataContext context, IUserService userService)
        {
            _context = context;
            _userService = userService;
        }

        [HttpGet]
        public async Task<ActionResult<List<RunNoteDTO>>> GetNotes(Guid id)
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
                return BadRequest(_errorHandling.DataNotValid());
            }

            var notes = await _context.RunNotes.Where(note => note.RunId == id)
                .Select(note => new RunNoteDTO
                {
                    Id = note.Id,
                    RunId = note.RunId,
                    Note = note.Note
                })
                .ToListAsync();

            return Ok(notes);
        }

        [HttpPost]
        public async Task<ActionResult> UpdateNote(RunNoteDTO request)
        {
            var user = await _userService.GetUser();
            if (user == null)
            {
                return Unauthorized(_errorHandling.Unauthorized());
            }

            var existingNote = await _context.RunNotes.FindAsync(request.Id);
            var run = await _context.Runs.FindAsync(request.RunId);

            if (run == null)
            {
                return Ok(null);
            }

            if (run.UserId != user.Id)
            {
                return BadRequest(_errorHandling.DataNotValid());
            }

            if (existingNote == null)
            {
                _context.RunNotes.Add(new RunNote
                {
                    Id = request.Id,
                    Note = request.Note,
                    RunId = request.RunId
                });
            }
            else
            {
                existingNote.Note = request.Note;
            }

            await _context.SaveChangesAsync();
            return Ok();
        }

        [HttpDelete]
        public async Task<ActionResult> DeleteNote(Guid id)
        {
            var user = await _userService.GetUser();
            if (user == null)
            {
                return Unauthorized(_errorHandling.Unauthorized());
            }

            var note = await _context.RunNotes.FindAsync(id);
            if (note == null)
            {
                return BadRequest(_errorHandling.ErrorNotFound());
            }

            var run = await _context.Runs.FindAsync(note.RunId);
            if (run == null || run.UserId != user.Id)
            {
                return BadRequest(_errorHandling.ErrorNotFound());
            }

            _context.RunNotes.Remove(note);
            await _context.SaveChangesAsync();
            return Ok();
        }
    }
}
