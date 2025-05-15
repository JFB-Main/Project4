using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Web;
using System.Web.Mvc;

namespace Project4.Controllers
{
    public class StockController : Controller
    {
        private inventoryEntities model;

        public StockController()
        {
            model = new inventoryEntities();
        }
        // GET: Stock
        public ActionResult stockControl(int? actionType, int? supplierName, string stockname)
        {
            var query = model.stocks.AsQueryable();

            var categorylist = model.categories.ToList();

            var supplierlist = model.suppliers_web.ToList();


            category selectedCategory = new category();

            suppliers_web selectedsupplier = new suppliers_web();

            if (supplierName != null)
            {
                query = query.Where(s => s.supplier_id == supplierName);
            }

            if (!string.IsNullOrEmpty(stockname))
            {
                query = query.Where(s => s.stock_name.Contains(stockname));
            }

            if (actionType != null)
            {
                query = query.Where(s => s.category_id == actionType);
            }


            ViewBag.CategoryList = new SelectList(categorylist, "id", "name", selectedCategory.id);

            ViewBag.supplierlist = new SelectList(supplierlist, "id", "name", selectedsupplier.id);

            ViewBag.stockList = query.ToList();

            return View();
        }

        public ActionResult stockHistory(string userAdmin, string stockName, string actionType, int? category, int? supplierName)
        {
            var query = model.history_stocks.AsQueryable();

            var categorylist = model.categories.ToList();

            var supplierlist = model.suppliers_web.ToList();


            category selectedCategory = new category();

            suppliers_web selectedsupplier = new suppliers_web();

            if (supplierName != null)
            {
                query = query.Where(s => s.supplier_id == supplierName);
            }
            if (category != null)
            {
                query = query.Where(s => s.category_id == category);
            }
            if (!string.IsNullOrEmpty(userAdmin))
            {
                query = query.Where(s => s.username.Contains(userAdmin));
            }

            if (!string.IsNullOrEmpty(stockName))
            {
                query = query.Where(s => s.stock_name.Contains(stockName));
            }

            if (actionType != null)
            {
                query = query.Where(s => s.action_type.Contains(actionType));
            }



            ViewBag.CategoryList = new SelectList(categorylist, "id", "name", selectedCategory.id);

            ViewBag.supplierlist = new SelectList(supplierlist, "id", "name", selectedsupplier.id);

            ViewBag.stockList = query.ToList();


            return View();
        }
    }
}