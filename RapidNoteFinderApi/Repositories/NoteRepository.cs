using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using RapidNoteFinderApi.Models;
using RapidNoteFinderApi.Data;
using System.Linq;
using RapidNoteFinderApi.Interfaces;


namespace RapidNoteFinderApi.Repositories
{
    public class NoteRepository : IRepository<Note>
    {
        private readonly ApplicationDbContext _context;

        private readonly IRedisCacheService _cache;

        private const string INVALID_NOTE_ERR_MSG = "Note fields like: description, associate and content cannot be empty";

        private const int MAX_RESULTS_COUNT = 20;

        public NoteRepository(ApplicationDbContext context, IRedisCacheService cache)
        {
            _context = context;
            _cache = cache;
        }

        public async Task<IEnumerable<Note>> GetAllAsync()
        {
            return await _context.Notes.ToListAsync();
        }

        public async Task<IEnumerable<Note>> FindNotesByDescription(string description, string associate)
        { 
            IEnumerable<Note> cachedNotes = await _cache.GetCacheValueAsync<Note>(associate);
            if(cachedNotes != null)
            {
                cachedNotes = cachedNotes.Where(note => note.Description.Contains(description) && note.Associate == associate)
                                .OrderByDescending(note => note.Id)
                                .Take(NoteRepository.MAX_RESULTS_COUNT);
                return cachedNotes;
            }
       

            IEnumerable<Note> notes =  await _context.Notes
                                .Where(note => note.Description.Contains(description) && note.Associate == associate)
                                .OrderByDescending(note => note.Id)
                                .Take(NoteRepository.MAX_RESULTS_COUNT)
                                .ToListAsync();

            await _cache.SetCacheValueAsync<Note>(associate, notes);
            return notes;
        }


        public async Task<IEnumerable<Note>> FindLatestNotes(string associate)
        {
            IEnumerable<Note> cachedNotes = await _cache.GetCacheValueAsync<Note>(associate);
            if(cachedNotes != null)
            {
               cachedNotes = cachedNotes.Where(note => note.Associate == associate)
                                .OrderByDescending(note => note.Id)
                                .Take(NoteRepository.MAX_RESULTS_COUNT);
               return cachedNotes;
            }

            IEnumerable<Note> notes = await _context.Notes
                                .Where(note => note.Associate == associate)
                                .OrderByDescending(note => note.Id)
                                .Take(NoteRepository.MAX_RESULTS_COUNT)
                                .ToListAsync();

            await _cache.SetCacheValueAsync<Note>(associate, notes);
            return notes;
        }


        public async Task<Note> GetByIdAsync(int? id)
        {
            return await _context.Notes.FindAsync(id);
        }

        public async Task AddAsync(Note entity)
        {
            if(!this.IsNoteValid(entity))
                throw new Exception(NoteRepository.INVALID_NOTE_ERR_MSG);

            _context.Notes.Add(entity);
            await _cache.DeleteCacheValueAsync(entity.Associate);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Note entity)
        {
            if(!this.IsNoteValid(entity))
                throw new Exception(NoteRepository.INVALID_NOTE_ERR_MSG);

            _context.Notes.Update(entity);
            await _cache.DeleteCacheValueAsync(entity.Associate);
            await _context.SaveChangesAsync();
        }

        private bool IsNoteValid(Note entity)
        {
            if(string.IsNullOrWhiteSpace(entity.Description) 
                || string.IsNullOrWhiteSpace(entity.Associate)
                || string.IsNullOrWhiteSpace(entity.Content))
                return false;

            return true;
        }

        public async Task DeleteAsync(int id)
        {
            var note = await _context.Notes.FindAsync(id);
            if (note != null)
            {
                await _cache.DeleteCacheValueAsync(note.Associate);
                _context.Notes.Remove(note);
                await _context.SaveChangesAsync();
            }
        }
    }
}
