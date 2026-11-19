Imports System.Data.SqlClient
Imports DevExpress.XtraReports.UI

Public Class EncodingDaily
    Dim dbcontext As New SemouEntities
    Dim REASON As String = ""
    Private Sub CmdExit_Click(sender As Object, e As EventArgs) Handles CmdExit.Click
        Me.Close()
    End Sub

    Private Sub EncodingDaily_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Try
            RegDocPosted.WHATFORMTYPEGUID("opendaily")
            TxtOpenGuid.Text = RegDocPosted.TYPEGUID
            RegDocPosted.WHATFORMTYPEGUID("daily")
            TxtTypeGuid.Text = RegDocPosted.TYPEGUID
            Dim row As DataGridViewRow = AccountGrid.Rows(0)
            row.Height = 27
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
        Dim DIPLOMTOTAL As Double = 0
        For i As Integer = 0 To AccountGrid.Rows.Count - 1
            If AccountGrid.Rows(i).Cells(6).Value <> Nothing Then
                DIPLOMTOTAL = Val(DIPLOMTOTAL) + (Val(AccountGrid.Rows(i).Cells(3).Value))
                DIPLOMTAX = Val(DIPLOMTAX) + (Val(AccountGrid.Rows(i).Cells(4).Value))
            End If
        Next
        TxtTotalMaden.Text = Math.Round(Val(DIPLOMTOTAL), 2)
        TxtTotalDaen.Text = Math.Round(Val(DIPLOMTAX), 2)
        TxtDef.Text = Math.Round(Val(TxtTotalMaden.Text) - Val(TxtTotalDaen.Text), 2)
    End Sub
    Private Sub AccountGrid_CellClick(sender As Object, e As DataGridViewCellEventArgs) Handles AccountGrid.CellClick

    End Sub

    Private Sub AccountGrid_CellEndEdit(sender As Object, e As DataGridViewCellEventArgs) Handles AccountGrid.CellEndEdit
        Try
            Dim i As Integer = AccountGrid.CurrentCell.RowIndex
            Dim j As Integer = AccountGrid.CurrentCell.ColumnIndex
            If j = 3 Or j = 4 Then
                DAILYTOTAL()
            End If
        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try
    End Sub

    Private Sub AccountGrid_UserDeletedRow(sender As Object, e As DataGridViewRowEventArgs) Handles AccountGrid.UserDeletedRow
        Try
            DAILYTOTAL()
        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try
    End Sub
    Function VALIDATE_DATA() As Boolean
        'If TxtBranchGuid.Text = Nothing Then
        '    MessageBox.Show("برجاء اختيار الفرع اولا", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Error)
        '    Return False
        If TxtNotes.Text = Nothing Then
            MessageBox.Show("برجاء ادخال ملاحظات القيد لتسهيل عملية مراجعه الحسابات", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return False
        ElseIf TxtTotalMaden.Text = 0 And TxtZeroAmount.Text = 0 Then
            MessageBox.Show("برجاء التأكد من المبالغ المدرجة فى الجانب المدين", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return False
        ElseIf TxtTotalDaen.Text = 0 And TxtZeroAmount.Text = 0 Then
            MessageBox.Show("برجاء التأكد من المبالغ المدرجة فى الجانب الدائن", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return False
        ElseIf TxtDef.Text <> 0 Then
            MessageBox.Show("القيد غير متزن برجاء مراجعه تفاصيل القيد", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Error)
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
    Public Function GETDAY1__NO() As Integer
        Dim OPENGUID As Guid = Guid.Parse(TxtOpenGuid.Text)
        Dim MaxId = dbcontext.Day1.Where(Function(m) m.TypeGuid <> OPENGUID).OrderByDescending(Function(p) p.ID).FirstOrDefault()?.ID
        Dim MaxDay = New Day1
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
    Sub ADDDAY1()
        Dim connection = dbcontext.Database.Connection
        Dim comm = New SqlCommand("AddDailyDay1", connection)
        comm.CommandType = CommandType.StoredProcedure
        comm.Parameters.AddWithValue("@OpenGuid", TxtOpenGuid.Text)
        comm.Parameters.AddWithValue("@DayDate", Convert.ToDateTime(TxtDayDate.Value))
        comm.Parameters.AddWithValue("@Notes", TxtNotes.Text)
        comm.Parameters.AddWithValue("@DayType", False)
        comm.Parameters.AddWithValue("@DocGuid", "00000000-0000-0000-0000-000000000000")
        comm.Parameters.AddWithValue("@IsUse", ChkStaut.Value)
        comm.Parameters.AddWithValue("@TypeGuid", TxtTypeGuid.Text)
        'comm.Parameters.AddWithValue("@ActionCode", TxtCode.Text)
        comm.Parameters.AddWithValue("@BranchGuid", "00000000-0000-0000-0000-000000000000")
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
    Sub SHOWDAY1INFO()
        Dim connection = dbcontext.Database.Connection
        Dim comm As New SqlCommand("ShowDay1Info2", connection)
        comm.Parameters.AddWithValue("@Code", TxtCode.Text)
        comm.Parameters.AddWithValue("@TypeGuid", TxtOpenGuid.Text)
        comm.CommandType = CommandType.StoredProcedure
        If connection.State = ConnectionState.Open Then
            connection.Close()
        End If
        connection.Open()
        comm.ExecuteNonQuery()
        Dim adp As New SqlDataAdapter(comm)
        Dim ds As DataSet = New DataSet()
        adp.Fill(ds, "ShowDay1Info2")
        Dim COUNT__ As Integer = 0
        If ds.Tables(0).Rows.Count > 0 Then
            For i As Integer = 0 To ds.Tables(0).Rows.Count - 1
                TxtDayDate.Value = Convert.ToDateTime(ds.Tables(0).Rows(i)(0)).ToShortDateString()
                TxtNotes.Text = ds.Tables(0).Rows(i)(1).ToString()
                ChkStaut.Value = ds.Tables(0).Rows(i)(2)
                'TxtBranchGuid.Text = ds.Tables(0).Rows(i)(3).ToString()
                'TxtBranchName.Text = ds.Tables(0).Rows(i)(4).ToString()
                TxtGuid.Text = ds.Tables(0).Rows(i)(3).ToString()
                TxtType.Text = ds.Tables(0).Rows(i)(5).ToString()
                COUNT__ = COUNT__ + 1
            Next
        End If
    End Sub
    Sub ADDDAY2()
        Dim connection = dbcontext.Database.Connection
        Dim comm = New SqlCommand("AddDay2", connection)
        comm.CommandType = CommandType.StoredProcedure
        For i As Integer = 0 To AccountGrid.Rows.Count - 1
            If AccountGrid.Rows(i).Cells(6).Value = Nothing Then
                Exit For
            Else
                comm.Parameters.AddWithValue("@Day1Guid", TxtGuid.Text)
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
    Sub UPDATEDAY1()
        Dim connection = dbcontext.Database.Connection
        Dim comm = New SqlCommand("UpdateDay1", connection)
        comm.CommandType = CommandType.StoredProcedure
        comm.Parameters.AddWithValue("@Guid", TxtGuid.Text)
        comm.Parameters.AddWithValue("@DayDate", Convert.ToDateTime(TxtDayDate.Value))
        comm.Parameters.AddWithValue("@Notes", TxtNotes.Text)
        comm.Parameters.AddWithValue("@IsUse", ChkStaut.Value)
        comm.Parameters.AddWithValue("@BranchGuid", "00000000-0000-0000-0000-000000000000")
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
        comm.Parameters.AddWithValue("@Day1Guid", TxtGuid.Text)
        If connection.State = ConnectionState.Open Then
            connection.Close()
        End If
        connection.Open()
        comm.ExecuteNonQuery()
        comm.Parameters.Clear()
    End Sub
    Sub FORSAVE()
        If AllFiles.FORSAVE("daily") = False Then
            Return
        Else
            AllFiles.ActionType___ = "جديد"
            ADDDAY1()
            SHOWDAY1INFO()
            ADDDAY2()
            ADD_USERACTION_()
            MessageBox.Show("تم حفظ قيد اليومية بنجاح", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Information)
        End If
    End Sub
    Sub FORUPDATE()
        If AllFiles.FORUPDATE("daily") = False Then
            Return
        Else
            AllFiles.ActionType___ = "تعديل"
            UPDATEDAY1()
            DELETEDAY2()
            ADDDAY2()
            ADD_USERACTION_()
            MessageBox.Show("تم تعديل قيد اليومية بنجاح", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Information)
        End If
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
                            If TxtType.Text = True Then
                                MessageBox.Show("لا يمكن التعديل على قيد يومية مرتبط بمستند آخر", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Error)
                                Return
                            Else
                                Dim w As New UpdateResaon
                                w.ShowDialog()
                                If w.IS_Yes = True Then
                                    REASON = w.TxtNotes.Text
                                    FORUPDATE()
                                End If
                            End If
                        End If
                    End If
                End If
            End If
        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try
    End Sub
    Sub SHOWDAILYDETAILS()
        Dim connection = dbcontext.Database.Connection
        Dim comm As New SqlCommand("ShowDay1Details", connection)
        comm.Parameters.AddWithValue("@Day1Guid", TxtGuid.Text)
        comm.CommandType = CommandType.StoredProcedure
        If connection.State = ConnectionState.Open Then
            connection.Close()
        End If
        connection.Open()
        comm.ExecuteNonQuery()
        Dim adp As New SqlDataAdapter(comm)
        Dim ds As DataSet = New DataSet()
        adp.Fill(ds, "ShowDay1Details")
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
                AccountGrid.Rows(COUNT__).Cells(8).Value = ds.Tables(0).Rows(i)(8).ToString()
                COUNT__ = COUNT__ + 1
            Next
        End If
    End Sub
    Private Sub CmdFind_Click(sender As Object, e As EventArgs) Handles CmdFind.Click
        Try
            Dim w As New ListOpenDaily
            w.Text = "قائمة قيود اليومية"
            w.TxtType.Text = "normal"
            w.TxtTypeGuid.Text = TxtOpenGuid.Text
            w.ShowDialog()
            If w.IS_Yes = True Then
                Dim ii As Integer = w.BranchGrid.CurrentRow.Index
                TxtCode.Text = w.BranchGrid.Rows(ii).Cells(0).Value.ToString()
            End If
            If TxtCode.Text <> Nothing Then
                SHOWDAY1INFO()
                If TxtGuid.Text <> Nothing Then
                    SHOWDAILYDETAILS()
                    DAILYTOTAL()
                End If
            End If
        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try
    End Sub
    Sub NEW___()
        TxtCode.Text = Nothing
        TxtGuid.Text = Nothing
        TxtBranchName.Text = Nothing
        TxtBranchGuid.Text = Nothing
        TxtDayDate.Value = Convert.ToDateTime(Now.ToShortDateString)
        TxtNotes.Text = Nothing
        AccountGrid.Rows.Clear()
        TxtTotalDaen.Text = 0
        TxtTotalMaden.Text = 0
        TxtDef.Text = 0
        ChkStaut.Value = True
        TxtType.Text = Nothing
    End Sub
    Private Sub CMDNew_Click(sender As Object, e As EventArgs) Handles CMDNew.Click
        Try
            NEW___()
            EncodingDaily_Load(e, e)
        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try
    End Sub
    Sub PRINT()
        Dim connection = dbcontext.Database.Connection
        Dim comm As New SqlCommand
        comm = New SqlCommand("PrintDailyDoc", connection)
        comm.Parameters.AddWithValue("@Guid", TxtGuid.Text)
        comm.CommandType = CommandType.StoredProcedure
        If connection.State = ConnectionState.Open Then
            connection.Close()
        End If
        connection.Open()
        comm.ExecuteNonQuery()
        Dim adp As New SqlDataAdapter(comm)
        Dim ds As DataSet = New DataSet()
        adp.Fill(ds, "PrintDailyDoc")
        Dim dt = ds.Tables(0)
        Dim report As New DailyDoc
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
                PRINT()
            Else
                MessageBox.Show("برجاء اختيار قيد او حفظ القيد الحالي", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Error)
                Return
            End If
        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try
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
                    AccountGrid.Rows(i).Cells(8).Value = w.BranchGrid.Rows(ii).Cells(6).Value.ToString()
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
                    AccountGrid.Rows(i).Cells(8).Value = w.BranchGrid.Rows(ii).Cells(6).Value.ToString()
                End If
            ElseIf j = 5 Then
                If AccountGrid.Rows(i).Cells(8).Value.ToString() = 0 Or AccountGrid.Rows(i).Cells(8).Value.ToString() = 1 Then
                    Dim w As New BranchCostList
                    'w.TxtAccountType.Text = 1
                    w.ShowDialog()
                    If w.IS_Yes = True Then
                        Dim ii As Integer = w.BranchGrid.CurrentRow.Index
                        AccountGrid.Rows(i).Cells(5).Value = w.BranchGrid.Rows(ii).Cells(1).Value.ToString()
                        AccountGrid.Rows(i).Cells(7).Value = w.BranchGrid.Rows(ii).Cells(2).Value.ToString()
                    End If
                Else
                    AccountGrid.Rows(i).Cells(7).Value = "00000000-0000-0000-0000-000000000000"
                    MessageBox.Show("الحساب غير مطلوب له مركز تكلفة", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Error)
                    Return
                End If
            End If
        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try
    End Sub
End Class