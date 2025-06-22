using Microsoft.AspNetCore.Mvc.Rendering;

namespace CloudDevelopmentPOE1.Models
{
    public class EventFilterViewModel
    {
        public string? EventType { get; set; }
        public int? VenueId { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int? EventTypeID { get; set; }
        public bool OnlyAvailableVenues { get; set; }
        public List<SelectListItem>? EventTypes { get; set; }
        public List<SelectListItem>? Venues { get; set; }

        public List<Event>? FilteredEvents { get; set; }
    }
}
