using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace ServiceSiteForTheElderly.Models.Common
{
    public static class CommonModel
    {
        // 宅配配送サービス
        public static int GetDatabaseCategoriesWithoutContact(ref List<MCategores> mCategores)
        {
            DBAccess dba = new DBAccess();
            DataTable dt = null;
            dba.Query("select * from Categories where isContact=0 order by id asc;", ref dt);

            for (int row = 0; row < dt.Rows.Count; row++)
            {
                MCategores aCategory = new MCategores();
                aCategory.Name = dt.Rows[row].Field<string>("name");
                aCategory.Link = dt.Rows[row].Field<string>("link");
                mCategores.Add(aCategory);
            }

            return 0;
        }

        // その他のサービス
        public static int GetDatabaseCategoriesWithContact(ref List<MCategores> mCategores)
        {
            DBAccess dba = new DBAccess();
            DataTable dt = null;
            dba.Query("select * from Categories where isContact=1 order by id asc;", ref dt);

            for (int row = 0; row < dt.Rows.Count; row++)
            {
                MCategores aCategory = new MCategores();
                aCategory.Name = dt.Rows[row].Field<string>("name");
                aCategory.Link = dt.Rows[row].Field<string>("link");
                mCategores.Add(aCategory);
            }

            return 0;
        }


        public static int CheckDatabaseLogin(string inputTel, string inputPassword, ref MCustomers cust, ref int hitCount)
        {
            DBAccess dba = new DBAccess();
            DataTable dt = null;
            string sql = "select * from Customers";
            sql += $" where tel = '{inputTel}'";

            try
            {
                dba.Query(sql, ref dt);
                if (dt.Rows.Count > 0)
                {
                    hitCount = dt.Rows.Count;

                    string databasePassword = dt.Rows[0].Field<string>("password");

                    inputPassword = inputPassword.Trim();

                    if (databasePassword == inputPassword)
                    {
                        MCustomers mCustomers = new MCustomers() { 
                            Id = dt.Rows[0].Field<int>("id"),
                            Name = dt.Rows[0].Field<string>("name"),
                            Furigana = dt.Rows[0].Field<string>("furigana"),
                            Tel = dt.Rows[0].Field<string>("tel"),
                            Mail = dt.Rows[0].Field<string>("mail"),
                            Postcode = dt.Rows[0].Field<string>("postcode"),
                            Address = dt.Rows[0].Field<string>("address"),
                            Password = databasePassword
                        };
                        return 0;
                    }
                    else
                    {
                        return -2;
                    }

                }
                else
                {
                    return -1;
                }

            }
            catch (Exception)
            {
                return -3;
            }
            finally
            {
                dt = null;
            }
        }
    }
}