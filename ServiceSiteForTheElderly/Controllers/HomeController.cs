﻿using ServiceSiteForTheElderly.Models.Common;
using ServiceSiteForTheElderly.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Web.Mvc;

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

            foreach (var aCategory in mCategores)
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
        public ActionResult Login(LoginModel postModel)
        {

            MCustomers cust = new MCustomers();
            int hitCount = 0;

            postModel.Tel = postModel.Tel.Replace("-", "");

            ReturnOfCheckDatabaseLogin rtn = CommonModel.CheckDatabaseLogin(postModel.Tel, postModel.Password, ref cust, ref hitCount);
            switch (rtn)
            {
                case ReturnOfCheckDatabaseLogin.Success:
                    // ログインOK
                    return Json(new MJsonWithStatus() { status = "success" });
                case ReturnOfCheckDatabaseLogin.WrongUserId:
                    // ユーザー名が間違い
                    return Json(new MJsonWithStatus() { status = "wrongUserId" });
                case ReturnOfCheckDatabaseLogin.WrongPassword:
                    // パスワードが間違い
                    return Json(new MJsonWithStatus() { status = "wrongPassword" });
                case ReturnOfCheckDatabaseLogin.RunException:
                default:
                    return Json(new MJsonWithStatus() { status = "error" });

            }

        }

        public ActionResult SignUp()
        {
            return View();
        }

        [HttpPost]
        public ActionResult SignUp(SignUpModel postModel)
        {
            postModel.Tel = postModel.Tel.Replace("-", "");
            postModel.Postcode = postModel.Postcode.Replace("-", "");

            if (CommonModel.CheckDatabaseIsUserIdExist(postModel.Tel) == ReturnOfCheckDatabaseIsUserIdExist.UserIdIsNotExist)
            {
                if (string.IsNullOrEmpty(postModel.Name) || string.IsNullOrEmpty(postModel.Furigana) || string.IsNullOrEmpty(postModel.Tel) || string.IsNullOrEmpty(postModel.Mail) || string.IsNullOrEmpty(postModel.Postcode) || string.IsNullOrEmpty(postModel.Address) || string.IsNullOrEmpty(postModel.Password))
                {
                    return Json(new MJsonWithStatus() { status = "containEmptyChar" });
                }

                CommonModel.RegistDatabaseCustmer(new MCustomers() { Name = postModel.Name, Furigana = postModel.Furigana, Tel = postModel.Tel, Mail = postModel.Mail, Postcode = postModel.Postcode, Address = postModel.Address, Password = postModel.Password });
                return Json(new MJsonWithStatus() { status = "success" });
            }
            else
            {
                return Json(new MJsonWithStatus() { status = "duplicateTelError" });
            }
        }
    }
}