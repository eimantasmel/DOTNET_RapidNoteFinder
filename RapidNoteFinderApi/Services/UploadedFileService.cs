using System.Collections.Generic;
using System.Text.RegularExpressions;
using System;
using System.IO;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using RapidNoteFinderApi.Interfaces;
using Microsoft.IdentityModel.Tokens;


namespace RapidNoteFinderApi.Services
{
    public class UploadedFileService : IUploadedFileService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IConfiguration _configuration;

        public UploadedFileService(IHttpContextAccessor httpContextAccessor, IConfiguration configuration)
        {
            _configuration = configuration;
            _httpContextAccessor = httpContextAccessor;
        }
        public string HandleImage(string imageDataUri)
        {
            // Get host and port
            var request = _httpContextAccessor.HttpContext.Request;
            var host = request.Host.Host;
            var port = _httpContextAccessor.HttpContext.Request.Host.Port ?? 80;

            // Extract and decode the base64 image data
            var base64Image = imageDataUri.Substring(imageDataUri.IndexOf(',') + 1);
            var imageData = Convert.FromBase64String(base64Image);

            // Generate unique ID and file path
            var uniqId = Guid.NewGuid().ToString();
            var publicStoragePath = _configuration["PublicStoragePath"] ?? "";
            var filePath = Path.Combine(publicStoragePath, uniqId + ".png");

            // Save the image to the file system
            File.WriteAllBytes(filePath, imageData);

            // Construct the URL based on environment and port
            var appEnv = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            if (appEnv == "Development")
            {
                return $"http://{host}:{port}/storage/{uniqId}.png";
            }
            return $"https://{host}/public/storage/{uniqId}.png";
        }

        public string HandleNoteContent(string content)
        {
            List<string> imageSources = this.ExtractImageSourcesFromContent(content);
            List<string> updatedImageSources = new List<string>();

            foreach (string source in imageSources)
            {
                updatedImageSources.Add(this.HandleImage(source));
            }
            
            return this.UpdateNoteContent(content, updatedImageSources);
        }

        public void RemoveChangedImages(string updatedContent, string oldContent)
        {
            List<string> newSources = this.ExtractImageSourcesFromContent(updatedContent, "storage");
            List<string> oldSources = this.ExtractImageSourcesFromContent(oldContent, "storage");

            foreach (string source in oldSources)
            {
                if(!newSources.Contains(source))
                {
                    string filename = Path.GetFileName(source);
                    this.DeleteFile(filename);
                }
            }
        }

        private List<string> ExtractImageSourcesFromContent(string content, string alias = "data:image")
        {
            List<string> imageSources = new List<string>();
            string pattern = $@"<img[^>]+src=[""'](?=.*{alias})([^""']+)[""'][^>]*>";

            MatchCollection matches = Regex.Matches(content, pattern, RegexOptions.IgnoreCase);

            foreach (Match match in matches)
            {
                string src = match.Groups[1].Value;
                if (Regex.IsMatch(src, alias, RegexOptions.IgnoreCase))
                {
                    imageSources.Add(src);
                }
            }

            return imageSources;
        }


        private string UpdateNoteContent(string content, List<string> imageSources)
        {
            // Match all img tags in the content
            string pattern = @"<img[^>]+src=[""']([^""']*data:image[^""']+)[""'][^>]*>";
            MatchCollection matches = Regex.Matches(content, pattern, RegexOptions.IgnoreCase);

            // Replace src attributes with new values
            for (int index = 0; index < matches.Count; index++)
            {
                if (index < imageSources.Count)
                {
                    // Get the current img tag and src attribute value
                    string imgTag = matches[index].Value;
                    string srcPattern = @"src=[""']([^""']+)[""']";
                    Match srcMatch = Regex.Match(imgTag, srcPattern, RegexOptions.IgnoreCase);

                    if (srcMatch.Success)
                    {
                        string oldSrc = srcMatch.Groups[1].Value;
                        string newSrc = imageSources[index];

                        // Replace src attribute value with the new source
                        string newImgTag = imgTag.Replace(oldSrc, newSrc);
                        int pos = content.IndexOf(imgTag);
                        if (pos != -1)
                        {
                            content = content.Remove(pos, imgTag.Length).Insert(pos, newImgTag);
                        }
                    }
                }
            }

            return content;
        }

        private void DeleteFile(string filename)
        {
            try
            {
                // Construct the full file path
                var publicStoragePath = _configuration["PublicStoragePath"] ?? "";
                string filePath = Path.Combine(publicStoragePath, filename);

                // Delete the file
                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                }
            }
            catch (Exception e)
            {
                throw new Exception($"An error occurred: {e.Message}");
            }
        }
    }
}
