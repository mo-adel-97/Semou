Imports System.Data.SqlClient

Public Class RegDocPosted
    Public Shared PRICELISTGUID As String
    Public Shared BRANCHNAME As String
    Public Shared BRANCHGUID As String
    Public Shared CHKDAILY As Boolean
    Public Shared CHKMADEN As Boolean
    Public Shared MADENGUID As String
    Public Shared CHKDAEN As Boolean
    Public Shared DAENGUID As String
    Public Shared COSTCENTERGUID As String
    Public Shared TYPEGUID As String
    Public Shared CHKOTHERFEES As Boolean
    Public Shared CHKSTARTPAY As Boolean
    Public Shared DOCPAYNAME As String
    Public Shared DOCPAYGUID As String
    Public Shared DOCBRANCHNAME As String
    Public Shared DOCBRANCHGUID As String
    Public Shared CHKPAYAMOUNT As Boolean
    Public Shared PAYAMOUNT As Double
    Public Shared PAYDOCCODE As String
    Public Shared VATACCOUNTGUID As String
    Public Shared CHKVAT As Boolean

    Public Shared Sub SHOWREFSETTINGINFO(DOCCODE As String, DOCTYPEGUID As Guid)
        Dim dbcontext As New SemouEntities
        Dim connection = dbcontext.Database.Connection
        Dim comm As New SqlCommand
        comm = New SqlCommand("ShowRegSetting", connection)
        comm.CommandType = CommandType.StoredProcedure
        comm.Parameters.AddWithValue("@Code", DOCCODE)
        comm.Parameters.AddWithValue("@DocTypeGuid", DOCTYPEGUID)
        If connection.State = ConnectionState.Open Then
            connection.Close()
        End If
        connection.Open()
        comm.ExecuteNonQuery()
        Dim adp As New SqlDataAdapter(comm)
        Dim ds As DataSet = New DataSet()
        adp.Fill(ds, "ShowRegSetting")
        Dim COUNT__ As Integer = 0
        If ds.Tables(0).Rows.Count > 0 Then
            For i As Integer = 0 To ds.Tables(0).Rows.Count - 1
                PRICELISTGUID = ds.Tables(0).Rows(i)(2).ToString()
                BRANCHNAME = ds.Tables(0).Rows(i)(3).ToString()
                BRANCHGUID = ds.Tables(0).Rows(i)(4).ToString()
                CHKDAILY = ds.Tables(0).Rows(i)(5)
                CHKMADEN = ds.Tables(0).Rows(i)(6)
                MADENGUID = ds.Tables(0).Rows(i)(8).ToString()
                CHKDAEN = ds.Tables(0).Rows(i)(9)
                DAENGUID = ds.Tables(0).Rows(i)(11).ToString()
                COSTCENTERGUID = ds.Tables(0).Rows(i)(13).ToString()
                CHKOTHERFEES = ds.Tables(0).Rows(i)(18)
                CHKSTARTPAY = ds.Tables(0).Rows(i)(19)
                DOCPAYNAME = ds.Tables(0).Rows(i)(22).ToString()
                DOCPAYGUID = ds.Tables(0).Rows(i)(23).ToString()
                DOCBRANCHNAME = ds.Tables(0).Rows(i)(24).ToString()
                DOCBRANCHGUID = ds.Tables(0).Rows(i)(25).ToString()
                CHKPAYAMOUNT = ds.Tables(0).Rows(i)(20)
                PAYAMOUNT = Val(ds.Tables(0).Rows(i)(21))
                PAYDOCCODE = ds.Tables(0).Rows(i)(26).ToString()
                VATACCOUNTGUID = ds.Tables(0).Rows(i)(29).ToString()
                CHKVAT = ds.Tables(0).Rows(i)(30)
            Next
        End If
    End Sub
    Public Shared Sub WHATFORMTYPEGUID(FORMNAME As String)
        Dim dbcontext As New SemouEntities
        Dim connection = dbcontext.Database.Connection
        Dim comm As New SqlCommand
        comm = New SqlCommand("FormTypeGuid", connection)
        comm.CommandType = CommandType.StoredProcedure
        comm.Parameters.AddWithValue("@FormName", FORMNAME)
        If connection.State = ConnectionState.Open Then
            connection.Close()
        End If
        connection.Open()
        comm.ExecuteNonQuery()
        Dim adp As New SqlDataAdapter(comm)
        Dim ds As DataSet = New DataSet()
        adp.Fill(ds, "FormTypeGuid")
        Dim COUNT__ As Integer = 0
        If ds.Tables(0).Rows.Count > 0 Then
            For i As Integer = 0 To ds.Tables(0).Rows.Count - 1
                TYPEGUID = ds.Tables(0).Rows(i)(0).ToString()
            Next
        End If
    End Sub
End Class
