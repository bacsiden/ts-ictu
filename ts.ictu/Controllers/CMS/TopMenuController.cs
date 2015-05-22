using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ts.ictu.Controllers
{
    public class TopMenuController : BaseController
    {
        public ActionResult Index()
        {
            return View(DB.Entities.Menu.OrderBy(m => m.ParentID).ThenBy(m => m.Oder).ToList());
        }

        [Authorize]
        public ActionResult NewOrEdit(int? id = 0)
        {
            var db = DB.Entities;
            Menu model = id == 0 ? new Menu() { Activated = true, ParentID = 0, Oder = 0 } : db.Menu.FirstOrDefault(m => m.ID == id);

            var lst = new List<Menu>();
            var obj = new Menu() { ID = 0, Title = "None" };
            lst.Add(obj);
            lst.AddRange(DB.Entities.Menu);
            ViewBag.dropDown = new SelectList(lst.ToList(), "ID", "Title", "");

            var lstcate = new List<Cate>();
            var objcate = new Cate() { ID = 0, Title = "None" };
            lstcate.Add(objcate);
            lstcate.AddRange(DB.Entities.Cate);
            ViewBag.kenhtin = new SelectList(lstcate.ToList(), "ID", "Title", "");

            return View(model);
        }
        [Authorize]
        [HttpPost]
        public ActionResult NewOrEdit(Menu model, FormCollection frm)
        {
            var db = DB.Entities;
            try
            {
                if (model.ID == 0)
                {
                    // Edit                    
                    db.Menu.AddObject(model);
                }
                else
                {
                    // Add new      
                    db.AttachTo("Menu", model);
                    db.ObjectStateManager.ChangeObjectState(model, System.Data.EntityState.Modified);
                }
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            catch
            {
                var lst = new List<Menu>();
                var obj = new Menu() { ID = 0, Title = "None" };
                lst.Add(obj);
                lst.AddRange(DB.Entities.Menu);
                ViewBag.dropDown = new SelectList(lst.ToList(), "ID", "Title", model.ParentID);

                var lstcate = new List<Cate>();
                var objcate = new Cate() { ID = 0, Title = "None" };
                lstcate.Add(objcate);
                lstcate.AddRange(DB.Entities.Cate);
                ViewBag.kenhtin = new SelectList(lstcate.ToList(), "ID", "Title", "");
                return View(model);
            }
        }
        [Authorize]
        public string DoActive(int id)
        {
            try
            {
                var db = DB.Entities;
                var obj = db.Menu.FirstOrDefault(m => m.ID == id);
                obj.Activated = !obj.Activated;
                db.ObjectStateManager.ChangeObjectState(obj, System.Data.EntityState.Modified);
                db.SaveChanges();
                if (obj.Activated == true)
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
        [Authorize]
        public ActionResult Delete(int id)
        {
            try
            {
                var db = DB.Entities;
                var obj = db.Menu.FirstOrDefault(m => m.ID == id);
                db.DeleteObject(obj);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}
