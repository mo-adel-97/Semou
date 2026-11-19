Imports System.Data.SqlClient
Imports DevExpress.XtraReports.UI

Public Class StudentCourseList
    Dim dbcontext As New SemouEntities
    Dim NEWREGBRANCH As String
    Dim NEWSTUDYBRANCH As String
    Sub LOADSTUDENT()
        StudentGrid.Rows.Clear()
        Dim connection = dbcontext.Database.Connection
        Dim comm As New SqlCommand("ShowStudentCourse", connection)
        If TxtCourseGuid.Text <> Nothing Then
            comm.Parameters.AddWithValue("@CourseGuid", TxtCourseGuid.Text)
            StudentGrid.Columns(6).Visible = False
            StudentGrid.Columns(7).Visible = False
        Else
            StudentGrid.Columns(6).Visible = True
            StudentGrid.Columns(7).Visible = True
            TxtFromDate.Text = Nothing
            TxtToDate.Text = Nothing
        End If
        comm.Parameters.AddWithValue("@BranchGuid", TxtBranchGuid.Text)
        If TxtCaseGuid.Text <> Nothing Then
            comm.Parameters.AddWithValue("@CauseGuid", TxtCaseGuid.Text)
            StudentGrid.Columns(4).Visible = False
        Else
            StudentGrid.Columns(4).Visible = True
        End If
        If TxtFind2.Text <> Nothing Then
            comm.Parameters.AddWithValue("@StudentName", TxtFind2.Text)
        End If
        comm.CommandType = CommandType.StoredProcedure
        If connection.State = ConnectionState.Open Then
            connection.Close()
        End If
        connection.Open()
        comm.ExecuteNonQuery()
        Dim adp As New SqlDataAdapter(comm)
        Dim ds As DataSet = New DataSet()
        adp.Fill(ds, "ShowStudentCourse")
        Dim COUNT__ As Integer = 0
        If ds.Tables(0).Rows.Count > 0 Then
            StudentGrid.RowCount = ds.Tables(0).Rows.Count
            TxtCount.Text = StudentGrid.Rows.Count()
            For i As Integer = 0 To ds.Tables(0).Rows.Count - 1
                StudentGrid.Rows(COUNT__).Cells(0).Value = ds.Tables(0).Rows(i)(0).ToString()
                StudentGrid.Rows(COUNT__).Cells(1).Value = ds.Tables(0).Rows(i)(1).ToString()
                StudentGrid.Rows(COUNT__).Cells(2).Value = ds.Tables(0).Rows(i)(2).ToString()
                StudentGrid.Rows(COUNT__).Cells(3).Value = ds.Tables(0).Rows(i)(3).ToString()
                StudentGrid.Rows(COUNT__).Cells(4).Value = ds.Tables(0).Rows(i)(4).ToString()
                StudentGrid.Rows(COUNT__).Cells(5).Value = ds.Tables(0).Rows(i)(8).ToString()
                StudentGrid.Rows(COUNT__).Cells(6).Value = ds.Tables(0).Rows(i)(9).ToString()
                StudentGrid.Rows(COUNT__).Cells(10).Value = ds.Tables(0).Rows(i)(5).ToString()
                StudentGrid.Rows(COUNT__).Cells(11).Value = ds.Tables(0).Rows(i)(6).ToString()
                StudentGrid.Rows(COUNT__).Cells(12).Value = ds.Tables(0).Rows(i)(7).ToString()
                StudentGrid.Rows(COUNT__).Cells(15).Value = ds.Tables(0).Rows(i)(10).ToString()
                StudentGrid.Rows(COUNT__).Cells(7).Value = ds.Tables(0).Rows(i)(11).ToString()
                StudentGrid.Rows(COUNT__).Cells(17).Value = ds.Tables(0).Rows(i)(12).ToString()
                StudentGrid.Rows(COUNT__).Cells(18).Value = ds.Tables(0).Rows(i)(13).ToString()
                COUNT__ = COUNT__ + 1
            Next
        Else
            TxtCount.Text = 0
        End If
    End Sub
    Private Sub CmdExit_Click(sender As Object, e As EventArgs) Handles CmdExit.Click
        Me.Close()
    End Sub

    Private Sub CmdDelCause_Click(sender As Object, e As EventArgs) Handles CmdDelCause.Click
        Try
            TxtCaseName.Text = Nothing
            TxtCaseGuid.Text = Nothing
            If TxtBranchGuid.Text = Nothing Then
                MessageBox.Show("برجاء اختيار فرع اولا", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Error)
                Return
            Else
                'If TxtCourseGuid.Text = Nothing Then
                '    MessageBox.Show("برجاء اختيار الدورة اولا", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Error)
                '    Return
                'Else
                LOADSTUDENT()
                'End If
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
            If TxtCaseGuid.Text <> Nothing Then
                If TxtBranchGuid.Text = Nothing Then
                    MessageBox.Show("برجاء اختيار فرع اولا", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Error)
                    Return
                Else
                    'If TxtCourseGuid.Text = Nothing Then
                    '    MessageBox.Show("برجاء اختيار الدورة اولا", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Error)
                    '    Return
                    'Else
                    LOADSTUDENT()
                    'End If
                End If
            End If
        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try
    End Sub

    Private Sub CmdFindDepart_Click(sender As Object, e As EventArgs) Handles CmdFindDepart.Click
        Try
            AllFiles.CHEKDEFBRANCHINFO(AllFiles.USER_GUID____.ToString)
            Dim w As New NewBranchList
            If AllFiles.DEFBRANCH = True Then
                w.TxtType.Text = 1
                w.TxtUserGuid.Text = AllFiles.USER_GUID____.ToString
            Else
                w.TxtType.Text = 0
                w.TxtIsUse.Text = True
            End If
            w.ShowDialog()
            If w.IS_Yes = True Then
                Dim ii As Integer = w.BranchGrid.CurrentRow.Index
                TxtBranchName.Text = w.BranchGrid.Rows(ii).Cells(1).Value.ToString()
                TxtBranchGuid.Text = w.BranchGrid.Rows(ii).Cells(3).Value.ToString()
            End If
            If TxtBranchGuid.Text <> Nothing Then
                LOADSTUDENT()
            End If
        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try
    End Sub

    Private Sub CmdFindDiplom_Click(sender As Object, e As EventArgs) Handles CmdFindDiplom.Click
        Try
            If TxtBranchGuid.Text <> Nothing Then
                Dim w As New CourseList
                w.TxtBranchGuid.Text = TxtBranchGuid.Text
                w.TxtIsUse.Text = True
                w.ShowDialog()
                If w.IS_Yes = True Then
                    Dim ii As Integer = w.BranchGrid.CurrentRow.Index
                    TxtCourseName.Text = w.BranchGrid.Rows(ii).Cells(3).Value.ToString()
                    TxtCourseGuid.Text = w.BranchGrid.Rows(ii).Cells(5).Value.ToString()
                    TxtFromDate.Text = w.BranchGrid.Rows(ii).Cells(1).Value.ToString()
                    TxtToDate.Text = w.BranchGrid.Rows(ii).Cells(2).Value.ToString()
                End If
                If TxtCourseGuid.Text <> Nothing Then
                    If TxtBranchGuid.Text <> Nothing Then
                        LOADSTUDENT()
                    End If
                End If
            Else
                MessageBox.Show("برجاء اختيار فرغ", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Error)
                Return
            End If
        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try
    End Sub

    Private Sub CmdView_Click(sender As Object, e As EventArgs) Handles CmdView.Click
        Try
            If TxtBranchGuid.Text = Nothing Then
                MessageBox.Show("برجاء اختيار فرع اولا", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Error)
                Return
            Else
                'If TxtCourseGuid.Text = Nothing Then
                '    MessageBox.Show("برجاء اختيار الدورة اولا", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Error)
                '    Return
                'Else
                LOADSTUDENT()
                'End If
            End If
        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try
    End Sub

    Private Sub StudentGrid_CellContentClick(sender As Object, e As DataGridViewCellEventArgs) Handles StudentGrid.CellContentClick

    End Sub

    Private Sub StudentGrid_CellClick(sender As Object, e As DataGridViewCellEventArgs) Handles StudentGrid.CellClick
        Try
            Dim i As Integer = StudentGrid.CurrentCell.RowIndex
            Dim j As Integer = StudentGrid.CurrentCell.ColumnIndex
            If j = 7 Then
                Dim w As New EncodingNewPayInvoice
                w.TxtStudentName.Text = StudentGrid.Rows(i).Cells(1).Value.ToString
                w.TxtNationalId.Text = StudentGrid.Rows(i).Cells(2).Value.ToString
                w.TxtTel.Text = StudentGrid.Rows(i).Cells(3).Value.ToString
                w.TxtAccountGuid.Text = StudentGrid.Rows(i).Cells(11).Value.ToString
                w.TxtRegDocGuid.Text = StudentGrid.Rows(i).Cells(10).Value.ToString
                If StudentGrid.Rows(i).Cells(10).Value <> "00000000-0000-0000-0000-000000000000" Then
                    w.SHOWREGDOCDIPLOM(0)
                Else
                    w.SHOWDIPLOMFROMSTUDENTLEVEL(StudentGrid.Rows(i).Cells(12).Value.ToString)
                End If
                w.ShowDialog()
            ElseIf j = 8 Then
                Dim w As New EncodingNewPayInvoice
                w.TxtStudentName.Text = StudentGrid.Rows(i).Cells(1).Value.ToString
                w.TxtNationalId.Text = StudentGrid.Rows(i).Cells(2).Value.ToString
                w.TxtTel.Text = StudentGrid.Rows(i).Cells(3).Value.ToString
                w.TxtAccountGuid.Text = StudentGrid.Rows(i).Cells(11).Value.ToString
                w.TxtRegDocGuid.Text = StudentGrid.Rows(i).Cells(10).Value.ToString
                w.SHOWREGDOCDIPLOM(1)
                w.ShowDialog()
            ElseIf j = 9 Then
                If AllFiles.FORFIND("studentstatment") = False Then
                    Return
                Else
                    Dim w As New StusdentStatment
                    w.TxtId.Text = StudentGrid.Rows(i).Cells(0).Value.ToString
                    w.TxtStudentName.Text = StudentGrid.Rows(i).Cells(1).Value.ToString
                    w.TxtAccountGuid.Text = StudentGrid.Rows(i).Cells(11).Value.ToString
                    w.TxtNationalId.Text = StudentGrid.Rows(i).Cells(2).Value.ToString
                    w.ShowDialog()
                End If
            ElseIf j = 13 Then
                Dim w As New AddCourseStudentNotes
                w.TxtBranchGuid.Text = TxtBranchGuid.Text
                If TxtCourseGuid.Text <> Nothing Then
                    w.TxtCourseGuid.Text = TxtCourseGuid.Text
                Else
                    w.TxtCourseGuid.Text = StudentGrid.Rows(i).Cells(15).Value.ToString
                End If
                w.TxtNotes.Text = StudentGrid.Rows(i).Cells(5).Value.ToString
                w.TxtAccountGuid.Text = StudentGrid.Rows(i).Cells(11).Value.ToString
                w.ShowDialog()
                If w.IS_Yes = True Then
                    LOADSTUDENT()
                End If
            ElseIf j = 14 Then
                    Dim w As New TransferStudentCourse
                    w.TxtRegType.Text = 1
                    w.TxtBranch1Name.Text = StudentGrid.Rows(i).Cells(17).Value.ToString
                    w.TxtBranch1Guid.Text = StudentGrid.Rows(i).Cells(18).Value.ToString
                    w.TxtBranch2Name.Text = TxtBranchName.Text
                    w.TxtBranch2Guid.Text = TxtBranchGuid.Text
                    w.TxtCourseName.Text = StudentGrid.Rows(i).Cells(6).Value.ToString
                    w.TxtCourseGuid.Text = StudentGrid.Rows(i).Cells(15).Value.ToString
                    w.TxtFromDate.Text = StudentGrid.Rows(i).Cells(7).Value.ToString
                    w.TxtAccountGuid.Text = StudentGrid.Rows(i).Cells(11).Value.ToString
                    w.TxtOldCourseDiplom.Text = StudentGrid.Rows(i).Cells(12).Value.ToString
                    w.ShowDialog()
                ElseIf j = 16 Then
                    If AllFiles.FORUPDATE("addstudent") = False Then
                    Return
                Else
                    Dim w As New ChangeStaut
                    w.TxtRegType.Text = 1
                    w.TxtStudentName.Text = StudentGrid.Rows(i).Cells(1).Value.ToString
                    w.TxtnationalId.Text = StudentGrid.Rows(i).Cells(2).Value.ToString
                    w.TxtGuid.Text = StudentGrid.Rows(i).Cells(11).Value.ToString
                    w.TxtCourseGuid.Text = StudentGrid.Rows(i).Cells(15).Value.ToString
                    w.ShowDialog()
                    If w.IS_Yes = True Then
                        LOADSTUDENT()
                    End If
                End If
            End If
        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try
    End Sub

    Private Sub StudentCourseList_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Try
            'AllFiles.CHEKDEFBRANCHINFO(AllFiles.USER_GUID____.ToString)
            'If AllFiles.DEFBRANCH = True Then
            '    CmdFindDepart.Enabled = False
            '    TxtBranchName.Text = AllFiles.DEFBRANCHNAME
            '    TxtBranchGuid.Text = AllFiles.DEFBRANCHGUID.ToString()
            'Else
            '    CmdFindDepart.Enabled = True
            '    TxtBranchName.Text = Nothing
            '    TxtBranchGuid.Text = Nothing
            'End If
        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try
    End Sub
    Sub PRINT()
        Dim connection = dbcontext.Database.Connection
        Dim comm As New SqlCommand("ShowStudentCourse", connection)
        comm.Parameters.AddWithValue("@CourseGuid", TxtCourseGuid.Text)
        comm.Parameters.AddWithValue("@BranchGuid", TxtBranchGuid.Text)
        If TxtCaseGuid.Text <> Nothing Then
            comm.Parameters.AddWithValue("@CauseGuid", TxtCaseGuid.Text)
        End If
        comm.CommandType = CommandType.StoredProcedure
        If connection.State = ConnectionState.Open Then
            connection.Close()
        End If
        connection.Open()
        comm.ExecuteNonQuery()
        Dim adp As New SqlDataAdapter(comm)
        Dim ds As DataSet = New DataSet()
        adp.Fill(ds, "ShowStudentCourse")
        Dim dt = ds.Tables(0)
        Dim report As New StudentCourseReport
        report.DataSource = dt
        report.DataAdapter = adp
        report.DataMember = "DataTable1"
        report.RequestParameters = False
        report.Parameters(0).Value = TxtBranchName.Text
        report.Parameters(1).Value = TxtCourseName.Text
        If TxtCaseName.Text <> Nothing Then
            report.Parameters(2).Value = TxtCaseName.Text
        Else
            report.Parameters(2).Value = "الكل"
        End If
        report.Parameters(3).Value = TxtFromDate.Text
        report.Parameters(4).Value = TxtToDate.Text
        Dim Tool_Report As ReportPrintTool = New ReportPrintTool(report)
        report.CreateDocument()
        Dim frm As New ViewReport
        frm.DocumentViewer1.DocumentSource = report
        frm.ShowDialog()
    End Sub
    Private Sub CmdPrint_Click(sender As Object, e As EventArgs) Handles CmdPrint.Click
        Try
            If TxtBranchGuid.Text <> Nothing And TxtCourseGuid.Text <> Nothing Then
                PRINT()
            End If
        Catch ex As Exception

        End Try
    End Sub

    Private Sub TxtFind2_TextChanged(sender As Object, e As EventArgs) Handles TxtFind2.TextChanged
        Try
            If TxtBranchGuid.Text = Nothing Then
                MessageBox.Show("برجاء اختيار فرع اولا", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Error)
                Return
            Else
                'If TxtCourseGuid.Text = Nothing Then
                '    MessageBox.Show("برجاء اختيار الدورة اولا", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Error)
                '    Return
                'Else
                LOADSTUDENT()
                'End If
            End If
        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try
    End Sub

    Private Sub CmdDelCourse_Click(sender As Object, e As EventArgs) Handles CmdDelCourse.Click
        Try
            TxtCourseGuid.Text = Nothing
            TxtCourseName.Text = Nothing
            If TxtBranchGuid.Text <> Nothing Then
                LOADSTUDENT()
            End If
        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try
    End Sub

    Private Sub ChkAll_CheckedChanged(sender As Object, e As EventArgs) Handles ChkAll.CheckedChanged
        Try
            For Each row As DataGridViewRow In StudentGrid.Rows
                If row.Cells(11).Value <> Nothing Then
                    If ChkAll.Checked = True Then
                        row.Cells(19).Value = True
                    Else
                        row.Cells(19).Value = False
                    End If
                End If
            Next
        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try
    End Sub
    Sub CHANGEREGBRANCH()
        Dim connection = dbcontext.Database.Connection
        Dim comm = New SqlCommand("ChangeRegBranch", connection)
        comm.CommandType = CommandType.StoredProcedure
        For i As Integer = 0 To StudentGrid.Rows.Count - 1
            If StudentGrid.Rows(i).Cells(11).Value.ToString() = Nothing Then
                Exit For
            Else
                If StudentGrid.Rows(i).Cells(19).Value = True Then
                    comm.Parameters.AddWithValue("@AccountGuid", StudentGrid.Rows(i).Cells(11).Value.ToString())
                    comm.Parameters.AddWithValue("@BarnchGuid", NEWREGBRANCH)
                    If connection.State = ConnectionState.Open Then
                        connection.Close()
                    End If
                    connection.Open()
                    Dim rs As Integer = comm.ExecuteNonQuery()
                    If rs > 0 Then
                        comm.Parameters.Clear()
                    End If
                End If
            End If
        Next
    End Sub
    Sub CHANGESTUDYBRANCH()
        Dim connection = dbcontext.Database.Connection
        Dim comm = New SqlCommand("ChangeAllStudentLevelDiplom", connection)
        comm.CommandType = CommandType.StoredProcedure
        For i As Integer = 0 To StudentGrid.Rows.Count - 1
            If StudentGrid.Rows(i).Cells(11).Value.ToString() = Nothing Then
                Exit For
            Else
                If StudentGrid.Rows(i).Cells(19).Value = True Then
                    comm.Parameters.AddWithValue("@StudentGuid", StudentGrid.Rows(i).Cells(11).Value.ToString())
                    comm.Parameters.AddWithValue("@BranchGuid", NEWSTUDYBRANCH)
                    comm.Parameters.AddWithValue("@DiplomGuid", TxtCourseGuid.Text)
                    comm.Parameters.AddWithValue("@OldBranchGuid", TxtBranchGuid.Text)
                    If connection.State = ConnectionState.Open Then
                        connection.Close()
                    End If
                    connection.Open()
                    Dim rs As Integer = comm.ExecuteNonQuery()
                    If rs > 0 Then
                        comm.Parameters.Clear()
                    End If
                End If
            End If
        Next
    End Sub
    Private Sub CmdTransAll_Click(sender As Object, e As EventArgs) Handles CmdTransAll.Click
        Try
            Dim w As New TransAllDiplomStudent
            w.ShowDialog()
            If w.IS_Yes = True Then
                If w.ChkUpdate1.Checked = True Then
                    NEWREGBRANCH = w.TxtBranch1Guid.Text
                    CHANGEREGBRANCH()
                End If
                If w.ChkUpdate2.Checked = True Then
                    NEWSTUDYBRANCH = w.TxtBranch2Guid.Text
                    CHANGESTUDYBRANCH()
                End If
                MessageBox.Show("تم نقل المتدرب/المتدربين بنجاح", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Information)
            End If
        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try
    End Sub
End Class