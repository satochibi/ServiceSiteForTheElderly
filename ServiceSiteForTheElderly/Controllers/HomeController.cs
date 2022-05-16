﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ServiceSiteForTheElderly.Models;
using ServiceSiteForTheElderly.Models.Common;
using ServiceSiteForTheElderly.Models.ViewModels;

namespace ServiceSiteForTheElderly.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            string sid = System.Web.HttpContext.Current.Session.SessionID;
            ViewData["SessionID"] = sid;

            List<MCategores> mCategores = new List<MCategores>();
            CommonModel.GetDatabaseCategoriesWithoutContact(ref mCategores);

            string html = "";

            foreach(var aCategory in mCategores)
            {
                html += $"<a href=\"{@Url.Action("About", "Home")}\" class=\"btn-flat\"><span>{aCategory.Name.ToString()}<i class=\"fas fa-chevron-right\"></i></span></a>" + Environment.NewLine;
                
            }

            ViewData["categories"] = html;

            mCategores.Clear();
            CommonModel.GetDatabaseCategoriesWithContact(ref mCategores);

            html = "";

            foreach (var aCategory in mCategores)
            {
                html += $"<a href=\"{@Url.Action("About", "Home")}\" class=\"btn-flat\"><span>{aCategory.Name.ToString()}<i class=\"fas fa-chevron-right\"></i></span></a>" + Environment.NewLine;

            }

            ViewData["contacts"] = html;

            return View();
        }

        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(LoginModel postmodel)
        {
            string rtnCustCode = string.Empty;
            MCustomers cust = new MCustomers();
            int hitCount = 0;
            int rtn = CommonModel.CheckDatabaseLogin(postmodel.Tel, postmodel.Password, ref cust, ref hitCount);
            switch (rtn)
            {
                case 0:
                    // ログインOK
                    return Json("returnstring");
                case -1:
                    // ユーザー名が間違い
                    return Json("userng");

                case -2:
                    // パスワードが間違い
                    return Json("passng");
                default:
                    return Json("error");
            }

            
        }
    }
}