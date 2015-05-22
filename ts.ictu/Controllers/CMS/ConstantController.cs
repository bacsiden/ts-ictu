using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ts.ictu.Controllers
{
    public class ConstantController : BaseController
    {
        public ActionResult Index()
        {
            return View(DB.Entities.mConstant.ToList());
        }
        [Authorize]
        public ActionResult NewOrEdit(int? id = 0)
        {
            var db = DB.Entities;

            var model = DB.Entities.mConstant.FirstOrDefault(m => m.ID == id);
            return View(model);
        }
        [Authorize]
        [HttpPost, ValidateInput(false)]
        public ActionResult NewOrEdit(mConstant model, FormCollection frm)
        {
            var db = DB.Entities;
            try
            {
                if (model.ID == 0)
                {
                    // Edit                    
                    db.mConstant.AddObject(model);
                }
                else
                {
                    // Add new      
                    db.AttachTo("mConstant", model);
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
                var obj = db.mConstant.FirstOrDefault(m => m.ID == id);
                db.mConstant.DeleteObject(obj);
                return View("Index");
            }
            catch
            {
                return RedirectToAction("Index");
            }
        }

        public string Details(string id)
        {
            var db = DB.Entities;
            var obj = db.mConstant.FirstOrDefault(m => m.KeyUrl == id);
            return obj == null ? id : obj.Content;
        }
    }
}
