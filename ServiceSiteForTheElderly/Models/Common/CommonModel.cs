using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace ServiceSiteForTheElderly.Models.Common
{
    public enum ReturnOfCheckDatabaseLogin
    {
        Success = 0,
        WrongUserId = -1,
        WrongPassword = -2,
        RunException = -3,
    }

    public enum ReturnOfCheckDatabaseIsUserIdExist
    {
        UserIdIsExist = 0,
        UserIdIsNotExist = -1,
        RunException = -2
    }

    public enum ReturnOfBasicDatabase
    {
        Success = 0,
        Error = -1
    }

    /// <summary>
    /// データベースからモデルを作成するクラス
    /// </summary>
    public static class CommonModel
    {
        /// <summary>
        /// Indexの宅配配送サービス
        /// </summary>
        /// <param name="mCategores">返されるカテゴリーモデルのリスト</param>
        /// <returns>正常終了なら0</returns>
        public static int GetDatabaseCategoriesWithoutContact(ref List<MCategores> mCategores)
        {
            DBAccess dba = new DBAccess();
            DataTable dt = null;
            dba.Query("select * from Categories where isContact=0 order by id asc;", ref dt);

            for (int row = 0; row < dt.Rows.Count; row++)
            {
                MCategores aCategory = new MCategores();
                aCategory.Id = dt.Rows[row].Field<int>("id");
                aCategory.IsContact = dt.Rows[row].Field<bool>("isContact");
                aCategory.Name = dt.Rows[row].Field<string>("name");
                aCategory.Link = dt.Rows[row].Field<string>("link");
                mCategores.Add(aCategory);
            }

            return 0;
        }

        /// <summary>
        /// Indexのその他のサービス
        /// </summary>
        /// <param name="mCategores">返されるカテゴリーモデルのリスト</param>
        /// <returns>正常終了なら0</returns>
        public static int GetDatabaseCategoriesWithContact(ref List<MCategores> mCategores)
        {
            DBAccess dba = new DBAccess();
            DataTable dt = null;
            dba.Query("select * from Categories where isContact=1 order by id asc;", ref dt);

            for (int row = 0; row < dt.Rows.Count; row++)
            {
                MCategores aCategory = new MCategores();
                aCategory.Id = dt.Rows[row].Field<int>("id");
                aCategory.IsContact = dt.Rows[row].Field<bool>("isContact");
                aCategory.Name = dt.Rows[row].Field<string>("name");
                aCategory.Link = dt.Rows[row].Field<string>("link");
                mCategores.Add(aCategory);
            }

            return 0;
        }

        /// <summary>
        /// ユーザがいるかどうかチェック
        /// </summary>
        /// <param name="inputTel">電話番号</param>
        /// <returns>結果のステータス</returns>
        public static ReturnOfCheckDatabaseIsUserIdExist CheckDatabaseIsUserIdExist(string inputTel) {
            MCustomers cust = new MCustomers();
            int hitCount = 0;

            switch (CheckDatabaseLogin(inputTel, "", ref cust, ref hitCount))
            {
                case ReturnOfCheckDatabaseLogin.Success:
                case ReturnOfCheckDatabaseLogin.WrongPassword:
                    return ReturnOfCheckDatabaseIsUserIdExist.UserIdIsExist;
                case ReturnOfCheckDatabaseLogin.WrongUserId:
                    return ReturnOfCheckDatabaseIsUserIdExist.UserIdIsNotExist;
                default:
                case ReturnOfCheckDatabaseLogin.RunException:
                    return ReturnOfCheckDatabaseIsUserIdExist.RunException;
            }
        }


        /// <summary>
        /// ログインできるかどうかチェック
        /// </summary>
        /// <param name="inputTel">電話番号</param>
        /// <param name="inputPassword">パスワード</param>
        /// <param name="cust">返される顧客情報</param>
        /// <param name="hitCount">ヒット件数</param>
        /// <returns>結果のステータス</returns>
        public static ReturnOfCheckDatabaseLogin CheckDatabaseLogin(string inputTel, string inputPassword, ref MCustomers cust, ref int hitCount)
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
                    
                    if (inputPassword == null)
                    {
                        inputPassword = "";
                    }
                    inputPassword = inputPassword.Trim();
                    

                    if (databasePassword == inputPassword)
                    {
                        MCustomers mCustomers = new MCustomers()
                        {
                            Id = dt.Rows[0].Field<int>("id"),
                            Name = dt.Rows[0].Field<string>("name"),
                            Furigana = dt.Rows[0].Field<string>("furigana"),
                            Tel = dt.Rows[0].Field<string>("tel"),
                            Mail = dt.Rows[0].Field<string>("mail"),
                            Postcode = dt.Rows[0].Field<string>("postcode"),
                            Address = dt.Rows[0].Field<string>("address"),
                            Password = databasePassword
                        };
                        return ReturnOfCheckDatabaseLogin.Success;
                    }
                    else
                    {
                        return ReturnOfCheckDatabaseLogin.WrongPassword;
                    }

                }
                else
                {
                    return ReturnOfCheckDatabaseLogin.WrongUserId;
                }

            }
            catch (Exception)
            {
                return ReturnOfCheckDatabaseLogin.RunException;
            }
            finally
            {
                dt = null;
            }
        }

        /// <summary>
        /// 新規登録
        /// </summary>
        /// <param name="cust">登録する顧客情報</param>
        /// <returns>結果のステータス</returns>
        public static ReturnOfBasicDatabase RegistDatabaseCustomer(MCustomers cust)
        {
            DBAccess dba = new DBAccess();
            string sql = $"insert into Customers (name, furigana, tel, mail, postcode, address, password) values('{cust.Name}', '{cust.Furigana}', '{cust.Tel}', '{cust.Mail}', '{cust.Postcode}', '{cust.Address}', '{cust.Password}')";
            if (dba.Execute(sql) >= 0)
            {
                return ReturnOfBasicDatabase.Success;
            }else
            {
                return ReturnOfBasicDatabase.Error;
            }
        }

        /// <summary>
        /// カテゴリ名からカテゴリidを取得する
        /// </summary>
        /// <param name="categoryName">カテゴリ名</param>
        /// <returns>カテゴリid</returns>
        public static int GetDataBaseCategotyId(string categoryName)
        {
            DBAccess dba = new DBAccess();
            DataTable dt = null;
            dba.Query($"select id from Categories where name = '{categoryName}'", ref dt);
            int categoryId = dt.Rows[0].Field<int>("id");
            return categoryId;
        }

        /// <summary>
        /// 店名から店舗idを取得する
        /// </summary>
        /// <param name="shopName">店名</param>
        /// <returns>店舗id</returns>
        public static int GetDataBaseShopId(string shopName)
        {

            DBAccess dba = new DBAccess();
            DataTable dt = null;
            dba.Query($"select id from Shops where displayName = '{shopName}'", ref dt);
            int shopId = dt.Rows[0].Field<int>("id");
            return shopId;
        }

        /// <summary>
        /// 商品を取得(カテゴリidからのバージョン)
        /// </summary>
        /// <param name="categoryId">カテゴリid</param>
        /// <param name="mGoods">返される商品モデルのリスト</param>
        /// <returns>結果のステータス</returns>
        public static ReturnOfBasicDatabase GetDataBaseGoodsOfCategory(int categoryId, ref List<MGoods> mGoods)
        {

            DBAccess dba = new DBAccess();
            DataTable dt = null;
            bool isSuccess = true;

            dba.Query($"select * from Goods left outer join Shops on Goods.shopId = Shops.id where publicationStartDate <= GETDATE() and GETDATE() <= publicationEndDate and categoryId = {categoryId} order by orderOfPublication desc, publicationStartDate desc;", ref dt);


            for (int row = 0; row < dt.Rows.Count; row++)
            {
                MGoods aGoods = new MGoods();
                aGoods.Id = dt.Rows[row].Field<int>("id");
                aGoods.OrderOfPublication = dt.Rows[row].Field<int?>("orderOfPublication");
                aGoods.Name = dt.Rows[row].Field<string>("name");
                aGoods.Description = dt.Rows[row].Field<string>("description");
                aGoods.Picture = dt.Rows[row].Field<string>("picture");
                aGoods.ShopId = dt.Rows[row].Field<int>("shopId");
                aGoods.Publisher = dt.Rows[row].Field<string>("publisher");
                aGoods.Author = dt.Rows[row].Field<string>("author");
                aGoods.PublicationStartDate = dt.Rows[row].Field<DateTime>("publicationStartDate");
                aGoods.PublicationEndDate = dt.Rows[row].Field<DateTime>("publicationEndDate");

                // 最新の価格をセット
                DataTable dt2 = null;
                dba.Query($"select Top 1 price from GoodsPriceTrends where goodsId = {aGoods.Id} order by priceChangeDate desc;", ref dt2);

                int price = 0;

                try
                {
                    price = dt2.Rows[0].Field<int>("price");
                }
                catch (Exception)
                {
                    isSuccess = false;
                    price = 0;
                }
                aGoods.Price = price;

                mGoods.Add(aGoods);
            }



            if (isSuccess)
            {
                return ReturnOfBasicDatabase.Success;

            }
            else
            {
                return ReturnOfBasicDatabase.Error;
            }
        }


        /// <summary>
        /// 商品を取得(店舗idからのバージョン)
        /// </summary>
        /// <param name="shopId">店舗id</param>
        /// <param name="mGoods">返される雑誌モデルのリスト</param>
        /// <returns>結果のステータス</returns>
        public static ReturnOfBasicDatabase GetDataBaseGoodsOfShop(int shopId, ref List<MGoods> mGoods)
        {
            DBAccess dba = new DBAccess();
            DataTable dt = null;
            bool isSuccess = true;

            dba.Query($"select * from Goods where publicationStartDate <= GETDATE() and GETDATE() <= publicationEndDate and shopId = {shopId} order by orderOfPublication desc, publicationStartDate desc;", ref dt);

           
            for (int row = 0; row < dt.Rows.Count; row++)
            {
                MGoods aGoods = new MGoods();
                aGoods.Id = dt.Rows[row].Field<int>("id");
                aGoods.OrderOfPublication = dt.Rows[row].Field<int?>("orderOfPublication");
                aGoods.Name = dt.Rows[row].Field<string>("name");
                aGoods.Description = dt.Rows[row].Field<string>("description");
                aGoods.Picture = dt.Rows[row].Field<string>("picture");
                aGoods.ShopId = dt.Rows[row].Field<int>("shopId");
                aGoods.Publisher = dt.Rows[row].Field<string>("publisher");
                aGoods.Author = dt.Rows[row].Field<string>("author");
                aGoods.PublicationStartDate = dt.Rows[row].Field<DateTime>("publicationStartDate");
                aGoods.PublicationEndDate = dt.Rows[row].Field<DateTime>("publicationEndDate");

                // 最新の価格をセット
                DataTable dt2 = null;
                dba.Query($"select Top 1 price from GoodsPriceTrends where goodsId = {aGoods.Id} order by priceChangeDate desc;", ref dt2);
                
                int price = 0;

                try
                {
                    price = dt2.Rows[0].Field<int>("price");
                }
                catch(Exception)
                {
                    isSuccess = false;
                    price = 0;
                }
                aGoods.Price = price;

                mGoods.Add(aGoods);
            }

            if (isSuccess)
            {
                return ReturnOfBasicDatabase.Success;

            }
            else
            {
                return ReturnOfBasicDatabase.Error;
            }
        }



    }
}