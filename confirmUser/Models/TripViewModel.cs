using Microsoft.AspNetCore.Mvc.Rendering;

namespace confirmUser.Models
{
    public class TripViewModel
    {
        public string Name { get; set; }
        public long Id { get; set; }
        public long TripsNum { get; set; }
        public long HoursNum { get; set; }
        public float TripDistanceKM { get; set; }
        public string TripDate { get; set; }
        public string ApplicationName { get; set; }
        public string TaxFis { get; set; }
        public long ApplicationID { get; set; } // Add this if it needs to be part of the model
        public IEnumerable<SelectListItem> Applications { get; set; }
    }
}
