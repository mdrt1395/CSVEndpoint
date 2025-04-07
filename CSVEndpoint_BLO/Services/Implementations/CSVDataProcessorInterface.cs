using System.Data;

namespace CSVEndpoint_BLO.Services.Implementations
{
    public interface CSVDataProcessorInterface
    {
        List<Dictionary<string, object>> ExtractAndPivotData(Stream filestream);

    }
}
