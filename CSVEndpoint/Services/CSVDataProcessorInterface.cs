using System.Data;

namespace CSVEndpoint_API.Services
{
    public interface CSVDataProcessorInterface
    {
        (DataTable, int, int) CsvToDataTable (Stream cvsStream, string cveLayout, string cveCreator);
    }
}
