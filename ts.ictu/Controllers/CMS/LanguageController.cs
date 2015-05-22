using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ts.ictu.Controllers
{
    public class LanguageController : BaseController
    {
        [Authorize]
        public ActionResult AdminIndex()
        {
            return View(DB.Entities.mLanguage.ToList());
        }
        [Authorize]
        public ActionResult AdminEdit(int? id = 0)
        {
            var db = DB.Entities;

            var model = DB.Entities.mLanguage.FirstOrDefault(m => m.ID == id);
            return View(model);
        }
        [Authorize]
        [HttpPost]
        public ActionResult AdminEdit(mLanguage model)
        {
            var db = DB.Entities;
            try
            {
                if (model.ID == 0)
                {
                    // Edit               
                    db.mLanguage.AddObject(model);
                }
                else
                {
                    // Add new      
                    db.AttachTo("mLanguage", model);
                    db.ObjectStateManager.ChangeObjectState(model, System.Data.EntityState.Modified);
                }
                db.SaveChanges();
                return RedirectToAction("AdminIndex");
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
                        var obj = db.mLanguage.FirstOrDefault(m => m.ID == tmpID);
                        db.mLanguage.DeleteObject(obj);
                    }
                    db.SaveChanges();
                }
            }
            catch
            {
            }
            return RedirectToAction("AdminIndex");
        }
    }
}
