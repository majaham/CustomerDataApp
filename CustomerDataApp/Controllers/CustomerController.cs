using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CustomerDataApp.Data;
using CustomerDataApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CustomerDataApp.Controllers
{
    [Authorize]
    public class CustomerController : Controller
    {
        private readonly CustomerDBContext _dbcontext;

        public CustomerController(CustomerDBContext dbcontext)
        {
            _dbcontext = dbcontext ?? throw new ArgumentNullException(nameof(dbcontext));
        }
       
        [HttpGet]
        public IActionResult Index(int pg = 1)
        {
            const int pageSize = 10;
            if(pg < 1)
            {
                pg = 1;
            }
            int recCount = _dbcontext.CustomerData.Count();
            var pager = new Pager(recCount, pg, pageSize);

            int recSkip = (pg - 1) * pageSize;

            var customers = _dbcontext.CustomerData.Skip(recSkip).Take(pager.PageSize).ToList();

            this.ViewBag.Pager = pager;

            return View(customers);
        }
       
        [HttpGet("CustSearch")]
        public async Task<IActionResult> IndexSearch(string CustSearch)
        {
            List<CustomerData> customers = new();
            ViewBag.GetCustomer = CustSearch;

            if (!String.IsNullOrEmpty(CustSearch))
            {
                customers = await _dbcontext.CustomerData.Where(c => c.Bname.StartsWith(CustSearch)
                || c.Cname.StartsWith(CustSearch)).ToListAsync();
            }

            return View("Index", customers);
        }
        
        public IActionResult Details(int id)
        {
            var customer = _dbcontext.CustomerData.Where(c => c.Serno == id).FirstOrDefault();

            return View(customer);
        }
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public IActionResult Edit(int id)
        {
            var customer = _dbcontext.CustomerData.Where(c => c.Serno == id).FirstOrDefault();

            return View(customer);
        }
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> Edit(CustomerData customer)
        {
            _dbcontext.Attach(customer);
            _dbcontext.Entry(customer).State = EntityState.Modified;
            await _dbcontext.SaveChangesAsync();
            return View(customer);
        }
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public IActionResult Delete(int id)
        {
            var customer = _dbcontext.CustomerData.Where(c => c.Serno == id).FirstOrDefault();

            return View(customer);
        }
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> Delete(CustomerData customer)
        {
            _dbcontext.Attach(customer);
            _dbcontext.Entry(customer).State = EntityState.Deleted;
            await _dbcontext.SaveChangesAsync();
            return RedirectToAction("index");
        }
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public IActionResult Create()
        {
            var customer = new CustomerData();

            return View(customer);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> Create(CustomerData customer)
        {
            _dbcontext.Attach(customer);
            _dbcontext.Entry(customer).State = EntityState.Added;
            await _dbcontext.SaveChangesAsync();
            return RedirectToAction("details", new { id = customer.Serno} );
        }

    }
}
