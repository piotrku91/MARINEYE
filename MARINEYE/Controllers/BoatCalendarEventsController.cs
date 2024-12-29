using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MARINEYE.Areas.Identity.Data;
using MARINEYE.Models;

namespace MARINEYE.Controllers
{
    public class BoatCalendarEventsController : Controller
    {
        private readonly MARINEYEContext _context;

        public BoatCalendarEventsController(MARINEYEContext context)
        {
            _context = context;
        }

        // GET: BoatCalendarEvents
        public async Task<IActionResult> Index()
        {
            var mARINEYEContext = _context.BoatCalendarEventModel.Include(b => b.Boat).Include(b => b.User);
            return View(await mARINEYEContext.ToListAsync());
        }


        // GET: BoatCalendarEvents/Create
        public IActionResult Create()
        {
            ViewData["BoatId"] = new SelectList(_context.BoatModel, "Id", "Name");
            return View();
        }

        // POST: BoatCalendarEvents/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,BeginDate,EndDate,BoatId")] BoatCalendarEventDTO boatCalendarEventDTO)
        {
            if (ModelState.IsValid)
            {
                BoatCalendarEvent boatCalendarEvent = new BoatCalendarEvent();
                boatCalendarEvent.Id = boatCalendarEventDTO.Id;
                boatCalendarEvent.BeginDate = boatCalendarEventDTO.BeginDate;
                boatCalendarEvent.EndDate = boatCalendarEventDTO.EndDate;
                boatCalendarEvent.BoatId = boatCalendarEventDTO.BoatId;
                var userId = User.Identity.Name; // or User.FindFirstValue(ClaimTypes.NameIdentifier) depending on how user id is stored
                var currentUser = await _context.Users.FirstOrDefaultAsync(u => u.UserName == userId);
                boatCalendarEvent.UserId = userId;
                boatCalendarEvent.User = currentUser;
                boatCalendarEvent.EventState = Utilities.BoatCalendarEventState.Reserved;

                _context.Add(boatCalendarEvent);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            if (!ModelState.IsValid) {
                var errors = ModelState.Values.SelectMany(v => v.Errors);
                foreach (var error in errors) {
                    Console.WriteLine(error.ErrorMessage);
                }
   
            }

            ViewData["BoatId"] = new SelectList(_context.BoatModel, "Id", "Name", boatCalendarEventDTO.BoatId);
            return View(boatCalendarEventDTO);
        }

    }
}
