using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VacationManagement.Data;
using VacationManagement.Models;

namespace VacationManagement.Controllers
{
    public class EmployeesController : Controller
    {
        private readonly AppDbContext _context;

        public EmployeesController(AppDbContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index()
        {
            return View(await _context.Employees.Include(x => x.Department).ToListAsync());
        }
        //.OrderBy(x => x.Id).
        public async Task<IActionResult> Create()
        {
            ViewBag.Departments =await _context.Departments.OrderBy(x => x.Name).ToListAsync();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Employee model)
        {
            if (ModelState.IsValid)
            {
                _context.Employees.Add(model);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewBag.Departments = await _context.Departments.OrderBy(x => x.Name).ToListAsync();
            return View(model);
        }

        public async Task<IActionResult> Edit(int? Id)
        {
            ViewBag.Departments =await _context.Departments.OrderBy(x => x.Name).ToListAsync();
            return View(await _context.Employees.Include(x => x.Department).FirstOrDefaultAsync(x => x.Id == Id));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Employee model)
        {
            if (ModelState.IsValid)
            {
                _context.Employees.Update(model);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewBag.Departments =await _context.Departments.OrderBy(x => x.Name).ToListAsync();
            return View(model);
        }

        public async Task<IActionResult> Delete(int? Id)
        {
            return View(await _context.Employees.Include(x => x.Department).FirstOrDefaultAsync(x => x.Id == Id));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(Employee model)
        {
            if (model != null)
            {
                _context.Employees.Remove(model);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return View(model);
        }
    }
}
