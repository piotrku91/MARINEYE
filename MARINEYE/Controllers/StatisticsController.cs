using System.Diagnostics;
using MARINEYE.Areas.Identity.Data;
using MARINEYE.Models;
using MARINEYE.Models.Statistics;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MARINEYE.Controllers
{
    public class StatisticsController : Controller
    {
        private readonly MARINEYEContext _context;

        public StatisticsController(MARINEYEContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var mostUsedBoats = await _context.BoatCalendarEventModel
            .GroupBy(e => e.BoatId) 
            .Select(g => new MostUsedBoatDTO {
                BoatId = g.Key, 
                Boat = g.Select(e => e.Boat).FirstOrDefault(), 
                SumOfUseTimes = g.Count() 
            })
            .OrderByDescending(dto => dto.SumOfUseTimes) 
            .ToListAsync();

            var mostUsedDaysBoats = await _context.BoatCalendarEventModel
            .GroupBy(e => e.BoatId) 
            .Select(g => new MostUsedBoatDTO {
                BoatId = g.Key, 
                Boat = g.Select(e => e.Boat).FirstOrDefault(), 
                SumOfUseTimes = g.Sum(e => EF.Functions.DateDiffDay(e.BeginDate, e.EndDate)) 
        })
        .OrderByDescending(dto => dto.SumOfUseTimes) 
        .ToListAsync();

            return View(Tuple.Create(mostUsedBoats, mostUsedDaysBoats));
        }

    }
}
