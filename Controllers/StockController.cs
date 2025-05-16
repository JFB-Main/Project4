using System;
using System.Collections.Generic;
using System.IO;
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
        [HttpGet]
        public ActionResult stockControl(string actionTypeupd, int? actionType, int? supplierName, string stockname, int? stockid)
        {
            //if (Session["Username"] == null)
            //{
            //    return RedirectToAction("Login", "Account");
            //}

            var query = model.stocks.AsQueryable();

            var categorylist = model.categories.ToList();

            var supplierlist = model.suppliers.ToList();


            category selectedCategory = new category();

            supplier selectedsupplier = new supplier();

            stock selectedStock = new stock();

            if (stockid.HasValue)
            {
                selectedStock = model.stocks.FirstOrDefault(s => s.id == stockid.Value);
                if (selectedStock == null)
                {
                    TempData["Message"] = "Supplier not found.";
                    selectedStock = new stock();
                }
            }
            else
            {
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
            }


            ViewBag.CategoryList = new SelectList(categorylist, "id", "name", selectedCategory.id);

            ViewBag.supplierlist = new SelectList(supplierlist, "id", "name", selectedsupplier.id);

            ViewBag.stockList = query.ToList();

            ViewBag.ActionType = actionTypeupd;

            return View(selectedStock);
        }

        public ActionResult stockHistory(string userAdmin, string stockName, string actionType, int? category, int? supplierName)
        {
            if (Session["Username"] == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var query = model.history_stocks.AsQueryable();

            var categorylist = model.categories.ToList();

            var supplierlist = model.suppliers.ToList();


            category selectedCategory = new category();

            supplier selectedsupplier = new supplier();

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

        [HttpPost]

        public ActionResult LoadStock(int id, string actionTypeupd)
        {
            //if (Session["Username"] == null)
            //{
            //    return RedirectToAction("Login", "Account");
            //}


            var selectedStock = model.stocks.FirstOrDefault(s => s.id == id);

            if (actionTypeupd == "Delete")
            {

                if (selectedStock != null)
                {
                    //TempData["Message"] = "Nganu test delete.";
                    model.stocks.Remove(selectedStock);
                    model.SaveChanges();
                    TempData["Message"] = "Delete successful.";
                }
                actionTypeupd = "Delete";

                return RedirectToAction("stockControl");
            }
            else
            {
                var categories = model.categories.ToList();

                if (selectedStock == null)
                {
                    TempData["Message"] = "Supplier not found.";
                    return View("stockControl", new stock());
                }


                ViewBag.CategoryList = new SelectList(categories, "id", "name");
                ViewBag.SupplierList = categories;

                //ViewBag.supplierlist = new SelectList(supplierlist, "id", "name", selectedsupplier.id);

                actionTypeupd = "Update";
                ViewBag.ActionType = actionTypeupd;
            }

            return RedirectToAction("stockControl", new { actionTypeupd = "Update", stockid = id });
        }

        public ActionResult stockControl(stock input, string actionTypeupd, int? actionType, int? supplierName)
        {
            //if (Session["Username"] == null)
            //{
            //    return RedirectToAction("Login", "Account");
            //}

            //var categories = model.suppliers_web.ToList();
            //ViewBag.CategoryList = new SelectList(categories, "id", "name");
            //ViewBag.SupplierList = categories;
            ViewBag.ActionType = actionTypeupd; // e.g. "Add", "Update", or "Delete"
            //using ()
            // Replace with your actual context
            {
                if (ModelState.IsValid)
                {
                    if (actionTypeupd == "Add")
                    {
                        if (ModelState.IsValid)
                        {
                            var stockValues = new stock()
                            {
                                stock_name = input.stock_name,
                                description = input.description,
                                stock_price = input.stock_price,
                                stock_amount = input.stock_amount,
                                supplier_id = supplierName,
                                category_id = actionType
                            };
                            model.stocks.Add(stockValues);
                            model.SaveChanges();
                            TempData["Message"] = "Stock successfully added.";
                            return RedirectToAction("stockControl");
                        }

                        // In case of validation failure, reload lists and return view
                        ViewBag.CategoryList = new SelectList(model.categories.ToList(), "id", "name", input.category_id);
                        ViewBag.supplierlist = new SelectList(model.suppliers_web.ToList(), "id", "name", input.supplier_id);
                        ViewBag.stockList = model.stocks.ToList();
                        ViewBag.ActionType = "Add";
                        return View("stockControl", input);
                    }
                    else if (actionTypeupd == "Update")
                    {
                        var existingStock = model.stocks.Find(input.id);
                        if (existingStock != null)
                        {
                            existingStock.stock_name = input.stock_name;
                            existingStock.description = input.description;
                            existingStock.stock_price = input.stock_price;
                            existingStock.stock_amount = input.stock_amount;
                            existingStock.supplier_id = supplierName;
                            existingStock.category_id = actionType;

                            model.SaveChanges();

                            TempData["Message"] = "Stock updated successfully.";
                            return RedirectToAction("stockControl");
                        }
                        else
                        {
                            TempData["errorMessage"] = "Stock not found for update.";
                        }

                        // Reload form with existing data if failed
                        ViewBag.CategoryList = new SelectList(model.categories.ToList(), "id", "name", input.category_id);
                        ViewBag.supplierlist = new SelectList(model.suppliers.ToList(), "id", "name", input.supplier_id);
                        ViewBag.stockList = model.stocks.ToList();
                        ViewBag.ActionType = "Update";
                        return View("stockControl", input);
                    }
                    //else if (actionTypeupd == "Update")
                    //{
                    //    //ViewBag.textboxdata = categories;
                    //    var supplierToUpdate = model.stocks.Find(input.id);
                    //    if (supplierToUpdate != null)
                    //    {
                    //        supplierToUpdate.name = input.name;
                    //        supplierToUpdate.description = input.description;
                    //        supplierToUpdate.address = input.address;
                    //        supplierToUpdate.image_path = "/images/" + input.image_path;
                    //        model.SaveChanges();
                    //        TempData["Message"] = "Supplier updated.";
                    //    }
                    //    actionType = "Update";
                    //}
                    //else if (actionType == "Delete")
                    //{
                    //    var supplierToDelete = model.suppliers_web.Find(input.id);
                    //    if (supplierToDelete != null)
                    //    {
                    //        model.suppliers_web.Remove(supplierToDelete);
                    //        model.SaveChanges();
                    //        TempData["Message"] = "Supplier deleted.";
                    //    }
                    //    actionType = "Delete";
                    //}
                }
                else
                {
                    Console.WriteLine("ngaddasdsadsnu ");
                    TempData["errorMessage"] = "Model data is not valid";
                    return View();
                }
            }

            return RedirectToAction("stockControl"); // Or wherever your main page is
        }
    }
}