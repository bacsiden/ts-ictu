using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ts.ictu.Controllers
{
    [Authorize]    
    public class FunctionController : BaseController
    {
        //
        // GET: /Function/

        [Authorize]
        [ValidationFunction(ActionName.SystemAdmin)]
        public ActionResult Index()
        {
            var db = DB.Entities;
            var list = db.mFunction.ToList();
            return View(list);
        }
        [Authorize]
        [ValidationFunction(ActionName.SystemAdmin)]
        public ActionResult Menu(int id)
        {
            try
            {
                var db = DB.Entities;
                var lst = db.mMenu.Where(m => m.mFunction.FirstOrDefault(n => n.ID == id) != null);
                string s = "";
                foreach (var item in db.mMenu)
                {
                    string check = "";
                    if (lst.FirstOrDefault(m => m.ID == item.ID) != null)
                    {
                        check = "checked='checked'";
                    }
                    s += "<label class='checkbox'><input type='checkbox' class='checkitem' " + check + " value='" + item.ID + "' />" + item.Title + "</label>";
                }
                ViewBag.listMenu = s;
                return View(db.mFunction.FirstOrDefault(m => m.ID == id));

            }
            catch (Exception)
            {
                return View();
            }
        }
        [Authorize]
        [HttpPost]
        [ValidationFunction(ActionName.SystemAdmin)]
        public ActionResult Menu(int functionID, string listCheck)
        {
            try
            {
                var db = DB.Entities;
                var function = db.mFunction.FirstOrDefault(m => m.ID == functionID);
                string[] listChecked = listCheck.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                foreach (var item in db.mMenu)
                {
                    if (listChecked.Contains(item.ID.ToString()))
                    {
                        if (function.mMenu.FirstOrDefault(m => m.ID == item.ID) == null)
                        {
                            function.mMenu.Add(item);
                        }
                    }
                    else
                        if (function.mMenu.FirstOrDefault(m => m.ID == item.ID) != null)
                        {
                            function.mMenu.Remove(item);
                        }
                }

                db.SaveChanges();
                return RedirectToAction("index");
            }
            catch (Exception)
            {
                return View();
            }
        }

    }
}
