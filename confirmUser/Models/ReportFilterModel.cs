namespace confirmUser.Models
{
    public class ReportFilterModel
    {
        public long ApplicationID { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public ReportModel ReportData { get; set; }

       
    }
}
