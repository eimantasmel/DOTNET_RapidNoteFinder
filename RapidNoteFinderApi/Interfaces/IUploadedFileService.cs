namespace RapidNoteFinderApi.Interfaces
{
    public interface IUploadedFileService 
    {
        string HandleImage(string? imageDataUri);
        string HandleNoteContent(string content);
        void RemoveChangedImages(string updatedContent, string oldContent);
    }
}