using Microsoft.AspNetCore.Mvc.Rendering;
using static System.Net.Mime.MediaTypeNames;

namespace confirmUser.Models
{
    public class tripDetails
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public long TripsNum { get; set; }
        public long HoursNum { get; set; }
        public float TripDistanceKM { get; set; }
       // public string ApplicationName { get; set; }
        public long ApplicationID { get; set; }

        public IEnumerable<SelectListItem>? Applications { get; set; } // Nullable
        public int User { get; set; }
        public DateTime TripDate { get; set; }

    }
}
