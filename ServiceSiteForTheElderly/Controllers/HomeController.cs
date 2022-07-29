using ServiceSiteForTheElderly.Models.Common;
using ServiceSiteForTheElderly.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;

namespace ServiceSiteForTheElderly.Controllers
{
    public class HomeController : Controller
    {
        const string mentainanceTitle = "メンテナンス中";
        const string mentainanceMessage = "しばらく経ってから再度アクセスしなおして下さい";

        /// <summary>
        /// メールを送る関数
        /// 参考にしたコード: https://dobon.net/vb/dotnet/internet/smtpclient.html
        /// Gmailでの設定: https://work-note32.com/vb-smtp-gmail-send
        /// </summary>
        /// <example>
        ///     <code>
        ///         MailSend("smtp.gmail.com", "satochibibu@gmail.com", "satochibibu@gmail.com","test", "testbody");
        ///     </code>
        /// </example>
        /// <param name="smtpServer">SMTPサーバ</param>
        /// <param name="senderMail">差出人のメールアドレス</param>
        /// <param name="recipientMail">宛先のメールアドレス</param>
        /// <param name="subject">件名</param>
        /// <param name="body">メール本文</param>
        public static void MailSend(string smtpServer, string senderMail, string recipientMail, string subject, string body)
        {
            SmtpClient sc = new SmtpClient() { Host = smtpServer, Port = 587, DeliveryMethod = SmtpDeliveryMethod.Network, Credentials = new System.Net.NetworkCredential(senderMail, "qhtdcoppzgkkigfv"), EnableSsl = true };
            sc.Send(senderMail, recipientMail, subject, body);
            sc.Dispose();
        }

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
            // セッションIDを保持
            sid = System.Web.HttpContext.Current.Session.SessionID;
            if (Session["CurrentSessionID"] == null)
            {
                Session["CurrentSessionID"] = sid;
            }
            else
            {
                sid = Session["CurrentSessionID"] as string;
            }

            // セッションの本体を保持
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


            if (CurrentSession.cartModelInfo == null)
            {
                CurrentSession.cartModelInfo = new List<CartModel>();
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

            //カート個数バッジ
            int count = CurrentSession.cartModelInfo.Select(x => x.Quantity).Sum();
            ViewData["HeaderBadge"] = (count == 0) ? "" : count.ToString();
            ViewData["HeaderBadgeStatus"] = (count == 0) ? "disabled" : "";

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

            // メンテナンス中ページを表示させるために下記のコードは絶対必要
            if (CommonModel.GetDatabaseGlobalStatus() == ReturnOfBasicDatabase.Error)
            {
                ViewData["title"] = mentainanceTitle;
                ViewData["message"] = mentainanceMessage;
                return View("Error");
            }

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

            if (CommonModel.GetDatabaseGlobalStatus() == ReturnOfBasicDatabase.Error)
            {
                ViewData["title"] = mentainanceTitle;
                ViewData["message"] = mentainanceMessage;
                return View("Error");
            }

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

            if (CommonModel.GetDatabaseGlobalStatus() == ReturnOfBasicDatabase.Error)
            {
                ViewData["title"] = mentainanceTitle;
                ViewData["message"] = mentainanceMessage;
                return View("Error");
            }

            // セッションのログイン情報を捨てる
            CurrentSession.customerUserInfo = null;

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

            if (CommonModel.GetDatabaseGlobalStatus() == ReturnOfBasicDatabase.Error)
            {
                ViewData["title"] = mentainanceTitle;
                ViewData["message"] = mentainanceMessage;
                return View("Error");
            }

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
            if (postModel.Postcode != null)
            {
                postModel.Postcode = postModel.Postcode.Replace("-", "");
            }

            // ユーザが既に存在するかの判定
            if (CommonModel.CheckDatabaseIsUserIdExist(postModel.Tel) == ReturnOfCheckDatabaseIsUserIdExist.UserIdIsNotExist)
            {
                // 存在しなかったら、登録処理

                if (string.IsNullOrEmpty(postModel.Name) || string.IsNullOrEmpty(postModel.Furigana) || string.IsNullOrEmpty(postModel.Tel) || string.IsNullOrEmpty(postModel.Password))
                {
                    // 未入力があるかどうかチェック
                    return Json(new MJsonWithStatus() { status = "containEmptyChar" });
                }

                // 顧客情報を作ってデータベースに登録
                MCustomers cust = new MCustomers() { Name = postModel.Name, Furigana = postModel.Furigana, Tel = postModel.Tel, Mail = string.IsNullOrEmpty(postModel.Mail) ? null : postModel.Mail, Postcode = string.IsNullOrEmpty(postModel.Postcode) ? null : postModel.Postcode, Address = string.IsNullOrEmpty(postModel.Address) ? null : postModel.Address, Password = postModel.Password };
                int? custId = null;
                CommonModel.RegistDatabaseCustomer(cust, ref custId);
                if (custId == null)
                {
                    return Json(new MJsonWithStatus() { status = "error" });
                }
                else
                {
                    cust.Id = custId.Value;
                }

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
        /// <returns>結果をstatusで返す。<c>success</c>なら成功。<c>maxQuantityError</c>なら最大数量エラー</returns>
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

            postModelList = postModelList.Select(postModel => new CartModel() { GoodsId = postModel.GoodsId, Variety = string.IsNullOrEmpty(postModel.Variety) ? "" : postModel.Variety, Quantity = postModel.Quantity }).ToList();



            // カートの状態のコピーをとっておく、そのコピーに対して商品を入れる
            var copyCurrentCart = new List<CartModel>(CurrentSession.cartModelInfo);


            // 商品を入れる
            foreach (var postModel in postModelList)
            {
                copyCurrentCart.Add(postModel);
            }

            // https://paiza.io/projects/WUXuluJbpntGJrTOFsvmOg

            // 量が0であるものを除外する
            copyCurrentCart.RemoveAll(item => item.Quantity == 0);

            // 同じgoodId同士をまとめる
            var query = copyCurrentCart.GroupBy(item => new { GoodsId = item.GoodsId, Variety = item.Variety })
            .Select(item => new
            {
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
            copyCurrentCart = newList;



            // 商品をカートに入れる前に、カートに入れた時に、10個以上の商品が入っているかチェック
            bool checkQuantity = copyCurrentCart.All(cartModel => cartModel.Quantity < 10);

            if (checkQuantity)
            {
                CurrentSession.cartModelInfo = copyCurrentCart;
                Session["CurrentSession"] = CurrentSession;
                return Json(new MJsonWithStatus() { status = "success" });
            }
            else
            {
                // 10個以上ならstatusをerrorにして終了
                return Json(new MJsonWithStatus() { status = "maxQuantityError" });
            }

        }

        /// <summary>
        /// カートの商品の個数を変更するREST API(Deleteは、Quantityを0にする)
        /// </summary>
        /// <param name="postModel">カートに入れるに必要な項目から構成されるJsonから生成されたオブジェクト
        /// <example>
        /// {
        ///     "GoodsId":3,
        ///     "Variety":"",
        ///     "Quantity": 3
        /// }
        /// </example>
        /// </param>
        /// <returns>結果をstatusで返す。<c>success</c>なら成功。</returns>
        [HttpPost]
        public ActionResult ChangeToCart(CartModel postModel)
        {
            string sid = null;
            SessionModel CurrentSession = null;
            GetAndSetSession(Session, ViewData, Url, ref sid, ref CurrentSession);

            string postModelVariety = string.IsNullOrEmpty(postModel.Variety) ? null : postModel.Variety;

            if (postModel.Quantity == 0)
            {
                // 数量が0ならDelete
                if (postModelVariety == null)
                {
                    CurrentSession.cartModelInfo.RemoveAll(item => item.GoodsId == postModel.GoodsId);
                }
                else
                {
                    CurrentSession.cartModelInfo.RemoveAll(item => item.GoodsId == postModel.GoodsId && item.Variety == postModel.Variety);
                }
            }
            else
            {
                // 数量が0より大きいならChange
                if (postModelVariety == null)
                {
                    var selectedItem = CurrentSession.cartModelInfo.Find(item => item.GoodsId == postModel.GoodsId);
                    selectedItem.Quantity = postModel.Quantity;
                }
                else
                {
                    var selectedItem = CurrentSession.cartModelInfo.Find(item => item.GoodsId == postModel.GoodsId && item.Variety == postModel.Variety);
                    selectedItem.Quantity = postModel.Quantity;
                }
            }

            Session["CurrentSession"] = CurrentSession;
            return Json(new MJsonWithStatus() { status = "success" });

        }

        /// <summary>
        /// カートの中身を全部消去する
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult ClearToCart()
        {
            string sid = null;
            SessionModel CurrentSession = null;
            GetAndSetSession(Session, ViewData, Url, ref sid, ref CurrentSession);

            CurrentSession.cartModelInfo.Clear();
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

            if (CommonModel.GetDatabaseGlobalStatus() == ReturnOfBasicDatabase.Error)
            {
                ViewData["title"] = mentainanceTitle;
                ViewData["message"] = mentainanceMessage;
                return View("Error");
            }

            int allTotalPrice = 0;

            // カートに商品があれば
            if (CurrentSession.cartModelInfo != null && CurrentSession.cartModelInfo.Count > 0)
            {
                List<MGoodsOfCart> mGoodsOfCartList = new List<MGoodsOfCart>();
                CommonModel.GetDataBaseGoodsInCart(CurrentSession.cartModelInfo, ref mGoodsOfCartList);

                // 店ごとにグループ化
                var query = mGoodsOfCartList.GroupBy(item => new { ShopName = item.ShopName, ShippingCost = item.ShippingCost });
                string html = "";

                foreach (var group in query)
                {
                    // それぞれの店ごとにテーブルを作成

                    int shopTotalPrice = 0;

                    // テーブルヘッダー
                    html += string.Format(@"
                        <div class=""a-shop"">
                            <h3>{0}</h3>
                            <div class=""cart-table-heading"">
                                <ul>
                                    <li class=""cart-table-body-list-item"">削除</li>
                                    <li class=""cart-table-body-list-item"">商品内容</li>
                                    <li class=""cart-table-body-list-item"">数量</li>
                                    <li class=""cart-table-body-list-item"">小計(税込)</li>
                                </ul>
                            </div>
                        <div class=""cart-table-body"">", group.Key.ShopName);



                    foreach (var item in group)
                    {
                        int totalPrice = item.Price * item.Quantity;
                        string varietyDisplay = string.IsNullOrEmpty(item.Variety) ? "" : $"({item.Variety})";
                        string varietyId = string.IsNullOrEmpty(item.Variety) ? "" : $"-{item.Variety}";

                        string aPicture = string.IsNullOrEmpty(item.Picture) ? Url.Content($"~/GoodsPictures/noimage.png") : Url.Content($"~/GoodsPictures/{item.Picture}");


                        // 選択式セレクトボックスを生成
                        string select = @"<select class=""num"" name=""num"">";
                        for (int index = 1; index <= 9; index++)
                        {
                            if (index == item.Quantity)
                            {
                                select += string.Format(@"<option value=""{0}"" selected>{0}個</option>", index);
                            }
                            else
                            {
                                select += string.Format(@"<option value=""{0}"">{0}個</option>", index);
                            }
                        }
                        select += @"</select>";


                        // テーブルボディー
                        html += string.Format(@"
                            <ul class=""goods"" id=""goods-{6}{7}"">
                                
                                <li class=""cart-table-body-list-item"">
                                    <div class=""trash-icon""><i class=""fas fa-trash-alt""></i></div>
                                </li>
                                <li class=""cart-table-body-list-item shopping-item-column"">
                                    <img src = ""{5}"" alt=""{0}{1}"">
                                    <div>
                                        <h4>{0}{1}</h4>
                                        <p>{2}円</p>
                                    </div>

                                </li>
                                <li class=""cart-table-body-list-item"">
                                    {3}
                                </li>
                                <li class=""cart-table-body-list-item"">
                                    {4}円
                                </li>
                             </ul>", item.Name, varietyDisplay, item.Price, select, totalPrice, aPicture, item.GoodsId, varietyId);

                        shopTotalPrice += totalPrice;
                    }

                    shopTotalPrice += group.Key.ShippingCost;

                    html += string.Format(@"
                        </div>
                        <div class=""cart-table-postage"">
                            <span>送料 {0}円</span>
                        </div>

                        <div class=""cart-table-price"">
                            <span>合計金額 {1}円</span>
                        </div>", group.Key.ShippingCost, shopTotalPrice);

                    // test += $"送料 {group.Key.ShippingCost}円<br>合計金額 {shopTotalPrice}円";
                    allTotalPrice += shopTotalPrice;
                    html += "</div>";

                }


                ViewData["goodsOfCart"] = html;
            }

            ViewData["totalPrice"] = allTotalPrice;

            return View();
        }

        /// <summary>
        /// 送り先アドレスの入力画面
        /// </summary>
        /// <returns>送り先アドレスの入力画面のビュー</returns>
        [HttpGet]
        public ActionResult EnterShippingAddress()
        {
            string sid = null;
            SessionModel CurrentSession = null;
            GetAndSetSession(Session, ViewData, Url, ref sid, ref CurrentSession);

            if (CommonModel.GetDatabaseGlobalStatus() == ReturnOfBasicDatabase.Error)
            {
                ViewData["title"] = mentainanceTitle;
                ViewData["message"] = mentainanceMessage;
                return View("Error");
            }

            // ログインしていなければ、ログイン画面にリダイレクト
            if (CurrentSession.customerUserInfo == null)
            {
                return View("Login");
            }

            // セッション内のカゴに入れている商品が1つもなければ
            // トップページにリダイレクト
            if (CurrentSession.cartModelInfo.Count == 0)
            {
                IndexMakeView();
                return View("Index");
            }

            ViewData["CurrentSession"] = CurrentSession;
            return View();

        }

        /// <summary>
        /// 送り先情報をセッションに保存するためのAPI
        /// </summary>
        /// <param name="postModel">送り先情報から必要な項目から構成されるJsonから生成されたオブジェクト</param>
        /// <example>
        /// {
        ///     "Name": "砥部花子",
        ///     "Furigana": "とべはなこ",
        ///     "Tel": "0000-000-0000",
        ///     "Postcode": "000-0000",
        ///     "Address": "砥部町麻生000-0"
        /// }
        /// </example>
        /// <returns></returns>
        [HttpPost]
        public ActionResult EnterShippingAddress(ShippingAddressModel postModel)
        {
            string sid = null;
            SessionModel CurrentSession = null;
            GetAndSetSession(Session, ViewData, Url, ref sid, ref CurrentSession);

            postModel.Tel = postModel.Tel.Replace("-", "");
            postModel.Postcode = postModel.Postcode.Replace("-", "");

            CurrentSession.shippingAddressInfo = new MShippingAddress()
            {
                Name = postModel.Name,
                Furigana = postModel.Furigana,
                Tel = postModel.Tel,
                Postcode = postModel.Postcode,
                Address = postModel.Address
            };

            return Json(new MJsonWithStatus() { status = "success" });
        }

        /// <summary>
        /// セッション内の送り先情報を削除するためのAPI
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult ClearShippingAddress()
        {
            string sid = null;
            SessionModel CurrentSession = null;
            GetAndSetSession(Session, ViewData, Url, ref sid, ref CurrentSession);

            CurrentSession.shippingAddressInfo = null;

            return Json(new MJsonWithStatus() { status = "success" });

        }


        /// <summary>
        /// 注文最終確認画面
        /// </summary>
        /// <returns></returns>
        public ActionResult ShoppingConfirm()
        {
            string sid = null;
            SessionModel CurrentSession = null;
            GetAndSetSession(Session, ViewData, Url, ref sid, ref CurrentSession);

            if (CommonModel.GetDatabaseGlobalStatus() == ReturnOfBasicDatabase.Error)
            {
                ViewData["title"] = mentainanceTitle;
                ViewData["message"] = mentainanceMessage;
                return View("Error");
            }


            // ログインしていなければ、ログイン画面にリダイレクト
            // 既にログイン済みならトップページにリダイレクト
            if (CurrentSession.customerUserInfo == null)
            {
                return View("Login");
            }

            // セッション内のカゴに入れている商品が1つもなければ
            // トップページにリダイレクト
            if (CurrentSession.cartModelInfo.Count == 0)
            {
                IndexMakeView();
                return View("Index");
            }


            int allTotalPrice = 0;

            // カートに商品があれば
            if (CurrentSession.cartModelInfo != null && CurrentSession.cartModelInfo.Count > 0)
            {
                List<MGoodsOfCart> mGoodsOfCartList = new List<MGoodsOfCart>();
                CommonModel.GetDataBaseGoodsInCart(CurrentSession.cartModelInfo, ref mGoodsOfCartList);

                // 店ごとにグループ化
                var query = mGoodsOfCartList.GroupBy(item => new { ShopName = item.ShopName, ShippingCost = item.ShippingCost });
                string html = "";

                foreach (var group in query)
                {
                    // それぞれの店ごとにテーブルを作成

                    int shopTotalPrice = 0;

                    // テーブルヘッダー
                    html += string.Format(@"
                        <div class=""a-shop"">
                            <h3>{0}</h3>
                            <div class=""cart-table-heading"">
                                <ul>
                                    <li class=""cart-table-body-list-item"">商品内容</li>
                                    <li class=""cart-table-body-list-item"">数量</li>
                                    <li class=""cart-table-body-list-item"">小計(税込)</li>
                                </ul>
                            </div>
                        <div class=""cart-table-body"">", group.Key.ShopName);



                    foreach (var item in group)
                    {
                        int totalPrice = item.Price * item.Quantity;
                        string varietyDisplay = string.IsNullOrEmpty(item.Variety) ? "" : $"({item.Variety})";
                        string varietyId = string.IsNullOrEmpty(item.Variety) ? "" : $"-{item.Variety}";

                        string aPicture = string.IsNullOrEmpty(item.Picture) ? Url.Content($"~/GoodsPictures/noimage.png") : Url.Content($"~/GoodsPictures/{item.Picture}");

                        // テーブルボディー
                        html += string.Format(@"
                            <ul class=""goods"" id=""goods-{6}{7}"">
                                
                                <li class=""cart-table-body-list-item shopping-item-column"">
                                    <img src = ""{5}"" alt=""{0}{1}"">
                                    <div>
                                        <h4>{0}{1}</h4>
                                        <p>{2}円</p>
                                    </div>

                                </li>
                                <li class=""cart-table-body-list-item"">
                                    {3}
                                </li>
                                <li class=""cart-table-body-list-item"">
                                    {4}円
                                </li>
                             </ul>", item.Name, varietyDisplay, item.Price, item.Quantity, totalPrice, aPicture, item.GoodsId, varietyId);

                        shopTotalPrice += totalPrice;
                    }

                    shopTotalPrice += group.Key.ShippingCost;

                    html += string.Format(@"
                        </div>
                        <div class=""cart-table-postage"">
                            <span>送料 {0}円</span>
                        </div>

                        <div class=""cart-table-price"">
                            <span>合計金額 {1}円</span>
                        </div>", group.Key.ShippingCost, shopTotalPrice);

                    allTotalPrice += shopTotalPrice;
                    html += "</div>";

                }


                ViewData["goodsOfCart"] = html;
            }

            ViewData["totalPrice"] = allTotalPrice;

            ViewData["CurrentSession"] = CurrentSession;
            return View();
        }

        /// <summary>
        /// 注文確定時に、カートの中身をデータベースに入れるAPI
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult CartToDatabase()
        {
            string sid = null;
            SessionModel CurrentSession = null;
            GetAndSetSession(Session, ViewData, Url, ref sid, ref CurrentSession);

            // もし、カートの中身がなかったらエラー
            if (CurrentSession.cartModelInfo == null || CurrentSession.cartModelInfo.Count <= 0)
            {
                return Json(new MJsonWithStatus() { status = "error" });
            }

            // セッションのカートの情報・送り先情報をもとに、データベースに書き込み
            int? shippingAddressesId = null;
            CommonModel.RegistDatabaseShippingAddresses(CurrentSession, ref shippingAddressesId);
            int ordersId = -1;
            CommonModel.RegistDatabaseOrders(CurrentSession, shippingAddressesId, ref ordersId);
            CommonModel.RegistDatabaseOrderGoods(CurrentSession, ordersId);
            CurrentSession.randomId = CommonModel.GetDatabaseOrdersIdToRandomId(ordersId);
            // カートの中身を空にする
            ClearToCart();

            // 送り先情報を捨てる
            CurrentSession.shippingAddressInfo = null;

            return Json(new MJsonWithStatus() { status = "success" });
        }

        /// <summary>
        /// 完了画面
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult ShoppingComplete()
        {
            string sid = null;
            SessionModel CurrentSession = null;
            GetAndSetSession(Session, ViewData, Url, ref sid, ref CurrentSession);

            if (CommonModel.GetDatabaseGlobalStatus() == ReturnOfBasicDatabase.Error)
            {
                ViewData["title"] = mentainanceTitle;
                ViewData["message"] = mentainanceMessage;
                return View("Error");
            }

            // 再アクセスの禁止
            if (string.IsNullOrEmpty(CurrentSession.randomId))
            {
                ViewData["title"] = "エラーが起こりました";
                ViewData["message"] = "注文が複数起こりました";
                return View("Error");
            }


            ViewData["title"] = "ご注文手続きが完了いたしました";
            ViewData["message"] = "ご注文いただき、ありがとうございます。";
            ViewData["randomId"] = CurrentSession.randomId;
            ViewData["tableTitle"] = "ご注文番号";
            CurrentSession.randomId = "";
            return View("Complete");
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

            if (CommonModel.GetDatabaseGlobalStatus() == ReturnOfBasicDatabase.Error)
            {
                ViewData["title"] = mentainanceTitle;
                ViewData["message"] = mentainanceMessage;
                return View("Error");
            }

            string paramArg1;
            string q;
            try
            {
                paramArg1 = Request.Params["id"];
                q = Request.Params["q"];
            }
            catch (Exception)
            {
                ViewData["title"] = "入力エラー";
                ViewData["message"] = "特殊記号を含む文字列で検索することはできません";
                return View("Error");
            }


            q = string.IsNullOrEmpty(q) ? "" : q;

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
                CommonModel.GetDataBaseGoodsOfShop(shopId, ref mGoods, q);

                string categoryName = "お弁当";
                ViewData["categoryName"] = categoryName;
                string unitName = "個";

                GoodsMakeView(mGoods, unitName);

                ViewData["id"] = shopId;
                ViewData["q"] = q;
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


            if (CommonModel.GetDatabaseGlobalStatus() == ReturnOfBasicDatabase.Error)
            {
                ViewData["title"] = mentainanceTitle;
                ViewData["message"] = mentainanceMessage;
                return View("Error");
            }

            string q;
            try
            {
                q = Request.Params["q"];
            }
            catch (Exception)
            {
                ViewData["title"] = "入力エラー";
                ViewData["message"] = "特殊記号を含む文字列で検索することはできません";
                return View("Error");
            }

            q = string.IsNullOrEmpty(q) ? "" : q;

            List<MGoods> mGoods = new List<MGoods>();

            string categoryName = "本";
            string unitName = "冊";
            ViewData["categoryName"] = categoryName;

            int categoryId = CommonModel.GetDataBaseCategotyId(categoryName);
            CommonModel.GetDataBaseGoodsOfCategory(categoryId, ref mGoods, q);

            GoodsMakeView(mGoods, unitName);

            ViewData["q"] = q;
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

                string aPicture = string.IsNullOrEmpty(aGoods.Picture) ? Url.Content($"~/GoodsPictures/noimage.png") : Url.Content($"~/GoodsPictures/{aGoods.Picture}");

                string priceHtml = "";
                if (aGoods.Price.Count == 0)
                {
                    priceHtml += @"<div class=""clearfix menu-list"">
                            未発売
                        </div>";
                }
                else if (aGoods.Price.Count == 1)
                {
                    priceHtml += string.Format(@"<div class=""clearfix menu-list"">
                            <label for=""{3}"">{0} {1}円<span class=""tax-text"">(税込)</span></label>
                            <select id=""{3}"" name=""{3}"">
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
                        </div>", string.IsNullOrEmpty(aGoods.Price[0].Variety) ? "" : aGoods.Price[0].Variety, aGoods.Price[0].Price, unitName, string.IsNullOrEmpty(aGoods.Price[0].Variety) ? "variety" : aGoods.Price[0].Variety);
                }
                else if (aGoods.Price.Count >= 2)
                {
                    foreach (var price in aGoods.Price)
                    {
                        priceHtml += string.Format(@"<div class=""clearfix menu-list"">
                            <label for=""{3}"">{0} {1}円<span class=""tax-text"">(税込)</span></label>
                            <select id=""{3}"" name=""{3}"">
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
                        </div>", string.IsNullOrEmpty(price.Variety) ? "" : price.Variety, price.Price, unitName, string.IsNullOrEmpty(price.Variety) ? "variety" : price.Variety);

                    }
                }

                string clickable = (aGoods.Price.Count == 0) ? "disabled" : "";


                html += string.Format(@"
                    <div class=""item-cell"">
                        <div class=""item-header"">
                            <h3>{4}</h3>
                            <span>{5}</span>
                        </div>

                        <img src=""{2}"" alt=""{0}"">

                        <div class=""item-body"">
                            
                            <h4>{0}</h4>
                            <hr>
                            <form id=""goods-{6}"" class=""goods"">
                                {3}

                                <div class=""detail"">
                                    {1}
                                </div>

                                <button class=""cart-button"" type=""submit"" {7}>
                                    <i class=""cart-icon fas fa-cart-arrow-down""></i>カゴに入れる
                                </button>
                            </form>
                        </div>
                    </div>", aGoods.Name, aGoods.Description, aPicture, priceHtml, aGoods.ShopName, aGoods.ShopGenre, aGoods.Id, clickable);
            }

            ViewData["goods"] = html;
        }

        /// <summary>
        /// マイページ(注文履歴)
        /// </summary>
        /// <returns></returns>
        public ActionResult MyPageOrder()
        {
            string sid = null;
            SessionModel CurrentSession = null;
            GetAndSetSession(Session, ViewData, Url, ref sid, ref CurrentSession);

            if (CommonModel.GetDatabaseGlobalStatus() == ReturnOfBasicDatabase.Error)
            {
                ViewData["title"] = mentainanceTitle;
                ViewData["message"] = mentainanceMessage;
                return View("Error");
            }

            // ログインしていなければ、ログイン画面にリダイレクト
            if (CurrentSession.customerUserInfo == null)
            {
                return View("Login");
            }

            List<MOrders> mOrders = new List<MOrders>();

            CommonModel.GetDatabaseOrders(CurrentSession.customerUserInfo.Id, ref mOrders);

            string html = "";

            foreach (var aOrder in mOrders)
            {

                html += string.Format(@"
                        <tr data-href=""{0}"" tabindex=""0"">
                            <td>{1}</td>
                            <td>発送準備中&nbsp;<i class=""fa-solid fa-arrow-up-right-from-square""></i></td>
                        </tr>", aOrder.RandomId, aOrder.OrderDate.ToString("yyyy/MM/dd HH:mm:ss"));
            }

            ViewData["orders"] = html;
            ViewData["index"] = 0;
            ViewData["title"] = "注文履歴";


            return View("MyPage");
        }

        /// <summary>
        /// マイページ(お問い合わせ履歴)
        /// </summary>
        /// <returns></returns>
        public ActionResult MyPageContact()
        {
            string sid = null;
            SessionModel CurrentSession = null;
            GetAndSetSession(Session, ViewData, Url, ref sid, ref CurrentSession);

            if (CommonModel.GetDatabaseGlobalStatus() == ReturnOfBasicDatabase.Error)
            {
                ViewData["title"] = mentainanceTitle;
                ViewData["message"] = mentainanceMessage;
                return View("Error");
            }

            // ログインしていなければ、ログイン画面にリダイレクト
            if (CurrentSession.customerUserInfo == null)
            {
                return View("Login");
            }

            List<MContacts> mContacts = new List<MContacts>();

            CommonModel.GetDatabaseContacts(CurrentSession.customerUserInfo.Id, ref mContacts);

            string html = "";

            foreach (var aContact in mContacts)
            {

                html += string.Format(@"
                        <tr data-href=""{0}"" tabindex=""0"">
                            <td>{1}</td>
                            <td>回答待ち&nbsp;<i class=""fa-solid fa-arrow-up-right-from-square""></i></td>
                        </tr>", aContact.RandomId, aContact.CreatedAt.ToString("yyyy/MM/dd HH:mm:ss"));
            }

            ViewData["orders"] = html;
            ViewData["index"] = 2;
            ViewData["title"] = "問い合わせ履歴";

            return View("MyPage");
        }

        /// <summary>
        /// マイページ(お客様情報変更)
        /// </summary>
        /// <returns></returns>
        public ActionResult MyPageSettings()
        {
            string sid = null;
            SessionModel CurrentSession = null;
            GetAndSetSession(Session, ViewData, Url, ref sid, ref CurrentSession);

            if (CommonModel.GetDatabaseGlobalStatus() == ReturnOfBasicDatabase.Error)
            {
                ViewData["title"] = mentainanceTitle;
                ViewData["message"] = mentainanceMessage;
                return View("Error");
            }

            // ログインしていなければ、ログイン画面にリダイレクト
            if (CurrentSession.customerUserInfo == null)
            {
                return View("Login");
            }

            ViewData["orders"] = "";
            ViewData["index"] = 3;
            ViewData["title"] = "お客様情報変更";
            ViewData["CurrentSession"] = CurrentSession;

            return View("MyPage");

        }

        /// <summary>
        /// マイページの注文履歴の詳細
        /// </summary>
        /// <returns></returns>
        public ActionResult OrderDetail()
        {
            string sid = null;
            SessionModel CurrentSession = null;
            GetAndSetSession(Session, ViewData, Url, ref sid, ref CurrentSession);

            if (CommonModel.GetDatabaseGlobalStatus() == ReturnOfBasicDatabase.Error)
            {
                ViewData["title"] = mentainanceTitle;
                ViewData["message"] = mentainanceMessage;
                return View("Error");
            }

            // ログインしていなければ、ログイン画面にリダイレクト
            if (CurrentSession.customerUserInfo == null)
            {
                return View("Login");
            }

            string paramArg1 = Request.Params["randomid"];

            MOrders mOrder = null;
            List<MOrderGoods> mOrderGoods = new List<MOrderGoods>();
            var isSuccess = CommonModel.GetDatabaseOrderGoods(paramArg1, ref mOrder, ref mOrderGoods);
            // クエリがない、またはランダムidが不正なら、トップページにリダイレクト
            if (isSuccess == ReturnOfBasicDatabase.Error || string.IsNullOrEmpty(paramArg1))
            {
                IndexMakeView();
                return View("Index");
            }
            List<MGoodsOfCart> mGoodsOfCartList = new List<MGoodsOfCart>();
            CommonModel.GetDataBaseOrderGoodsInCart(mOrder.OrderDate, mOrderGoods, ref mGoodsOfCartList);
            MShippingAddress mShippingAddress = null;

            if (mOrder.ShippingAddressesId != null)
            {
                CommonModel.GetDatabaseShippingAddress(mOrder.ShippingAddressesId.Value, ref mShippingAddress);
            }
            else
            {
                mShippingAddress = null;
            }
            int allTotalPrice = 0;
            // 店ごとにグループ化
            var query = mGoodsOfCartList.GroupBy(item => new { ShopName = item.ShopName, ShippingCost = item.ShippingCost });
            string html = "";

            foreach (var group in query)
            {
                // それぞれの店ごとにテーブルを作成

                int shopTotalPrice = 0;

                // テーブルヘッダー
                html += string.Format(@"
                        <div class=""a-shop"">
                            <h3>{0}</h3>
                            <div class=""cart-table-heading"">
                                <ul>
                                    <li class=""cart-table-body-list-item"">商品内容</li>
                                    <li class=""cart-table-body-list-item"">数量</li>
                                    <li class=""cart-table-body-list-item"">小計(税込)</li>
                                    <li class=""cart-table-body-list-item"">配達状況</li>
                                </ul>
                            </div>
                        <div class=""cart-table-body"">", group.Key.ShopName);



                foreach (var item in group)
                {
                    int totalPrice = item.Price * item.Quantity;
                    string varietyDisplay = string.IsNullOrEmpty(item.Variety) ? "" : $"({item.Variety})";
                    string varietyId = string.IsNullOrEmpty(item.Variety) ? "" : $"-{item.Variety}";

                    string aPicture = string.IsNullOrEmpty(item.Picture) ? Url.Content($"~/GoodsPictures/noimage.png") : Url.Content($"~/GoodsPictures/{item.Picture}");

                    string delivery = "エラー";

                    if (item.StartTimeOfDist != null)
                    {
                        if (item.EndTimeOfDist != null)
                        {
                            delivery = "配達済";
                        }
                        else
                        {
                            delivery = "配達中";
                        }
                    }
                    else
                    {
                        delivery = "未発送";
                    }



                    // テーブルボディー
                    html += string.Format(@"
                            <ul class=""goods"" id=""goods-{6}{7}"">
                                
                                <li class=""cart-table-body-list-item shopping-item-column"">
                                    <img src = ""{5}"" alt=""{0}{1}"">
                                    <div>
                                        <h4>{0}{1}</h4>
                                        <p>{2}円</p>
                                    </div>

                                </li>
                                <li class=""cart-table-body-list-item"">
                                    {3}
                                </li>
                                <li class=""cart-table-body-list-item"">
                                    {4}円
                                </li>
                                <li class=""cart-table-body-list-item"">
                                    {8}
                                </li>
                             </ul>", item.Name, varietyDisplay, item.Price, item.Quantity, totalPrice, aPicture, item.GoodsId, varietyId, delivery);

                    shopTotalPrice += totalPrice;
                }

                shopTotalPrice += group.Key.ShippingCost;

                html += string.Format(@"
                        </div>
                        <div class=""cart-table-postage"">
                            <span>送料 {0}円</span>
                        </div>

                        <div class=""cart-table-price"">
                            <span>合計金額 {1}円</span>
                        </div>", group.Key.ShippingCost, shopTotalPrice);

                allTotalPrice += shopTotalPrice;
                html += "</div>";

            }


            ViewData["goodsOfCart"] = html;
            ViewData["mOrder"] = mOrder;
            ViewData["totalPrice"] = allTotalPrice;
            ViewData["CurrentSession"] = CurrentSession;
            ViewData["ShippingAddress"] = mShippingAddress;
            return View();
        }

        /// <summary>
        /// マイページのお問い合わせ履歴の詳細
        /// </summary>
        /// <returns></returns>
        public ActionResult ContactDetail()
        {
            string sid = null;
            SessionModel CurrentSession = null;
            GetAndSetSession(Session, ViewData, Url, ref sid, ref CurrentSession);

            if (CommonModel.GetDatabaseGlobalStatus() == ReturnOfBasicDatabase.Error)
            {
                ViewData["title"] = mentainanceTitle;
                ViewData["message"] = mentainanceMessage;
                return View("Error");
            }

            // ログインしていなければ、ログイン画面にリダイレクト
            if (CurrentSession.customerUserInfo == null)
            {
                return View("Login");
            }

            string paramArg1 = Request.Params["randomid"];
            MContacts mContact = null;
            var isSuccess = CommonModel.GetDatabaseContact(paramArg1, ref mContact);
            // クエリがない、またはランダムidが不正なら、トップページにリダイレクト
            if (isSuccess == ReturnOfBasicDatabase.Error || string.IsNullOrEmpty(paramArg1))
            {
                IndexMakeView();
                return View("Index");
            }

            ViewData["categoryName"] = CommonModel.GetDatabaseCategoryName(mContact.CategoryId);

            ViewData["mContact"] = mContact;
            ViewData["CurrentSession"] = CurrentSession;
            return View();
        }

        public ActionResult Foodstuff()
        {
            string sid = null;
            SessionModel CurrentSession = null;
            GetAndSetSession(Session, ViewData, Url, ref sid, ref CurrentSession);

            if (CommonModel.GetDatabaseGlobalStatus() == ReturnOfBasicDatabase.Error)
            {
                ViewData["title"] = mentainanceTitle;
                ViewData["message"] = mentainanceMessage;
                return View("Error");
            }

            ViewData["title"] = "食料品";
            ViewData["message"] = "現在商品がありません";
            return View("Error");
        }

        public ActionResult DailyNecessities()
        {
            string sid = null;
            SessionModel CurrentSession = null;
            GetAndSetSession(Session, ViewData, Url, ref sid, ref CurrentSession);

            if (CommonModel.GetDatabaseGlobalStatus() == ReturnOfBasicDatabase.Error)
            {
                ViewData["title"] = mentainanceTitle;
                ViewData["message"] = mentainanceMessage;
                return View("Error");
            }

            ViewData["title"] = "日用品";
            ViewData["message"] = "現在商品がありません";
            return View("Error");
        }

        public ActionResult LocalSpecialty()
        {
            string sid = null;
            SessionModel CurrentSession = null;
            GetAndSetSession(Session, ViewData, Url, ref sid, ref CurrentSession);

            if (CommonModel.GetDatabaseGlobalStatus() == ReturnOfBasicDatabase.Error)
            {
                ViewData["title"] = mentainanceTitle;
                ViewData["message"] = mentainanceMessage;
                return View("Error");
            }

            ViewData["title"] = "特産品";
            ViewData["message"] = "現在商品がありません";
            return View("Error");
        }

        public ActionResult Medicine()
        {
            string sid = null;
            SessionModel CurrentSession = null;
            GetAndSetSession(Session, ViewData, Url, ref sid, ref CurrentSession);

            if (CommonModel.GetDatabaseGlobalStatus() == ReturnOfBasicDatabase.Error)
            {
                ViewData["title"] = mentainanceTitle;
                ViewData["message"] = mentainanceMessage;
                return View("Error");
            }

            ViewData["title"] = "お薬";
            ViewData["message"] = "現在商品がありません";
            return View("Error");
        }

        public ActionResult CongratulatoryGift()
        {
            string sid = null;
            SessionModel CurrentSession = null;
            GetAndSetSession(Session, ViewData, Url, ref sid, ref CurrentSession);

            if (CommonModel.GetDatabaseGlobalStatus() == ReturnOfBasicDatabase.Error)
            {
                ViewData["title"] = mentainanceTitle;
                ViewData["message"] = mentainanceMessage;
                return View("Error");
            }

            ViewData["title"] = "お祝品";
            ViewData["message"] = "現在商品がありません";
            return View("Error");
        }

        /// <summary>
        /// お問い合わせの本文を更新するAPI
        /// </summary>
        /// <param name="postModel">必要な項目から構成されるJsonから生成されたオブジェクト</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult SetMessage(MessageModel postModel)
        {
            string sid = null;
            SessionModel CurrentSession = null;
            GetAndSetSession(Session, ViewData, Url, ref sid, ref CurrentSession);

            CurrentSession.message = (postModel.Message == null) ? "" : postModel.Message;
            CurrentSession.categoryId = postModel.CategoryId;

            return Json(new MJsonWithStatus() { status = "success" });
        }

        /// <summary>
        /// お問い合わせを送信するAPI
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult PostContact()
        {
            string sid = null;
            SessionModel CurrentSession = null;
            GetAndSetSession(Session, ViewData, Url, ref sid, ref CurrentSession);

            // データベースに書き込み
            string randomId = null;
            if (CurrentSession.categoryId == null)
            {
                return Json(new MJsonWithStatus() { status = "error" });
            }
            CommonModel.RegistDatabaseContacts(CurrentSession, CurrentSession.categoryId.Value, ref randomId);
            CurrentSession.randomId = randomId;
            // クリア
            CurrentSession.message = "";

            return Json(new MJsonWithStatus() { status = "success" });
        }

        /// <summary>
        /// お問い合わせ画面
        /// </summary>
        /// <returns></returns>
        public ActionResult Contact()
        {
            string sid = null;
            SessionModel CurrentSession = null;
            GetAndSetSession(Session, ViewData, Url, ref sid, ref CurrentSession);

            if (CommonModel.GetDatabaseGlobalStatus() == ReturnOfBasicDatabase.Error)
            {
                ViewData["title"] = mentainanceTitle;
                ViewData["message"] = mentainanceMessage;
                return View("Error");
            }


            // ログインしていなければ、ログイン画面にリダイレクト
            if (CurrentSession.customerUserInfo == null)
            {
                return View("Login");
            }

            ViewData["CurrentSession"] = CurrentSession;
            string title = "お問い合わせ";
            var categoryId = CommonModel.GetDataBaseCategotyId(title);
            ViewData["categoryId"] = categoryId;
            ViewData["title"] = title;
            ViewData["NavigatorNext"] = "ContactConfirm";
            return View();
        }


        /// <summary>
        /// お問い合わせ確認画面
        /// </summary>
        /// <returns></returns>
        public ActionResult ContactConfirm()
        {
            string sid = null;
            SessionModel CurrentSession = null;
            GetAndSetSession(Session, ViewData, Url, ref sid, ref CurrentSession);

            if (CommonModel.GetDatabaseGlobalStatus() == ReturnOfBasicDatabase.Error)
            {
                ViewData["title"] = mentainanceTitle;
                ViewData["message"] = mentainanceMessage;
                return View("Error");
            }

            // ログインしていなければ、ログイン画面にリダイレクト
            if (CurrentSession.customerUserInfo == null)
            {
                return View("Login");
            }

            if (CurrentSession.categoryId == null)
            {
                ViewData["title"] = "エラーが起こりました";
                ViewData["message"] = "お問い合わせが複数起こりました";
                return View("Error");
            }

            string title = "お問い合わせ";
            ViewData["title"] = title;

            ViewData["CurrentSession"] = CurrentSession;
            ViewData["NavigatorPrev"] = "Contact";
            ViewData["NavigatorNext"] = "ContactComplete";
            return View("Confirm");
        }

        /// <summary>
        /// お問い合わせ完了画面
        /// </summary>
        /// <returns></returns>
        public ActionResult ContactComplete()
        {
            string sid = null;
            SessionModel CurrentSession = null;
            GetAndSetSession(Session, ViewData, Url, ref sid, ref CurrentSession);

            if (CommonModel.GetDatabaseGlobalStatus() == ReturnOfBasicDatabase.Error)
            {
                ViewData["title"] = mentainanceTitle;
                ViewData["message"] = mentainanceMessage;
                return View("Error");
            }

            // 再アクセスの禁止
            if (string.IsNullOrEmpty(CurrentSession.randomId))
            {
                ViewData["title"] = "エラーが起こりました";
                ViewData["message"] = "お問い合わせが複数起こりました";
                return View("Error");
            }

            ViewData["title"] = "お問い合わせを送信しました";
            ViewData["message"] = "お問い合わせいただき、ありがとうございます。<br><br>お問い合わせいただいた内容については、ご確認の上、返信させていただきます。";
            ViewData["randomId"] = CurrentSession.randomId;
            ViewData["tableTitle"] = "お問い合わせ番号";
            CurrentSession.randomId = "";
            CurrentSession.categoryId = null;
            return View("Complete");
        }

        /// <summary>
        /// お家の修理サービス画面
        /// </summary>
        /// <returns></returns>
        public ActionResult HomeRepair()
        {
            string sid = null;
            SessionModel CurrentSession = null;
            GetAndSetSession(Session, ViewData, Url, ref sid, ref CurrentSession);

            if (CommonModel.GetDatabaseGlobalStatus() == ReturnOfBasicDatabase.Error)
            {
                ViewData["title"] = mentainanceTitle;
                ViewData["message"] = mentainanceMessage;
                return View("Error");
            }


            // ログインしていなければ、ログイン画面にリダイレクト
            if (CurrentSession.customerUserInfo == null)
            {
                return View("Login");
            }

            ViewData["CurrentSession"] = CurrentSession;
            string title = "お家の修理サービス";
            var categoryId = CommonModel.GetDataBaseCategotyId(title);
            ViewData["categoryId"] = categoryId;
            ViewData["title"] = title;
            ViewData["NavigatorNext"] = "HomeRepairConfirm";
            return View("Contact");
        }

        /// <summary>
        /// お家の修理サービス確認画面
        /// </summary>
        /// <returns></returns>
        public ActionResult HomeRepairConfirm()
        {
            string sid = null;
            SessionModel CurrentSession = null;
            GetAndSetSession(Session, ViewData, Url, ref sid, ref CurrentSession);

            if (CommonModel.GetDatabaseGlobalStatus() == ReturnOfBasicDatabase.Error)
            {
                ViewData["title"] = mentainanceTitle;
                ViewData["message"] = mentainanceMessage;
                return View("Error");
            }

            // ログインしていなければ、ログイン画面にリダイレクト
            if (CurrentSession.customerUserInfo == null)
            {
                return View("Login");
            }

            if (CurrentSession.categoryId == null)
            {
                ViewData["title"] = "エラーが起こりました";
                ViewData["message"] = "お問い合わせが複数起こりました";
                return View("Error");
            }

            string title = "お家の修理サービス";
            ViewData["title"] = title;
            ViewData["CurrentSession"] = CurrentSession;
            ViewData["NavigatorPrev"] = "HomeRepair";
            ViewData["NavigatorNext"] = "HomeRepairComplete";

            return View("Confirm");
        }

        /// <summary>
        /// お家の修理サービス完了画面
        /// </summary>
        /// <returns></returns>
        public ActionResult HomeRepairComplete()
        {
            string sid = null;
            SessionModel CurrentSession = null;
            GetAndSetSession(Session, ViewData, Url, ref sid, ref CurrentSession);

            if (CommonModel.GetDatabaseGlobalStatus() == ReturnOfBasicDatabase.Error)
            {
                ViewData["title"] = mentainanceTitle;
                ViewData["message"] = mentainanceMessage;
                return View("Error");
            }

            // 再アクセスの禁止
            if (string.IsNullOrEmpty(CurrentSession.randomId))
            {
                ViewData["title"] = "エラーが起こりました";
                ViewData["message"] = "お問い合わせが複数起こりました";
                return View("Error");
            }

            ViewData["title"] = "お家の修理サービス送信完了";
            ViewData["message"] = "折り返し担当より連絡いたします。";
            ViewData["randomId"] = CurrentSession.randomId;
            ViewData["tableTitle"] = "お問い合わせ番号";
            CurrentSession.randomId = "";
            CurrentSession.categoryId = null;
            return View("Complete");
        }

        /// <summary>
        /// 旅行計画サービス画面
        /// </summary>
        /// <returns></returns>
        public ActionResult TravelPlanning()
        {
            string sid = null;
            SessionModel CurrentSession = null;
            GetAndSetSession(Session, ViewData, Url, ref sid, ref CurrentSession);

            if (CommonModel.GetDatabaseGlobalStatus() == ReturnOfBasicDatabase.Error)
            {
                ViewData["title"] = mentainanceTitle;
                ViewData["message"] = mentainanceMessage;
                return View("Error");
            }


            // ログインしていなければ、ログイン画面にリダイレクト
            if (CurrentSession.customerUserInfo == null)
            {
                return View("Login");
            }

            ViewData["CurrentSession"] = CurrentSession;
            string title = "旅行計画サービス";
            var categoryId = CommonModel.GetDataBaseCategotyId(title);
            ViewData["categoryId"] = categoryId;
            ViewData["title"] = title;
            ViewData["NavigatorNext"] = "TravelPlanningConfirm";
            return View("Contact");
        }

        /// <summary>
        /// 旅行計画サービス確認画面
        /// </summary>
        /// <returns></returns>
        public ActionResult TravelPlanningConfirm()
        {
            string sid = null;
            SessionModel CurrentSession = null;
            GetAndSetSession(Session, ViewData, Url, ref sid, ref CurrentSession);

            if (CommonModel.GetDatabaseGlobalStatus() == ReturnOfBasicDatabase.Error)
            {
                ViewData["title"] = mentainanceTitle;
                ViewData["message"] = mentainanceMessage;
                return View("Error");
            }

            // ログインしていなければ、ログイン画面にリダイレクト
            if (CurrentSession.customerUserInfo == null)
            {
                return View("Login");
            }

            if (CurrentSession.categoryId == null)
            {
                ViewData["title"] = "エラーが起こりました";
                ViewData["message"] = "お問い合わせが複数起こりました";
                return View("Error");
            }

            string title = "旅行計画サービス";
            ViewData["title"] = title;
            ViewData["CurrentSession"] = CurrentSession;
            ViewData["NavigatorPrev"] = "TravelPlanning";
            ViewData["NavigatorNext"] = "TravelPlanningComplete";

            return View("Confirm");
        }

        /// <summary>
        /// 旅行計画サービス完了画面
        /// </summary>
        /// <returns></returns>
        public ActionResult TravelPlanningComplete()
        {
            string sid = null;
            SessionModel CurrentSession = null;
            GetAndSetSession(Session, ViewData, Url, ref sid, ref CurrentSession);

            if (CommonModel.GetDatabaseGlobalStatus() == ReturnOfBasicDatabase.Error)
            {
                ViewData["title"] = mentainanceTitle;
                ViewData["message"] = mentainanceMessage;
                return View("Error");
            }

            // 再アクセスの禁止
            if (string.IsNullOrEmpty(CurrentSession.randomId))
            {
                ViewData["title"] = "エラーが起こりました";
                ViewData["message"] = "お問い合わせが複数起こりました";
                return View("Error");
            }

            ViewData["title"] = "旅行計画サービス送信完了";
            ViewData["message"] = "折り返し担当より連絡いたします。";
            ViewData["randomId"] = CurrentSession.randomId;
            ViewData["tableTitle"] = "お問い合わせ番号";
            CurrentSession.randomId = "";
            CurrentSession.categoryId = null;
            return View("Complete");
        }

        /// <summary>
        /// 家庭内作業サービス画面
        /// </summary>
        /// <returns></returns>
        public ActionResult InHomeWork()
        {
            string sid = null;
            SessionModel CurrentSession = null;
            GetAndSetSession(Session, ViewData, Url, ref sid, ref CurrentSession);

            if (CommonModel.GetDatabaseGlobalStatus() == ReturnOfBasicDatabase.Error)
            {
                ViewData["title"] = mentainanceTitle;
                ViewData["message"] = mentainanceMessage;
                return View("Error");
            }


            // ログインしていなければ、ログイン画面にリダイレクト
            if (CurrentSession.customerUserInfo == null)
            {
                return View("Login");
            }

            ViewData["CurrentSession"] = CurrentSession;
            string title = "家庭内作業サービス";
            var categoryId = CommonModel.GetDataBaseCategotyId(title);
            ViewData["categoryId"] = categoryId;
            ViewData["title"] = title;
            ViewData["NavigatorNext"] = "InHomeWorkConfirm";
            return View("Contact");
        }

        /// <summary>
        /// 家庭内作業サービス確認画面
        /// </summary>
        /// <returns></returns>
        public ActionResult InHomeWorkConfirm()
        {
            string sid = null;
            SessionModel CurrentSession = null;
            GetAndSetSession(Session, ViewData, Url, ref sid, ref CurrentSession);

            if (CommonModel.GetDatabaseGlobalStatus() == ReturnOfBasicDatabase.Error)
            {
                ViewData["title"] = mentainanceTitle;
                ViewData["message"] = mentainanceMessage;
                return View("Error");
            }

            // ログインしていなければ、ログイン画面にリダイレクト
            if (CurrentSession.customerUserInfo == null)
            {
                return View("Login");
            }

            if (CurrentSession.categoryId == null)
            {
                ViewData["title"] = "エラーが起こりました";
                ViewData["message"] = "お問い合わせが複数起こりました";
                return View("Error");
            }

            string title = "家庭内作業サービス";
            ViewData["title"] = title;
            ViewData["CurrentSession"] = CurrentSession;
            ViewData["NavigatorPrev"] = "InHomeWork";
            ViewData["NavigatorNext"] = "InHomeWorkComplete";

            return View("Confirm");
        }

        /// <summary>
        /// 家庭内作業サービス完了画面
        /// </summary>
        /// <returns></returns>
        public ActionResult InHomeWorkComplete()
        {
            string sid = null;
            SessionModel CurrentSession = null;
            GetAndSetSession(Session, ViewData, Url, ref sid, ref CurrentSession);

            if (CommonModel.GetDatabaseGlobalStatus() == ReturnOfBasicDatabase.Error)
            {
                ViewData["title"] = mentainanceTitle;
                ViewData["message"] = mentainanceMessage;
                return View("Error");
            }

            // 再アクセスの禁止
            if (string.IsNullOrEmpty(CurrentSession.randomId))
            {
                ViewData["title"] = "エラーが起こりました";
                ViewData["message"] = "お問い合わせが複数起こりました";
                return View("Error");
            }

            ViewData["title"] = "家庭内作業サービス送信完了";
            ViewData["message"] = "折り返し担当より連絡いたします。";
            ViewData["randomId"] = CurrentSession.randomId;
            ViewData["tableTitle"] = "お問い合わせ番号";
            CurrentSession.randomId = "";
            CurrentSession.categoryId = null;
            return View("Complete");
        }


    }
}