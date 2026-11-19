Imports System.Data.SqlClient

Public Class EncodingTransMoney
    Dim dbcontext As New SemouEntities
    Dim REASON As String = ""
    Dim OPENGUID As Guid
    Dim NOTES As String
    Private Sub TxtCenterGuid_Click(sender As Object, e As EventArgs) Handles TxtDay1Guid.Click

    End Sub

    Private Sub CmdClose_Click(sender As Object, e As EventArgs) Handles CmdClose.Click
        Me.Close()
    End Sub

    Private Sub CmdFindBranch_Click(sender As Object, e As EventArgs) Handles CmdFindBranch.Click
        Try
            Dim w As New NewBranchList
            w.TxtType.Text = 0
            w.TxtIsUse.Text = True
            w.ShowDialog()
            If w.IS_Yes = True Then
                Dim ii As Integer = w.BranchGrid.CurrentRow.Index
                TxtBranchName.Text = w.BranchGrid.Rows(ii).Cells(1).Value.ToString()
                TxtBranchGuid.Text = w.BranchGrid.Rows(ii).Cells(3).Value.ToString()
            End If
        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try
    End Sub

    Private Sub CmdFindFromCashBox_Click(sender As Object, e As EventArgs) Handles CmdFindFromCashBox.Click
        Try
            Dim w As New CashBoxList
            w.Text = "قائمة الخزائن/البنوك"
            w.ShowDialog()
            If w.IS_Yes = True Then
                Dim ii As Integer = w.BranchGrid.CurrentRow.Index
                TxtFromCashBoxName.Text = w.BranchGrid.Rows(ii).Cells(1).Value.ToString()
                TxtFromCashBoxGuid.Text = w.BranchGrid.Rows(ii).Cells(3).Value.ToString()
            End If
        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try
    End Sub

    Private Sub CmdFindToCashBox_Click(sender As Object, e As EventArgs) Handles CmdFindToCashBox.Click
        Try
            Dim w As New CashBoxList
            w.Text = "قائمة الخزائن/البنوك"
            w.ShowDialog()
            If w.IS_Yes = True Then
                Dim ii As Integer = w.BranchGrid.CurrentRow.Index
                TxtToCashBoxName.Text = w.BranchGrid.Rows(ii).Cells(1).Value.ToString()
                TxtToCashBoxGuid.Text = w.BranchGrid.Rows(ii).Cells(3).Value.ToString()
            End If
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

    Private Sub EncodingTransMoney_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Try
            RegDocPosted.WHATFORMTYPEGUID("transmoney")
            TxtTypeGuid.Text = RegDocPosted.TYPEGUID
            RegDocPosted.WHATFORMTYPEGUID("opendaily")
            TxtOpenDailyType.Text = RegDocPosted.TYPEGUID
            OPENGUID = Guid.Parse(TxtOpenDailyType.Text)
        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try
    End Sub
    Public Function GET_TRANS__NO() As Integer
        Dim MaxId = dbcontext.MoneyTrans.OrderByDescending(Function(p) p.Id).FirstOrDefault()?.Id
        Dim MaxDay = New MoneyTran
        If (IsNothing(MaxId)) Then
            MaxId = 1
            TxtCode.Text = MaxId
        Else
            MaxId += 1
            MaxDay.Id = MaxId
            TxtCode.Text = MaxId
        End If
        Return MaxId
    End Function
    Sub ADDTRANS()
        Dim connection = dbcontext.Database.Connection
        Dim comm = New SqlCommand("AddMoneyTrans", connection)
        comm.CommandType = CommandType.StoredProcedure
        comm.Parameters.AddWithValue("@Id", GET_TRANS__NO)
        comm.Parameters.AddWithValue("@Code", TxtCode.Text)
        comm.Parameters.AddWithValue("@ActionDate", Convert.ToDateTime(TxtDayDate.Value))
        comm.Parameters.AddWithValue("@BranchGuid", TxtBranchGuid.Text)
        comm.Parameters.AddWithValue("@FromCashBoxGuid", TxtFromCashBoxGuid.Text)
        comm.Parameters.AddWithValue("@ToCashBoxGuid", TxtToCashBoxGuid.Text)
        comm.Parameters.AddWithValue("@CostCenterGuid", TxtCostCenterGuid.Text)
        comm.Parameters.AddWithValue("@Amount", Val(TxtAmount.Text))
        comm.Parameters.AddWithValue("@Ref", TxtRef.Text)
        comm.Parameters.AddWithValue("@Notes", TxtNotes.Text)
        comm.Parameters.AddWithValue("@IsUse", ChkStaut.Value)
        If connection.State = ConnectionState.Open Then
            connection.Close()
        End If
        connection.Open()
        comm.ExecuteNonQuery()
        comm.Parameters.Clear()
    End Sub
    Sub UPDATETRANS()
        Dim connection = dbcontext.Database.Connection
        Dim comm = New SqlCommand("UpdateMoneyTrans", connection)
        comm.CommandType = CommandType.StoredProcedure
        comm.Parameters.AddWithValue("@Guid", TxtGuid.Text)
        comm.Parameters.AddWithValue("@ActionDate", Convert.ToDateTime(TxtDayDate.Value))
        comm.Parameters.AddWithValue("@BranchGuid", TxtBranchGuid.Text)
        comm.Parameters.AddWithValue("@FromCashBoxGuid", TxtFromCashBoxGuid.Text)
        comm.Parameters.AddWithValue("@ToCashBoxGuid", TxtToCashBoxGuid.Text)
        comm.Parameters.AddWithValue("@CostCenterGuid", TxtCostCenterGuid.Text)
        comm.Parameters.AddWithValue("@Amount", Val(TxtAmount.Text))
        comm.Parameters.AddWithValue("@Ref", TxtRef.Text)
        comm.Parameters.AddWithValue("@Notes", TxtNotes.Text)
        comm.Parameters.AddWithValue("@IsUse", ChkStaut.Value)
        If connection.State = ConnectionState.Open Then
            connection.Close()
        End If
        connection.Open()
        comm.ExecuteNonQuery()
        comm.Parameters.Clear()
    End Sub
    Sub SHOWTRANSINFO()
        Dim connection = dbcontext.Database.Connection
        Dim comm As New SqlCommand("ShowTransMoneyInfo", connection)
        comm.Parameters.AddWithValue("@Code", TxtCode.Text)
        comm.CommandType = CommandType.StoredProcedure
        If connection.State = ConnectionState.Open Then
            connection.Close()
        End If
        connection.Open()
        comm.ExecuteNonQuery()
        Dim adp As New SqlDataAdapter(comm)
        Dim ds As DataSet = New DataSet()
        adp.Fill(ds, "ShowTransMoneyInfo")
        Dim COUNT__ As Integer = 0
        If ds.Tables(0).Rows.Count > 0 Then
            For i As Integer = 0 To ds.Tables(0).Rows.Count - 1
                TxtDayDate.Text = Convert.ToDateTime(ds.Tables(0).Rows(i)(0)).ToShortDateString()
                TxtBranchName.Text = ds.Tables(0).Rows(i)(1).ToString()
                TxtBranchGuid.Text = ds.Tables(0).Rows(i)(2).ToString()
                TxtFromCashBoxName.Text = ds.Tables(0).Rows(i)(3).ToString()
                TxtFromCashBoxGuid.Text = ds.Tables(0).Rows(i)(4).ToString()
                TxtToCashBoxName.Text = ds.Tables(0).Rows(i)(5).ToString()
                TxtToCashBoxGuid.Text = ds.Tables(0).Rows(i)(6).ToString()
                TxtCostCenterName.Text = ds.Tables(0).Rows(i)(7).ToString()
                TxtCostCenterGuid.Text = ds.Tables(0).Rows(i)(8).ToString()
                TxtAmount.Text = ds.Tables(0).Rows(i)(9).ToString()
                TxtRef.Text = ds.Tables(0).Rows(i)(10).ToString()
                TxtNotes.Text = ds.Tables(0).Rows(i)(11).ToString()
                ChkStaut.Value = ds.Tables(0).Rows(i)(12)
                TxtGuid.Text = ds.Tables(0).Rows(i)(13).ToString()
                TxtDay1Code.Text = ds.Tables(0).Rows(i)(14).ToString()
                TxtDay1Guid.Text = ds.Tables(0).Rows(i)(15).ToString()
                COUNT__ = COUNT__ + 1
            Next
            'SHOWTAFKEET()
        End If
    End Sub
    Public Function GETDAY1_TRANS_NO() As Integer
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
    Sub ADDPAYDAY1()
        Dim connection = dbcontext.Database.Connection
        Dim comm = New SqlCommand("AddDay1", connection)
        comm.CommandType = CommandType.StoredProcedure
        comm.Parameters.AddWithValue("@OpenGuid", OPENGUID)
        'comm.Parameters.AddWithValue("@Id", GETDAY1_TRANS_NO())
        'comm.Parameters.AddWithValue("@Code", TxtDay1Code.Text)
        comm.Parameters.AddWithValue("@DayDate", Convert.ToDateTime(TxtDayDate.Value))
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
                TxtDay1Code.Text = ds.Tables(0).Rows(i)(0).ToString()
                TxtDay1Guid.Text = ds.Tables(0).Rows(i)(1).ToString()
                COUNT__ = COUNT__ + 1
            Next
        End If
    End Sub
    Sub ADDRPAYDAY2_MADEN()
        Dim connection = dbcontext.Database.Connection
        Dim comm = New SqlCommand("AddDay2", connection)
        comm.CommandType = CommandType.StoredProcedure
        comm.Parameters.AddWithValue("@Day1Guid", TxtDay1Guid.Text)
        comm.Parameters.AddWithValue("@AccountGuid", TxtToCashBoxGuid.Text)
        comm.Parameters.AddWithValue("@Maden", Val(TxtAmount.Text))
        comm.Parameters.AddWithValue("@Daen", 0)
        comm.Parameters.AddWithValue("@Notes", TxtNotes.Text)
        comm.Parameters.AddWithValue("@CostGuid", TxtCostCenterGuid.Text)
        comm.Parameters.AddWithValue("@IsUse", True)
        If connection.State = ConnectionState.Open Then
            connection.Close()
        End If
        connection.Open()
        comm.ExecuteNonQuery()
        comm.Parameters.Clear()
    End Sub
    Sub ADDRPAYDAY2_DAEN()
        Dim connection = dbcontext.Database.Connection
        Dim comm = New SqlCommand("AddDay2", connection)
        comm.CommandType = CommandType.StoredProcedure
        comm.Parameters.AddWithValue("@Day1Guid", TxtDay1Guid.Text)
        comm.Parameters.AddWithValue("@AccountGuid", TxtFromCashBoxGuid.Text)
        comm.Parameters.AddWithValue("@Maden", 0)
        comm.Parameters.AddWithValue("@Daen", Val(TxtAmount.Text))
        comm.Parameters.AddWithValue("@Notes", TxtNotes.Text)
        comm.Parameters.AddWithValue("@CostGuid", TxtCostCenterGuid.Text)
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
    Sub SHOWTRANSGUID()
        Dim REGDOC_GUID_ = dbcontext.MoneyTrans.FirstOrDefault(Function(m) m.Code = TxtCode.Text)
        TxtGuid.Text = REGDOC_GUID_.Guid.ToString()
    End Sub
    Sub UPDATEDAY1()
        Dim connection = dbcontext.Database.Connection
        Dim comm = New SqlCommand("UpdateDay1", connection)
        comm.CommandType = CommandType.StoredProcedure
        comm.Parameters.AddWithValue("@Guid", TxtDay1Guid.Text)
        comm.Parameters.AddWithValue("@DayDate", Convert.ToDateTime(TxtDayDate.Value))
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
    Sub FORSAVE()
        If AllFiles.FORSAVE("transmoney") = False Then
            Return
        Else
            AllFiles.ActionType___ = "جديد"
            ADDTRANS()
            SHOWTRANSGUID()
            ADDPAYDAY1()
            SHOWPAYDAY1INFO()
            ADDRPAYDAY2_MADEN()
            ADDRPAYDAY2_DAEN()
            ADD_USERACTION_()
            SHOWTRANSINFO()
            MessageBox.Show("تم حفظ سند التحويل بنجاح", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Information)
        End If
    End Sub
    Sub FORUPDATE()
        If AllFiles.FORSAVE("transmoney") = False Then
            Return
        Else
            AllFiles.ActionType___ = "تعديل"
            UPDATETRANS()
            UPDATEDAY1()
            DELETEDAY2()
            ADDRPAYDAY2_MADEN()
            ADDRPAYDAY2_DAEN()
            ADD_USERACTION_()
            MessageBox.Show("تم تعديل سند التحويل بنجاح", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Information)
        End If
    End Sub
    Function VALIDATE_DATA() As Boolean
        If TxtBranchGuid.Text = Nothing Then
            MessageBox.Show("برجاء اختيار الفرع اولا", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return False
        ElseIf TxtFromCashBoxGuid.Text = Nothing Then
            MessageBox.Show("برجاء اختيار الخزينة - البنك المحول منها", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return False
        ElseIf TxtToCashBoxGuid.Text = Nothing Then
            MessageBox.Show("برجاء اختيار الخزينة - البنك المحول اليها", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return False
        ElseIf TxtFromCashBoxGuid.Text = TxtToCashBoxGuid.Text Then
            MessageBox.Show("لايمكن التحويل من / الى نفس الخزينة /البنك", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return False
        ElseIf TxtCostCenterGuid.Text = Nothing Then
            MessageBox.Show("برجاء اختيار مركز تكلفة الخاص بعملية السداد", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return False
        ElseIf TxtRef.Text = Nothing Then
            MessageBox.Show("برجاء ادخال رقم المرجع الخاص بعملية السداد", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return False
        ElseIf TxtAmount.Text = 0 Then
            MessageBox.Show("برجاء ادخال المبلغ الخاص بعملية السداد", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return False
        ElseIf TxtNotes.Text = Nothing Then
            MessageBox.Show("برجاء ادخال البيان الخاص بعملية التحويل", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Error)
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

    Private Sub CmdFind_Click(sender As Object, e As EventArgs) Handles CmdFind.Click
        Try
            Dim w As New TransMoneyList
            w.ShowDialog()
            If w.IS_Yes = True Then
                Dim ii As Integer = w.BranchGrid.CurrentRow.Index
                TxtCode.Text = w.BranchGrid.Rows(ii).Cells(0).Value.ToString()
            End If
            If TxtCode.Text <> Nothing Then
                SHOWTRANSINFO()
            End If
        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try
    End Sub
    Sub NEW__()
        TxtBranchName.Text = Nothing
        TxtBranchGuid.Text = Nothing
        TxtFromCashBoxName.Text = Nothing
        TxtFromCashBoxGuid.Text = Nothing
        TxtToCashBoxName.Text = Nothing
        TxtToCashBoxGuid.Text = Nothing
        TxtCostCenterName.Text = Nothing
        TxtCostCenterGuid.Text = Nothing
        TxtAmount.Text = 0
        TxtRef.Text = Nothing
        TxtNotes.Text = Nothing
        ChkStaut.Value = True
        TxtGuid.Text = Nothing
        TxtDay1Code.Text = Nothing
        TxtDay1Guid.Text = Nothing
        TxtCode.Text = Nothing
    End Sub

    Private Sub CMDNew_Click(sender As Object, e As EventArgs) Handles CMDNew.Click
        Try
            NEW__()
            EncodingTransMoney_Load(e, e)
        Catch ex As Exception
        MessageBox.Show(ex.ToString)
        End Try
    End Sub
End Class