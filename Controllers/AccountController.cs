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
                    login_logs history = new login_logs()
                    {
                        username = input.username,
                        login_time = DateTime.Now,
                        status = "failed"
                    };

                    bool isPasswordValid = BCrypt.Net.BCrypt.EnhancedVerify(input.password_hash, existingUser.password_hash);

                    if (isPasswordValid)
                    {

                        history.status = "success";

                        Session["Username"] = input.username;

                        model.login_logs.Add(history);
                        model.SaveChanges();

                        return RedirectToAction("home_supplier", "Landing");
                    }

                    model.login_logs.Add(history);
                    model.SaveChanges();
                }

                // Wrong username or password
                TempData["Message"] = "Invalid username or password.";
                //return RedirectToAction("login");

                return RedirectToAction("login", "account");

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
            var usernameSession = Session["Username"];
            login_logs history = new login_logs()
            {
                username = usernameSession.ToString(),
                login_time = DateTime.Now,
                status = "logout"
            };

            model.login_logs.Add(history);
            model.SaveChanges();

            Session.Clear();

            return RedirectToAction("home_supplier", "Landing");
        }
    }
}