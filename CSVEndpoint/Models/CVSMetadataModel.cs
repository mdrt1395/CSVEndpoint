namespace CSVEndpoint.Models
{
    public class CVSMetadataModel
    {
        public int Id { get; set; }
        public int LayoutId { get; set; }
        public int TotalColumns { get; set; }
        public int TotalRows { get; set; }
        public DateTime CreationDate { get; set; }

    }
}
