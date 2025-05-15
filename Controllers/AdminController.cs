using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Project4.Controllers
{
    public class AdminController : Controller
    {
        private inventoryEntities model;

        public AdminController()
        {
            model = new inventoryEntities();
        }

        [HttpGet]
        // GET: Admin
        public ActionResult addAdmin(string at_RefillTextBox, int? userid)
        {
            var list = model.users.ToList();

            user selectedUser = new user();

            if (userid.HasValue)
            {
                selectedUser = model.users.FirstOrDefault(s => s.id == userid.Value);
                if (selectedUser == null)
                {
                    TempData["Message"] = "user not found addAdmin.";
                    selectedUser = new user();
                }
            }

            ViewBag.ActionType = at_RefillTextBox;
            ViewBag.UserList = list;

            return View(selectedUser);
        }

        public ActionResult loginHistory(string statusSearch, string userAdmin)
        {
            var query = model.login_logs.AsQueryable();

            login_logs selectedlogs = new login_logs();

            if (!string.IsNullOrEmpty(statusSearch))
            {
                query = query.Where(s => s.status.Contains(statusSearch));
            }

            if (!string.IsNullOrEmpty(userAdmin))
            {
                query = query.Where(s => s.username.Contains(userAdmin));
            }


            ViewBag.LogList = query.ToList();

            return View();
        }

        [HttpPost]

        public ActionResult LoadUser(int id, string at_RefillTextBox)
        {
            var selectedUser = model.users.FirstOrDefault(s => s.id == id);

            if (at_RefillTextBox == "Delete")
            {

                if (selectedUser != null)
                {
                    TempData["Message"] = "Nganu test delete.";
                    ViewBag.at = at_RefillTextBox;
                    model.users.Remove(selectedUser);
                    model.SaveChanges();
                    TempData["Message"] = "Delete successful.";
                    at_RefillTextBox = "Delete";

                    return RedirectToAction("addAdmin");
                }
            }
            else
            {
                var list = model.users.ToList();

                if (selectedUser == null)
                {
                    TempData["Message"] = "user not found.";
                    return RedirectToAction("addAdmin", new user());
                }


                ViewBag.UserList = list;
                at_RefillTextBox = "Update";
                ViewBag.ActionType = at_RefillTextBox;
            }

            return RedirectToAction("addAdmin", new { at_RefillTextBox = "Update", userid = id });
        }

        public ActionResult addAdmin(user input, string actionType, string userType)
        {
            var list = model.users.ToList();
            ViewBag.ActionType = actionType; // e.g. "Add", "Update", or "Delete"
            //using ()
            // Replace with your actual context
            {
                if (ModelState.IsValid)
                {
                    if (actionType == "Add")
                    {
                        string hashed_pw = BCrypt.Net.BCrypt.EnhancedHashPassword(input.password_hash);
                        var userValues = new user()
                        {
                            username = input.username,
                            password_hash = hashed_pw,
                            role = userType,
                            created_at = DateTime.Now,
                            updated_at = DateTime.Now
                        };
                        model.users.Add(userValues);

                        model.SaveChanges();
                        TempData["Message"] = "user added successfully.";
                    }
                    else if (actionType == "Update")
                    {
                        string hashed_pw = BCrypt.Net.BCrypt.EnhancedHashPassword(input.password_hash);
                        var userToUpdate = model.users.Find(input.id);
                        if (userToUpdate != null)
                        {
                            userToUpdate.username = input.username;
                            userToUpdate.password_hash = hashed_pw;
                            userToUpdate.role = userType;
                            userToUpdate.updated_at = DateTime.Now;
                            model.SaveChanges();
                            TempData["Message"] = "user updated.";
                        }
                        actionType = "Update";
                    }
                }
                else
                {
                    Console.WriteLine("ngaddasdsadsnu ");
                    TempData["errorMessage"] = "Model data is not valid";
                    return View();
                }


                ViewBag.UserList = list;

                return RedirectToAction("addAdmin", new {});
            }
        }
    }
}