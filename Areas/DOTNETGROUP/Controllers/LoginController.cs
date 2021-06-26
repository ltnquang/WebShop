using DemoWeb.Areas.DOTNETGROUP.Models;
using Facebook;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.OpenIdConnect;
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
    [RequireHttps]
    public class LoginController : Controller
    {
        // GET: DOTNETGROUP/Login
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index(LoginModel login)
        {
            if (ModelState.IsValid)
            {
                var user = new UserDao();
                var result = user.login(login.username, Common.EncryptMD5(login.password));

                if (result != null)
                {
                    login.username = result.Username;
                    login.fullName = result.FullName;
                    //ModelState.AddModelError("", "Đăng nhập thành công");
                    Session.Add(Constants.USER_SESSION, login);
                    return RedirectToAction("Index", "Home");
                }
            }
            else
            {
                ModelState.AddModelError("", "Đăng nhập thất bại");
            }

            return View("Index");
        }

        /// <summary>
        /// Send an OpenID Connect sign-in request.
        /// Alternatively, you can just decorate the SignIn method with the [Authorize] attribute
        /// </summary>
        public ActionResult LoginOutlook()
        {
            if (!Request.IsAuthenticated)
            {
                HttpContext.GetOwinContext().Authentication.Challenge(
                   new AuthenticationProperties { RedirectUri = "/DOTNETGROUP/Home/" },
                   OpenIdConnectAuthenticationDefaults.AuthenticationType);
                new HttpUnauthorizedResult();
            }
            else
            //if (Request.IsAuthenticated)
            {
                var resultFetch = new UserDao().Find(System.Security.Claims.ClaimsPrincipal.Current.FindFirst("preferred_username").Value);
                if (resultFetch != null)
                {
                    var userSession = new LoginModel();
                    userSession.username = resultFetch.Username;
                    userSession.fullName = resultFetch.FullName;
                    Session.Add(Constants.USER_SESSION, userSession);
                    return Redirect("/DOTNETGROUP/Home");
                }
            }
            return View("Index");

        }

        public ActionResult SignOut()
        {
            Session[Constants.USER_SESSION] = null;
            return RedirectToAction("Index", "Login");
        }

        public void LogOut()
        {
            Session[Constants.USER_SESSION] = null;
            Response.Redirect("~/DOTNETGROUP/Login/");

        }

        public ActionResult SignOutCallback()
        {
            if (!Request.IsAuthenticated)
            {
                // Redirect to home page if the user is authenticated.
                return RedirectToAction("Index", "Login");
            }

            return View();
        }

        public void RefreshSession()
        {
            string strRedirectController = Request.QueryString["redirect"];

            HttpContext.GetOwinContext().Authentication.Challenge(new AuthenticationProperties { RedirectUri = String.Format("/{0}", strRedirectController) }, OpenIdConnectAuthenticationDefaults.AuthenticationType);
        }

        public ActionResult LoginFacebook()
        {
            var fb = new FacebookClient();
            var loginUrl = fb.GetLoginUrl(new
            {
                client_id = ConfigurationManager.AppSettings["FbAppId"],
                client_secret = ConfigurationManager.AppSettings["FbAppSecret"],
                redirect_uri = RedirectUri.AbsoluteUri,
                response_type = "code",
                scope = "email",
            });
            return Redirect(loginUrl.AbsoluteUri);
        }

        private Uri RedirectUri
        {
            get
            {
                var uriBuilder = new UriBuilder(Request.Url);
                uriBuilder.Query = null;
                uriBuilder.Fragment = null;
                uriBuilder.Path = Url.Action("FacebookCallback");
                return uriBuilder.Uri;
            }
        }

        public ActionResult FacebookCallback(string code)
        {
            var fb = new FacebookClient();
            dynamic result = fb.Post("oauth/access_token", new
            {
                client_id = ConfigurationManager.AppSettings["FbAppId"],
                client_secret = ConfigurationManager.AppSettings["FbAppSecret"],
                redirect_uri = RedirectUri.AbsoluteUri,
                code = code
            });
            var accessToken = result.access_token;
            if (!string.IsNullOrEmpty(accessToken))
            {
                fb.AccessToken = accessToken;
                // Get the user's information, like email, first name, middle name etc
                dynamic me = fb.Get("me?fields=first_name,middle_name,last_name,id,email");
                string email = me.email;
                string userName = me.email;
                string firstname = me.first_name;
                string middlename = me.middle_name;
                string lastname = me.last_name;
                var resultUser = new UserDao().Find(me.email);
                if (resultUser != null)
                {
                    var userSession = new LoginModel();
                    userSession.username = resultUser.Username;
                    userSession.fullName = resultUser.FullName;
                    Session.Add(Constants.USER_SESSION, userSession);
                }
            }
            return Redirect("/DOTNETGROUP/Home");
        }

        //your client id  
        string clientid = ConfigurationManager.AppSettings["GgAppId"];
        string redirection_url = ConfigurationManager.AppSettings["redirectUri"];
        public void LoginGoogle()
        {
            string urls = "https://accounts.google.com/o/oauth2/v2/auth?scope=email&include_granted_scopes=true&redirect_uri=" + redirection_url + "&response_type=code&client_id=" + clientid + "";
            Response.Redirect(urls);
        }

    }
}