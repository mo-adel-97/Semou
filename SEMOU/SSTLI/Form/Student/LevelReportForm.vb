Imports System.Data.SqlClient
Imports CrystalDecisions.CrystalReports.Engine
Imports CrystalDecisions.Shared
Imports DevExpress.XtraReports.UI
Imports Microsoft.Office.Interop

Public Class LevelReportForm
    Dim dbcontext As New SemouEntities
    Dim NEWREGBRANCH As String
    Dim NEWSTUDYBRANCH As String
    Private Sub CmdExit_Click(sender As Object, e As EventArgs) Handles CmdExit.Click
        Me.Close()
    End Sub

    Private Sub TxtDiplomGuid_Click(sender As Object, e As EventArgs) Handles TxtDiplomGuid.Click

    End Sub

    Private Sub CmdFindDiplom_Click(sender As Object, e As EventArgs) Handles CmdFindDiplom.Click
        Try
            Dim w As New DiplomList
            w.TxtIsUse.Text = True
            w.TxtType.Text = 0
            w.ShowDialog()
            If w.IS_Yes = True Then
                Dim ii As Integer = w.BranchGrid.CurrentRow.Index
                TxtDiplomName.Text = w.BranchGrid.Rows(ii).Cells(1).Value.ToString()
                TxtDiplomGuid.Text = w.BranchGrid.Rows(ii).Cells(3).Value.ToString()
            End If
            If TxtBranchGuid.Text <> Nothing Then
                If TxtDiplomGuid.Text <> Nothing Then
                    If TxtCaseGuid.Text <> Nothing Then
                        VIEWSTUDENTINLEVEL()
                    End If
                End If
            End If
        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try
    End Sub

    Private Sub TxtDiplomName_TextChanged(sender As Object, e As EventArgs) Handles TxtDiplomName.TextChanged

    End Sub

    Private Sub Label1_Click(sender As Object, e As EventArgs) Handles Label1.Click

    End Sub

    Private Sub Label4_Click(sender As Object, e As EventArgs) Handles Label4.Click

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
                If TxtDiplomGuid.Text <> Nothing Then
                    If TxtCaseGuid.Text <> Nothing Then
                        VIEWSTUDENTINLEVEL()
                    End If
                End If
            End If
        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try
    End Sub

    Private Sub TxtBranchGuid_Click(sender As Object, e As EventArgs) Handles TxtBranchGuid.Click

    End Sub

    Private Sub TxtBranchName_TextChanged(sender As Object, e As EventArgs) Handles TxtBranchName.TextChanged

    End Sub
    Sub VIEWSTUDENTINLEVEL()
        StudentGrid.Rows.Clear()
        Dim connection = dbcontext.Database.Connection
        Dim comm As New SqlCommand("ShowStudentByDiplomAndLevel", connection)
        comm.Parameters.AddWithValue("@DiplomGuid", TxtDiplomGuid.Text)
        comm.Parameters.AddWithValue("@BranchGuid", TxtBranchGuid.Text)
        comm.Parameters.AddWithValue("@CaseGuid", TxtCaseGuid.Text)
        If TxtBatchGuid.Text <> Nothing Then
            comm.Parameters.AddWithValue("@BatchGuid", TxtBatchGuid.Text)
        End If
        If TxtLevelGuid.Text <> Nothing Then
            comm.Parameters.AddWithValue("@LevelGuid", TxtLevelGuid.Text)
        End If
        If TxtFind2.Text <> Nothing Then
            comm.Parameters.AddWithValue("@NationalId", TxtFind2.Text)
        End If
        comm.CommandType = CommandType.StoredProcedure
        If connection.State = ConnectionState.Open Then
            connection.Close()
        End If
        connection.Open()
        comm.ExecuteNonQuery()
        Dim adp As New SqlDataAdapter(comm)
        Dim ds As DataSet = New DataSet()
        adp.Fill(ds, "ShowStudentByDiplomAndLevel")
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
                StudentGrid.Rows(COUNT__).Cells(5).Value = ds.Tables(0).Rows(i)(5).ToString()
                StudentGrid.Rows(COUNT__).Cells(12).Value = ds.Tables(0).Rows(i)(6).ToString()
                StudentGrid.Rows(COUNT__).Cells(13).Value = ds.Tables(0).Rows(i)(7).ToString()
                StudentGrid.Rows(COUNT__).Cells(14).Value = ds.Tables(0).Rows(i)(8).ToString()
                StudentGrid.Rows(COUNT__).Cells(15).Value = ds.Tables(0).Rows(i)(9).ToString()
                StudentGrid.Rows(COUNT__).Cells(16).Value = ds.Tables(0).Rows(i)(10).ToString()
                COUNT__ = COUNT__ + 1
            Next
        Else
            TxtCount.Text = 0
        End If
    End Sub
    Private Sub CmdView_Click(sender As Object, e As EventArgs) Handles CmdView.Click
        Try
            If TxtBranchGuid.Text = Nothing Then
                MessageBox.Show("برجاء اختيار الفرع", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Error)
                Return
            Else
                If TxtDiplomGuid.Text = Nothing Then
                    MessageBox.Show("برجاء اختيار الدبلوم - الدورة", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Error)
                    Return
                Else
                    If TxtCaseGuid.Text = Nothing Then
                        MessageBox.Show("برجاء اختيار الحالة", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Error)
                        Return
                    Else
                        VIEWSTUDENTINLEVEL()
                    End If
                End If
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
            If TxtBranchGuid.Text <> Nothing Then
                If TxtDiplomGuid.Text <> Nothing Then
                    If TxtCaseGuid.Text <> Nothing Then
                        VIEWSTUDENTINLEVEL()
                    End If
                End If
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
            If TxtBranchGuid.Text <> Nothing Then
                If TxtDiplomGuid.Text <> Nothing Then
                    If TxtCaseGuid.Text <> Nothing Then
                        VIEWSTUDENTINLEVEL()
                    End If
                End If
            End If
        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try
    End Sub

    Private Sub CmdRefresh_Click(sender As Object, e As EventArgs) Handles CmdRefresh.Click
        Try
            TxtBatchGuid.Text = Nothing
            TxtBatchName.Text = Nothing
            TxtLevelGuid.Text = Nothing
            TxtLevelName.Text = Nothing
            TxtDiplomGuid.Text = Nothing
            TxtDiplomName.Text = Nothing
            TxtBranchGuid.Text = Nothing
            TxtBranchName.Text = Nothing
            TxtFind2.Text = Nothing
            StudentGrid.Rows.Clear()
        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try
    End Sub
    Private Sub ApplyNewServer(ByVal report As ReportDocument)

        For Each subReport As ReportDocument In report.Subreports
            For Each crTable As Table In subReport.Database.Tables
                Dim loi As TableLogOnInfo = crTable.LogOnInfo
                loi.ConnectionInfo.ServerName = ServerConfig.SERVERNAME
                loi.ConnectionInfo.UserID = ServerConfig.USERNAME
                loi.ConnectionInfo.Password = ServerConfig.USERPASSWORD
                crTable.ApplyLogOnInfo(loi)
            Next
        Next
        For Each crTable As Table In report.Database.Tables
            Dim loi As TableLogOnInfo = crTable.LogOnInfo
            loi.ConnectionInfo.ServerName = ServerConfig.SERVERNAME
            loi.ConnectionInfo.UserID = ServerConfig.USERNAME
            loi.ConnectionInfo.Password = ServerConfig.USERPASSWORD
            crTable.ApplyLogOnInfo(loi)
        Next
    End Sub
    Sub PRINTEXAM()
        Dim reportview As New ReportView
        Dim report As New ExamDoc
        Dim connection = dbcontext.Database.Connection
        Dim comm As New SqlCommand("PrintExam", connection)
        comm.CommandType = CommandType.StoredProcedure
        comm.Parameters.AddWithValue("@ExamGuid", TxtExamGuid.Text)
        If connection.State = ConnectionState.Open Then
            connection.Close()
        End If
        connection.Open()
        comm.ExecuteNonQuery()
        Dim adp As New SqlDataAdapter(comm)
        Dim ds As DataSet = New DataSet()
        adp.Fill(ds, "PrintExam")
        report.SetDataSource(ds.Tables(0))
        reportview.CrystalReportViewer1.ReportSource = report
        ApplyNewServer(reportview.CrystalReportViewer1.ReportSource)
        reportview.CrystalReportViewer1.Refresh()
        reportview.Text = "أختبار مستوي"
        reportview.ShowDialog()
    End Sub
    Private Sub CmdPrintExam_Click(sender As Object, e As EventArgs) Handles CmdPrintExam.Click
        Try
            If TxtCount.Text <> 0 Then
                Dim w As New PrintExam
                w.TxtDiplomGuid.Text = TxtDiplomGuid.Text
                w.TxtLevelGuid.Text = TxtLevelGuid.Text
                w.ShowDialog()
                If w.IS_Yes = True Then
                    TxtExamGuid.Text = w.TxtExamGuid.Text
                End If
                If TxtExamGuid.Text <> Nothing Then
                    PRINTEXAM()
                End If
            Else
                MessageBox.Show("لا تتوفر اي بيانات للطلاب لطباعة اختبار المستوي", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Error)
                Return
            End If
        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try
    End Sub

    Private Sub LevelReportForm_Load(sender As Object, e As EventArgs) Handles MyBase.Load
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

    Private Sub StudentGrid_CellContentClick(sender As Object, e As DataGridViewCellEventArgs) Handles StudentGrid.CellContentClick

    End Sub

    Private Sub StudentGrid_CellClick(sender As Object, e As DataGridViewCellEventArgs) Handles StudentGrid.CellClick
        Try
            Dim i As Integer = StudentGrid.CurrentCell.RowIndex
            Dim j As Integer = StudentGrid.CurrentCell.ColumnIndex
            If j = 8 Then
                If AllFiles.FORFIND("studentstatment") = False Then
                    Return
                Else
                    Dim w As New StusdentStatment
                    w.TxtId.Text = StudentGrid.Rows(i).Cells(0).Value.ToString
                    w.TxtStudentName.Text = StudentGrid.Rows(i).Cells(1).Value.ToString
                    w.TxtAccountGuid.Text = StudentGrid.Rows(i).Cells(12).Value.ToString
                    w.TxtNationalId.Text = StudentGrid.Rows(i).Cells(2).Value.ToString
                    w.ShowDialog()
                End If
            ElseIf j = 7 Then
                Dim w As New UpdateStudentLevel
                w.TxtBatchGuid.Text = TxtBatchGuid.Text
                w.TxtDiplomGuid.Text = TxtDiplomGuid.Text
                w.TxtBranchGuid.Text = TxtBranchGuid.Text
                w.TxtAccountGuid.Text = StudentGrid.Rows(i).Cells(12).Value.ToString
                w.ShowDialog()
                If w.IS_Yes = True Then
                    VIEWSTUDENTINLEVEL()
                End If
            ElseIf j = 9 Then
                'If TxtBatchGuid.Text <> Nothing Then
                Dim w As New AddStudentLevelNotes
                    w.TxtBatchGuid.Text = TxtBatchGuid.Text
                    w.TxtDiplomGuid.Text = TxtDiplomGuid.Text
                    w.TxtBranchGuid.Text = TxtBranchGuid.Text
                    w.TxtlevelGuid.Text = TxtLevelGuid.Text
                    w.TxtAccountGuid.Text = StudentGrid.Rows(i).Cells(12).Value.ToString
                    w.TxtNotes.Text = StudentGrid.Rows(i).Cells(5).Value.ToString
                    w.ShowDialog()
                    If w.IS_Yes = True Then
                        VIEWSTUDENTINLEVEL()
                    End If
                'Else
                '    MessageBox.Show("يرجي اختيار الدفعة المسجل بها الطالب حاليا", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Error)
                '    Return
                'End If
            ElseIf j = 10 Then
                    If TxtBatchGuid.Text <> Nothing Then
                        Dim w As New TransferStudentCourse
                        w.TxtRegType.Text = 0
                        w.TxtBranch1Name.Text = StudentGrid.Rows(i).Cells(15).Value.ToString
                        w.TxtBranch1Guid.Text = StudentGrid.Rows(i).Cells(16).Value.ToString
                        w.TxtBranch2Name.Text = TxtBranchName.Text
                        w.TxtBranch2Guid.Text = TxtBranchGuid.Text
                        w.TxtDiplomName.Text = StudentGrid.Rows(i).Cells(14).Value.ToString
                        w.TxtOldDiplomGuid.Text = StudentGrid.Rows(i).Cells(13).Value.ToString
                        w.TxtBatchName.Text = TxtBatchName.Text
                        w.TxtAccountGuid.Text = StudentGrid.Rows(i).Cells(12).Value.ToString
                        w.TxtOldBatchGuid.Text = TxtBatchGuid.Text
                        w.ShowDialog()
                    Else
                        MessageBox.Show("يرجي اختيار الدفعة المسجل بها الطالب حاليا", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Error)
                        Return
                    End If

                ElseIf j = 11 Then
                    If AllFiles.FORSAVE("changestaut") = False Then
                        Return
                    Else
                        Dim w As New ChangeStaut
                        w.TxtRegType.Text = 0
                        w.TxtStudentName.Text = StudentGrid.Rows(i).Cells(1).Value.ToString
                        w.TxtnationalId.Text = StudentGrid.Rows(i).Cells(2).Value.ToString
                        w.TxtGuid.Text = StudentGrid.Rows(i).Cells(12).Value.ToString
                        w.TxtDiplomGuid.Text = StudentGrid.Rows(i).Cells(13).Value.ToString
                        w.TxtBranchGuid.Text = TxtBranchGuid.Text
                        w.ShowDialog()
                        If w.IS_Yes = True Then
                            VIEWSTUDENTINLEVEL()
                        End If
                    End If
                ElseIf j = 6 Then
                    If StudentGrid.Rows(i).Cells(6).Value = False Then
                        StudentGrid.Rows(i).Cells(6).Value = True
                    Else
                        StudentGrid.Rows(i).Cells(6).Value = False
                    End If
                ElseIf j = 17 Then
                If AllFiles.FORSAVE("finshstudy") = False Then
                    Return
                Else
                    If TxtBatchGuid.Text <> Nothing Then
                        Dim w As New FinshStudy
                        w.TxtRegType.Text = 0
                        w.TxtBatchGuid.Text = TxtBatchGuid.Text
                        w.TxtStudentName.Text = StudentGrid.Rows(i).Cells(1).Value.ToString
                        w.TxtnationalId.Text = StudentGrid.Rows(i).Cells(2).Value.ToString
                        w.TxtGuid.Text = StudentGrid.Rows(i).Cells(12).Value.ToString
                        w.TxtDiplomGuid.Text = StudentGrid.Rows(i).Cells(13).Value.ToString
                        w.TxtBranchGuid.Text = TxtBranchGuid.Text
                        w.ShowDialog()
                        If w.IS_Yes = True Then
                            VIEWSTUDENTINLEVEL()
                        End If
                    Else
                        MessageBox.Show("برجاء تحديد دفعه الطالب المراد انهاء الدراسة بها", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Error)
                        Return
                    End If
                End If
            End If
        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try
    End Sub
    Sub PRINT()
        Dim connection = dbcontext.Database.Connection
        Dim comm As New SqlCommand
        comm = New SqlCommand("ShowStudentByDiplomAndLevel", connection)
        comm.Parameters.AddWithValue("@DiplomGuid", TxtDiplomGuid.Text)
        comm.Parameters.AddWithValue("@BranchGuid", TxtBranchGuid.Text)
        If TxtBatchGuid.Text <> Nothing Then
            comm.Parameters.AddWithValue("@BatchGuid", TxtBatchGuid.Text)
        End If
        If TxtLevelGuid.Text <> Nothing Then
            comm.Parameters.AddWithValue("@LevelGuid", TxtLevelGuid.Text)
        End If
        If TxtCaseGuid.Text <> Nothing Then
            comm.Parameters.AddWithValue("@CaseGuid", TxtCaseGuid.Text)
        End If
        If TxtFind2.Text <> Nothing Then
            comm.Parameters.AddWithValue("@NationalId", TxtFind2.Text)
        End If
        comm.CommandType = CommandType.StoredProcedure
        If connection.State = ConnectionState.Open Then
            connection.Close()
        End If
        connection.Open()
        comm.ExecuteNonQuery()
        Dim adp As New SqlDataAdapter(comm)
        Dim ds As DataSet = New DataSet()
        adp.Fill(ds, "ShowStudentByDiplomAndLevel")
        Dim dt = ds.Tables(0)
        Dim report As New StudentDiplomLevel
        report.DataSource = dt
        report.DataAdapter = adp
        report.DataMember = "DataTable1"
        report.RequestParameters = False
        report.Parameters(0).Value = TxtBranchName.Text
        report.Parameters(1).Value = TxtDiplomName.Text
        report.Parameters(2).Value = TxtBatchName.Text
        report.Parameters(3).Value = TxtLevelName.Text
        Dim Tool_Report As ReportPrintTool = New ReportPrintTool(report)
        report.CreateDocument()
        Dim frm As New ViewReport
        frm.DocumentViewer1.DocumentSource = report
        frm.ShowDialog()
    End Sub
    Private Sub CmdPrint_Click(sender As Object, e As EventArgs) Handles CmdPrint.Click
        Try
            If TxtBranchGuid.Text = Nothing Then
            Else
                If TxtDiplomGuid.Text = Nothing Then
                Else
                    PRINT()
                End If
            End If
        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try
    End Sub

    Private Sub ChkAll_CheckedChanged(sender As Object, e As EventArgs) Handles ChkAll.CheckedChanged
        Try
            For Each row As DataGridViewRow In StudentGrid.Rows
                If row.Cells(12).Value <> Nothing Then
                    If ChkAll.Checked = True Then
                        row.Cells(6).Value = True
                    Else
                        row.Cells(6).Value = False
                    End If
                End If
            Next
        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try
    End Sub
    Sub UPDATELEVEL()
        Dim connection = dbcontext.Database.Connection
        Dim comm = New SqlCommand("UpdateStudentLevel", connection)
        comm.CommandType = CommandType.StoredProcedure
        For i As Integer = 0 To StudentGrid.Rows.Count - 1
            If StudentGrid.Rows(i).Cells(12).Value.ToString() = Nothing Then
                Exit For
            Else
                If StudentGrid.Rows(i).Cells(6).Value = True Then
                    comm.Parameters.AddWithValue("@StudentGuid", StudentGrid.Rows(i).Cells(12).Value.ToString())
                    comm.Parameters.AddWithValue("@BatchGuid", TxtBatchGuid.Text)
                    comm.Parameters.AddWithValue("@DiplomGuid", TxtDiplomGuid.Text)
                    comm.Parameters.AddWithValue("@BranchGuid", TxtBranchGuid.Text)
                    comm.Parameters.AddWithValue("@LevelGuid", TxtNewLevelGuid.Text)
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
    Private Sub CmdPostAll_Click(sender As Object, e As EventArgs) Handles CmdPostAll.Click
        Try
            If TxtNewLevelGuid.Text <> Nothing Then
                UPDATELEVEL()
                MessageBox.Show("تم ترحيل كل الطلاب بنجاح", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Information)
            Else
                MessageBox.Show("برجاء اختيار المستوي المراد ترحيل الكل اليه", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Error)
                Return
            End If
        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try
    End Sub

    Private Sub CmdFindNewLevel_Click(sender As Object, e As EventArgs) Handles CmdFindNewLevel.Click
        Try
            Dim w As New LevelList
            w.TxtIsUse.Text = True
            w.ShowDialog()
            If w.IS_Yes = True Then
                Dim ii As Integer = w.BranchGrid.CurrentRow.Index
                TxtNewLevelName.Text = w.BranchGrid.Rows(ii).Cells(1).Value.ToString()
                TxtNewLevelGuid.Text = w.BranchGrid.Rows(ii).Cells(3).Value.ToString()
            End If
        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try
    End Sub

    Private Sub CmdFindStudent_Click(sender As Object, e As EventArgs) Handles CmdFindStudent.Click
        Try
            Dim w As New FindStudent
            w.ShowDialog()
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
            If TxtBranchGuid.Text <> Nothing Then
                If TxtDiplomGuid.Text <> Nothing Then
                    If TxtCaseGuid.Text <> Nothing Then
                        VIEWSTUDENTINLEVEL()
                    End If
                End If
            End If
        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try
    End Sub

    Private Sub CmdDelStaut_Click(sender As Object, e As EventArgs) Handles CmdDelStaut.Click
        TxtCaseName.Text = Nothing
        TxtCaseGuid.Text = Nothing
        If TxtDiplomGuid.Text <> Nothing Then
            If TxtBranchGuid.Text <> Nothing Then
                VIEWSTUDENTINLEVEL()
            End If
        End If
    End Sub

    Private Sub CmdDelBatch_Click(sender As Object, e As EventArgs) Handles CmdDelBatch.Click
        TxtBatchGuid.Text = Nothing
        TxtBatchName.Text = Nothing
        If TxtBranchGuid.Text <> Nothing Then
            If TxtDiplomGuid.Text <> Nothing Then
                If TxtCaseGuid.Text <> Nothing Then
                    VIEWSTUDENTINLEVEL()
                End If
            End If
        End If
    End Sub

    Private Sub CmdDelLevel_Click(sender As Object, e As EventArgs) Handles CmdDelLevel.Click
        TxtLevelGuid.Text = Nothing
        TxtLevelName.Text = Nothing
        If TxtBranchGuid.Text <> Nothing Then
            If TxtDiplomGuid.Text <> Nothing Then
                If TxtCaseGuid.Text <> Nothing Then
                    VIEWSTUDENTINLEVEL()
                End If
            End If
        End If
    End Sub

    Private Sub TxtFind2_TextChanged(sender As Object, e As EventArgs) Handles TxtFind2.TextChanged
        Try
            If TxtDiplomGuid.Text <> Nothing Then
                If TxtBranchGuid.Text <> Nothing Then
                    VIEWSTUDENTINLEVEL()
                End If
            End If
        Catch ex As Exception
            MessageBox.Show(ex, ToString)
        End Try
    End Sub

    Private Sub CMDNew_Click(sender As Object, e As EventArgs) Handles CMDNew.Click
        Try
            If StudentGrid.Rows.Count = 0 Then
                MessageBox.Show("لا توجد بيانات لتصديرها", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Error)
                Return
            Else
                ' Open a SaveFileDialog to choose the output location
                SaveFileDialog1.InitialDirectory = "C:"
                SaveFileDialog1.Title = "Save as Excel File"
                SaveFileDialog1.FileName = "قائمة طلاب الدبلوم"
                SaveFileDialog1.Filter = "Excel Files(2003)|*.xls| Excel Files(2007)|*.xlsx"
                If SaveFileDialog1.ShowDialog() <> DialogResult.Cancel Then
                    ' Create a new Excel Application instance
                    Dim ExcelApp As New Excel.Application()
                    ' Add a new workbook
                    ExcelApp.Workbooks.Add(Type.Missing)

                    ' Autofit columns
                    ExcelApp.Columns.AutoFit()

                    ' Get the active worksheet
                    Dim sheet As Excel.Worksheet = ExcelApp.ActiveSheet

                    ' Write column headers from DataGridView
                    Dim columnsToExclude As New List(Of Integer) From {6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16}

                    For i As Integer = 0 To StudentGrid.Columns.Count - 1
                        If Not columnsToExclude.Contains(i) Then
                            sheet.Cells(1, i + 1) = StudentGrid.Columns(i).HeaderText
                        End If
                    Next
                    ' Write data from DataGridView rows
                    For i As Integer = 0 To StudentGrid.Rows.Count - 1
                        For j As Integer = 0 To StudentGrid.Columns.Count - 1
                            If Not columnsToExclude.Contains(j) Then
                                sheet.Cells(i + 2, j + 1) = StudentGrid.Rows(i).Cells(j).Value.ToString()
                            End If
                        Next
                    Next
                    ' Show success message
                    MessageBox.Show("تم تصدير البيانات بنجاح")
                    ' Save the workbook
                    sheet.SaveAs(SaveFileDialog1.FileName)
                    Dim result As DialogResult = MessageBox.Show("هل تريد عرض الملف", "تنبيه", MessageBoxButtons.OKCancel, MessageBoxIcon.Question)
                    If result = DialogResult.OK Then
                        ' Make Excel visible (optional)
                        ExcelApp.Visible = True
                    Else
                        ExcelApp.Visible = False
                        ' Quit Excel application
                        ExcelApp.Quit()
                    End If
                End If
            End If
        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try
    End Sub
    Sub CHANGEREGBRANCH()
        Dim connection = dbcontext.Database.Connection
        Dim comm = New SqlCommand("ChangeRegBranch", connection)
        comm.CommandType = CommandType.StoredProcedure
        For i As Integer = 0 To StudentGrid.Rows.Count - 1
            If StudentGrid.Rows(i).Cells(12).Value.ToString() = Nothing Then
                Exit For
            Else
                If StudentGrid.Rows(i).Cells(6).Value = True Then
                    comm.Parameters.AddWithValue("@AccountGuid", StudentGrid.Rows(i).Cells(12).Value.ToString())
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
            If StudentGrid.Rows(i).Cells(12).Value.ToString() = Nothing Then
                Exit For
            Else
                If StudentGrid.Rows(i).Cells(6).Value = True Then
                    comm.Parameters.AddWithValue("@StudentGuid", StudentGrid.Rows(i).Cells(12).Value.ToString())
                    comm.Parameters.AddWithValue("@BranchGuid", NEWSTUDYBRANCH)
                    comm.Parameters.AddWithValue("@DiplomGuid", TxtDiplomGuid.Text)
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