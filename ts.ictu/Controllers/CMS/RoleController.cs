using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ts.ictu.Controllers
{
    public class RoleController : BaseController
    {
        //
        // GET: /Role/

        [Authorize]
        [ValidationFunction(ActionName.SystemAdmin)]
        public ActionResult Index()
        {
            return View(DB.Entities.mRole);
        }

        [Authorize]
        [ValidationFunction(ActionName.SystemAdmin)]
        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [Authorize]
        [ValidationFunction(ActionName.SystemAdmin)]
        public ActionResult Create(mRole model)
        {
            try
            {
                // TODO: Add insert logic here
                var db = DB.Entities;
                var role = new mRole() { Name = model.Name };
                db.mRole.AddObject(role);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        [Authorize]
        [ValidationFunction(ActionName.SystemAdmin)]
        public ActionResult Edit(int id = 0)
        {
            var obj = DB.Entities.mRole.First(m => m.ID == id);
            if (obj == null)
                return RedirectToAction("Index");
            return View(obj);
        }

        [HttpPost]
        [Authorize]
        [ValidationFunction(ActionName.SystemAdmin)]
        public ActionResult Edit(mRole model)
        {
            try
            {
                // TODO: Add update logic here
                var db = DB.Entities;
                var obj = db.mRole.First(m => m.ID == model.ID);
                obj.Name = model.Name;
                db.ObjectStateManager.ChangeObjectState(obj, System.Data.EntityState.Modified);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        [Authorize]
        [ValidationFunction(ActionName.SystemAdmin)]
        public ActionResult Delete(int id)
        {
            try
            {
                var db = DB.Entities;
                var obj = db.mRole.First(m => m.ID == id);
                db.mRole.DeleteObject(obj);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                return RedirectToAction("Index");
            }
        }

        [Authorize]
        [ValidationFunction(ActionName.SystemAdmin)]
        public ActionResult Function(int id)
        {
            try
            {
                var db = DB.Entities;
                var lst = db.mFunction.Where(m => m.mRole.FirstOrDefault(n => n.ID == id) != null);
                string s = "";
                foreach (var item in db.mFunction)
                {
                    string check = "";
                    if (lst.FirstOrDefault(m => m.ID == item.ID) != null)
                    {
                        check = "checked='checked'";
                    }
                    s += "<label class='checkbox'><input type='checkbox' class='checkitem' " + check + " value='" + item.ID + "' />" + item.Title + "</label>";
                }
                ViewBag.listFuntion = s;
                return View(db.mRole.FirstOrDefault(m => m.ID == id));

            }
            catch (Exception)
            {
                return View();
            }
        }

        [Authorize]
        [ValidationFunction(ActionName.SystemAdmin)]
        public ActionResult DoFunction(int roleID, string listCheck)
        {
            try
            {
                var db = DB.Entities;
                var role = db.mRole.FirstOrDefault(m => m.ID == roleID);
                string[] listChecked = listCheck.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                foreach (var item in db.mFunction)
                {
                    if (listChecked.Contains(item.ID.ToString()))
                    {
                        if (role.mFunction.FirstOrDefault(m => m.ID == item.ID) == null)
                        {
                            role.mFunction.Add(item);
                        }
                    }
                    else
                        if (role.mFunction.FirstOrDefault(m => m.ID == item.ID) != null)
                        {
                            role.mFunction.Remove(item);
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

