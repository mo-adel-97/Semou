Imports System.Data.SqlClient

Public Class IfStudentNotRegister
    Dim dbcontext As New SemouEntities
    Dim Olddbcontext As New SSTLI2023Entities
    Public IS_Yes As Boolean = False
    Dim NOTES As String
    Dim REASON As String = ""
    Private Sub IfStudentNotRegister_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Try
            RegDocPosted.WHATFORMTYPEGUID("opendaily")
            TxtTypeGuid.Text = RegDocPosted.TYPEGUID
            AllFiles.GETPARENTINFOFROMSETTING("10")
            TxtOpenAccountGuid.Text = AllFiles.OPENACCOUNTGUID.ToString()
        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try
    End Sub

    Private Sub CmdFindDepart_Click(sender As Object, e As EventArgs) Handles CmdFindDepart.Click
        Try
            Dim w As New NewBranchList
            w.TxtType.Text = 0
            w.TxtIsUse.Text = True
            w.ShowDialog()
            If w.IS_Yes = True Then
                Dim ii As Integer = w.BranchGrid.CurrentRow.Index
                TxtBranchId.Text = w.BranchGrid.Rows(ii).Cells(0).Value.ToString()
                TxtBranchName.Text = w.BranchGrid.Rows(ii).Cells(1).Value.ToString()
                TxtBranchGuid.Text = w.BranchGrid.Rows(ii).Cells(3).Value.ToString()
            End If
            If TxtAction.Text = 1 Then
                If TxtBranchId.Text <> Nothing Then
                    SHOWBRANCHINFO()
                End If
            End If
        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try
    End Sub

    Private Sub CmdFindDiplom_Click(sender As Object, e As EventArgs) Handles CmdFindDiplom.Click
        Try
            Dim w As New DiplomList
            w.TxtIsUse.Text = True
            w.TxtType.Text = 0
            w.TxtStudyType.Text = 0
            w.ShowDialog()
            If w.IS_Yes = True Then
                Dim ii As Integer = w.BranchGrid.CurrentRow.Index
                TxtDiplomName.Text = w.BranchGrid.Rows(ii).Cells(1).Value.ToString()
                TxtDiplomGuid.Text = w.BranchGrid.Rows(ii).Cells(3).Value.ToString()
            End If
        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try
    End Sub

    Private Sub CmdFindCourse_Click(sender As Object, e As EventArgs) Handles CmdFindCourse.Click
        Try
            Dim w As New CourseList
            w.TxtBranchGuid.Text = TxtBranchGuid.Text
            w.TxtIsUse.Text = True
            w.ShowDialog()
            If w.IS_Yes = True Then
                Dim ii As Integer = w.BranchGrid.CurrentRow.Index
                TxtCourseName.Text = w.BranchGrid.Rows(ii).Cells(6).Value.ToString()
                TxtCourseGuid.Text = w.BranchGrid.Rows(ii).Cells(5).Value.ToString()
                TxtCourseDiplomGuid.Text = w.BranchGrid.Rows(ii).Cells(7).Value.ToString()
            End If
        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try
    End Sub

    Private Sub CmdFindBatch_Click(sender As Object, e As EventArgs) Handles CmdFindBatch.Click
        Try
            Dim w As New BatchList
            w.TxtIsUse.Text = True
            w.ShowDialog()
            If w.IS_Yes = True Then
                Dim ii As Integer = w.BranchGrid.CurrentRow.Index
                TxtBatchName.Text = w.BranchGrid.Rows(ii).Cells(1).Value.ToString()
                TxtBatchGuid.Text = w.BranchGrid.Rows(ii).Cells(3).Value.ToString()
            End If
        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try
    End Sub

    Private Sub CmdFindLevel_Click(sender As Object, e As EventArgs) Handles CmdFindLevel.Click
        Try
            Dim w As New LevelList
            w.TxtIsUse.Text = True
            w.ShowDialog()
            If w.IS_Yes = True Then
                Dim ii As Integer = w.BranchGrid.CurrentRow.Index
                TxtLevelName.Text = w.BranchGrid.Rows(ii).Cells(1).Value.ToString()
                TxtLevelGuid.Text = w.BranchGrid.Rows(ii).Cells(3).Value.ToString()
            End If
        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try
    End Sub

    Private Sub CmdFindCase_Click(sender As Object, e As EventArgs) Handles CmdFindCase.Click
        Try
            Dim w As New StautList
            w.TxtIsUse.Text = True
            w.ShowDialog()
            If w.IS_Yes = True Then
                Dim ii As Integer = w.BranchGrid.CurrentRow.Index
                TxtCaseName.Text = w.BranchGrid.Rows(ii).Cells(1).Value.ToString()
                TxtCaseGuid.Text = w.BranchGrid.Rows(ii).Cells(3).Value.ToString()
            End If
        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try
    End Sub

    Private Sub CboType_SelectedIndexChanged(sender As Object, e As EventArgs) Handles CboType.SelectedIndexChanged

    End Sub

    Private Sub CboType_SelectionChangeCommitted(sender As Object, e As EventArgs) Handles CboType.SelectionChangeCommitted
        Try
            If CboType.SelectedIndex = 0 Then
                TxtBatchName.Enabled = True
                TxtDiplomName.Enabled = True
                CmdFindDiplom.Enabled = True
                CmdFindBatch.Enabled = True
                TxtCourseName.Enabled = False
                CmdFindCourse.Enabled = False
                TxtCourseName.Text = Nothing
                TxtCourseGuid.Text = Nothing
                CmdFindLevel.Enabled = True
            ElseIf CboType.SelectedIndex = 1 Then
                TxtBatchName.Enabled = False
                TxtDiplomName.Enabled = False
                CmdFindDiplom.Enabled = False
                CmdFindBatch.Enabled = False
                TxtCourseName.Enabled = True
                CmdFindCourse.Enabled = True
                TxtBatchName.Text = Nothing
                TxtBatchGuid.Text = Nothing
                TxtDiplomName.Text = Nothing
                TxtDiplomGuid.Text = Nothing
                CmdFindLevel.Enabled = False
                TxtLevelGuid.Text = Nothing
                TxtLevelName.Text = Nothing
            End If
        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try
    End Sub
    Sub SHOWBRANCHINFO()
        Dim connection = dbcontext.Database.Connection
        Dim comm As New SqlCommand
        comm = New SqlCommand("ShowBarnchInfo", connection)
        comm.CommandType = CommandType.StoredProcedure
        comm.Parameters.AddWithValue("@Code", TxtBranchId.Text)
        If connection.State = ConnectionState.Open Then
            connection.Close()
        End If
        connection.Open()
        comm.ExecuteNonQuery()
        Dim adp As New SqlDataAdapter(comm)
        Dim ds As DataSet = New DataSet()
        adp.Fill(ds, "ShowBarnchInfo")
        Dim COUNT__ As Integer = 0
        If ds.Tables(0).Rows.Count > 0 Then
            For i As Integer = 0 To ds.Tables(0).Rows.Count - 1
                TxtCostCenterGuid.Text = ds.Tables(0).Rows(i)(12).ToString()
            Next
        End If
    End Sub
    Public Function GETDAY1__NO() As Integer
        Dim TYPEGUID As Guid = Guid.Parse(TxtTypeGuid.Text)
        Dim MaxId = dbcontext.Day1.Where(Function(m) m.TypeGuid = TYPEGUID).OrderByDescending(Function(p) p.ID).FirstOrDefault()?.ID
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
        Dim comm = New SqlCommand("AddOpenDay1", connection)
        comm.CommandType = CommandType.StoredProcedure
        'comm.Parameters.AddWithValue("@Id", GETDAY1__NO)
        'comm.Parameters.AddWithValue("@Code", TxtCode.Text)
        comm.Parameters.AddWithValue("@DayDate", Convert.ToDateTime(DateTime.Now))
        comm.Parameters.AddWithValue("@Notes", NOTES)
        comm.Parameters.AddWithValue("@DayType", False)
        comm.Parameters.AddWithValue("@DocGuid", "00000000-0000-0000-0000-000000000000")
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
        TxtCode.Text = RETURNCODE.ToString()
        comm.Parameters.Clear()
    End Sub
    Sub SHOWDAY1INFO()
        Dim connection = dbcontext.Database.Connection
        Dim comm As New SqlCommand("ShowDay1Info", connection)
        comm.Parameters.AddWithValue("@Code", TxtCode.Text)
        comm.Parameters.AddWithValue("@TypeGuid", TxtTypeGuid.Text)
        comm.CommandType = CommandType.StoredProcedure
        If connection.State = ConnectionState.Open Then
            connection.Close()
        End If
        connection.Open()
        comm.ExecuteNonQuery()
        Dim adp As New SqlDataAdapter(comm)
        Dim ds As DataSet = New DataSet()
        adp.Fill(ds, "ShowDay1Info")
        Dim COUNT__ As Integer = 0
        If ds.Tables(0).Rows.Count > 0 Then
            For i As Integer = 0 To ds.Tables(0).Rows.Count - 1
                TxtDay1Guid.Text = ds.Tables(0).Rows(i)(3).ToString()
                COUNT__ = COUNT__ + 1
            Next
        End If
    End Sub
    Sub ADDPAYDAY2_MADEN_STUDENT()
        Dim connection = dbcontext.Database.Connection
        Dim comm = New SqlCommand("AddDay2", connection)
        comm.CommandType = CommandType.StoredProcedure
        comm.Parameters.AddWithValue("@Day1Guid", TxtDay1Guid.Text)
        comm.Parameters.AddWithValue("@AccountGuid", TxtAccountGuid.Text)
        comm.Parameters.AddWithValue("@Maden", Val(TxtMaden.Text))
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
    Sub ADDPAYDAY2_DAEN_OPENACCOUNT()
        Dim connection = dbcontext.Database.Connection
        Dim comm = New SqlCommand("AddDay2", connection)
        comm.CommandType = CommandType.StoredProcedure
        comm.Parameters.AddWithValue("@Day1Guid", TxtDay1Guid.Text)
        comm.Parameters.AddWithValue("@AccountGuid", TxtOpenAccountGuid.Text)
        comm.Parameters.AddWithValue("@Maden", 0)
        comm.Parameters.AddWithValue("@Daen", Val(TxtMaden.Text))
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
    Sub ADDPAYDAY2_MADEN_OPENACCOUNT()
        Dim connection = dbcontext.Database.Connection
        Dim comm = New SqlCommand("AddDay2", connection)
        comm.CommandType = CommandType.StoredProcedure
        comm.Parameters.AddWithValue("@Day1Guid", TxtDay1Guid.Text)
        comm.Parameters.AddWithValue("@AccountGuid", TxtOpenAccountGuid.Text)
        comm.Parameters.AddWithValue("@Maden", Val(TxtDaen.Text))
        comm.Parameters.AddWithValue("@Daen", 0)
        comm.Parameters.AddWithValue("@Notes", "الرصيد الأفتتاحي للطالب")
        comm.Parameters.AddWithValue("@CostGuid", TxtCostCenterGuid.Text)
        comm.Parameters.AddWithValue("@IsUse", True)
        If connection.State = ConnectionState.Open Then
            connection.Close()
        End If
        connection.Open()
        comm.ExecuteNonQuery()
        comm.Parameters.Clear()
    End Sub
    Sub ADDPAYDAY2_DAEN_STUDENT()
        Dim connection = dbcontext.Database.Connection
        Dim comm = New SqlCommand("AddDay2", connection)
        comm.CommandType = CommandType.StoredProcedure
        comm.Parameters.AddWithValue("@Day1Guid", TxtDay1Guid.Text)
        comm.Parameters.AddWithValue("@AccountGuid", TxtAccountGuid.Text)
        comm.Parameters.AddWithValue("@Maden", 0)
        comm.Parameters.AddWithValue("@Daen", Val(TxtDaen.Text))
        comm.Parameters.AddWithValue("@Notes", "الرصيد الأفتتاحي للطالب")
        comm.Parameters.AddWithValue("@CostGuid", TxtCostCenterGuid.Text)
        comm.Parameters.AddWithValue("@IsUse", True)
        If connection.State = ConnectionState.Open Then
            connection.Close()
        End If
        connection.Open()
        comm.ExecuteNonQuery()
        comm.Parameters.Clear()
    End Sub
    Sub ADDSTUDENTLEVEL()
        Dim connection = dbcontext.Database.Connection
        Dim comm = New SqlCommand("AddStudentLevel", connection)
        comm.CommandType = CommandType.StoredProcedure
        comm.Parameters.AddWithValue("@StudentGuid", TxtAccountGuid.Text)
        comm.Parameters.AddWithValue("@RegDocGuid", "00000000-0000-0000-0000-000000000000")
        comm.Parameters.AddWithValue("@LevelGuid", TxtLevelGuid.Text)
        comm.Parameters.AddWithValue("@LevelMark", 0)
        comm.Parameters.AddWithValue("@LevelStaut", True)
        comm.Parameters.AddWithValue("@BatchGuid", TxtBatchGuid.Text)
        comm.Parameters.AddWithValue("@DiplomGuid", TxtDiplomGuid.Text)
        comm.Parameters.AddWithValue("@BranchGuid", TxtBranchGuid.Text)
        comm.Parameters.AddWithValue("@CourseGuid", "00000000-0000-0000-0000-000000000000")
        comm.Parameters.AddWithValue("@CaseGuid", TxtCaseGuid.Text)
        comm.Parameters.AddWithValue("@RegType", 0)

        If connection.State = ConnectionState.Open Then
            connection.Close()
        End If
        connection.Open()
        comm.ExecuteNonQuery()
        comm.Parameters.Clear()
    End Sub
    Sub ADDSTUDENTCOURSE()
        Dim connection = dbcontext.Database.Connection
        Dim comm = New SqlCommand("AddStudentLevel", connection)
        comm.CommandType = CommandType.StoredProcedure
        comm.Parameters.AddWithValue("@StudentGuid", TxtAccountGuid.Text)
        comm.Parameters.AddWithValue("@RegDocGuid", "00000000-0000-0000-0000-000000000000")
        comm.Parameters.AddWithValue("@LevelGuid", "00000000-0000-0000-0000-000000000000")
        comm.Parameters.AddWithValue("@LevelMark", 0)
        comm.Parameters.AddWithValue("@LevelStaut", True)
        comm.Parameters.AddWithValue("@BatchGuid", "00000000-0000-0000-0000-000000000000")
        comm.Parameters.AddWithValue("@DiplomGuid", TxtCourseDiplomGuid.Text)
        comm.Parameters.AddWithValue("@BranchGuid", TxtBranchGuid.Text)
        comm.Parameters.AddWithValue("@CourseGuid", TxtCourseGuid.Text)
        comm.Parameters.AddWithValue("@CaseGuid", TxtCaseGuid.Text)
        comm.Parameters.AddWithValue("@RegType", 1)
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
        comm.Parameters.AddWithValue("@ActionGuid", TxtDay1Guid.Text)
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
    Function VALIDATE_DATA() As Boolean
        If CboType.Text = Nothing Then
            MessageBox.Show("برجاء اختيار نوع التسجيل اولا", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return False
        Else
            If CboType.SelectedIndex = 0 Then ''''''' دبلوم
                If TxtBranchGuid.Text = Nothing Then
                    MessageBox.Show("برجاء اختيار الفرع اولا", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Error)
                    Return False
                ElseIf TxtDiplomGuid.Text = Nothing Then
                    MessageBox.Show("برجاء اختيار الدبلوم اولا", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Error)
                    Return False
                ElseIf TxtBatchGuid.Text = Nothing Then
                    MessageBox.Show("برجاء اختيار الدفعه اولا", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Error)
                    Return False
                ElseIf TxtLevelGuid.Text = Nothing Then
                    MessageBox.Show("برجاء اختيار المستوي اولا", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Error)
                    Return False
                ElseIf TxtCaseGuid.Text = Nothing Then
                    MessageBox.Show("برجاءاختيار الحالة اولا", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Error)
                    Return False
                End If
            ElseIf CboType.SelectedIndex = 1 Then '''''' دورة
                If TxtBranchGuid.Text = Nothing Then
                    MessageBox.Show("برجاء اختيار الفرع اولا", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Error)
                    Return False
                ElseIf TxtCourseGuid.Text = Nothing Then
                    MessageBox.Show("برجاء اختيار الدورة اولا", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Error)
                    Return False
                ElseIf TxtCaseGuid.Text = Nothing Then
                    MessageBox.Show("برجاءاختيار الحالة اولا", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Error)
                    Return False
                End If
            End If
        End If
        Return True
    End Function
    Private Sub CmdAccept_Click(sender As Object, e As EventArgs) Handles CmdAccept.Click
        If TxtAction.Text = 0 Then
            If VALIDATE_DATA() = False Then
                Return
            Else
                IS_Yes = True
                Me.Close()
            End If
        Else
            If VALIDATE_DATA() = False Then
                Return
            Else
                If TxtDay1Guid.Text = Nothing Then
                    AllFiles.ActionType___ = "دمج حساب"
                    If TxtMaden.Text <> 0 Or TxtDaen.Text <> 0 Then
                        NOTES = "رصيد أفتتاحي للطالب"
                        ADDDAY1()
                        SHOWDAY1INFO()
                        If TxtMaden.Text <> 0 Then
                            ADDPAYDAY2_MADEN_STUDENT()
                            ADDPAYDAY2_DAEN_OPENACCOUNT()
                        ElseIf TxtDaen.Text <> 0 Then
                            ADDPAYDAY2_MADEN_OPENACCOUNT()
                            ADDPAYDAY2_DAEN_STUDENT()
                        End If
                    End If
                    If CboType.SelectedIndex = 0 Then
                        ADDSTUDENTLEVEL()
                    ElseIf CboType.SelectedIndex = 1 Then
                        ADDSTUDENTCOURSE()
                    End If
                    ADD_USERACTION_()
                    MessageBox.Show("تم دمج الحساب بنجاح", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Information)
                End If
            End If
        End If
    End Sub

    Private Sub CmdExit_Click(sender As Object, e As EventArgs) Handles CmdExit.Click
        Me.Close()
    End Sub
    Sub SHOWBALANCE(BRANCHID As String)
        Dim connection = Olddbcontext.Database.Connection
        Dim comm As New SqlCommand
        comm = New SqlCommand("CalcCustBAlance", connection)
        comm.CommandType = CommandType.StoredProcedure
        comm.Parameters.AddWithValue("@CustNo", TxtOldCustNo.Text)
        comm.Parameters.AddWithValue("@BrnId", BRANCHID)
        If connection.State = ConnectionState.Open Then
            connection.Close()
        End If
        connection.Open()
        comm.ExecuteNonQuery()
        Dim adp As New SqlDataAdapter(comm)
        Dim ds As DataSet = New DataSet()
        adp.Fill(ds, "CalcCustBAlance")
        Dim COUNT__ As Integer = 0
        If ds.Tables(0).Rows.Count > 0 Then
            For i As Integer = 0 To ds.Tables(0).Rows.Count - 1
                If Val(ds.Tables(0).Rows(i)(3)) > 0 Then
                    TxtMaden.Text = ds.Tables(0).Rows(i)(3).ToString
                Else
                    TxtDaen.Text = Math.Abs(ds.Tables(0).Rows(i)(3))
                End If
            Next
        End If
    End Sub
End Class