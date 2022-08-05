using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Routing;

namespace ServiceSiteForTheElderly.Models.ViewModels
{
    public class MyPage
    {
        public static string AddClassActiveOrNormalButton(int index, int? ViewDataIndex)
        {
            if (ViewDataIndex == null)
            {
                return "normal-button";
            }
            return (index == ViewDataIndex) ? "active-button" : "normal-button";
        }

        public static string UrlActionId(int? ViewDataIndex)
        {
            if (ViewDataIndex == null)
            {
                return null;
            }

            switch (ViewDataIndex)
            {
                case 0:
                    return "UrlActionOrderDetail";
                case 1:
                    return "UrlActionBackOrderDetail";
                case 2:
                    return "UrlActionContactDetail";
                case 3:
                default:
                    return null;
            }
        }
    }
}