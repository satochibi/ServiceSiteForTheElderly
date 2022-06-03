using ServiceSiteForTheElderly.Models.Common;
using ServiceSiteForTheElderly.Models.ViewModels;
using System;
using System.Collections.Generic;
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

            // 画面右上のボタン
            if (CurrentSession?.customerUserInfo == null)
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
            if (CurrentSession != null)
            {
                IndexMakeView();
                return View("Index");
            }


            return View();
        }

        /// <summary>
        /// ログインのREST API
        /// </summary>
        /// <param name="postModel">電話番号とパスワードから構成されるJsonから生成されたオブジェクト</param>
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
                    if (CurrentSession == null)
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
            if (CurrentSession != null)
            {
                IndexMakeView();
                return View("Index");
            }

            return View();
        }

        /// <summary>
        /// 新規登録のREST API
        /// </summary>
        /// <param name="postModel">新規登録に必要な項目から構成されるJsonから生成されたオブジェクト</param>
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
                if (CurrentSession == null)
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
        /// <param name="postModel">カートに入れるに必要な項目から構成されるJsonから生成されたオブジェクト</param>
        /// <returns>結果をstatusで返す。<c>success</c>なら成功。</returns>
        [HttpPost]
        public ActionResult AddToCart(CartModel postModel)
        {
            string sid = null;
            SessionModel CurrentSession = null;
            GetAndSetSession(Session, ViewData, Url, ref sid, ref CurrentSession);

            if (CurrentSession.cartModelInfo == null)
            {
                CurrentSession.cartModelInfo = new List<CartModel>();

            }

            CurrentSession.cartModelInfo.Add(postModel);

            Session["CurrentSession"] = CurrentSession;

            return Json(new MJsonWithStatus() { status = "success" });
        }

        
        public ActionResult ShoppingCart()
        {
            string sid = null;
            SessionModel CurrentSession = null;
            GetAndSetSession(Session, ViewData, Url, ref sid, ref CurrentSession);


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
            // idがなければ、店舗一覧を表示
            if (string.IsNullOrEmpty(paramArg1))
            {
                BentoShopsList(ViewData);
                return View();

            }
            else
            {
                int shopId = int.Parse(paramArg1);
                List<MGoods> mGoods = new List<MGoods>();
                CommonModel.GetDataBaseGoodsOfShop(shopId, ref mGoods);

                string categoryName = "お弁当";
                ViewData["categoryName"] = categoryName;

                string html = "";
                foreach (var aGoods in mGoods)
                {
                    string aPicture = Url.Content($"~/GoodsPictures/{aGoods.Picture}");


                    string priceHtml = "";
                    if (aGoods.Price.Count == 1)
                    {
                        priceHtml += string.Format(@"<div class=""clearfix menu-list"">
                            <label for=""variety"">{1}円<span class=""tax-text"">(税込)</span></label>
                            <select id =""variety"" name=""variety"">
                                <option value =""one"">1個</option>
                                <option value =""two"">2個</option>
                                <option value =""three"">3個</option>
                                <option value =""four"">4個</option>
                                <option value =""five"">5個</option>
                                <option value =""six"">6個</option>
                                <option value =""seven"">7個</option>
                                <option value =""eight"">8個</option>
                                <option value =""nine"">9個</option>
                                <option value =""ten"">10個</option>
                            </select>
                        </div>", aGoods.Price[0].Variety, aGoods.Price[0].Price);
                    }
                    else if (aGoods.Price.Count >= 2)
                    {
                        foreach (var price in aGoods.Price)
                        {
                            priceHtml += string.Format(@"<div class=""clearfix menu-list"">
                            <label for=""{0}"">{0} {1}円<span class=""tax-text"">(税込)</span></label>
                            <select id =""{0}"" name=""{0}"">
                                <option value =""zero"">0個</option>
                                <option value =""one"">1個</option>
                                <option value =""two"">2個</option>
                                <option value =""three"">3個</option>
                                <option value =""four"">4個</option>
                                <option value =""five"">5個</option>
                                <option value =""six"">6個</option>
                                <option value =""seven"">7個</option>
                                <option value =""eight"">8個</option>
                                <option value =""nine"">9個</option>
                                <option value =""ten"">10個</option>
                            </select>
                        </div>", price.Variety, price.Price);

                        }
                    }


                    html += string.Format(@"
                        <div class=""item-cell"">
                            <div class=""item-header"">
                                <h3>{4}</h3>
                                <span>{5}</span>
                            </div>

                            <div class=""item-body"">
                                <img src = ""{2}"" alt=""{0}"">
                                <h4>{0}</h4>
                                <hr>
                                <form>
                                    {3}

                                    <div class=""detail"">
                                        {1}
                                    </div>

                                    <button type = ""submit"">
                                        <i class=""cart-icon fas fa-cart-arrow-down""></i>カゴに入れる
                                    </button>
                                </form>
                            </div>
                        </div>", aGoods.Name, aGoods.Description, aPicture, priceHtml, aGoods.ShopName, aGoods.ShopGenre);
                }
                ViewData["goods"] = html;
                return View("Magazine");
            }



        }


        public void BentoShopsList(ViewDataDictionary ViewData)
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
            ViewData["categoryName"] = categoryName;

            int categoryId = CommonModel.GetDataBaseCategotyId(categoryName);
            CommonModel.GetDataBaseGoodsOfCategory(categoryId, ref mGoods);

            string html = "";

            foreach (var aGoods in mGoods)
            {
                string aPicture = Url.Content($"~/GoodsPictures/{aGoods.Picture}");

                string priceHtml = "";
                if (aGoods.Price.Count == 1)
                {
                    priceHtml += string.Format(@"<div class=""clearfix menu-list"">
                            <label for=""variety"">{1}円<span class=""tax-text"">(税込)</span></label>
                            <select id =""variety"" name=""variety"">
                                <option value =""one"">1個</option>
                                <option value =""two"">2個</option>
                                <option value =""three"">3個</option>
                                <option value =""four"">4個</option>
                                <option value =""five"">5個</option>
                                <option value =""six"">6個</option>
                                <option value =""seven"">7個</option>
                                <option value =""eight"">8個</option>
                                <option value =""nine"">9個</option>
                                <option value =""ten"">10個</option>
                            </select>
                        </div>", aGoods.Price[0].Variety, aGoods.Price[0].Price);
                }
                else if (aGoods.Price.Count >= 2)
                {
                    foreach (var price in aGoods.Price)
                    {
                        priceHtml += string.Format(@"<div class=""clearfix menu-list"">
                            <label for=""{0}"">{0} {1}円<span class=""tax-text"">(税込)</span></label>
                            <select id =""{0}"" name=""{0}"">
                                <option value =""zero"">0個</option>
                                <option value =""one"">1個</option>
                                <option value =""two"">2個</option>
                                <option value =""three"">3個</option>
                                <option value =""four"">4個</option>
                                <option value =""five"">5個</option>
                                <option value =""six"">6個</option>
                                <option value =""seven"">7個</option>
                                <option value =""eight"">8個</option>
                                <option value =""nine"">9個</option>
                                <option value =""ten"">10個</option>
                            </select>
                        </div>", price.Variety, price.Price);

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
                        <form>
                            {3}

                            <div class=""detail"">
                                {1}
                            </div>

                            <button class=""cart-button"" type=""submit"">
                                <i class=""cart-icon fas fa-cart-arrow-down""></i>カゴに入れる
                            </button>
                        </form>
                    </div>
                </div>", aGoods.Name, aGoods.Description, aPicture, priceHtml, aGoods.ShopName, aGoods.ShopGenre);
            }

            ViewData["goods"] = html;


            return View();
        }
    }
}