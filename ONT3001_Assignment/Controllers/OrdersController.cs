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
    public class OrdersController : Controller
    {
        private StoreDBEntities1 db = new StoreDBEntities1();

        // GET: Orders
        public async Task<ActionResult> Index()
        {
            var orders = await db.Orders.Include(o => o.Customer).Include(o => o.Product)
                                         .OrderByDescending(o => o.OrderDate).ToListAsync(); 

            return View(orders);
        }

        // GET: Orders/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Order order = await db.Orders.FindAsync(id);
            if (order == null)
            {
                return HttpNotFound();
            }
            return View(order);
        }

        // GET: Orders/Create
        public ActionResult Create()
        {
            ViewBag.CustomerID = new SelectList(db.Customers, "CustomerID", "FirstName");
            ViewBag.ProductID = new SelectList(db.Products, "ProductID", "ProductName");
            return View();
        }

        // POST: Orders/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "OrderID,CustomerID,ProductID,OrderDate,QuantityBought")] Order order)
        {
            if (order.QuantityBought <= 0)
            {
                ModelState.AddModelError("QuantityBought", "Quantity must be greater than zero.");
                ViewBag.CustomerID = new SelectList(db.Customers, "CustomerID", "FirstName", order.CustomerID);
                ViewBag.ProductID = new SelectList(db.Products, "ProductID", "ProductName", order.ProductID);
                return View(order);
            }

            bool customerExists = await db.Customers.AnyAsync(c => c.CustomerID == order.CustomerID);
            bool productExists = await db.Products.AnyAsync(p => p.ProductID == order.ProductID);

            if (!customerExists)
            {
                ModelState.AddModelError("CustomerID", "Selected customer does not exist.");
            }
            if (!productExists)
            {
                ModelState.AddModelError("ProductID", "Selected product does not exist.");
            }

            if (!customerExists || !productExists)
            {
                ViewBag.CustomerID = new SelectList(db.Customers, "CustomerID", "FirstName", order.CustomerID);
                ViewBag.ProductID = new SelectList(db.Products, "ProductID", "ProductName", order.ProductID);
                return View(order);
            }


            if (ModelState.IsValid)
            {
                db.Orders.Add(order);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            ViewBag.CustomerID = new SelectList(db.Customers, "CustomerID", "FirstName", order.CustomerID);
            ViewBag.ProductID = new SelectList(db.Products, "ProductID", "ProductName", order.ProductID);
            return View(order);
        }

        // GET: Orders/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Order order = await db.Orders.FindAsync(id);
            if (order == null)
            {
                return HttpNotFound();
            }
            ViewBag.CustomerID = new SelectList(db.Customers, "CustomerID", "FirstName", order.CustomerID);
            ViewBag.ProductID = new SelectList(db.Products, "ProductID", "ProductName", order.ProductID);
            return View(order);
        }

        // POST: Orders/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "OrderID,CustomerID,ProductID,OrderDate,QuantityBought")] Order order)
        {

            if (order.QuantityBought <= 0)
            {
                ModelState.AddModelError("QuantityBought", "Quantity must be greater than zero.");
                ViewBag.CustomerID = new SelectList(db.Customers, "CustomerID", "FirstName", order.CustomerID);
                ViewBag.ProductID = new SelectList(db.Products, "ProductID", "ProductName", order.ProductID);
                return View(order);
            }

            
            bool customerExists = await db.Customers.AnyAsync(c => c.CustomerID == order.CustomerID);
            bool productExists = await db.Products.AnyAsync(p => p.ProductID == order.ProductID);

            if (!customerExists)
            {
                ModelState.AddModelError("CustomerID", "Selected customer does not exist.");
            }
            if (!productExists)
            {
                ModelState.AddModelError("ProductID", "Selected product does not exist.");
            }

            if (!customerExists || !productExists)
            {
                ViewBag.CustomerID = new SelectList(db.Customers, "CustomerID", "FirstName", order.CustomerID);
                ViewBag.ProductID = new SelectList(db.Products, "ProductID", "ProductName", order.ProductID);
                return View(order);
            }


            if (ModelState.IsValid)
            {
                db.Entry(order).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewBag.CustomerID = new SelectList(db.Customers, "CustomerID", "FirstName", order.CustomerID);
            ViewBag.ProductID = new SelectList(db.Products, "ProductID", "ProductName", order.ProductID);
            return View(order);
        }

        // GET: Orders/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Order order = await db.Orders.FindAsync(id);
            if (order == null)
            {
                return HttpNotFound();
            }
            return View(order);
        }

        // POST: Orders/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult>  DeleteConfirmed(int id)
        {
            Order order = await db.Orders.FindAsync(id);
            if (order == null) return HttpNotFound(); 

            db.Orders.Remove(order);
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
