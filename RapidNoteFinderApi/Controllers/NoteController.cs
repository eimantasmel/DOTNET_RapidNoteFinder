using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using RapidNoteFinderApi.Models;
using RapidNoteFinderApi.Repositories;
using System.Threading.Tasks;
using System;
using RapidNoteFinderApi.Interfaces;

namespace RapidNoteFinderApi.Controllers;

[Route("api/[controller]")]
[ApiController] // Use ApiController attribute for API controllers
public class NoteController : ControllerBase // Use ControllerBase for API controllers
{
    private readonly ILogger<NoteController> _logger;
    private NoteRepository _noteRepository;

    private IUploadedFileService _fileService;

    public NoteController(ILogger<NoteController> logger, NoteRepository noteRepository, IUploadedFileService fileService)
    {
        _logger = logger;
        _noteRepository = noteRepository;
        _fileService = fileService;
    }

    [HttpGet("index")]
    public IActionResult Index()
    {
        return Ok(new { value = "Hello this is Rapid Note Finder Api" });
    }

    [HttpPost("add")]
    public async Task<IActionResult> Add([FromBody] Note note)
    {
        note.CreatedAt = DateTime.Now;
        try{
            note.Content = _fileService.HandleNoteContent(note.Content);
            await _noteRepository.AddAsync(note);
        }
        catch(Exception e)
        {
            return BadRequest(new { error = e.Message });  
        }
        return Ok(new { value = "success" }); // Return JSON response
    }

    [HttpGet("find")]
    public async Task<IActionResult> FindNote([FromQuery] string? description, [FromQuery] string associate)
    {
        try{
            IEnumerable<Note> foundedNotes;
            if(string.IsNullOrWhiteSpace(description))
                foundedNotes = await _noteRepository.FindLatestNotes(associate);
            else 
                foundedNotes  = await _noteRepository.FindNotesByDescription(description, associate);

            return Ok(foundedNotes); // Return JSON response
        }
        catch(Exception e)
        {
            return BadRequest(new { error = e.Message });  
        }
    }
    

    [HttpPut("update")]
    public async Task<IActionResult> UpdateNote([FromBody] Note note)
    {
        try{

            Note noteToUpdate =  await _noteRepository.GetByIdAsync(note.Id);
            string oldContent = noteToUpdate.Content;
            noteToUpdate.Content = _fileService.HandleNoteContent(note.Content);
            _fileService.RemoveChangedImages(noteToUpdate.Content, oldContent);
            await _noteRepository.UpdateAsync(noteToUpdate);
 
            return Ok(noteToUpdate); // Return JSON response
        }
        catch(Exception e)
        {
            return BadRequest(new { error = e.Message });  
        }
    }
}