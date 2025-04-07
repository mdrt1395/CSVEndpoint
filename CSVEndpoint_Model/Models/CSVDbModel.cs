using System.ComponentModel.DataAnnotations;

namespace CSVEndpoint.Models
{
    public class CSVDbModel
    {
        [Key]
        public int Id { get; set; }
        public string? LayoutId { get; set; } 
        public string? ColumnName { get; set; } 
        public string? ColumnValue { get; set; }
    }
}
