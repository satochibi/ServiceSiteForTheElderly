using ServiceSiteForTheElderly.Models.ViewModels;
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
            string sql = $"insert into Customers (name, furigana, tel, mail, postcode, address, password) values('{cust.Name}', '{cust.Furigana}', '{cust.Tel}', '{cust.Mail}', '{cust.Postcode}', '{cust.Address}', '{cust.Password}'); select @@IDENTITY;";
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
            dba.Execute($"create view LatestGoodsPriceTrends as (select Latest.goodsId, Latest.priceChangeDate, NULLIF(Latest.variety, '') as variety, price from(select goodsId, MAX(priceChangeDate) as priceChangeDate, isnull(variety, '') as variety from GoodsPriceTrends where priceChangeDate <= GETDATE() group by goodsId, variety) as Latest left join(select goodsId, priceChangeDate, isnull(variety, '') as variety, price from GoodsPriceTrends) as Trends on Latest.goodsId = Trends.goodsId and Latest.priceChangeDate = Trends.priceChangeDate and Latest.variety = Trends.variety);");
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
        /// Ordersテーブルを取得
        /// </summary>
        /// <returns></returns>
        public static ReturnOfBasicDatabase GetDatabaseOrders(SessionModel CurrentSession, ref List<MOrders> mOrders)
        {
            DBAccess dba = new DBAccess();
            DataTable dt = null;

            string sql = $"select * from Orders where customerId={CurrentSession.customerUserInfo.Id} order by orderDate desc;";
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

    }
}