using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ts.ictu.Controllers
{
    public class HtmlPageController : BaseController
    {

        public ActionResult Index()
        {
            return View(DB.Entities.HtmlPage.ToList());
        }
        [Authorize]
        public ActionResult NewOrEdit(int? id = 0)
        {
            var db = DB.Entities;

            var model = DB.Entities.HtmlPage.FirstOrDefault(m => m.ID == id);
            return View(model);
        }
        [Authorize]
        [HttpPost, ValidateInput(false)]
        public ActionResult NewOrEdit(HtmlPage model, FormCollection frm)
        {
            var db = DB.Entities;
            try
            {
                if (model.ID == 0)
                {
                    // Edit                    
                    db.HtmlPage.AddObject(model);
                }
                else
                {
                    // Add new      
                    db.AttachTo("HtmlPage", model);
                    db.ObjectStateManager.ChangeObjectState(model, System.Data.EntityState.Modified);
                }
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            catch
            {
                return View(model);
            }
        }
        [Authorize]
        public ActionResult Delete(int id = 0)
        {
            var db = DB.Entities;
            try
            {
                var obj = db.HtmlPage.FirstOrDefault(m => m.ID == id);
                db.HtmlPage.DeleteObject(obj);
                return View("Index");
            }
            catch
            {
                return RedirectToAction("Index");
            }
        }

        public ActionResult Details(string id)
        {
            var db = DB.Entities;

            var obj = db.HtmlPage.FirstOrDefault(m => m.KeyUrl == id);
            return View(obj);
        }
    }
}
