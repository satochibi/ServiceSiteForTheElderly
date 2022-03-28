



Public Class MCust

        Public Property [CUST_CODE]() As String

        Public Property [CUST_NUM]() As String   ' 会員番号 12文字(年月日番号　○○○○年○○月○○日○○○○) 顧客番号 12文字

        Public Property [CUST_PASS]() As String

        Public Property [CUST_NAME]() As String

        Public Property [CUST_KANA]() As String

        Public Property [CUST_POST]() As String

        Public Property [CUST_ADDR1]() As String
        Public Property [CUST_ADDR2]() As String
        Public Property [CUST_ADDR3]() As String

        Public Property [CUST_TEL]() As String

        Public Property [CUST_AGE]() As String

        Public Property [CUST_MAIL]() As String

        Public Property [CUST_SMAF]() As String

        Public Property [CUST_CARD]() As String

        Public Property [CUST_CARDNUM]() As String

        Public Property [CUST_SECU]() As String

        Sub New()
            [CUST_CODE] = ""
            [CUST_NUM] = ""
            [CUST_PASS] = ""
            [CUST_NAME] = ""
            [CUST_KANA] = ""
            [CUST_POST] = ""
            [CUST_ADDR1] = ""
            [CUST_ADDR2] = ""
            [CUST_ADDR3] = ""
            [CUST_TEL] = ""
            [CUST_AGE] = ""
            [CUST_MAIL] = ""
            [CUST_SMAF] = ""
            [CUST_CARD] = ""
            [CUST_CARDNUM] = ""
            [CUST_SECU] = ""
        End Sub

    End Class

Public Class MCategory

    Public Property CATEGORY1() As String

    Public Property CATEGORY2() As String

    Public Property CATEGORY3() As String

    Public Property SORT_NUM() As Integer

    Public Property CATEGORY_NAME() As String

    Public Property CATEGORY_PARENTNAME() As String

End Class

Public Class MItem

    Public Property ITEM_CODE() As String

    Public Property ITEM_PHOT() As String

    Public Property ITEM_NAME() As String

    Public Property ITEM_PRICE() As String

    Public Property ITEM_TAXPRICE() As String

    Public Property DATE_START() As String

    Public Property DATE_END() As String

    Public Property CATEGORY1() As String

    Public Property CATEGORY2() As String

    Sub New()
        ITEM_CODE = ""
        ITEM_PHOT = ""
        ITEM_NAME = ""
        ITEM_PRICE = ""
        ITEM_TAXPRICE = ""
        CATEGORY1 = ""
        CATEGORY2 = ""
    End Sub

End Class

Public Class TBasket

    Public Property ORDER_CODE() As String

    Public Property CUST_CODE() As String

    Public Property ITEM_CODE() As String

    Public Property SERIAL_NO() As Integer

    Public Property AMOUNT() As Integer

    Public Property ITEM_BODY() As New MItem

    Sub New()
        ITEM_CODE = ""
        ORDER_CODE = ""
        CUST_CODE = ""
        SERIAL_NO = 0
        AMOUNT = 0
    End Sub

End Class

Public Class TResult

    Public Property ORDER_CODE() As String
    Public Property CUST_CODE() As String
    Public Property ITEM_CODE() As String

    Public Property RES_YEAR() As Integer
    Public Property RES_MONTH() As Integer
    Public Property RES_DATE() As Integer
    Public Property RES_TIME() As Integer
    Public Property RES_DELITIME() As String

    Public Property CARD_CODE() As String
    Public Property RES_DELICASH() As String

    Public Property RES_AMOUNT() As Integer
    Public Property RES_PRICE() As Integer

    Public Property RES_STATUS() As Integer
    Public Property DELETE_FLAG() As Integer
    Public Property CREATE_DATE() As Date

    Public Property ITEM_BODY() As New MItem

    Sub New()
        ITEM_CODE = ""
        ORDER_CODE = ""
        CUST_CODE = ""

        RES_DELITIME = ""
        CARD_CODE = ""
        RES_DELICASH = ""
        RES_AMOUNT = 0
        RES_PRICE = 0
        RES_STATUS = 0
        DELETE_FLAG = 0
        CREATE_DATE = Now
    End Sub

End Class

Public Class TResultorder

    Public Property ORDER_CODE() As String
    Public Property CUST_CODE() As String

    Public Property RES_YEAR() As Integer
    Public Property RES_MONTH() As Integer
    Public Property RES_DATE() As Integer
    Public Property RES_TIME() As Integer
    Public Property RES_DELITIME() As String

    Public Property [CUST_ADDR1]() As String
    Public Property [CUST_ADDR2]() As String
    Public Property [CUST_ADDR3]() As String

    Public Property [CUST_NAME]() As String
    Public Property [CUST_KANA]() As String
    Public Property [CUST_POST]() As String
    Public Property [CUST_TEL]() As String
    Public Property [CUST_MAIL]() As String

    Public Property METH_PAYM() As Integer      ' 支払い方法 1:クレジットカード決済, 2:代金引き換え
    Public Property CARRIAGE() As Integer

    Public Property [CUST_CARD]() As String
    Public Property [CUST_CARDNUM]() As String
    Public Property [CUST_SECU]() As String

    Public Property RES_STATUS() As Integer
    Public Property DELETE_FLAG() As Integer
    Public Property CREATE_DATE() As Date

    Sub New()
        ORDER_CODE = ""
        CUST_CODE = ""

        [CUST_ADDR1] = ""
        [CUST_ADDR2] = ""
        [CUST_ADDR3] = ""
        [CUST_NAME] = ""
        [CUST_KANA] = ""
        [CUST_POST] = ""
        [CUST_TEL] = ""
        [CUST_MAIL] = ""

        METH_PAYM = 0
        CARRIAGE = 0
        RES_STATUS = 0
        DELETE_FLAG = 0
        CREATE_DATE = Now
    End Sub
End Class

