using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ServiceSiteForTheElderly.Models;

namespace ServiceSiteForTheElderly.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            DBAccess dba = new DBAccess();

            // 宅配配送サービス
            DataTable dt = null;
            dba.Query("select * from Categories where isContact=0 order by id asc;", ref dt);

            List<MCategores> categories = new List<MCategores>();

            for (int row = 0; row < dt.Rows.Count; row++)
            {
                MCategores aCategory = new MCategores();
                aCategory.Name = dt.Rows[row].Field<string>("name");
                aCategory.Path = dt.Rows[row].Field<string>("path");
                categories.Add(aCategory);
            }

            string html = "";

            foreach(var aCategory in categories)
            {
                html += $"<a href=\"{@Url.Action("About", "Home")}\" class=\"btn-flat\"><span>{aCategory.Name.ToString()}<i class=\"fas fa-chevron-right\"></i></span></a>" + Environment.NewLine;
                
            }

            ViewData["categories"] = html;

            // その他のサービス
            dt = null;
            dba.Query("select * from Categories where isContact=1 order by id asc;", ref dt);

            categories.Clear();

            for (int row = 0; row < dt.Rows.Count; row++)
            {
                MCategores aCategory = new MCategores();
                aCategory.Name = dt.Rows[row].Field<string>("name");
                aCategory.Path = dt.Rows[row].Field<string>("path");
                categories.Add(aCategory);
            }

            html = "";

            foreach (var aCategory in categories)
            {
                html += $"<a href=\"{@Url.Action("About", "Home")}\" class=\"btn-flat\"><span>{aCategory.Name.ToString()}<i class=\"fas fa-chevron-right\"></i></span></a>" + Environment.NewLine;

            }

            ViewData["contacts"] = html;




            //dba.Execute("insert into Books (title, description) values ('JSビギナー', 'JavaScriptに関する本');");
            //ViewData["id"] = 1;
            return View();
        }

        public ActionResult About()
        {
            List<MBooks> books = new List<MBooks>();
            GetDatabaseCategoryList(ref books);

            ViewBag.Message = "";

            foreach (var aBook in books)
            {
                ViewBag.Message += $"({aBook.id}, {aBook.title}, {aBook.description})" + Environment.NewLine;
            }

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public int GetDatabaseCategoryList(ref List<MBooks> items)
        {
            DBAccess dba = new DBAccess();
            DataTable dt = null;
            dba.Query("select * from Books", ref dt);

            for (int row = 0; row < dt.Rows.Count; row++)
            {
                MBooks item = new MBooks();
                item.id = dt.Rows[row].Field<int>("id");
                item.title = dt.Rows[row].Field<string>("title"); ;
                item.description = dt.Rows[row].Field<string>("description"); ;
                items.Add(item);

            }

            return 0;
        }
    }

    
}