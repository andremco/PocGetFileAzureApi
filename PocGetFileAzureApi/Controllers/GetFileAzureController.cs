using Azure.Storage.Files.Shares;
using Azure.Storage.Files.Shares.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace PocGetFileAzureApi.Controllers
{
    [ApiController]
    [Route("api/v1/file")]
    public class GetFileAzureController : ControllerBase
    {
        private readonly ILogger<GetFileAzureController> _logger;
        private readonly IConfiguration _configuration;

        public GetFileAzureController(ILogger<GetFileAzureController> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
        }

        [HttpGet("logo/{name}")]
        public async Task<IActionResult> Get(string name)
        {
            var connectionString = _configuration.GetSection("StorageAccount:ConnectionString").Value;
            var shareName = _configuration.GetSection("StorageAccount:ShareName").Value;

            ShareClient share = new ShareClient(connectionString, shareName);

            ShareDirectoryClient directory = share.GetDirectoryClient("logos");

            var file = directory.GetFileClient("logo-vestbank.png");

            Stream stream = await file.OpenReadAsync();

            return File(stream, "image/png");
        }


    }
}
