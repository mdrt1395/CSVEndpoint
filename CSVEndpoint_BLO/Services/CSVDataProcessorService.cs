using System.Globalization;
using System.Text.RegularExpressions;
using CSVEndpoint_BLO.Services.Implementations;
using CsvHelper;
using CsvHelper.Configuration;

namespace CSVEndpoint.Services
{
    public class CSVDataProcessorService: CSVDataProcessorInterface
    {
        public List<Dictionary<string, object>> ExtractAndPivotData(Stream fileStream)
        {
            using var reader = new StreamReader(fileStream);
            using var csv = new CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture));
            csv.Read();
            csv.ReadHeader();
            var headers = csv.HeaderRecord.ToList();

            if (headers.Count < 2) throw new InvalidDataException("Se necesitan mínimo dos filas en el CSV para poder pivotearlas.");

            string keyColumn = headers[0];
            var dataRecords = new List<Dictionary<string, object>>();

            while (csv.Read())
            {
                var record = new Dictionary<string, object>
                {
                    { keyColumn, csv.GetField(keyColumn) }
                };

                foreach (var column in headers.Skip(1))
                {
                    record[column] = csv.GetField(column);
                }
                dataRecords.Add(record);
            }

            var pivotedData = dataRecords
                .GroupBy(r => r[keyColumn].ToString())
                .Select(group =>
                {
                    var row = new Dictionary<string, object> { { keyColumn, group.Key } };
                    foreach (var record in group) {
                        foreach (var column in headers.Skip(1)){
                            row[column] = record[column];
                        } } 
                    return row;
                }
                ).ToList();
            return pivotedData;
        }
    }
}
