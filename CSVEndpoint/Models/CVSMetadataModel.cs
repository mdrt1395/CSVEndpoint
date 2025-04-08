namespace CSVEndpoint.Models
{
    public class CVSMetadataModel
    {
        public int Id { get; set; }
        public string CveLayout { get; set; }
        public int TotalColumns { get; set; }
        public int TotalRows { get; set; }
        public string CveCreator { get; set; }
        public DateTime UploadDate { get; set; } = DateTime.UtcNow;


    }
}
