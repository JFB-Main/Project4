using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;


namespace Project4.Controllers
{
    public class LandingController : Controller
    {

        private inventoryEntities model;

        public LandingController()
        {
            model = new inventoryEntities();
        }

        // GET: Landing
        [HttpGet]
        public ActionResult home_supplier()
        {
            var listofData = model.suppliers_web.ToList();
            ViewBag.ListofData = listofData;
            return View();
        }

        public ActionResult home_product()
        {
            var listofData = model.stocks.ToList();
            ViewBag.ListofData = listofData;
            return View();
        }

        [HttpPost]
        public ActionResult mailAdd(contact input)
        {
            if (ModelState.IsValid)
            {

                var emailValues = new contact()
                {
                    name = input.name,
                    email = input.email,
                    message = input.message,
                    status = "pending",
                    created_at = DateTime.Now,
                    updated_at = DateTime.Now
                };

                model.contacts.Add(emailValues);
                model.SaveChanges();

                TempData["Message"] = "Message sent successfully.";
                return RedirectToAction("home_supplier"); // <- Important!
            }
            else
            {
                TempData["errorMessage"] = "Model data is not valid";
                return RedirectToAction("home_supplier");
            }
        }


        //public ActionResult Create(stock input)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        var stockValues = new stock()
        //        {
        //            stock_name = input.stock_name,
        //            description = input.description,
        //        };

        //        model.stocks.Add(stockValues);
        //        model.SaveChanges();
        //        TempData["successMessage"] = "Stock Inserted Succesfully";
        //        return RedirectToAction("Index");
        //    }
        //    else
        //    {
        //        TempData["errorMessage"] = "Model data is not valid";
        //        return View();
        //    }

        //}

    }
}