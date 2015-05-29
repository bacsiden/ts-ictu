using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ts.ictu
{
    public class Constant
    {
        public const string ROLES = "ROLES";
        public const string CURRENT_USER = "CurrentUser";
        public const string CACHE_CURRENT_LANGUAGE = "CACHE_CURRENT_LANGUAGE";
        public const string SESSION_CURRENT_LANGUAGE_CODE = "CURRENT_LANGUAGE_CODE";
        public const string V_FILE_SERVER = "{V_FILE_SERVER}";
        public const string TEMP_FOLDER = "App_Data";

        public const string SESSION_CART = "SESSION_CART";
        public const string SESSION_CART_COUNT = "SESSION_CART_COUNT";
        public static string SESSION_PAYMENT_SUCCESS = "SESSION_PAYMENT_SUCCESS";
        public static string SESSION_BORROW_SUCCESS = "SESSION_BORROW_SUCCESS";
        public static string SESSION_PAYMENT_CONFIRM = "SESSION_PAYMENT_CONFIRM";
        public static string SESSION_TRANSACTION_ID = "TRANSACTION_ID";

        public const string PREFIX = "FBx1x2";

        public static string SESSION_CAPTCHA = "SESSION_CAPTCHA";
    }
    public class CateCode
    {
        public const string TTTS = "TTTS";
        public const string DaoTao = "DaoTao";
    }
    public class MenuCode
    {
        public const string DaoTao = "DaoTao";
    }
    public enum HinhThuc
    {
        CanBan = 1,
        CanMua = 2,
        ChoThue = 3
    }
    public enum PostStatus
    {
        Init = 1,
        Enabled = 2,
        Disabled = 3
    }
}