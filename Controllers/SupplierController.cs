using System;
using System.Configuration;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Project4.Controllers
{

    public class SupplierController : Controller
    {
        private inventoryEntities model;

        public SupplierController()
        {
            model = new inventoryEntities();
        }
        [HttpGet]
        // GET: Supplier
        public ActionResult supplierControl(string actionType, int? supplierId)
        {
            if (Session["Username"] == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var categories = model.suppliers_web.ToList();

            suppliers_web selectedSupplier = new suppliers_web();

            if (supplierId.HasValue)
            {
                selectedSupplier = model.suppliers_web.FirstOrDefault(s => s.id == supplierId.Value);
                if (selectedSupplier == null)
                {
                    TempData["Message"] = "Supplier not found.";
                    selectedSupplier = new suppliers_web();
                }
            }

            ViewBag.CategoryList = new SelectList(categories, "id", "name", selectedSupplier.id);
            ViewBag.SupplierList = categories;
            ViewBag.ActionType = actionType;

            return View(selectedSupplier);
        }

        public ActionResult supplierHistory(string actionType, string supplierName, string userAdmin)
        {
            if (Session["Username"] == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var query = model.history_suppliers_web.AsQueryable();

            if (!string.IsNullOrEmpty(supplierName))
            {
                query = query.Where(s => s.name.Contains(supplierName));
            }

            if (!string.IsNullOrEmpty(userAdmin))
            {
                query = query.Where(s => s.username.Contains(userAdmin));
            }

            if (!string.IsNullOrEmpty(actionType))
            {
                query = query.Where(s => s.action_type == actionType);
            }

            ViewBag.HistoryList = query.ToList();
            return View();
        }

        [HttpPost]

        public ActionResult LoadSupplier(int id, string actionType)
        {
            if (Session["Username"] == null)
            {
                return RedirectToAction("Login", "Account");
            }


            var selectedSupplier = model.suppliers_web.FirstOrDefault(s => s.id == id);

            if (actionType == "Delete")
            {

                if (selectedSupplier != null)
                {
                    //TempData["Message"] = "Nganu test delete.";
                    model.suppliers_web.Remove(selectedSupplier);
                    model.SaveChanges();
                    TempData["Message"] = "Delete successful.";
                }
                actionType = "Delete";

                return RedirectToAction("supplierControl");
            }
            else
            {
                var categories = model.suppliers_web.ToList();

                if (selectedSupplier == null)
                {
                    TempData["Message"] = "Supplier not found.";
                    return View("supplierControl", new suppliers_web());
                }


                ViewBag.CategoryList = new SelectList(categories, "id", "name");
                ViewBag.SupplierList = categories;
                actionType = "Update";
                ViewBag.ActionType = actionType;
            }

            return RedirectToAction("supplierControl", new { actionType = "Update", supplierId = id });
        }

        public ActionResult supplierControl(suppliers_web input, HttpPostedFileBase UploadFile, string actionType)
        {
            if (Session["Username"] == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var categories = model.suppliers_web.ToList();
            ViewBag.CategoryList = new SelectList(categories, "id", "name");
            ViewBag.SupplierList = categories;
            ViewBag.ActionType = actionType; // e.g. "Add", "Update", or "Delete"
            //using ()
            // Replace with your actual context
            {
                if (ModelState.IsValid)
                {
                    if (actionType == "Add")
                    {
                        string fileName = null;
                        string imagePath = "";

                        // Check if a file is uploaded
                        if (UploadFile != null && UploadFile.ContentLength > 0)
                        {
                            // Get file name
                            fileName = Path.GetFileName(UploadFile.FileName);

                            // Define path to save in the project folder
                            string path = Path.Combine(Server.MapPath("~/Content/images/"), fileName);

                            // Save file to disk
                            UploadFile.SaveAs(path);

                            // Set image_path relative for database
                            imagePath = "/images/" + fileName;
                        }

                        var supplierValues = new suppliers_web()
                        {
                            name = input.name,
                            description = input.description,
                            address = input.address,
                            image_path = imagePath
                        };
                        var supplierNormalValues = new supplier()
                        {
                            name = input.name,
                            description = input.description,
                            address = input.address
                        };

                        model.suppliers_web.Add(supplierValues);
                        model.suppliers.Add(supplierNormalValues);
                        //model.history_suppliers_web.Add(supplierValues);
                        model.SaveChanges();

                        history_suppliers_web historySupplierWeb = new history_suppliers_web()
                        {
                            name = input.name,
                            description = input.description,
                            address = input.address,
                            image_path = imagePath,
                            username = Session["Username"].ToString(),
                            action_type = "INSERT",
                            created_at = DateTime.Now,
                        };

                        var selectedSupplierHistory = model.suppliers.OrderByDescending(s => s.id).FirstOrDefault();
                        history_suppliers historySupplier = new history_suppliers()
                        {
                            name = input.name,
                            description = input.description,
                            address = input.address,
                            username = Session["Username"].ToString(),
                            action_type = "INSERT",
                            action_time = DateTime.Now,
                            supplier_id = selectedSupplierHistory.id
                        };

                        model.history_suppliers_web.Add(historySupplierWeb);
                        model.history_suppliers.Add(historySupplier);
                        //model.history_suppliers_web.Add(supplierValues);
                        model.SaveChanges();

                        TempData["Message"] = "Supplier added successfully.";
                    }
                    else if (actionType == "Update")
                    {
                        ViewBag.textboxdata = categories;
                        var supplierToUpdate = model.suppliers_web.Find(input.id);
                        if (supplierToUpdate != null)
                        {
                            supplierToUpdate.name = input.name;
                            supplierToUpdate.description = input.description;
                            supplierToUpdate.address = input.address;
                            supplierToUpdate.image_path = "/images/" + input.image_path;
                            TempData["Message"] = "Supplier updated.";

                            var supplierOriToUpdate = model.suppliers.Where(s => s.name == input.name).FirstOrDefault();

                            supplierOriToUpdate.name = input.name;
                            supplierOriToUpdate.description = input.description;
                            supplierOriToUpdate.address = input.address;

                            model.SaveChanges();

                            history_suppliers_web historySupplierWeb = new history_suppliers_web()
                            {
                                name = supplierToUpdate.name,
                                description = supplierToUpdate.description,
                                address = supplierToUpdate.address,
                                image_path = supplierToUpdate.image_path,
                                username = Session["Username"].ToString(),
                                action_type = "UPDATE",
                                created_at = DateTime.Now,
                            };

                            var selectedSupplierHistory = model.suppliers.Where(s => s.name == input.name).FirstOrDefault();
                            history_suppliers historySupplier = new history_suppliers()
                            {
                                name = input.name,
                                description = input.description,
                                address = input.address,
                                username = Session["Username"].ToString(),
                                action_type = "UPDATE",
                                action_time = DateTime.Now,
                                supplier_id = selectedSupplierHistory.id
                            };

                            model.history_suppliers_web.Add(historySupplierWeb);
                            model.history_suppliers.Add(historySupplier);
                            model.SaveChanges();
                        }
                        actionType = "Update";
                    }
                    else if (actionType == "Delete")
                    {
                        var supplierwebToDelete = model.suppliers_web.Find(input.id);
                        var supplierToDelete = model.suppliers.Where(s => s.name == input.name).FirstOrDefault();
                        if (supplierwebToDelete != null)
                        {
                            history_suppliers_web historySupplierWeb = new history_suppliers_web()
                            {
                                name = supplierwebToDelete.name,
                                description = supplierwebToDelete.description,
                                address = supplierwebToDelete.address,
                                image_path = supplierwebToDelete.image_path,
                                username = Session["Username"].ToString(),
                                action_type = "DELETE",
                                created_at = DateTime.Now,
                            };

                            var selectedSupplierHistory = model.suppliers.Where(s => s.name == input.name).FirstOrDefault();
                            history_suppliers historySupplier = new history_suppliers()
                            {
                                name = supplierToDelete.name,
                                description = supplierToDelete.description,
                                address = supplierToDelete.address,
                                username = Session["Username"].ToString(),
                                action_type = "DELETE",
                                action_time = DateTime.Now,
                                supplier_id = supplierToDelete.id
                            };

                            model.history_suppliers.Add(historySupplier);
                            model.history_suppliers_web.Add(historySupplierWeb);
                            model.suppliers_web.Remove(supplierwebToDelete);
                            model.suppliers.Remove(supplierToDelete);
                            model.SaveChanges();
                            TempData["Message"] = "Supplier deleted.";
                        }
                        actionType = "Delete";
                    }
                }
                else
                {
                    Console.WriteLine("ngaddasdsadsnu");
                    TempData["errorMessage"] = "Model data is not valid";
                    return View();
                }
            }

            return RedirectToAction("supplierControl"); // Or wherever your main page is
        }
    }

}