using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Webdiyer.WebControls.Mvc;
using System.Data.Objects.SqlClient;
using Microsoft.Reporting.WebForms;
using System.Data;
using System.IO;

namespace ts.ictu.Controllers
{

    public class PostController : BaseController
    {
        #region Admin Page
        int pageSize = 5;
        [Authorize]
        public ActionResult AdminIndex(int? page, int? hinhthuc, int? loaiID, int? status, int? noibat)
        {
            var db = DB.Entities;
            var lst = db.Post.Where(m => !m.Deleted && (loaiID == null ? true : m.CateID == loaiID)
                && (status == null || m.Status == status) && (noibat == null || m.Hot == true));

            var list = lst.OrderByDescending(m => m.ID).ToPagedList(!page.HasValue ? 0 : page.Value, pageSize);
            if (Request.IsAjaxRequest())
            {
                return PartialView("_AdminIndex", list);
            }
            SelectOption();
            return View(list);
        }
        [Authorize]
        public ActionResult ListDeleted(int? page)
        {
            var db = DB.Entities;
            var lst = db.Post.Where(m => m.Deleted);
            return View(lst.OrderBy(m => m.ID).ToPagedList(!page.HasValue ? 0 : page.Value, pageSize));
        }
        void SelectOption()
        {
            #region SELECT OPTION
            var db = DB.Entities;

            string dataLoai = "<option value=''>- Tất cả kênh tin -</option>";
            foreach (var item in db.Cate.ToList())
            {
                dataLoai += string.Format("<option value='{0}'>{1}</option>", item.ID, item.Title);
            }
            ViewBag.dataLoai = dataLoai;
            #endregion
        }
        [Authorize]
        public ActionResult AdminEdit(int? id = 0)
        {
            var obj = DB.Entities.Post.FirstOrDefault(m => m.ID == id);
            var lstcate = new List<Cate>();
            var objcate = new Cate() { ID = 0, Title = "None" };
            lstcate.Add(objcate);
            lstcate.AddRange(DB.Entities.Cate);
            ViewBag.kenhtin = new SelectList(lstcate.ToList(), "ID", "Title", "");

            return View(obj);
        }

        [Authorize]
        [HttpPost, ValidateInput(false)]
        public ActionResult AdminEdit(Post model, FormCollection frm, HttpPostedFileBase file)
        {
            try
            {
                var db = DB.Entities;
                Post obj = null;
                if (model.ID == 0)
                {
                    obj = new Post();
                    obj.Created = DateTime.Now.Date;
                    obj.UserID = CurrentUser.ID;
                    obj.Status = (int)PostStatus.Enabled;
                    obj.Hot = true;
                    obj.ViewCount = 0;
                    obj.Deleted = false;
                }
                else
                    obj = db.Post.FirstOrDefault(m => m.ID == model.ID);
                obj.Title = model.Title;
                obj.Summary = model.Summary;
                obj.Content = model.Content;
                obj.CateID = model.CateID;
                if (file != null)
                {
                    var now = DateTime.Now;
                    var fileName = string.Format("{0}-{1}-{2}-{3}-{4}-{5}", now.Day, now.Hour, now.Minute, now.Second,
                        now.Millisecond, CurrentUser.Email.Replace("@", "--")) + Path.GetExtension(file.FileName);
                    file.SaveAs(Path.Combine(Server.MapPath("~/Uploads/"), fileName));
                    obj.ImageUrl = "/Uploads/" + fileName;
                }
                if (model.ID == 0)
                    db.Post.AddObject(obj);
                db.SaveChanges();
            }
            catch
            {
                var lstcate = new List<Cate>();
                var objcate = new Cate() { ID = 0, Title = "None" };
                lstcate.Add(objcate);
                lstcate.AddRange(DB.Entities.Cate);
                ViewBag.kenhtin = new SelectList(lstcate.ToList(), "ID", "Title", "");
                return View(model);
            }
            return RedirectToAction("AdminIndex");

        }
        [Authorize]
        public ActionResult AdminDelete(int id)
        {
            var db = DB.Entities;
            var obj = db.Post.FirstOrDefault(m => m.ID == id);
            db.DeleteObject(obj);
            db.SaveChanges();
            return RedirectToAction("ListDeleted");
        }
        [Authorize]
        public ActionResult AdminDeleteAll()
        {
            var db = DB.Entities;
            var lst = db.Post.Where(m => m.Deleted);
            foreach (var item in lst)
            {
                db.DeleteObject(item);
            }
            db.SaveChanges();
            return RedirectToAction("ListDeleted");
        }
        [Authorize]
        public ActionResult SetDeleted(int id)
        {
            var db = DB.Entities;
            var obj = db.Post.FirstOrDefault(m => m.ID == id);
            obj.Deleted = true;
            db.SaveChanges();
            return RedirectToAction("AdminIndex");
        }
        [Authorize]
        public ActionResult Restore(int id)
        {
            var db = DB.Entities;
            var obj = db.Post.FirstOrDefault(m => m.ID == id);
            obj.Deleted = false;
            db.SaveChanges();
            return RedirectToAction("ListDeleted");
        }
        [Authorize]
        public string DoEnable(int id)
        {
            try
            {
                var db = DB.Entities;
                var obj = db.Post.FirstOrDefault(m => m.ID == id);
                obj.Status = obj.Status == 1 ? 2 : 1;
                db.ObjectStateManager.ChangeObjectState(obj, System.Data.EntityState.Modified);
                db.SaveChanges();
                if (obj.Status == 2)
                {
                    return "Đã đăng";
                }
                return "<span class='validation-summary-errors'>Chờ đăng</span>";
            }
            catch (Exception ex)
            {
                return "";
            }

        }
        [Authorize]
        public string DoHot(int id)
        {
            try
            {
                var db = DB.Entities;
                var obj = db.Post.FirstOrDefault(m => m.ID == id);
                obj.Hot = !obj.Hot;
                db.ObjectStateManager.ChangeObjectState(obj, System.Data.EntityState.Modified);
                db.SaveChanges();
                if (obj.Hot == true)
                {
                    return "Tin nổi bật";
                }
                return "<span class='validation-summary-errors'>Tin thường</span>";
            }
            catch
            {
                return "";
            }
        }
        #endregion

        #region Present page
        public ActionResult Details(string id)
        {
            int id1 = Common.GetIDFromURLParam(id);
            var db = DB.Entities;
            var obj = db.Post.FirstOrDefault(m => m.ID == id1);
            if (obj == null)
                return NotFound();
            var lstRelate = db.Post.Where(m => m.CateID == obj.CateID && m.Status == (int)PostStatus.Enabled)
                .OrderByDescending(m => m.ID).Take(8).ToList();
            ViewBag.lstRelate = lstRelate;
            return View(obj);
        }
        public ActionResult RightPanel(Post model)
        {
            return PartialView("_RightPanel", model);
        }
        public ActionResult Channel(string id)
        {
            var db = DB.Entities;

            var cate = db.Cate.FirstOrDefault(m => m.KeyUrl == id);
            if (cate == null)
                return NotFound();
            ViewBag.cate = cate;
            var lst = db.Post.Where(m => m.CateID == cate.ID && m.Status == (int)PostStatus.Enabled).OrderByDescending(m => m.Created).Take(10).ToList();
            return View(lst);
        }
        #endregion
    }
}
