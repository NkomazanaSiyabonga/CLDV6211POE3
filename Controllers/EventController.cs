using CloudDevelopmentPOE1.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace CloudDevelopmentPOE1.Controllers
{
    public class EventController : Controller
    {
        private readonly ApplicationDbContext _context;

        public EventController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Event/Index
        public IActionResult Index(EventFilterViewModel filter)
        {
            var events = _context.Event
     .Include(e => e.Venue)
     .Include(e => e.EventType) 
     .AsQueryable();


            filter.EventTypes = _context.EventType
     .AsEnumerable() 
     .GroupBy(et => et.Name)
     .Select(g => g.First())
     .Select(et => new SelectListItem
     {
         Value = et.EventTypeID.ToString(),
         Text = et.Name
     })
     .OrderBy(et => et.Text)
     .ToList();
            ;


            filter.Venues = _context.Venue
                .Select(v => new SelectListItem
                {
                    Value = v.VenueId.ToString(),
                    Text = v.Name
                }).ToList();

            if (filter.EventTypeID.HasValue)
                events = events.Where(e => e.EventTypeID == filter.EventTypeID.Value);

            if (filter.VenueId.HasValue)
                events = events.Where(e => e.VenueId == filter.VenueId.Value);

            if (filter.StartDate.HasValue)
                events = events.Where(e => e.EventDate >= filter.StartDate.Value);

            if (filter.EndDate.HasValue)
                events = events.Where(e => e.EventDate <= filter.EndDate.Value);

            if (filter.OnlyAvailableVenues)
            {
                var bookedVenueIds = _context.Event
                    .Where(e =>
                        (!filter.StartDate.HasValue || e.EventDate >= filter.StartDate.Value) &&
                        (!filter.EndDate.HasValue || e.EventDate <= filter.EndDate.Value))
                    .Select(e => e.VenueId)
                    .Distinct()
                    .ToList();

                events = events.Where(e => !bookedVenueIds.Contains(e.VenueId));
            }

            filter.EventTypes = _context.EventType
                .AsEnumerable()
                .GroupBy(et => et.Name)
                .Select(g => g.First())
                .Select(et => new SelectListItem
                {
                    Value = et.EventTypeID.ToString(),
                    Text = et.Name
                })
                .OrderBy(et => et.Text)
                .ToList();

            filter.Venues = _context.Venue
                .Select(v => new SelectListItem
                {
                    Value = v.VenueId.ToString(),
                    Text = v.Name
                })
                .ToList();

            filter.FilteredEvents = events.ToList();

            return View(filter);
        }

        // GET: Event/Details/{id}
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var @event = await _context.Event
                .Include(e => e.Venue)
                .FirstOrDefaultAsync(m => m.EventId == id);

            if (@event == null) return NotFound();

            return View(@event);
        }

        // GET: Event/Create
        public IActionResult Create()
        {
            ViewData["Venues"] = _context.Venue.ToList();
            ViewData["EventTypes"] = _context.EventType.ToList();
            return View(); 
        }


        // POST: Event/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Event @event)
        {

            

            if (ModelState.IsValid)
            {
                // 1. Check venue availability
                bool isVenueTaken = await _context.Event
                    .AnyAsync(e => e.VenueId == @event.VenueId && e.EventDate == @event.EventDate);

                if (isVenueTaken)
                {
                    ModelState.AddModelError("", "The venue is already booked for this date and time.");
                }
                else
                {
                    // 2. Handle custom EventType
                    if (!string.IsNullOrWhiteSpace(@event.EventTypeText))
                    {
                        var existingType = await _context.EventType
                            .FirstOrDefaultAsync(et => et.Name.ToLower() == @event.EventTypeText.ToLower());

                        if (existingType != null)
                        {
                            @event.EventTypeID = existingType.EventTypeID;
                        }
                        else
                        {
                            var newType = new EventType { Name = @event.EventTypeText };
                            _context.EventType.Add(newType);
                            await _context.SaveChangesAsync();
                            @event.EventTypeID = newType.EventTypeID;
                        }
                    }

                    _context.Event.Add(@event);
                    await _context.SaveChangesAsync();

                    TempData["SuccessMessage"] = "Event created successfully.";
                    return RedirectToAction(nameof(Index));
                }
            }

            // Re-populate dropdowns if model is invalid
            ViewData["Venues"] = _context.Venue.ToList();
            ViewData["EventTypes"] = _context.EventType.ToList();
            return View(@event);
        }



        // GET: Event/Edit/{id}

        private bool EventExists(int id)
        {
            return _context.Event.Any(e => e.EventId == id);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var @event = await _context.Event.FindAsync(id);
            if (@event == null) return NotFound();

            ViewData["Venues"] = _context.Venue.ToList();


            ViewData["EventTypes"] = _context.EventType.ToList();
            return View(@event);
        }

        // POST: Event/Edit/{id}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Event @event)
        {
            if (id != @event.EventId) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(@event);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Event updated successfully.";
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Event.Any(e => e.EventId == @event.EventId))
                        return NotFound();
                    else
                        throw;
                }
            }

            ViewData["Venues"] = _context.Venue.ToList();
            return View(@event);
        }

        // GET: Event/Delete/{id}
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var @event = await _context.Event
                .Include(e => e.Venue)
                .FirstOrDefaultAsync(m => m.EventId == id);

            if (@event == null) return NotFound();

            return View(@event);
        }

        // POST: Event/Delete/{id}
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var @event = await _context.Event.FindAsync(id);

            if (@event == null)
            {
                TempData["ErrorMessage"] = "Event not found.";
                return RedirectToAction(nameof(Index));
            }

            // Check for existing bookings
            bool hasEventBookings = await _context.Booking.AnyAsync(b => b.EventId == id);
            if (hasEventBookings)
            {
                TempData["ErrorMessage"] = "Cannot delete this event as it has active bookings.";
                return RedirectToAction(nameof(Index));
            }

            try
            {
                _context.Event.Remove(@event);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Event deleted successfully.";
            }
            catch (DbUpdateException ex)
            {
                // Log the error (optional)
                Console.WriteLine(ex.Message);
                TempData["ErrorMessage"] = "Error deleting the event.";
            }

            return RedirectToAction(nameof(Index));
        }


           
        }


    }


