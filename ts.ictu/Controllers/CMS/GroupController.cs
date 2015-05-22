using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ts.ictu.Controllers
{
    public class GroupController : BaseController
    {
        [Authorize]
        [ValidationFunction(ActionName.SystemAdmin)]
        public ActionResult Index()
        {
            return View(DB.Entities.mGroup);
        }

        [Authorize]
        [ValidationFunction(ActionName.SystemAdmin)]
        public ActionResult UsersInGroup(int id = 0)
        {
            var db = DB.Entities;
            var g = db.mGroup.First(m => m.ID == id);
            if (g == null)
                return RedirectToAction("Index");

            ViewBag.GoupName = g.Title;
            ViewBag.GroupID = id;
            return View(DB.Entities.mUser.Where(m => m.mGroup.FirstOrDefault(x => x.ID == g.ID) != null));
        }

        [Authorize]
        [ValidationFunction(ActionName.SystemAdmin)]
        public ActionResult RemoveUser(int id, int groupID)
        {
            var db = DB.Entities;
            var user = db.mUser.First(m => m.ID == id);
            var group = db.mGroup.First(m => m.ID == groupID);
            user.mGroup.Remove(group);
            db.SaveChanges();
            return RedirectToAction("UsersInGroup", new { id = groupID });
        }

        [Authorize]
        [ValidationFunction(ActionName.SystemAdmin)]
        public ActionResult AddUser(int id)
        {
            var db = DB.Entities;
            ViewBag.GroupID = id;
            var obj = db.mGroup.First(m => m.ID == id);
            ViewBag.GroupName = obj.Title;
            #region SELECT OPTION
            List<int> listUserIDinGroup = obj.mUser.Select(m => m.ID).ToList();
            var listUser = db.mUser.Where(m => !listUserIDinGroup.Contains(m.ID)).ToList();
            string dataUserName = "<option value=''>--Select User name--</option>";
            foreach (var item in listUser)
            {
                dataUserName += string.Format("<option value='{0}'>{1} ({2})</option>", item.ID, item.Name,item.UserName);
            }
            ViewBag.dataUserName = dataUserName;
            #endregion
            return View(new mUser());
        }

        [Authorize]
        [ValidationFunction(ActionName.SystemAdmin)]
        public ActionResult DoAddUser(mUser objUser, int groupID)
        {
            try
            {
                var db = DB.Entities;
                var user = db.mUser.FirstOrDefault(m => m.ID == objUser.ID);
                var group = db.mGroup.First(m => m.ID == groupID);
                user.mGroup.Add(group);
                db.SaveChanges();
                return RedirectToAction("UsersInGroup", new { id = groupID });
            }
            catch (Exception ex)
            {
                #region SELECT OPTION
                string dataUserName = "<option value=''>--Select User name--</option>";
                foreach (var item in ts.ictu.DB.Entities.mUser)
                {
                    dataUserName += string.Format("<option value='{0}'>{1} ({2})</option>", item.ID, item.Name, item.UserName);
                }
                ViewBag.dataUserName = dataUserName;
                #endregion
                ModelState.AddModelError("", "Can not add this user to group.");
                return View("AddUser", objUser);
            }
        }

        [Authorize]
        public ActionResult Details(int id = 0)
        {
            var obj = DB.Entities.mGroup.First(m => m.ID == id);
            if (obj == null)
                return RedirectToAction("Index");
            return View(obj);
        }

        [Authorize]
        [ValidationFunction(ActionName.SystemAdmin)]
        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [Authorize]
        [ValidationFunction(ActionName.SystemAdmin)]
        public ActionResult Create(mGroup model)
        {
            try
            {
                // TODO: Add insert logic here
                var db = DB.Entities;
                var group = new mGroup() { Title = model.Title };
                db.mGroup.AddObject(group);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        [Authorize]
        [ValidationFunction(ActionName.SystemAdmin)]
        public ActionResult Edit(int id = 0)
        {
            var obj = DB.Entities.mGroup.First(m => m.ID == id);
            if (obj == null)
                return RedirectToAction("Index");
            return View(obj);
        }

        [HttpPost]
        [Authorize]
        [ValidationFunction(ActionName.SystemAdmin)]
        public ActionResult Edit(mGroup model)
        {
            try
            {
                // TODO: Add update logic here
                var db = DB.Entities;
                var obj = db.mGroup.First(m => m.ID == model.ID);
                obj.Title = model.Title;
                db.ObjectStateManager.ChangeObjectState(obj, System.Data.EntityState.Modified);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        [Authorize]
        [ValidationFunction(ActionName.SystemAdmin)]
        public ActionResult Delete(int id)
        {
            try
            {
                var db = DB.Entities;
                var obj = db.mGroup.First(m => m.ID == id);
                db.mGroup.DeleteObject(obj);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                return RedirectToAction("Index");
            }
        }

        [Authorize]
        [ValidationFunction(ActionName.SystemAdmin)]
        public ActionResult Role(int id)
        {
            try
            {
                var db = DB.Entities;
                var lst = db.mRole.Where(m => m.mGroup.FirstOrDefault(n => n.ID == id) != null);
                string s = "";
                foreach (var item in db.mRole)
                {
                    string check = "";
                    if (lst.FirstOrDefault(m => m.ID == item.ID) != null)
                    {
                        check = "checked='checked'";
                    }
                    s += "<label class='checkbox'><input type='checkbox' " + check + " value='" + item.ID + "' />" + item.Name + "</label>";
                }
                ViewBag.listRole = s;
                return View(db.mGroup.FirstOrDefault(m => m.ID == id));

            }
            catch (Exception)
            {
                return View();
            }
        }

        [Authorize]
        [ValidationFunction(ActionName.SystemAdmin)]
        public ActionResult DoRole(int groupID, string listCheck)
        {
            try
            {
                var db = DB.Entities;
                var group = db.mGroup.FirstOrDefault(m => m.ID == groupID);
                string[] listChecked = listCheck.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                foreach (var item in db.mRole)
                {
                    if (listChecked.Contains(item.ID.ToString()))
                    {
                        if (group.mRole.FirstOrDefault(m => m.ID == item.ID) == null)
                        {
                            group.mRole.Add(item);
                        }
                    }
                    else
                        if (group.mRole.FirstOrDefault(m => m.ID == item.ID) != null)
                        {
                            group.mRole.Remove(item);
                        }
                }

                db.SaveChanges();
                return RedirectToAction("index");
            }
            catch (Exception)
            {
                return View();
            }
        }
    }
}
