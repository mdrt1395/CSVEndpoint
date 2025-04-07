using System.Data;
using CSVEndpoint_API.Services;

namespace CSVEndpoint.Services
{
    public class CSVProcessor: CSVDataProcessorInterface
    {
        public DataTable CsvToDataTable(Stream cvsStream, string cveLayout)
        {
            //to-do: metadata table CON STORED PROCEDURE (investigar lo de INSERT) como mandar a llamar SP desde .NET (USAR DATATABLE COMO PARÁMETRO) 
            DataTable dt = new DataTable();
            dt.Columns.Add("ID_ROW"); //agarrar posición del renglón en el file y agregar a la fila
            dt.Columns.Add("CVE_LAYOUT"); //nombre del archivo o algo que lo identifique SE SOLICITA a quien sube
            dt.Columns.Add("Field", typeof(string));
            dt.Columns.Add("Value", typeof(string));
            using (var reader = new StreamReader(cvsStream))
            {
                bool isHeader = true;
                string[] headers = null;

                while (!reader.EndOfStream) 
                {
                    string line = reader.ReadLine();
                    if(string.IsNullOrEmpty(line) ) continue;
                    string[] values = line.Split(';');
                    
                    if (isHeader)
                    {
                        headers = values.Select(h => h.Trim()).ToArray();
                        isHeader = false;
                    }
                    else
                    {
                        if (headers.Length != values.Length)
                        {
                            throw new Exception("CSV row column count does not match header count.");
                        }

                        for (int i = 0; i < headers.Length; i++)
                        {
                            dt.Rows.Add(headers[i], values[i]);
                        }
                    }
                }
            }
            LogActualDt(dt);
            return dt;
        }
        private void LogActualDt(DataTable dt)
        {
            foreach (DataRow row in dt.Rows) 
            {
                Console.WriteLine($"{row["Field"]}: {row["Value"]}");
            }
        }
    }
}
