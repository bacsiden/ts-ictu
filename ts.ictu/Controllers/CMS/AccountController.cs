using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Security.Principal;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Security;
using shop.cnc.Models;
using System.Data.SqlClient;
using System.Data;

namespace shop.cnc.Controllers
{
    public class UserDAL : DB.BaseClass<mUser>
    {
        public const string ADMIN = "Administrator";
        public List<string> GetListFunctionCodeByUsername(string username)
        {
            List<string> listCode = new List<string>();
            try
            {
                //var db = DB.Entities;                
                //var list = (from p in db.V_Function
                //            join
                //            q in db.V_FunctionInRole
                //            on p.ID equals q.FunctionID
                //            where q.V_Role.V_UserInRole.FirstOrDefault(m => m.V_User.UserName == username) != null
                //            select p).ToList();

                //if (list != null && list.Count() > 0)
                //{
                //    foreach (var item in list)
                //    {
                //        listCode.Add(item.Code);
                //    }
                //}
                var db = DB.Entities;
                var list = db.mFunction.Where(m => m.mRole.FirstOrDefault(n => n.mGroup.FirstOrDefault(x => x.mUser.FirstOrDefault(y => y.UserName == username) != null) != null) != null);
                if (list != null && list.Count() > 0)
                {
                    foreach (var item in list)
                    {
                        listCode.Add(item.Code);
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return listCode;
        }
        public List<int> GetMenuIDByUsername(string username)
        {
            List<int> listID = new List<int>();
            try
            {
                var db = DB.Entities;
                var list = db.mMenu.Where(n => n.mFunction.FirstOrDefault(x => x.mRole.FirstOrDefault(y => y.mGroup.FirstOrDefault(z => z.mUser.FirstOrDefault(z1 => z1.UserName == username) != null) != null) != null) != null).ToList();
                if (list != null && list.Count() > 0)
                {
                    foreach (var item in list)
                    {
                        listID.Add(item.ID);
                    }
                }
            }
            catch (Exception ex)
            {
                //throw ex;
            }

            return listID;
        }
        public mUser GetUserByUserName(string userName)
        {
            var context = DB.Entities;
            return context.mUser.FirstOrDefault(m => m.UserName.ToLower() == userName.ToLower());
        }
        /// <summary>
        /// Reset password (dành cho Admin muốn reset mật khẩu của member)
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="newPassword"></param>
        /// <returns></returns>
        public bool ChangePassword(string userName, string newPassword)
        {
            try
            {
                MembershipUser aspnetUser = Membership.GetUser(userName);
                string resetPassword = aspnetUser.ResetPassword();
                return aspnetUser.ChangePassword(resetPassword, newPassword);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        /// <summary>
        /// Đổi mật khẩu yêu cầu nhập vào mật khẩu cũ (dành cho member sau khi đăng nhập)
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="oldPassword"></param>
        /// <param name="newPassword"></param>
        /// <returns></returns>
        public bool ChangePassword(string userName, string oldPassword, string newPassword)
        {
            try
            {
                MembershipUser aspnetUser = Membership.GetUser(userName);
                return aspnetUser.ChangePassword(oldPassword, newPassword);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public Guid CreateAspnetUser(string username, string password)
        {
            try
            {
                MembershipUser aspnetUser = Membership.CreateUser(username, password);
                return (Guid)aspnetUser.ProviderUserKey;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public mUser GetUserByID(int id)
        {
            return GetByID(id);
        }
        public mUser GetCurrentUser
        {
            get
            {
                MembershipUser mbsUser = null;
                try
                {
                    mbsUser = Membership.GetUser();
                }
                catch { }
                if (mbsUser != null)
                {
                    Guid id = (Guid)mbsUser.ProviderUserKey;
                    return DB.Entities.mUser.FirstOrDefault(m => m.AspnetUserID == id);
                }
                return null;
            }
        }
        public void LockUserByID(int id)
        {
            var user = GetUserByID(id);
            if (UserDAL.ADMIN.Equals(user.UserName))
            {
                return;
            }
            user.Locked = true;
            Update(user);
        }
    }

    public class AccountController : BaseController
    {

        public IFormsAuthenticationService FormsService { get; set; }
        public IMembershipService MembershipService { get; set; }

        protected override void Initialize(RequestContext requestContext)
        {
            if (FormsService == null) { FormsService = new FormsAuthenticationService(); }
            if (MembershipService == null) { MembershipService = new AccountMembershipService(); }

            base.Initialize(requestContext);
        }

        // **************************************
        // URL: /Account/LogOn
        // **************************************


        [Authorize]
        [ValidationFunction(ActionName.SystemAdmin)]
        public ActionResult Index(int id = 0)
        {
            try
            {
                if (id == 0)
                {
                    @ViewBag.GroupName = "List User";
                    return View(DB.Entities.mUser);
                }
                else
                {
                    var db = DB.Entities;
                    var g = db.mGroup.First(m => m.ID == id);
                    //@ViewBag.GroupName = "Tên nhóm: <a href='" + HttpContext.Request.Url + "'>" + g.Title + "</a>";
                    @ViewBag.GroupName = g.Title;
                    return View(db.mUser.Where(m => m.mGroup.FirstOrDefault(x => x.ID == id) != null));
                }
            }
            catch (Exception ex)
            {
                return View(new List<mUser>());
            }
        }

        [Authorize]
        [ValidationFunction(ActionName.SystemAdmin)]
        public ActionResult NewOrEdit(int? id = 0)
        {
            var obj = DB.Entities.mUser.FirstOrDefault(m => m.ID == id);
            ViewBag.Title = "Edit user";
            if (obj == null)
            {
                ViewBag.Title = "Add user, default password = 1";
                obj = new mUser();
            }
            return View(obj);
        }

        [HttpPost]
        [Authorize]
        [ValidationFunction(ActionName.SystemAdmin)]
        public ActionResult NewOrEdit(mUser model)
        {
            try
            {
                var db = DB.Entities;
                if (model.ID == 0)
                {
                    // Add new   
                    var aspNewUserID = new UserDAL().CreateAspnetUser(model.UserName, "1");
                    model.AspnetUserID = aspNewUserID;
                    db.mUser.AddObject(model);
                }
                else
                {
                    // Edit    
                    db.AttachTo("mUser", model);
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

        [Authorize]
        [ValidationFunction(ActionName.SystemAdmin)]
        public ActionResult DeleteByListID(string arrayID = "")
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
                        var obj = db.mUser.FirstOrDefault(m => m.ID == tmpID);
                        if (UserDAL.ADMIN.Equals(obj.UserName))
                        {
                            continue;
                        }
                        new UserDAL().Delete(tmpID);
                    }
                    db.SaveChanges();
                }
            }
            catch
            {

            }
            return RedirectToAction("Index");
        }

        [Authorize]
        [ValidationFunction(ActionName.SystemAdmin)]
        public ActionResult LockByListID(string arrayID = "")
        {
            try
            {
                // TODO: Add delete logic here
                var lstID = arrayID.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                var db = DB.Entities;
                var userDAL = new UserDAL();
                if (lstID.Length > 0)
                {
                    foreach (var item in lstID)
                    {
                        // Thực hiện khóa tài khoản người dùng
                        userDAL.LockUserByID(int.Parse(item));
                    }
                }
            }
            catch
            {

            }
            return RedirectToAction("Index");
        }

        public ActionResult LogOn(string hidefilter)
        {
            if (string.IsNullOrEmpty(hidefilter))
                Response.Redirect("/Account/LogOn/?hidefilter=1");
            return View();
        }

        [HttpPost]
        public ActionResult LogOn(LogOnModel model, string ReturnUrl)
        {
            if (ModelState.IsValid)
            {
                var user = new UserDAL().GetUserByUserName(model.UserName);
                if (user != null && !user.Locked)
                {
                    if (MembershipService.ValidateUser(model.UserName, model.Password))
                    {
                        FormsService.SignIn(model.UserName, model.RememberMe);
                        ListFunctionCode = new UserDAL().GetListFunctionCodeByUsername(model.UserName);
                        ReturnUrl = Request.QueryString["ReturnUrl"];
                        if (Url.IsLocalUrl(ReturnUrl))
                        {
                            return Redirect(ReturnUrl);
                        }
                        else
                        {
                            return RedirectToAction("AdminIndex", "Post");
                        }
                    }
                    else
                    {
                        ModelState.AddModelError("", "Sai tên hoặc mật khẩu. ");
                    }
                }
                else
                {
                    ModelState.AddModelError("", "Sai tên hoặc mật khẩu. ");
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        // **************************************
        // URL: /Account/LogOff
        // **************************************

        [Authorize]
        public ActionResult LogOff()
        {
            FormsService.SignOut();

            return RedirectToAction("Index", "Home");
        }

        // **************************************
        // URL: /Account/Register
        // **************************************

        [Authorize]
        public ActionResult Register()
        {
            ViewBag.PasswordLength = MembershipService.MinPasswordLength;
            // DropDown Bưu cục
            var db = DB.Entities;
            return View();
        }

        [HttpPost]
        [Authorize]
        public ActionResult Register(RegisterModel model)
        {
            if (ModelState.IsValid)
            {
                var db = DB.Entities;
                // Attempt to register the user
                if (db.mUser.FirstOrDefault(m => m.UserName.Equals(model.UserName)) == null)
                {


                    MembershipUser aspnetUser = Membership.CreateUser(model.UserName, model.Password, model.Email);
                    Guid userCreated = (Guid)aspnetUser.ProviderUserKey;
                    if (userCreated != null)
                    {

                        db.mUser.AddObject(new mUser() { UserName = model.UserName, Email = model.Email, PhoneNumber = model.Phone, Address = model.Address, AspnetUserID = userCreated, Name = model.Name });
                        db.SaveChanges();
                        FormsService.SignIn(model.UserName, false /* createPersistentCookie */);
                        return RedirectToAction("AdminIndex", "Post");
                    }
                    else
                    {
                        ModelState.AddModelError("", "Register unsuccesfully!");
                    }
                }
                else
                {
                    ModelState.AddModelError("", "Tài khoản đã tồn tại!");
                }
            }

            // If we got this far, something failed, redisplay form
            ViewBag.PasswordLength = MembershipService.MinPasswordLength;
            return View(model);
        }

        // **************************************
        // URL: /Account/ChangePassword
        // **************************************

        //[Authorize]
        //public ActionResult ChangePassword()
        //{
        //    ViewBag.PasswordLength = MembershipService.MinPasswordLength;
        //    return View();
        //}

        //[Authorize]
        //[HttpPost]
        //public ActionResult ChangePassword(ChangePasswordModel model)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        if (MembershipService.ChangePassword(User.Identity.Name, model.OldPassword, model.NewPassword))
        //        {
        //            return RedirectToAction("ChangePasswordSuccess");
        //        }
        //        else
        //        {
        //            ModelState.AddModelError("", "The current password is incorrect or the new password is invalid.");
        //        }
        //    }

        //    // If we got this far, something failed, redisplay form
        //    ViewBag.PasswordLength = MembershipService.MinPasswordLength;
        //    return View(model);
        //}

        [Authorize]
        public ActionResult ChangePassword()
        {
            return PartialView("_ChangePass");
        }

        [Authorize]
        [HttpPost]
        public ActionResult ChangePassword(Models.ChangePasswordModel model)
        {
            if (ModelState.IsValid)
            {

                try
                {
                    var userDAL = new UserDAL();
                    bool succeeded = userDAL.ChangePassword(CurrentUser.UserName, model.OldPassword, model.NewPassword);
                    if (succeeded)
                    {
                        return JavaScript(@"noticeChangePassWord(true, 'Change password succesfully.');");
                    }
                    else
                    {
                        ModelState.AddModelError("OldPassword", "OldPassword is invailid");
                    }
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("NewPassword", "Change password error");
                }

            }
            // If we got this far, something failed, redisplay form
            return PartialView("_ChangePass", model);
        }

        // **************************************
        // URL: /Account/ChangePasswordSuccess
        // **************************************

        [Authorize]
        [ValidationFunction(ActionName.SystemAdmin)]
        public ActionResult SetPassword(string username)
        {
            return View("SetPass");
        }

        [Authorize]
        [ValidationFunction(ActionName.SystemAdmin)]
        [HttpPost]
        public ActionResult SetPassword(Models.ChangePasswordModel model, string username)
        {
            if (model.NewPassword == model.ConfirmPassword)
            {

                try
                {
                    var userDAL = new UserDAL();
                    bool succeeded = userDAL.ChangePassword(username, model.NewPassword);
                    if (succeeded)
                    {
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        ModelState.AddModelError("NewPassword", "NewPassword is invailid");
                    }
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("NewPassword", "Set password error");
                }

            }
            // If we got this far, something failed, redisplay form
            return RedirectToAction("SetPassword", new { username = username });
        }

        [Authorize]
        public ActionResult ChangePasswordSuccess()
        {
            return View();
        }

        [Authorize]
        public ActionResult AccessDenied()
        {
            return View();
        }

        public static string temp1 = "<li class='{3}'><a href=\"{1}\" class='menu-item-a {5}'>{2}<span>&nbsp;{0}</span>{4} </a>{6}</li>";
        public static string temp2 = "<ul class=\"submenu\">{0}</ul>";
        public static string BuildMenu()
        {
            var user = new UserDAL().GetCurrentUser;
            string s = "";
            string cls = "dropdown-toggle";
            string icondrop = "<b class=\"arrow icon-angle-right\"></b>";
            var lst = user.UserName == UserDAL.ADMIN ? DB.Entities.mMenu.OrderBy(m => m.Oder) :
                DB.Entities.mMenu.Where(x => x.IsActive.Value).OrderBy(m => m.Oder);
            foreach (var item in lst)
            {
                if (item.ParentID == null || item.ParentID.Value == 0)
                {
                    string tmp = "";
                    if (item.IsActive != true || !(new AccountController().IsMenuInGroup(item.ID)))
                    {
                        tmp = "hiddenField";
                    }
                    var listChild = lst.Where(m => m.ParentID == item.ID).OrderBy(m => m.Oder).ToList();
                    if (listChild.Count > 0)
                    {
                        string subLI = "";
                        foreach (var itemSub in listChild)
                        {
                            subLI += string.Format("<li><a href=\"{1}\" class='menu-item-a'>{0}</a></li>", itemSub.Title, itemSub.Url);
                        }
                        string subMenu = string.Format(temp2, subLI);
                        s += string.Format(temp1, item.Title, "#", item.Icon, tmp, icondrop, cls, subMenu);
                    }
                    else
                    {
                        s += string.Format(temp1, item.Title, item.Url, item.Icon, tmp, "", "", "");
                    }

                }
            }
            return s;
        }
        public bool IsMenuInGroup(int id)
        {
            var menuIDList = new List<int>();
            if (System.Web.HttpContext.Current.Session["ListMenuID"] != null)
            {
                menuIDList = (List<int>)System.Web.HttpContext.Current.Session["ListMenuID"];
            }
            else
            {
                menuIDList = new UserDAL().GetMenuIDByUsername(System.Web.HttpContext.Current.User.Identity.Name);
            }

            if (menuIDList != null)
            {
                if (menuIDList.Contains(id))
                {
                    return true;
                }
            }
            return false;
        }
    }
}
