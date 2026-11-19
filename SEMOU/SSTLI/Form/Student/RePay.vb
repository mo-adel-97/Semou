Imports System.Data.SqlClient

Public Class RePay
    Dim dbcontext As New SemouEntities
    Dim OPENGUID As Guid
    Dim REASON As String = ""
    Private Sub CmdClose_Click(sender As Object, e As EventArgs) Handles CmdClose.Click
        Me.Close()
    End Sub
    Sub SHOWBILLDIPLOM(DOCGUID As Guid)
        PayGrid.Rows.Clear()
        Dim connection = dbcontext.Database.Connection
        Dim comm As New SqlCommand("ShowBillDetail", connection)
        comm.Parameters.AddWithValue("@BillGuid", DOCGUID)
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

    Private Sub RePay_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Try
            RegDocPosted.WHATFORMTYPEGUID("repay")
            TxtTypeGuid.Text = RegDocPosted.TYPEGUID
            RegDocPosted.WHATFORMTYPEGUID("opendaily")
            TxtOpenDailyType.Text = RegDocPosted.TYPEGUID
            OPENGUID = Guid.Parse(TxtOpenDailyType.Text)
        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try
    End Sub
    Sub SHOWOLDBILLINFO()
        Dim connection = dbcontext.Database.Connection
        Dim comm As New SqlCommand("ShowBillInfo", connection)
        comm.Parameters.AddWithValue("@Code", TxtRegDocCode.Text)
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
                TxtRegDocBranchGuid.Text = ds.Tables(0).Rows(i)(4).ToString()
                TxtSalesManGuid.Text = ds.Tables(0).Rows(i)(8).ToString()
                TxtSalesManName.Text = ds.Tables(0).Rows(i)(9).ToString()
                TxtCostCenterGuid.Text = ds.Tables(0).Rows(i)(6).ToString()
                COUNT__ = COUNT__ + 1
            Next
        End If
    End Sub
    Sub CHKACCOUNT()
        RegDocPosted.SHOWREFSETTINGINFO(TxtDocCode.Text, Guid.Parse(TxtTypeGuid.Text))
        TxtBranchName.Text = RegDocPosted.BRANCHNAME.ToString()
        TxtBranchGuid.Text = RegDocPosted.BRANCHGUID.ToString()
        TxtPriceListGuid.Text = RegDocPosted.PRICELISTGUID.ToString()
        TxtVatGuid.Text = RegDocPosted.VATACCOUNTGUID.ToString()
        ChkVat.Checked = RegDocPosted.CHKVAT
    End Sub
    Private Sub CmdFindDoc_Click(sender As Object, e As EventArgs) Handles CmdFindDoc.Click
        Try
            Dim w As New DocRegSettingList
            w.TxtTypeGuid.Text = TxtTypeGuid.Text
            w.TxtBranchGuid.Text = TxtRegDocBranchGuid.Text
            w.ShowDialog()
            If w.IS_Yes = True Then
                Dim ii As Integer = w.BranchGrid.CurrentRow.Index
                TxtDocCode.Text = w.BranchGrid.Rows(ii).Cells(0).Value.ToString()
                TxtDocName.Text = w.BranchGrid.Rows(ii).Cells(1).Value.ToString()
                TxtDocGuid.Text = w.BranchGrid.Rows(ii).Cells(2).Value.ToString()
            End If
            If TxtDocGuid.Text <> Nothing Then
                CHKACCOUNT()
            End If
        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try
    End Sub
    Function VALIDATE_DATA() As Boolean
        If TxtDocGuid.Text = Nothing Then
            MessageBox.Show("برجاء اختيار نوع المستند", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return False
        ElseIf TxtSalesManGuid.Text = Nothing Then
            MessageBox.Show("برجاء اختيار مندوب البيع", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return False
        ElseIf TxtNotes.Text = Nothing Then
            MessageBox.Show("لا يمكن حفظ مرتجع المبيعات بدون ملاحظات", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return False
        End If
        Return True
    End Function
    Public Function GET___NO() As Integer
        Dim MaxId = dbcontext.ReBills.OrderByDescending(Function(p) p.ID).FirstOrDefault()?.ID
        Dim MaxDay = New ReRegDoc
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
    Sub ADDREPAY()
        Dim connection = dbcontext.Database.Connection
        Dim comm = New SqlCommand("AddReBill", connection)
        comm.CommandType = CommandType.StoredProcedure
        comm.Parameters.AddWithValue("@Id", GET___NO)
        comm.Parameters.AddWithValue("@Code", TxtCode.Text)
        comm.Parameters.AddWithValue("@ReDate", Convert.ToDateTime(TxtDateReg.Value))
        comm.Parameters.AddWithValue("@DocGuid", TxtDocGuid.Text)
        comm.Parameters.AddWithValue("@BranchGuid", TxtBranchGuid.Text)
        comm.Parameters.AddWithValue("@UserGuid", TxtSalesManGuid.Text)
        comm.Parameters.AddWithValue("@AccountGuid", TxtAccountGuid.Text)
        comm.Parameters.AddWithValue("@ReTotal", Val(TxtFTotal1.Text))
        comm.Parameters.AddWithValue("@ReTax", Val(TxtFTotal2.Text))
        comm.Parameters.AddWithValue("@ReSubTotal", Val(TxtFTotal3.Text))
        comm.Parameters.AddWithValue("@ReNotes", TxtNotes.Text)
        comm.Parameters.AddWithValue("@IsUse", ChkStaut.Value)
        comm.Parameters.AddWithValue("@BillGuid", TxtRegDocGuid.Text)
        If connection.State = ConnectionState.Open Then
            connection.Close()
        End If
        connection.Open()
        comm.ExecuteNonQuery()
        comm.Parameters.Clear()
    End Sub
    Sub SHOWREPAYGUID()
        Dim REGDOC_GUID_ = dbcontext.ReBills.FirstOrDefault(Function(m) m.Code = TxtCode.Text)
        TxtGuid.Text = REGDOC_GUID_.Guid.ToString()
    End Sub
    Sub ADDREBILLITEM()
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
    Public Function GETDAY1__NO() As Integer
        Dim MaxId = dbcontext.Day1.Where(Function(m) m.TypeGuid <> OPENGUID).OrderByDescending(Function(p) p.ID).FirstOrDefault()?.ID
        Dim MaxDay = New Day1
        If (IsNothing(MaxId)) Then
            MaxId = 1
            TxtDay1Code.Text = MaxId
        Else
            MaxId += 1
            MaxDay.ID = MaxId
            TxtDay1Code.Text = MaxId
        End If
        Return MaxId
    End Function
    Sub ADDDAY1()
        Dim connection = dbcontext.Database.Connection
        Dim comm = New SqlCommand("AddDay1", connection)
        comm.CommandType = CommandType.StoredProcedure
        comm.Parameters.AddWithValue("@OpenGuid", OPENGUID)
        'comm.Parameters.AddWithValue("@Id", GETDAY1__NO)
        'comm.Parameters.AddWithValue("@Code", TxtDay1Code.Text)
        comm.Parameters.AddWithValue("@DayDate", Convert.ToDateTime(TxtDateReg.Value))
        comm.Parameters.AddWithValue("@Notes", TxtNotes.Text)
        comm.Parameters.AddWithValue("@DayType", True)
        comm.Parameters.AddWithValue("@DocGuid", TxtGuid.Text)
        comm.Parameters.AddWithValue("@IsUse", True)
        comm.Parameters.AddWithValue("@TypeGuid", TxtTypeGuid.Text)
        comm.Parameters.AddWithValue("@ActionCode", TxtCode.Text)
        comm.Parameters.AddWithValue("@BranchGuid", TxtBranchGuid.Text)
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
        TxtDay1Code.Text = RETURNCODE.ToString()
        comm.Parameters.Clear()
    End Sub
    Sub SHOWDAY1INFO()
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
                TxtDay1Code.Text = ds.Tables(0).Rows(i)(0).ToString()
                TxtDay1Guid.Text = ds.Tables(0).Rows(i)(1).ToString()
                COUNT__ = COUNT__ + 1
            Next
        End If
    End Sub
    Sub ADDPAYDAY2_MADEN()
        Dim connection = dbcontext.Database.Connection
        Dim comm = New SqlCommand("AddDay2", connection)
        comm.CommandType = CommandType.StoredProcedure
        comm.Parameters.AddWithValue("@Day1Guid", TxtDay1Guid.Text)
        If RegDocPosted.CHKMADEN = True Then
            comm.Parameters.AddWithValue("@AccountGuid", TxtAccountGuid.Text)
        Else
            comm.Parameters.AddWithValue("@AccountGuid", RegDocPosted.MADENGUID)
        End If
        comm.Parameters.AddWithValue("@Maden", Val(TxtFTotal1.Text))
        comm.Parameters.AddWithValue("@Daen", 0)
        comm.Parameters.AddWithValue("@Notes", TxtNotes.Text)
        comm.Parameters.AddWithValue("@CostGuid", "00000000-0000-0000-0000-000000000000")
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
        comm.Parameters.AddWithValue("@Day1Guid", TxtDay1Guid.Text)
        If RegDocPosted.CHKDAEN = True Then
            comm.Parameters.AddWithValue("@AccountGuid", TxtAccountGuid.Text)
        Else
            comm.Parameters.AddWithValue("@AccountGuid", RegDocPosted.DAENGUID)
        End If
        comm.Parameters.AddWithValue("@Maden", 0)
        comm.Parameters.AddWithValue("@Daen", Val(TxtFTotal3.Text))
        comm.Parameters.AddWithValue("@Notes", TxtNotes.Text)
        comm.Parameters.AddWithValue("@CostGuid", "00000000-0000-0000-0000-000000000000")
        comm.Parameters.AddWithValue("@IsUse", True)
        If connection.State = ConnectionState.Open Then
            connection.Close()
        End If
        connection.Open()
        comm.ExecuteNonQuery()
        comm.Parameters.Clear()
    End Sub
    Sub ADDPAYTAXDAY2_MADEN()
        Dim connection = dbcontext.Database.Connection
        Dim comm = New SqlCommand("AddDay2", connection)
        comm.CommandType = CommandType.StoredProcedure
        comm.Parameters.AddWithValue("@Day1Guid", TxtDay1Guid.Text)
        comm.Parameters.AddWithValue("@AccountGuid", TxtVatGuid.Text)
        comm.Parameters.AddWithValue("@Maden", Val(TxtFTotal2.Text))
        comm.Parameters.AddWithValue("@Daen", 0)
        comm.Parameters.AddWithValue("@Notes", TxtNotes.Text)
        comm.Parameters.AddWithValue("@CostGuid", "00000000-0000-0000-0000-000000000000")
        comm.Parameters.AddWithValue("@IsUse", True)
        If connection.State = ConnectionState.Open Then
            connection.Close()
        End If
        connection.Open()
        comm.ExecuteNonQuery()
        comm.Parameters.Clear()
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
    Sub FORSAVE()
        If AllFiles.FORSAVE("repay") = False Then
            Return
        Else
            AllFiles.ActionType___ = "جديد"
            ADDREPAY()
            SHOWREPAYGUID()
            ADDREBILLITEM()
            ADDDAY1()
            SHOWDAY1INFO()
            ADDPAYDAY2_MADEN()
            If ChkVat.Checked = True Then
                If TxtFTotal2.Text <> 0 Then
                    ADDPAYTAXDAY2_MADEN()
                End If
            End If
            ADDPAYDAY2_DAEN()
            UPDATEOLDBILL()
            SHOWREPAYINFO()
            ADD_USERACTION_()
            MessageBox.Show("تم حفظ مرتجع المبيعات بنجاح", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Information)
        End If
    End Sub
    Sub SHOWREPAYINFO()
        Dim connection = dbcontext.Database.Connection
        Dim comm As New SqlCommand("ShowRePayInfo", connection)
        comm.Parameters.AddWithValue("@Code", TxtCode.Text)
        comm.CommandType = CommandType.StoredProcedure
        If connection.State = ConnectionState.Open Then
            connection.Close()
        End If
        connection.Open()
        comm.ExecuteNonQuery()
        Dim adp As New SqlDataAdapter(comm)
        Dim ds As DataSet = New DataSet()
        adp.Fill(ds, "ShowRePayInfo")
        Dim COUNT__ As Integer = 0
        If ds.Tables(0).Rows.Count > 0 Then
            For i As Integer = 0 To ds.Tables(0).Rows.Count - 1
                TxtDateReg.Value = Convert.ToDateTime(ds.Tables(0).Rows(i)(0))
                TxtDocName.Text = ds.Tables(0).Rows(i)(1).ToString()
                TxtDocCode.Text = ds.Tables(0).Rows(i)(2).ToString()
                TxtDocGuid.Text = ds.Tables(0).Rows(i)(3).ToString()
                TxtStudentName.Text = ds.Tables(0).Rows(i)(4).ToString()
                TxtTel.Text = ds.Tables(0).Rows(i)(5).ToString()
                TxtNationalId.Text = ds.Tables(0).Rows(i)(6).ToString()
                TxtAccountGuid.Text = ds.Tables(0).Rows(i)(7).ToString()
                TxtBranchName.Text = ds.Tables(0).Rows(i)(8).ToString()
                TxtBranchGuid.Text = ds.Tables(0).Rows(i)(9).ToString()
                TxtSalesManName.Text = ds.Tables(0).Rows(i)(10).ToString()
                TxtSalesManGuid.Text = ds.Tables(0).Rows(i)(11).ToString()
                TxtFTotal1.Text = ds.Tables(0).Rows(i)(12).ToString()
                TxtFTotal2.Text = ds.Tables(0).Rows(i)(13).ToString()
                TxtFTotal3.Text = ds.Tables(0).Rows(i)(14).ToString()
                TxtNotes.Text = ds.Tables(0).Rows(i)(15).ToString()
                ChkStaut.Value = ds.Tables(0).Rows(i)(16).ToString()
                TxtRegDocGuid.Text = ds.Tables(0).Rows(i)(17).ToString()
                TxtGuid.Text = ds.Tables(0).Rows(i)(18).ToString()
                TxtDay1Code.Text = ds.Tables(0).Rows(i)(19).ToString()
                TxtDay1Guid.Text = ds.Tables(0).Rows(i)(20).ToString()
                COUNT__ = COUNT__ + 1
            Next
        End If
    End Sub
    Sub UPDATEREPAY()
        Dim connection = dbcontext.Database.Connection
        Dim comm = New SqlCommand("UpdateRePay", connection)
        comm.CommandType = CommandType.StoredProcedure
        comm.Parameters.AddWithValue("@Guid", TxtGuid.Text)
        comm.Parameters.AddWithValue("@ReDate", Convert.ToDateTime(TxtDateReg.Value))
        comm.Parameters.AddWithValue("@DocGuid", TxtDocGuid.Text)
        comm.Parameters.AddWithValue("@BranchGuid", TxtBranchGuid.Text)
        comm.Parameters.AddWithValue("@ReTotal", Val(TxtFTotal1.Text))
        comm.Parameters.AddWithValue("@ReTax", Val(TxtFTotal2.Text))
        comm.Parameters.AddWithValue("@ReSubTotal", Val(TxtFTotal3.Text))
        comm.Parameters.AddWithValue("@ReNotes", TxtNotes.Text)
        comm.Parameters.AddWithValue("@IsUse", ChkStaut.Value)
        If connection.State = ConnectionState.Open Then
            connection.Close()
        End If
        connection.Open()
        comm.ExecuteNonQuery()
        comm.Parameters.Clear()
    End Sub
    Sub DELETEREPAYDIPLOM()
        Dim connection = dbcontext.Database.Connection
        Dim comm = New SqlCommand("DeleteReRegDocDiplom", connection)
        comm.CommandType = CommandType.StoredProcedure
        comm.Parameters.AddWithValue("@RegDocGuid", TxtGuid.Text)
        If connection.State = ConnectionState.Open Then
            connection.Close()
        End If
        connection.Open()
        comm.ExecuteNonQuery()
        comm.Parameters.Clear()
    End Sub
    Sub UPDATEDAY1()
        Dim connection = dbcontext.Database.Connection
        Dim comm = New SqlCommand("UpdateDay1", connection)
        comm.CommandType = CommandType.StoredProcedure
        comm.Parameters.AddWithValue("@Guid", TxtDay1Guid.Text)
        comm.Parameters.AddWithValue("@DayDate", Convert.ToDateTime(TxtDateReg.Value))
        comm.Parameters.AddWithValue("@Notes", TxtNotes.Text)
        comm.Parameters.AddWithValue("@IsUse", ChkStaut.Value)
        comm.Parameters.AddWithValue("@BranchGuid", TxtBranchGuid.Text)
        If connection.State = ConnectionState.Open Then
            connection.Close()
        End If
        connection.Open()
        comm.ExecuteNonQuery()
        comm.Parameters.Clear()
    End Sub
    Sub DELETEDAY2()
        Dim connection = dbcontext.Database.Connection
        Dim comm = New SqlCommand("DeleteDay2", connection)
        comm.CommandType = CommandType.StoredProcedure
        comm.Parameters.AddWithValue("@Day1Guid", TxtDay1Guid.Text)
        If connection.State = ConnectionState.Open Then
            connection.Close()
        End If
        connection.Open()
        comm.ExecuteNonQuery()
        comm.Parameters.Clear()
    End Sub
    Sub FORUPDATE()
        If AllFiles.FORUPDATE("repay") = False Then
            Return
        Else
            AllFiles.ActionType___ = "تعديل"
            UPDATEREPAY()
            DELETEREPAYDIPLOM()
            ADDREBILLITEM()
            UPDATEDAY1()
            DELETEDAY2()
            ADDPAYDAY2_MADEN()
            If ChkVat.Checked = True Then
                If TxtFTotal2.Text <> 0 Then
                    ADDPAYTAXDAY2_MADEN()
                End If
            End If
            ADDPAYDAY2_DAEN()
            UPDATEOLDBILL()
            ADD_USERACTION_()
            MessageBox.Show("تم تعديل مرتجع المبيعات بنجاح", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Information)
        End If
    End Sub
    Sub UPDATEOLDBILL()
        Dim connection = dbcontext.Database.Connection
        Dim comm = New SqlCommand("RePayBill", connection)
        comm.CommandType = CommandType.StoredProcedure
        comm.Parameters.AddWithValue("@BillGuid", TxtRegDocGuid.Text)
        comm.Parameters.AddWithValue("@RePay", ChkStaut.Value)
        If connection.State = ConnectionState.Open Then
            connection.Close()
        End If
        connection.Open()
        comm.ExecuteNonQuery()
        comm.Parameters.Clear()
    End Sub
    Private Sub CmdSave_Click(sender As Object, e As EventArgs) Handles CmdSave.Click
        Try
            If VALIDATE_DATA() = False Then
                Return
            Else
                CHKACCOUNT()
                If TxtGuid.Text = Nothing Then
                    FORSAVE()
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

    Private Sub PayGrid_CellContentClick(sender As Object, e As DataGridViewCellEventArgs) Handles PayGrid.CellContentClick

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
End Class