using Azure.Storage.Files.Shares;
using Azure.Storage.Files.Shares.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mime;
using System.Text.Json;
using System.Threading.Tasks;

namespace PocGetFileAzureApi.Controllers
{
    [ApiController]
    [Route("api/v1/file")]
    public class FileShareAzureController : ControllerBase
    {
        private readonly ILogger<FileShareAzureController> _logger;
        private readonly IConfiguration _configuration;

        public FileShareAzureController(ILogger<FileShareAzureController> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
        }

        [HttpGet("logo/{fileName}")]
        public async Task<IActionResult> GetLogo(string fileName)
        {
            var connectionString = _configuration.GetSection("StorageAccount:ConnectionString").Value;
            var shareName = _configuration.GetSection("StorageAccount:ShareName").Value;

            if (string.IsNullOrEmpty(fileName)) return NotFound();

            var extension = GetExtensionFileName(fileName);

            var contentType = GetContentTypeForResponse(extension);
            
            ShareClient share = new ShareClient(connectionString, shareName);

            ShareDirectoryClient directory = share.GetDirectoryClient("logos");

            var file = directory.GetFileClient(fileName);

            if (!file.Exists()) return NotFound();

            Stream stream = await file.OpenReadAsync();

            return File(stream, contentType);
        }

        [HttpGet("logo/filesName")]
        [HttpGet("logo/filesName/{clientNameSx1}")]
        public async Task<IActionResult> GetFilesName(string clientNameSx1)
        {
            var connectionString = _configuration.GetSection("StorageAccount:ConnectionString").Value;
            var shareName = _configuration.GetSection("StorageAccount:ShareName").Value;

            ShareClient share = new ShareClient(connectionString, shareName);

            ShareDirectoryClient directory = share.GetDirectoryClient("logos");

            var filesName = new List<string>();

            if (string.IsNullOrEmpty(clientNameSx1))
            {
                filesName = directory.GetFilesAndDirectories().Select(f => f.Name).ToList();
            }
            else
            {
                filesName = directory.GetFilesAndDirectories().Where(f => f.Name.Contains(clientNameSx1)).Select(f => f.Name).ToList();
            }

            if (filesName == null || filesName.Count < 1) NotFound();

            return Ok(filesName);
        }

        private string GetExtensionFileName(string fileName)
        {
            var extension = System.IO.Path.GetExtension(fileName);

            return extension;
        }

        private string GetContentTypeForResponse(string extension)
        {
            if (string.IsNullOrEmpty(extension)) return String.Empty;

            switch (extension.ToLower())
            {
                case ".svg":
                    return "image/svg+xml";
                case ".png":
                    return "image/png";
                case ".jpeg":
                case ".jpg":
                    return "image/jpeg";
                default:
                    return String.Empty;
            }
        }
    }
}
