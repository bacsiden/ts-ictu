using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Webdiyer.WebControls.Mvc;

namespace ts.ictu.Controllers
{
    [Authorize]
    public class ConfigController : BaseController
    {
        public ActionResult Index()
        {
            return View(DB.Entities.mConfig.ToList());
        }
        public ActionResult NewOrEdit(int? id = 0)
        {
            var obj = DB.Entities.mConfig.FirstOrDefault(m => m.ID == id);
            return View(obj);
        }
        [HttpPost]
        public ActionResult NewOrEdit(mConfig model, FormCollection frm)
        {
            try
            {
                var db = DB.Entities;
                if (model.ID == 0)
                {
                    db.mConfig.AddObject(model);
                }
                else
                {
                    db.AttachTo("mConfig", model);
                    db.ObjectStateManager.ChangeObjectState(model, System.Data.EntityState.Modified);
                }
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
