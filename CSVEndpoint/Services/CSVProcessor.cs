using System.Data;
using CSVEndpoint.Data;
using CSVEndpoint_API.Services;
using Microsoft.EntityFrameworkCore;

namespace CSVEndpoint.Services
{
    public class CSVProcessor: CSVDataProcessorInterface
    {

        private readonly AppDbContext _context;

        public CSVProcessor(AppDbContext context)
        {
            _context = context;
        }

        public (DataTable, int, int) CsvToDataTable(Stream cvsStream, string cveLayout, string cveCreator)
        {
            //to-do: metadata table CON STORED PROCEDURE (investigar lo de INSERT) como mandar a llamar SP desde .NET (USAR DATATABLE COMO PARÁMETRO) 
            DataTable dt = new DataTable();
            
            dt.Columns.Add("CVE_LAYOUT", typeof(string)); //Nombre o identificador del archivo. Se solicita desde el endpoint a quien sube el CVS.
            //dt.Columns.Add("ID_ROW", typeof(int)); //Toma la posición del renglón en el archivo CSV y lo agrega a la fila
            dt.Columns.Add("Field", typeof(string));
            dt.Columns.Add("Value", typeof(string));

            int totalColumns = 0;
            int totalRows = 0;

            using (var reader = new StreamReader(cvsStream))
            {
                bool isHeader = true;
                string[] headers = null;
                //int rowIndex = 0;
                

                while (!reader.EndOfStream) 
                {
                    string line = reader.ReadLine();
                    if(string.IsNullOrEmpty(line) ) continue;
                    string[] values = line.Split(';');
                    
                    if (isHeader)
                    {
                        headers = values.Select(h => h.Trim()).ToArray();
                        totalColumns = headers.Length;
                        isHeader = false;
                    }
                    else
                    {
                        //rowIndex++;
                        totalRows++;

                        if (headers.Length != values.Length)
                        {
                            throw new Exception("CSV row column count does not match header count.");
                        }

                        for (int i = 0; i < headers.Length; i++)
                        {
                            dt.Rows.Add(cveLayout, /*rowIndex */headers[i], values[i]);
                        }
                    }
                }
            }

            _context.Database.ExecuteSqlRaw("EXEC InsertCSVMetadata @p0, @p1, @p2, @p3",
            cveLayout, totalColumns, totalRows, cveCreator);

            int csvId = _context.CSVMetadataModel.OrderByDescending(m => m.Id).Select(m => m.Id).FirstOrDefault();

            SaveCSVDataToDatabase(dt, csvId);

            //LogActualDt(dt);
            return (dt, totalColumns, totalRows);
        }

        public void SaveCSVDataToDatabase(DataTable dt, int csvId)
        {
            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    foreach (DataRow row in dt.Rows)
                    {
                        string field = row["Field"].ToString();
                        string value = row["Value"].ToString();

                        _context.Database.ExecuteSqlRaw("EXEC InsertCSVData @p0, @p1, @p2",
                            csvId, field, value);
                    }

                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    Console.WriteLine("Error inserting CSV data: " + ex.Message);
                }

            }

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
