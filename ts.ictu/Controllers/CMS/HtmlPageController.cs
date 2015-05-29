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
        public ActionResult AdminEdit(int? id = 0)
        {
            var db = DB.Entities;

            var model = DB.Entities.HtmlPage.FirstOrDefault(m => m.ID == id);
            return View(model);
        }
        [Authorize]
        [HttpPost, ValidateInput(false)]
        public ActionResult AdminEdit(HtmlPage model, FormCollection frm)
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
        public ActionResult AdminDelete(string arrayID = "")
        {
            try
            {
                // TODO: Add delete logic here
                var lstID = arrayID.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                var db = DB.Entities;
                if (lstID.Length > 0)
                {
                    foreach (var item in lstID)
                    {
                        int tmpID = int.Parse(item);
                        var obj = db.HtmlPage.FirstOrDefault(m => m.ID == tmpID);
                        db.HtmlPage.DeleteObject(obj);
                    }
                    db.SaveChanges();
                }
            }
            catch
            {
            }
            return RedirectToAction("Index");
        }

        public ActionResult Details(string id)
        {
            var db = DB.Entities;

            var obj = db.HtmlPage.FirstOrDefault(m => m.KeyUrl.Equals(id, StringComparison.OrdinalIgnoreCase));
            return View(obj);
        }

        public string GetContent(string id)
        {
            var db = DB.Entities;

            var obj = db.HtmlPage.FirstOrDefault(m => m.KeyUrl.Equals(id, StringComparison.OrdinalIgnoreCase));
            return obj == null ? null : obj.Content;
        }
    }
}
