using DemoWeb.Areas.DOTNETGROUP.Models;
using Models;
using Models.DAO;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace DemoWeb.Areas.DOTNETGROUP.Controllers
{
    public class HomeController : Controller
    {
        // GET: DOTNETGROUP/Home
        public ActionResult Index()
        {
            if (Request.QueryString["code"] != null)
            {
                GoogleCallback(Request.QueryString["code"].ToString());
            }

            if (Session[Constants.USER_SESSION] == null)
                return RedirectToAction("Index", "Login");
            return View();
        }

        public void GoogleCallback(string code)
        {
            string poststring = "grant_type=authorization_code&code=" + code + "&client_id=" + clientid + "&client_secret=" + clientsecret + "&redirect_uri=" + redirection_url + "";
            var request = (HttpWebRequest)WebRequest.Create(url);
            request.ContentType = "application/x-www-form-urlencoded";
            request.Method = "POST";
            UTF8Encoding utfenc = new UTF8Encoding();
            byte[] bytes = utfenc.GetBytes(poststring);
            Stream outputstream = null;
            try
            {
                request.ContentLength = bytes.Length;
                outputstream = request.GetRequestStream();
                outputstream.Write(bytes, 0, bytes.Length);
            }
            catch
            { }
            var response = (HttpWebResponse)request.GetResponse();
            var streamReader = new StreamReader(response.GetResponseStream());
            string responseFromServer = streamReader.ReadToEnd();
            JavaScriptSerializer js = new JavaScriptSerializer();
            Tokenclass obj = js.Deserialize<Tokenclass>(responseFromServer);

            GetLogin(obj.access_token);
        }

        //your client id  
        string clientid = ConfigurationManager.AppSettings["GgAppId"];
        //your client secret  
        string clientsecret = ConfigurationManager.AppSettings["GgAppSecret"];
        string redirection_url = ConfigurationManager.AppSettings["redirectUri"];
        string url = "https://accounts.google.com/o/oauth2/token";

        public ActionResult GetLogin(string token)
        {
            string url3 = "https://www.googleapis.com/oauth2/v1/userinfo?alt=json&access_token=" + token + "";
            WebRequest request2 = WebRequest.Create(url3);
            request2.Credentials = CredentialCache.DefaultCredentials;
            WebResponse response2 = request2.GetResponse();
            Stream dataStream = response2.GetResponseStream();
            StreamReader reader = new StreamReader(dataStream);
            string responseFromServer2 = reader.ReadToEnd();
            reader.Close();
            response2.Close();
            JavaScriptSerializer js2 = new JavaScriptSerializer();

            LoginModel userinfo = js2.Deserialize<LoginModel>(responseFromServer2);
            var resultUser = new UserDao().Find(userinfo.email);
            if (resultUser != null)
            {
                var userSession = new LoginModel();
                userSession.username = resultUser.Username;
                userSession.fullName = resultUser.FullName;
                Session.Add(Constants.USER_SESSION, userSession);
            }
            return View();
        }

        public ActionResult Logout()
        {

            Session[Constants.USER_SESSION] = null;
            return RedirectToAction("Index", "Login");
        }
    }
}