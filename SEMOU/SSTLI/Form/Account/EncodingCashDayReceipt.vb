Imports System.Data.SqlClient
Imports DevExpress.XtraReports.UI

Public Class EncodingCashDayReceipt
    Dim dbcontext As New SemouEntities
    Dim REASON As String = ""
    Dim OPENGUID As Guid
    Private Sub EncodingCashDayReceipt_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Try
            RegDocPosted.WHATFORMTYPEGUID("receipt")
            TxtTypeGuid.Text = RegDocPosted.TYPEGUID
            RegDocPosted.WHATFORMTYPEGUID("opendaily")
            TxtOpenDailyType.Text = RegDocPosted.TYPEGUID
            OPENGUID = Guid.Parse(TxtOpenDailyType.Text)
            Dim row As DataGridViewRow = AccountGrid.Rows(0)
            row.Height = 27
        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try
    End Sub

    Private Sub CmdFindAccount_Click(sender As Object, e As EventArgs) Handles CmdFindAccount.Click
        Try
            Dim w As New CashBoxList
            w.Text = "قائمة الخزائن/البنوك"
            'w.TxtType.Text = 1
            w.ShowDialog()
            If w.IS_Yes = True Then
                Dim ii As Integer = w.BranchGrid.CurrentRow.Index
                TxtAccountName.Text = w.BranchGrid.Rows(ii).Cells(1).Value.ToString()
                TxtAccountGuid.Text = w.BranchGrid.Rows(ii).Cells(3).Value.ToString()
                TxtCenterName.Text = w.BranchGrid.Rows(ii).Cells(4).Value.ToString()
                TxtCenterGuid.Text = w.BranchGrid.Rows(ii).Cells(5).Value.ToString()
            End If
        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try
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

    Private Sub AccountGrid_CellContentClick(sender As Object, e As DataGridViewCellEventArgs) Handles AccountGrid.CellContentClick

    End Sub
    Sub DAILYTOTAL()
        Dim DIPLOMTAX As Double = 0
        For i As Integer = 0 To AccountGrid.Rows.Count - 1
            If AccountGrid.Rows(i).Cells(6).Value <> Nothing Then
                DIPLOMTAX = Val(DIPLOMTAX) + (Val(AccountGrid.Rows(i).Cells(4).Value))
            End If
        Next
        TxtTotalDaen.Text = Math.Round(Val(DIPLOMTAX), 2)
        TxtDef.Text = Math.Round(Val(TxtAmount.Text) - Val(TxtTotalDaen.Text), 2)
    End Sub
    Private Sub AccountGrid_CellDoubleClick(sender As Object, e As DataGridViewCellEventArgs) Handles AccountGrid.CellDoubleClick
        Try
            Dim i As Integer = AccountGrid.CurrentCell.RowIndex
            Dim j As Integer = AccountGrid.CurrentCell.ColumnIndex
            If j = 0 Then
                Dim w As New AccountList
                w.TxtAccountType2.Text = 2
                w.TxtIsUse.Text = True
                w.FindByCode.Checked = True
                w.FindByName.Enabled = False
                w.ShowDialog()
                If w.IS_Yes = True Then
                    Dim ii As Integer = w.BranchGrid.CurrentRow.Index
                    AccountGrid.Rows(i).Cells(0).Value = w.BranchGrid.Rows(ii).Cells(0).Value.ToString()
                    AccountGrid.Rows(i).Cells(1).Value = w.BranchGrid.Rows(ii).Cells(1).Value.ToString()
                    AccountGrid.Rows(i).Cells(6).Value = w.BranchGrid.Rows(ii).Cells(3).Value.ToString()
                    If TxtCenterName.Text <> Nothing Then
                        AccountGrid.Rows(i).Cells(5).Value = TxtCenterName.Text
                        AccountGrid.Rows(i).Cells(7).Value = TxtCenterGuid.Text
                    End If
                End If
            ElseIf j = 1 Then
                Dim w As New AccountList
                w.TxtAccountType2.Text = 2
                w.TxtIsUse.Text = True
                w.FindByName.Checked = True
                w.FindByCode.Enabled = False
                w.ShowDialog()
                If w.IS_Yes = True Then
                    Dim ii As Integer = w.BranchGrid.CurrentRow.Index
                    AccountGrid.Rows(i).Cells(0).Value = w.BranchGrid.Rows(ii).Cells(0).Value.ToString()
                    AccountGrid.Rows(i).Cells(1).Value = w.BranchGrid.Rows(ii).Cells(1).Value.ToString()
                    AccountGrid.Rows(i).Cells(6).Value = w.BranchGrid.Rows(ii).Cells(3).Value.ToString()
                    If TxtCenterName.Text <> Nothing Then
                        AccountGrid.Rows(i).Cells(5).Value = TxtCenterName.Text
                        AccountGrid.Rows(i).Cells(7).Value = TxtCenterGuid.Text
                    End If
                End If
            ElseIf j = 5 Then
                Dim w As New CostCenterList
                w.TxtAccountType.Text = 1
                w.ShowDialog()
                If w.IS_Yes = True Then
                    Dim ii As Integer = w.BranchGrid.CurrentRow.Index
                    AccountGrid.Rows(i).Cells(5).Value = w.BranchGrid.Rows(ii).Cells(1).Value.ToString()
                    AccountGrid.Rows(i).Cells(7).Value = w.BranchGrid.Rows(ii).Cells(3).Value.ToString()
                End If
            End If
        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try
    End Sub
    Public Function GET_RECEPIT__NO() As Integer
        Dim MaxId = dbcontext.CashDays.Where(Function(m) m.CashDayType = 0).OrderByDescending(Function(p) p.Id).FirstOrDefault()?.Id
        Dim MaxDay = New CashDay
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
    Sub ADDRECEPIT()
        Dim connection = dbcontext.Database.Connection
        Dim comm = New SqlCommand("AddCashDay", connection)
        comm.CommandType = CommandType.StoredProcedure
        'comm.Parameters.AddWithValue("@Id", GET_RECEPIT__NO)
        'comm.Parameters.AddWithValue("@Code", TxtCode.Text)
        comm.Parameters.AddWithValue("@CashDayDate", Convert.ToDateTime(TxtDayDate.Value))
        comm.Parameters.AddWithValue("@FAccountGuid", TxtAccountGuid.Text)
        comm.Parameters.AddWithValue("@Amount", Val(TxtAmount.Text))
        comm.Parameters.AddWithValue("@BranchGuid", TxtBranchGuid.Text)
        comm.Parameters.AddWithValue("@Notes", TxtNotes.Text)
        comm.Parameters.AddWithValue("@LinkGuid", "00000000-0000-0000-0000-000000000000")
        comm.Parameters.AddWithValue("@LinkRef", "")
        comm.Parameters.AddWithValue("@CashDayType", 0)
        comm.Parameters.AddWithValue("@IsUse", ChkStaut.Value)
        comm.Parameters.AddWithValue("@Ref", "")
        comm.Parameters.AddWithValue("@PayDate", Convert.ToDateTime(TxtDayDate.Value))
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
    Sub SHOWRECEPITINFO()
        Dim connection = dbcontext.Database.Connection
        Dim comm As New SqlCommand("ShowCashdayInfo", connection)
        comm.Parameters.AddWithValue("@Code", TxtCode.Text)
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
                TxtDayDate.Text = Convert.ToDateTime(ds.Tables(0).Rows(i)(0)).ToShortDateString()
                TxtAccountName.Text = ds.Tables(0).Rows(i)(1).ToString()
                TxtAccountGuid.Text = ds.Tables(0).Rows(i)(2).ToString()
                TxtBranchName.Text = ds.Tables(0).Rows(i)(3).ToString()
                TxtBranchGuid.Text = ds.Tables(0).Rows(i)(4).ToString()
                TxtAmount.Text = ds.Tables(0).Rows(i)(5).ToString()
                TxtNotes.Text = ds.Tables(0).Rows(i)(6).ToString()
                TxtLinkName.Text = ds.Tables(0).Rows(i)(7).ToString()
                TxtLinkCode.Text = ds.Tables(0).Rows(i)(8).ToString()
                ChkStaut.Value = ds.Tables(0).Rows(i)(9)
                TxtGuid.Text = ds.Tables(0).Rows(i)(10).ToString()
                TxtCenterName.Text = ds.Tables(0).Rows(i)(14).ToString()
                TxtCenterGuid.Text = ds.Tables(0).Rows(i)(15).ToString()
                COUNT__ = COUNT__ + 1
            Next
            SHOWTAFKEET()
        End If
    End Sub
    Sub ADDDRECEPITDETAIL()
        Dim connection = dbcontext.Database.Connection
        Dim comm = New SqlCommand("AddCashDayDetail", connection)
        comm.CommandType = CommandType.StoredProcedure
        For i As Integer = 0 To AccountGrid.Rows.Count - 1
            If AccountGrid.Rows(i).Cells(6).Value = Nothing Then
                Exit For
            Else
                comm.CommandType = CommandType.StoredProcedure
                comm.Parameters.AddWithValue("@CashDayGuid", TxtGuid.Text)
                comm.Parameters.AddWithValue("@AccountGuid", AccountGrid.Rows(i).Cells(6).Value.ToString())
                comm.Parameters.AddWithValue("@Daen", Val(AccountGrid.Rows(i).Cells(4).Value))
                comm.Parameters.AddWithValue("@CostGuid", AccountGrid.Rows(i).Cells(7).Value.ToString())
                comm.Parameters.AddWithValue("@Notes", AccountGrid.Rows(i).Cells(2).Value.ToString())
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
    Public Function GETDAY1_RECEPIT_NO() As Integer
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
    Sub ADDRECEPITDAY1()
        Dim connection = dbcontext.Database.Connection
        Dim comm = New SqlCommand("AddDay1", connection)
        comm.CommandType = CommandType.StoredProcedure
        comm.Parameters.AddWithValue("@OpenGuid", OPENGUID)
        'comm.Parameters.AddWithValue("@Id", GETDAY1_RECEPIT_NO())
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
    Sub SHOWRECEPITDAY1INFO()
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
    Sub ADDRECEPITDAY2_MADEN()
        Dim connection = dbcontext.Database.Connection
        Dim comm = New SqlCommand("AddDay2", connection)
        comm.CommandType = CommandType.StoredProcedure
        comm.Parameters.AddWithValue("@Day1Guid", TxtDay1Guid.Text)
        comm.Parameters.AddWithValue("@AccountGuid", TxtAccountGuid.Text)
        comm.Parameters.AddWithValue("@Maden", Val(TxtAmount.Text))
        comm.Parameters.AddWithValue("@Daen", 0)
        comm.Parameters.AddWithValue("@Notes", TxtNotes.Text)
        comm.Parameters.AddWithValue("@CostGuid", TxtCenterGuid.Text)
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
        For i As Integer = 0 To AccountGrid.Rows.Count - 1
            If AccountGrid.Rows(i).Cells(6).Value = Nothing Then
                Exit For
            Else
                comm.Parameters.AddWithValue("@Day1Guid", TxtDay1Guid.Text)
                comm.Parameters.AddWithValue("@AccountGuid", AccountGrid.Rows(i).Cells(6).Value.ToString())
                comm.Parameters.AddWithValue("@Maden", Val(AccountGrid.Rows(i).Cells(3).Value))
                comm.Parameters.AddWithValue("@Daen", Val(AccountGrid.Rows(i).Cells(4).Value))
                comm.Parameters.AddWithValue("@Notes", AccountGrid.Rows(i).Cells(2).Value.ToString())
                comm.Parameters.AddWithValue("@CostGuid", AccountGrid.Rows(i).Cells(7).Value.ToString())
                comm.Parameters.AddWithValue("@IsUse", True)
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
    Sub UPDATERECEPIT()
        Dim connection = dbcontext.Database.Connection
        Dim comm = New SqlCommand("UpdateCashDay", connection)
        comm.CommandType = CommandType.StoredProcedure
        comm.Parameters.AddWithValue("@Guid", TxtGuid.Text)
        comm.Parameters.AddWithValue("@CashDayDate", Convert.ToDateTime(TxtDayDate.Value))
        comm.Parameters.AddWithValue("@FAccountGuid", TxtAccountGuid.Text)
        comm.Parameters.AddWithValue("@Amount", Val(TxtAmount.Text))
        comm.Parameters.AddWithValue("@BranchGuid", TxtBranchGuid.Text)
        comm.Parameters.AddWithValue("@Notes", TxtNotes.Text)
        comm.Parameters.AddWithValue("@IsUse", ChkStaut.Value)
        comm.Parameters.AddWithValue("@Ref", "")
        comm.Parameters.AddWithValue("@PayDate", Convert.ToDateTime(TxtDayDate.Value))
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
    Sub DELETECASHDAYDETAIL()
        Dim connection = dbcontext.Database.Connection
        Dim comm = New SqlCommand("DeleteCashDayDetail", connection)
        comm.CommandType = CommandType.StoredProcedure
        comm.Parameters.AddWithValue("@CashDayGuid", TxtGuid.Text)
        If connection.State = ConnectionState.Open Then
            connection.Close()
        End If
        connection.Open()
        comm.ExecuteNonQuery()
        comm.Parameters.Clear()
    End Sub
    Sub FORSAVE()
        If AllFiles.FORSAVE("receipt") = False Then
            Return
        Else
            AllFiles.ActionType___ = "جديد"
            ADDRECEPIT()
            SHOWRECEPITINFO()
            ADDDRECEPITDETAIL()
            ADDRECEPITDAY1()
            SHOWRECEPITDAY1INFO()
            ADDRECEPITDAY2_MADEN()
            ADDRECEPITDAY2_DAEN()
            ADD_USERACTION_()
            MessageBox.Show("تم حفظ سند القبض بنجاح", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Information)
        End If
    End Sub
    Sub FORUPDATE()
        If AllFiles.FORUPDATE("receipt") = False Then
            Return
        Else
            AllFiles.ActionType___ = "تعديل"
            UPDATERECEPIT()
            DELETECASHDAYDETAIL()
            ADDDRECEPITDETAIL()
            UPDATEDAY1()
            DELETEDAY2()
            ADDRECEPITDAY2_MADEN()
            ADDRECEPITDAY2_DAEN()
            ADD_USERACTION_()
            MessageBox.Show("تم تعديل سند القبض بنجاح", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Information)
        End If
    End Sub
    Function VALIDATE_DATA() As Boolean
        If TxtBranchGuid.Text = Nothing Then
            MessageBox.Show("برجاء اختيار الفرع اولا", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return False
        ElseIf TxtAccountGuid.Text = Nothing Then
            MessageBox.Show("برجاء اختيار الحساب المدين اولا", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return False
        ElseIf TxtNotes.Text = Nothing Then
            MessageBox.Show("برجاء ادخال ملاحظات سند القبض لتسهيل عملية مراجعه الحسابات", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return False
        ElseIf TxtAmount.Text = 0 Then
            MessageBox.Show("برجاء ادخال قيمة سند القبض", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return False
        ElseIf TxtTotalDaen.Text = 0 Then
            MessageBox.Show("برجاء التأكد من المبالغ المدرجة فى الجانب الدائن", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return False
        ElseIf TxtDef.Text <> 0 Then
            MessageBox.Show("السند غير متزن برجاء مراجعه تفاصيل السند", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return False
        End If
        Return True
    End Function
    Public Function NOCOSTCENTER_() As Boolean
        For i As Integer = 0 To AccountGrid.Rows.Count - 1
            If AccountGrid.Rows(i).Cells(7).Value = Nothing And AccountGrid.Rows(i).Cells(6).Value <> Nothing Then
                Return False
            End If
        Next
        Return True
    End Function
    Public Function NODAY2NOTES_() As Boolean
        For i As Integer = 0 To AccountGrid.Rows.Count - 1
            If AccountGrid.Rows(i).Cells(2).Value = Nothing And AccountGrid.Rows(i).Cells(6).Value <> Nothing Then
                Return False
            End If
        Next
        Return True
    End Function

    Private Sub AccountGrid_CellEndEdit(sender As Object, e As DataGridViewCellEventArgs) Handles AccountGrid.CellEndEdit
        Try
            Dim i As Integer = AccountGrid.CurrentCell.RowIndex
            Dim j As Integer = AccountGrid.CurrentCell.ColumnIndex
            If j = 4 Then
                DAILYTOTAL()
            End If
        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try
    End Sub

    Private Sub TxtAmount_TextChanged(sender As Object, e As EventArgs) Handles TxtAmount.TextChanged
        Try
            DAILYTOTAL()
        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try
    End Sub

    Private Sub CmdSave_Click(sender As Object, e As EventArgs) Handles CmdSave.Click
        Try
            If VALIDATE_DATA() = False Then
                Return
            Else
                If NOCOSTCENTER_() = False Then
                    MessageBox.Show("لا يمكن الحفظ بدون اختيار مركز التكلفة", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Error)
                    Return
                Else
                    If NODAY2NOTES_() = False Then
                        MessageBox.Show("لا يمكن الحفظ بدون ادخال البيان لتسهيل عملية المراجعة", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Error)
                        Return
                    Else
                        If TxtGuid.Text = Nothing Then
                            FORSAVE()
                        Else
                            If TxtLinkName.Text = Nothing Then
                                Dim w As New UpdateResaon
                                w.ShowDialog()
                                If w.IS_Yes = True Then
                                    REASON = w.TxtNotes.Text
                                    FORUPDATE()
                                End If
                            Else
                                MessageBox.Show("لا يمكن إجراء اى تعديلات على السند المحدد لانه مرتبط مع مستند آخر", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Error)
                                Return
                            End If
                        End If
                    End If
                End If
            End If
        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try
    End Sub

    Private Sub CmdFind_Click(sender As Object, e As EventArgs) Handles CmdFind.Click
        Try
            Dim w As New CashDayList
            w.Text = "سندات القبض"
            w.TxtType.Text = 0
            w.ShowDialog()
            If w.IS_Yes = True Then
                Dim ii As Integer = w.BranchGrid.CurrentRow.Index
                TxtCode.Text = w.BranchGrid.Rows(ii).Cells(0).Value.ToString()
                TxtGuid.Text = w.BranchGrid.Rows(ii).Cells(5).Value.ToString()
            End If
            If TxtGuid.Text <> Nothing Then
                SHOWRECEPITINFO()
                SHOWCASHDAYDETAIL()
                DAILYTOTAL()
                SHOWRECEPITDAY1INFO()
            End If
        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try
    End Sub
    Sub SHOWCASHDAYDETAIL()
        Dim connection = dbcontext.Database.Connection
        Dim comm As New SqlCommand("ShowCashDayDetail", connection)
        comm.Parameters.AddWithValue("@CashDayGuid", TxtGuid.Text)
        comm.CommandType = CommandType.StoredProcedure
        If connection.State = ConnectionState.Open Then
            connection.Close()
        End If
        connection.Open()
        comm.ExecuteNonQuery()
        Dim adp As New SqlDataAdapter(comm)
        Dim ds As DataSet = New DataSet()
        adp.Fill(ds, "ShowCashDayDetail")
        Dim COUNT__ As Integer = 0
        If ds.Tables(0).Rows.Count > 0 Then
            AccountGrid.RowCount = ds.Tables(0).Rows.Count
            For i As Integer = 0 To ds.Tables(0).Rows.Count - 1
                AccountGrid.Rows(COUNT__).Cells(0).Value = ds.Tables(0).Rows(i)(0).ToString()
                AccountGrid.Rows(COUNT__).Cells(1).Value = ds.Tables(0).Rows(i)(1).ToString()
                AccountGrid.Rows(COUNT__).Cells(2).Value = ds.Tables(0).Rows(i)(2).ToString()
                AccountGrid.Rows(COUNT__).Cells(3).Value = ds.Tables(0).Rows(i)(3).ToString()
                AccountGrid.Rows(COUNT__).Cells(4).Value = ds.Tables(0).Rows(i)(4).ToString()
                AccountGrid.Rows(COUNT__).Cells(5).Value = ds.Tables(0).Rows(i)(5).ToString()
                AccountGrid.Rows(COUNT__).Cells(6).Value = ds.Tables(0).Rows(i)(6).ToString()
                AccountGrid.Rows(COUNT__).Cells(7).Value = ds.Tables(0).Rows(i)(7).ToString()
                COUNT__ = COUNT__ + 1
            Next
        End If
    End Sub
    Sub NEW____()
        TxtDayDate.Value = Convert.ToDateTime(Now.ToShortDateString)
        TxtAccountName.Text = Nothing
        TxtAccountGuid.Text = Nothing
        TxtBranchName.Text = Nothing
        TxtBranchGuid.Text = Nothing
        TxtAmount.Text = 0
        TxtNotes.Text = Nothing
        TxtLinkName.Text = Nothing
        TxtLinkCode.Text = Nothing
        ChkStaut.Value = True
        TxtGuid.Text = Nothing
        TxtCode.Text = Nothing
        AccountGrid.Rows.Clear()
        TxtCenterName.Text = Nothing
        TxtCenterGuid.Text = Nothing
        DAILYTOTAL()
    End Sub
    Private Sub CMDNew_Click(sender As Object, e As EventArgs) Handles CMDNew.Click
        Try
            NEW____()
            EncodingCashDayReceipt_Load(e, e)
        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try
    End Sub

    Private Sub CmdClose_Click(sender As Object, e As EventArgs) Handles CmdClose.Click
        Me.Close()
    End Sub
    Sub SHOWTAFKEET()
        Dim connection = dbcontext.Database.Connection
        Dim comm As New SqlCommand("Tafkeet", connection)
        comm.Parameters.AddWithValue("@Total", Val(TxtAmount.Text))
        comm.CommandType = CommandType.StoredProcedure
        If connection.State = ConnectionState.Open Then
            connection.Close()
        End If
        connection.Open()
        comm.ExecuteNonQuery()
        Dim adp As New SqlDataAdapter(comm)
        Dim ds As DataSet = New DataSet()
        adp.Fill(ds, "Tafkeet")
        Dim COUNT__ As Integer = 0
        If ds.Tables(0).Rows.Count > 0 Then
            For i As Integer = 0 To ds.Tables(0).Rows.Count - 1
                TxtTafkeet.Text = ds.Tables(0).Rows(i)(0).ToString()
            Next
        End If
    End Sub
    Sub PRINT()
        Dim connection = dbcontext.Database.Connection
        Dim comm As New SqlCommand
        comm = New SqlCommand("PrintCashday", connection)
        comm.Parameters.AddWithValue("@Guid", TxtGuid.Text)
        comm.CommandType = CommandType.StoredProcedure
        If connection.State = ConnectionState.Open Then
            connection.Close()
        End If
        connection.Open()
        comm.ExecuteNonQuery()
        Dim adp As New SqlDataAdapter(comm)
        Dim ds As DataSet = New DataSet()
        adp.Fill(ds, "PrintCashday")
        Dim dt = ds.Tables(0)
        Dim report As New CashDayDoc
        report.DataSource = dt
        report.DataAdapter = adp
        report.DataMember = "DataTable1"
        report.RequestParameters = False
        report.Parameters(0).Value = TxtTafkeet.Text
        Dim Tool_Report As ReportPrintTool = New ReportPrintTool(report)
        report.CreateDocument()
        Dim frm As New ViewReport
        frm.DocumentViewer1.DocumentSource = report
        frm.ShowDialog()
    End Sub
    Private Sub CmdExit_Click(sender As Object, e As EventArgs) Handles CmdExit.Click
        Try
            If TxtGuid.Text <> Nothing Then
                PRINT()
            Else
                MessageBox.Show("برجاء اختيار سند او حفظ السند الحالي", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Error)
                Return
            End If
        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try
    End Sub
End Class