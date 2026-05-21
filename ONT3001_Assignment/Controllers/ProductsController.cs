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
    public class ProductsController : Controller
    {
        private StoreDBEntities1 db = new StoreDBEntities1();

        // GET: Products
        public async Task<ActionResult> Index()
        {
            // Use .Where() to only show items in stock, and .OrderBy() to sort by price
            var products = await db.Products
                                      .Where(p => p.StockQuantity > 0)
                                      .OrderBy(p => p.Price)
                                      .ToListAsync();

            return View(products);

           
        }

        // GET: Products/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Product product = await db.Products
                                     .Where(p => p.ProductID == id)
                                     .FirstOrDefaultAsync();

            if (product == null)
            {
                return HttpNotFound();
            }
            return View(product);
        }

        // GET: Products/Create
        public ActionResult Create()
        {
            ViewBag.CategoryID = new SelectList(db.Categories, "CategoryID", "CategoryName");
            return View();
        }

        // POST: Products/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult>  Create([Bind(Include = "ProductID,CategoryID,ProductName,Price,StockQuantity")] Product product)
        {

            // validating price- price cant be 0 
            if (product.Price <= 0)
            {
                ModelState.AddModelError("Price", "Price cant be zero.");
                ViewBag.CategoryID = new SelectList(db.Categories, "CategoryID", "CategoryName", product.CategoryID);
                return View(product);
            }

            // the stock cant be below 0 
            if (product.StockQuantity < 0)
            {
                ModelState.AddModelError("StockQuantity", "Cant have negative stock.");
                ViewBag.CategoryID = new SelectList(db.Categories, "CategoryID", "CategoryName", product.CategoryID);
                return View(product);
            }

            // Catagory needs to avalible to actaully be placed in 
            bool categoryExists = await db.Categories.AnyAsync(c => c.CategoryID == product.CategoryID);

            if (!categoryExists)
            {
                ModelState.AddModelError("CategoryID", "Catagory doesnt excists");
                ViewBag.CategoryID = new SelectList(db.Categories, "CategoryID", "CategoryName", product.CategoryID);
                return View(product);
            }

            if (ModelState.IsValid)
            {
                db.Products.Add(product);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            ViewBag.CategoryID = new SelectList(db.Categories, "CategoryID", "CategoryName", product.CategoryID);
            return View(product);
        }

        // GET: Products/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Product product = await db.Products.Where(p=> p.ProductID == id)
                                               .FirstOrDefaultAsync();
            
            if (product == null)
            {
                return HttpNotFound();
            }
            ViewBag.CategoryID = new SelectList(db.Categories, "CategoryID", "CategoryName", product.CategoryID);
            return View(product);
        }

        // POST: Products/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "ProductID,CategoryID,ProductName,Price,StockQuantity")] Product product)
        {
            // procduct price cant be 0 
            if (product.Price <= 0)
            {
                ModelState.AddModelError("Price", "Price cant be zero.");
                ViewBag.CategoryID = new SelectList(db.Categories, "CategoryID", "CategoryName", product.CategoryID);
                return View(product);
            }

            // The Stock cant be less than 0 
            if (product.StockQuantity < 0)
            {
                ModelState.AddModelError("StockQuantity", "Cant have negative stock.");
                ViewBag.CategoryID = new SelectList(db.Categories, "CategoryID", "CategoryName", product.CategoryID);
                return View(product);
            }

            // The category has to exists
            bool categoryExists = await db.Categories.AnyAsync(c => c.CategoryID == product.CategoryID);
            if (!categoryExists)
            {
                ModelState.AddModelError("CategoryID", "Catagory doesnt exist.");
                ViewBag.CategoryID = new SelectList(db.Categories, "CategoryID", "CategoryName", product.CategoryID);
                return View(product);
            }


            if (ModelState.IsValid)
            {
                db.Entry(product).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewBag.CategoryID = new SelectList(db.Categories, "CategoryID", "CategoryName", product.CategoryID);
            return View(product);
        }

        // GET: Products/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Product product = await db.Products.Where(p=>p.ProductID == id)
                                               .FirstOrDefaultAsync(); 

            if (product == null)
            {
                return HttpNotFound();
            }
            return View(product);
        }

        // POST: Products/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async  Task<ActionResult> DeleteConfirmed(int id)
        {
            Product product = await db.Products.Where(p => p.ProductID == id)
                                               .FirstOrDefaultAsync();
            if (product == null) return HttpNotFound();

            db.Products.Remove(product);
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
