using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Threading.Tasks;
using ONT3001_Assignment.Models;

namespace ONT3001_Assignment.Controllers
{
    public class CustomersController : Controller
    {
        private StoreDBEntities1 db = new StoreDBEntities1();

        // GET: Customers
        public async Task<ActionResult> Index()
        {

            var customer = await db.Customers.OrderByDescending(c => c.CustomerID).Take(10).ToListAsync(); 


            return View(customer);
        }

        // GET: Customers/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Customer customer = await db.Customers.FindAsync(id);
            if (customer == null)
            {
                return HttpNotFound();
            }
            return View(customer);
        }

        // GET: Customers/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Customers/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "CustomerID,FirstName,LastName,Email,PhoneNumber")] Customer customer)
        {
            // Doesnt allow people to have the same 
            bool emailExists = await db.Customers.AnyAsync(c => c.Email == customer.Email);
            if (emailExists)
            {
                ModelState.AddModelError("Email", "A customer with this email already exists.");
                return View(customer);
            }

            if (ModelState.IsValid)
            {
                db.Customers.Add(customer);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return View(customer);
        }

        // GET: Customers/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null) //null ID check
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Customer customer = await db.Customers.FindAsync(id);
            if (customer == null) //Check if custmer exists
            {
                return HttpNotFound();
            }
            return View(customer);
        }

        // POST: Customers/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "CustomerID,FirstName,LastName,Email,PhoneNumber")] Customer customer)
        {
            // Check if they are the same. 
            bool emailExists = await db.Customers.AnyAsync(c => c.Email == customer.Email
                                                           && c.CustomerID != customer.CustomerID);
            if (emailExists)
            {
                ModelState.AddModelError("Email", "A customer with this email already exists.");
                return View(customer);
            }
            if (ModelState.IsValid)
            {
                db.Entry(customer).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(customer);
        }

        // GET: Customers/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)  // Null ID check 
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Customer customer =await db.Customers.FindAsync(id);
            if (customer == null)
            {
                return HttpNotFound();
            }
            return View(customer);
        }

        // POST: Customers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            Customer customer = await db.Customers.FindAsync(id);

            if (customer == null) return HttpNotFound(); // Check if customer actaully exists 

            if (customer.Orders.Any())
            {
                ModelState.AddModelError(String.Empty, "Cannot delete, customer has order in place.");
                return View(customer); 
                
            }
            db.Customers.Remove(customer);
            await db.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
