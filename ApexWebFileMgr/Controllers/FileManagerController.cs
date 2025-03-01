﻿using ApexWebFileMgr.core.Models;
using ApexWebFileMgr.core.Services.DbCallService;
using ApexWebFileMgr.core.Services.FileMgrService;
using ApexWebFileMgr.Shared.Enums;
using ApexWebFileMgr.Shared.Options;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace ApexWebFileMgr.Controllers
{
    [ApiController]
    [Route("api/FileMgr")]
    public class FileManagerController : ControllerBase
    {
        private readonly ILogger<FileManagerController> _logger;
        //private readonly DocumentTypeOptionsSettings _documentTypeOptions;
        private readonly IFileManagerService _fileMgr;
        private readonly IDbCallService _dbCall;


        public FileManagerController(ILogger<FileManagerController> logger,
            IOptions<DocumentTypeOptionsSettings> documentTypeOptions,
            IFileManagerService fileMgr,
            IDbCallService dbCall)
        {
            _logger = logger;
            //_documentTypeOptions = documentTypeOptions.Value; 
            _fileMgr = fileMgr;
            _dbCall = dbCall;
        }

        [HttpGet("Health")]
        public IActionResult Health()
        {
            return Ok("File Manger is running...");
        }

        [HttpPost("UploadDocuments")]
        public async Task<IActionResult> UploadDocuments([FromForm] string DocType, [FromForm] IFormFile file)
        {
            try
            {
                if (!Enum.TryParse<DocumentType>(DocType, out DocumentType result))
                {
                    return BadRequest($"{DocType} is a not valid FileType.");
                }

                if (file.ContentType != "image/png" && file.ContentType != "image/jpeg" && file.ContentType != "image/jpg")
                {
                    return BadRequest("Only .png, .jpeg, .jpg formats are allowed.");
                }

                var imageConfiguration = await _dbCall.GetConfigurationAsync();
                if (imageConfiguration == null || String.IsNullOrEmpty(imageConfiguration.Value))
                {
                    return BadRequest("There is server face the error. Please Try After some Time.");
                }

                string directoryPath = Path.Combine(imageConfiguration.Value, DocType);
                bool exists = System.IO.Directory.Exists(directoryPath);

                if (!exists)
                {
                    _logger.Log(LogLevel.Information, $"Directory '{directoryPath}' does not exist. Creating directory at '{directoryPath}' on {DateTime.Now:yyyy-MM-dd HH:mm:ss}.");
                    System.IO.Directory.CreateDirectory(directoryPath);
                }

                if (file.Length > 0)
                {
                    string fileName = _fileMgr.GenerateFileName(DocType, file.FileName);
                    //string directoryPath = Path.Combine(env.WebRootPath, "uploads", DocType);
                    string filePath = Path.Combine(directoryPath, fileName);

                    using (var fileStream = file.OpenReadStream())
                    {
                        // Compress the image to 30KB
                        var compressedBytes = _fileMgr.CompressImage(fileStream, 30);

                        // Write the compressed image to the file system
                        await System.IO.File.WriteAllBytesAsync(filePath, compressedBytes);
                    }
                    //var image = System.IO.File.OpenRead(filePath);

                    ApplicationUploadFileOut applicationUploadFileOut = new ApplicationUploadFileOut
                    {
                        FileName = file.FileName,
                        FilePath = filePath,
                        DocType = DocType
                    };

                    _logger.Log(LogLevel.Information, $"File : '{file.FileName} || {DocType}' was stored at '{filePath}' on {DateTime.Now:yyyy-MM-dd HH:mm:ss}. ");
                    return Ok(applicationUploadFileOut);
                }
                else
                {
                    _logger.LogError($"API call made without file upload on {DateTime.Now:yyyy-MM-dd HH:mm:ss}.");
                    return BadRequest("File is missing.");
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }   

        [HttpPost("GetFile")]
        public IActionResult GetImage([FromBody]string filePath)
        {
            if (!System.IO.File.Exists(filePath))
            {
                return NotFound();
            }
            var image = System.IO.File.OpenRead(filePath);
            return File(image, "image/jpeg"); // or the appropriate MIME type
        }
        [HttpPost("GetBase64")]
        public IActionResult GetImageAsBase64(string filePath)
        {
            if (!System.IO.File.Exists(filePath))
            {
                return NotFound();
            }
            var imageBytes = System.IO.File.ReadAllBytes(filePath);
            var base64String = Convert.ToBase64String(imageBytes);
            return Ok(new { base64String });
        }
    }
}
