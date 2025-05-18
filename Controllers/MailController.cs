using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Project4.Controllers
{
    public class MailController : Controller
    {
        private inventoryEntities model;

        public MailController()
        {
            model = new inventoryEntities();
        }

        //[HttpGet]
        // GET: Mail
        public ActionResult mailcontrol(contact input, string mailStatus, string actionType)
        {
            if (Session["Username"] == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var query = model.contacts.AsQueryable();

            if (actionType == "search")
            {
                if (!string.IsNullOrEmpty(mailStatus))
                {
                    query = query.Where(s => s.status.Contains(mailStatus));
                }

                if (!string.IsNullOrEmpty(input.email))
                {
                    query = query.Where(s => s.email.Contains(input.email));
                }
            }
            else if (actionType == "update")
            {
                var mailToUpdate = model.contacts.FirstOrDefault(c => c.id == input.id);
                if (mailToUpdate != null)
                {
                    mailToUpdate.status = mailStatus;

                    model.SaveChanges();

                    var historyEmailValues = new history_contacts()
                    {
                        email = input.email,
                        old_status = "pending",
                        new_status = "responded",
                        username = Session["Username"].ToString(),
                        changed_at = DateTime.Now,
                        message = mailToUpdate.message
                    };

                    model.history_contacts.Add(historyEmailValues);
                    model.SaveChanges();

                    TempData["Message"] = "Supplier updated.";
                }

                return RedirectToAction("mailcontrol", new user());
            }


            ViewBag.EmailList = query.ToList();

            return View();
        }

        public ActionResult mailHistory(string senderEmail, string adminName, string mailStatus)
        {
            if (Session["Username"] == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var query = model.history_contacts.AsQueryable();

            if (!string.IsNullOrEmpty(senderEmail))
            {
                query = query.Where(s => s.email.Contains(senderEmail));
            }
            if (!string.IsNullOrEmpty(adminName)) {
                query = query.Where(s => s.username.Contains(adminName));
            }
            if (!string.IsNullOrEmpty(mailStatus))
            {
                query = query.Where(s => s.new_status.Contains(mailStatus));
            }

            ViewBag.EmailHistoryList = query.ToList();

            return View();
        }
        //[HttpPost]

        //public ActionResult mailAdd(string senderName, string emailName, string comment)
        //{

        //    if (ModelState.IsValid)
        //    {
        //            var emailValues = new contact()
        //            {
        //                name = senderName,
        //                email = emailName,
        //                message = comment,
        //                status = "pending",
        //                created_at = DateTime.Now,
        //                updated_at = DateTime.Now
        //            };
        //            model.contacts.Add(emailValues);

        //            model.SaveChanges();
        //            TempData["Message"] = "Supplier added successfully.";
        //    }
        //    else
        //    {
        //        Console.WriteLine("ngaddasdsadsnu ");
        //        TempData["errorMessage"] = "Model data is not valid";
        //        return View();
        //    }

        //    return RedirectToAction("supplierHistory");
        //}
    }
}