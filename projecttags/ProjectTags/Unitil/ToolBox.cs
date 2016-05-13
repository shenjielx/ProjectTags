using ProjectTags.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Web;
using System.Web.Security;

namespace ProjectTags.Unitil
{
    public class ToolBox
    {
        #region 获取IP
        /// <summary>
        /// 获取IP
        /// </summary>
        /// <returns></returns>
        public static string GetIP()
        {
            string ip = string.Empty;
            if (!string.IsNullOrEmpty(HttpContext.Current.Request.ServerVariables["HTTP_VIA"]))
                ip = Convert.ToString(HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"]);
            if (string.IsNullOrEmpty(ip))
                ip = Convert.ToString(HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"]);
            return ip;
        }
        #endregion

        #region 加密/解密
        private static byte[] KEY_64 = { 42, 16, 93, 156, 78, 4, 218, 32 };
        private static byte[] IV_64 = { 55, 103, 246, 79, 36, 99, 167, 3 };

        /// <summary>
        /// DES加密
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string DESEncrypt(string str)
        {
            string res = "";
            if (!string.IsNullOrEmpty(str))
            {
                DESCryptoServiceProvider des = new DESCryptoServiceProvider();
                MemoryStream ms = new MemoryStream();
                CryptoStream cs = new CryptoStream(ms, des.CreateEncryptor(KEY_64, IV_64), CryptoStreamMode.Write);
                StreamWriter sw = new StreamWriter(cs);
                sw.Write(str);
                sw.Flush();
                cs.FlushFinalBlock();
                ms.Flush();

                res = Convert.ToBase64String(ms.GetBuffer(), 0, Convert.ToInt32(ms.Length));
            }
            return res.Replace("+", "{projects}").Replace("/", "{tags}").Replace("=", "{pt}");
        }

        /// <summary>
        /// DES解密方法
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string DESDecrypt(string str)
        {
            try
            {
                string res = "";
                if (string.IsNullOrEmpty(str) == true)
                {
                    return str;
                }
                str = str.Replace("{projects}", "+").Replace("{tags}", "/").Replace("{pt}", "=");
                int d = str.Length;
                if (str != "")
                {
                    DESCryptoServiceProvider cp = new DESCryptoServiceProvider();
                    byte[] buffer = { };
                    try
                    {
                        buffer = Convert.FromBase64String(str);
                    }
                    catch (Exception ex)
                    {
                        res = ex.Message;
                    }
                    MemoryStream ms = new MemoryStream(buffer);
                    CryptoStream cs = new CryptoStream(ms, cp.CreateDecryptor(KEY_64, IV_64), CryptoStreamMode.Read);
                    StreamReader sr = new StreamReader(cs);

                    res = sr.ReadToEnd();
                }
                return res;
            }
            catch (Exception ex)
            {
            }
            return "";
        }
        #endregion

        #region 认证登录cookie

        /// <summary>
        /// 保存在客户端的认证信息Cookie名称
        /// </summary>
        public static string FormsAuthCookieName
        {
            get { return "_ptliname"; }
        }
        /// <summary>
        /// 用来分割用户票据中用户信息的分隔符
        /// </summary>
        public static string AUTH_TKT_USERDATA_DELIMITER
        {
            get { return "|"; }
        }

        /// <summary>
        /// 安全校验码
        /// </summary>
        public static string SecurityValidationKey
        {
            // Domain.Security 的MD5编码
            get { return "905ACE6698EE001A4F1F38D3BE1EA1A3"; }
        }
        /// <summary>
        /// 写入登录用户信息
        /// </summary>
        /// <param name="user"></param>
        /// <param name="expireTime"></param>
        public static void WriteUserInfo(LoginBase user, DateTime expireTime)
        {
            HttpContext context = System.Web.HttpContext.Current;
            DateTime expirationDate = expireTime;// DateTime.Now.Add(FormsAuthentication.Timeout);

            var userData = string.Format("{0}{1}{2}{1}{3}{1}{4}{1}{5}", SecurityValidationKey, AUTH_TKT_USERDATA_DELIMITER,
                user.UserID, user.UserName, user.Name,user.Token);
            FormsAuthenticationTicket ticket = new FormsAuthenticationTicket(
                1,
                "_ptli",// projects tags login info
                DateTime.Now,
                expirationDate,
                true,
                userData,
                FormsAuthentication.FormsCookiePath
            );

            string encryptedTicket = FormsAuthentication.Encrypt(ticket);

            HttpCookie cookie = new HttpCookie(FormsAuthCookieName, encryptedTicket);
            cookie.Expires = expireTime;
            cookie.HttpOnly = true;
            cookie.Path = "/";
            //cookie.Domain = "domain.com";
            context.Response.Cookies.Set(cookie);
        }
        /// <summary>
        /// 获取登录的TokenID
        /// </summary>
        public static LoginBase GetLoginUser
        {
            get
            {
                LoginBase user = new LoginBase();
                var cookie = ToolBox.GetCookie(FormsAuthCookieName);
                if (!string.IsNullOrEmpty(cookie))
                {
                    var ticket = FormsAuthentication.Decrypt(cookie);
                    if (!string.IsNullOrEmpty(ticket.UserData))
                    {
                        var arr = ticket.UserData.Split(new[] { AUTH_TKT_USERDATA_DELIMITER }, StringSplitOptions.RemoveEmptyEntries);
                        if (arr.Length > 1)
                        {
                            user.UserID = Convert.ToInt64(arr[1]);
                        }
                        if (arr.Length >2)
                        {
                            user.UserName = arr[2].ToString();
                        }
                        if (arr.Length > 3)
                        {
                            user.Name = arr[3].ToString();
                        }
                        if (arr.Length > 4)
                        {
                            user.Token = arr[4].ToString();
                        }
                    }
                }
                return user;
            }
        }
        #endregion
        
        #region Cookie操作
        /// <summary>
        /// 写cookie值
        /// </summary>
        /// <param name="strName">名称</param>
        /// <param name="strValue">值</param>
        public static void WriteCookie(string strName, string strValue)
        {
            HttpCookie cookie = HttpContext.Current.Request.Cookies[strName];
            if (cookie == null)
            {
                cookie = new HttpCookie(strName);
            }
            cookie.Value = strValue;
            HttpContext.Current.Response.AppendCookie(cookie);
        }
        /// <summary>
        /// 写cookie，自带domain
        /// </summary>
        /// <param name="strName">名称</param>
        /// <param name="strValue">值</param>
        /// <param name="expires">过期值：分钟</param>
        public static void WriteCookieHaveDomain(string strName, string strValue, int expires)
        {
            //当前域名的cookie
            ToolBox.WriteCookie(strName, strValue, expires);
            if (System.Web.HttpContext.Current.Request.Url.Host != "localhost")
            {
                //设置domin
                ToolBox.WriteCookie(strName, strValue, expires, "likecoder.com");
            }
        }
        /// <summary>
        /// 写cookie值
        /// </summary>
        /// <param name="strName">名称</param>
        /// <param name="strValue">值</param>
        /// <param name="expires">过期值：分钟</param>
        public static void WriteCookie(string strName, string strValue, int expires)
        {
            HttpCookie cookie = HttpContext.Current.Request.Cookies[strName];
            if (cookie == null)
            {
                cookie = new HttpCookie(strName);
            }
            cookie.Value = strValue;
            cookie.Expires = DateTime.Now.AddMinutes(expires);
            HttpContext.Current.Response.AppendCookie(cookie);

        }
        /// <summary>
        /// 写cookie值
        /// </summary>
        /// <param name="strName">名称</param>
        /// <param name="strValue">值</param>
        /// <param name="expires">过期时间,单位分钟,-1为不设过期</param>
        /// <param name="strDomain">域</param>
        public static void WriteCookie(string strName, string strValue, int expires, string strDomain)
        {
            HttpCookie cookie = HttpContext.Current.Request.Cookies[strName];
            if (cookie == null)
            {
                cookie = new HttpCookie(strName);
            }
            cookie.Value = strValue;
            cookie.Domain = strDomain;
            if (expires != -1) { cookie.Expires = DateTime.Now.AddMinutes(expires); }
            HttpContext.Current.Response.AppendCookie(cookie);
        }
        /// <summary>
        /// 清理cookie
        /// </summary>
        /// <param name="CookieName"></param>
        /// <returns></returns>
        public static bool DeleteCookie(string CookieName)
        {
            WriteCookie(CookieName, string.Empty, -60 * 24);
            return true;
        }

        /// <summary>
        /// 清理cookie，自带domin
        /// </summary>
        /// <param name="CookieName"></param>
        /// <returns></returns>
        public static bool DeleteCookieHaveDomain(string CookieName)
        {
            WriteCookieHaveDomain(CookieName, string.Empty, -60 * 24);
            return true;
        }
        /// <summary>
        /// 重载 清理cookie 指定域名
        /// </summary>
        /// <param name="CookieName"></param>
        /// <param name="DomainName"></param>
        /// <returns></returns>
        public static bool DeleteCookie(string CookieName, string DomainName)
        {
            WriteCookie(CookieName, string.Empty, -60 * 24, DomainName);
            return true;
        }

        /// <summary>
        /// 读cookie值
        /// </summary>
        /// <param name="strName">名称</param>
        /// <returns>cookie值</returns>
        public static string GetCookie(string strName)
        {
            try
            {
                if (HttpContext.Current.Request.Cookies != null && HttpContext.Current.Request.Cookies[strName] != null)
                {
                    return HttpContext.Current.Request.Cookies[strName].Value.ToString();
                }
            }
            catch { }
            return "";
        }
        #endregion

    }
}