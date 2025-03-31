using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CSVEndpoint.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CSVUploadController : ControllerBase
    {
        private readonly IWebHostEnvironment _webHostEnvironment;

        public CSVUploadController(IWebHostEnvironment webHostEnvironment)
        {
            _webHostEnvironment = webHostEnvironment;
        }
        [HttpPost("[action]")]
        public IActionResult UploadFile(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest();

            if (Path.GetExtension(file.FileName).ToLower() != ".csv")
                return BadRequest("Only CSV files are allowed.");

            string directoryPath = Path.Combine(_webHostEnvironment.ContentRootPath, "UploadedFiles");
            Directory.CreateDirectory(directoryPath);

           
            string filePath = Path.Combine(directoryPath, file.FileName);
            using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    file.CopyTo(stream);
                }
            
              return Ok();
            
        }
    }
}
