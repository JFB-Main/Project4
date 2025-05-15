using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;

namespace Project4.Controllers
{
    public class AccountController : Controller
    {
        private inventoryEntities model;

        public AccountController()
        {
            model = new inventoryEntities();
        }
        // GET: Account
        public ActionResult login()
        {
            return View();
        }

        // POST: /Account/Login
        [HttpPost]
        public ActionResult Login(user input)
        {

            if (ModelState.IsValid)
            {
                var existingUser = model.users.FirstOrDefault(u => u.username == input.username);
                //Replace this with your real auth check(DB, etc.)

                // Compare hashed password with input password
                //bool isPasswordValid = BCrypt.Net.BCrypt.Verify(input.password_hash, existingUser.password_hash);

                if (existingUser != null)
                {
                    bool isPasswordValid = BCrypt.Net.BCrypt.EnhancedVerify(input.password_hash, existingUser.password_hash);

                    if (isPasswordValid)
                    {
                        Session["Username"] = input.username;
                        return RedirectToAction("home_supplier", "Landing");
                    }
                }

                // Wrong username or password
                TempData["Message"] = "Invalid username or password.";
                //return RedirectToAction("login");

                return RedirectToAction("home_product", "Landing");

                //if (existingUser != null && isPasswordValid == true)
                //{
                //    // Compare hashed password with input password


                //    // Set login session
                //    Session["Username"] = input.username;

                //    // Redirect to home page or protected page
                //    return RedirectToAction("home_supplier", "Landing");

                //    //string passg = BCrypt.Net.BCrypt.EnhancedHashPassword("test");


                //}
                //else
                //{
                //    return RedirectToAction("home_product", "Landing");
                //}

                //Replace this with your real auth check(DB, etc.)
                //    if (input.username == "admin" && input.password_hash == "1234")
                //{
                //    // Set login session
                //    Session["Username"] = input.username;

                //    // Redirect to home page or protected page
                //    return RedirectToAction("Index", "Home");

                //    //string passg = BCrypt.Net.BCrypt.EnhancedHashPassword("test");

                //}

                //ModelState.AddModelError("", "invalid password or username.");

            }

            return View(input);
        }

        // GET: /Account/Logout
        public ActionResult Logout()
        {
            Session.Clear();
            return RedirectToAction("home_supplier", "Landing");
        }
    }
}