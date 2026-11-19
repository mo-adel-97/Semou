Imports System.Data.SqlClient
Imports DevExpress.XtraReports.UI

Public Class EncodingOfferPrice
    Dim dbcontext As New SemouEntities
    Dim NOTES As String
    Dim REASON As String = ""
    Private Sub CmdClose_Click(sender As Object, e As EventArgs) Handles CmdClose.Click
        Me.Close()
    End Sub

    Private Sub CmdAddItem_Click(sender As Object, e As EventArgs) Handles CmdAddItem.Click
        Try
            Dim dbcontext As New SemouEntities
            Dim ASKIFFONDDEF = dbcontext.PriceLists.FirstOrDefault(Function(m) m.ChkOfferPrice = True)
            If (IsNothing(ASKIFFONDDEF)) Then
                MessageBox.Show("لا توجد قائمة اسعار مخصصة لعروض السعر برجاء مراجعه الادارة", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Error)
                Return
            Else
                Dim w As New AddServicesOfferPrice
                w.ShowDialog()
                If w.IS_Yes = True Then
                    Dim ii As Integer = w.MainFeesGrid.CurrentRow.Index
                    For Each row In w.MainFeesGrid.SelectedRows
                        PayGrid.Rows.Add()
                        Dim i As Integer = PayGrid.Rows.Count() - 1
                        PayGrid.Rows(i).Cells(0).Value = w.MainFeesGrid.Rows(ii).Cells(0).Value.ToString()
                        PayGrid.Rows(i).Cells(1).Value = w.MainFeesGrid.Rows(ii).Cells(1).Value.ToString()
                        PayGrid.Rows(i).Cells(2).Value = w.MainFeesGrid.Rows(ii).Cells(2).Value.ToString()
                        PayGrid.Rows(i).Cells(3).Value = 1
                        PayGrid.Rows(i).Cells(4).Value = w.MainFeesGrid.Rows(ii).Cells(3).Value.ToString()
                        PayGrid.Rows(i).Cells(5).Value = w.MainFeesGrid.Rows(ii).Cells(4).Value.ToString()
                        PayGrid.Rows(i).Cells(6).Value = 0
                        PayGrid.Rows(i).Cells(7).Value = w.MainFeesGrid.Rows(ii).Cells(3).Value.ToString()
                        PayGrid.Rows(i).Cells(8).Value = w.MainFeesGrid.Rows(ii).Cells(5).Value.ToString()
                    Next
                    PAYTOTAL()
                End If
            End If
        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try
    End Sub

    Private Sub PayGrid_CellContentClick(sender As Object, e As DataGridViewCellEventArgs) Handles PayGrid.CellContentClick

    End Sub
    Sub PAYTOTAL()
        Dim DIPLOMTAX As Double = 0
        Dim DIPLOMTOTAL As Double = 0
        Dim DIPLOMSUBTOTAL As Double = 0
        For i As Integer = 0 To PayGrid.Rows.Count - 1
            If PayGrid.Rows(i).Cells(2).Value <> Nothing Then
                DIPLOMTOTAL = Val(DIPLOMTOTAL) + (Val(PayGrid.Rows(i).Cells(4).Value))
                DIPLOMTAX = Val(DIPLOMTAX) + (Val(PayGrid.Rows(i).Cells(6).Value))
                DIPLOMSUBTOTAL = Val(DIPLOMSUBTOTAL) + (Val(PayGrid.Rows(i).Cells(7).Value))
            End If
        Next
        TxtFTotal1.Text = Math.Round(Val(DIPLOMTOTAL), 2)
        TxtFTotal2.Text = Math.Round(Val(DIPLOMTAX), 2)
        TxtFTotal3.Text = Math.Round(DIPLOMSUBTOTAL, 2)
    End Sub
    Private Sub PayGrid_CellEndEdit(sender As Object, e As DataGridViewCellEventArgs) Handles PayGrid.CellEndEdit
        Try
            Dim i As Integer = PayGrid.CurrentCell.RowIndex
            Dim j As Integer = PayGrid.CurrentCell.ColumnIndex
            If j = 4 Then
                If Val(PayGrid.Rows(i).Cells(5).Value) <> 0 Then
                    PayGrid.Rows(i).Cells(6).Value = Math.Round(Val(PayGrid.Rows(i).Cells(4).Value) * (Val(PayGrid.Rows(i).Cells(5).Value) / 100), 3)
                    PayGrid.Rows(i).Cells(7).Value = Val(PayGrid.Rows(i).Cells(4).Value) + Val(PayGrid.Rows(i).Cells(6).Value)
                Else
                    PayGrid.Rows(i).Cells(7).Value = PayGrid.Rows(i).Cells(4).Value
                End If
            ElseIf j = 7 Then
                If Val(PayGrid.Rows(i).Cells(5).Value) <> 0 Then
                    Dim VAT As Double = (1 + Val(PayGrid.Rows(i).Cells(5).Value) / 100)
                    PayGrid.Rows(i).Cells(4).Value = Math.Round(Val(PayGrid.Rows(i).Cells(7).Value) / Val(VAT), 3)
                    PayGrid.Rows(i).Cells(6).Value = Math.Round(Val(PayGrid.Rows(i).Cells(4).Value) * (Val(PayGrid.Rows(i).Cells(5).Value) / 100), 3)
                Else
                    PayGrid.Rows(i).Cells(4).Value = PayGrid.Rows(i).Cells(7).Value
                End If
            End If
            PAYTOTAL()
        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try
    End Sub

    Private Sub PayGrid_UserDeletedRow(sender As Object, e As DataGridViewRowEventArgs) Handles PayGrid.UserDeletedRow
        Try
            PAYTOTAL()
        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try
    End Sub

    Private Sub EncodingOfferPrice_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Try
            RegDocPosted.WHATFORMTYPEGUID("offerprice")
            TxtTypeGuid.Text = RegDocPosted.TYPEGUID
        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try
    End Sub
    Public Function GET_OFFER__NO() As Integer
        Dim MaxId = dbcontext.OfferPrices.OrderByDescending(Function(p) p.ID).FirstOrDefault()?.ID
        Dim MaxDay = New OfferPrice
        If (IsNothing(MaxId)) Then
            MaxId = 1
            TxtCode.Text = MaxId
        Else
            MaxId += 1
            MaxDay.ID = MaxId
            TxtCode.Text = MaxId
        End If
        Return MaxId
    End Function
    Sub ADDOFFERPRICE()
        Dim connection = dbcontext.Database.Connection
        Dim comm = New SqlCommand("AddOfferPrice", connection)
        comm.CommandType = CommandType.StoredProcedure
        comm.Parameters.AddWithValue("@Id", GET_OFFER__NO)
        comm.Parameters.AddWithValue("@Code", TxtCode.Text)
        comm.Parameters.AddWithValue("@OfferDate", Convert.ToDateTime(TxtDateReg.Value))
        comm.Parameters.AddWithValue("@StudentName", TxtStudentName.Text)
        comm.Parameters.AddWithValue("@StudenTel", TxtTel.Text)
        comm.Parameters.AddWithValue("@StudentId", TxtNationalId.Text)
        comm.Parameters.AddWithValue("@UserGuid", TxtUserGuid.Text)
        comm.Parameters.AddWithValue("@OfferTotal", Val(TxtFTotal1.Text))
        comm.Parameters.AddWithValue("@OfferTax", Val(TxtFTotal2.Text))
        comm.Parameters.AddWithValue("@OfferSubTotal", Val(TxtFTotal3.Text))
        comm.Parameters.AddWithValue("@IsUse", ChkStaut.Value)
        comm.Parameters.AddWithValue("@AccountNo", TxtAccountNo.Text)
        comm.Parameters.AddWithValue("@IBANNo", TxtIban.Text)
        If connection.State = ConnectionState.Open Then
            connection.Close()
        End If
        connection.Open()
        comm.ExecuteNonQuery()
        comm.Parameters.Clear()
    End Sub
    Sub SHOWOFFERPRICEGUID()
        Dim OFFERPRICE_GUID_ = dbcontext.OfferPrices.FirstOrDefault(Function(m) m.Code = TxtCode.Text)
        TxtGuid.Text = OFFERPRICE_GUID_.Guid.ToString()
    End Sub
    Sub ADDOFFERDETAIL()
        Dim connection = dbcontext.Database.Connection
        Dim comm = New SqlCommand("AddOfferPriceDetail", connection)
        comm.CommandType = CommandType.StoredProcedure
        For i As Integer = 0 To PayGrid.Rows.Count - 1
            If PayGrid.Rows(i).Cells(8).Value = Nothing Then
                Exit For
            Else
                comm.Parameters.AddWithValue("@OfferPriceGuid", TxtGuid.Text)
                comm.Parameters.AddWithValue("@OfferUnit", PayGrid.Rows(i).Cells(2).Value.ToString())
                comm.Parameters.AddWithValue("@OfferQty", PayGrid.Rows(i).Cells(3).Value.ToString())
                comm.Parameters.AddWithValue("@OfferCost", Val(PayGrid.Rows(i).Cells(4).Value))
                comm.Parameters.AddWithValue("@OfferTaxVat", Val(PayGrid.Rows(i).Cells(5).Value))
                comm.Parameters.AddWithValue("@OfferTaxValue", Val(PayGrid.Rows(i).Cells(6).Value))
                comm.Parameters.AddWithValue("@OfferSubtotal", Val(PayGrid.Rows(i).Cells(7).Value))
                comm.Parameters.AddWithValue("@DiplomGuid", PayGrid.Rows(i).Cells(8).Value.ToString())
            End If
            If connection.State = ConnectionState.Open Then
                connection.Close()
            End If
            connection.Open()
            Dim rs As Integer = comm.ExecuteNonQuery()
            If rs > 0 Then
                comm.Parameters.Clear()
            End If
        Next
    End Sub
    Sub ADD_USERACTION_()
        Dim connection = dbcontext.Database.Connection
        Dim comm = New SqlCommand("Add_UserAction", connection)
        comm.CommandType = CommandType.StoredProcedure
        comm.Parameters.AddWithValue("@ActionGuid", TxtGuid.Text)
        comm.Parameters.AddWithValue("@UserGuid", AllFiles.USER_GUID____)
        comm.Parameters.AddWithValue("@Date_", Convert.ToDateTime(DateTime.Now))
        comm.Parameters.AddWithValue("@Time_", Convert.ToDateTime(DateTime.Now.ToShortTimeString))
        comm.Parameters.AddWithValue("@Notes_", AllFiles.ActionType___)
        comm.Parameters.AddWithValue("@ActionTypeGuid", TxtTypeGuid.Text)
        comm.Parameters.AddWithValue("@ActionReason", REASON)
        comm.Parameters.AddWithValue("@ActionCode", TxtCode.Text)
        If connection.State = ConnectionState.Open Then
            connection.Close()
        End If
        connection.Open()
        comm.ExecuteNonQuery()
        comm.Parameters.Clear()
    End Sub
    Sub SHOWOFFERPRICEINFO()
        Dim connection = dbcontext.Database.Connection
        Dim comm As New SqlCommand
        comm = New SqlCommand("ShowOfferPriceInfo", connection)
        comm.CommandType = CommandType.StoredProcedure
        comm.Parameters.AddWithValue("@Code", TxtCode.Text)
        If connection.State = ConnectionState.Open Then
            connection.Close()
        End If
        connection.Open()
        comm.ExecuteNonQuery()
        Dim adp As New SqlDataAdapter(comm)
        Dim ds As DataSet = New DataSet()
        adp.Fill(ds, "ShowOfferPriceInfo")
        Dim COUNT__ As Integer = 0
        If ds.Tables(0).Rows.Count > 0 Then
            For i As Integer = 0 To ds.Tables(0).Rows.Count - 1
                TxtDateReg.Value = Convert.ToDateTime(ds.Tables(0).Rows(i)(0))
                ChkStaut.Value = ds.Tables(0).Rows(i)(1)
                TxtStudentName.Text = ds.Tables(0).Rows(i)(2).ToString()
                TxtTel.Text = ds.Tables(0).Rows(i)(3).ToString()
                TxtNationalId.Text = ds.Tables(0).Rows(i)(4).ToString()
                TxtAccountNo.Text = ds.Tables(0).Rows(i)(5).ToString()
                TxtIban.Text = ds.Tables(0).Rows(i)(6).ToString()
                TxtUserName.Text = ds.Tables(0).Rows(i)(7).ToString()
                TxtUserGuid.Text = ds.Tables(0).Rows(i)(8).ToString()
                TxtFTotal1.Text = ds.Tables(0).Rows(i)(9).ToString()
                TxtFTotal2.Text = ds.Tables(0).Rows(i)(10).ToString()
                TxtFTotal3.Text = ds.Tables(0).Rows(i)(11).ToString()
                TxtGuid.Text = ds.Tables(0).Rows(i)(12).ToString()
            Next
        End If
    End Sub
    Sub SHOWOFFERPRICEDETAIL()
        Dim connection = dbcontext.Database.Connection
        Dim comm As New SqlCommand("ShowOfferPriceDetail", connection)
        comm.Parameters.AddWithValue("@OfferPriceGuid", TxtGuid.Text)
        comm.CommandType = CommandType.StoredProcedure
        If connection.State = ConnectionState.Open Then
            connection.Close()
        End If
        connection.Open()
        comm.ExecuteNonQuery()
        Dim adp As New SqlDataAdapter(comm)
        Dim ds As DataSet = New DataSet()
        adp.Fill(ds, "ShowOfferPriceDetail")
        Dim COUNT__ As Integer = 0
        If ds.Tables(0).Rows.Count > 0 Then
            PayGrid.RowCount = ds.Tables(0).Rows.Count
            For i As Integer = 0 To ds.Tables(0).Rows.Count - 1
                PayGrid.Rows(COUNT__).Cells(0).Value = ds.Tables(0).Rows(i)(0).ToString()
                PayGrid.Rows(COUNT__).Cells(1).Value = ds.Tables(0).Rows(i)(1).ToString()
                PayGrid.Rows(COUNT__).Cells(2).Value = ds.Tables(0).Rows(i)(2).ToString()
                PayGrid.Rows(COUNT__).Cells(3).Value = ds.Tables(0).Rows(i)(3).ToString()
                PayGrid.Rows(COUNT__).Cells(4).Value = ds.Tables(0).Rows(i)(4).ToString()
                PayGrid.Rows(COUNT__).Cells(5).Value = ds.Tables(0).Rows(i)(5).ToString()
                PayGrid.Rows(COUNT__).Cells(6).Value = ds.Tables(0).Rows(i)(6).ToString()
                PayGrid.Rows(COUNT__).Cells(7).Value = ds.Tables(0).Rows(i)(7).ToString()
                PayGrid.Rows(COUNT__).Cells(8).Value = ds.Tables(0).Rows(i)(8).ToString()
                COUNT__ = COUNT__ + 1
            Next
        End If
    End Sub
    Sub FORSAVE()
        If AllFiles.FORSAVE("offerprice") = False Then
            Return
        Else
            AllFiles.ActionType___ = "جديد"
            ADDOFFERPRICE()
            SHOWOFFERPRICEGUID()
            ADDOFFERDETAIL()
            SHOWOFFERPRICEINFO()
            ADD_USERACTION_()
            MessageBox.Show("تم حفظ عرض السعر بنجاح", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Information)
        End If
    End Sub
    Function VALIDATE_DATA() As Boolean
        If TxtStudentName.Text = Nothing Then
            MessageBox.Show("برجاء ادخال اسم الطالب", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return False
        ElseIf TxtTel.Text = Nothing Then
            MessageBox.Show("برجاء ادخال رقم الهاتف", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return False
        ElseIf TxtNationalId.Text = Nothing Then
            MessageBox.Show("برجاء ادخال رقم الهوية", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return False
        ElseIf TxtAccountNo.Text = Nothing Then
            MessageBox.Show("برجاء ادخال رقم الحساب البنكي", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return False
        ElseIf TxtIban.Text = Nothing Then
            MessageBox.Show("برجاء ادخال رقم الايبان", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return False
        ElseIf PayGrid.Rows.Count = 0 Then
            MessageBox.Show("برجاء ادخال الخدمات الخاصة بعرض السعر", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return False
        End If
        Return True
    End Function
    Private Sub CmdSave_Click(sender As Object, e As EventArgs) Handles CmdSave.Click
        Try
            If VALIDATE_DATA() = False Then
                Return
            Else
                If TxtGuid.Text = Nothing Then
                    AllFiles.WHATONLINEUSERINFO()
                    TxtUserName.Text = AllFiles.ONLINE_USER_NAME____
                    TxtUserGuid.Text = AllFiles.USER_GUID____.ToString()
                    FORSAVE()
                End If
            End If
        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try
    End Sub

    Private Sub CmdFind_Click(sender As Object, e As EventArgs) Handles CmdFind.Click
        Try
            Dim w As New OfferPriceList
            w.ShowDialog()
            If w.IS_Yes = True Then
                Dim ii As Integer = w.BranchGrid.CurrentRow.Index
                TxtCode.Text = w.BranchGrid.Rows(ii).Cells(0).Value.ToString()
            End If
            If TxtCode.Text <> Nothing Then
                SHOWOFFERPRICEINFO()
                If TxtGuid.Text <> Nothing Then
                    SHOWOFFERPRICEDETAIL()
                End If
            End If
        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try
    End Sub
    Sub PRINTOFFERPRICE()
        Dim connection = dbcontext.Database.Connection
        Dim comm As New SqlCommand
        comm = New SqlCommand("PrintOfferPrice", connection)
        comm.Parameters.AddWithValue("@OfferPriceGuid", TxtGuid.Text)
        comm.CommandType = CommandType.StoredProcedure
        If connection.State = ConnectionState.Open Then
            connection.Close()
        End If
        connection.Open()
        comm.ExecuteNonQuery()
        Dim adp As New SqlDataAdapter(comm)
        Dim ds As DataSet = New DataSet()
        adp.Fill(ds, "PrintOfferPrice")
        Dim dt = ds.Tables(0)
        Dim report As New OfferPriceDoc
        report.DataSource = dt
        report.DataAdapter = adp
        report.DataMember = "DataTable1"
        report.RequestParameters = False
        Dim Tool_Report As ReportPrintTool = New ReportPrintTool(report)
        report.CreateDocument()
        Dim frm As New ViewReport
        frm.DocumentViewer1.DocumentSource = report
        frm.ShowDialog()
    End Sub
    Private Sub CmdPrint_Click(sender As Object, e As EventArgs) Handles CmdPrint.Click
        Try
            If TxtGuid.Text <> Nothing Then
                PRINTOFFERPRICE()
            End If
        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try
    End Sub
End Class