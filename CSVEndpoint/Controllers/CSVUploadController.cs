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
        public IActionResult UploadFile(IFormFile file, [FromForm] string fileName)
        {
            if (file == null || file.Length == 0)
                return BadRequest();

            string finalFileName = string.IsNullOrWhiteSpace(fileName) ? file.FileName : fileName;

            using (var stream = file.OpenReadStream())
        {
                DataTable dt = _CSVDataProcessorInterface.CsvToDataTable(stream, file.FileName);
            }

            return Ok("CSV processed and pivoted table logged to the terminal.");

        }
    }
}
