

Public Class CommonViewModels

End Class


Public Class UserModel
    Inherits MCust

    ' Public Property CUST_ADDR1() As String

    Public Property ORDER_CODE() As String
End Class

Public Class SessionModel

    ' ユーザー情報
    Public Property CUR_UserInfo() As New UserModel

    ' セッション情報
    'Public Property CUR_OrderCode() As String  ' 注文番号 14文字

    ' 買い物方法
    Public Property CUR_DeliCode() As String   ' 配送コード 1:会員宅配, 2:無料宅配, 3:優良宅配
    Public Property CUR_MethPaym() As String   ' 支払い方法 1:クレジットカード決済, 2:代金引き換え
    Public Property CUR_Carriage() As String   ' 配送時間帯
    Public Property CUR_DeliPric() As Integer  ' 送料
    Public Property CUR_CustCard() As String   ' [CUST_CARD]
    Public Property CUR_CustCanm() As String   ' [CUST_CARDNUM]
    Public Property CUR_CustSecu() As String   ' [CUST_SECU]

    ' カテゴリー
    Public Property CUR_CATEGORY1() As String
    Public Property CUR_CATEGORY2() As String
    Public Property CUR_CATEGORYNAME() As String
    Public Property CUR_Items() As IEnumerable(Of MItem)

    ' ローカルメモリー
    Public Property CategoryView() As String
    Public Property CategoryList() As IEnumerable(Of MCategory)

    ' 制御フラグ
    Public Property ControllerIndex() As Integer

    Public Property PageIndex() As Integer

End Class

Public Class LoginModel
    Public Property Username() As String

    Public Property Password() As String
End Class

Public Class CategoryViewModel

    Public Property Categorys() As IEnumerable(Of MCategory)

End Class

Public Class ItemListViewModel 'Category

    '--------View用---------------------------------------



    Public Property PageIndex() As String

    Public Property EnablePrev() As String
    Public Property EnableNext() As String
    '--------View用---------------------------------------

    '--------Post用---------------------------------------
    Public Property ItemCode() As String ' 
    Public Property Amount() As String
    Public Property SelectCol() As String
    Public Property SelectRow() As String ' 
    '--------Post用---------------------------------------

    Public Property CurrentItems() As IEnumerable(Of MItem)

End Class

Public Class ItemSummaryViewModel

    Public Property CUR_UserInfo() As New UserModel

    Public Property BasketItems() As IEnumerable(Of TBasket)

End Class


Public Class OrderformViewModel

    Public Property ORDER_CODE() As String

    Public Property CustTel() As String
    Public Property CustTel1() As String
    Public Property CustTel2() As String
    Public Property CustTel3() As String

    Public Property CustName() As String
    Public Property CustKana() As String
    Public Property CustPost() As String
    Public Property CustAddr1() As String
    Public Property CustAddr2() As String
    Public Property CustAddr3() As String
    Public Property CustMail() As String





    Public Property DeliveryFlag() As String ' 1:会員宅配, 2:無料宅配, 3:優良宅配
    Public Property CarriageFlag() As String ' 配達時間

End Class

Public Class UserRegisterModel

    Public Property CustName() As String

    Public Property CustKana() As String

    Public Property CustPost() As String

    Public Property CustAddr1() As String

    Public Property CustAddr2() As String

    Public Property CustAddr3() As String

    Public Property CustTel() As String

    Public Property CustSmaf() As String

    Public Property CustMail() As String

    Public Property CustPass() As String

End Class





Public Class OrderformCardViewModel

    Public Property payment() As String

    Public Property cctype() As String
    Public Property ccnumber() As String
    Public Property cccsc() As String
    Public Property ccname() As String
    Public Property ccexpmonth() As String
    Public Property ccexpyear() As String

    Sub New()
        payment = ""
        cctype = ""
        ccnumber = ""
        cccsc = ""
        ccname = ""
        ccexpmonth = ""
        ccexpyear = ""
    End Sub
End Class



Public Class VtestViewModel
    Public Property ID() As Integer
    Public Property Title() As String
    Public Property ReleaseDate() As Date
    Public Property Genre() As String
    Public Property Price() As Decimal

    Public Property strhtml() As String


    Public Property PageIndex() As String

    Public Function OutText() As String

        Return "AAAA"
    End Function

End Class


Public Class Movie
    Public Property ID() As Integer
    Public Property Title() As String
    Public Property ReleaseDate() As Date
    Public Property Genre() As String
    Public Property Price() As Decimal

    Public Property strhtml() As String

    Public Function OutText() As String

        Return "AAAA"
    End Function

End Class

