using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using VacationManagement.Data;
using VacationManagement.Models;

namespace VacationManagement.Controllers
{
    public class VacationTypesController : Controller
    {
        private readonly AppDbContext _context;

        public VacationTypesController(AppDbContext context)
        {
            _context = context;
        }

        // GET: VacationTypes
        public async Task<IActionResult> Index()
        {
            return View(await _context.VacationTypes.ToListAsync());
        }

        // GET: VacationTypes/Details/5
        //public async Task<IActionResult> Details(int? id)
        //{
        //    if (id == null)
        //    {
        //        return NotFound();
        //    }

        //    var vacationType = await _context.VacationTypes
        //        .FirstOrDefaultAsync(m => m.Id == id);
        //    if (vacationType == null)
        //    {
        //        return NotFound();
        //    }

        //    return View(vacationType);
        //}

        // GET: VacationTypes/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: VacationTypes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("VacationName,backgroundColor,NumberDays,Id")] VacationType vacationType)
        {
            if (ModelState.IsValid)
            {
                var result = _context.VacationTypes.FirstOrDefault(x =>x.VacationName.Contains(vacationType.VacationName.Trim()));
                if(result ==null)
                {
                    await _context.VacationTypes.AddAsync(vacationType);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                ViewBag.ErrorMsg = false;
               
            }
            return View(vacationType);
        }

        // GET: VacationTypes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var vacationType = await _context.VacationTypes.FindAsync(id);
            if (vacationType == null)
            {
                return NotFound();
            }
            return View(vacationType);
        }

        // POST: VacationTypes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("VacationName,backgroundColor,NumberDays,Id")] VacationType vacationType)
        {
            if (id != vacationType.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(vacationType);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!VacationTypeExists(vacationType.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(vacationType);
        }

        // GET: VacationTypes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var vacationType = await _context.VacationTypes
                .FirstOrDefaultAsync(m => m.Id == id);
            if (vacationType == null)
            {
                return NotFound();
            }

            return View(vacationType);
        }

        // POST: VacationTypes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var vacationType = await _context.VacationTypes.FindAsync(id);
            if (vacationType != null)
            {
                _context.VacationTypes.Remove(vacationType);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool VacationTypeExists(int id)
        {
            return _context.VacationTypes.Any(e => e.Id == id);
        }
    }
}
