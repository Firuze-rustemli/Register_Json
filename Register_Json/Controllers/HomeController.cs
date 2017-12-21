using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using Register_Json.Models;

namespace Register_Json.Controllers
{
    public class HomeController : Controller
    {
        RegJsonEntities db = new RegJsonEntities();

        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public JsonResult alreadyExist(string Email)
        {
            if (Email.Length > 0)
            {
                User usr = db.User.FirstOrDefault(u => u.email == Email);
                if (usr == null)
                {
                    var response = new
                    {
                        valid = true
                    };
                    return Json(response, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    var response = new
                    {
                        valid = false,
                        message = "Bu e-poctla artiq qeydiyyat kecmisiniz"
                    };
                    return Json(response, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                var response = new
                {
                    valid = false,
                    message = "Email bos olammalidir"
                };
                return Json(response, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public ActionResult addUser(User usr)
        {
            if (usr.email != null && usr.password != null && usr.fullname != null && usr.phone != null)
            {
                usr.password = Crypto.HashPassword(usr.password);
                db.User.Add(usr);
                db.SaveChanges();
                Session["userAdded"] = "Sizin istifdeciliyiniz yaradildi";
                return RedirectToAction("index");
            }
            else
            {
                return RedirectToAction("index");
            }
        }

        [HttpPost]
        public ActionResult login(User usr)
        {
            if (usr.email != null && usr.password != null )
            {

                User girisEden = db.User.FirstOrDefault(u => u.email == usr.email);
                if (girisEden != null)
                {
                    if (Crypto.VerifyHashedPassword(girisEden.password, usr.password))
                    {
                        Session["LoginMsj"] = "Her sey duzdu";
                        return RedirectToAction("index");
                    }
                    else
                    {
                        Session["LoginMsj"] = "Email ve ya sifre sehvdir";
                        return RedirectToAction("index");
                    }
                }
                else
                {
                    Session["LoginMsj"] = "Email ve ya sifre sehvdir";
                    return RedirectToAction("index");
                }
            }
            else
            {
                return RedirectToAction("index");
            }
        }
    }
}