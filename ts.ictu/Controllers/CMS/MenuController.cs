using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ts.ictu.Controllers
{
    public class MenuController : BaseController
    {
        [Authorize]
        [ValidationFunction(ActionName.SystemAdmin)]
        public ActionResult Index()
        {
            return View(DB.Entities.mMenu.OrderBy(m=>m.ParentID).ThenBy(m=>m.Oder));
        }

        [Authorize]
        [ValidationFunction(ActionName.SystemAdmin)]
        public ActionResult Create()
        {
            var db = new List<mMenu>();
            var obj = new mMenu() { ID = 0, Title = "None" };
            db.Add(obj);
            db.AddRange(DB.Entities.mMenu);
            SelectList select = new SelectList(db.ToList(), "ID", "Title");
            ViewBag.dropDown = select;
            return View(new mMenu());
        }

        //
        // POST: /CuocPhi/Create

        [HttpPost]
        [Authorize]
        [ValidationFunction(ActionName.SystemAdmin)]
        public ActionResult Create(mMenu model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    // TODO: Add insert logic here
                    var db = DB.Entities;
                    if (model.ParentID == 0)
                    {
                        model.ParentID = null;
                    }
                    if (!model.Oder.HasValue)
                    {
                        model.Oder = 100;
                    }                    
                    model.IsActive = true;
                    db.mMenu.AddObject(model);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                else
                {
                    var lst = new List<mMenu>();
                    var obj = new mMenu() { ID = 0, Title = "None" };
                    lst.Add(obj);
                    lst.AddRange(DB.Entities.mMenu);
                    SelectList select = new SelectList(lst.ToList(), "ID", "Title");
                    ViewBag.dropDown = select;
                    return View();
                }
            }
            catch
            {
                return View();
            }
        }

        //
        // GET: /CuocPhi/Edit/5

        [Authorize]
        [ValidationFunction(ActionName.SystemAdmin)]
        public ActionResult Edit(int id)
        {
            var obj = DB.Entities.mMenu.FirstOrDefault(m => m.ID == id);
            var db = new List<mMenu>();
            var dfobj = new mMenu() { ID = 0, Title = "None" };
            db.Add(dfobj);
            db.AddRange(DB.Entities.mMenu);
            SelectList select = new SelectList(db.ToList(), "ID", "Title", DB.Entities.mMenu.FirstOrDefault(m => m.ID == obj.ParentID));
            ViewBag.dropDown = select;

            return View(obj);
        }

        //
        // POST: /CuocPhi/Edit/5

        [HttpPost]
        [Authorize]
        [ValidationFunction(ActionName.SystemAdmin)]
        public ActionResult Edit(mMenu model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    // TODO: Add update logic here
                    var db = DB.Entities;
                    var obj = db.mMenu.FirstOrDefault(m => m.ID == model.ID);
                    obj.Title = model.Title;
                    obj.Oder = model.Oder;
                    if (model.ParentID == 0)
                    {
                        model.ParentID = null;
                    }
                    obj.ParentID = model.ParentID;
                    obj.Url = model.Url;
                    db.ObjectStateManager.ChangeObjectState(obj, System.Data.EntityState.Modified);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                else
                {
                    var obj = DB.Entities.mMenu.FirstOrDefault(m => m.ID == model.ID);
                    var db = new List<mMenu>();
                    var dfobj = new mMenu() { ID = 0, Title = "None" };
                    db.Add(dfobj);
                    db.AddRange(DB.Entities.mMenu);
                    SelectList select = new SelectList(db.ToList(), "ID", "Title", DB.Entities.mMenu.FirstOrDefault(m => m.ID == obj.ParentID));
                    ViewBag.dropDown = select;
                    return View(model);
                }

            }
            catch
            {
                return View();
            }
        }

        //
        // GET: /CuocPhi/Delete/5



        [Authorize]
        [ValidationFunction(ActionName.SystemAdmin)]
        public ActionResult Delete(int id)
        {
            try
            {
                var db = DB.Entities;
                var obj = db.mMenu.FirstOrDefault(m => m.ID == id);
                db.DeleteObject(obj);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
        [ValidationFunction(ActionName.SystemAdmin)]
        public string DoActive(int id)
        {
            try
            {
                var db = DB.Entities;
                var obj = db.mMenu.FirstOrDefault(m => m.ID == id);
                obj.IsActive = !obj.IsActive;
                db.ObjectStateManager.ChangeObjectState(obj, System.Data.EntityState.Modified);
                db.SaveChanges();
                if (obj.IsActive == true)
                {
                    return "Đã bật";
                }
                return "<span class='validation-summary-errors'>Đã tắt</span>";
            }
            catch (Exception ex)
            {

                return "";
            }
        }
    }
}
