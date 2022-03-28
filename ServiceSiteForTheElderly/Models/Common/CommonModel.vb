Module CommonModel

    ' [顧客]
    Public Function CheckDatabaseLogin(sCustNum As String, sPassword As String,
                                ByRef sCustCode As String,
                                Optional ByRef hitCount As Integer = 0) As Integer
        Dim cust As New MCust
        Dim rtn = CheckDatabaseLogin(sCustNum, sPassword, cust, hitCount)

        If rtn = 0 Then
            sCustCode = cust.CUST_CODE
        End If

        Return rtn
    End Function
    Public Function CheckDatabaseLogin(sCustNum As String, sPassword As String,
                             ByRef cust As MCust,
                             Optional ByRef hitCount As Integer = 0) As Integer

        Dim dba As New DBAccess
        Dim dt As DataTable = Nothing
        Dim sql = "select * from M_CUSTS "
        sql += " WHERE CUST_NUM = '" & sCustNum & "'"

        Try
            dba.Query(sql, dt)
            If dt.Rows.Count > 0 Then
                hitCount = dt.Rows.Count

                Dim sCustPass = GetDbString(dt.Rows(0).Item("CUST_PASS"))

                sPassword = Trim(sPassword)

                If sPassword = sCustPass Then
                    cust = New MCust
                    cust.CUST_CODE = dt.Rows(0).Item("CUST_CODE")
                    cust.CUST_ADDR1 = dt.Rows(0).Item("CUST_ADDR1")
                    cust.CUST_ADDR2 = dt.Rows(0).Item("CUST_ADDR2")
                    cust.CUST_ADDR3 = dt.Rows(0).Item("CUST_ADDR3")
                    cust.CUST_AGE = dt.Rows(0).Item("CUST_AGE")
                    cust.CUST_CARD = dt.Rows(0).Item("CUST_CARD")
                    cust.CUST_CARDNUM = dt.Rows(0).Item("CUST_CARDNUM")
                    cust.CUST_MAIL = dt.Rows(0).Item("CUST_MAIL")
                    cust.CUST_NAME = dt.Rows(0).Item("CUST_NAME")
                    cust.CUST_KANA = dt.Rows(0).Item("CUST_KANA")
                    cust.CUST_NUM = dt.Rows(0).Item("CUST_NUM")
                    cust.CUST_POST = dt.Rows(0).Item("CUST_POST")
                    cust.CUST_SECU = dt.Rows(0).Item("CUST_SECU")
                    cust.CUST_SMAF = dt.Rows(0).Item("CUST_SMAF")
                    cust.CUST_TEL = dt.Rows(0).Item("CUST_TEL")
                    Return 0
                Else
                    Return -2
                End If
            Else
                Return -1
            End If

        Catch ex As Exception
            Return -3
        Finally
            dt = Nothing
        End Try

    End Function
    Public Function CheckDatabaseCust(cust As MCust, Optional ByRef custPass As String = "") As Integer

        If cust.CUST_CODE = "" Then
            Return -1
        End If

        Dim dba As New DBAccess
        Dim dt As DataTable = Nothing
        Dim sql = "select * from M_CUSTS "
        sql += " WHERE CUST_CODE = '" & cust.CUST_CODE & "'"

        Try
            dba.Query(sql, dt)
            If dt.Rows.Count > 0 Then
                custPass = GetDbString(dt.Rows(0).Item("CUST_PASS"))
                Return 0
            Else
                Return -1
            End If

        Catch ex As Exception
            Return -2
        Finally
            dt = Nothing
        End Try

    End Function
    Public Function RegistDatabaseCust(item As MCust) As Integer
        Dim sCustCode = item.CUST_CODE

        Dim dba As New DBAccess
        Dim dt As DataTable = Nothing
        Dim sql = "select * from M_CUSTS"
        sql += " WHERE CUST_CODE='" & sCustCode & "'  "

        dba.Query(sql, dt)
        If dt.Rows.Count > 0 Then

        Else
            sql = "insert into M_CUSTS (CUST_CODE, CUST_NUM, CUST_PASS, CUST_NAME, CUST_KANA"
            sql += ", CUST_POST, CUST_ADDR1, CUST_ADDR2, CUST_ADDR3, CUST_TEL, CUST_AGE "
            sql += ", CUST_MAIL,CUST_SMAF,CUST_CARD,CUST_CARDNUM,CUST_SECU)"

            sql += " values ('" & sCustCode & "','" & item.CUST_NUM & "','" & item.CUST_PASS & "','" & item.CUST_NAME & "', '" & item.CUST_KANA & "', "
            sql += "'" & item.CUST_POST & "','" & item.CUST_ADDR1 & "','" & item.CUST_ADDR2 & "','" & item.CUST_ADDR3 & "','" & item.CUST_TEL & "','" & item.CUST_AGE & "', "
            sql += "'" & item.CUST_MAIL & "','" & item.CUST_SMAF & "','" & item.CUST_CARD & "','" & item.CUST_CARDNUM & "', "
            sql += "'" & item.CUST_SECU & "'"
            sql += " )"

            If dba.Execute(sql) >= 0 Then

                Return 1
            Else
                Return -1
            End If
        End If

        Return 0
    End Function
    Public Function UpdateDatabaseCust(item As MCust) As Integer
        Dim sCustCode = item.CUST_CODE

        Dim dba As New DBAccess
        Dim dt As DataTable = Nothing
        Dim sql = "update M_CUSTS "

        sql += "set "

        If Not String.IsNullOrEmpty(item.CUST_ADDR1) Then
            sql += " CUST_ADDR1 = '" & item.CUST_ADDR1 & "', "
        End If
        If Not String.IsNullOrEmpty(item.CUST_ADDR2) Then
            sql += " CUST_ADDR2 = '" & item.CUST_ADDR2 & "', "
        End If
        If Not String.IsNullOrEmpty(item.CUST_ADDR3) Then
            sql += " CUST_ADDR3 = '" & item.CUST_ADDR3 & "', "
        End If
        If Not String.IsNullOrEmpty(item.CUST_AGE) Then
            sql += " CUST_AGE   =  " & item.CUST_AGE & ", "
        End If
        If Not String.IsNullOrEmpty(item.CUST_CARD) Then
            sql += " CUST_CARD  = '" & item.CUST_CARD & "', "
        End If
        If Not String.IsNullOrEmpty(item.CUST_CARDNUM) Then
            sql += " CUST_CARDNUM = '" & item.CUST_CARDNUM & "', "
        End If
        If Not String.IsNullOrEmpty(item.CUST_KANA) Then
            sql += " CUST_KANA  = '" & item.CUST_KANA & "', "
        End If
        If Not String.IsNullOrEmpty(item.CUST_MAIL) Then
            sql += " CUST_MAIL  = '" & item.CUST_MAIL & "', "
        End If
        If Not String.IsNullOrEmpty(item.CUST_NAME) Then
            sql += " CUST_NAME  = '" & item.CUST_NAME & "', "
        End If
        If Not String.IsNullOrEmpty(item.CUST_NUM) Then
            sql += " CUST_NUM   = '" & item.CUST_NUM & "', "
        End If
        If Not String.IsNullOrEmpty(item.CUST_PASS) Then
            sql += " CUST_PASS  = '" & item.CUST_PASS & "', "
        End If
        If Not String.IsNullOrEmpty(item.CUST_POST) Then
            sql += " CUST_POST  = '" & item.CUST_POST & "', "
        End If
        If Not String.IsNullOrEmpty(item.CUST_SECU) Then
            sql += " CUST_SECU  = '" & item.CUST_SECU & "', "
        End If
        If Not String.IsNullOrEmpty(item.CUST_SMAF) Then
            sql += " CUST_SMAF  = '" & item.CUST_SMAF & "', "
        End If
        If Not String.IsNullOrEmpty(item.CUST_TEL) Then
            sql += " CUST_TEL   = '" & item.CUST_TEL & "', "
        End If

        sql = sql.TrimEnd(","c)

        sql += " WHERE CUST_CODE='" & sCustCode & "' "


        If dba.Execute(sql) >= 0 Then
            Return 1
        Else
            Return -1
        End If

    End Function
    Public Function GetDatabaseCustList(ByRef custs As List(Of MCust)) As Integer
        Dim dba As New DBAccess
        Dim dt As DataTable = Nothing
        Dim sql = "select * from M_CUSTS "

        Try
            dba.Query(sql, dt)
            If dt.Rows.Count > 0 Then

                Dim cust As New MCust
                For r = 0 To dt.Rows.Count - 1
                    cust = New MCust
                    cust.CUST_ADDR1 = GetDbString(dt.Rows(r).Item("CUST_ADDR1"))
                    cust.CUST_ADDR2 = GetDbString(dt.Rows(r).Item("CUST_ADDR2"))
                    cust.CUST_ADDR3 = GetDbString(dt.Rows(r).Item("CUST_ADDR3"))
                    cust.CUST_AGE = GetDbString(dt.Rows(r).Item("CUST_AGE"))
                    cust.CUST_CARD = GetDbString(dt.Rows(r).Item("CUST_CARD"))
                    cust.CUST_CARDNUM = GetDbString(dt.Rows(r).Item("CUST_CARDNUM"))
                    cust.CUST_CODE = GetDbString(dt.Rows(r).Item("CUST_CODE"))
                    cust.CUST_MAIL = GetDbString(dt.Rows(r).Item("CUST_MAIL"))
                    cust.CUST_NAME = GetDbString(dt.Rows(r).Item("CUST_NAME"))
                    cust.CUST_KANA = GetDbString(dt.Rows(r).Item("CUST_KANA"))
                    cust.CUST_NUM = GetDbString(dt.Rows(r).Item("CUST_NUM"))
                    cust.CUST_PASS = GetDbString(dt.Rows(r).Item("CUST_PASS"))
                    cust.CUST_POST = GetDbString(dt.Rows(r).Item("CUST_POST"))
                    cust.CUST_SECU = GetDbString(dt.Rows(r).Item("CUST_SECU"))
                    cust.CUST_SMAF = GetDbString(dt.Rows(r).Item("CUST_SMAF"))
                    cust.CUST_TEL = GetDbString(dt.Rows(r).Item("CUST_TEL"))
                    custs.Add(cust)
                Next

                Return dt.Rows.Count
            Else
                Return -1
            End If

        Catch ex As Exception

            Return -2

        Finally
            dt = Nothing
            dba = Nothing
        End Try

    End Function
    Public Function DeleteDatabaseCust(cust As MCust) As Integer
        Dim dba As New DBAccess

        Dim sql = "delete from M_CUSTS"
        sql += " WHERE CUST_CODE = '" & cust.CUST_CODE & "'"

        If dba.Execute(sql) >= 0 Then
            Return 0
        Else
            Return -1
        End If

        Return 0

    End Function

    ' [商品]
    ' カテゴリー毎の商品
    Public Function GetDatabaseItemList(category As String, curDate As Date?, ByRef items As List(Of MItem)) As Integer
        Dim rtn As Integer

        Dim errortask As Func(Of List(Of MItem), Integer) =
            Function(items_ As List(Of MItem)) As Integer
                'Dim item As New MItem
                'item.ITEM_CODE = "エラー"
                'item.ITEM_NAME = "エラー"
                'item.ITEM_PHOT = "~/Pics/error.jpg"
                'items_.Add(item)
                Return 0
            End Function


        ' カテゴリー
        Dim category1 As String = "", category2 As String = ""

        If category.Length = 5 Then
            category1 = Mid(category, 1, 2)
            category2 = Mid(category, 3, 3)
        End If

        Dim dba As New DBAccess
        Dim dt As DataTable = Nothing
        Dim sql = "select * from M_ITEMS "

        ' WHERE
        If category1 <> "" Then
            sql += "WHERE CATEGORY1 = '" & category1 & "' AND CATEGORY2 = '" & category2 & "' "

            If curDate IsNot Nothing Then

                Dim dtDate As Date = curDate
                Dim compDate As String = dtDate.ToString("yyyyMMdd")

                sql += " AND ((DATE_START <= '" & compDate & "' AND '" & compDate & "' <= DATE_END) "
                sql += " OR (CATEGORY3 = 1)) " 'CATEGORY3が１なら無条件で表示 
            End If
        End If


        dba.Query(sql, dt)

        If dt.Rows.Count = 0 Then
            errortask(items)
            rtn = -1
        Else
            Dim item As New MItem
            For r = 0 To dt.Rows.Count - 1
                item = New MItem
                item.ITEM_CODE = dt.Rows(r).Item("ITEM_CODE")
                item.ITEM_NAME = dt.Rows(r).Item("ITEM_NAME")
                If InStr(dt.Rows(r).Item("ITEM_PHOT"), ".") > 0 Then
                    item.ITEM_PHOT = "~/Pics/" & dt.Rows(r).Item("ITEM_PHOT")
                Else
                    item.ITEM_PHOT = "~/Pics/" & dt.Rows(r).Item("ITEM_PHOT") & ".png"  '"~/Pics/download01.jpg"
                End If
                item.ITEM_PRICE = dt.Rows(r).Item("ITEM_PRICE")
                item.ITEM_TAXPRICE = dt.Rows(r).Item("ITEM_TAXPRICE")

                ' 条件データ
                item.DATE_START = GetDbString(dt.Rows(r).Item("DATE_START"))
                item.DATE_END = GetDbString(dt.Rows(r).Item("DATE_END"))
                item.CATEGORY1 = dt.Rows(r).Item("CATEGORY1")
                item.CATEGORY2 = dt.Rows(r).Item("CATEGORY2")
                items.Add(item)
            Next
            rtn = dt.Rows.Count
        End If

        Return rtn
    End Function

    ' [カテゴリー]
    ' カテゴリー一覧
    Public Function GetDatabaseCategoryList(ByRef items As List(Of MCategory)) As Integer

        Dim dba As New DBAccess
        Dim dt As DataTable = Nothing
        dba.Query("select * from M_CATEGORYS ORDER BY CATEGORY1, SORT_NUM ", dt)

        Dim cate1 As String, cateName = ""
        Dim item As New MCategory
        For r = 0 To dt.Rows.Count - 1
            item = New MCategory
            item.CATEGORY1 = dt.Rows(r).Item("CATEGORY1")
            item.CATEGORY2 = dt.Rows(r).Item("CATEGORY2")
            item.CATEGORY3 = dt.Rows(r).Item("CATEGORY3")
            item.SORT_NUM = dt.Rows(r).Item("SORT_NUM")
            item.CATEGORY_NAME = dt.Rows(r).Item("CATEGORY_NAME")

            '---------------------------------------------------------------
            cate1 = item.CATEGORY1 : cateName = ""
            If cate1 = "01" Then cateName = "野菜"
            If cate1 = "02" Then cateName = "果物"
            If cate1 = "03" Then cateName = "お肉"
            If cate1 = "04" Then cateName = "お魚"
            If cate1 = "05" Then cateName = "パン・たまご・デザート・和菓子"
            If cate1 = "06" Then cateName = "冷凍食品・アイス・氷"
            If cate1 = "07" Then cateName = "牛乳・乳製品・納豆・豆腐・練り物"
            If cate1 = "08" Then cateName = "調味料・酢・つゆ・ドレッシング"
            If cate1 = "09" Then cateName = "乾物・缶詰"
            If cate1 = "10" Then cateName = "粉類・カレー・インスタント食品"
            If cate1 = "11" Then cateName = "お菓子・製菓材料"
            If cate1 = "12" Then cateName = "お米・お餅"
            If cate1 = "13" Then cateName = "お酒・水・飲料"
            If cate1 = "14" Then cateName = "日用品・生活雑貨"
            If cate1 = "15" Then cateName = "ドラッグ・ベビー・介護用品"
            If cate1 = "16" Then cateName = "ペット用品"
            If cate1 = "17" Then cateName = "お弁当・惣菜"
            item.CATEGORY_PARENTNAME = cateName
            items.Add(item)
        Next

        Return 0
    End Function
    ' Category2
    Public Function GetDatabaseCategoryCategory2(category1 As String, ByRef items As List(Of MCategory)) As String

        Dim dba As New DBAccess
        Dim dt As DataTable = Nothing

        Dim sql = "select * from M_CATEGORYS"
        sql += " WHERE CATEGORY1 = '" & category1 & "'" ' AND CATEGORY2 = '" & category2 & "' "
        sql += " ORDER BY SORT_NUM "

        dba.Query(sql, dt)

        Dim item As New MCategory
        For r = 0 To dt.Rows.Count - 1
            item = New MCategory
            item.CATEGORY1 = dt.Rows(r).Item("CATEGORY1")
            item.CATEGORY2 = dt.Rows(r).Item("CATEGORY2")
            item.CATEGORY3 = dt.Rows(r).Item("CATEGORY3")
            item.SORT_NUM = dt.Rows(r).Item("SORT_NUM")
            item.CATEGORY_NAME = dt.Rows(r).Item("CATEGORY_NAME")

            '---------------------------------------------------------------

            items.Add(item)
        Next

        Return dt.Rows(0).Item("CATEGORY2")
    End Function

    ' [買い物かご]
    ' 買い物かご削除
    Public Function DeleteDatabaseBasketList(sCustCode As String, sOrderCode As String, sItemCode As String) As Integer
        Dim dba As New DBAccess

        Dim sql = "delete from T_BASKETS "

        sql += " WHERE CUST_CODE='" & sCustCode & "' AND ORDER_CODE='" & sOrderCode & "'"
        If sItemCode <> "" Then
            sql += " AND ITEM_CODE='" & sItemCode & "' "
        End If

        If dba.Execute(sql) >= 0 Then
            Return 0
        Else
            Return -1
        End If

        Return 0

    End Function
    Public Function DeleteDatabaseBasketList(sOrderCode As String) As Integer
        Dim dba As New DBAccess

        Dim sql = "delete from T_BASKETS "

        sql += " WHERE T_BASKETS.ORDER_CODE <= '" & sOrderCode & "' "

        Dim iCount = dba.Execute(sql)
        If iCount >= 0 Then
            Return iCount
        Else
            Return -1
        End If

        Return 0
    End Function
    Public Function PlusDatabaseBasketList(sCustCode As String, sOrderCode As String, sItemCode As String, Optional mode As Integer = 0, Optional pAmount As String = "") As Integer
        Dim dba As New DBAccess
        Dim dt As DataTable = Nothing
        Dim sql = "SELECT AMOUNT from T_BASKETS"
        Dim where = " WHERE ORDER_CODE='" & sOrderCode & "' AND CUST_CODE='" & sCustCode & "' AND ITEM_CODE='" & sItemCode & "' "
        where += " AND [DECISION_FLAG] = 0 "

        dba.Query(sql + where, dt)

        If dt.Rows.Count > 0 Then

            Dim iAMOUNT = dt.Rows(0).Item("AMOUNT")
            Dim sAmount As String = ""
            If mode <> -1 Then
                sAmount = CStr(Val(iAMOUNT) + 1)

                If pAmount <> "" Then
                    sAmount = CStr(Val(iAMOUNT) + Val(pAmount))
                End If
            ElseIf mode = -1 Then
                sAmount = CStr(Val(iAMOUNT) - 1)

                If pAmount <> "" Then
                    sAmount = CStr(Val(iAMOUNT) - Val(pAmount))
                End If
            End If

            If sAmount = "0" Then
                sql = "delete from T_BASKETS " & where
            Else
                sql = "update T_BASKETS set AMOUNT = " & sAmount & where
            End If

            If dba.Execute(sql) >= 0 Then

                Return 0
            Else
                Return -1
            End If
        End If

        Return 0

    End Function
    ' 買い物かご編集
    Public Function EditDatabaseBasketList(sCustCode As String, sOrderCode As String, sItemCode As String,
                                    Optional mode As Integer = 0, Optional ByRef results As List(Of TResult) = Nothing) As Integer
        Dim dba As New DBAccess
        Dim dt As DataTable = Nothing
        'Dim sql = "SELECT * from T_BASKETS "

        Dim sql = "SELECT  T_BASKETS.ORDER_CODE, T_BASKETS.CUST_CODE, T_BASKETS.ITEM_CODE, T_BASKETS.AMOUNT,"
        sql += "           M_ITEMS.ITEM_PHOT, M_ITEMS.ITEM_NAME, M_ITEMS.ITEM_PRICE, M_ITEMS.ITEM_TAXPRICE "
        sql += " FROM  T_BASKETS LEFT OUTER JOIN M_ITEMS ON T_BASKETS.ITEM_CODE = M_ITEMS.ITEM_CODE "

        Dim where = " WHERE T_BASKETS.ORDER_CODE='" & sOrderCode & "' AND T_BASKETS.CUST_CODE='" & sCustCode & "'  "

        'Dim where = " WHERE ORDER_CODE='" & sOrderCode & "' AND CUST_CODE='" & sCustCode & "' "

        If sItemCode <> "" Then
            where += " AND T_BASKETS.ITEM_CODE='" & sItemCode & "' "
            where += " AND T_BASKETS.DECISION_FLAG = 0 "
        End If

        dba.Query(sql + where, dt)

        If dt.Rows.Count > 0 Then

            If mode = 0 Then
                ' 
                sql = "update T_BASKETS set DECISION_FLAG = 1 "
            ElseIf mode = 1 Then
                sql = "update T_BASKETS set DECISION_FLAG = 2 "

                Dim itemBase As New TResult
                itemBase.RES_YEAR = results(0).RES_YEAR
                itemBase.RES_MONTH = results(0).RES_MONTH
                itemBase.RES_DATE = results(0).RES_DATE
                itemBase.RES_TIME = results(0).RES_TIME
                itemBase.RES_DELITIME = results(0).RES_DELITIME
                itemBase.CARD_CODE = results(0).CARD_CODE
                itemBase.RES_DELICASH = results(0).RES_DELICASH
                itemBase.CREATE_DATE = results(0).CREATE_DATE

                results.Clear()
                Dim item As New TResult
                For r = 0 To dt.Rows.Count - 1
                    item = New TResult
                    item.ORDER_CODE = dt.Rows(r).Item("ORDER_CODE")
                    item.CUST_CODE = dt.Rows(r).Item("CUST_CODE")
                    item.ITEM_CODE = dt.Rows(r).Item("ITEM_CODE")

                    item.RES_AMOUNT = dt.Rows(r).Item("AMOUNT")
                    item.RES_PRICE = dt.Rows(r).Item("ITEM_TAXPRICE")

                    item.RES_YEAR = itemBase.RES_YEAR
                    item.RES_MONTH = itemBase.RES_MONTH
                    item.RES_DATE = itemBase.RES_DATE
                    item.RES_TIME = itemBase.RES_TIME
                    item.RES_DELITIME = itemBase.RES_DELITIME
                    item.CARD_CODE = itemBase.CARD_CODE
                    item.RES_DELICASH = itemBase.RES_DELICASH
                    item.CREATE_DATE = itemBase.CREATE_DATE

                    results.Add(item)
                Next
            End If

            If dba.Execute(sql + where) >= 0 Then

                Return 0
            Else
                Return -1
            End If
        End If

        Return 0

    End Function
    ' 買い物かご 取得
    Public Function GetDatabaseBasketList(sCustCode As String, sOrderCode As String,
                                   ByRef items As List(Of TBasket), ByRef totalA As String, ByRef totalB As String) As Integer

        Dim dba As New DBAccess
        Dim dt As DataTable = Nothing
        Dim sql = "SELECT  T_BASKETS.ORDER_CODE, T_BASKETS.CUST_CODE, T_BASKETS.ITEM_CODE, T_BASKETS.AMOUNT,"
        sql += "  M_ITEMS.ITEM_PHOT, M_ITEMS.ITEM_NAME, M_ITEMS.ITEM_PRICE, M_ITEMS.ITEM_TAXPRICE "
        sql += " FROM  T_BASKETS LEFT OUTER JOIN M_ITEMS ON T_BASKETS.ITEM_CODE = M_ITEMS.ITEM_CODE "
        sql += " WHERE T_BASKETS.ORDER_CODE='" & sOrderCode & "' AND T_BASKETS.CUST_CODE='" & sCustCode & "'  "

        dba.Query(sql, dt)

        Dim item As New TBasket
        Dim itotalA = 0, itotalB = 0
        For r = 0 To dt.Rows.Count - 1
            item = New TBasket
            item.CUST_CODE = sCustCode
            item.ORDER_CODE = sOrderCode
            item.ITEM_CODE = dt.Rows(r).Item("ITEM_CODE")
            item.AMOUNT = dt.Rows(r).Item("AMOUNT")
            item.ITEM_BODY.ITEM_NAME = dt.Rows(r).Item("ITEM_NAME")
            If InStr(dt.Rows(r).Item("ITEM_PHOT"), ".") > 0 Then
                item.ITEM_BODY.ITEM_PHOT = "~/Pics/" & dt.Rows(r).Item("ITEM_PHOT")
            Else
                item.ITEM_BODY.ITEM_PHOT = "~/Pics/" & dt.Rows(r).Item("ITEM_PHOT") & ".png"  '  "~/Pics/download01.jpg"
            End If
            item.ITEM_BODY.ITEM_PRICE = dt.Rows(r).Item("ITEM_PRICE")
            item.ITEM_BODY.ITEM_TAXPRICE = dt.Rows(r).Item("ITEM_TAXPRICE")

            items.Add(item)

            itotalA += Val(item.ITEM_BODY.ITEM_PRICE) * Val(item.AMOUNT)
            itotalB += Val(item.ITEM_BODY.ITEM_TAXPRICE) * Val(item.AMOUNT)
        Next

        totalA = itotalA.ToString
        totalB = itotalB.ToString
        Return 0
    End Function
    Public Function GetDatabaseBasketListCount(ByRef iCount As Integer) As Integer

        Dim cdt = Now.AddDays(-7).ToString("yyyyMMddHHmmss") ' 一週間前

        Dim dba As New DBAccess
        Dim dt As DataTable = Nothing
        Dim sql = "SELECT  T_BASKETS.* "
        sql += " FROM  T_BASKETS "
        sql += " WHERE T_BASKETS.ORDER_CODE <= '" & cdt & "' "

        dba.Query(sql, dt)

        iCount = dt.Rows.Count

        Return 0
    End Function
    ' 買い物かご 登録
    Public Function RegistDatabaseBasket(sCustCode As String, sOrderCode As String, iAmount As Integer, item As MItem) As Integer

        Dim sItemCode = item.ITEM_CODE
        Dim sAmount = CStr(iAmount)

        Dim dba As New DBAccess
        Dim dt As DataTable = Nothing
        Dim sql = "select * from T_BASKETS "

        sql += " WHERE ORDER_CODE='" & sOrderCode & "' AND CUST_CODE='" & sCustCode & "' AND ITEM_CODE='" & sItemCode & "' "

        dba.Query(sql, dt)

        If dt.Rows.Count > 0 Then

            PlusDatabaseBasketList(sCustCode, sOrderCode, sItemCode, 1, sAmount)

        Else
            sql = "insert into T_BASKETS (ORDER_CODE, CUST_CODE, ITEM_CODE, AMOUNT, DECISION_FLAG) " &
                  " values ('" & sOrderCode & "','" & sCustCode & "','" & sItemCode & "'," & sAmount & ",0) "

            If dba.Execute(sql) >= 0 Then

                Return 0
            Else
                Return -1
            End If
        End If

        Return 0

    End Function

    ' マスタメンテナンス 商品マスタ更新
    Public Function EditDatabaseItem(item As MItem) As Integer
        Dim dba As New DBAccess
        Dim sql = ""
        Dim where = " WHERE ITEM_CODE='" & item.ITEM_CODE & "' "
        where += "  "

        If item.DATE_START.Length = 8 OrElse item.DATE_END.Length = 8 Then

            sql = "update M_ITEMS set  "

            If item.DATE_START.Length = 8 Then
                sql += "DATE_START = '" & item.DATE_START & "' ,"
            End If
            If item.DATE_END.Length = 8 Then
                sql += "DATE_END = '" & item.DATE_END & "' "
            End If
            sql = sql.Trim(","c)

            sql += where
            If dba.Execute(sql) >= 0 Then

                Return 0
            Else
                Return -1
            End If
        Else
            Return -1
        End If

    End Function


    ' 購入実績
    Public Function RegistDatabaseResult(item As TResult) As Integer

        If IsNullorSpace(item.ORDER_CODE) OrElse IsNullorSpace(item.CUST_CODE) OrElse IsNullorSpace(item.ITEM_CODE) Then
            Return 0
        End If

        Dim dba As New DBAccess
        Dim dt As DataTable = Nothing
        Dim sql = "select * from T_RESULTS "
        sql += " WHERE ORDER_CODE = '" & item.ORDER_CODE & "' AND CUST_CODE = '" & item.CUST_CODE & "' AND ITEM_CODE = '" & item.ITEM_CODE & "' "

        dba.Query(sql, dt)
        If dt.Rows.Count > 0 Then

        Else
            sql = "insert into T_RESULTS (ORDER_CODE, CUST_CODE, ITEM_CODE, "
            sql += "RES_YEAR, RES_MONTH, RES_DATE, RES_TIME, RES_DELITIME "
            sql += ",CARD_CODE,RES_DELICASH,RES_AMOUNT,RES_PRICE,RES_STATUS,DELETE_FLAG,CREATE_DATE)"
            sql += " values ('" & item.ORDER_CODE & "','" & item.CUST_CODE & "','" & item.ITEM_CODE & "',"
            sql += "" & item.RES_YEAR & ", " & item.RES_MONTH & ", " & item.RES_DATE & ", " & item.RES_TIME & ", '" & item.RES_DELITIME & "',"
            sql += "'" & item.CARD_CODE & "','" & item.RES_DELICASH & "'," & item.RES_AMOUNT & "," & item.RES_PRICE & ", "
            sql += " " & item.RES_STATUS & "," & item.DELETE_FLAG & ",'" & item.CREATE_DATE & "'"
            sql += " )"
            If dba.Execute(sql) >= 0 Then
                Return 1
            Else
                Return -1
            End If
        End If

        Return 0
    End Function
    Public Function GetDatabaseResultList(ByRef items As List(Of TResult)) As Integer

        Dim dba As New DBAccess
        Dim dt As DataTable = Nothing
        Dim sql = "SELECT  T_RESULTS.*, " ' T_RESULTS.CUST_CODE, T_RESULTS.ITEM_CODE, T_RESULTS.RES_AMOUNT,
        sql += "  M_ITEMS.ITEM_PHOT, M_ITEMS.ITEM_NAME, M_ITEMS.ITEM_PRICE, M_ITEMS.ITEM_TAXPRICE "
        sql += " FROM  T_RESULTS LEFT OUTER JOIN M_ITEMS ON T_RESULTS.ITEM_CODE = M_ITEMS.ITEM_CODE "
        'sql += " WHERE T_BASKETS.ORDER_CODE='" & sOrderCode & "' AND T_BASKETS.CUST_CODE='" & sCustCode & "'  "

        dba.Query(sql, dt)
        Dim item As New TResult
        For r = 0 To dt.Rows.Count - 1
            item = New TResult
            item.CUST_CODE = dt.Rows(r).Item("CUST_CODE")
            item.ORDER_CODE = dt.Rows(r).Item("ORDER_CODE")
            item.ITEM_CODE = dt.Rows(r).Item("ITEM_CODE")
            item.RES_AMOUNT = dt.Rows(r).Item("RES_AMOUNT")
            item.CARD_CODE = dt.Rows(r).Item("CARD_CODE")
            item.CREATE_DATE = dt.Rows(r).Item("CREATE_DATE")
            item.DELETE_FLAG = dt.Rows(r).Item("DELETE_FLAG")
            item.RES_DATE = dt.Rows(r).Item("RES_DATE")
            item.RES_DELICASH = dt.Rows(r).Item("RES_DELICASH")
            item.RES_DELITIME = dt.Rows(r).Item("RES_DELITIME")
            item.RES_MONTH = dt.Rows(r).Item("RES_MONTH")
            item.RES_PRICE = dt.Rows(r).Item("RES_PRICE")
            item.RES_STATUS = dt.Rows(r).Item("RES_STATUS")
            item.RES_TIME = dt.Rows(r).Item("RES_TIME")
            item.RES_YEAR = dt.Rows(r).Item("RES_YEAR")

            item.ITEM_BODY.ITEM_NAME = GetDbString(dt.Rows(r).Item("ITEM_NAME"))
            item.ITEM_BODY.ITEM_PRICE = GetDbString(dt.Rows(r).Item("ITEM_PRICE"))
            item.ITEM_BODY.ITEM_TAXPRICE = GetDbString(dt.Rows(r).Item("ITEM_TAXPRICE"))

            items.Add(item)
        Next

        Return dt.Rows.Count
    End Function


    ' 購入実績-配送先
    Public Function RegistDatabaseResultorder(item As TResultorder, cust As MCust) As Integer

        If IsNullorSpace(item.ORDER_CODE) OrElse IsNullorSpace(item.CUST_CODE) Then
            Return 0
        End If

        Dim dba As New DBAccess
        Dim dt As DataTable = Nothing
        Dim sql = "select * from T_RESULTORDERS "
        sql += " WHERE ORDER_CODE = '" & item.ORDER_CODE & "' AND CUST_CODE = '" & item.CUST_CODE & "' "

        Try
            dba.Query(sql, dt)
            If dt.Rows.Count > 0 Then

            Else
                sql = "insert into T_RESULTORDERS (ORDER_CODE, CUST_CODE,  "
                sql += "RES_YEAR, RES_MONTH, RES_DATE, RES_TIME, RES_DELITIME "

                sql += ",CUST_ADDR1, CUST_ADDR2, CUST_ADDR3 "
                sql += ",CUST_NAME, CUST_KANA, CUST_POST "
                sql += ",CUST_TEL, CUST_MAIL "

                sql += ",METH_PAYM,CARRIAGE"
                sql += ",CUST_CARD,CUST_CARDNUM,CUST_SECU"

                sql += ",RES_STATUS,DELETE_FLAG,CREATE_DATE)"
                sql += " values ('" & item.ORDER_CODE & "','" & item.CUST_CODE & "',"
                sql += "" & item.RES_YEAR & ", " & item.RES_MONTH & ", " & item.RES_DATE & ", " & item.RES_TIME & ", '" & item.RES_DELITIME & "',"

                sql += "'" & item.CUST_ADDR1 & "','" & item.CUST_ADDR2 & "','" & item.CUST_ADDR3 & "',"
                sql += "'" & item.CUST_NAME & "','" & item.CUST_KANA & "','" & item.CUST_POST & "',"
                sql += "'" & item.CUST_TEL & "','" & item.CUST_MAIL & "',"

                sql += "" & item.METH_PAYM & "," & item.CARRIAGE & ", "
                sql += "'" & item.CUST_CARD & "','" & item.CUST_CARDNUM & "','" & item.CUST_SECU & "',"

                sql += " " & item.RES_STATUS & "," & item.DELETE_FLAG & ",'" & item.CREATE_DATE & "'"
                sql += " )"
                If dba.Execute(sql) >= 0 Then '
                    Return 1
                Else
                    Return -1
                End If
            End If
        Catch ex As Exception

            Return -2
        End Try

        Return 0
    End Function
    Public Function GetDatabaseResultorderList(ByRef items As List(Of TResultorder)) As Integer

        Return GetDatabaseResultorderList(items, Now, Now)
    End Function
    Public Function GetDatabaseResultorderList(ByRef items As List(Of TResultorder), startDate As DateTimeDB, destDate As DateTimeDB) As Integer

        Dim termS As DateTime, termD As DateTime

        termS = New DateTime(startDate.YEAR_VALUE, startDate.MONTH_VALUE, startDate.DATE_VALUE)
        termD = New DateTime(destDate.YEAR_VALUE, destDate.MONTH_VALUE, destDate.DATE_VALUE)

        Return GetDatabaseResultorderList(items, termS, termD)

    End Function
    Public Function GetDatabaseResultorderList(ByRef items As List(Of TResultorder), termS As DateTime, termD As DateTime) As Integer

        Dim whereTermS As String = ""
        Dim whereTermD As String = ""

        If termS = Nothing Then
            termS = DateTime.MinValue
        End If
        If termD = Nothing Then
            termD = DateTime.MinValue
        End If
        If termS > DateTime.MinValue Then
            'whereTermS = " RES_YEAR >= " & termS.Year & " AND RES_MONTH >= " & termS.Month & " AND RES_DATE >=" & termS.Day & " "
            whereTermS = " (RES_YEAR * 10000 + RES_MONTH * 100 + RES_DATE) >= " & (termS.Year * 10000 + termS.Month * 100 + termS.Day)
        End If
        If termD > DateTime.MinValue Then
            'whereTermD = " RES_YEAR <= " & termD.Year & " AND RES_MONTH <= " & termD.Month & " AND RES_DATE <=" & termD.Day & " "
            whereTermD = " (RES_YEAR * 10000 + RES_MONTH * 100 + RES_DATE) <= " & (termD.Year * 10000 + termD.Month * 100 + termD.Day)
        End If

        Dim dba As New DBAccess
        Dim dt As DataTable = Nothing
        Dim sql = "SELECT T_RESULTORDERS.* "
        sql += " FROM T_RESULTORDERS "
        sql += " WHERE DELETE_FLAG = 0 "

        If whereTermS <> "" Then
            sql += " AND " & whereTermS
        End If
        If whereTermD <> "" Then
            sql += " AND " & whereTermD
        End If


        dba.Query(sql, dt)
        Dim item As New TResultorder
        For r = 0 To dt.Rows.Count - 1
            item = New TResultorder
            item.CUST_CODE = dt.Rows(r).Item("CUST_CODE")
            item.ORDER_CODE = dt.Rows(r).Item("ORDER_CODE")

            item.RES_YEAR = dt.Rows(r).Item("RES_YEAR")
            item.RES_MONTH = dt.Rows(r).Item("RES_MONTH")
            item.RES_DATE = dt.Rows(r).Item("RES_DATE")
            item.RES_TIME = dt.Rows(r).Item("RES_TIME")
            item.RES_DELITIME = dt.Rows(r).Item("RES_DELITIME")
            item.CUST_ADDR1 = dt.Rows(r).Item("CUST_ADDR1") ' 都道府県
            item.CUST_ADDR2 = dt.Rows(r).Item("CUST_ADDR2") ' 市町村
            item.CUST_ADDR3 = dt.Rows(r).Item("CUST_ADDR3") ' 以降
            item.CUST_NAME = dt.Rows(r).Item("CUST_NAME")   ' 氏名
            item.CUST_KANA = dt.Rows(r).Item("CUST_KANA")   ' カナ
            item.CUST_POST = dt.Rows(r).Item("CUST_POST")   ' 郵便番号
            item.CUST_TEL = dt.Rows(r).Item("CUST_TEL")     ' 電話番号
            item.CUST_MAIL = dt.Rows(r).Item("CUST_MAIL")   ' メール
            item.METH_PAYM = dt.Rows(r).Item("METH_PAYM")   ' 支払い方法
            item.CARRIAGE = dt.Rows(r).Item("CARRIAGE")     ' 配送時間帯
            item.CUST_CARD = dt.Rows(r).Item("CUST_CARD")
            item.CUST_CARDNUM = dt.Rows(r).Item("CUST_CARDNUM")
            item.CUST_SECU = dt.Rows(r).Item("CUST_SECU")
            item.RES_STATUS = dt.Rows(r).Item("RES_STATUS")
            item.DELETE_FLAG = dt.Rows(r).Item("DELETE_FLAG")
            item.CREATE_DATE = dt.Rows(r).Item("CREATE_DATE")
            items.Add(item)
        Next

        Return dt.Rows.Count
    End Function
    ' 削除
    Public Function DeleteDatabaseResultorderList(sCustCode As String, sOrderCode As String, logicFlag As Boolean) As Integer
        Dim dba As New DBAccess

        Dim sql = "delete from T_RESULTORDERS "

        If logicFlag = False Then
            sql += " WHERE CUST_CODE='" & sCustCode & "' AND ORDER_CODE='" & sOrderCode & "'"

            If dba.Execute(sql) >= 0 Then
                Return 1
            Else
                Return -1
            End If

        Else
            sql = "update T_RESULTORDERS set DELETE_FLAG = 1 "
            sql += " WHERE CUST_CODE='" & sCustCode & "' AND ORDER_CODE='" & sOrderCode & "'"

            If dba.Execute(sql) >= 0 Then
                Return 1
            Else
                Return -1
            End If
        End If

        Return 0

    End Function



    ' Public

    Public Function IsNullorSpace(arg As Object) As Boolean

        If arg Is Nothing Then
            Return True
        ElseIf Trim(arg) = "" Then
            Return True
        End If

        Return False
    End Function

End Module
