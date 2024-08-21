namespace confirmUser.Models
{
    public class ReportModel
    {
        public long SumTripsNum { get; set; } = 0;
        public long SumHoursNum { get; set; } = 0;
        public float SumTripDistanceKM { get; set; } = 0;
        public long FeesPaidToTheCompany { get; set; } = 0;
        public long FuelCost { get; set; } = 0;
        public long ApplicationID { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}
