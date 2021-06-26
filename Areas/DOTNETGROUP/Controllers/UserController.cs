using DemoWeb.Areas.DOTNETGROUP.Models;
using Models;
using Models.DAO;
using Models.EF;
using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DemoWeb.Areas.DOTNETGROUP.Controllers
{
    public class UserController : BaseController
    {
        // GET: DOTNETGROUP/User
        public ActionResult Index(int page = 1, int pageSize = 2, string sort = "")
        {
            ViewBag.SortByFullName = string.IsNullOrEmpty(sort) ? "descending FullName" : "";
            var user = new UserDao();
            var model = user.ListAll();
            return View(model.OrderBy(x => (sort == "descending FullName" ? x.FullName : x.Username)).ToPagedList(page, pageSize));
        }

        [HttpPost]
        public ActionResult Index(string keyword, int page=1, int pageSize=2, string sort="")
        {
            ViewBag.SortByFullName = string.IsNullOrEmpty(sort) ? "descending FullName" : "";
            var user = new UserDao();
            var model = user.ListWhere(keyword);
            ViewBag.SearchKeyword = keyword;
            return View(model.OrderBy(x => (sort == "descending FullName" ? x.FullName : x.Username)).ToPagedList(page, pageSize));
        }


        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Create(User ent_User)
        {
            if (ModelState.IsValid)
            {
                var dao = new UserDao();
                if (dao.FindUserName(ent_User.Username, ent_User.Email) !=null)
                {
                    SetAlert("Người dùng đã tồn tại trong hệ thống", "warning"); 
                    return RedirectToAction("Create", "User");
                }
                var pass = Common.EncryptMD5(ent_User.Password);
                ent_User.Password = pass;
                string result = dao.Insert(ent_User);
                if (!string.IsNullOrEmpty(result))
                {
                    SetAlert("Tạo mới Người dùng thành công", "success");
                    return RedirectToAction("Index", "User");
                }
                else
                {
                    SetAlert("Tạo mới người dùng không thành công", "error");
                    //ModelState.AddModelError("", "Tạo mới người dùng không thành công");
                }
            }
            return View();
        }
    }
}