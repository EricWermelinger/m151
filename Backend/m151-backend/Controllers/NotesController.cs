using m151_backend.DTOs;
using m151_backend.Entities;
using m151_backend.ErrorHandling;
using m151_backend.Framework;
using Microsoft.AspNetCore.Mvc;

namespace m151_backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NotesController : Controller
    {
        private readonly DataContext _context;
        private ErrorhandlingM151<RunNote> _errorHandling = new();
        private AuthorizationM151 authorization = new();

        public NotesController(DataContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<List<RunNoteDTO>>> GetNotes(Guid runId)
        {
            Guid jwtUserId = authorization.JwtUserId();

            var run = await _context.Runs.FindAsync(runId);

            if (run == null || run.UserId != jwtUserId)
            {
                return BadRequest(_errorHandling.DataNotValid());
            }

            var notes = _context.RunNotes.Where(note => note.RunId == runId)
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
            Guid jwtUserId = authorization.JwtUserId();

            var existingNote = await _context.RunNotes.FindAsync(request.Id);
            var run = await _context.Runs.FindAsync(request.RunId);

            if (run == null || run.UserId != jwtUserId)
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
        public async Task<ActionResult> DeleteNote(Guid noteId)
        {
            Guid jwtUserId = authorization.JwtUserId();

            var note = await _context.RunNotes.FindAsync(noteId);
            if (note == null)
            {
                return BadRequest(_errorHandling.ErrorNotFound());
            }

            var run = await _context.Runs.FindAsync(note.RunId);
            if (run == null || run.UserId != jwtUserId)
            {
                return BadRequest(_errorHandling.ErrorNotFound());
            }

            _context.RunNotes.Remove(note);
            await _context.SaveChangesAsync();
            return Ok();
        }
    }
}
