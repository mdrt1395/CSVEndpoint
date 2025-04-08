using System.Collections.Generic;
using System.Data;
using System.IO;
using CSVEndpoint.Services;
using CSVEndpoint_API.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CSVEndpoint.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CSVUploadController : ControllerBase
    {
        private readonly CSVDataProcessorInterface _CSVDataProcessorInterface;

        public CSVUploadController(CSVDataProcessorInterface csvProcessor)
        {
            _CSVDataProcessorInterface = csvProcessor;
        }


        [HttpPost("[action]")]
        public IActionResult UploadFile(IFormFile file, [FromForm] string cveLayout, [FromForm] string cveCreator)
        {
            if (file == null || file.Length == 0)
                return BadRequest();

            if (string.IsNullOrWhiteSpace(cveLayout))
                return BadRequest("File name is required.");

            if (string.IsNullOrWhiteSpace(cveCreator))
                return BadRequest("Uploader's name is required.");

            using (var stream = file.OpenReadStream())
        {
                (DataTable, int, int) dt = _CSVDataProcessorInterface.CsvToDataTable(stream, cveLayout, cveCreator);
            }

            return Ok("CSV processed and pivoted table logged to the terminal.");

        }
    }
}
