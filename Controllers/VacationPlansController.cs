using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using VacationManagement.Data;
using VacationManagement.Models;

namespace VacationManagement.Controllers
{
    public class VacationPlansController : Controller
    {
        private readonly AppDbContext _context;

        public VacationPlansController(AppDbContext context)
        {
            _context = context;
        }
        
        public async Task<IActionResult> Index()
        {
            //var vacationPlanCount = await _context.VacationPlans.CountAsync();
            //ViewBag.VacationPlanCount = vacationPlanCount; // Pass count to the view



            return View(await _context.RequestVacations
                .Include(x => x.Employee)
                .Include(x => x.VacationType)
                .OrderByDescending(x => x.RequestDate)
                .ToListAsync());
        }

        public async Task<IActionResult> Create()
        {
            ViewBag.Employees = new SelectList(await _context.Employees.OrderBy(x => x.Name).ToListAsync(), "Id", "Name");
            ViewBag.VacationTypes = new SelectList(await _context.VacationTypes.OrderBy(x => x.VacationName).ToListAsync(), "Id", "VacationName");
            return View();
        }





        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(VacationPlan model, int[] DayOfWeekCheckbox)
        {
            ViewBag.Employees = new SelectList(await _context.Employees.OrderBy(x => x.Name).ToListAsync(), "Id", "Name");
            ViewBag.VacationTypes = new SelectList(await _context.VacationTypes.OrderBy(x => x.VacationName).ToListAsync(), "Id", "VacationName");
            if (ModelState.IsValid)
            {
                var Result = await _context.VacationPlans
                    .Where(x => x.RequestVacation.EmployeeId == model.RequestVacation.EmployeeId
                    && x.VacationDate >= model.RequestVacation.StartDate
                    && x.VacationDate <= model.RequestVacation.EndDate)
                    .FirstOrDefaultAsync();

                if (Result != null)
                {
                    ViewBag.ErrorVacation = false;
                    return View(model);
                }

                for (DateTime date = model.RequestVacation.StartDate;
                    date <= model.RequestVacation.EndDate; date = date.AddDays(1))
                {
                    if (Array.IndexOf(DayOfWeekCheckbox, (int)date.DayOfWeek) != -1)
                    {
                        model.Id = 0;
                        model.VacationDate = date;
                        model.RequestVacation.RequestDate = DateTime.Now;
                        await _context.VacationPlans.AddAsync(model);
                        await _context.SaveChangesAsync();
                    }
                }
                return RedirectToAction("Index");
            }

            return View(model);
        }
        //[HttpGet]

        public IActionResult Edit(int? Id)
        {
            //ViewBag.Employees =  _context.Employees.OrderBy(x => x.Name).ToList();
            //ViewBag.VacationTypes = _context.VacationTypes.OrderBy(x => x.VacationName).ToList(); // Fixed Typo

            var requestVacation= _context.RequestVacations
                .Include(x => x.Employee)
                .Include(x => x.VacationType)
                .Include(x => x.VacationPlanList).FirstOrDefault(x => x.Id == Id);
            if(requestVacation==null)
            {
                return NotFound();
            }
            ViewBag.Employees = _context.Employees.OrderBy(x => x.Name).ToList();
            ViewBag.VacationTypes = _context.VacationTypes.OrderBy(x => x.VacationName).ToList(); // Fixed Typo
            return View(requestVacation);
        }
        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public IActionResult Edit(RequestVacation model)
        {
          
            if (ModelState.IsValid)
            {
                if (model.Approved == true)
                     model.DateApproved = DateTime.Now;
                _context.RequestVacations.Update(model);
                _context.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.Employees = _context.Employees.OrderBy(x => x.Name).ToList();
            ViewBag.VactionTypes = _context.VacationTypes.OrderBy(x => x.VacationName).ToList();

            return View(model);
        }
        public async Task<IActionResult> GetCountVacationEmployee(int? Id)
        {
            return Json(await _context.VacationPlans.Where(x => x.RequestVacationId == Id).CountAsync());
        }
        public async Task<IActionResult> GetVacationTypes()
        {
            return Json(await _context.VacationTypes.OrderBy(x => x.Id).ToListAsync());
        }
        public async Task<IActionResult> Delete(int? Id)
        {
            return View(await _context.RequestVacations
                .Include(x => x.Employee)
                .Include(x => x.VacationType)
                .Include(x => x.VacationPlanList)
                .FirstOrDefaultAsync(x => x.Id == Id));
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int Id)
        {
            var requestVacation = await _context.RequestVacations
                .Include(x => x.VacationPlanList)
                .FirstOrDefaultAsync(x => x.Id == Id);

            if (requestVacation == null)
            {
                return NotFound();
            }

            // Remove associated vacation plans first
            _context.VacationPlans.RemoveRange(requestVacation.VacationPlanList);
            _context.RequestVacations.Remove(requestVacation);

            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        //[HttpPost]
        //public async Task<IActionResult> Delete(RequestVacation model)
        //{
        //    if (model != null)
        //    {
        //        _context.RequestVacations.Remove(model);
        //        await _context.SaveChangesAsync();
        //        return RedirectToAction("Index");
        //    }
        //    return View(model);
        //}

        public async Task<IActionResult> ViewReportVactionPlan()
        {
            ViewBag.Employees = await _context.Employees.ToListAsync();
            return View();
        }

        //public async Task <IActionResult> ViewReportVactionPlan2()
        //{
        //    ViewBag.Employees =await _context.Employees.ToListAsync();
        //    return View();
        //}
        public async Task<IActionResult> GetReportVactionPlan
                (int EmployeeId, DateTime FromDate, DateTime ToDate)
        {
            string Id = "";
            if (EmployeeId != 0 && EmployeeId.ToString() != "")
                Id = $"and Employees.Id={EmployeeId}";
            #region MyRegion
            var sqlQuery = _context.SqlDataTable($@"SELECT Distinct dbo.Employees.Id, dbo.Employees.Name, 
               dbo.Employees.VacationBalance,
               Sum(dbo.VacationTypes.NumberDays) As TotalVacation,
			   dbo.Employees.VacationBalance - Sum(dbo.VacationTypes.NumberDays) As Total
               FROM   dbo.Employees INNER JOIN
             dbo.RequestVacations ON dbo.Employees.Id = dbo.RequestVacations.EmployeeId INNER JOIN
             dbo.VacationPlans ON dbo.RequestVacations.Id = dbo.VacationPlans.RequestVacationId INNER JOIN
             dbo.VacationTypes ON dbo.RequestVacations.VacationtypeId = dbo.VacationTypes.Id

			 where VacationPlans.VacationDate between '" + FromDate.ToString("yyyy-MM-dd") + "' and '" + ToDate.ToString("yyyy-MM-dd") + "'"+

             " and RequestVacations.Approved='True'" +
             $"{Id} Group By dbo.Employees.Id, dbo.Employees.Name, dbo.Employees.VacationBalance");




            ViewBag.Employees = await _context.Employees.ToListAsync();

            return View("ViewReportVactionPlan", sqlQuery);
            //var SpGetData = _context.SpGetReportVacationPlans
            //            .FromSqlRaw("SpGetReportVacationPlans {0},{1},{2}", EmployeeId, FromDate, ToDate).ToList();

            //ViewBag.Employees = await _context.Employees.ToListAsync();
            //return View("ViewReportVactionPlan", SpGetData);
            ////}
            #endregion

            //#region MyRegion
            //string sqlQuery = $@"SELECT Distinct dbo.Employees.Id, dbo.Employees.Name,
            //            dbo.Employees.VacationBalance,
            //Sum(dbo.VacationTypes.NumberDays) As TotalVacation,
            //dbo.Employees.VacationBalance - Sum(dbo.VacationTypes.NumberDays) As Total
            //            FROM dbo.Employees INNER JOIN
            //            dbo.RequestVacations ON dbo.Employees.Id = dbo.RequestVacations.EmployeeId INNER JOIN
            //            dbo.VacationPlans ON dbo.RequestVacations.Id = dbo.VacationPlans.RequestVacationId INNER JOIN
            //            dbo.VacationTypes ON dbo.RequestVacations.VacationTypeId = dbo.VacationTypes.Id
            //   where VacationPlans.VacationDate between 
            //            '" + FromDate.ToString("yyyy-MM-dd") + "' and '" + ToDate.ToString("yyyy-MM-dd") + "' " +
            //            " and RequestVacations.Approved = 'True'" +
            //            $"{Id} Group By dbo.Employees.Id, dbo.Employees.Name, dbo.Employees.VacationBalance";
            //#endregion


            //var SpGetData =await _context.SpGetReportVacationPlans.FromSqlRaw(sqlQuery).ToListAsync();

            //var SpGetData = _context.SpGetReportVacationPlans
            //            .FromSqlRaw("SpGetReportVacationPlans {0},{1},{2}", EmployeeId, FromDate, ToDate).ToList();

            //ViewBag.Employees = await _context.Employees.ToListAsync();
            //return View("ViewReportVactionPlan2", SpGetData);
            //}
        }
    }
    }