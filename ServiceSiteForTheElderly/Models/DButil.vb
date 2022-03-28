Public Module DButil

    Public Function GetDateTime(arg As Object) As DateTime?

        'Dim rtn As DateTime?

        If arg Is DBNull.Value Then
            Return Nothing
        Else
            Return arg
        End If

    End Function

    Public Function GetDbString(arg As Object) As String

        If arg Is DBNull.Value Then
            Return ""
        Else
            Return arg
        End If

    End Function

    Public Function GetDbInteger(arg As Object) As Integer
        If arg Is Nothing Then
            Return 0
        ElseIf arg Is DBNull.Value Then
            Return 0
        Else
            Return arg
        End If

    End Function

    ''' <summary>
    ''' 文字列に全角文字が含まれているか調べる。
    ''' </summary>
    ''' <param name="value">調べる対象の文字列。</param>
    ''' <returns>全角文字が含まれている場合はTrue、そうでない場合False。</returns>
    Public Function IsZenkaku(value As String) As Boolean

        If String.IsNullOrEmpty(value) Then
            Return False
        End If

        Dim byteLength As Integer

#If NETCOREAPP Then
    '.NET Core/.NET 5以降の場合
    System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance)
#End If

        byteLength = System.Text.Encoding.GetEncoding("shift_jis").GetByteCount(value)
        Return New System.Globalization.StringInfo(value).LengthInTextElements <> byteLength

    End Function

End Module

Public Class DateTimeDB

    Public Property YEAR_VALUE() As Integer
    Public Property MONTH_VALUE() As Integer
    Public Property DATE_VALUE() As Integer
    Public Property TIME_VALUE() As Integer

End Class
