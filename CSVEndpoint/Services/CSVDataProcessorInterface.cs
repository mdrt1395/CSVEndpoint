using System.Data;

namespace CSVEndpoint_API.Services
{
    public interface CSVDataProcessorInterface
    {
        DataTable CsvToDataTable (Stream cvsStream);
    }
}
