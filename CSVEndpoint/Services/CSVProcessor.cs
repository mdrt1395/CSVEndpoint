using System.Data;
using CSVEndpoint.Data;
using CSVEndpoint_API.Services;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace CSVEndpoint.Services
{
    public class CSVProcessor: CSVDataProcessorInterface
    {

        private readonly AppDbContext _context;
        private readonly string _connectionString;

        public CSVProcessor(AppDbContext context, IConfiguration configuration)
        {
            _context = context;
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public (DataTable, int, int) CsvToDataTable(Stream cvsStream, string cveLayout, string cveCreator)
        {
            DataTable dt = new DataTable();
            
            dt.Columns.Add("csv_id", typeof(int));
            dt.Columns.Add("Field", typeof(string));
            dt.Columns.Add("Value", typeof(string));

            int totalColumns = 0;
            int totalRows = 0;

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
                        totalColumns = headers.Length;
                        isHeader = false;
                    }
                    else
                    {
                        totalRows++;

                        if (headers.Length != values.Length)
                        {
                            throw new Exception("CSV row column count does not match header count.");
                        }

                        for (int i = 0; i < headers.Length; i++)
                        {
                            dt.Rows.Add(DBNull.Value, headers[i], values[i]);
                        }
                    }
                }
            }

            _context.Database.ExecuteSqlRaw("EXEC InsertCSVMetadata @p0, @p1, @p2, @p3",
            cveLayout, totalColumns, totalRows, cveCreator);

            int csvId = _context.csv_metadata_model
                .OrderByDescending(m => m.Id)
                .Select(m => m.Id)
                .FirstOrDefault();

            foreach (DataRow row in dt.Rows)
            {
                row["csv_id"] = csvId;
            }

            SaveCSVDataToDatabase(dt, csvId);

            return (dt, totalColumns, totalRows);

        }

        public void SaveCSVDataToDatabase(DataTable dt, int csvId)
        {
            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    Console.WriteLine("Attempting to execute stored procedure InsertCSVData...");

                    using (SqlConnection conn = new SqlConnection(_connectionString))
                    {
                        conn.Open();
                        using (SqlCommand cmd = new SqlCommand("InsertCSVData", conn))
                        {
                            cmd.CommandType = CommandType.StoredProcedure;

                            if (!dt.Columns.Contains("csv_id"))
                            {
                                dt.Columns.Add("csv_id", typeof(int));
                            }

                            foreach (DataRow row in dt.Rows)
                            {
                                row["csv_id"] = csvId;
                            }

                            SqlParameter tableParam = cmd.Parameters.AddWithValue("@CSVData", dt);
                            tableParam.SqlDbType = SqlDbType.Structured;
                            tableParam.TypeName = "CSVDataType";

                            int rowsAffected = cmd.ExecuteNonQuery();
                            Console.WriteLine($"Stored procedure executed successfully. Rows inserted: {rowsAffected}");


                        }
                    }
                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error inserting CSV data: {ex.Message} | StackTrace: {ex.StackTrace}");
                    transaction.Rollback();
                }
            }
        }
    }
}
