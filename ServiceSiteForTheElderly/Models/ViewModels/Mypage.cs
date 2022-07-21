using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

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
    }
}