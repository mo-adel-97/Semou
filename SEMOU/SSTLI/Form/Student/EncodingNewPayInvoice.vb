Imports System.Data.SqlClient
Imports System.IO
Imports DevExpress.XtraReports.UI
Imports System.Globalization
Imports System.Net.Http
Imports System.Net.Http.Headers
Imports System.Text
Imports System.Threading.Tasks
Imports Newtonsoft.Json
Imports Newtonsoft.Json.Linq
Imports Formatting = Newtonsoft.Json.Formatting

Public Class EncodingNewPayInvoice
    Dim dbcontext As New SemouEntities
    Dim NOTES As String
    Dim REASON As String = ""
    Dim OPENGUID As Guid
    Dim NOGUID As String = "00000000-0000-0000-0000-000000000000"
    Dim BILLNOTES As String = ""
    Dim CASHDAYNOTES As String = ""
    Private client As HttpClient
    Dim jsonResult As String
    Sub PRINTINVOICE()
        AllFiles.PRINTTAXINVOICE(TxtGuid.Text, TxtPayBranchGuid.Text, TxtPayNotes.Text)
    End Sub
    Private Sub CmdPrint_Click(sender As Object, e As EventArgs) Handles CmdPrint.Click
        Try
            If TxtGuid.Text <> Nothing Then
                PRINTINVOICE()
            Else
                MessageBox.Show("برجاء حفظ الفاتورة الحالية اولا", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Error)
                Return
            End If
        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try
    End Sub

    Private Sub CmdClose_Click(sender As Object, e As EventArgs) Handles CmdClose.Click
        Me.Close()
    End Sub
    'Private Sub Txtref_Load(sender As Object, e As EventArgs) Handles MyBase.Load
    '    TxtRef.ContextMenuStrip = New ContextMenuStrip  ' إخفاء القائمة السياقية
    'End Sub
    Private Sub EncodingNewPayInvoice_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Try
            client = New HttpClient()
            'TxtRef.ContextMenuStrip = Nothing
            RegDocPosted.WHATFORMTYPEGUID("salesinvoice")
            TxtTypeGuid.Text = RegDocPosted.TYPEGUID
            RegDocPosted.WHATFORMTYPEGUID("receipt")
            TxtRecepitTypeGuid.Text = RegDocPosted.TYPEGUID
            RegDocPosted.WHATFORMTYPEGUID("opendaily")
            TxtOpenDailyType.Text = RegDocPosted.TYPEGUID
            OPENGUID = Guid.Parse(TxtOpenDailyType.Text)
            AllFiles.CHEKDEFBRANCHINFO(AllFiles.USER_GUID____.ToString)
            If AllFiles.ZEROAMOUNT = True Then
                TxtZeroAmount.Text = 1
            Else
                TxtZeroAmount.Text = 0
            End If
        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try
    End Sub
    Sub CHKACCOUNT()
        RegDocPosted.SHOWREFSETTINGINFO(TxtPayDocCode.Text, Guid.Parse(TxtTypeGuid.Text))
        TxtPayBranchName.Text = RegDocPosted.BRANCHNAME.ToString()
        TxtPayBranchGuid.Text = RegDocPosted.BRANCHGUID.ToString()
        TxtVatGuid.Text = RegDocPosted.VATACCOUNTGUID.ToString()
        ChkVat.Checked = RegDocPosted.CHKVAT
        TxtPriceListGuid.Text = RegDocPosted.PRICELISTGUID.ToString()
    End Sub
    Private Sub CmdFind_Click(sender As Object, e As EventArgs) Handles CmdFind.Click
        Try
            Dim w As New DocRegSettingList
            w.TxtTypeGuid.Text = TxtTypeGuid.Text
            w.ShowDialog()
            If w.IS_Yes = True Then
                Dim ii As Integer = w.BranchGrid.CurrentRow.Index
                TxtPayDocCode.Text = w.BranchGrid.Rows(ii).Cells(0).Value.ToString()
                TxtPayDocName.Text = w.BranchGrid.Rows(ii).Cells(1).Value.ToString()
                TxtPayDocGuid.Text = w.BranchGrid.Rows(ii).Cells(2).Value.ToString()
            End If
            If TxtPayDocGuid.Text <> Nothing Then
                CHKACCOUNT()
            End If
        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try
    End Sub

    Private Sub CmdFindCashBox_Click(sender As Object, e As EventArgs) Handles CmdFindCashBox.Click
        Try
            Dim w As New CashBoxList
            w.Text = "قائمة البنوك /الخزائن"
            w.TxtUseInSales.Text = True
            w.TxtIsUse.Text = True
            w.ShowDialog()
            If w.IS_Yes = True Then
                Dim ii As Integer = w.BranchGrid.CurrentRow.Index
                TxtCashBoxName.Text = w.BranchGrid.Rows(ii).Cells(1).Value.ToString()
                TxtCashBoxGuid.Text = w.BranchGrid.Rows(ii).Cells(3).Value.ToString()
                TxtCostCenterName.Text = w.BranchGrid.Rows(ii).Cells(4).Value.ToString()
                TxtCostCenterGuid.Text = w.BranchGrid.Rows(ii).Cells(5).Value.ToString()
            End If
            'If TxtCashBoxGuid.Text <> Nothing Then
            '    If TxtCashBoxName.Text <> "صندوق المكتب" Then
            '        CmdAttach1.Enabled = True
            '    Else
            '        CmdAttach1.Enabled = False
            '        TxtAttach1Path.Text = Nothing
            '    End If
            'End If
        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try
    End Sub

    Private Sub CmdFindCostCenter_Click(sender As Object, e As EventArgs) Handles CmdFindCostCenter.Click
        Try
            Dim w As New CostCenterList
            w.TxtAccountType.Text = 1
            w.ShowDialog()
            If w.IS_Yes = True Then
                Dim ii As Integer = w.BranchGrid.CurrentRow.Index
                TxtCostCenterName.Text = w.BranchGrid.Rows(ii).Cells(1).Value.ToString()
                TxtCostCenterGuid.Text = w.BranchGrid.Rows(ii).Cells(3).Value.ToString()
            End If
        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try
    End Sub
    Sub SHOWREGDOCDIPLOM(TYPE As Integer)
        PayGrid.Rows.Clear()
        Dim connection = dbcontext.Database.Connection
        Dim comm As New SqlCommand("ShowRegInfoForPay", connection)
        comm.Parameters.AddWithValue("@RegDocGuid", TxtRegDocGuid.Text)
        comm.Parameters.AddWithValue("@Type", TYPE)
        comm.CommandType = CommandType.StoredProcedure
        If connection.State = ConnectionState.Open Then
            connection.Close()
        End If
        connection.Open()
        comm.ExecuteNonQuery()
        Dim adp As New SqlDataAdapter(comm)
        Dim ds As DataSet = New DataSet()
        adp.Fill(ds, "ShowRegInfoForPay")
        Dim COUNT__ As Integer = 0
        If ds.Tables(0).Rows.Count > 0 Then
            PayGrid.RowCount = ds.Tables(0).Rows.Count
            For i As Integer = 0 To ds.Tables(0).Rows.Count - 1
                PayGrid.Rows(COUNT__).Cells(0).Value = ds.Tables(0).Rows(i)(0).ToString()
                PayGrid.Rows(COUNT__).Cells(1).Value = ds.Tables(0).Rows(i)(1).ToString()
                PayGrid.Rows(COUNT__).Cells(2).Value = ds.Tables(0).Rows(i)(2).ToString()
                PayGrid.Rows(COUNT__).Cells(3).Value = ds.Tables(0).Rows(i)(3).ToString()
                PayGrid.Rows(COUNT__).Cells(4).Value = ds.Tables(0).Rows(i)(6).ToString()
                PayGrid.Rows(COUNT__).Cells(5).Value = ds.Tables(0).Rows(i)(7).ToString()
                PayGrid.Rows(COUNT__).Cells(6).Value = ds.Tables(0).Rows(i)(4).ToString()
                PayGrid.Rows(COUNT__).Cells(7).Value = ds.Tables(0).Rows(i)(8).ToString()
                PayGrid.Rows(COUNT__).Cells(8).Value = ds.Tables(0).Rows(i)(5).ToString()
                PayGrid.Rows(COUNT__).Cells(9).Value = ds.Tables(0).Rows(i)(9).ToString()
                COUNT__ = COUNT__ + 1
            Next
            PAYTOTAL()
        Else
            CmdAddItem.Visible = True
        End If
    End Sub
    Sub SHOWDIPLOMFROMSTUDENTLEVEL(DIPLOMGUID As String)
        PayGrid.Rows.Clear()
        Dim connection = dbcontext.Database.Connection
        Dim comm As New SqlCommand("GetDiplomInfo", connection)
        comm.Parameters.AddWithValue("@Guid", DIPLOMGUID)
        comm.CommandType = CommandType.StoredProcedure
        If connection.State = ConnectionState.Open Then
            connection.Close()
        End If
        connection.Open()
        comm.ExecuteNonQuery()
        Dim adp As New SqlDataAdapter(comm)
        Dim ds As DataSet = New DataSet()
        adp.Fill(ds, "GetDiplomInfo")
        Dim COUNT__ As Integer = 0
        If ds.Tables(0).Rows.Count > 0 Then
            PayGrid.RowCount = ds.Tables(0).Rows.Count
            For i As Integer = 0 To ds.Tables(0).Rows.Count - 1
                PayGrid.Rows(COUNT__).Cells(0).Value = ds.Tables(0).Rows(i)(0).ToString()
                PayGrid.Rows(COUNT__).Cells(1).Value = ds.Tables(0).Rows(i)(1).ToString()
                PayGrid.Rows(COUNT__).Cells(2).Value = ds.Tables(0).Rows(i)(2).ToString()
                PayGrid.Rows(COUNT__).Cells(3).Value = ds.Tables(0).Rows(i)(3).ToString()
                PayGrid.Rows(COUNT__).Cells(4).Value = ds.Tables(0).Rows(i)(4).ToString()
                If TxtStudentNational.Text = 0 Then ''''''' مواطن
                    PayGrid.Rows(COUNT__).Cells(5).Value = ds.Tables(0).Rows(i)(5).ToString()
                ElseIf TxtStudentNational.Text = 1 Then ''''''أجنبي
                    PayGrid.Rows(COUNT__).Cells(5).Value = 15
                End If
                PayGrid.Rows(COUNT__).Cells(6).Value = ds.Tables(0).Rows(i)(6).ToString()
                PayGrid.Rows(COUNT__).Cells(7).Value = ds.Tables(0).Rows(i)(7).ToString()
                PayGrid.Rows(COUNT__).Cells(8).Value = ds.Tables(0).Rows(i)(8).ToString()
                PayGrid.Rows(COUNT__).Cells(9).Value = ds.Tables(0).Rows(i)(9).ToString()
                COUNT__ = COUNT__ + 1
            Next
        End If
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
        TxtPayTotal.Text = Math.Round(Val(DIPLOMTOTAL), 2)
        TxtPayTax.Text = Math.Round(Val(DIPLOMTAX), 2)
        TxtPaySubTotal.Text = Math.Round(DIPLOMSUBTOTAL, 2)
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
    Public Function GET_PAY__NO() As Integer
        Dim MaxId = dbcontext.Bills.OrderByDescending(Function(p) p.ID).FirstOrDefault()?.ID
        Dim MaxDay = New Bill
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
    Sub ADDBILLBYJSON()
        Dim connection = dbcontext.Database.Connection
        Dim comm = New SqlCommand("AddBillByJson", connection)
        comm.CommandType = CommandType.StoredProcedure
        comm.Parameters.AddWithValue("@BillCode", TxtCode.Text)
        comm.Parameters.AddWithValue("@BillGuid", TxtGuid.Text)
        comm.Parameters.AddWithValue("@JsonCintent", jsonResult.ToString())
        comm.Parameters.AddWithValue("@ZacatType", 388)
        If connection.State = ConnectionState.Open Then
            connection.Close()
        End If
        connection.Open()
        comm.ExecuteNonQuery()
        comm.Parameters.Clear()
    End Sub
    Sub ADDBILL()
        Dim connection = dbcontext.Database.Connection
        Dim comm = New SqlCommand("AddBill", connection)
        comm.CommandType = CommandType.StoredProcedure
        comm.Parameters.AddWithValue("@BillDate", Convert.ToDateTime(TxtDateReg.Value))
        comm.Parameters.AddWithValue("@DocGuid", TxtPayDocGuid.Text)
        comm.Parameters.AddWithValue("@BranchGuid", TxtPayBranchGuid.Text)
        comm.Parameters.AddWithValue("@CostCenterGuid", TxtCostCenterGuid.Text)
        comm.Parameters.AddWithValue("@UserGuid", TxtUserGuid.Text)
        comm.Parameters.AddWithValue("@AccountGuid", TxtAccountGuid.Text)
        comm.Parameters.AddWithValue("@BillTotal", Val(TxtPayTotal.Text))
        comm.Parameters.AddWithValue("@BillTax", Val(TxtPayTax.Text))
        comm.Parameters.AddWithValue("@BillSubTotal", Val(TxtPaySubTotal.Text))
        comm.Parameters.AddWithValue("@Notes", TxtPayNotes.Text)
        comm.Parameters.AddWithValue("@IsUse", ChkStaut.Value)
        comm.Parameters.AddWithValue("@Refrance", TxtRef.Text)
        comm.Parameters.AddWithValue("@CashBoxGuid", TxtCashBoxGuid.Text)
        comm.Parameters.AddWithValue("@ChkPayDate", ChkPayDate.Checked)
        comm.Parameters.AddWithValue("@PayDate", Convert.ToDateTime(TxtPayDate.Value))
        comm.Parameters.AddWithValue("@ChkOtherBank", ChlOtherBank.Checked)
        If ChlOtherBank.Checked = True Then
            comm.Parameters.AddWithValue("@CashBoxguid2", TxtCashBoxGuid2.Text)
            comm.Parameters.AddWithValue("@CostCentGuid2", TxtCostCenterGuid2.Text)
        Else
            comm.Parameters.AddWithValue("@CashBoxguid2", NOGUID)
            comm.Parameters.AddWithValue("@CostCentGuid2", NOGUID)
        End If
        comm.Parameters.AddWithValue("@ChkPayDate2", ChkPayDate2.Checked)
        comm.Parameters.AddWithValue("@PayDate2", Convert.ToDateTime(TxtPayDate2.Value))
        comm.Parameters.AddWithValue("@Ref2", TxtRef2.Text)
        ' إضافة معامل إخراج لاستلام @Code
        Dim outputParam As New SqlParameter("@CodeOut", SqlDbType.Float)
        outputParam.Direction = ParameterDirection.Output
        comm.Parameters.Add(outputParam)
        If connection.State = ConnectionState.Open Then
            connection.Close()
        End If
        connection.Open()
        comm.ExecuteNonQuery()
        ' استرجاع القيمة الفعلية لمعامل الإخراج
        Dim RETURNCODE As Double = Convert.ToDouble(outputParam.Value)
        TxtCode.Text = RETURNCODE.ToString()
        comm.Parameters.Clear()
    End Sub
    Sub SHOWBILLDIPLOM()
        PayGrid.Rows.Clear()
        Dim connection = dbcontext.Database.Connection
        Dim comm As New SqlCommand("ShowBillDetail", connection)
        comm.Parameters.AddWithValue("@BillGuid", TxtGuid.Text)
        comm.Parameters.AddWithValue("@Type_", 1)
        comm.CommandType = CommandType.StoredProcedure
        If connection.State = ConnectionState.Open Then
            connection.Close()
        End If
        connection.Open()
        comm.ExecuteNonQuery()
        Dim adp As New SqlDataAdapter(comm)
        Dim ds As DataSet = New DataSet()
        adp.Fill(ds, "ShowBillDetail")
        Dim COUNT__ As Integer = 0
        If ds.Tables(0).Rows.Count > 0 Then
            PayGrid.RowCount = ds.Tables(0).Rows.Count
            For i As Integer = 0 To ds.Tables(0).Rows.Count - 1
                PayGrid.Rows(COUNT__).Cells(0).Value = ds.Tables(0).Rows(i)(0).ToString()
                PayGrid.Rows(COUNT__).Cells(1).Value = ds.Tables(0).Rows(i)(1).ToString()
                TxtDiplomName.Text = ds.Tables(0).Rows(i)(1).ToString()
                PayGrid.Rows(COUNT__).Cells(2).Value = ds.Tables(0).Rows(i)(2).ToString()
                PayGrid.Rows(COUNT__).Cells(3).Value = ds.Tables(0).Rows(i)(3).ToString()
                PayGrid.Rows(COUNT__).Cells(4).Value = ds.Tables(0).Rows(i)(4).ToString()
                PayGrid.Rows(COUNT__).Cells(5).Value = ds.Tables(0).Rows(i)(5).ToString()
                PayGrid.Rows(COUNT__).Cells(6).Value = ds.Tables(0).Rows(i)(6).ToString()
                PayGrid.Rows(COUNT__).Cells(7).Value = ds.Tables(0).Rows(i)(7).ToString()
                PayGrid.Rows(COUNT__).Cells(8).Value = ds.Tables(0).Rows(i)(8).ToString()
                PayGrid.Rows(COUNT__).Cells(9).Value = ds.Tables(0).Rows(i)(9).ToString()
                COUNT__ = COUNT__ + 1
            Next
        End If
    End Sub
    Sub SHOWBILLINFO()
        Dim connection = dbcontext.Database.Connection
        Dim comm As New SqlCommand("ShowBillInfo", connection)
        comm.Parameters.AddWithValue("@Code", TxtCode.Text)
        comm.CommandType = CommandType.StoredProcedure
        If connection.State = ConnectionState.Open Then
            connection.Close()
        End If
        connection.Open()
        comm.ExecuteNonQuery()
        Dim adp As New SqlDataAdapter(comm)
        Dim ds As DataSet = New DataSet()
        adp.Fill(ds, "ShowBillInfo")
        Dim COUNT__ As Integer = 0
        If ds.Tables(0).Rows.Count > 0 Then
            For i As Integer = 0 To ds.Tables(0).Rows.Count - 1
                TxtDateReg.Value = Convert.ToDateTime(ds.Tables(0).Rows(i)(0))
                TxtPayDocName.Text = ds.Tables(0).Rows(i)(1).ToString()
                TxtPayDocGuid.Text = ds.Tables(0).Rows(i)(2).ToString()
                TxtPayDocCode.Text = ds.Tables(0).Rows(i)(3).ToString()
                TxtPayBranchGuid.Text = ds.Tables(0).Rows(i)(4).ToString()
                TxtPayBranchName.Text = ds.Tables(0).Rows(i)(5).ToString()
                TxtCostCenterGuid.Text = ds.Tables(0).Rows(i)(6).ToString()
                TxtCostCenterName.Text = ds.Tables(0).Rows(i)(7).ToString()
                TxtUserGuid.Text = ds.Tables(0).Rows(i)(8).ToString()
                TxtUserName.Text = ds.Tables(0).Rows(i)(9).ToString()
                TxtAccountGuid.Text = ds.Tables(0).Rows(i)(10).ToString()
                TxtStudentName.Text = ds.Tables(0).Rows(i)(11).ToString()
                TxtTel.Text = ds.Tables(0).Rows(i)(12).ToString()
                TxtNationalId.Text = ds.Tables(0).Rows(i)(13).ToString()
                TxtPayNotes.Text = ds.Tables(0).Rows(i)(17).ToString()
                ChkStaut.Value = ds.Tables(0).Rows(i)(18)
                TxtGuid.Text = ds.Tables(0).Rows(i)(19).ToString()
                TxtRef.Text = ds.Tables(0).Rows(i)(20).ToString()
                TxtCashBoxGuid.Text = ds.Tables(0).Rows(i)(21).ToString()
                TxtCashBoxName.Text = ds.Tables(0).Rows(i)(22).ToString()
                TxtDay1PayCode.Text = ds.Tables(0).Rows(i)(23).ToString()
                TxtDay1PayGuid.Text = ds.Tables(0).Rows(i)(24).ToString()
                TxtDay1RecepitCode.Text = ds.Tables(0).Rows(i)(25).ToString()
                TxtDay1RecpitGuid.Text = ds.Tables(0).Rows(i)(26).ToString()
                TxtRecepitCode.Text = ds.Tables(0).Rows(i)(27).ToString()
                TxtRecepitGuid.Text = ds.Tables(0).Rows(i)(28).ToString()
                TxtVatGuid.Text = ds.Tables(0).Rows(i)(29).ToString()
                ChkVat.Checked = ds.Tables(0).Rows(i)(30)
                ChkPayDate.Checked = ds.Tables(0).Rows(i)(31)
                TxtPayDate.Value = Convert.ToDateTime(ds.Tables(0).Rows(i)(32)).ToShortDateString()
                ChlOtherBank.Checked = ds.Tables(0).Rows(i)(35)
                SHOWRECEIPT1INFO(TxtCode.Text, TxtCashBoxGuid.Text)
                If ds.Tables(0).Rows(i)(35) = True Then
                    'TxtAmount1.Text = ds.Tables(0).Rows(i)(36).ToString()
                    TxtCashBoxName2.Text = ds.Tables(0).Rows(i)(37).ToString()
                    TxtCashBoxGuid2.Text = ds.Tables(0).Rows(i)(38).ToString()
                    TxtCostCenterName2.Text = ds.Tables(0).Rows(i)(39).ToString()
                    TxtCostCenterGuid2.Text = ds.Tables(0).Rows(i)(40).ToString()
                    ChkPayDate2.Checked = ds.Tables(0).Rows(i)(41)
                    TxtPayDate2.Value = Convert.ToDateTime(ds.Tables(0).Rows(i)(42)).ToShortDateString()
                    SHOWRECEIPT2INFO(TxtCode.Text, TxtCashBoxGuid2.Text, TxtRecepitCode.Text)
                End If
                BILLNOTES = ds.Tables(0).Rows(i)(44).ToString()
                CASHDAYNOTES = ds.Tables(0).Rows(i)(45).ToString()
                COUNT__ = COUNT__ + 1
            Next
        End If
    End Sub
    Sub SHOWRECEIPT1INFO(BILLCODE As String, CASHGUID As String)
        Dim connection = dbcontext.Database.Connection
        Dim comm As New SqlCommand("ShowReceipt2Info", connection)
        comm.Parameters.AddWithValue("@BillCode", BILLCODE)
        comm.Parameters.AddWithValue("@CashBoxGuid", CASHGUID)
        comm.CommandType = CommandType.StoredProcedure
        If connection.State = ConnectionState.Open Then
            connection.Close()
        End If
        connection.Open()
        comm.ExecuteNonQuery()
        Dim adp As New SqlDataAdapter(comm)
        Dim ds As DataSet = New DataSet()
        adp.Fill(ds, "ShowReceipt2Info")
        If ds.Tables(0).Rows.Count > 0 Then
            For i As Integer = 0 To ds.Tables(0).Rows.Count - 1
                TxtRecepitCode.Text = ds.Tables(0).Rows(i)(0).ToString()
                TxtRecepitGuid.Text = ds.Tables(0).Rows(i)(1).ToString()
                TxtDay1RecepitCode.Text = ds.Tables(0).Rows(i)(2).ToString()
                TxtDay1RecpitGuid.Text = ds.Tables(0).Rows(i)(3).ToString()
                TxtAmount1.Text = ds.Tables(0).Rows(i)(4).ToString()
                TxtRef.Text = ds.Tables(0).Rows(i)(5).ToString()
            Next
        End If
    End Sub
    Sub SHOWRECEIPT2INFO(BILLCODE As String, CASHGUID As String, RECEIPT1 As String)
        Dim connection = dbcontext.Database.Connection
        Dim comm As New SqlCommand("ShowReceipt2Info", connection)
        comm.Parameters.AddWithValue("@BillCode", BILLCODE)
        comm.Parameters.AddWithValue("@CashBoxGuid", CASHGUID)
        comm.Parameters.AddWithValue("@ReceiptCode", RECEIPT1)
        comm.CommandType = CommandType.StoredProcedure
        If connection.State = ConnectionState.Open Then
            connection.Close()
        End If
        connection.Open()
        comm.ExecuteNonQuery()
        Dim adp As New SqlDataAdapter(comm)
        Dim ds As DataSet = New DataSet()
        adp.Fill(ds, "ShowReceipt2Info")
        If ds.Tables(0).Rows.Count > 0 Then
            For i As Integer = 0 To ds.Tables(0).Rows.Count - 1
                TxtRecepitCode2.Text = ds.Tables(0).Rows(i)(0).ToString()
                TxtRecepitGuid2.Text = ds.Tables(0).Rows(i)(1).ToString()
                TxtDay1RecepitCode2.Text = ds.Tables(0).Rows(i)(2).ToString()
                TxtDay1RecpitGuid2.Text = ds.Tables(0).Rows(i)(3).ToString()
                TxtAmount2.Text = ds.Tables(0).Rows(i)(4).ToString()
                TxtRef2.Text = ds.Tables(0).Rows(i)(5).ToString()
            Next
        End If
    End Sub
    Sub ADDBILLITEM()
        Dim connection = dbcontext.Database.Connection
        Dim comm = New SqlCommand("AddRegDocDiplom", connection)
        comm.CommandType = CommandType.StoredProcedure
        For i As Integer = 0 To PayGrid.Rows.Count - 1
            If PayGrid.Rows(i).Cells(8).Value = Nothing Then
                Exit For
            Else
                comm.Parameters.AddWithValue("@RegDocGuid", TxtGuid.Text)
                comm.Parameters.AddWithValue("@DiplomGuid", PayGrid.Rows(i).Cells(8).Value.ToString())
                comm.Parameters.AddWithValue("@Cost", Val(PayGrid.Rows(i).Cells(4).Value))
                comm.Parameters.AddWithValue("@Tax", Val(PayGrid.Rows(i).Cells(6).Value))
                comm.Parameters.AddWithValue("@SubTotal", Val(PayGrid.Rows(i).Cells(7).Value))
                comm.Parameters.AddWithValue("@DiplomType", PayGrid.Rows(i).Cells(9).Value.ToString())
                comm.Parameters.AddWithValue("@Type_", 1)
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
    Public Function GETDAY1_PAY_NO() As Integer
        Dim MaxId = dbcontext.Day1.Where(Function(m) m.TypeGuid <> OPENGUID).OrderByDescending(Function(p) p.ID).FirstOrDefault()?.ID
        Dim MaxDay = New Day1
        If (IsNothing(MaxId)) Then
            MaxId = 1
            TxtDay1PayCode.Text = MaxId
        Else
            MaxId += 1
            MaxDay.ID = MaxId
            TxtDay1PayCode.Text = MaxId
        End If
        Return MaxId
    End Function
    Sub ADDPAYDAY1()
        Dim connection = dbcontext.Database.Connection
        Dim comm = New SqlCommand("AddDay1", connection)
        comm.CommandType = CommandType.StoredProcedure
        comm.Parameters.AddWithValue("@OpenGuid", OPENGUID)
        comm.Parameters.AddWithValue("@DayDate", Convert.ToDateTime(TxtDateReg.Value))
        comm.Parameters.AddWithValue("@Notes", NOTES)
        comm.Parameters.AddWithValue("@DayType", True)
        comm.Parameters.AddWithValue("@DocGuid", TxtGuid.Text)
        comm.Parameters.AddWithValue("@IsUse", True)
        comm.Parameters.AddWithValue("@TypeGuid", TxtTypeGuid.Text)
        comm.Parameters.AddWithValue("@ActionCode", TxtCode.Text)
        comm.Parameters.AddWithValue("@BranchGuid", TxtPayBranchGuid.Text)
        ' إضافة معامل إخراج لاستلام @Code
        Dim outputParam As New SqlParameter("@CodeOut", SqlDbType.Float)
        outputParam.Direction = ParameterDirection.Output
        comm.Parameters.Add(outputParam)
        If connection.State = ConnectionState.Open Then
            connection.Close()
        End If
        connection.Open()
        comm.ExecuteNonQuery()
        ' استرجاع القيمة الفعلية لمعامل الإخراج
        Dim RETURNCODE As Double = Convert.ToDouble(outputParam.Value)
        TxtDay1PayCode.Text = RETURNCODE.ToString()
        comm.Parameters.Clear()
    End Sub
    Sub SHOWPAYDAY1INFO()
        Dim connection = dbcontext.Database.Connection
        Dim comm As New SqlCommand("ShowDay1InfoForDoc", connection)
        comm.Parameters.AddWithValue("@DocGuid", TxtGuid.Text)
        comm.CommandType = CommandType.StoredProcedure
        If connection.State = ConnectionState.Open Then
            connection.Close()
        End If
        connection.Open()
        comm.ExecuteNonQuery()
        Dim adp As New SqlDataAdapter(comm)
        Dim ds As DataSet = New DataSet()
        adp.Fill(ds, "ShowDay1InfoForDoc")
        Dim COUNT__ As Integer = 0
        If ds.Tables(0).Rows.Count > 0 Then
            For i As Integer = 0 To ds.Tables(0).Rows.Count - 1
                TxtDay1PayCode.Text = ds.Tables(0).Rows(i)(0).ToString()
                TxtDay1PayGuid.Text = ds.Tables(0).Rows(i)(1).ToString()
                COUNT__ = COUNT__ + 1
            Next
        End If
    End Sub
    Sub ADDPAYDAY2_MADEN()
        Dim connection = dbcontext.Database.Connection
        Dim comm = New SqlCommand("AddDay2", connection)
        comm.CommandType = CommandType.StoredProcedure
        comm.Parameters.AddWithValue("@Day1Guid", TxtDay1PayGuid.Text)
        If RegDocPosted.CHKMADEN = True Then
            comm.Parameters.AddWithValue("@AccountGuid", TxtAccountGuid.Text)
        Else
            comm.Parameters.AddWithValue("@AccountGuid", RegDocPosted.MADENGUID)
        End If
        If ChkVat.Checked = True Then
            comm.Parameters.AddWithValue("@Maden", Val(TxtPaySubTotal.Text))
        Else
            comm.Parameters.AddWithValue("@Maden", Val(TxtPayTotal.Text))
        End If
        comm.Parameters.AddWithValue("@Daen", 0)
        comm.Parameters.AddWithValue("@Notes", NOTES)
        comm.Parameters.AddWithValue("@CostGuid", TxtCostCenterGuid.Text)
        comm.Parameters.AddWithValue("@IsUse", True)
        If connection.State = ConnectionState.Open Then
            connection.Close()
        End If
        connection.Open()
        comm.ExecuteNonQuery()
        comm.Parameters.Clear()
    End Sub
    Sub ADDPAYDAY2_DAEN()
        Dim connection = dbcontext.Database.Connection
        Dim comm = New SqlCommand("AddDay2", connection)
        comm.CommandType = CommandType.StoredProcedure
        comm.Parameters.AddWithValue("@Day1Guid", TxtDay1PayGuid.Text)
        If RegDocPosted.CHKDAEN = True Then
            comm.Parameters.AddWithValue("@AccountGuid", TxtAccountGuid.Text)
        Else
            comm.Parameters.AddWithValue("@AccountGuid", RegDocPosted.DAENGUID)
        End If
        comm.Parameters.AddWithValue("@Maden", 0)
        comm.Parameters.AddWithValue("@Daen", Val(TxtPayTotal.Text))
        comm.Parameters.AddWithValue("@Notes", NOTES)
        comm.Parameters.AddWithValue("@CostGuid", TxtCostCenterGuid.Text)
        comm.Parameters.AddWithValue("@IsUse", True)
        If connection.State = ConnectionState.Open Then
            connection.Close()
        End If
        connection.Open()
        comm.ExecuteNonQuery()
        comm.Parameters.Clear()
    End Sub
    Public Function GET_RECEPIT__NO() As Integer
        Dim MaxId = dbcontext.CashDays.Where(Function(m) m.CashDayType = 0).OrderByDescending(Function(p) p.Id).FirstOrDefault()?.Id
        Dim MaxDay = New CashDay
        If (IsNothing(MaxId)) Then
            MaxId = 1
            TxtRecepitCode.Text = MaxId
        Else
            MaxId += 1
            MaxDay.Id = MaxId
            TxtRecepitCode.Text = MaxId
        End If
        Return MaxId
    End Function
    Sub ADDRECEPIT()
        Dim connection = dbcontext.Database.Connection
        Dim comm = New SqlCommand("AddCashDay", connection)
        comm.CommandType = CommandType.StoredProcedure
        'comm.Parameters.AddWithValue("@Id", TxtRecepitCode.Text)
        'comm.Parameters.AddWithValue("@Code", TxtRecepitCode.Text)
        comm.Parameters.AddWithValue("@CashDayDate", Convert.ToDateTime(TxtDateReg.Value))
        comm.Parameters.AddWithValue("@FAccountGuid", TxtCashBoxGuid.Text)
        If ChlOtherBank.Checked = True Then
            comm.Parameters.AddWithValue("@Amount", Val(TxtAmount1.Text))
        Else
            comm.Parameters.AddWithValue("@Amount", Val(TxtPaySubTotal.Text))
        End If
        comm.Parameters.AddWithValue("@BranchGuid", TxtPayBranchGuid.Text)
        comm.Parameters.AddWithValue("@Notes", NOTES)
        comm.Parameters.AddWithValue("@LinkGuid", TxtTypeGuid.Text)
        comm.Parameters.AddWithValue("@LinkRef", TxtCode.Text)
        comm.Parameters.AddWithValue("@CashDayType", 0)
        comm.Parameters.AddWithValue("@IsUse", ChkStaut.Value)
        comm.Parameters.AddWithValue("@Ref", TxtRef.Text)
        comm.Parameters.AddWithValue("@PayDate", Convert.ToDateTime(TxtPayDate.Value))
        ' إضافة معامل إخراج لاستلام @Code
        Dim outputParam As New SqlParameter("@CodeOut", SqlDbType.Float)
        outputParam.Direction = ParameterDirection.Output
        comm.Parameters.Add(outputParam)
        If connection.State = ConnectionState.Open Then
            connection.Close()
        End If
        connection.Open()
        comm.ExecuteNonQuery()
        ' استرجاع القيمة الفعلية لمعامل الإخراج
        Dim RETURNCODE As Double = Convert.ToDouble(outputParam.Value)
        TxtRecepitCode.Text = RETURNCODE.ToString()
        comm.Parameters.Clear()
    End Sub
    Sub SHOWRECEPITINFO()
        Dim connection = dbcontext.Database.Connection
        Dim comm As New SqlCommand("ShowCashdayInfo", connection)
        comm.Parameters.AddWithValue("@Code", TxtRecepitCode.Text)
        comm.Parameters.AddWithValue("@CashType", 0)
        comm.CommandType = CommandType.StoredProcedure
        If connection.State = ConnectionState.Open Then
            connection.Close()
        End If
        connection.Open()
        comm.ExecuteNonQuery()
        Dim adp As New SqlDataAdapter(comm)
        Dim ds As DataSet = New DataSet()
        adp.Fill(ds, "ShowCashdayInfo")
        Dim COUNT__ As Integer = 0
        If ds.Tables(0).Rows.Count > 0 Then
            For i As Integer = 0 To ds.Tables(0).Rows.Count - 1
                TxtRecepitGuid.Text = ds.Tables(0).Rows(i)(10).ToString()
                COUNT__ = COUNT__ + 1
            Next
        End If
    End Sub
    Sub ADDRECEPITDETAILS()
        Dim connection = dbcontext.Database.Connection
        Dim comm = New SqlCommand("AddCashDayDetail", connection)
        comm.CommandType = CommandType.StoredProcedure
        comm.Parameters.AddWithValue("@CashDayGuid", TxtRecepitGuid.Text)
        comm.Parameters.AddWithValue("@AccountGuid", TxtAccountGuid.Text)
        If ChlOtherBank.Checked = False Then
            comm.Parameters.AddWithValue("@Daen", Val(TxtPaySubTotal.Text))
        Else
            comm.Parameters.AddWithValue("@Daen", Val(TxtAmount1.Text))
        End If
        comm.Parameters.AddWithValue("@CostGuid", TxtCostCenterGuid.Text)
        comm.Parameters.AddWithValue("@Notes", NOTES)
        If connection.State = ConnectionState.Open Then
            connection.Close()
        End If
        connection.Open()
        comm.ExecuteNonQuery()
        comm.Parameters.Clear()
    End Sub
    Public Function GETDAY1_RECEPIT_NO() As Integer
        Dim MaxId = dbcontext.Day1.Where(Function(m) m.TypeGuid <> OPENGUID).OrderByDescending(Function(p) p.ID).FirstOrDefault()?.ID
        Dim MaxDay = New Day1
        If (IsNothing(MaxId)) Then
            MaxId = 1
            TxtDay1RecepitCode.Text = MaxId
        Else
            MaxId += 1
            MaxDay.ID = MaxId
            TxtDay1RecepitCode.Text = MaxId
        End If
        Return MaxId
    End Function
    Sub ADDRECEPITDAY1()
        Dim connection = dbcontext.Database.Connection
        Dim comm = New SqlCommand("AddDay1", connection)
        comm.CommandType = CommandType.StoredProcedure
        comm.Parameters.AddWithValue("@OpenGuid", OPENGUID)
        comm.Parameters.AddWithValue("@DayDate", Convert.ToDateTime(TxtDateReg.Value))
        comm.Parameters.AddWithValue("@Notes", NOTES)
        comm.Parameters.AddWithValue("@DayType", True)
        comm.Parameters.AddWithValue("@DocGuid", TxtRecepitGuid.Text)
        comm.Parameters.AddWithValue("@IsUse", True)
        comm.Parameters.AddWithValue("@TypeGuid", TxtRecepitTypeGuid.Text)
        comm.Parameters.AddWithValue("@ActionCode", TxtRecepitCode.Text)
        comm.Parameters.AddWithValue("@BranchGuid", TxtPayBranchGuid.Text)
        ' إضافة معامل إخراج لاستلام @Code
        Dim outputParam As New SqlParameter("@CodeOut", SqlDbType.Float)
        outputParam.Direction = ParameterDirection.Output
        comm.Parameters.Add(outputParam)
        If connection.State = ConnectionState.Open Then
            connection.Close()
        End If
        connection.Open()
        comm.ExecuteNonQuery()
        ' استرجاع القيمة الفعلية لمعامل الإخراج
        Dim RETURNCODE As Double = Convert.ToDouble(outputParam.Value)
        TxtDay1RecepitCode.Text = RETURNCODE.ToString()
        comm.Parameters.Clear()
    End Sub
    Sub SHOWRECEPITDAY1INFO()
        Dim connection = dbcontext.Database.Connection
        Dim comm As New SqlCommand("ShowDay1InfoForDoc", connection)
        comm.Parameters.AddWithValue("@DocGuid", TxtRecepitGuid.Text)
        comm.CommandType = CommandType.StoredProcedure
        If connection.State = ConnectionState.Open Then
            connection.Close()
        End If
        connection.Open()
        comm.ExecuteNonQuery()
        Dim adp As New SqlDataAdapter(comm)
        Dim ds As DataSet = New DataSet()
        adp.Fill(ds, "ShowDay1InfoForDoc")
        Dim COUNT__ As Integer = 0
        If ds.Tables(0).Rows.Count > 0 Then
            For i As Integer = 0 To ds.Tables(0).Rows.Count - 1
                TxtDay1RecepitCode.Text = ds.Tables(0).Rows(i)(0).ToString()
                TxtDay1RecpitGuid.Text = ds.Tables(0).Rows(i)(1).ToString()
                COUNT__ = COUNT__ + 1
            Next
        End If
    End Sub
    Sub ADDRECEPITDAY2_MADEN()
        Dim connection = dbcontext.Database.Connection
        Dim comm = New SqlCommand("AddDay2", connection)
        comm.CommandType = CommandType.StoredProcedure
        comm.Parameters.AddWithValue("@Day1Guid", TxtDay1RecpitGuid.Text)
        comm.Parameters.AddWithValue("@AccountGuid", TxtCashBoxGuid.Text)
        If ChlOtherBank.Checked = False Then
            comm.Parameters.AddWithValue("@Maden", Val(TxtPaySubTotal.Text))
        Else
            comm.Parameters.AddWithValue("@Maden", Val(TxtAmount1.Text))
        End If
        comm.Parameters.AddWithValue("@Daen", 0)
        comm.Parameters.AddWithValue("@Notes", NOTES)
        comm.Parameters.AddWithValue("@CostGuid", TxtCostCenterGuid.Text)
        comm.Parameters.AddWithValue("@IsUse", True)
        If connection.State = ConnectionState.Open Then
            connection.Close()
        End If
        connection.Open()
        comm.ExecuteNonQuery()
        comm.Parameters.Clear()
    End Sub
    Sub ADDRECEPITDAY2_DAEN()
        Dim connection = dbcontext.Database.Connection
        Dim comm = New SqlCommand("AddDay2", connection)
        comm.CommandType = CommandType.StoredProcedure
        comm.Parameters.AddWithValue("@Day1Guid", TxtDay1RecpitGuid.Text)
        comm.Parameters.AddWithValue("@AccountGuid", TxtAccountGuid.Text)
        comm.Parameters.AddWithValue("@Maden", 0)
        If ChlOtherBank.Checked = False Then
            comm.Parameters.AddWithValue("@Daen", Val(TxtPaySubTotal.Text))
        Else
            comm.Parameters.AddWithValue("@Daen", Val(TxtAmount1.Text))
        End If
        comm.Parameters.AddWithValue("@Notes", NOTES)
        comm.Parameters.AddWithValue("@CostGuid", TxtCostCenterGuid.Text)
        comm.Parameters.AddWithValue("@IsUse", True)
        If connection.State = ConnectionState.Open Then
            connection.Close()
        End If
        connection.Open()
        comm.ExecuteNonQuery()
        comm.Parameters.Clear()
    End Sub
    Sub ADDPAYTAXDAY2_DAEN()
        Dim connection = dbcontext.Database.Connection
        Dim comm = New SqlCommand("AddDay2", connection)
        comm.CommandType = CommandType.StoredProcedure
        comm.Parameters.AddWithValue("@Day1Guid", TxtDay1PayGuid.Text)
        comm.Parameters.AddWithValue("@AccountGuid", TxtVatGuid.Text)
        comm.Parameters.AddWithValue("@Maden", 0)
        comm.Parameters.AddWithValue("@Daen", Val(TxtPayTax.Text))
        comm.Parameters.AddWithValue("@Notes", NOTES)
        comm.Parameters.AddWithValue("@CostGuid", TxtCostCenterGuid.Text)
        comm.Parameters.AddWithValue("@IsUse", True)
        If connection.State = ConnectionState.Open Then
            connection.Close()
        End If
        connection.Open()
        comm.ExecuteNonQuery()
        comm.Parameters.Clear()
    End Sub
    Sub ADD_USERACTION_(GUID As String, TYPEGUID As String, DOCCODE As String)
        Dim connection = dbcontext.Database.Connection
        Dim comm = New SqlCommand("Add_UserAction", connection)
        comm.CommandType = CommandType.StoredProcedure
        comm.Parameters.AddWithValue("@ActionGuid", GUID)
        comm.Parameters.AddWithValue("@UserGuid", AllFiles.USER_GUID____)
        comm.Parameters.AddWithValue("@Date_", Convert.ToDateTime(DateTime.Now))
        comm.Parameters.AddWithValue("@Time_", Convert.ToDateTime(DateTime.Now.ToShortTimeString))
        comm.Parameters.AddWithValue("@Notes_", AllFiles.ActionType___)
        comm.Parameters.AddWithValue("@ActionTypeGuid", TYPEGUID)
        comm.Parameters.AddWithValue("@ActionReason", REASON)
        comm.Parameters.AddWithValue("@ActionCode", DOCCODE)
        If connection.State = ConnectionState.Open Then
            connection.Close()
        End If
        connection.Open()
        comm.ExecuteNonQuery()
        comm.Parameters.Clear()
    End Sub
    Sub SHOWBILLGUID()
        Dim REGDOC_GUID_ = dbcontext.Bills.FirstOrDefault(Function(m) m.Code = TxtCode.Text)
        TxtGuid.Text = REGDOC_GUID_.Guid.ToString()
    End Sub
    Public Function PAYNOEXISTS(PAYCODE As Integer) As Boolean
        Dim QUERY = dbcontext.Bills.FirstOrDefault(Function(m) m.ID = PAYCODE)
        If (IsNothing(QUERY)) Then
            Return False
        Else
            Return True
        End If
    End Function
    Public Function RECEIPTNOEXISTS(RECEIPTCODE As Integer) As Boolean
        Dim QUERY = dbcontext.CashDays.FirstOrDefault(Function(m) m.Id = RECEIPTCODE And m.CashDayType = 0)
        If (IsNothing(QUERY)) Then
            Return False
        Else
            Return True
        End If
    End Function
    Public Function DAY1PAYNOEXISTS(DAY1CODE As Integer) As Boolean
        Dim QUERY = dbcontext.Day1.FirstOrDefault(Function(m) m.ID = DAY1CODE And m.TypeGuid <> OPENGUID)
        If (IsNothing(QUERY)) Then
            Return False
        Else
            Return True
        End If
    End Function
    ' دالة إرسال الطلب إلى API
    'Private Async Function SendRequestAsync(userName As String, password As String) As Task(Of String)
    '    Dim request As New HttpRequestMessage(HttpMethod.Post, "http://localhost:2020/Auth/login")
    '    request.Headers.Add("accept", "*/*")

    '    ' إنشاء محتوى JSON بناءً على المدخلات
    '    Dim jsonContent As String = $"{{""userName"": ""{userName}"", ""password"": ""{password}""}}"
    '    request.Content = New StringContent(jsonContent, Encoding.UTF8, "application/json")

    '    Try
    '        Dim response As HttpResponseMessage = Await client.SendAsync(request)
    '        response.EnsureSuccessStatusCode()
    '        Dim responseBody As String = Await response.Content.ReadAsStringAsync()

    '        ' استخراج التوكن فقط
    '        Dim json As JObject = JObject.Parse(responseBody)
    '        Dim token As String = json("token")?.ToString()

    '        Return If(String.IsNullOrEmpty(token), "Token not found!", token)
    '    Catch ex As Exception
    '        Return "Error: " & ex.Message
    '    End Try
    'End Function
    Private Async Function GetInvoiceDataAsync(invoiceID As String) As Task(Of String)
        Dim invoiceData As New Dictionary(Of String, Object)
        Dim dbcontext As New SemouEntities
        Using connection = dbcontext.Database.Connection
            If connection.State = ConnectionState.Closed Then
                connection.Open()
            End If
            Try
                Using comm = New SqlCommand("ShowBillInfo", CType(connection, SqlConnection))
                    comm.CommandType = CommandType.StoredProcedure
                    comm.Parameters.AddWithValue("@Code", TxtCode.Text)

                    Using reader As SqlDataReader = Await comm.ExecuteReaderAsync()
                        If reader.HasRows Then
                            While reader.Read()
                                invoiceData("id") = AllFiles.GetTaxInvoiceNo(reader("Guid").ToString()) ' رقم الفاتورة الضريبية الجديد
                                invoiceData("issueDate") = Convert.ToDateTime(reader("BillDate")).ToString("yyyy-MM-dd")
                                invoiceData("issueTime") = Convert.ToDateTime(reader("BillDate")).ToString("HH:mm:ss")
                                invoiceData("customer") = New Dictionary(Of String, Object) From {
                                              {"registrationName", reader("StudentName").ToString()},
                                              {"taxId", Nothing},
                                              {"address", Nothing}
                                                                   }
                                invoiceData("invoiceTotals") = New Dictionary(Of String, Object) From {
                                             {"lineExtensionAmount", Convert.ToDecimal(reader("BillTotal")).ToString("F2", CultureInfo.InvariantCulture)},
                                             {"taxExclusiveAmount", Convert.ToDecimal(reader("BillTotal")).ToString("F2", CultureInfo.InvariantCulture)},
                                             {"taxInclusiveAmount", Convert.ToDecimal(reader("BillSubTotal")).ToString("F2", CultureInfo.InvariantCulture)},
                                             {"prepaidAmount", 0.00},
                                             {"payableAmount", Convert.ToDecimal(reader("BillSubTotal")).ToString("F2", CultureInfo.InvariantCulture)}
                                                                                                          }
                            End While
                        Else
                            MessageBox.Show("لم يتم العثور على بيانات الفاتورة!", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                            Return Nothing
                        End If
                    End Using
                End Using
            Catch ex As Exception
                MessageBox.Show(ex.ToString)
            End Try

            Dim invoiceLines As New List(Of Dictionary(Of String, Object))

            Using comm = New SqlCommand("ShowBillDetail", CType(connection, SqlConnection))
                comm.CommandType = CommandType.StoredProcedure
                comm.Parameters.AddWithValue("@BillGuid", TxtGuid.Text)
                comm.Parameters.AddWithValue("@Type_", 1)
                Using reader As SqlDataReader = Await comm.ExecuteReaderAsync()
                    While reader.Read()
                        invoiceLines.Add(New Dictionary(Of String, Object) From {
                            {"id", reader("Code").ToString()},
                            {"quantity", Convert.ToDecimal(reader("Qty")).ToString("F2", CultureInfo.InvariantCulture)},
                            {"itemName", reader("DiplomName").ToString()},
                            {"price", Convert.ToDecimal(reader("Cost")).ToString("F2", CultureInfo.InvariantCulture)},
                            {"taxRate", Convert.ToDecimal(reader("TaxValue")).ToString},
                            {"taxCategoryCode", reader("TAXCATOGREY").ToString()},
                            {"lineExtensionAmount", Convert.ToDecimal(reader("SubTotal")).ToString("F2", CultureInfo.InvariantCulture)},
                            {"taxAmount", Convert.ToDecimal(reader("Tax")).ToString("F2", CultureInfo.InvariantCulture)},
                            {"roundingAmount", Convert.ToDecimal(reader("SubTotal")).ToString("F2", CultureInfo.InvariantCulture)}
                        })
                    End While
                End Using
            End Using

            invoiceData("invoiceLines") = invoiceLines
        End Using

        'Return JsonConvert.SerializeObject(invoiceData, Formatting.Indented)
        jsonResult = JsonConvert.SerializeObject(invoiceData, Formatting.Indented)

    End Function
    'Private Async Function SendInvoiceAsync(jsonContent As String) As Task(Of String)
    '    'MessageBox.Show(jsonContent, "JSON Content")
    '    Dim client As New HttpClient()
    '    Dim request As New HttpRequestMessage(HttpMethod.Post, "http://localhost:2020/SendInvoice")

    '    request.Headers.Add("accept", "*/*")
    '    request.Headers.Authorization = New AuthenticationHeaderValue("Bearer", OtpSender.Token__)

    '    request.Content = New StringContent(jsonContent)
    '    request.Content.Headers.ContentType = New MediaTypeHeaderValue("application/json")

    '    Dim response As HttpResponseMessage = Await client.SendAsync(request)
    '    response.EnsureSuccessStatusCode()
    '    Return Await response.Content.ReadAsStringAsync()
    'End Function

    'Private Async Function SENDTOZATCK() As Task
    '    AllFiles.SHOWSYSTEMINFO()
    '    Dim userName As String = AllFiles.ZUSERNAME
    '    Dim password As String = AllFiles.ZPASSWORD

    '    If String.IsNullOrWhiteSpace(userName) OrElse String.IsNullOrWhiteSpace(password) Then
    '        MessageBox.Show("الرجاء إدخال اسم المستخدم وكلمة المرور.", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Warning)
    '        Exit Function
    '    End If

    '    Dim result As String = Await SendRequestAsync(userName, password)

    '    ' حفظ التوكن
    '    OtpSender.Token__ = result

    '    ''''''''''''''''''''''''''''''''' OTP
    '    Dim otp As String = AllFiles.ZOTP
    '    Dim sender2 As New OtpSender()
    '    Dim result2 As String = Await sender2.SendOtpAsync(otp)
    '    'MessageBox.Show(result2, "Response", MessageBoxButtons.OK, MessageBoxIcon.Information)

    '    '''''''''''''''''''''''''''''''''' Send Invoice
    '    Dim invoiceID As String = TxtCode.Text
    '    If String.IsNullOrEmpty(invoiceID) Then
    '        MessageBox.Show("يرجى إدخال كود الفاتورة!", "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error)
    '        Return
    '    End If

    '    Dim jsonContent As String = Await GetInvoiceDataAsync(invoiceID)
    '    If String.IsNullOrEmpty(jsonContent) Then
    '        MessageBox.Show("لم يتم العثور على بيانات الفاتورة!", "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error)
    '        Return
    '    End If
    '    Dim response As String = Await SendInvoiceAsync(jsonContent)
    'End Function
    Private Async Sub FORSAVE()
        If AllFiles.FORSAVE("salesinvoice") = False Then
            Return
        Else
            AllFiles.ActionType___ = "جديد"
            ADDBILL()
            SHOWBILLGUID()
            ADDBILLITEM()
            NOTES = "فاتورة مبيعات " & " " & TxtStudentName.Text & vbCrLf & " " & TxtPayDocName.Text & vbCrLf & TxtDiplomName.Text
            ADDPAYDAY1()
            SHOWPAYDAY1INFO()
            RegDocPosted.SHOWREFSETTINGINFO(TxtPayDocCode.Text, Guid.Parse(TxtTypeGuid.Text))
            ADDPAYDAY2_MADEN()
            ADDPAYDAY2_DAEN()
            If ChkVat.Checked = True And TxtPayTax.Text <> 0 Then
                ADDPAYTAXDAY2_DAEN()
            End If

            ' سند القبض
            NOTES = "سداد فاتورة مبيعات" & " " & TxtStudentName.Text & vbCrLf & " " & TxtPayDocName.Text & vbCrLf & TxtDiplomName.Text
            ADDRECEPIT()
            SHOWRECEPITINFO()
            ADDRECEPITDETAILS()
            ADDRECEPITDAY1()
            SHOWRECEPITDAY1INFO()
            ADDRECEPITDAY2_MADEN()
            ADDRECEPITDAY2_DAEN()
            ADD_USERACTION_(TxtRecepitGuid.Text, TxtRecepitTypeGuid.Text, TxtRecepitCode.Text)

            ' سند القبض الخاص بالحوالة الثانية إن وُجدت
            If ChlOtherBank.Checked = True Then
                ADDRECEPIT2()
                SHOWRECEPITINFO2()
                ADDRECEPITDETAILS2()
                ADDRECEPIT2DAY1()
                SHOWRECEPIT2DAY1INFO()
                ADDRECEPIT2DAY2_MADEN()
                ADDRECEPIT2DAY2_DAEN()
                ADD_USERACTION_(TxtRecepitGuid2.Text, TxtRecepitTypeGuid.Text, TxtRecepitCode2.Text)
            End If

            SHOWBILLINFO()
            Await GetInvoiceDataAsync(TxtCode.Text)
            ADDBILLBYJSON()
            ADD_USERACTION_(TxtGuid.Text, TxtTypeGuid.Text, TxtCode.Text)
            ' استدعاء إرسال الفاتورة للزكاة والدخل
            MessageBox.Show("تم حفظ فاتورة المبيعات بنجاح", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Information)
        End If
    End Sub

    Sub ADDRECEPIT2DAY2_DAEN()
        Dim connection = dbcontext.Database.Connection
        Dim comm = New SqlCommand("AddDay2", connection)
        comm.CommandType = CommandType.StoredProcedure
        comm.Parameters.AddWithValue("@Day1Guid", TxtDay1RecpitGuid2.Text)
        comm.Parameters.AddWithValue("@AccountGuid", TxtAccountGuid.Text)
        comm.Parameters.AddWithValue("@Maden", 0)
        comm.Parameters.AddWithValue("@Daen", Val(TxtAmount2.Text))
        comm.Parameters.AddWithValue("@Notes", NOTES)
        comm.Parameters.AddWithValue("@CostGuid", TxtCostCenterGuid2.Text)
        comm.Parameters.AddWithValue("@IsUse", True)
        If connection.State = ConnectionState.Open Then
            connection.Close()
        End If
        connection.Open()
        comm.ExecuteNonQuery()
        comm.Parameters.Clear()
    End Sub
    Sub ADDRECEPIT2DAY2_MADEN()
        Dim connection = dbcontext.Database.Connection
        Dim comm = New SqlCommand("AddDay2", connection)
        comm.CommandType = CommandType.StoredProcedure
        comm.Parameters.AddWithValue("@Day1Guid", TxtDay1RecpitGuid2.Text)
        comm.Parameters.AddWithValue("@AccountGuid", TxtCashBoxGuid2.Text)
        comm.Parameters.AddWithValue("@Maden", Val(TxtAmount2.Text))
        comm.Parameters.AddWithValue("@Daen", 0)
        comm.Parameters.AddWithValue("@Notes", NOTES)
        comm.Parameters.AddWithValue("@CostGuid", TxtCostCenterGuid2.Text)
        comm.Parameters.AddWithValue("@IsUse", True)
        If connection.State = ConnectionState.Open Then
            connection.Close()
        End If
        connection.Open()
        comm.ExecuteNonQuery()
        comm.Parameters.Clear()
    End Sub
    Sub SHOWRECEPIT2DAY1INFO()
        Dim connection = dbcontext.Database.Connection
        Dim comm As New SqlCommand("ShowDay1InfoForDoc", connection)
        comm.Parameters.AddWithValue("@DocGuid", TxtRecepitGuid2.Text)
        comm.CommandType = CommandType.StoredProcedure
        If connection.State = ConnectionState.Open Then
            connection.Close()
        End If
        connection.Open()
        comm.ExecuteNonQuery()
        Dim adp As New SqlDataAdapter(comm)
        Dim ds As DataSet = New DataSet()
        adp.Fill(ds, "ShowDay1InfoForDoc")
        Dim COUNT__ As Integer = 0
        If ds.Tables(0).Rows.Count > 0 Then
            For i As Integer = 0 To ds.Tables(0).Rows.Count - 1
                TxtDay1RecepitCode2.Text = ds.Tables(0).Rows(i)(0).ToString()
                TxtDay1RecpitGuid2.Text = ds.Tables(0).Rows(i)(1).ToString()
                COUNT__ = COUNT__ + 1
            Next
        End If
    End Sub
    Sub ADDRECEPIT2DAY1()
        Dim connection = dbcontext.Database.Connection
        Dim comm = New SqlCommand("AddDay1", connection)
        comm.CommandType = CommandType.StoredProcedure
        comm.Parameters.AddWithValue("@OpenGuid", OPENGUID)
        comm.Parameters.AddWithValue("@DayDate", Convert.ToDateTime(TxtDateReg.Value))
        comm.Parameters.AddWithValue("@Notes", NOTES)
        comm.Parameters.AddWithValue("@DayType", True)
        comm.Parameters.AddWithValue("@DocGuid", TxtRecepitGuid2.Text)
        comm.Parameters.AddWithValue("@IsUse", True)
        comm.Parameters.AddWithValue("@TypeGuid", TxtRecepitTypeGuid.Text)
        comm.Parameters.AddWithValue("@ActionCode", TxtRecepitCode2.Text)
        comm.Parameters.AddWithValue("@BranchGuid", TxtPayBranchGuid.Text)
        ' إضافة معامل إخراج لاستلام @Code
        Dim outputParam As New SqlParameter("@CodeOut", SqlDbType.Float)
        outputParam.Direction = ParameterDirection.Output
        comm.Parameters.Add(outputParam)
        If connection.State = ConnectionState.Open Then
            connection.Close()
        End If
        connection.Open()
        comm.ExecuteNonQuery()
        ' استرجاع القيمة الفعلية لمعامل الإخراج
        Dim RETURNCODE As Double = Convert.ToDouble(outputParam.Value)
        TxtDay1RecepitCode2.Text = RETURNCODE.ToString()
        comm.Parameters.Clear()
    End Sub
    Public Function GETDAY1_RECEPIT2_NO() As Integer
        Dim MaxId = dbcontext.Day1.Where(Function(m) m.TypeGuid <> OPENGUID).OrderByDescending(Function(p) p.ID).FirstOrDefault()?.ID
        Dim MaxDay = New Day1
        If (IsNothing(MaxId)) Then
            MaxId = 1
            TxtDay1RecepitCode2.Text = MaxId
        Else
            MaxId += 1
            MaxDay.ID = MaxId
            TxtDay1RecepitCode2.Text = MaxId
        End If
        Return MaxId
    End Function
    Public Function GET_RECEPIT2__NO() As Integer
        Dim MaxId = dbcontext.CashDays.Where(Function(m) m.CashDayType = 0).OrderByDescending(Function(p) p.Id).FirstOrDefault()?.Id
        Dim MaxDay = New CashDay
        If (IsNothing(MaxId)) Then
            MaxId = 1
            TxtRecepitCode2.Text = MaxId
        Else
            MaxId += 1
            MaxDay.Id = MaxId
            TxtRecepitCode2.Text = MaxId
        End If
        Return MaxId
    End Function
    Sub ADDRECEPIT2()
        Dim connection = dbcontext.Database.Connection
        Dim comm = New SqlCommand("AddCashDay", connection)
        comm.CommandType = CommandType.StoredProcedure
        'comm.Parameters.AddWithValue("@Id", TxtRecepitCode2.Text)
        'comm.Parameters.AddWithValue("@Code", TxtRecepitCode2.Text)
        comm.Parameters.AddWithValue("@CashDayDate", Convert.ToDateTime(TxtDateReg.Value))
        comm.Parameters.AddWithValue("@FAccountGuid", TxtCashBoxGuid2.Text)
        comm.Parameters.AddWithValue("@Amount", Val(TxtAmount2.Text))
        comm.Parameters.AddWithValue("@BranchGuid", TxtPayBranchGuid.Text)
        comm.Parameters.AddWithValue("@Notes", NOTES)
        comm.Parameters.AddWithValue("@LinkGuid", TxtTypeGuid.Text)
        comm.Parameters.AddWithValue("@LinkRef", TxtCode.Text)
        comm.Parameters.AddWithValue("@CashDayType", 0)
        comm.Parameters.AddWithValue("@IsUse", ChkStaut.Value)
        comm.Parameters.AddWithValue("@Ref", TxtRef2.Text)
        comm.Parameters.AddWithValue("@PayDate", Convert.ToDateTime(TxtPayDate2.Value))
        ' إضافة معامل إخراج لاستلام @Code
        Dim outputParam As New SqlParameter("@CodeOut", SqlDbType.Float)
        outputParam.Direction = ParameterDirection.Output
        comm.Parameters.Add(outputParam)
        If connection.State = ConnectionState.Open Then
            connection.Close()
        End If
        connection.Open()
        comm.ExecuteNonQuery()
        ' استرجاع القيمة الفعلية لمعامل الإخراج
        Dim RETURNCODE As Double = Convert.ToDouble(outputParam.Value)
        TxtRecepitCode2.Text = RETURNCODE.ToString()
        comm.Parameters.Clear()
    End Sub
    Sub SHOWRECEPITINFO2()
        Dim connection = dbcontext.Database.Connection
        Dim comm As New SqlCommand("ShowCashdayInfo", connection)
        comm.Parameters.AddWithValue("@Code", TxtRecepitCode2.Text)
        comm.Parameters.AddWithValue("@CashType", 0)
        comm.CommandType = CommandType.StoredProcedure
        If connection.State = ConnectionState.Open Then
            connection.Close()
        End If
        connection.Open()
        comm.ExecuteNonQuery()
        Dim adp As New SqlDataAdapter(comm)
        Dim ds As DataSet = New DataSet()
        adp.Fill(ds, "ShowCashdayInfo")
        Dim COUNT__ As Integer = 0
        If ds.Tables(0).Rows.Count > 0 Then
            For i As Integer = 0 To ds.Tables(0).Rows.Count - 1
                TxtRecepitGuid2.Text = ds.Tables(0).Rows(i)(10).ToString()
                COUNT__ = COUNT__ + 1
            Next
        End If
    End Sub
    Sub ADDRECEPITDETAILS2()
        Dim connection = dbcontext.Database.Connection
        Dim comm = New SqlCommand("AddCashDayDetail", connection)
        comm.CommandType = CommandType.StoredProcedure
        comm.Parameters.AddWithValue("@CashDayGuid", TxtRecepitGuid2.Text)
        comm.Parameters.AddWithValue("@AccountGuid", TxtAccountGuid.Text)
        comm.Parameters.AddWithValue("@Daen", Val(TxtAmount2.Text))
        comm.Parameters.AddWithValue("@CostGuid", TxtCostCenterGuid2.Text)
        comm.Parameters.AddWithValue("@Notes", NOTES)
        If connection.State = ConnectionState.Open Then
            connection.Close()
        End If
        connection.Open()
        comm.ExecuteNonQuery()
        comm.Parameters.Clear()
    End Sub
    Sub UPDATEBILL()
        Dim connection = dbcontext.Database.Connection
        Dim comm = New SqlCommand("UpdateBill", connection)
        comm.CommandType = CommandType.StoredProcedure
        comm.Parameters.AddWithValue("@Guid", TxtGuid.Text)
        comm.Parameters.AddWithValue("@BillDate", Convert.ToDateTime(TxtDateReg.Value))
        comm.Parameters.AddWithValue("@DocGuid", TxtPayDocGuid.Text)
        comm.Parameters.AddWithValue("@BranchGuid", TxtPayBranchGuid.Text)
        comm.Parameters.AddWithValue("@CostCenterGuid", TxtCostCenterGuid.Text)
        comm.Parameters.AddWithValue("@BillTotal", Val(TxtPayTotal.Text))
        comm.Parameters.AddWithValue("@BillTax", Val(TxtPayTax.Text))
        comm.Parameters.AddWithValue("@BillSubTotal", Val(TxtPaySubTotal.Text))
        comm.Parameters.AddWithValue("@Notes", TxtPayNotes.Text)
        comm.Parameters.AddWithValue("@Refrance", TxtRef.Text)
        comm.Parameters.AddWithValue("@CashBoxGuid", TxtCashBoxGuid.Text)
        comm.Parameters.AddWithValue("@IsUse", ChkStaut.Value)
        comm.Parameters.AddWithValue("@ChkPayDate", ChkPayDate.Checked)
        comm.Parameters.AddWithValue("@PayDate", Convert.ToDateTime(TxtPayDate.Value))
        comm.Parameters.AddWithValue("@ChkOtherBank", ChlOtherBank.Checked)
        If ChlOtherBank.Checked = True Then
            comm.Parameters.AddWithValue("@CashBoxguid2", TxtCashBoxGuid2.Text)
            comm.Parameters.AddWithValue("@CostCentGuid2", TxtCostCenterGuid2.Text)
        Else
            comm.Parameters.AddWithValue("@CashBoxguid2", NOGUID)
            comm.Parameters.AddWithValue("@CostCentGuid2", NOGUID)
        End If
        comm.Parameters.AddWithValue("@ChkPayDate2", ChkPayDate2.Checked)
        comm.Parameters.AddWithValue("@PayDate2", Convert.ToDateTime(TxtPayDate2.Value))
        comm.Parameters.AddWithValue("@Ref2", TxtRef2.Text)
        If connection.State = ConnectionState.Open Then
            connection.Close()
        End If
        connection.Open()
        comm.ExecuteNonQuery()
        comm.Parameters.Clear()
    End Sub
    Sub DELETEBILLITEM()
        Dim connection = dbcontext.Database.Connection
        Dim comm = New SqlCommand("DeleteBillItem", connection)
        comm.CommandType = CommandType.StoredProcedure
        comm.Parameters.AddWithValue("@RegDocGuid", TxtGuid.Text)
        If connection.State = ConnectionState.Open Then
            connection.Close()
        End If
        connection.Open()
        comm.ExecuteNonQuery()
        comm.Parameters.Clear()
    End Sub
    Sub UPDATEDAY1(DAY1GUID As String)
        Dim connection = dbcontext.Database.Connection
        Dim comm = New SqlCommand("UpdateDay1", connection)
        comm.CommandType = CommandType.StoredProcedure
        comm.Parameters.AddWithValue("@Guid", DAY1GUID)
        comm.Parameters.AddWithValue("@DayDate", Convert.ToDateTime(TxtDateReg.Value))
        comm.Parameters.AddWithValue("@Notes", NOTES)
        comm.Parameters.AddWithValue("@IsUse", ChkStaut.Value)
        comm.Parameters.AddWithValue("@BranchGuid", TxtPayBranchGuid.Text)
        If connection.State = ConnectionState.Open Then
            connection.Close()
        End If
        connection.Open()
        comm.ExecuteNonQuery()
        comm.Parameters.Clear()
    End Sub
    Sub DELETEDAY2(DAY1GUID As String)
        Dim connection = dbcontext.Database.Connection
        Dim comm = New SqlCommand("DeleteDay2", connection)
        comm.CommandType = CommandType.StoredProcedure
        comm.Parameters.AddWithValue("@Day1Guid", DAY1GUID)
        If connection.State = ConnectionState.Open Then
            connection.Close()
        End If
        connection.Open()
        comm.ExecuteNonQuery()
        comm.Parameters.Clear()
    End Sub
    Sub UPDATERECEPIT()
        Dim connection = dbcontext.Database.Connection
        Dim comm = New SqlCommand("UpdateCashDay", connection)
        comm.CommandType = CommandType.StoredProcedure
        comm.Parameters.AddWithValue("@Guid", TxtRecepitGuid.Text)
        comm.Parameters.AddWithValue("@CashDayDate", Convert.ToDateTime(TxtDateReg.Value))
        comm.Parameters.AddWithValue("@FAccountGuid", TxtCashBoxGuid.Text)
        If ChlOtherBank.Checked = False Then
            comm.Parameters.AddWithValue("@Amount", Val(TxtPaySubTotal.Text))
        Else
            comm.Parameters.AddWithValue("@Amount", Val(TxtAmount1.Text))
        End If
        comm.Parameters.AddWithValue("@BranchGuid", TxtPayBranchGuid.Text)
        comm.Parameters.AddWithValue("@Notes", NOTES)
        comm.Parameters.AddWithValue("@IsUse", ChkStaut.Value)
        comm.Parameters.AddWithValue("@Ref", TxtRef.Text)
        comm.Parameters.AddWithValue("@PayDate", Convert.ToDateTime(TxtPayDate.Value))
        If connection.State = ConnectionState.Open Then
            connection.Close()
        End If
        connection.Open()
        comm.ExecuteNonQuery()
        comm.Parameters.Clear()
    End Sub
    Sub DELETECASHDAYDETAIL()
        Dim connection = dbcontext.Database.Connection
        Dim comm = New SqlCommand("DeleteCashDayDetail", connection)
        comm.CommandType = CommandType.StoredProcedure
        comm.Parameters.AddWithValue("@CashDayGuid", TxtRecepitGuid.Text)
        If connection.State = ConnectionState.Open Then
            connection.Close()
        End If
        connection.Open()
        comm.ExecuteNonQuery()
        comm.Parameters.Clear()
    End Sub
    Sub UPDATERECEPIT2()
        Dim connection = dbcontext.Database.Connection
        Dim comm = New SqlCommand("UpdateCashDay", connection)
        comm.CommandType = CommandType.StoredProcedure
        comm.Parameters.AddWithValue("@Guid", TxtRecepitGuid2.Text)
        comm.Parameters.AddWithValue("@CashDayDate", Convert.ToDateTime(TxtDateReg.Value))
        comm.Parameters.AddWithValue("@FAccountGuid", TxtCashBoxGuid2.Text)
        comm.Parameters.AddWithValue("@Amount", Val(TxtAmount2.Text))
        comm.Parameters.AddWithValue("@BranchGuid", TxtPayBranchGuid.Text)
        comm.Parameters.AddWithValue("@Notes", NOTES)
        comm.Parameters.AddWithValue("@IsUse", ChkStaut.Value)
        comm.Parameters.AddWithValue("@Ref", TxtRef2.Text)
        comm.Parameters.AddWithValue("@PayDate", Convert.ToDateTime(TxtPayDate2.Value))
        If connection.State = ConnectionState.Open Then
            connection.Close()
        End If
        connection.Open()
        comm.ExecuteNonQuery()
        comm.Parameters.Clear()
    End Sub
    Sub DELETECASHDAYDETAIL2()
        Dim connection = dbcontext.Database.Connection
        Dim comm = New SqlCommand("DeleteCashDayDetail", connection)
        comm.CommandType = CommandType.StoredProcedure
        comm.Parameters.AddWithValue("@CashDayGuid", TxtRecepitGuid2.Text)
        If connection.State = ConnectionState.Open Then
            connection.Close()
        End If
        connection.Open()
        comm.ExecuteNonQuery()
        comm.Parameters.Clear()
    End Sub
    Sub FORUPDATE()
        If AllFiles.FORUPDATE("salesinvoice") = False Then
            Return
        Else
            AllFiles.ActionType___ = "تعديل"
            '''''''''''''''''''''''''''''''''''''''''' فاتورة المبيعات
            UPDATEBILL()
            DELETEBILLITEM()
            ADDBILLITEM()
            If BILLNOTES <> Nothing Then
                NOTES = BILLNOTES
            Else
                NOTES = "فاتورة مبيعات " & " " & TxtStudentName.Text & vbCrLf & " " & TxtPayDocName.Text & vbCrLf & TxtDiplomName.Text
            End If
            If TxtDay1PayGuid.Text = Nothing Then
                'Do
                '    GETDAY1_PAY_NO()
                'Loop While DAY1PAYNOEXISTS(TxtDay1PayCode.Text)
                'If DAY1PAYNOEXISTS(TxtDay1PayCode.Text) = False Then
                ADDPAYDAY1()
                SHOWPAYDAY1INFO()
                'End If
            Else
                UPDATEDAY1(TxtDay1PayGuid.Text)
                DELETEDAY2(TxtDay1PayGuid.Text)
            End If
            RegDocPosted.SHOWREFSETTINGINFO(TxtPayDocCode.Text, Guid.Parse(TxtTypeGuid.Text))
            ADDPAYDAY2_MADEN()
            ADDPAYDAY2_DAEN()
            If ChkVat.Checked = True And TxtPayTax.Text <> 0 Then
                ADDPAYTAXDAY2_DAEN()
            End If

            ''''''''''''''''''''''''''''''''''''''' سند القبض
            If TxtRecepitGuid.Text = Nothing Then
                If CASHDAYNOTES <> Nothing Then
                    NOTES = CASHDAYNOTES
                Else
                    NOTES = "سداد فاتورة مبيعات" & " " & TxtStudentName.Text & vbCrLf & " " & TxtPayDocName.Text & vbCrLf & TxtDiplomName.Text
                End If
                'Do
                '    GET_RECEPIT__NO()
                'Loop While RECEIPTNOEXISTS(TxtRecepitCode.Text)
                'If RECEIPTNOEXISTS(TxtRecepitCode.Text) = False Then
                ADDRECEPIT()
                SHOWRECEPITINFO()
                ADDRECEPITDETAILS()
                'Do
                '    GETDAY1_RECEPIT_NO()
                'Loop While DAY1PAYNOEXISTS(TxtDay1RecepitCode.Text)
                'If DAY1PAYNOEXISTS(TxtDay1RecepitCode.Text) = False Then
                ADDRECEPITDAY1()
                SHOWRECEPITDAY1INFO()
                ADDRECEPITDAY2_MADEN()
                ADDRECEPITDAY2_DAEN()
                AllFiles.ActionType___ = "جديد"
                ADD_USERACTION_(TxtRecepitGuid.Text, TxtRecepitTypeGuid.Text, TxtRecepitCode.Text)
                'End If
                'End If
            Else
                UPDATERECEPIT()
                DELETECASHDAYDETAIL()
                ADDRECEPITDETAILS()
                NOTES = CASHDAYNOTES
                UPDATEDAY1(TxtDay1RecpitGuid.Text)
                DELETEDAY2(TxtDay1RecpitGuid.Text)
                ADDRECEPITDAY2_MADEN()
                ADDRECEPITDAY2_DAEN()
                AllFiles.ActionType___ = "تعديل"
                ADD_USERACTION_(TxtRecepitGuid.Text, TxtRecepitTypeGuid.Text, TxtRecepitCode.Text)
            End If
            '''''''''''''''''''''''''''''''''''''''''' سند القبض الخاص الحوالة الثانية ان وجدت
            If ChlOtherBank.Checked = True Then
                If TxtRecepitGuid2.Text <> Nothing Then
                    UPDATERECEPIT2()
                    DELETECASHDAYDETAIL2()
                    AllFiles.ActionType___ = "تعديل"
                    ADD_USERACTION_(TxtRecepitGuid2.Text, TxtRecepitTypeGuid.Text, TxtRecepitCode2.Text)
                Else
                    'Do
                    '    GET_RECEPIT2__NO()
                    'Loop While RECEIPTNOEXISTS(TxtRecepitCode2.Text)
                    'If RECEIPTNOEXISTS(TxtRecepitCode2.Text) = False Then
                    ADDRECEPIT2()
                    SHOWRECEPITINFO2()
                    AllFiles.ActionType___ = "جديد"
                    ADD_USERACTION_(TxtRecepitGuid2.Text, TxtRecepitTypeGuid.Text, TxtRecepitCode2.Text)
                    'End If
                End If
                ADDRECEPITDETAILS2()
                If TxtDay1RecpitGuid2.Text <> Nothing Then
                    If CASHDAYNOTES <> Nothing Then
                        NOTES = CASHDAYNOTES
                    Else
                        NOTES = "سداد فاتورة مبيعات" & " " & TxtStudentName.Text & vbCrLf & " " & TxtPayDocName.Text & vbCrLf & TxtDiplomName.Text
                    End If
                    UPDATEDAY1(TxtDay1RecpitGuid2.Text)
                    DELETEDAY2(TxtDay1RecpitGuid2.Text)
                Else
                    'Do
                    '    GETDAY1_RECEPIT2_NO()
                    'Loop While DAY1PAYNOEXISTS(TxtDay1RecepitCode2.Text)
                    'If DAY1PAYNOEXISTS(TxtDay1RecepitCode2.Text) = False Then
                    NOTES = CASHDAYNOTES
                    ADDRECEPIT2DAY1()
                    SHOWRECEPIT2DAY1INFO()
                    'End If
                End If
                ADDRECEPIT2DAY2_MADEN()
                ADDRECEPIT2DAY2_DAEN()
            End If
            ADD_USERACTION_(TxtGuid.Text, TxtTypeGuid.Text, TxtCode.Text)
            MessageBox.Show("تم تعديل فاتورة المبيعات بنجاح", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Information)
        End If
    End Sub
    Function VALIDATE_DATA() As Boolean
        If TxtPayDocGuid.Text = Nothing Then
            MessageBox.Show("برجاء اختيار نوع المستند", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return False
        ElseIf TxtCashBoxGuid.Text = Nothing Then
            MessageBox.Show("برجاء اختيار الخزينة - البنك بعملية السداد", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return False
        ElseIf TxtCostCenterGuid.Text = Nothing Then
            MessageBox.Show("برجاء اختيار مركز تكلفة الخاص بعملية السداد", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return False
        ElseIf TxtRef.Text = Nothing Then
            MessageBox.Show("برجاء ادخال رقم المرجع الخاص بعملية السداد", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return False
        ElseIf TxtPayTotal.Text = 0 And TxtZeroAmount.Text = 0 Then
            MessageBox.Show("برجاء ادخال المبلغ الخاص بعملية السداد", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return False
        ElseIf ChkPayDate.Checked = False Then
            MessageBox.Show("برجاء تاريخ الحوالة البنكية / السداد", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return False
        ElseIf ChlOtherBank.Checked = True Then
            If TxtCostCenterGuid2.Text = Nothing Then
                MessageBox.Show("برجاء اختيار مركز تكلفة 2 الخاص بعملية السداد", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Error)
                Return False
            ElseIf TxtRef2.Text = Nothing Then
                MessageBox.Show("برجاء ادخال رقم المرجع 2 الخاص بعملية السداد", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Error)
                Return False
            ElseIf ChkPayDate2.Checked = False Then
                MessageBox.Show("برجاء تاريخ الحوالة 2 البنكية / السداد", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Error)
                Return False
            ElseIf TxtAmount1.Text = 0 Then
                MessageBox.Show("برجاء ادخال قيمة الحوالة الاولي", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Error)
                Return False
            ElseIf TxtAmount2.Text = 0 Then
                MessageBox.Show("برجاء ادخال قيمة الحوالة الثانية", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Error)
                Return False
            ElseIf (Val(TxtAmount1.Text) + Val(TxtAmount2.Text)) <> Val(TxtFTotal3.Text) Then
                MessageBox.Show("المبالغ الخاصة بالحولات غير متساوية مع اجمالي الفاتورة", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Error)
                Return False
            End If
        End If
        Return True
    End Function
    Function VALIDATE_AATACH_DATA() As Boolean
        If TxtCashBoxName.Text <> "صندوق المكتب" And TxtAttach1Path.Text = Nothing Then
            MessageBox.Show("لا يمكن اتمام عملية السداد بدون ارفاق صورة الحوالة الاولي", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return False
        ElseIf TxtCashBoxName2.Text <> "صندوق المكتب" And TxtAttach2Path.Text = Nothing Then
            MessageBox.Show("لا يمكن اتمام عملية السداد بدون ارفاق صورة الحوالة الثانية", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return False
        End If
        Return True
    End Function
    Sub AADATTACHFILE()
        Try
            Dim targetDirectory As String = "\\192.168.8.200\Arc\" & TxtNationalId.Text
            Dim fileExtension1 As String = Path.GetExtension(TxtAttach1Path.Text)
            Dim fileExtension2 As String = Path.GetExtension(TxtAttach2Path.Text)
            If Not Directory.Exists(targetDirectory) Then
                Try
                    Directory.CreateDirectory(targetDirectory)
                Catch ex As Exception
                    MessageBox.Show("حدث خطأ أثناء إنشاء المجلد: " + ex.Message)
                    Exit Sub
                End Try
            End If
            Dim SOURCE_FILE = TxtAttach1Path.Text
            Dim dest_file As String = targetDirectory & "\" & TxtCashBoxName.Text & "_" & TxtCode.Text & "_" & TxtPayDate.Value.ToString("ddMMyyyy") & fileExtension1
            System.IO.File.Copy(SOURCE_FILE, dest_file, True)
            If ChlOtherBank.Checked = True Then
                If TxtCashBoxName2.Text <> "صندوق المكتب" Then
                    Dim SOURCE2_FILE = TxtAttach2Path.Text
                    Dim dest2_file As String = targetDirectory & "\" & TxtCashBoxName2.Text & "_" & TxtCode.Text & "_" & TxtPayDate2.Value.ToString("ddMMyyyy") & fileExtension2
                    System.IO.File.Copy(SOURCE2_FILE, dest2_file, True)
                End If
            End If
        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try
    End Sub
    Private Async Sub CmdSave_Click(sender As Object, e As EventArgs) Handles CmdSave.Click
        Try
            If VALIDATE_DATA() = False Then
                Return
            Else
                CHKACCOUNT()
                If TxtGuid.Text = Nothing Then
                    AllFiles.WHATONLINEUSERINFO()
                    TxtUserName.Text = AllFiles.ONLINE_USER_NAME____
                    TxtUserGuid.Text = AllFiles.USER_GUID____.ToString()
                    'If VALIDATE_AATACH_DATA() = False Then
                    '    Return
                    'Else
                    FORSAVE()
                    'FORSAVE()
                    'End If
                Else
                    Dim w As New UpdateResaon
                    w.ShowDialog()
                    If w.IS_Yes = True Then
                        REASON = w.TxtNotes.Text
                        FORUPDATE()
                    End If
                End If
            End If
        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try
    End Sub

    Private Sub CmdAddItem_Click(sender As Object, e As EventArgs) Handles CmdAddItem.Click
        Try
            If TxtPayDocGuid.Text <> Nothing Then
                Dim w As New AddStudentOtherFess
                w.TxtPriceListGuid.Text = TxtPriceListGuid.Text
                w.ShowDialog()
                If w.IS_Yes = True Then
                    Dim ii As Integer = w.MainFeesGrid.CurrentRow.Index
                    For Each row In w.MainFeesGrid.SelectedRows
                        PayGrid.Rows.Add()
                        Dim i As Integer = PayGrid.Rows.Count() - 1
                        PayGrid.Rows(i).Cells(0).Value = w.MainFeesGrid.Rows(ii).Cells(0).Value.ToString()
                        PayGrid.Rows(i).Cells(1).Value = w.MainFeesGrid.Rows(ii).Cells(1).Value.ToString()
                        PayGrid.Rows(i).Cells(2).Value = "Pcs"
                        PayGrid.Rows(i).Cells(3).Value = 1
                        PayGrid.Rows(i).Cells(4).Value = w.MainFeesGrid.Rows(ii).Cells(2).Value.ToString()
                        PayGrid.Rows(i).Cells(5).Value = 0
                        PayGrid.Rows(i).Cells(6).Value = 0
                        PayGrid.Rows(i).Cells(7).Value = w.MainFeesGrid.Rows(ii).Cells(4).Value.ToString()
                        PayGrid.Rows(i).Cells(8).Value = w.MainFeesGrid.Rows(ii).Cells(5).Value.ToString()
                        PayGrid.Rows(i).Cells(9).Value = 1
                    Next
                    PAYTOTAL()
                End If
            Else
                MessageBox.Show("برجاء اختيار نوع المستند اولا", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Error)
                Return
            End If
        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try
    End Sub

    Private Sub ChkPayDate_CheckedChanged(sender As Object, e As EventArgs) Handles ChkPayDate.CheckedChanged
        Try
            If ChkPayDate.Checked = True Then
                TxtPayDate.Enabled = True
            Else
                TxtPayDate.Enabled = False
            End If
        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try
    End Sub

    Private Sub ChlOtherBank_CheckedChanged(sender As Object, e As EventArgs) Handles ChlOtherBank.CheckedChanged
        Try
            If ChlOtherBank.Checked = True Then
                CmdFinCashBox2.Enabled = True
                TxtRef2.Enabled = True
                ChkPayDate2.Enabled = True
                TxtCashBoxGuid2.Text = Nothing
                TxtCostCenterGuid2.Text = Nothing
                TxtAmount1.Enabled = True
                TxtAmount2.Enabled = True
                'CmdAttach2.Enabled = True
            Else
                CmdFinCashBox2.Enabled = False
                TxtCashBoxName2.Text = Nothing
                TxtCashBoxGuid2.Text = "00000000-0000-0000-0000-000000000000"
                TxtCostCenterName2.Text = Nothing
                TxtCostCenterGuid2.Text = "00000000-0000-0000-0000-000000000000"
                TxtRef2.Text = Nothing
                TxtRef2.Enabled = False
                ChkPayDate2.Enabled = False
                ChkPayDate2.Checked = False
                TxtAmount1.Enabled = False
                TxtAmount2.Enabled = False
                TxtAmount1.Text = 0
                TxtAmount2.Text = 0
                'CmdAttach2.Enabled = False
            End If
        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try
    End Sub

    Private Sub CmdFinCashBox2_Click(sender As Object, e As EventArgs) Handles CmdFinCashBox2.Click
        Try
            Dim w As New CashBoxList
            w.Text = "قائمة البنوك /الخزائن"
            w.TxtIsUse.Text = True
            w.ShowDialog()
            If w.IS_Yes = True Then
                Dim ii As Integer = w.BranchGrid.CurrentRow.Index
                TxtCashBoxName2.Text = w.BranchGrid.Rows(ii).Cells(1).Value.ToString()
                TxtCashBoxGuid2.Text = w.BranchGrid.Rows(ii).Cells(3).Value.ToString()
                TxtCostCenterName2.Text = w.BranchGrid.Rows(ii).Cells(4).Value.ToString()
                TxtCostCenterGuid2.Text = w.BranchGrid.Rows(ii).Cells(5).Value.ToString()
            End If
            'If TxtCashBoxGuid2.Text <> Nothing Then
            '    If TxtCashBoxName2.Text <> "صندوق المكتب" Then
            '        CmdAttach2.Enabled = True
            '    Else
            '        CmdAttach2.Enabled = False
            '        TxtAttach2Path.Text = Nothing
            '    End If
            'End If
        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try
    End Sub

    Private Sub ChkPayDate2_CheckedChanged(sender As Object, e As EventArgs) Handles ChkPayDate2.CheckedChanged
        Try
            If ChkPayDate2.Checked = True Then
                TxtPayDate2.Enabled = True
            Else
                TxtPayDate2.Enabled = False
            End If
        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try
    End Sub

    Private Sub TxtRef_TextChanged(sender As Object, e As EventArgs) Handles TxtRef.TextChanged

    End Sub

    Private Sub TxtRef_KeyDown(sender As Object, e As KeyEventArgs) Handles TxtRef.KeyDown
        If e.Control And e.KeyCode = Keys.C Then ' منع النسخ (Ctrl+C)
            e.SuppressKeyPress = True
        ElseIf e.Control And e.KeyCode = Keys.V Then ' منع اللصق (Ctrl+V)
            e.SuppressKeyPress = True
        End If
    End Sub

    Private Sub EncodingNewPayInvoice_MouseClick(sender As Object, e As MouseEventArgs) Handles Me.MouseClick
        If e.Button = Windows.Forms.MouseButtons.Right Then

            ' المزيد من المنطق هنا
        End If
    End Sub

    Private Sub TxtRef2_TextChanged(sender As Object, e As EventArgs) Handles TxtRef2.TextChanged

    End Sub

    Private Sub TxtRef2_KeyDown(sender As Object, e As KeyEventArgs) Handles TxtRef2.KeyDown
        If e.Control And e.KeyCode = Keys.C Then ' منع النسخ (Ctrl+C)
            e.SuppressKeyPress = True
        ElseIf e.Control And e.KeyCode = Keys.V Then ' منع اللصق (Ctrl+V)
            e.SuppressKeyPress = True
        End If
    End Sub

    Private Sub CmdAttach1_Click(sender As Object, e As EventArgs) Handles CmdAttach1.Click
        Try
            OpenFileDialog1.Title = "برجاء اختيار صورة الحوالة"
            'OpenFileDialog1.InitialDirectory = "c:"
            OpenFileDialog1.FileName = Nothing
            OpenFileDialog1.ShowDialog()
            TxtAttach1Path.Text = OpenFileDialog1.FileName.ToString
            Dim fileExtension As String = Path.GetExtension(TxtAttach1Path.Text)
        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try
    End Sub

    Private Sub CmdAttach2_Click(sender As Object, e As EventArgs) Handles CmdAttach2.Click
        Try
            OpenFileDialog1.Title = "برجاء اختيار صورة الحوالة"
            'OpenFileDialog1.InitialDirectory = "c:"
            OpenFileDialog1.FileName = Nothing
            OpenFileDialog1.ShowDialog()
            TxtAttach2Path.Text = OpenFileDialog1.FileName.ToString
            Dim fileExtension As String = Path.GetExtension(TxtAttach2Path.Text)
        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try
    End Sub

    Private Sub TxtCode_TextChanged(sender As Object, e As EventArgs) Handles TxtCode.TextChanged

    End Sub
End Class