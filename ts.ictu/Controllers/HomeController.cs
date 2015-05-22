using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Threading;
using System.Globalization;
using System.Data;
using System.Data.SqlClient;
using ts.ictu.Models;
using System.IO;

namespace ts.ictu.Controllers
{
    public class HomeController : BaseController
    {
        public ActionResult Index()
        {
            var db = DB.Entities;

            return View(db.Cate.ToList());
        }
        public ActionResult Details()
        {
            var db = DB.Entities;

            return View();
        }
        [Authorize]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult InsertFile(int id, HttpPostedFileBase upload, string CKEditorFuncNum, string CKEditor, string langCode)
        {
            string message = "Upload failure!"; // message to display (optional) 
            string url = ""; // url to return 
            if (upload.ContentLength > 0)
            {
                var now = DateTime.Now;
                var fileName = string.Format("{0}-{1}-{2}-{3}-{4}-{5}", now.Day, now.Hour, now.Minute, now.Second,
                    now.Millisecond, CurrentUser.Email.Replace("@", "--")) + Path.GetExtension(upload.FileName);
                upload.SaveAs(Path.Combine(Server.MapPath("~/Uploads/"), fileName));
                url = "/Uploads/" + fileName;
                message = "Upload success!";
            }
            // since it is an ajax request it requires this string 
            string output = @"<html><body><script>window.parent.CKEDITOR.tools.callFunction(" + CKEditorFuncNum + ", \"" + url + "\", \"" + message + "\");</script></body></html>";
            return Content(output);
        }
    }
}
