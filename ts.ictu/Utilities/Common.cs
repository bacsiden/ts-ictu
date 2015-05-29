using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Globalization;
using System.FtpClient;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;

namespace ts.ictu
{
    public class Common
    {
        public static HttpContext HttpContext
        {
            get { return HttpContext.Current; }
        }
        public static string RouteData(string key)
        {
            var obj=  HttpContext.Current.Request.RequestContext.RouteData.Values[key];
            return obj == null ? null : obj.ToString();
        }
        private static readonly string[] VietnameseSigns = new string[]
        {
                        "aAeEoOuUiIdDyY",                        
                        "áàạảãâấầậẩẫăắằặẳẵ",                        
                        "ÁÀẠẢÃÂẤẦẬẨẪĂẮẰẶẲẴ",                        
                        "éèẹẻẽêếềệểễ",                        
                        "ÉÈẸẺẼÊẾỀỆỂỄ",                        
                        "óòọỏõôốồộổỗơớờợởỡ",                        
                        "ÓÒỌỎÕÔỐỒỘỔỖƠỚỜỢỞỠ",                        
                        "úùụủũưứừựửữ",                        
                        "ÚÙỤỦŨƯỨỪỰỬỮ",                        
                        "íìịỉĩ",                        
                        "ÍÌỊỈĨ",                        
                        "đ",                        
                        "Đ",                        
                        "ýỳỵỷỹ",                        
                        "ÝỲỴỶỸ"
        };

        public static string RemoveSign4Vietnamese(string str)
        {

            //Tiến hành thay thế , lọc bỏ dấu cho chuỗi

            for (int i = 1; i < VietnameseSigns.Length; i++)
            {

                for (int j = 0; j < VietnameseSigns[i].Length; j++)

                    str = str.Replace(VietnameseSigns[i][j], VietnameseSigns[0][i - 1]);

            }

            return str;
        }

        public static string RemoveSymbol(string str)
        {
            //Giữ 1 ký tự trắng
            str = string.Join(" ", str.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries));

            //Loại trừ các ký tự đặc biệt - ngoài các ký tự sau
            string notSymbol =
                        " -[]abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789" +
                        "áàạảãâấầậẩẫăắằặẳẵ" +
                        "ÁÀẠẢÃÂẤẦẬẨẪĂẮẰẶẲẴ" +
                        "éèẹẻẽêếềệểễ" +
                        "ÉÈẸẺẼÊẾỀỆỂỄ" +
                        "óòọỏõôốồộổỗơớờợởỡ" +
                        "ÓÒỌỎÕÔỐỒỘỔỖƠỚỜỢỞỠ" +
                        "úùụủũưứừựửữ" +
                        "ÚÙỤỦŨƯỨỪỰỬỮ" +
                        "íìịỉĩ" +
                        "ÍÌỊỈĨ" +
                        "đ" +
                        "Đ" +
                        "ýỳỵỷỹ" +
                        "ÝỲỴỶỸ";

            for (int i = 0; i < str.Length; i++)
            {
                if (notSymbol.IndexOf(str[i]) == -1)
                {
                    str = str.Remove(i, 1);
                    i--;
                }
            }
            return str;
        }

        public static string CreateURLParam(int id, string value)
        {
            return string.Format("{0}-{1}", RemoveSymbol(RemoveSign4Vietnamese(value)).Replace(" ", "-").Replace("--", "-"), id);
        }
        public static string CreateURLParam(string value)
        {
            return RemoveSymbol(RemoveSign4Vietnamese(value)).Replace(" ", "-").Replace("--", "-");
        }
        public static int GetIDFromURLParam(string param)
        {
            //Trả về 0 nếu không lọc được ID. Khi gọi, kiểm tra != 0 thì thực hiện tiếp
            int id = 0;
            if (!string.IsNullOrEmpty(param))
            {
                int startIndex = param.LastIndexOf('-');
                int endIndex = param.Length;

                if (startIndex < endIndex)
                {
                    int.TryParse(param.Substring(startIndex + 1, endIndex - startIndex - 1), out id);
                }
            }
            return id;
        }
        public static List<mLanguage> GetListLanguage()
        {

            if (HttpContext.Current.Cache[Constant.CACHE_CURRENT_LANGUAGE] == null)
            {
                List<mLanguage> lst = DB.Entities.mLanguage.ToList();
                HttpContext.Current.Cache[Constant.CACHE_CURRENT_LANGUAGE] = lst;
                return lst;
            }
            return (List<mLanguage>)HttpContext.Current.Cache[Constant.CACHE_CURRENT_LANGUAGE];
        }
        public static string getYesNO(bool value)
        {
            if (value)
            {
                return "YES";
            }
            return "NO";
        }
        public static DateTime? ChangeFormatDate(string date)
        {
            if (string.IsNullOrEmpty(date))
            {
                return null;
            }
            string[] t = date.Trim().Split(new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries);

            return new DateTime(int.Parse(t[2]), int.Parse(t[0]), int.Parse(t[1]));
        }
        public static string BuildDropdown(object[] values, object[] texts, object selectedValue, string none = "- none -")
        {
            System.Text.StringBuilder st = new StringBuilder();
            if (none != null)
                st = new StringBuilder("<option value=''>" + none + "</option>");
            string s = selectedValue + "";
            for (int i = 0; i < values.Length; i++)
            {
                if(s.Equals(values[i]))
                    st.Append(string.Format("<option value='{0}' selected='selected'>{1}</option>", values[i], texts[i]));
                else
                    st.Append(string.Format("<option value='{0}'>{1}</option>", values[i], texts[i]));
            }

            return st.ToString();
        }
        public static string BuildDropdown(int[] values, object[] texts, object selectedValue, string none = "- none -")
        {
            System.Text.StringBuilder st = new StringBuilder();
            if (none != null)
                st = new StringBuilder("<option value=''>" + none + "</option>");
            string s = selectedValue + "";
            for (int i = 0; i < values.Length; i++)
            {
                if (s.Equals(values[i]))
                    st.Append(string.Format("<option value='{0}' selected='selected'>{1}</option>", values[i], texts[i]));
                else
                    st.Append(string.Format("<option value='{0}'>{1}</option>", values[i], texts[i]));
            }

            return st.ToString();
        }
    }
    public static class Cultures
    {
        public static readonly CultureInfo UnitedKingdom =
            CultureInfo.ReadOnly(new CultureInfo("en-US"));
    }
    public class FTPUtilities
    {
        public static string ftpServer = DB.Entities.mConfig.FirstOrDefault(m => m.Key == "FTPServer").Title;
        public static string ftpUser = DB.Entities.mConfig.FirstOrDefault(m => m.Key == "FTPUser").Title;
        public static string ftpPass = DB.Entities.mConfig.FirstOrDefault(m => m.Key == "FTPPassword").Title;

        private static FtpConnection GetFtpConnection()
        {
            string ftpserver = ftpServer.Replace("ftp://", null).Replace("/", null);
            if (ftpserver.Contains(":"))
            {
                string ftpserver1 = ftpserver.Split(':')[0];
                int port = int.Parse(ftpserver.Split(':')[1]);
                return new FtpConnection(ftpserver1, port, ftpUser, ftpPass);
            }
            return new FtpConnection(ftpserver, ftpUser, ftpPass);
        }
        #region Folder
        public static bool DirectoryExists(string path)
        {
            FtpConnection conn = GetFtpConnection();
            conn.Open();
            conn.Login();
            bool exists = conn.DirectoryExists(path);
            conn.Close();
            conn.Dispose();
            return exists;
        }
        public static bool CreateDirectory(string directoryName)
        {
            FTPClient ftpClient = new FTPClient(ftpServer, ftpUser, ftpPass);
            return ftpClient.CreateDirectory(directoryName);
        }

        public static FtpDirectoryInfo[] GetDirectories(string path)
        {
            FtpConnection conn = GetFtpConnection();
            conn.Open();
            conn.Login();
            FtpDirectoryInfo[] exists = conn.GetDirectories(path);
            conn.Close();
            conn.Dispose();
            return exists;
        }
        #endregion
        public static FtpFileInfo[] GetFiles(string path)
        {
            FtpConnection conn = GetFtpConnection();
            conn.Open();
            conn.Login();
            FtpFileInfo[] files = conn.GetFiles(path);
            conn.Close();
            conn.Dispose();
            return files;
        }
        public static bool FileExists(string fileName)
        {
            FTPClient ftpClient = new FTPClient(ftpServer, ftpUser, ftpPass);

            return ftpClient.FileExist(fileName);
        }

        public static bool CreateFile(string fileName, Stream stream)
        {
            FTPClient ftpClient = new FTPClient(ftpServer, ftpUser, ftpPass);
            return ftpClient.Upload(fileName, stream);
        }

        public static bool CreateFile(string fileName, byte[] data)
        {
            FTPClient ftpClient = new FTPClient(ftpServer, ftpUser, ftpPass);
            return ftpClient.Upload(fileName, data);
        }

        public static bool CreateFile(string fileName, string localFile)
        {
            FTPClient ftpClient = new FTPClient(ftpServer, ftpUser, ftpPass);
            return ftpClient.Upload(fileName, localFile);
        }
        public static byte[] GetByteArray(string fileName)
        {
            FTPClient ftpClient = new FTPClient(ftpServer, ftpUser, ftpPass);
            var ftpStream = ftpClient.GetStream(fileName);
            MemoryStream ms = new MemoryStream();
            byte[] byteBuffer = new byte[2048];
            int bytesRead = ftpStream.Read(byteBuffer, 0, 2048);
            while (bytesRead > 0)
            {
                ms.Write(byteBuffer, 0, bytesRead);
                bytesRead = ftpStream.Read(byteBuffer, 0, 2048);
            }
            return ms.ToArray();
        }
        public static Stream GetStream(string fileName)
        {
            FTPClient ftpClient = new FTPClient(ftpServer, ftpUser, ftpPass);
            return ftpClient.GetStream(fileName);
        }

        public static string ReadToEnd(string fileName)
        {
            Stream st = GetStream(fileName);
            byte[] buffer = new byte[st.Length];
            st.Read(buffer, 0, buffer.Length);
            return UTF8Encoding.UTF8.GetString(buffer);
        }

        public static bool Download(string fileName, string localFile, bool deleteOld = true)
        {
            FTPClient ftpClient = new FTPClient(ftpServer, ftpUser, ftpPass);
            return ftpClient.Download(fileName, localFile, deleteOld);
        }

        public static bool RenameFile(string oldFileName, string newFileName)
        {
            FTPClient ftpClient = new FTPClient(ftpServer, ftpUser, ftpPass);
            return ftpClient.Upload(oldFileName, newFileName);
        }

        public static void DeleteFile(string fileName)
        {
            FTPClient ftpClient = new FTPClient(ftpServer, ftpUser, ftpPass);
            ftpClient.Delete(fileName);
        }

        #region File Utilities

        public static bool Serialize(object obj, string filePath, bool append)
        {
            FileStream fileStream = null;
            bool isSuccess = false;
            try
            {
                fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write);
                BinaryFormatter binaryFormatter = new BinaryFormatter();
                binaryFormatter.Serialize(fileStream, obj);
                isSuccess = true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (fileStream != null)
                {
                    fileStream.Close();
                }
            }
            return isSuccess;
        }

        public static object DeSerialize(string filePath)
        {
            FileStream fileStream = null;
            object output = null;
            try
            {
                fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
                BinaryFormatter binaryFormatter = new BinaryFormatter();
                output = binaryFormatter.Deserialize(fileStream);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (fileStream != null)
                {
                    fileStream.Close();
                }
            }
            return output;
        }

        #endregion
    }
}