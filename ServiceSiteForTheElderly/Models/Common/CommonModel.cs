﻿using ServiceSiteForTheElderly.Models.ViewModels;
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
        /// グローバルステータス(メンテナンス中かどうか)を取得
        /// </summary>
        /// <returns></returns>
        public static ReturnOfBasicDatabase GetDatabaseGlobalStatus()
        {
            DBAccess dba = new DBAccess();
            DataTable dt = null;
            dba.Query("select status from Global;", ref dt);

            switch (dt.Rows[0].Field<int>("status"))
            {

                case 0:
                    return ReturnOfBasicDatabase.Success;
                case -1:
                default:
                    return ReturnOfBasicDatabase.Error;

            }
        }

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
                MCategores aCategory = new MCategores
                {
                    Id = dt.Rows[row].Field<int>("id"),
                    IsContact = dt.Rows[row].Field<bool>("isContact"),
                    Name = dt.Rows[row].Field<string>("name"),
                    Link = dt.Rows[row].Field<string>("link")
                };
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
                MCategores aCategory = new MCategores
                {
                    Id = dt.Rows[row].Field<int>("id"),
                    IsContact = dt.Rows[row].Field<bool>("isContact"),
                    Name = dt.Rows[row].Field<string>("name"),
                    Link = dt.Rows[row].Field<string>("link")
                };
                mCategores.Add(aCategory);
            }

            return 0;
        }

        /// <summary>
        /// カテゴリidから、カテゴリ名を取得
        /// </summary>
        /// <param name="categoryId"></param>
        /// <returns></returns>
        public static string GetDatabaseCategoryName(int categoryId)
        {

            DBAccess dba = new DBAccess();
            DataTable dt = null;
            dba.Query($"select name from Categories where id = {categoryId};", ref dt);
            string categoryName = dt.Rows[0].Field<string>("name");
            return categoryName;
        }

        /// <summary>
        /// ユーザがいるかどうかチェック
        /// </summary>
        /// <param name="inputTel">電話番号</param>
        /// <returns>結果のステータス</returns>
        public static ReturnOfCheckDatabaseIsUserIdExist CheckDatabaseIsUserIdExist(string inputTel)
        {
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
            string sql = "select top (1) * from Customers";
            sql += $" where tel = '{inputTel}'";
            sql += " order by createdAt desc;";
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
                            CreatedAt = dt.Rows[0].Field<DateTime>("createdAt"),
                            Name = dt.Rows[0].Field<string>("name"),
                            Furigana = dt.Rows[0].Field<string>("furigana"),
                            Tel = dt.Rows[0].Field<string>("tel"),
                            Mail = dt.Rows[0].Field<string>("mail"),
                            Postcode = dt.Rows[0].Field<string>("postcode"),
                            Address = dt.Rows[0].Field<string>("address"),
                            Password = databasePassword
                        };

                        cust = mCustomers;
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
        /// <param name="custId">登録した顧客情報のid</param>
        /// <returns>結果のステータス</returns>
        public static ReturnOfBasicDatabase RegistDatabaseCustomer(MCustomers cust, ref int? custId)
        {
            DBAccess dba = new DBAccess();
            DataTable dt = null;
            string mail = string.IsNullOrEmpty(cust.Mail) ? "NULL" : $"'{cust.Mail}'";
            string postcode = string.IsNullOrEmpty(cust.Postcode) ? "NULL" : $"'{cust.Postcode}'";
            string address = string.IsNullOrEmpty(cust.Address) ? "NULL" : $"'{cust.Address}'";
            string sql = $"insert into Customers (createdAt, name, furigana, tel, mail, postcode, address, password) values(GETDATE(), '{cust.Name}', '{cust.Furigana}', '{cust.Tel}', {mail}, {postcode}, {address}, '{cust.Password}'); select @@IDENTITY;";
            try
            {
                dba.Query(sql, ref dt);
                int id = int.Parse(dt.Rows[0].ItemArray[0].ToString());
                custId = id;
                return ReturnOfBasicDatabase.Success;
            }
            catch (Exception)
            {
                return ReturnOfBasicDatabase.Error;
            }
        }

        /// <summary>
        /// 顧客情報の更新
        /// </summary>
        /// <param name="cust">登録する顧客情報</param>
        /// <returns></returns>
        public static ReturnOfBasicDatabase UpdateDatabaseCustomer(MCustomers cust)
        {
            DBAccess dba = new DBAccess();
            string mail = string.IsNullOrEmpty(cust.Mail) ? "NULL" : $"'{cust.Mail}'";
            string postcode = string.IsNullOrEmpty(cust.Postcode) ? "NULL" : $"'{cust.Postcode}'";
            string address = string.IsNullOrEmpty(cust.Address) ? "NULL" : $"'{cust.Address}'";

            string sql = "SET IDENTITY_INSERT Customers ON;";
            sql += $"insert into Customers (id, createdAt, name, furigana, tel, mail, postcode, address, password) values({cust.Id}, GETDATE(), '{cust.Name}', '{cust.Furigana}', '{cust.Tel}', {mail}, {postcode}, {address}, '{cust.Password}');";
            try
            {
                dba.Execute(sql);
                return ReturnOfBasicDatabase.Success;
            }
            catch (Exception)
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
        /// 弁当の店舗一覧を取得
        /// </summary>
        /// <param name="mShops">返される店舗モデルのリスト</param>
        /// <returns>結果のステータス</returns>
        public static ReturnOfBasicDatabase GetDataBaseShopsOfBento(ref List<MShops> mShops)
        {
            DBAccess dba = new DBAccess();
            DataTable dt = null;

            int categoryId = GetDataBaseCategotyId("料理とお弁当");

            dba.Query($"select * from Shops where categoryId = {categoryId};", ref dt);

            for (int row = 0; row < dt.Rows.Count; row++)
            {
                MShops aShop = new MShops();
                aShop.Id = dt.Rows[row].Field<int>("id");
                aShop.DisplayName = dt.Rows[row].Field<string>("displayName");
                aShop.ShippingCost = dt.Rows[row].Field<int?>("shippingCost");
                aShop.CategoryId = dt.Rows[row].Field<int>("categoryId");
                aShop.CompanyId = dt.Rows[row].Field<int>("companyId");
                aShop.Picture = dt.Rows[row].Field<string>("picture");
                aShop.Genre = dt.Rows[row].Field<string>("genre");
                mShops.Add(aShop);
            }

            return ReturnOfBasicDatabase.Success;
        }

        /// <summary>
        /// 商品の一覧を取得(カテゴリidからのバージョン)
        /// </summary>
        /// <param name="categoryId">カテゴリid</param>
        /// <param name="mGoods">返される商品モデルのリスト</param>
        /// <param name="q">検索キーワード(省略可能)</param>
        /// <returns>結果のステータス</returns>
        public static ReturnOfBasicDatabase GetDataBaseGoodsOfCategory(int categoryId, ref List<MGoods> mGoods, string q = "")
        {

            DBAccess dba = new DBAccess();
            DataTable dt = null;
            bool isSuccess = true;

            q = string.IsNullOrEmpty(q) ? "" : $"and (name like '%{q}%' or description like '%{q}%')";

            dba.Query($"select * from Goods left outer join Shops on Goods.shopId = Shops.id where publicationStartDate <= GETDATE() and GETDATE() <= publicationEndDate and categoryId = {categoryId} {q} order by orderOfPublication desc, publicationStartDate desc;", ref dt);


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
                aGoods.ShopName = dt.Rows[row].Field<string>("displayName");
                aGoods.ShopGenre = dt.Rows[row].Field<string>("genre");

                // 最新の価格をセット
                DataTable dt2 = null;

                // 最新の価格ビュー(Latest)を作成
                MakeViewDataBaseLatestGoodsPriceTrends(dba);

                try
                {
                    dba.Query($"select variety, price from LatestGoodsPriceTrends where goodsId = {aGoods.Id} order by price desc;", ref dt2);
                    // 商品価格をリストに追加
                    List<MPrice> prices = new List<MPrice>();
                    for (int index = 0; index < dt2.Rows.Count; index++)
                    {
                        MPrice mPrice = new MPrice() { Variety = dt2.Rows[index].Field<string>("variety"), Price = dt2.Rows[index].Field<int>("price") };
                        prices.Add(mPrice);
                    }
                    aGoods.Price = prices;
                    mGoods.Add(aGoods);
                }
                catch (Exception)
                {
                    isSuccess = false;
                }
            }


            return isSuccess ? ReturnOfBasicDatabase.Success : ReturnOfBasicDatabase.Error;
        }

        /// <summary>
        /// 商品の一覧を取得(店舗idからのバージョン)
        /// </summary>
        /// <param name="shopId">店舗id</param>
        /// <param name="mGoods">返される雑誌モデルのリスト</param>
        /// <returns>結果のステータス</returns>
        public static ReturnOfBasicDatabase GetDataBaseGoodsOfShop(int shopId, ref List<MGoods> mGoods, string q = "")
        {
            DBAccess dba = new DBAccess();
            DataTable dt = null;
            bool isSuccess = true;

            q = string.IsNullOrEmpty(q) ? "" : $"and (name like '%{q}%' or description like '%{q}%')";

            dba.Query($"select * from Goods left outer join Shops on Goods.shopId = Shops.id where publicationStartDate <= GETDATE() and GETDATE() <= publicationEndDate and shopId = {shopId} {q} order by orderOfPublication desc, publicationStartDate desc;", ref dt);

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
                aGoods.ShopName = dt.Rows[row].Field<string>("displayName");
                aGoods.ShopGenre = dt.Rows[row].Field<string>("genre");

                // 最新の価格をセット
                DataTable dt2 = null;

                // 最新の価格変動ビュー(LatestGoodsPriceTrends)を作成
                MakeViewDataBaseLatestGoodsPriceTrends(dba);

                try
                {
                    dba.Query($"select variety, price from LatestGoodsPriceTrends where goodsId = {aGoods.Id} order by price desc;", ref dt2);
                    // 商品価格をリストに追加
                    List<MPrice> prices = new List<MPrice>();
                    for (int index = 0; index < dt2.Rows.Count; index++)
                    {
                        MPrice mPrice = new MPrice() { Variety = dt2.Rows[index].Field<string>("variety"), Price = dt2.Rows[index].Field<int>("price") };
                        prices.Add(mPrice);
                    }
                    aGoods.Price = prices;
                    mGoods.Add(aGoods);
                }
                catch (Exception)
                {
                    isSuccess = false;
                }
            }


            return isSuccess ? ReturnOfBasicDatabase.Success : ReturnOfBasicDatabase.Error;
        }

        /// <summary>
        /// 過去～現在の最新のGoodsPriceTrendsのビューを作成する
        /// </summary>
        /// <param name="dba">データベースオブジェクト</param>
        private static void MakeViewDataBaseLatestGoodsPriceTrends(DBAccess dba)
        {
            dba.Execute("drop view LatestGoodsPriceTrends;");
            dba.Execute($"create view LatestGoodsPriceTrends as (select Latest.goodsId, Latest.priceChangeDate, NULLIF(Latest.variety, '') as variety, price from(select goodsId, MAX(priceChangeDate) as priceChangeDate, isnull(variety, '') as variety from GoodsPriceTrends where priceChangeDate <= GETDATE() group by goodsId, variety) as Latest left join(select goodsId, priceChangeDate, isnull(variety, '') as variety, price from GoodsPriceTrends) as Trends on Latest.goodsId = Trends.goodsId and Latest.priceChangeDate = Trends.priceChangeDate and Latest.variety = Trends.variety);");
            return;
        }

        /// <summary>
        /// 引数にセットした日付より過去のPreviousGoodsPriceTrendsのビューを作成する
        /// </summary>
        /// <param name="dba">データベースオブジェクト</param>
        /// <param name="dateTime">過去の当時の日付</param>
        private static void MakeViewDataBasePreviousGoodsPriceTrends(DBAccess dba, DateTime dateTime)
        {
            dba.Execute("drop view PreviousGoodsPriceTrends;");
            string datetimeStr = dateTime.ToString("yyyy-MM-dd HH:mm:ss.fff");
            dba.Execute($"create view PreviousGoodsPriceTrends as (select Latest.goodsId, Latest.priceChangeDate, NULLIF(Latest.variety, '') as variety, price from(select goodsId, MAX(priceChangeDate) as priceChangeDate, isnull(variety, '') as variety from GoodsPriceTrends where priceChangeDate <= '{datetimeStr}' group by goodsId, variety) as Latest left join(select goodsId, priceChangeDate, isnull(variety, '') as variety, price from GoodsPriceTrends) as Trends on Latest.goodsId = Trends.goodsId and Latest.priceChangeDate = Trends.priceChangeDate and Latest.variety = Trends.variety);");
            return;
        }

        /// <summary>
        /// カートに入っている商品をデータベースに問い合わせ
        /// </summary>
        /// <param name="cartModelInfo">カートのモデル</param>
        /// <param name="mGoodsOfCart">返される商品モデルのリスト</param>
        public static void GetDataBaseGoodsInCart(List<CartModel> cartModelInfo, ref List<MGoodsOfCart> mGoodsOfCart)
        {
            DBAccess dba = new DBAccess();
            DataTable dt = null;

            if (cartModelInfo == null)
            {
                return;
            }

            MakeViewDataBaseLatestGoodsPriceTrends(dba);

            foreach (var aItemInCart in cartModelInfo)
            {
                // 商品idから商品を検索
                if (string.IsNullOrEmpty(aItemInCart.Variety))
                {
                    dba.Query($"select Top(1) * from Goods left join LatestGoodsPriceTrends on Goods.id = LatestGoodsPriceTrends.goodsId left join Shops on shopId = Shops.id where Goods.id = {aItemInCart.GoodsId} order by priceChangeDate desc;", ref dt);
                    aItemInCart.Variety = "";
                }
                else
                {
                    dba.Query($"select Top(1) * from Goods left join LatestGoodsPriceTrends on Goods.id = LatestGoodsPriceTrends.goodsId left join Shops on shopId = Shops.id where Goods.id = {aItemInCart.GoodsId} and variety = '{aItemInCart.Variety}' order by priceChangeDate desc;", ref dt);
                }

                MGoodsOfCart aGoods = new MGoodsOfCart();
                aGoods.GoodsId = aItemInCart.GoodsId;
                aGoods.Quantity = aItemInCart.Quantity;
                aGoods.Variety = string.IsNullOrEmpty(aItemInCart.Variety) ? "" : aItemInCart.Variety;
                aGoods.Price = dt.Rows[0].Field<int>("price");
                aGoods.Name = dt.Rows[0].Field<string>("name");
                aGoods.Picture = dt.Rows[0].Field<string>("picture");
                aGoods.ShopId = dt.Rows[0].Field<int>("shopId");
                aGoods.PublicationStartDate = dt.Rows[0].Field<DateTime>("publicationStartDate");
                aGoods.PublicationEndDate = dt.Rows[0].Field<DateTime>("publicationEndDate");
                aGoods.ShopName = dt.Rows[0].Field<string>("displayName");
                aGoods.ShopGenre = dt.Rows[0].Field<string>("genre");
                aGoods.ShippingCost = dt.Rows[0].Field<int>("shippingCost");
                mGoodsOfCart.Add(aGoods);
            }

        }

        /// <summary>
        /// ShippingAddressesに登録
        /// </summary>
        /// <param name="CurrentSession">現在のセッション情報</param>
        /// <param name="shippingAddressesId">出力されるShippingAddressesId</param>
        /// <returns></returns>
        public static ReturnOfBasicDatabase RegistDatabaseShippingAddresses(SessionModel CurrentSession, ref int? shippingAddressesId)
        {
            // 送り先情報がなければスキップ
            if (CurrentSession.shippingAddressInfo == null)
            {
                shippingAddressesId = null;
                return ReturnOfBasicDatabase.Success;
            }


            DBAccess dba = new DBAccess();
            DataTable dt = null;

            string sql = $"insert into ShippingAddresses (name, furigana, tel, postcode, address) values('{CurrentSession.shippingAddressInfo.Name}', '{CurrentSession.shippingAddressInfo.Furigana}', '{CurrentSession.shippingAddressInfo.Tel}', '{CurrentSession.shippingAddressInfo.Postcode}', '{CurrentSession.shippingAddressInfo.Address}'); select @@IDENTITY;";
            try
            {
                dba.Query(sql, ref dt);
                int id = int.Parse(dt.Rows[0].ItemArray[0].ToString());
                shippingAddressesId = id;
                return ReturnOfBasicDatabase.Success;
            }
            catch (Exception)
            {
                return ReturnOfBasicDatabase.Error;
            }

        }

        /// <summary>
        /// Orderテーブルに登録
        /// </summary>
        /// <param name="CurrentSession">現在のセッション情報</param>
        /// <param name="ordersId">出力されるOrderId</param>
        /// <returns></returns>
        public static ReturnOfBasicDatabase RegistDatabaseOrders(SessionModel CurrentSession, int? shippingAddressesId, ref int ordersId)
        {
            DBAccess dba = new DBAccess();
            DataTable dt = null;

            string shippingAddressesStr = (shippingAddressesId == null) ? " null" : $" '{shippingAddressesId}'";

            string sql = $"insert into Orders (randomId, customerId, orderDate, shippingAddressesId, isCash) values(REPLACE(CAST(NEWID() AS NCHAR(36)), '-', ''), '{CurrentSession.customerUserInfo.Id}', GETDATE(), {shippingAddressesStr}, 'false'); select @@IDENTITY;";
            try
            {
                dba.Query(sql, ref dt);
                int id = int.Parse(dt.Rows[0].ItemArray[0].ToString());
                ordersId = id;
                return ReturnOfBasicDatabase.Success;
            }
            catch (Exception)
            {
                return ReturnOfBasicDatabase.Error;
            }
        }

        /// <summary>
        /// OrderGoodsテーブルに登録
        /// </summary>
        /// <param name="CurrentSession">現在のセッション情報</param>
        /// <param name="ordersId">登録するordersId</param>
        /// <returns></returns>
        public static ReturnOfBasicDatabase RegistDatabaseOrderGoods(SessionModel CurrentSession, int ordersId)
        {
            DBAccess dba = new DBAccess();

            foreach (var item in CurrentSession.cartModelInfo)
            {
                var variety = string.IsNullOrEmpty(item.Variety) ? "null" : $"'{item.Variety}'";
                string sql = $"insert into OrderGoods (goodsId, variety, quantity, startTimeOfDist, endTimeOfDist, orderId) values('{item.GoodsId}', {variety}, '{item.Quantity}', null, null, '{ordersId}');";
                if (dba.Execute(sql) == -1)
                {
                    return ReturnOfBasicDatabase.Error;
                }
            }

            return ReturnOfBasicDatabase.Success;

        }

        /// <summary>
        /// orderIdからrandomIdに変換
        /// </summary>
        /// <param name="ordersId">変換元のorderId</param>
        /// <returns></returns>
        public static string GetDatabaseOrdersIdToRandomId(int ordersId)
        {
            DBAccess dba = new DBAccess();
            DataTable dt = null;
            string sql = $"select randomId from Orders where id = {ordersId};";
            dba.Query(sql, ref dt);
            return dt.Rows[0].Field<string>("randomId");
        }

        /// <summary>
        /// randomIdからorderIdに変換
        /// </summary>
        /// <param name="randomId">変換元のrandomId</param>
        /// <returns></returns>
        public static int? GetDatabaseRandomIdToOrdersId(string randomId)
        {
            DBAccess dba = new DBAccess();
            DataTable dt = null;
            string sql = $"select id from Orders where randomId = '{randomId}';";
            try
            {
                dba.Query(sql, ref dt);
                return dt.Rows[0].Field<int>("id");
            }
            catch (Exception)
            {
                return null;
            }

        }

        /// <summary>
        /// randomIdからcontactIdに変換
        /// </summary>
        /// <param name="randomId"></param>
        /// <returns></returns>
        public static int? GetDatabaseRandomIdToContactId(string randomId)
        {
            DBAccess dba = new DBAccess();
            DataTable dt = null;
            string sql = $"select id from Contacts where randomId = '{randomId}';";
            try
            {
                dba.Query(sql, ref dt);
                return dt.Rows[0].Field<int>("id");
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// randomIdからbackOrderIdに変換
        /// </summary>
        /// <param name="randomId"></param>
        /// <returns></returns>
        public static int? GetDatabaseRandomIdToBackOrderId(string randomId)
        {
            DBAccess dba = new DBAccess();
            DataTable dt = null;
            string sql = $"select id from Backorders where randomId = '{randomId}';";
            try
            {
                dba.Query(sql, ref dt);
                return dt.Rows[0].Field<int>("id");
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// Ordersテーブルを取得
        /// </summary>
        /// <param name="customerId">顧客id</param>
        /// <param name="mOrders">出力されるOrderのリスト</param>
        /// <returns></returns>
        public static ReturnOfBasicDatabase GetDatabaseOrders(int customerId, ref List<MOrders> mOrders)
        {
            DBAccess dba = new DBAccess();
            DataTable dt = null;

            string sql = $"select * from Orders where customerId={customerId} order by orderDate desc;";
            try
            {
                dba.Query(sql, ref dt);
                for (int row = 0; row < dt.Rows.Count; row++)
                {
                    MOrders aOrder = new MOrders();
                    aOrder.Id = dt.Rows[row].Field<int>("id");
                    aOrder.RandomId = dt.Rows[row].Field<string>("randomId");
                    aOrder.CustomerId = dt.Rows[row].Field<int>("customerId");
                    aOrder.OrderDate = dt.Rows[row].Field<DateTime>("orderDate");
                    aOrder.ShippingAddressesId = dt.Rows[row].Field<int?>("shippingAddressesId");
                    aOrder.IsCash = dt.Rows[row].Field<bool>("isCash");
                    mOrders.Add(aOrder);
                }
                return ReturnOfBasicDatabase.Success;
            }
            catch (Exception)
            {
                return ReturnOfBasicDatabase.Error;
            }
        }

        /// <summary>
        /// OrderGoodsテーブルからMGoodsOfCartに変換(注文の詳細のカートの中身のため)
        /// </summary>
        /// <param name="mOrderGoods"></param>
        /// <param name="mGoodsOfCart"></param>
        public static void GetDataBaseOrderGoodsInCart(DateTime dateTime, List<MOrderGoods> mOrderGoods, ref List<MGoodsOfCart> mGoodsOfCart)
        {
            DBAccess dba = new DBAccess();
            DataTable dt = null;

            if (mOrderGoods == null)
            {
                return;
            }

            MakeViewDataBasePreviousGoodsPriceTrends(dba, dateTime);

            foreach (var aItemInCart in mOrderGoods)
            {
                // 商品idから商品を検索
                if (string.IsNullOrEmpty(aItemInCart.Variety))
                {
                    dba.Query($"select Top(1) * from Goods left join PreviousGoodsPriceTrends on Goods.id = PreviousGoodsPriceTrends.goodsId left join Shops on shopId = Shops.id where Goods.id = {aItemInCart.GoodsId} order by priceChangeDate desc;", ref dt);
                    aItemInCart.Variety = "";
                }
                else
                {
                    dba.Query($"select Top(1) * from Goods left join PreviousGoodsPriceTrends on Goods.id = PreviousGoodsPriceTrends.goodsId left join Shops on shopId = Shops.id where Goods.id = {aItemInCart.GoodsId} and variety = '{aItemInCart.Variety}' order by priceChangeDate desc;", ref dt);
                }

                MGoodsOfCart aGoods = new MGoodsOfCart();
                aGoods.GoodsId = aItemInCart.GoodsId;
                aGoods.Quantity = aItemInCart.Quantity;
                aGoods.Variety = string.IsNullOrEmpty(aItemInCart.Variety) ? "" : aItemInCart.Variety;
                aGoods.Price = dt.Rows[0].Field<int>("price");
                aGoods.Name = dt.Rows[0].Field<string>("name");
                aGoods.Picture = dt.Rows[0].Field<string>("picture");
                aGoods.ShopId = dt.Rows[0].Field<int>("shopId");
                aGoods.PublicationStartDate = dt.Rows[0].Field<DateTime>("publicationStartDate");
                aGoods.PublicationEndDate = dt.Rows[0].Field<DateTime>("publicationEndDate");
                aGoods.ShopName = dt.Rows[0].Field<string>("displayName");
                aGoods.ShopGenre = dt.Rows[0].Field<string>("genre");
                aGoods.ShippingCost = dt.Rows[0].Field<int>("shippingCost");
                aGoods.StartTimeOfDist = aItemInCart.StartTimeOfDist;
                aGoods.EndTimeOfDist = aItemInCart.EndTimeOfDist;
                mGoodsOfCart.Add(aGoods);
            }

        }

        /// <summary>
        /// 注文の詳細を取得
        /// </summary>
        /// <param name="randomId">変換元のrandomId</param>
        /// <param name="mOrder">出力されるOrder情報</param>
        /// <param name="mOrderGoods">出力されるOrderGoods情報</param>
        /// <returns></returns>
        public static ReturnOfBasicDatabase GetDatabaseOrderGoods(string randomId, ref MOrders mOrder, ref List<MOrderGoods> mOrderGoods)
        {
            int? orderId = CommonModel.GetDatabaseRandomIdToOrdersId(randomId);

            if (orderId == null)
            {
                return ReturnOfBasicDatabase.Error;
            }

            DBAccess dba = new DBAccess();
            DataTable dt = null;
            string sql = $"select * from Orders where id = {orderId};";

            dba.Query(sql, ref dt);

            // Orderテーブルから取得
            mOrder = new MOrders();
            mOrder.Id = dt.Rows[0].Field<int>("id");
            mOrder.RandomId = dt.Rows[0].Field<string>("randomId");
            mOrder.CustomerId = dt.Rows[0].Field<int>("customerId");
            mOrder.OrderDate = dt.Rows[0].Field<DateTime>("orderDate");
            mOrder.ShippingAddressesId = dt.Rows[0].Field<int?>("shippingAddressesId");
            mOrder.IsCash = dt.Rows[0].Field<bool>("isCash");


            dt = null;
            // OrderGoodsテーブルから取得
            sql = $"select * from OrderGoods where orderId = {orderId};";
            dba.Query(sql, ref dt);

            for (int row = 0; row < dt.Rows.Count; row++)
            {
                MOrderGoods aOrderGoods = new MOrderGoods();
                aOrderGoods.GoodsId = dt.Rows[row].Field<int>("goodsId");
                aOrderGoods.Variety = dt.Rows[row].Field<string>("variety");
                aOrderGoods.Quantity = dt.Rows[row].Field<int>("quantity");
                aOrderGoods.StartTimeOfDist = dt.Rows[row].Field<DateTime?>("startTimeOfDist");
                aOrderGoods.EndTimeOfDist = dt.Rows[row].Field<DateTime?>("endTimeOfDist");
                aOrderGoods.OrderId = dt.Rows[row].Field<int>("orderId");
                mOrderGoods.Add(aOrderGoods);
            }

            return ReturnOfBasicDatabase.Success;

        }

        /// <summary>
        /// shippingAddressesIdからMShippingAddressを取得
        /// </summary>
        /// <param name="shippingAddressesId"></param>
        /// <param name="mShippingAddress"></param>
        /// <returns></returns>
        public static ReturnOfBasicDatabase GetDatabaseShippingAddress(int shippingAddressesId, ref MShippingAddress mShippingAddress)
        {
            DBAccess dba = new DBAccess();
            DataTable dt = null;
            string sql = $"select * from ShippingAddresses where id = {shippingAddressesId};";

            try
            {
                dba.Query(sql, ref dt);
                mShippingAddress = new MShippingAddress()
                {
                    Name = dt.Rows[0].Field<string>("name"),
                    Furigana = dt.Rows[0].Field<string>("furigana"),
                    Tel = dt.Rows[0].Field<string>("tel"),
                    Postcode = dt.Rows[0].Field<string>("postcode"),
                    Address = dt.Rows[0].Field<string>("address")
                };
                return ReturnOfBasicDatabase.Success;
            }
            catch (Exception)
            {
                return ReturnOfBasicDatabase.Error;
            }
        }

        /// <summary>
        /// お問い合わせ情報をデータベースに登録
        /// </summary>
        /// <param name="CurrentSession">現在のセッション情報</param>
        /// <param name="categoryId">登録するカテゴリid</param>
        /// <param name="randomId">出力されるランダムid</param>
        /// <returns></returns>
        public static ReturnOfBasicDatabase RegistDatabaseContacts(SessionModel CurrentSession, int categoryId, ref string randomId)
        {
            DBAccess dba = new DBAccess();
            DataTable dt = null;
            randomId = null;
            string sql = $"insert into Contacts (randomId, customerId, createdAt, categoryId, message, replyMessage, replyDate) values(REPLACE(CAST(NEWID() AS NCHAR(36)), '-', ''), {CurrentSession.customerUserInfo.Id}, GETDATE(), {categoryId}, '{CurrentSession.message}', NULL, NULL);select @@IDENTITY;";
            try
            {

                dba.Query(sql, ref dt);
                int id = int.Parse(dt.Rows[0].ItemArray[0].ToString());
                sql = $"select randomId from Contacts where id = {id}";
                dba.Query(sql, ref dt);
                randomId = dt.Rows[0].ItemArray[0].ToString();

                return ReturnOfBasicDatabase.Success;
            }
            catch (Exception)
            {
                return ReturnOfBasicDatabase.Error;
            }
        }

        /// <summary>
        /// Contactsテーブルを取得
        /// </summary>
        /// <param name="customerId">顧客id</param>
        /// <param name="mContacts">出力されるOrderのリスト</param>
        /// <returns></returns>
        public static ReturnOfBasicDatabase GetDatabaseContacts(int customerId, ref List<MContacts> mContacts)
        {
            DBAccess dba = new DBAccess();
            DataTable dt = null;

            string sql = $"select * from Contacts where customerId={customerId} order by createdAt desc;";
            try
            {
                dba.Query(sql, ref dt);
                for (int row = 0; row < dt.Rows.Count; row++)
                {
                    MContacts aContact = new MContacts();
                    aContact.Id = dt.Rows[row].Field<int>("id");
                    aContact.RandomId = dt.Rows[row].Field<string>("randomId");
                    aContact.CustomerId = dt.Rows[row].Field<int>("customerId");
                    aContact.CreatedAt = dt.Rows[row].Field<DateTime>("createdAt");
                    aContact.CategoryId = dt.Rows[row].Field<int>("categoryId");
                    aContact.Message = dt.Rows[row].Field<string>("message");
                    aContact.ReplyMessage = dt.Rows[row].Field<string>("replyMessage");
                    aContact.ReplyDate = dt.Rows[row].Field<DateTime?>("replyDate");

                    mContacts.Add(aContact);
                }
                return ReturnOfBasicDatabase.Success;
            }
            catch (Exception)
            {
                return ReturnOfBasicDatabase.Error;
            }
        }

        /// <summary>
        /// randomIdからContactモデルを取得
        /// </summary>
        /// <param name="randomId">ランダムid</param>
        /// <param name="mContact">出力されるContactモデルオブジェクト</param>
        /// <returns></returns>
        public static ReturnOfBasicDatabase GetDatabaseContact(string randomId, ref MContacts mContact)
        {
            int? contactId = CommonModel.GetDatabaseRandomIdToContactId(randomId);
            if (contactId == null)
            {
                return ReturnOfBasicDatabase.Error;
            }

            DBAccess dba = new DBAccess();
            DataTable dt = null;
            string sql = $"select * from Contacts where id = {contactId};";
            try
            {
                dba.Query(sql, ref dt);
                mContact = new MContacts()
                {
                    Id = dt.Rows[0].Field<int>("id"),
                    RandomId = dt.Rows[0].Field<string>("randomId"),
                    CustomerId = dt.Rows[0].Field<int>("customerId"),
                    CreatedAt = dt.Rows[0].Field<DateTime>("createdAt"),
                    CategoryId = dt.Rows[0].Field<int>("categoryId"),
                    Message = dt.Rows[0].Field<string>("message"),
                    ReplyMessage = dt.Rows[0].Field<string>("replyMessage"),
                    ReplyDate = dt.Rows[0].Field<DateTime?>("replyDate")
                };
                return ReturnOfBasicDatabase.Success;
            }
            catch (Exception)
            {
                return ReturnOfBasicDatabase.Error;
            }

        }

        /// <summary>
        /// 当時の顧客情報を取得
        /// </summary>
        /// <param name="custId">顧客id</param>
        /// <param name="dateTime">時刻(指定した時間より前の最新のユーザ情報をとってくる)</param>
        /// <param name="mCustomer">出力される顧客情報</param>
        /// <returns></returns>
        public static ReturnOfBasicDatabase GetDatabaseCustomer(int custId, DateTime dateTime, ref MCustomers mCustomer)
        {
            DBAccess dba = new DBAccess();
            DataTable dt = null;
            string sql = $"select Top(1) * from Customers where id = {custId} and createdAt <= '{dateTime.ToString("yyyy-MM-dd HH:mm:ss.fff")}' order by createdAt desc;";
            try
            {
                dba.Query(sql, ref dt);
                mCustomer = new MCustomers()
                {
                    Id = dt.Rows[0].Field<int>("id"),
                    CreatedAt = dt.Rows[0].Field<DateTime>("createdAt"),
                    Name = dt.Rows[0].Field<string>("name"),
                    Furigana = dt.Rows[0].Field<string>("furigana"),
                    Tel = dt.Rows[0].Field<string>("tel"),
                    Mail = dt.Rows[0].Field<string>("mail"),
                    Postcode = dt.Rows[0].Field<string>("postcode"),
                    Address = dt.Rows[0].Field<string>("address"),
                    Password = dt.Rows[0].Field<string>("password"),
                };
                return ReturnOfBasicDatabase.Success;
            }
            catch (Exception)
            {
                return ReturnOfBasicDatabase.Error;
            }
        }

        /// <summary>
        /// 取り寄せ情報をデータベースに登録
        /// </summary>
        /// <param name="CurrentSession">現在のセッション情報</param>
        /// <param name="backorderModel">登録する取り寄せ情報</param>
        /// <param name="randomId">出力されるランダムid</param>
        /// <returns></returns>
        public static ReturnOfBasicDatabase RegistDatabaseBackorder(SessionModel CurrentSession, BackorderModel backorderModel, ref string randomId)
        {
            DBAccess dba = new DBAccess();
            DataTable dt = null;

            string sql = $"insert into Backorders(randomId, customerId, createdAt, categoryId, goodsName) values(REPLACE(CAST(NEWID() AS NCHAR(36)), '-', ''), {CurrentSession.customerUserInfo.Id}, GETDATE(), {backorderModel.CategoryId}, '{backorderModel.GoodsName}'); select @@IDENTITY;";

            try
            {
                dba.Query(sql, ref dt);
                int id = int.Parse(dt.Rows[0].ItemArray[0].ToString());
                sql = $"select randomId from Backorders where id = {id}";
                dba.Query(sql, ref dt);
                randomId = dt.Rows[0].ItemArray[0].ToString();
                return ReturnOfBasicDatabase.Success;
            }
            catch (Exception)
            {
                return ReturnOfBasicDatabase.Error;
            }
        }

        /// <summary>
        /// BackOrdersテーブルを取得
        /// </summary>
        /// <param name="customerId">顧客id</param>
        /// <param name="mBackOrders]">出力されるBackOrderのリスト</param>
        /// <returns></returns>
        public static ReturnOfBasicDatabase GetDatabaseBackorders(int customerId, ref List<MBackOrders> mBackOrders)
        {
            DBAccess dba = new DBAccess();
            DataTable dt = null;

            string sql = $"select * from Backorders where customerId={customerId} order by createdAt desc;";
            try
            {
                dba.Query(sql, ref dt);
                for (int row = 0; row < dt.Rows.Count; row++)
                {
                    MBackOrders aBackOrder = new MBackOrders()
                    {
                        Id = dt.Rows[row].Field<int>("id"),
                        RandomId=dt.Rows[row].Field<string>("randomId"),
                        CustomerId= dt.Rows[row].Field<int>("customerId"),
                        CreatedAt = dt.Rows[row].Field<DateTime>("createdAt"),
                        CategoryId = dt.Rows[row].Field<int>("categoryId"),
                        GoodsName = dt.Rows[row].Field<string>("goodsName"),
                    };
                    mBackOrders.Add(aBackOrder);

                }
                return ReturnOfBasicDatabase.Success;
            }
            catch (Exception)
            {
                return ReturnOfBasicDatabase.Error;
            }
        }

        /// <summary>
        /// randomIdからBackOrderモデルを取得
        /// </summary>
        /// <param name="randomId"></param>
        /// <param name="mBackOrder"></param>
        /// <returns></returns>
        public static ReturnOfBasicDatabase GetDatabaseBackorder(string randomId, ref MBackOrders mBackOrder)
        {
            int? contactId = CommonModel.GetDatabaseRandomIdToBackOrderId(randomId);
            if (contactId == null)
            {
                return ReturnOfBasicDatabase.Error;
            }

            DBAccess dba = new DBAccess();
            DataTable dt = null;
            string sql = $"select * from Backorders where id = {contactId};";
            try
            {
                dba.Query(sql, ref dt);
                mBackOrder = new MBackOrders()
                {
                    Id = dt.Rows[0].Field<int>("id"),
                    RandomId = dt.Rows[0].Field<string>("randomId"),
                    CustomerId = dt.Rows[0].Field<int>("customerId"),
                    CreatedAt = dt.Rows[0].Field<DateTime>("createdAt"),
                    CategoryId = dt.Rows[0].Field<int>("categoryId"),
                    GoodsName = dt.Rows[0].Field<string>("goodsName"),
                };
                return ReturnOfBasicDatabase.Success;
            }
            catch (Exception)
            {
                return ReturnOfBasicDatabase.Error;
            }

        }


    }
}