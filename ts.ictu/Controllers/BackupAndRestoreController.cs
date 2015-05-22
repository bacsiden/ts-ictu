using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;
using System.Web.Configuration;
using System.IO;
using NationalIT.Models;
using System.Data.SqlClient;
using System.Configuration;

namespace NationalIT.Controllers
{
    public class BackupAndRestoreController : BaseController
    {
        //
        // GET: /BackupAndRestore/
        [Authorize]
        [ValidationFunction(ActionName.BACKUPDATABASE, ActionName.RESTOREDATABASE)]
        public ActionResult Index()
        {
            try
            {
                string FilePathXML = HttpContext.Server.MapPath("~/App_Data/config.xml");
                var doc = XDocument.Load(FilePathXML);
                bool isAdmin = CurrentUser.UserName.Equals(UserDAL.ADMIN);
                ViewBag.ShowBackupMenu = IsFunctionInRole(ActionName.BACKUPDATABASE.ToString()) || isAdmin;
                ViewBag.ShowDeleteMenu = ViewBag.ShowRestoreMenu = IsFunctionInRole(ActionName.RESTOREDATABASE.ToString()) || isAdmin;
                string folderName = doc.Element("system.config").Element("path-backup").Value;
                string[] files = Directory.GetFiles(folderName);
                var list = new List<BackupModel>();
                DirectoryInfo di = new DirectoryInfo(folderName);
                foreach (FileInfo item in di.GetFiles())
                {
                    BackupModel tmp = new BackupModel()
                    {
                        Name = item.Name,
                        Date = item.LastWriteTime.ToShortDateString(),
                    };
                    list.Add(tmp);
                }
                list = list.OrderByDescending(m => m.Date).ToList();
                if (Request.IsAjaxRequest())
                {
                    return PartialView("_RestorePartial", list);
                }
                return View(list);
            }
            catch { return View(); }
        }

        [Authorize]
        [ValidationFunction(ActionName.BACKUPDATABASE)]
        public ActionResult Backup()
        {
            DateTime now = DateTime.Now;
            string NameBak = "Backup_" + DateTime.Now.ToString("yyyy-MM-dd") + "_" + now.Hour + "-" + now.Minute + "-" + now.Second + ".bak";
            return View(new BackupModel { Name = NameBak });
        }

        [HttpPost]
        [Authorize]
        [ValidationFunction(ActionName.BACKUPDATABASE)]
        public ActionResult Backup(BackupModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if (model.Name.Contains("\\"))
                    {
                        ModelState.AddModelError("Name", "File name must not contain one of the following characters /\\:*?\"<>|");
                        return PartialView(model);
                    }
                    // Đường dẫn tới file xml.
                    string path = System.Web.Hosting.HostingEnvironment.ApplicationPhysicalPath + "/App_Data/config.xml";
                    // Tạo một đối tượng TextReader mới
                    var xtr = System.Xml.Linq.XDocument.Load(path);
                    string backupFolder = string.Format(xtr.Element("system.config").Element("path-backup").Value.Trim());   //Vuongj
                    // tạo folder backup neu chua co
                    if (!System.IO.Directory.Exists(backupFolder))
                    {
                        System.IO.Directory.CreateDirectory(backupFolder);
                    }

                    string fileName = model.Name;
                    if (fileName.IndexOf(".bak") <= 0)
                    {
                        fileName += ".bak";
                    }

                    var filePath = backupFolder;
                    if (filePath.LastIndexOf("\\") == filePath.Length - 1)
                    {
                        filePath += fileName;
                    }
                    else
                    {
                        filePath += "\\" + fileName;
                    }

                    String conStr = ConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString;
                    SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder(conStr);
                    string databaseName = builder.InitialCatalog;

                    string query = string.Format(@"BACKUP DATABASE {0} TO DISK = '{1}' WITH INIT", databaseName, filePath);
                    int result = DB.Entities.ExecuteStoreCommand(query);
                    if (result != 0)
                    {
                        //add to Data.xml
                        var backupObj = new Models.BackupModel();
                        backupObj.Name = model.Name;
                        backupObj.FilePath = filePath;
                        backupObj.Date = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                        return RedirectToAction("Index");
                    }
                }
                catch (Exception ex)
                {
                    if (ex.Message.Contains("Cannot open backup device"))
                    {
                        // Đường dẫn tới file xml.
                        string path = System.Web.Hosting.HostingEnvironment.ApplicationPhysicalPath + "/App_Data/config.xml";
                        // Tạo một đối tượng TextReader mới
                        var xtr = System.Xml.Linq.XDocument.Load(path);
                        string backupFolder = string.Format(xtr.Element("system.config").Element("path-backup").Value.Trim());
                        ModelState.AddModelError("", "<p class='field-validation-error'><b>Backup database unsuccessfully.</b></p><p class='field-validation-error'>Folder backup \"" + backupFolder + "\" not found. <br />Please check again system config!</p>");
                        return View(model);
                    }
                    ModelState.AddModelError("", "<p class='field-validation-error'><b>Backup database unsuccessfully.</b></p><p class='field-validation-error'>Please try again in a few minutes</p>");
                }
            }
            return View(model);
        }

        [Authorize]
        [ValidationFunction(ActionName.RESTOREDATABASE)]
        public ActionResult Restore()
        {
            string FilePathXML = HttpContext.Server.MapPath("~/App_Data/Data.xml");
            var doc = XDocument.Load(FilePathXML);

            return View(doc.Descendants("Note").OrderByDescending(m => (int)m.Element("ID")).ToList());
        }

        [HttpGet]
        [Authorize]
        [ValidationFunction(ActionName.RESTOREDATABASE)]
        public ActionResult RestoreByID(string name)
        {

            string pathConfigXML = System.Web.Hosting.HostingEnvironment.ApplicationPhysicalPath + "/App_Data/config.xml";
            // Tạo một đối tượng TextReader mới
            var xtr = System.Xml.Linq.XDocument.Load(pathConfigXML);
            string backupFolder = string.Format(xtr.Element("system.config").Element("path-backup").Value.Trim());
            var dbBackUp = DB.Entities;
            string pathFull = "";
            if ((backupFolder.LastIndexOf("\\") != backupFolder.Length - 1))
            {
                pathFull = backupFolder + "\\" + name;
            }
            else
            {
                pathFull = backupFolder + name;
            }
            if (System.IO.File.Exists(pathFull))
            {
                try
                {
                    string queryCheck = "DECLARE @result INT EXEC master.dbo.xp_fileexist '" + pathFull + "', @result OUTPUT  Select @result";
                    int isExists = dbBackUp.ExecuteStoreQuery<int>(queryCheck, null).FirstOrDefault();
                    if (isExists != 0)
                    {
                        string queryStore = "use master ALTER DATABASE DB_9B22F2_nationalit SET SINGLE_USER With ROLLBACK IMMEDIATE;";
                        queryStore += "RESTORE DATABASE DB_9B22F2_nationalit FROM disk = '" + pathFull + "' WITH REPLACE;";
                        queryStore += "use master ALTER DATABASE DB_9B22F2_nationalit SET MULTI_USER;";
                        queryStore += "use master ALTER DATABASE DB_9B22F2_nationalit SET ONLINE";

                        int result = dbBackUp.ExecuteStoreCommand(queryStore);
                        if (Convert.ToInt32(result) != 0)
                        {
                            ViewBag.Success = "1";
                            return JavaScript(@"alert('Restore database successfully.');");
                        }
                    }
                }
                catch (Exception ex)
                {

                }
            }
            string message = "$('#ModalGeneral').empty().html(\"<div class='modal-header'><a class='close' data-dismiss='modal'>x</a><h3>Error message</h3></div><div class='modal-body'><p class='field-validation-error'><b>Restore database unsuccessfully!</b> <br /> - Please check folder restore in system config: &#34;" + pathFull.Replace("\\", "&#92;") + "&#34;.<br />- Or your user sql server have not permisson restore.</p></div>\").modal('show');";
            return JavaScript(message);
        }


        [Authorize]
        [ValidationFunction(ActionName.RESTOREDATABASE)]
        public ActionResult DeleteBackupDatabaseByID(string name)
        {
            bool success = true;
            string message = "";
            try
            {

                string pathConfigXML = System.Web.Hosting.HostingEnvironment.ApplicationPhysicalPath + "/App_Data/config.xml";
                // Tạo một đối tượng TextReader mới
                var xtr = System.Xml.Linq.XDocument.Load(pathConfigXML);
                string backupFolder = string.Format(xtr.Element("system.config").Element("path-backup").Value.Trim());
                string pathFull = "";
                if ((backupFolder.LastIndexOf("\\") != backupFolder.Length - 1))
                {
                    pathFull = backupFolder + "\\" + name;
                }
                else
                {
                    pathFull = backupFolder + name;
                }

                if (System.IO.File.Exists(pathFull))
                {
                    System.IO.File.Delete(pathFull);
                }
                success = true;
                message = "Delete successfully";
            }
            catch (System.IO.IOException e)
            {
                success = false;
                message = "Error system. File not found or file restore is failed. Please try again";
            }
            catch (Exception ex)
            {
                success = false;
                message = "Error system. Please try again a few minutes.";
            }
            return Json(new { IsSuccess = success, Message = message }, JsonRequestBehavior.AllowGet);
        }

        [Authorize]
        [ValidationFunction(ActionName.RESTOREDATABASE)]
        public ActionResult DownloadFile(string name)
        {
            try
            {
                if (!string.IsNullOrEmpty(name))
                {
                    string pathConfigXML = System.Web.Hosting.HostingEnvironment.ApplicationPhysicalPath + "/App_Data/config.xml";
                    // Tạo một đối tượng TextReader mới
                    var xtr = System.Xml.Linq.XDocument.Load(pathConfigXML);
                    string backupFolder = string.Format(xtr.Element("system.config").Element("path-backup").Value.Trim());
                    string pathFull = "";
                    if ((backupFolder.LastIndexOf("\\") != backupFolder.Length - 1))
                    {
                        pathFull = backupFolder + "\\" + name;
                    }
                    else
                    {
                        pathFull = backupFolder + name;
                    }

                    if (System.IO.File.Exists(pathFull))
                    {
                        //lấy loại file
                        FileStream fs = new FileStream(pathFull, FileMode.Open);
                        string contentType = "application/" + Path.GetExtension(pathFull).Substring(1);
                        return File(fs, contentType, name);
                    }
                }
            }
            catch (Exception ex)
            {
                return RedirectToAction("FileNull", "PageError");
            }
            return null;
        }

        [Authorize]
        [ValidationFunction(ActionName.RESTOREDATABASE)]
        public ActionResult Upload()
        {
            return View();
        }
        [Authorize]
        [ValidationFunction(ActionName.RESTOREDATABASE)]
        [HttpPost]
        public ActionResult Upload(HttpPostedFileBase file)
        {
            try
            {

                if (file != null)
                {
                    string pathConfigXML = System.Web.Hosting.HostingEnvironment.ApplicationPhysicalPath + "/App_Data/config.xml";
                    // Tạo một đối tượng TextReader mới
                    var xtr = System.Xml.Linq.XDocument.Load(pathConfigXML);
                    string backupFolder = string.Format(xtr.Element("system.config").Element("path-backup").Value.Trim());
                    string fileName = backupFolder + file.FileName;
                    if (System.IO.File.Exists(fileName))
                    {
                        System.IO.File.Delete(fileName);
                    }
                    file.SaveAs(fileName);
                    return RedirectToAction("Index");
                }

            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
            }
            return View();
        }

        #region Online Server
        public static string onlineDBFolder = "/db/";
        public List<BackupModel> ListRestoreOnline()
        {
            try
            {
                var ftpFiles = FTPUtilities.GetFiles("/db");
                var lst = new List<BackupModel>();
                foreach (var item in ftpFiles)
                {
                    var obj = new BackupModel();
                    obj.Name = item.Name;
                    if (item.LastWriteTime != null)
                        obj.Date = item.LastWriteTime.Value.ToShortDateString();
                    lst.Add(obj);
                }
                return lst;
            }
            catch { return null; }

        }

        [Authorize]
        [ValidationFunction(ActionName.RESTOREDATABASE)]
        public ActionResult DeleteBackupFileOnline(string name)
        {
            bool success = true;
            string message = "";
            try
            {
                FTPUtilities.DeleteFile(onlineDBFolder + name);
                success = true;
                message = "Delete successfully";
            }
            catch (System.IO.IOException e)
            {
                success = false;
                message = e.Message + ". Error system. File not found or file restore is failed. Please try again";
            }
            catch (Exception ex)
            {
                success = false;
                message = ex.Message + ". Error system. Please try again a few minutes.";
            }
            return Json(new { IsSuccess = success, Message = message }, JsonRequestBehavior.AllowGet);
        }

        [Authorize]
        [ValidationFunction(ActionName.RESTOREDATABASE)]
        public ActionResult DownloadOnlineFile(string name)
        {
            try
            {
                //System.Net.FtpWebRequest.Create("ftp.Smarterasp.net/db/nationalit.bak");
                byte[] fs = FTPUtilities.GetByteArray(onlineDBFolder + name);
                string contentType = "application/" + Path.GetExtension(name).Substring(1);
                return File(fs, contentType, name);
            }
            catch (Exception ex)
            {
                return RedirectToAction("FileNull", "PageError");
            }
        }

        [Authorize]
        [ValidationFunction(ActionName.RESTOREDATABASE)]
        public ActionResult PushToLocalServer(string name)
        {
            try
            {
                string FilePathXML = HttpContext.Server.MapPath("~/App_Data/config.xml");
                var doc = XDocument.Load(FilePathXML);
                string folderName = doc.Element("system.config").Element("path-backup").Value;
                if (folderName.Trim().Last() != '\\')
                    folderName += '\\';
                string filename = folderName + name;
                byte[] fs = FTPUtilities.GetByteArray(onlineDBFolder + name);
                FileStream w = new FileStream(filename, FileMode.Create);
                w.Write(fs, 0, fs.Length);
                w.Flush();
                w.Dispose();
            }
            catch { }
            return RedirectToAction("Index");
        }

        [Authorize]
        [ValidationFunction(ActionName.RESTOREDATABASE)]
        public ActionResult PushToOnlineServer(string name)
        {
            try
            {
                string FilePathXML = HttpContext.Server.MapPath("~/App_Data/config.xml");
                var doc = XDocument.Load(FilePathXML);
                string folderName = doc.Element("system.config").Element("path-backup").Value;
                if (folderName.Trim().Last() != '\\')
                    folderName += '\\';
                string filename = folderName + name;

                StreamReader r = new StreamReader(filename);
                FTPUtilities.CreateFile(onlineDBFolder + name, r.BaseStream);
            }
            catch { }
            return RedirectToAction("Index");
        }


        [Authorize]
        [ValidationFunction(ActionName.RESTOREDATABASE)]
        public ActionResult UploadOnline()
        {
            return View();
        }

        [HttpPost]
        public ActionResult UploadOnline(HttpPostedFileBase file)
        {
            try
            {
                if (file != null)
                {
                    FTPUtilities.CreateFile(onlineDBFolder
                        + file.FileName, file.InputStream);
                    return RedirectToAction("Index");
                }

            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
            }
            return View();
        }

        #endregion
    }
}
