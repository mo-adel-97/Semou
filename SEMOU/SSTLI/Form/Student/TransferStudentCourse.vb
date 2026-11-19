Imports System.Data.SqlClient

Public Class TransferStudentCourse
    Dim dbcontext As New SemouEntities
    Dim REASON As String = ""
    Dim OLDCOURSE As String
    Dim OLDREGBRANCH As String
    Dim OLDSTDYBRANCH As String
    Dim OLDDIPLOMNAME As String
    Dim OLDBATCH As String
    Private Sub CmdExit_Click(sender As Object, e As EventArgs) Handles CmdExit.Click
        Me.Close()
    End Sub

    Private Sub ChkUpdate1_CheckedChanged(sender As Object, e As EventArgs) Handles ChkUpdate1.CheckedChanged
        Try
            If ChkUpdate1.Checked = True Then
                CmdFindBranch1.Enabled = True
                OLDREGBRANCH = TxtBranch1Name.Text
                TxtBranch1Name.Text = Nothing
                TxtNewBranch1Guid.Text = Nothing
            Else
                CmdFindBranch1.Enabled = False
                TxtBranch1Name.Text = OLDREGBRANCH
                TxtNewBranch1Guid.Text = "00000000-0000-0000-0000-000000000000"
            End If
        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try
    End Sub

    Private Sub ChkUpdate2_CheckedChanged(sender As Object, e As EventArgs) Handles ChkUpdate2.CheckedChanged
        Try
            If ChkUpdate2.Checked = True Then
                CmdFindBranch2.Enabled = True
                OLDSTDYBRANCH = TxtBranch2Name.Text
                TxtBranch2Name.Text = Nothing
                TxtNewBranch2Guid.Text = Nothing
            Else
                CmdFindBranch2.Enabled = False
                TxtBranch2Name.Text = OLDSTDYBRANCH
                TxtNewBranch2Guid.Text = "00000000-0000-0000-0000-000000000000"
            End If
        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try
    End Sub

    Private Sub ChkUpdate3_CheckedChanged(sender As Object, e As EventArgs) Handles ChkUpdate3.CheckedChanged
        Try
            If TxtRegType.Text = 1 Then
                If ChkUpdate3.Checked = True Then
                    CmdFindCourse.Enabled = True
                    OLDCOURSE = TxtCourseName.Text
                    TxtCourseName.Text = Nothing
                    TxtnewCourseGuid.Text = Nothing
                    TxtCourseDiplomGuid.Text = Nothing
                Else
                    CmdFindCourse.Enabled = False
                    TxtCourseName.Text = OLDCOURSE
                    TxtnewCourseGuid.Text = "00000000-0000-0000-0000-000000000000"
                    TxtCourseDiplomGuid.Text = "00000000-0000-0000-0000-000000000000"
                End If
            Else
                ChkUpdate3.Checked = False
            End If
        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try
    End Sub

    Private Sub CmdFindBranch2_Click(sender As Object, e As EventArgs) Handles CmdFindBranch2.Click
        Try
            Dim w As New NewBranchList
            w.TxtType.Text = 0
            w.TxtIsUse.Text = True
            w.ShowDialog()
            If w.IS_Yes = True Then
                Dim ii As Integer = w.BranchGrid.CurrentRow.Index
                TxtBranch2Name.Text = w.BranchGrid.Rows(ii).Cells(1).Value.ToString()
                TxtNewBranch2Guid.Text = w.BranchGrid.Rows(ii).Cells(3).Value.ToString()
            End If
        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try
    End Sub

    Private Sub CmdFindDiplom_Click(sender As Object, e As EventArgs) Handles CmdFindCourse.Click
        Try
            If TxtNewBranch2Guid.Text <> Nothing Then
                Dim w As New CourseList
                w.TxtBranchGuid.Text = TxtNewBranch2Guid.Text
                w.TxtIsUse.Text = True
                w.ShowDialog()
                If w.IS_Yes = True Then
                    Dim ii As Integer = w.BranchGrid.CurrentRow.Index
                    TxtCourseName.Text = w.BranchGrid.Rows(ii).Cells(3).Value.ToString()
                    TxtnewCourseGuid.Text = w.BranchGrid.Rows(ii).Cells(5).Value.ToString()
                    TxtFromDate.Text = w.BranchGrid.Rows(ii).Cells(1).Value.ToString()
                    TxtCourseDiplomGuid.Text = w.BranchGrid.Rows(ii).Cells(7).Value.ToString()
                End If
            Else
                MessageBox.Show("برجاء اختيار فرع الدراسة الجديد اولا", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Error)
                Return
            End If
        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try
    End Sub

    Private Sub ChkUpdate4_CheckedChanged(sender As Object, e As EventArgs) Handles ChkUpdate4.CheckedChanged
        Try
            If TxtRegType.Text = 0 Then
                If ChkUpdate4.Checked = True Then
                    CmdFindDiplom.Enabled = True
                    OLDDIPLOMNAME = TxtDiplomName.Text
                    TxtDiplomName.Text = Nothing
                    TxtNewDiplomGuid.Text = Nothing
                Else
                    CmdFindDiplom.Enabled = False
                    TxtDiplomName.Text = OLDDIPLOMNAME
                    TxtNewDiplomGuid.Text = "00000000-0000-0000-0000-000000000000"
                End If
            Else
                ChkUpdate4.Checked = False
            End If
        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try
    End Sub

    Private Sub ChkUpdate5_CheckedChanged(sender As Object, e As EventArgs) Handles ChkUpdate5.CheckedChanged
        Try
            If TxtRegType.Text = 0 Then
                If ChkUpdate5.Checked = True Then
                    CmdFindBatch.Enabled = True
                    OLDBATCH = TxtBatchName.Text
                    TxtBatchName.Text = Nothing
                Else
                    CmdFindBatch.Enabled = False
                    TxtBatchName.Text = OLDBATCH
                    TxtNewBatchGuid.Text = "00000000-0000-0000-0000-000000000000"
                End If
            Else
                ChkUpdate5.Checked = False
            End If
        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try
    End Sub

    Private Sub TransferStudentCourse_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Try
            RegDocPosted.WHATFORMTYPEGUID("studenttrans")
            TxtTypeGuid.Text = RegDocPosted.TYPEGUID
            If TxtRegType.Text = 1 Then
                TxtOldDiplomGuid.Text = "00000000-0000-0000-0000-000000000000"
                TxtNewDiplomGuid.Text = "00000000-0000-0000-0000-000000000000"
                TxtOldBatchGuid.Text = "00000000-0000-0000-0000-000000000000"
                TxtNewBatchGuid.Text = "00000000-0000-0000-0000-000000000000"
            ElseIf TxtRegType.Text = 0 Then
                TxtCourseGuid.Text = "00000000-0000-0000-0000-000000000000"
                TxtnewCourseGuid.Text = "00000000-0000-0000-0000-000000000000"
            End If
        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try
    End Sub
    Function VALIDATE_DATA() As Boolean
        If ChkUpdate1.Checked = True And TxtNewBranch1Guid.Text = Nothing Then
            MessageBox.Show("برجاء اختيار فرع التسجيل الجديد", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return False
        ElseIf ChkUpdate2.Checked = True And TxtNewBranch2Guid.Text = Nothing Then
            MessageBox.Show("برجاء اختيار فرع الدراسة الجديد", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return False
        ElseIf ChkUpdate3.Checked = True And TxtnewCourseGuid.Text = Nothing Then
            MessageBox.Show("برجاء اختيار الدورة المراد تسجيل الطالب عليها", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return False
        ElseIf ChkUpdate4.Checked = True And TxtNewDiplomGuid.Text = Nothing Then
            MessageBox.Show("برجاء اختيار الدبلوم المراد تسجيل الطالب عليه", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return False
        ElseIf ChkUpdate5.Checked = True And TxtNewBatchGuid.Text = Nothing Then
            MessageBox.Show("برجاء اختيار الدفعه المراد تسجيل الطالب عليه", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return False
        End If
        Return True
    End Function
    Sub ADDSTUDENTTRANS()
        Dim connection = dbcontext.Database.Connection
        Dim comm = New SqlCommand("AddStudentTransAction", connection)
        comm.CommandType = CommandType.StoredProcedure
        comm.Parameters.AddWithValue("@ActionDate", Convert.ToDateTime(DateTime.Now))
        comm.Parameters.AddWithValue("@OldRegBranchGuid", TxtBranch1Guid.Text)
        comm.Parameters.AddWithValue("@NewRegBranchGuid", TxtNewBranch1Guid.Text)
        comm.Parameters.AddWithValue("@OldStudyBranchGuid", TxtBranch2Guid.Text)
        comm.Parameters.AddWithValue("@NewStudyBranchGuid", TxtNewBranch2Guid.Text)
        comm.Parameters.AddWithValue("@OldCourseGuid", TxtCourseGuid.Text)
        comm.Parameters.AddWithValue("@NewCourseGuid", TxtnewCourseGuid.Text)
        If TxtRegType.Text = 0 Then
            comm.Parameters.AddWithValue("@OldDiplomGuid", TxtOldDiplomGuid.Text)
            comm.Parameters.AddWithValue("@NewDiplomGuid", TxtNewDiplomGuid.Text)
        ElseIf TxtRegType.Text = 1 Then
            comm.Parameters.AddWithValue("@OldDiplomGuid", TxtOldCourseDiplom.Text)
            comm.Parameters.AddWithValue("@NewDiplomGuid", TxtCourseDiplomGuid.Text)
        End If
        comm.Parameters.AddWithValue("@OldBatchGuid", TxtOldBatchGuid.Text)
        comm.Parameters.AddWithValue("@NewBatchGuid", TxtNewBatchGuid.Text)
        comm.Parameters.AddWithValue("@UserGuid", AllFiles.USER_GUID____)
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
        comm.Parameters.AddWithValue("@ActionGuid", TxtAccountGuid.Text)
        comm.Parameters.AddWithValue("@UserGuid", AllFiles.USER_GUID____)
        comm.Parameters.AddWithValue("@Date_", Convert.ToDateTime(DateTime.Now))
        comm.Parameters.AddWithValue("@Time_", Convert.ToDateTime(DateTime.Now.ToShortTimeString))
        comm.Parameters.AddWithValue("@Notes_", AllFiles.ActionType___)
        comm.Parameters.AddWithValue("@ActionTypeGuid", TxtTypeGuid.Text)
        comm.Parameters.AddWithValue("@ActionReason", REASON)
        comm.Parameters.AddWithValue("@ActionCode", "")
        If connection.State = ConnectionState.Open Then
            connection.Close()
        End If
        connection.Open()
        comm.ExecuteNonQuery()
        comm.Parameters.Clear()
    End Sub



    Private Sub CmdFindBranch1_Click(sender As Object, e As EventArgs) Handles CmdFindBranch1.Click
        Try
            Dim w As New NewBranchList
            w.TxtType.Text = 0
            w.TxtIsUse.Text = True
            w.ShowDialog()
            If w.IS_Yes = True Then
                Dim ii As Integer = w.BranchGrid.CurrentRow.Index
                TxtBranch1Name.Text = w.BranchGrid.Rows(ii).Cells(1).Value.ToString()
                TxtNewBranch1Guid.Text = w.BranchGrid.Rows(ii).Cells(3).Value.ToString()
            End If
        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try
    End Sub
    Sub CHANGEREGBRANCH()
        Dim connection = dbcontext.Database.Connection
        Dim comm = New SqlCommand("ChangeRegBranch", connection)
        comm.CommandType = CommandType.StoredProcedure
        comm.Parameters.AddWithValue("@AccountGuid", TxtAccountGuid.Text)
        comm.Parameters.AddWithValue("@BarnchGuid", TxtNewBranch1Guid.Text)
        If connection.State = ConnectionState.Open Then
            connection.Close()
        End If
        connection.Open()
        comm.ExecuteNonQuery()
        comm.Parameters.Clear()
    End Sub
    Sub CHANGESTUDENTLEVEL()
        Dim connection = dbcontext.Database.Connection
        Dim comm = New SqlCommand("ChangeStudentLevelInfo", connection)
        comm.CommandType = CommandType.StoredProcedure
        comm.Parameters.AddWithValue("@StudentGuid", TxtAccountGuid.Text)
        comm.Parameters.AddWithValue("@OldBranchGuid", TxtBranch2Guid.Text)
        comm.Parameters.AddWithValue("@OldCourseGuid", TxtCourseGuid.Text)
        comm.Parameters.AddWithValue("@BranchGuid", TxtNewBranch2Guid.Text)
        comm.Parameters.AddWithValue("@DiplomGuid", TxtCourseDiplomGuid.Text)
        comm.Parameters.AddWithValue("@CourseGuid", TxtnewCourseGuid.Text)
        comm.Parameters.AddWithValue("@BatchGuid", TxtNewBatchGuid.Text)
        If connection.State = ConnectionState.Open Then
            connection.Close()
        End If
        connection.Open()
        comm.ExecuteNonQuery()
        comm.Parameters.Clear()
    End Sub
    Sub CHANGESTUDENTLEVEL_DIPLOM()
        Dim connection = dbcontext.Database.Connection
        Dim comm = New SqlCommand("ChangeStudentLevelDiplom", connection)
        comm.CommandType = CommandType.StoredProcedure
        comm.Parameters.AddWithValue("@StudentGuid", TxtAccountGuid.Text)
        comm.Parameters.AddWithValue("@OldBranchGuid", TxtBranch2Guid.Text)
        comm.Parameters.AddWithValue("@OldDiplomGuid", TxtOldDiplomGuid.Text)
        comm.Parameters.AddWithValue("@OldBatchGuidGuid", TxtOldBatchGuid.Text)
        If ChkUpdate2.Checked = True Then
            comm.Parameters.AddWithValue("@BranchGuid", TxtNewBranch2Guid.Text)
        Else
            comm.Parameters.AddWithValue("@BranchGuid", TxtBranch2Guid.Text)
        End If
        If ChkUpdate4.Checked = True Then
            comm.Parameters.AddWithValue("@DiplomGuid", TxtNewDiplomGuid.Text)
        Else
            comm.Parameters.AddWithValue("@DiplomGuid", TxtOldDiplomGuid.Text)
        End If
        If ChkUpdate5.Checked = True Then
            comm.Parameters.AddWithValue("@BatchGuid", TxtNewBatchGuid.Text)
        Else
            comm.Parameters.AddWithValue("@BatchGuid", TxtOldBatchGuid.Text)
        End If
        If connection.State = ConnectionState.Open Then
            connection.Close()
        End If
        connection.Open()
        comm.ExecuteNonQuery()
        comm.Parameters.Clear()
    End Sub
    Sub FORSAVE()
        If AllFiles.FORSAVE("studenttrans") = False Then
            Return
        Else
            AllFiles.ActionType___ = "نقل طالب"
            ADDSTUDENTTRANS()
            If ChkUpdate1.Checked = True Then
                CHANGEREGBRANCH()
            End If
            If TxtRegType.Text = 1 Then
                If ChkUpdate2.Checked = True Or ChkUpdate3.Checked = True Then
                    CHANGESTUDENTLEVEL()
                End If
            End If
            If TxtRegType.Text = 0 Then
                If ChkUpdate2.Checked = True Or ChkUpdate4.Checked = True Or ChkUpdate5.Checked = True Then
                    CHANGESTUDENTLEVEL_DIPLOM()
                End If
            End If
            ADD_USERACTION_()
            MessageBox.Show("تم نقل الطالب بنجاح", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Information)
        End If
    End Sub
    Private Sub CmdSave_Click(sender As Object, e As EventArgs) Handles CmdSave.Click
        Try
            If VALIDATE_DATA() = False Then
                Return
            Else
                FORSAVE()
            End If
        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try
    End Sub

    Private Sub CmdFindDiplom_Click_1(sender As Object, e As EventArgs) Handles CmdFindDiplom.Click
        Try
            Dim w As New DiplomList
            w.TxtIsUse.Text = True
            w.TxtType.Text = 0
            w.ShowDialog()
            If w.IS_Yes = True Then
                Dim ii As Integer = w.BranchGrid.CurrentRow.Index
                TxtDiplomName.Text = w.BranchGrid.Rows(ii).Cells(1).Value.ToString()
                TxtNewDiplomGuid.Text = w.BranchGrid.Rows(ii).Cells(3).Value.ToString()
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
                TxtNewBatchGuid.Text = w.BranchGrid.Rows(ii).Cells(3).Value.ToString()
            End If
        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try
    End Sub
End Class