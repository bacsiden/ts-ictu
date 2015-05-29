using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ts.ictu.Controllers
{
    public class LearningRegisterController : BaseController
    {
        #region Front end

        public ActionResult DangKy(int id = 0)
        {
            Models.LearningRegisterModel obj = new Models.LearningRegisterModel();
            if (id != 0 && Session["LearningRegister"] != null)
            {
                var xx = DB.Entities.LearningRegister.FirstOrDefault(m => m.ID == id);
                obj.Address = xx.Address;
                obj.Birthday = xx.Birthday;
                obj.Class = xx.Class;
                obj.Created = xx.Created;
                obj.Email = xx.Email;
                obj.FullName = xx.FullName;
                obj.Gender = xx.Gender == null ? false : xx.Gender.Value;
                obj.Phone = xx.Phone;
                obj.Status = xx.Status;
                return View(obj);
            }
            obj.Birthday = DateTime.Now;
            obj.Gender = true;
            return View(obj);
        }
        [HttpPost]
        public ActionResult DangKy(Models.LearningRegisterModel model, int day, int month, int year, int gender)
        {
            var db = DB.Entities;
            model.Birthday = new DateTime(year, month, day);
            try
            {
                LearningRegister obj = null;
                if (model.ID != 0)
                    obj = db.LearningRegister.FirstOrDefault(m => m.ID == model.ID);
                if (obj == null)
                    obj = new LearningRegister();

                obj.Address = model.Address;
                obj.Birthday = model.Birthday;
                obj.Class = model.Class;
                obj.Created = DateTime.Now;
                obj.Email = model.Email;
                obj.FullName = model.FullName; ;
                obj.Gender = gender == 1 ? true : false;
                obj.Phone = model.Phone;
                obj.Status = 1;

                if (obj.ID == 0)
                    db.LearningRegister.AddObject(obj);
                db.SaveChanges();
                Session["LearningRegister"] = obj;
                return RedirectToAction("DangKySuccess");
            }
            catch
            {
                return View(model);
            }
        }

        public ActionResult DangKySuccess()
        {
            if (Session["LearningRegister"] != null)
            {
                var obj = (LearningRegister)Session["LearningRegister"];
                return View(obj);
            }
            else
                return null;

        }

        public ActionResult Complete()
        {
            if (Session["LearningRegister"] != null)
            {
                var db = DB.Entities;
                var obj = (LearningRegister)Session["LearningRegister"];
                var xx = db.LearningRegister.FirstOrDefault(m => m.ID == obj.ID);
                xx.Status = 2;
                db.SaveChanges();
                return View();
            }
            else
                return null;

        }
        #endregion

        #region Back end
        [Authorize]
        public ActionResult AdminIndex()
        {
            return View(DB.Entities.LearningRegister.ToList());
        }
        [Authorize]
        public ActionResult AdminEdit(int? id = 0)
        {
            var db = DB.Entities;
            var model = DB.Entities.LearningRegister.FirstOrDefault(m => m.ID == id);
            return View(model);
        }
        [Authorize]
        [HttpPost]
        public ActionResult AdminEdit(LearningRegister model)
        {
            var db = DB.Entities;
            try
            {
                //if (model.ID == 0)
                //{
                //    // Edit                    
                //    db.LearningRegister.AddObject(model);
                //}
                //else
                //{
                //    // Add new      
                //    db.AttachTo("LearningRegister", model);
                //    db.ObjectStateManager.ChangeObjectState(model, System.Data.EntityState.Modified);
                //}
                //if (string.IsNullOrEmpty(model.KeyUrl))
                //    model.KeyUrl = Common.CreateURLParam(model.Title);
                //db.SaveChanges();
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
                        var obj = db.LearningRegister.FirstOrDefault(m => m.ID == tmpID);
                        db.LearningRegister.DeleteObject(obj);
                    }
                    db.SaveChanges();
                }
            }
            catch
            {
            }
            return RedirectToAction("AdminIndex");
        }
        #endregion
    }
}
