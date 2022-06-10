using ServiceSiteForTheElderly.Models.Common;
using ServiceSiteForTheElderly.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ServiceSiteForTheElderly.Controllers
{
    public class HomeController : Controller
    {
        /// <summary>
        /// セッション管理メソッド
        /// </summary>
        /// <param name="Session">セッションオブジェクト(Sessionを代入すること)</param>
        /// <param name="ViewData">ビューデータオブジェクト(ViewDataを代入すること)</param>
        /// <param name="Url">Urlヘルパーオブジェクト(Urlを代入すること)</param>
        /// <param name="sid">返されるセッションID</param>
        /// <param name="CurrentSession">返されるカレントセッション</param>
        public static void GetAndSetSession(HttpSessionStateBase Session, ViewDataDictionary ViewData, UrlHelper Url, ref string sid, ref SessionModel CurrentSession)
        {
            sid = System.Web.HttpContext.Current.Session.SessionID;
            if (Session["CurrentSessionID"] == null)
            {
                Session["CurrentSessionID"] = sid;
            }
            else
            {
                sid = Session["CurrentSessionID"] as string;
            }


            CurrentSession = null;
            if (Session["CurrentSession"] != null)
            {
                CurrentSession = Session["CurrentSession"] as SessionModel;
            }
            else
            {
                CurrentSession = new SessionModel();
                Session["CurrentSession"] = CurrentSession;
            }


            // 画面右上のボタン
            if (CurrentSession.customerUserInfo == null)
            {
                ViewData["HeaderButtonText"] = "会員の方はこちら";
                ViewData["HeaderButtonLink"] = Url.Action("Login", "Home");
            }
            else
            {
                ViewData["HeaderButtonText"] = "マイページ";
                ViewData["HeaderButtonLink"] = Url.Action("MyPageOrder", "Home");

            }

        }

        /// <summary>
        /// トップページ
        /// </summary>
        /// <returns>トップページのビュー</returns>
        public ActionResult Index()
        {
            // 右上のボタンを表示するために下記のコードは絶対必要
            string sid = null;
            SessionModel CurrentSession = null;
            GetAndSetSession(Session, ViewData, Url, ref sid, ref CurrentSession);

            IndexMakeView();

            return View();
        }

        /// <summary>
        /// データベースからトップページのボタンを生成する
        /// </summary>
        void IndexMakeView()
        {
            // 宅配配送サービスのボタンたち
            List<MCategores> mCategores = new List<MCategores>();
            CommonModel.GetDatabaseCategoriesWithoutContact(ref mCategores);

            string html = "";

            foreach (var aCategory in mCategores)
            {
                html += $"<a href=\"{@Url.Action(aCategory.Link, "Home")}\" class=\"btn-flat\"><span>{aCategory.Name.ToString()}<i class=\"fas fa-chevron-right\"></i></span></a>" + Environment.NewLine;

            }

            ViewData["categories"] = html;

            mCategores.Clear();

            // その他のサービスのボタンたち
            CommonModel.GetDatabaseCategoriesWithContact(ref mCategores);

            html = "";

            foreach (var aCategory in mCategores)
            {
                html += $"<a href=\"{@Url.Action(aCategory.Link, "Home")}\" class=\"btn-flat\"><span>{aCategory.Name.ToString()}<i class=\"fas fa-chevron-right\"></i></span></a>" + Environment.NewLine;

            }

            ViewData["contacts"] = html;
        }

        /// <summary>
        /// ログイン画面
        /// </summary>
        /// <returns>ログイン画面のビュー</returns>
        [HttpGet]
        public ActionResult Login()
        {
            string sid = null;
            SessionModel CurrentSession = null;
            GetAndSetSession(Session, ViewData, Url, ref sid, ref CurrentSession);

            // 既にログイン済みならトップページにリダイレクト
            if (CurrentSession.customerUserInfo != null)
            {
                IndexMakeView();
                return View("Index");
            }


            return View();
        }

        /// <summary>
        /// ログインのREST API
        /// </summary>
        /// <param name="postModel">電話番号とパスワードから構成されるJsonから生成されたオブジェクト
        /// <example>
        /// {
        ///    "Tel": "000-0000-0000",
        ///    "Password": "asdf"
        /// }
        /// </example>
        /// </param>
        /// <returns>結果をstatusで返す。<c>success</c>ならログイン成功。<c>wrongUserId</c>ならユーザIDが間違い。<c>wrongPassword</c>ならパスワード間違い。<c>error</c>ならエラー。</returns>
        [HttpPost]
        public ActionResult Login(LoginModel postModel)
        {
            string sid = null;
            SessionModel CurrentSession = null;
            GetAndSetSession(Session, ViewData, Url, ref sid, ref CurrentSession);



            MCustomers cust = new MCustomers();
            int hitCount = 0;

            postModel.Tel = postModel.Tel.Replace("-", "");

            ReturnOfCheckDatabaseLogin rtn = CommonModel.CheckDatabaseLogin(postModel.Tel, postModel.Password, ref cust, ref hitCount);
            switch (rtn)
            {
                case ReturnOfCheckDatabaseLogin.Success:
                    if (CurrentSession.customerUserInfo == null)
                    {
                        // 未ログインならセッションに顧客情報を保持しておく
                        CurrentSession = new SessionModel();
                        CurrentSession.customerUserInfo = cust;
                        Session["CurrentSession"] = CurrentSession;
                    }
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

        /// <summary>
        /// ログアウト画面(実際に使うかどうかは未定)
        /// </summary>
        /// <returns>ログアウト画面のビュー</returns>
        public ActionResult Logout()
        {
            string sid = null;
            SessionModel CurrentSession = null;
            GetAndSetSession(Session, ViewData, Url, ref sid, ref CurrentSession);

            // セッションのログイン情報を捨てる
            CurrentSession = null;
            Session["CurrentSession"] = null;

            return View();
        }

        /// <summary>
        /// 新規登録画面
        /// </summary>
        /// <returns>新規登録画面のビュー</returns>
        [HttpGet]
        public ActionResult SignUp()
        {
            string sid = null;
            SessionModel CurrentSession = null;
            GetAndSetSession(Session, ViewData, Url, ref sid, ref CurrentSession);

            // 既にログイン済みならトップページにリダイレクト
            if (CurrentSession.customerUserInfo != null)
            {
                IndexMakeView();
                return View("Index");
            }

            return View();
        }

        /// <summary>
        /// 新規登録のREST API
        /// </summary>
        /// <param name="postModel">新規登録に必要な項目から構成されるJsonから生成されたオブジェクト
        /// <example>
        /// {
        ///    "Name": "イーブイ",
        ///    "Furigana": "いーぶい",
        ///    "Tel": "00000000000",
        ///    "Mail": "eevee@pokemon.com",
        ///    "Postcode": "7770000",
        ///    "Address": "カントーのどこか",
        ///    "Password": "pass"
        /// }
        /// </example>
        /// </param>
        /// <returns>結果をstatusで返す。<c>success</c>なら成功。<c>containEmptyChar</c>なら未入力がある。<c>duplicateTelError</c>なら既にその電話番号で登録済みのユーザがいる。</returns>
        [HttpPost]
        public ActionResult SignUp(SignUpModel postModel)
        {
            string sid = null;
            SessionModel CurrentSession = null;
            GetAndSetSession(Session, ViewData, Url, ref sid, ref CurrentSession);


            // 事前に電話番号と郵便番号のハイフンを取り除いておく
            postModel.Tel = postModel.Tel.Replace("-", "");
            postModel.Postcode = postModel.Postcode.Replace("-", "");

            // ユーザが既に存在するかの判定
            if (CommonModel.CheckDatabaseIsUserIdExist(postModel.Tel) == ReturnOfCheckDatabaseIsUserIdExist.UserIdIsNotExist)
            {
                // 存在しなかったら、登録処理

                if (string.IsNullOrEmpty(postModel.Name) || string.IsNullOrEmpty(postModel.Furigana) || string.IsNullOrEmpty(postModel.Tel) || string.IsNullOrEmpty(postModel.Mail) || string.IsNullOrEmpty(postModel.Postcode) || string.IsNullOrEmpty(postModel.Address) || string.IsNullOrEmpty(postModel.Password))
                {
                    // 未入力があるかどうかチェック
                    return Json(new MJsonWithStatus() { status = "containEmptyChar" });
                }

                // 顧客情報を作ってデータベースに登録
                MCustomers cust = new MCustomers() { Name = postModel.Name, Furigana = postModel.Furigana, Tel = postModel.Tel, Mail = postModel.Mail, Postcode = postModel.Postcode, Address = postModel.Address, Password = postModel.Password };

                CommonModel.RegistDatabaseCustomer(cust);

                // 未ログインならログインしておく(新規登録からの自動的なログイン)
                if (CurrentSession.customerUserInfo == null)
                {
                    CurrentSession = new SessionModel();
                    CurrentSession.customerUserInfo = cust;
                    Session["CurrentSession"] = CurrentSession;
                }
                return Json(new MJsonWithStatus() { status = "success" });
            }
            else
            {
                return Json(new MJsonWithStatus() { status = "duplicateTelError" });
            }
        }

        /// <summary>
        /// カートに入れるのREST API
        /// </summary>
        /// <param name="postModelList">カートに入れるに必要な項目から構成されるJsonから生成されたオブジェクト
        /// <example>
        /// [
        ///    {
        ///        "GoodsId":3,
        ///        "Variety":"",
        ///        "Quantity": 3
        ///    },
        ///    {
        ///        "GoodsId":3,
        ///        "Variety":"",
        ///        "Quantity": 3
        ///    }...
        /// ]
        /// </example>
        /// </param>
        /// <returns>結果をstatusで返す。<c>success</c>なら成功。</returns>
        [HttpPost]
        public ActionResult AddToCart(List<CartModel> postModelList)
        {
            string sid = null;
            SessionModel CurrentSession = null;
            GetAndSetSession(Session, ViewData, Url, ref sid, ref CurrentSession);

            if (CurrentSession.cartModelInfo == null)
            {
                CurrentSession.cartModelInfo = new List<CartModel>();

            }


            // 商品を入れる
            foreach (var postModel in postModelList)
            {
                CurrentSession.cartModelInfo.Add(postModel);
            }

            // https://paiza.io/projects/WUXuluJbpntGJrTOFsvmOg

            // 量が0であるものを除外する
            CurrentSession.cartModelInfo.RemoveAll(item => item.Quantity == 0);

            // 同じgoodId同士をまとめる
            var query = CurrentSession.cartModelInfo.GroupBy(item => new { GoodsId = item.GoodsId, Variety = item.Variety })
            .Select(item => new {
                GoodsId = item.Key.GoodsId,
                Variety = item.Key.Variety,
                Quantity = item.Sum(s => s.Quantity)
            });

            // クエリを実行してリストを更新
            List<CartModel> newList = new List<CartModel>();
            foreach (var item in query.ToList())
            {
                newList.Add(new CartModel() { GoodsId = item.GoodsId, Variety = item.Variety, Quantity = item.Quantity });
            }
            CurrentSession.cartModelInfo = newList;


            Session["CurrentSession"] = CurrentSession;

            return Json(new MJsonWithStatus() { status = "success" });
        }

        /// <summary>
        /// カゴの中の画面
        /// </summary>
        /// <returns>カゴの中の画面のビュー</returns>
        public ActionResult ShoppingCart()
        {
            string sid = null;
            SessionModel CurrentSession = null;
            GetAndSetSession(Session, ViewData, Url, ref sid, ref CurrentSession);

            if (CurrentSession.cartModelInfo == null)
            {
                CurrentSession.cartModelInfo = new List<CartModel>();
            }

            // カートに商品があれば
            if (CurrentSession.cartModelInfo != null && CurrentSession.cartModelInfo.Count > 0)
            {
                foreach (var item in CurrentSession.cartModelInfo)
                {
                    Console.WriteLine(item);
                }

            }
            return View();
        }

        /// <summary>
        /// お弁当の画面
        /// </summary>
        /// <returns>お弁当のビュー</returns>
        public ActionResult BentoShops()
        {
            string sid = null;
            SessionModel CurrentSession = null;
            GetAndSetSession(Session, ViewData, Url, ref sid, ref CurrentSession);

            string paramArg1 = Request.Params["id"];
            
            if (string.IsNullOrEmpty(paramArg1))
            {
                // idがなければ、店舗一覧を表示
                BentoShopsListMakeView();
                return View();

            }
            else
            {
                // idがあれば、その店の商品画面へ
                int shopId = int.Parse(paramArg1);
                List<MGoods> mGoods = new List<MGoods>();
                CommonModel.GetDataBaseGoodsOfShop(shopId, ref mGoods);

                string categoryName = "お弁当";
                ViewData["categoryName"] = categoryName;
                string unitName = "個";

                GoodsMakeView(mGoods, unitName);
                return View("Goods");
            }



        }

        /// <summary>
        /// 弁当の店舗一覧のhtmlを生成する
        /// </summary>
        public void BentoShopsListMakeView()
        {
            List<MShops> shops = new List<MShops>();

            CommonModel.GetDataBaseShopsOfBento(ref shops);

            string html = "";
            string css = "";

            foreach (var aShop in shops)
            {
                html += string.Format(@"<a href=""?id={0}"" class=""btn-flat shops-button-{0}""><span>{1}</span></a>" + Environment.NewLine, aShop.Id, aShop.DisplayName);
                string image = @Url.Content($"~/ShopPictures/{aShop.Picture}");
                image = "\"" + image + "\"";

                css += string.Format(@"
                    .shops-button-{0}{{
                        background-image: url({1}) !important;
                    }}
                ", aShop.Id, image);
            }

            ViewData["shops"] = html;
            ViewData["css"] = css;
        }

        /// <summary>
        /// 本の画面
        /// </summary>
        /// <returns>本の画面のビュー</returns>
        public ActionResult Magazine()
        {
            string sid = null;
            SessionModel CurrentSession = null;
            GetAndSetSession(Session, ViewData, Url, ref sid, ref CurrentSession);

            List<MGoods> mGoods = new List<MGoods>();

            string categoryName = "本";
            string unitName = "冊";
            ViewData["categoryName"] = categoryName;

            int categoryId = CommonModel.GetDataBaseCategotyId(categoryName);
            CommonModel.GetDataBaseGoodsOfCategory(categoryId, ref mGoods);

            GoodsMakeView(mGoods, unitName);


            return View("Goods");
        }

        /// <summary>
        /// 商品一覧のhtmlを生成する
        /// </summary>
        /// <param name="mGoods">商品のリスト</param>
        /// <param name="unitName">商品の個数の単位</param>
        void GoodsMakeView(List<MGoods> mGoods, string unitName)
        {
            string html = "";

            foreach (var aGoods in mGoods)
            {
                string aPicture = Url.Content($"~/GoodsPictures/{aGoods.Picture}");

                string priceHtml = "";
                if (aGoods.Price.Count == 1)
                {
                    priceHtml += string.Format(@"<div class=""clearfix menu-list"">
                            <label for=""variety"">{1}円<span class=""tax-text"">(税込)</span></label>
                            <select id=""variety"" name=""variety"">
                                <option value =""1"">1{2}</option>
                                <option value =""2"">2{2}</option>
                                <option value =""3"">3{2}</option>
                                <option value =""4"">4{2}</option>
                                <option value =""5"">5{2}</option>
                                <option value =""6"">6{2}</option>
                                <option value =""7"">7{2}</option>
                                <option value =""8"">8{2}</option>
                                <option value =""9"">9{2}</option>
                            </select>
                        </div>", aGoods.Price[0].Variety, aGoods.Price[0].Price, unitName);
                }
                else if (aGoods.Price.Count >= 2)
                {
                    foreach (var price in aGoods.Price)
                    {
                        priceHtml += string.Format(@"<div class=""clearfix menu-list"">
                            <label for=""{0}"">{0} {1}円<span class=""tax-text"">(税込)</span></label>
                            <select id=""{0}"" name=""{0}"">
                                <option value =""0"">0{2}</option>
                                <option value =""1"">1{2}</option>
                                <option value =""2"">2{2}</option>
                                <option value =""3"">3{2}</option>
                                <option value =""4"">4{2}</option>
                                <option value =""5"">5{2}</option>
                                <option value =""6"">6{2}</option>
                                <option value =""7"">7{2}</option>
                                <option value =""8"">8{2}</option>
                                <option value =""9"">9{2}</option>
                            </select>
                        </div>", price.Variety, price.Price, unitName);

                    }
                }



                html += string.Format(@"
                    <div class=""item-cell"">
                        <div class=""item-header"">
                            <h3>{4}</h3>
                            <span>{5}</span>
                        </div>

                    <div class=""item-body"">
                        <img src=""{2}"" alt=""{0}"">
                        <h4>{0}</h4>
                        <hr>
                        <form id=""goods-{6}"" class=""goods"">
                            {3}

                            <div class=""detail"">
                                {1}
                            </div>

                            <button class=""cart-button"" type=""submit"">
                                <i class=""cart-icon fas fa-cart-arrow-down""></i>カゴに入れる
                            </button>
                        </form>
                    </div>
                </div>", aGoods.Name, aGoods.Description, aPicture, priceHtml, aGoods.ShopName, aGoods.ShopGenre, aGoods.Id);
            }

            ViewData["goods"] = html;
        }


        
    }
}