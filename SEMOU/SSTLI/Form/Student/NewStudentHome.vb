Imports System.Data.SqlClient
Imports DevExpress.XtraReports.UI
Imports System.IO
Imports System.Windows.Forms.VisualStyles.VisualStyleElement
Imports System.Windows.Forms

Public Class NewStudentHome
    Dim dbcontext As New SemouEntities
    Dim REASON As String = ""
    Private listView1 As Object

    Private Sub CmdExit_Click(sender As Object, e As EventArgs) Handles CmdExit.Click
        Me.Close()
    End Sub
    Sub VIEWSTUDENTDIPLOM()
        StudentGrid.Rows.Clear()
        Dim connection = dbcontext.Database.Connection
        Dim comm As New SqlCommand("LoadStudentForAccept", connection)
        comm.Parameters.AddWithValue("@BranchGuid", TxtBranchGuid.Text)
        If TxtFind2.Text <> Nothing Then
            comm.Parameters.AddWithValue("@StudentName", TxtFind2.Text)
        End If
        comm.Parameters.AddWithValue("@RegType", 0)
        If TxtSalesManGuid.Text <> Nothing Then
            comm.Parameters.AddWithValue("@SalesManGuid", TxtSalesManGuid.Text)
        End If
        comm.CommandType = CommandType.StoredProcedure
        If connection.State = ConnectionState.Open Then
            connection.Close()
        End If
        connection.Open()
        comm.ExecuteNonQuery()
        Dim adp As New SqlDataAdapter(comm)
        Dim ds As DataSet = New DataSet()
        adp.Fill(ds, "LoadStudentForAccept")
        Dim COUNT__ As Integer = 0
        If ds.Tables(0).Rows.Count > 0 Then
            StudentGrid.RowCount = ds.Tables(0).Rows.Count
            TxtDiplomCount.Text = StudentGrid.Rows.Count()
            For i As Integer = 0 To ds.Tables(0).Rows.Count - 1
                StudentGrid.Rows(COUNT__).Cells(0).Value = ds.Tables(0).Rows(i)(0).ToString()
                StudentGrid.Rows(COUNT__).Cells(1).Value = Convert.ToDateTime(ds.Tables(0).Rows(i)(1)).ToShortDateString()
                StudentGrid.Rows(COUNT__).Cells(2).Value = ds.Tables(0).Rows(i)(2).ToString()
                StudentGrid.Rows(COUNT__).Cells(3).Value = ds.Tables(0).Rows(i)(3).ToString()
                StudentGrid.Rows(COUNT__).Cells(4).Value = ds.Tables(0).Rows(i)(4).ToString()
                StudentGrid.Rows(COUNT__).Cells(5).Value = ds.Tables(0).Rows(i)(5).ToString()
                StudentGrid.Rows(COUNT__).Cells(6).Value = ds.Tables(0).Rows(i)(6).ToString()
                StudentGrid.Rows(COUNT__).Cells(7).Value = ds.Tables(0).Rows(i)(7).ToString()
                StudentGrid.Rows(COUNT__).Cells(8).Value = ds.Tables(0).Rows(i)(8).ToString()
                StudentGrid.Rows(COUNT__).Cells(9).Value = ds.Tables(0).Rows(i)(9).ToString()
                StudentGrid.Rows(COUNT__).Cells(10).Value = ds.Tables(0).Rows(i)(10).ToString()
                StudentGrid.Rows(COUNT__).Cells(11).Value = ds.Tables(0).Rows(i)(11).ToString()
                StudentGrid.Rows(COUNT__).Cells(12).Value = ds.Tables(0).Rows(i)(12).ToString()
                StudentGrid.Rows(COUNT__).Cells(13).Value = ds.Tables(0).Rows(i)(13).ToString()
                StudentGrid.Rows(COUNT__).Cells(14).Value = ds.Tables(0).Rows(i)(14).ToString()
                StudentGrid.Rows(COUNT__).Cells(15).Value = ds.Tables(0).Rows(i)(15).ToString()
                StudentGrid.Rows(COUNT__).Cells(18).Value = ds.Tables(0).Rows(i)(16).ToString()
                StudentGrid.Rows(COUNT__).Cells(19).Value = ds.Tables(0).Rows(i)(17).ToString()
                StudentGrid.Rows(COUNT__).Cells(20).Value = ds.Tables(0).Rows(i)(18).ToString()
                StudentGrid.Rows(COUNT__).Cells(21).Value = ds.Tables(0).Rows(i)(19).ToString()
                StudentGrid.Rows(COUNT__).Cells(22).Value = ds.Tables(0).Rows(i)(20).ToString()
                COUNT__ = COUNT__ + 1
            Next
        End If
    End Sub
    Sub VIEWSTUDENTCOURSE()
        CourseGrid.Rows.Clear()
        Dim connection = dbcontext.Database.Connection
        Dim comm As New SqlCommand("LoadStudentForAccept", connection)
        comm.Parameters.AddWithValue("@BranchGuid", TxtBranchGuid.Text)
        If TxtCourseFind.Text <> Nothing Then
            comm.Parameters.AddWithValue("@StudentName", TxtCourseFind.Text)
        End If
        comm.Parameters.AddWithValue("@RegType", 1)
        If TxtSalesManGuid.Text <> Nothing Then
            comm.Parameters.AddWithValue("@SalesManGuid", TxtSalesManGuid.Text)
        End If
        comm.CommandType = CommandType.StoredProcedure
        If connection.State = ConnectionState.Open Then
            connection.Close()
        End If
        connection.Open()
        comm.ExecuteNonQuery()
        Dim adp As New SqlDataAdapter(comm)
        Dim ds As DataSet = New DataSet()
        adp.Fill(ds, "LoadStudentForAccept")
        Dim COUNT__ As Integer = 0
        If ds.Tables(0).Rows.Count > 0 Then
            CourseGrid.RowCount = ds.Tables(0).Rows.Count
            TxtCourseCount.Text = CourseGrid.Rows.Count()
            For i As Integer = 0 To ds.Tables(0).Rows.Count - 1
                CourseGrid.Rows(COUNT__).Cells(0).Value = ds.Tables(0).Rows(i)(0).ToString()
                CourseGrid.Rows(COUNT__).Cells(1).Value = Convert.ToDateTime(ds.Tables(0).Rows(i)(1)).ToShortDateString()
                CourseGrid.Rows(COUNT__).Cells(2).Value = ds.Tables(0).Rows(i)(2).ToString()
                CourseGrid.Rows(COUNT__).Cells(3).Value = ds.Tables(0).Rows(i)(3).ToString()
                CourseGrid.Rows(COUNT__).Cells(4).Value = ds.Tables(0).Rows(i)(4).ToString()
                CourseGrid.Rows(COUNT__).Cells(5).Value = ds.Tables(0).Rows(i)(5).ToString()
                CourseGrid.Rows(COUNT__).Cells(6).Value = ds.Tables(0).Rows(i)(6).ToString()
                CourseGrid.Rows(COUNT__).Cells(7).Value = ds.Tables(0).Rows(i)(7).ToString()
                CourseGrid.Rows(COUNT__).Cells(8).Value = ds.Tables(0).Rows(i)(8).ToString()
                CourseGrid.Rows(COUNT__).Cells(9).Value = ds.Tables(0).Rows(i)(9).ToString()
                CourseGrid.Rows(COUNT__).Cells(10).Value = ds.Tables(0).Rows(i)(10).ToString()
                CourseGrid.Rows(COUNT__).Cells(11).Value = ds.Tables(0).Rows(i)(11).ToString()
                CourseGrid.Rows(COUNT__).Cells(12).Value = ds.Tables(0).Rows(i)(12).ToString()
                CourseGrid.Rows(COUNT__).Cells(13).Value = ds.Tables(0).Rows(i)(13).ToString()
                CourseGrid.Rows(COUNT__).Cells(14).Value = ds.Tables(0).Rows(i)(14).ToString()
                CourseGrid.Rows(COUNT__).Cells(15).Value = ds.Tables(0).Rows(i)(15).ToString()
                CourseGrid.Rows(COUNT__).Cells(18).Value = ds.Tables(0).Rows(i)(16).ToString()
                CourseGrid.Rows(COUNT__).Cells(19).Value = ds.Tables(0).Rows(i)(17).ToString()
                CourseGrid.Rows(COUNT__).Cells(20).Value = ds.Tables(0).Rows(i)(18).ToString()
                CourseGrid.Rows(COUNT__).Cells(21).Value = ds.Tables(0).Rows(i)(19).ToString()
                CourseGrid.Rows(COUNT__).Cells(22).Value = ds.Tables(0).Rows(i)(20).ToString()
                COUNT__ = COUNT__ + 1
            Next
        End If
    End Sub
    Sub CLEAR()
        TxtStudentName.Text = Nothing
        TxtAccountGuid.Text = Nothing
        TxtTel.Text = Nothing
        TxtNationalId.Text = Nothing
        TxtRegDocGuid.Text = Nothing
        TxtDiplomGuid.Text = Nothing
        TxtDiplomName.Text = Nothing
        TxtBatchGuid.Text = Nothing
        TxtBatchName.Text = Nothing
        TxtLevelGuid.Text = Nothing
        TxtLevelName.Text = Nothing
        TxtRegBranchName.Text = Nothing
        TxtRegBranchGuid.Text = Nothing
        ChkAttach.Checked = False
        ChkAccept.Checked = False
        TxtNotes1.Text = Nothing
        TxtNotes2.Text = Nothing
        TxtRegDocCode.Text = Nothing
        TxtDiplomOk.Text = Nothing
    End Sub
    Sub CLEAR2()
        TxtStudentName2.Text = Nothing
        TxtAccountGuid.Text = Nothing
        TxtTel2.Text = Nothing
        TxtNationalId2.Text = Nothing
        TxtRegDocGuid.Text = Nothing
        TxtDiplomGuid2.Text = Nothing
        TxtDiplomName2.Text = Nothing
        TxtBranchName2.Text = Nothing
        TxtBranchGuid2.Text = Nothing
        TxtCourseName.Text = Nothing
        TxtCourseGuid.Text = Nothing
        ChkCourseAccept.Checked = False
        ChkAccept2.Checked = False
        TxtCourseNotes1.Text = Nothing
        TxtCourseNotes2.Text = Nothing
        TxtCauseGuid.Text = Nothing
        TxtRegDocCode.Text = Nothing
        TxtCourseOk.Text = Nothing
    End Sub
    Private Sub CmdRefresh_Click(sender As Object, e As EventArgs) Handles CmdRefresh.Click
        Try
            CLEAR()
            CLEAR2()
            If TxtBranchGuid.Text <> Nothing Then
                VIEWSTUDENTCOURSE()
                VIEWSTUDENTDIPLOM()
            Else
                StudentGrid.Rows.Clear()
                CourseGrid.Rows.Clear()
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
                w.TxtFind.ReadOnly = True
            Else
                w.TxtType.Text = 0
                w.TxtIsUse.Text = True
                w.TxtFind.ReadOnly = False
            End If
            w.ShowDialog()
            If w.IS_Yes = True Then
                Dim ii As Integer = w.BranchGrid.CurrentRow.Index
                TxtBranchName.Text = w.BranchGrid.Rows(ii).Cells(1).Value.ToString()
                TxtBranchGuid.Text = w.BranchGrid.Rows(ii).Cells(3).Value.ToString()
            End If
            If TxtBranchGuid.Text <> Nothing Then
                VIEWSTUDENTDIPLOM()
                VIEWSTUDENTCOURSE()
            End If
        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try
    End Sub

    Private Sub TxtFind2_TextChanged(sender As Object, e As EventArgs) Handles TxtFind2.TextChanged
        Try
            If TxtBranchGuid.Text <> Nothing Then
                VIEWSTUDENTDIPLOM()
            Else
                MessageBox.Show("برجاء اختيار الفرع اولا", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Error)
                Return
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
            CLEAR()
            TxtRegDocCode.Text = StudentGrid.Rows(i).Cells(0).Value.ToString
            TxtStudentName.Text = StudentGrid.Rows(i).Cells(2).Value.ToString
            TxtTel.Text = StudentGrid.Rows(i).Cells(3).Value.ToString
            TxtNationalId.Text = StudentGrid.Rows(i).Cells(4).Value.ToString
            TxtDiplomName.Text = StudentGrid.Rows(i).Cells(5).Value.ToString
            TxtDiplomGuid.Text = StudentGrid.Rows(i).Cells(12).Value.ToString
            TxtRegBranchName.Text = StudentGrid.Rows(i).Cells(6).Value.ToString
            TxtRegBranchGuid.Text = StudentGrid.Rows(i).Cells(14).Value.ToString
            TxtBatchName.Text = StudentGrid.Rows(i).Cells(7).Value.ToString
            TxtBatchGuid.Text = StudentGrid.Rows(i).Cells(11).Value.ToString
            TxtLevelName.Text = StudentGrid.Rows(i).Cells(8).Value.ToString
            TxtLevelGuid.Text = StudentGrid.Rows(i).Cells(10).Value.ToString
            TxtRegDocGuid.Text = StudentGrid.Rows(i).Cells(13).Value.ToString
            TxtAccountGuid.Text = StudentGrid.Rows(i).Cells(9).Value.ToString
            TxtDiplomCause.Text = StudentGrid.Rows(i).Cells(21).Value.ToString
            If j = 16 Then
                Dim w As New EncodingNewPayInvoice
                w.TxtStudentName.Text = StudentGrid.Rows(i).Cells(2).Value.ToString
                w.TxtNationalId.Text = StudentGrid.Rows(i).Cells(4).Value.ToString
                w.TxtTel.Text = StudentGrid.Rows(i).Cells(3).Value.ToString
                w.TxtAccountGuid.Text = StudentGrid.Rows(i).Cells(9).Value.ToString
                w.TxtRegDocGuid.Text = StudentGrid.Rows(i).Cells(13).Value.ToString
                w.SHOWREGDOCDIPLOM(0)
                w.ShowDialog()
            ElseIf j = 17 Then
                If AllFiles.FORFIND("studentstatment") = False Then
                    Return
                Else
                    Dim w As New StusdentStatment
                    w.TxtId.Text = StudentGrid.Rows(i).Cells(15).Value.ToString
                    w.TxtStudentName.Text = StudentGrid.Rows(i).Cells(2).Value.ToString
                    w.TxtAccountGuid.Text = StudentGrid.Rows(i).Cells(9).Value.ToString
                    w.ShowDialog()
                End If
            End If
        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try
    End Sub
    Sub ADDSTUDENTLEVEL()
        Dim connection = dbcontext.Database.Connection
        Dim comm = New SqlCommand("AddStudentLevel", connection)
        comm.CommandType = CommandType.StoredProcedure
        comm.Parameters.AddWithValue("@StudentGuid", TxtAccountGuid.Text)
        comm.Parameters.AddWithValue("@RegDocGuid", TxtRegDocGuid.Text)
        comm.Parameters.AddWithValue("@LevelGuid", TxtLevelGuid.Text)
        comm.Parameters.AddWithValue("@LevelMark", 0)
        comm.Parameters.AddWithValue("@LevelStaut", True)
        comm.Parameters.AddWithValue("@BatchGuid", TxtBatchGuid.Text)
        comm.Parameters.AddWithValue("@DiplomGuid", TxtDiplomGuid.Text)
        comm.Parameters.AddWithValue("@BranchGuid", TxtRegBranchGuid.Text)
        comm.Parameters.AddWithValue("@CourseGuid", "00000000-0000-0000-0000-000000000000")
        comm.Parameters.AddWithValue("@CaseGuid", TxtDiplomCause.Text)
        comm.Parameters.AddWithValue("@RegType", 0)
        If connection.State = ConnectionState.Open Then
            connection.Close()
        End If
        connection.Open()
        comm.ExecuteNonQuery()
        comm.Parameters.Clear()
    End Sub
    Sub ADDREGDOCNOTES()
        Dim connection = dbcontext.Database.Connection
        Dim comm = New SqlCommand("AddRegDocNotes", connection)
        comm.CommandType = CommandType.StoredProcedure
        comm.Parameters.AddWithValue("@RegDocGuid", TxtRegDocGuid.Text)
        comm.Parameters.AddWithValue("@ChkAttach", ChkAttach.Checked)
        comm.Parameters.AddWithValue("@AttachNotes", TxtNotes1.Text)
        comm.Parameters.AddWithValue("@ChlAccept", ChkAccept.Checked)
        comm.Parameters.AddWithValue("@AcceptNotes", TxtNotes2.Text)
        If connection.State = ConnectionState.Open Then
            connection.Close()
        End If
        connection.Open()
        comm.ExecuteNonQuery()
        comm.Parameters.Clear()
    End Sub
    Sub ADDCOURSEREGDOCNOTES()
        Dim connection = dbcontext.Database.Connection
        Dim comm = New SqlCommand("AddRegDocNotes", connection)
        comm.CommandType = CommandType.StoredProcedure
        comm.Parameters.AddWithValue("@RegDocGuid", TxtRegDocGuid.Text)
        comm.Parameters.AddWithValue("@ChkAttach", ChkAccept2.Checked)
        comm.Parameters.AddWithValue("@AttachNotes", TxtCourseNotes1.Text)
        comm.Parameters.AddWithValue("@ChlAccept", ChkCourseAccept.Checked)
        comm.Parameters.AddWithValue("@AcceptNotes", TxtCourseNotes2.Text)
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
        comm.Parameters.AddWithValue("@ActionGuid", TxtRegDocGuid.Text)
        comm.Parameters.AddWithValue("@UserGuid", AllFiles.USER_GUID____)
        comm.Parameters.AddWithValue("@Date_", Convert.ToDateTime(DateTime.Now))
        comm.Parameters.AddWithValue("@Time_", Convert.ToDateTime(DateTime.Now.ToShortTimeString))
        comm.Parameters.AddWithValue("@Notes_", AllFiles.ActionType___)
        comm.Parameters.AddWithValue("@ActionTypeGuid", TxtTypeGuid.Text)
        comm.Parameters.AddWithValue("@ActionReason", REASON)
        comm.Parameters.AddWithValue("@ActionCode", TxtRegDocCode.Text)
        If connection.State = ConnectionState.Open Then
            connection.Close()
        End If
        connection.Open()
        comm.ExecuteNonQuery()
        comm.Parameters.Clear()
    End Sub
    Sub ACCEPTREGDOC()
        Dim connection = dbcontext.Database.Connection
        Dim comm = New SqlCommand("AcceptRegDoc", connection)
        comm.CommandType = CommandType.StoredProcedure
        comm.Parameters.AddWithValue("@Guid", TxtRegDocGuid.Text)
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
        comm.Parameters.AddWithValue("@RegDocGuid", TxtRegDocGuid.Text)
        comm.Parameters.AddWithValue("@LevelGuid", "00000000-0000-0000-0000-000000000000")
        comm.Parameters.AddWithValue("@LevelMark", 0)
        comm.Parameters.AddWithValue("@LevelStaut", True)
        comm.Parameters.AddWithValue("@BatchGuid", "00000000-0000-0000-0000-000000000000")
        comm.Parameters.AddWithValue("@DiplomGuid", TxtDiplomGuid2.Text)
        comm.Parameters.AddWithValue("@BranchGuid", TxtBranchGuid2.Text)
        comm.Parameters.AddWithValue("@CourseGuid", TxtCourseGuid.Text)
        comm.Parameters.AddWithValue("@CaseGuid", TxtCauseGuid.Text)
        comm.Parameters.AddWithValue("@RegType", 1)
        If connection.State = ConnectionState.Open Then
            connection.Close()
        End If
        connection.Open()
        comm.ExecuteNonQuery()
        comm.Parameters.Clear()
    End Sub
    Private Sub CmdAccept_Click(sender As Object, e As EventArgs) Handles CmdAccept.Click
        Try
            If ChkAttach.Checked = False Then
                MessageBox.Show("برجاء تحديد حالة مستندات التسجيل", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Error)
                Return
            Else
                If ChkAccept.Checked = False Then
                    MessageBox.Show("برجاء تحديد حالة اتفاقية التدريب", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Error)
                    Return
                Else
                    If AllFiles.FORSAVE("newstudenthome") = False Then
                        Return
                    Else
                        If TxtDiplomOk.Text = Nothing Then
                            AllFiles.ActionType___ = "قبول التسجيل"
                            ADDSTUDENTLEVEL()
                            ADDREGDOCNOTES()
                            ACCEPTREGDOC()
                            ADD_USERACTION_()
                            TxtDiplomOk.Text = "ok"
                            MessageBox.Show("تم قبول تسجيل الطالب بنجاح", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Information)
                            VIEWSTUDENTDIPLOM()
                            TxtDiplomCount.Text = StudentGrid.Rows.Count()
                        End If
                    End If
                End If
            End If
        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try
    End Sub

    Private Sub NewStudentHome_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Try
            RegDocPosted.WHATFORMTYPEGUID("addregdoc")
            TxtTypeGuid.Text = RegDocPosted.TYPEGUID
            'AllFiles.CHEKDEFBRANCHINFO(AllFiles.USER_GUID____.ToString)
            'If AllFiles.DEFBRANCH = True Then
            '    CmdFindDepart.Enabled = False
            '    TxtBranchName.Text = AllFiles.DEFBRANCHNAME
            '    TxtBranchGuid.Text = AllFiles.DEFBRANCHGUID.ToString()
            '    If TxtBranchGuid.Text <> Nothing Then
            '        VIEWSTUDENTDIPLOM()
            '        VIEWSTUDENTCOURSE()
            '    End If
            'Else
            '    CmdFindDepart.Enabled = True
            '    TxtBranchName.Text = Nothing
            '    TxtBranchGuid.Text = Nothing
            'End If



            ' إخفاء القائمة السياقية
            'listView2.ContextMenuStrip = Nothing

            '' تعطيل مزدوج النقر
            'listView1.cl = False

            ' إضافة ListView إلى النموذج
            'Me.Controls.Add(listView1)

            ' إضافة حدث DoubleClick إلى ListView
            'AddHandler listView1.DoubleClick, AddressOf ListView1_DoubleClick
        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try
    End Sub
    Private Sub TxtCourseFind_TextChanged(sender As Object, e As EventArgs) Handles TxtCourseFind.TextChanged
        Try
            If TxtBranchGuid.Text <> Nothing Then
                VIEWSTUDENTCOURSE()
            Else
                MessageBox.Show("برجاء اختيار الفرع اولا", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Error)
                Return
            End If
        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try
    End Sub

    Private Sub CourseGrid_CellContentClick(sender As Object, e As DataGridViewCellEventArgs) Handles CourseGrid.CellContentClick

    End Sub

    Private Sub CourseGrid_CellClick(sender As Object, e As DataGridViewCellEventArgs) Handles CourseGrid.CellClick
        Try
            Dim i As Integer = CourseGrid.CurrentCell.RowIndex
            Dim j As Integer = CourseGrid.CurrentCell.ColumnIndex
            CLEAR2()
            TxtRegDocCode.Text = CourseGrid.Rows(i).Cells(0).Value.ToString
            TxtStudentName2.Text = CourseGrid.Rows(i).Cells(2).Value.ToString
            TxtTel2.Text = CourseGrid.Rows(i).Cells(3).Value.ToString
            TxtNationalId2.Text = CourseGrid.Rows(i).Cells(4).Value.ToString
            TxtRegDocGuid.Text = CourseGrid.Rows(i).Cells(13).Value.ToString
            TxtCourseName.Text = CourseGrid.Rows(i).Cells(18).Value.ToString
            TxtCourseGuid.Text = CourseGrid.Rows(i).Cells(19).Value.ToString
            TxtAccountGuid.Text = CourseGrid.Rows(i).Cells(9).Value.ToString
            TxtDiplomName2.Text = CourseGrid.Rows(i).Cells(5).Value.ToString
            TxtDiplomGuid2.Text = CourseGrid.Rows(i).Cells(12).Value.ToString
            TxtBranchName2.Text = CourseGrid.Rows(i).Cells(6).Value.ToString
            TxtBranchGuid2.Text = CourseGrid.Rows(i).Cells(14).Value.ToString
            TxtCountTime.Text = CourseGrid.Rows(i).Cells(20).Value.ToString
            TxtCauseGuid.Text = CourseGrid.Rows(i).Cells(21).Value.ToString
            If j = 16 Then
                Dim w As New EncodingNewPayInvoice
                w.TxtStudentName.Text = CourseGrid.Rows(i).Cells(2).Value.ToString
                w.TxtNationalId.Text = CourseGrid.Rows(i).Cells(4).Value.ToString
                w.TxtTel.Text = CourseGrid.Rows(i).Cells(3).Value.ToString
                w.TxtAccountGuid.Text = CourseGrid.Rows(i).Cells(9).Value.ToString
                w.TxtRegDocGuid.Text = CourseGrid.Rows(i).Cells(13).Value.ToString
                w.SHOWREGDOCDIPLOM(0)
                w.ShowDialog()
            ElseIf j = 17 Then
                If AllFiles.FORFIND("studentstatment") = False Then
                    Return
                Else
                    Dim w As New StusdentStatment
                    w.TxtId.Text = CourseGrid.Rows(i).Cells(15).Value.ToString
                    w.TxtStudentName.Text = CourseGrid.Rows(i).Cells(2).Value.ToString
                    w.TxtAccountGuid.Text = CourseGrid.Rows(i).Cells(9).Value.ToString
                    w.ShowDialog()
                End If
            End If
        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try
    End Sub

    Private Sub CmdAccept2_Click(sender As Object, e As EventArgs) Handles CmdAccept2.Click
        Try
            If ChkCourseAccept.Checked = False Then
                MessageBox.Show("برجاء تحديد حالة مستندات التسجيل", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Error)
                Return
            Else
                If ChkAccept2.Checked = False Then
                    MessageBox.Show("برجاء تحديد حالة اتفاقية التدريب", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Error)
                    Return
                Else
                    If AllFiles.FORSAVE("newstudenthome") = False Then
                        Return
                    Else
                        If TxtCourseOk.Text = Nothing Then
                            AllFiles.ActionType___ = "قبول التسجيل"
                            ADDSTUDENTCOURSE()
                            ADDCOURSEREGDOCNOTES()
                            ACCEPTREGDOC()
                            ADD_USERACTION_()
                            TxtCourseOk.Text = "ok"
                            MessageBox.Show("تم قبول تسجيل الطالب بنجاح", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Information)
                            VIEWSTUDENTCOURSE()
                            TxtCourseCount.Text = CourseGrid.Rows.Count()
                        End If
                    End If
                End If
            End If
        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try
    End Sub
    Sub PRINTDIPLOM()
        Dim report As New AcceptDoc
        report.RequestParameters = False
        report.Parameters(0).Value = TxtStudentName.Text
        report.Parameters(1).Value = TxtTel.Text
        report.Parameters(2).Value = TxtNationalId.Text
        report.Parameters(3).Value = TxtRegBranchName.Text
        report.Parameters(4).Value = TxtBatchName.Text
        report.Parameters(5).Value = TxtDiplomName.Text
        report.Parameters(6).Value = ChkAttach.Checked
        report.Parameters(7).Value = TxtNotes1.Text
        report.Parameters(8).Value = ChkAccept.Checked
        report.Parameters(9).Value = TxtNotes2.Text
        Dim Tool_Report As ReportPrintTool = New ReportPrintTool(report)
        report.CreateDocument()
        Dim frm As New ViewReport
        frm.DocumentViewer1.DocumentSource = report
        frm.ShowDialog()
    End Sub
    Sub PRINTCOURSE()
        Dim report As New AcceptDoc
        report.RequestParameters = False
        report.Parameters(0).Value = TxtStudentName2.Text
        report.Parameters(1).Value = TxtTel2.Text
        report.Parameters(2).Value = TxtNationalId2.Text
        report.Parameters(3).Value = TxtBranchName2.Text
        report.Parameters(4).Value = TxtCourseName.Text
        report.Parameters(5).Value = TxtDiplomName2.Text
        report.Parameters(6).Value = ChkAccept2.Checked
        report.Parameters(7).Value = TxtCourseNotes1.Text
        report.Parameters(8).Value = ChkCourseAccept.Checked
        report.Parameters(9).Value = TxtCourseNotes2.Text
        Dim Tool_Report As ReportPrintTool = New ReportPrintTool(report)
        report.CreateDocument()
        Dim frm As New ViewReport
        frm.DocumentViewer1.DocumentSource = report
        frm.ShowDialog()
    End Sub
    Private Sub CmdPrint_Click(sender As Object, e As EventArgs) Handles CmdPrint.Click
        Try
            If TxtRegDocGuid.Text = Nothing Then
                MessageBox.Show("برجاء اختيار طالب اولا", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Error)
                Return
            Else
                If ChkAttach.Checked = False Then
                    MessageBox.Show("برجاء تحديد حالة مستندات التسجيل", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Error)
                    Return
                Else
                    If ChkAccept.Checked = False Then
                        MessageBox.Show("برجاء تحديد حالة اتفاقية التدريب", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Error)
                        Return
                    Else
                        PRINTDIPLOM()
                    End If
                End If
            End If
        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try
    End Sub

    Private Sub CmdPrint2_Click(sender As Object, e As EventArgs) Handles CmdPrint2.Click
        Try
            If TxtRegDocGuid.Text = Nothing Then
                MessageBox.Show("برجاء اختيار طالب اولا", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Error)
                Return
            Else
                If ChkAccept2.Checked = False Then
                    MessageBox.Show("برجاء تحديد حالة مستندات التسجيل", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Error)
                    Return
                Else
                    If ChkCourseAccept.Checked = False Then
                        MessageBox.Show("برجاء تحديد حالة اتفاقية التدريب", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Error)
                        Return
                    Else
                        PRINTCOURSE()
                    End If
                End If
            End If
        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try
    End Sub

    Private Sub CmdFindSalesMan_Click(sender As Object, e As EventArgs) Handles CmdFindSalesMan.Click
        Try
            If TxtBranchGuid.Text <> Nothing Then
                Dim w As New SalesManList
                w.TxtIsUse.Text = True
                w.ShowDialog()
                If w.IS_Yes = True Then
                    Dim ii As Integer = w.BranchGrid.CurrentRow.Index
                    TxtSalesManName.Text = w.BranchGrid.Rows(ii).Cells(1).Value.ToString()
                    TxtSalesManGuid.Text = w.BranchGrid.Rows(ii).Cells(3).Value.ToString()
                End If
                If TxtSalesManGuid.Text <> Nothing Then
                    If TxtBranchGuid.Text <> Nothing Then
                        VIEWSTUDENTDIPLOM()
                        VIEWSTUDENTCOURSE()
                    End If
                End If
            Else
                MessageBox.Show("برجاء اختيار الفرع اولا", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Error)
                Return
            End If
        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try
    End Sub

    Private Sub CmdDelSalesMan_Click(sender As Object, e As EventArgs) Handles CmdDelSalesMan.Click
        Try
            TxtSalesManName.Text = Nothing
            TxtSalesManGuid.Text = Nothing
            If TxtBranchGuid.Text <> Nothing Then
                VIEWSTUDENTDIPLOM()
                VIEWSTUDENTCOURSE()
            End If
        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try
    End Sub

    Private Sub CmdOpenAttach_Click(sender As Object, e As EventArgs) Handles CmdOpenAttach.Click
        Try
            AllFiles.SHOWSYSTEMINFO()
            Dim baseDirectory As String = AllFiles.UPDATEURL
            Dim folderPath As String = baseDirectory & TxtNationalId.Text
            If TxtNationalId.Text = Nothing Then
                MessageBox.Show("برجاء اختيار الطالب اولا", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Error)
                Return
            Else
                If Not Directory.Exists(folderPath) Then
                    MessageBox.Show("لا يتوفر ملف مرفقات للطالب", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Error)
                    Return
                Else
                    Process.Start(folderPath)
                End If
            End If
        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try
    End Sub

    Private Sub ListView2_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ListView2.SelectedIndexChanged

    End Sub

    Private Sub ListView2_DoubleClick(sender As Object, e As EventArgs) Handles ListView2.DoubleClick
        Dim selectedItem As ListViewItem = ListView2.SelectedItems(0)
        Dim filePath As String = Path.Combine("\\192.168.100.16\d\" & TxtNationalId.Text, selectedItem.Text)

        ' التحقق من نوع الملف وعرضه
        Dim extension = Path.GetExtension(filePath).ToLower()
        Select Case extension
            Case ".jpeg", ".png", ".gif", ".jpg", ".pdf"
                '    ' عرض الصورة
                '    Dim image As Image = Image.FromFile(filePath)
                '    Dim pictureBox As New PictureBox
                '    pictureBox.Image = image
                '    pictureBox.SizeMode = PictureBoxSizeMode.Zoom
                '    pictureBox.Dock = DockStyle.Fill
                '    Me.Controls.Add(pictureBox)
                'Case ".jpg", ".pdf"
                Dim VIEW As New AttachView
                VIEW.WebBrowser1.Navigate(filePath)

                VIEW.ShowDialog()
            Case Else
                MessageBox.Show("نوع الملف غير مدعوم.")
        End Select
    End Sub

    Private Sub CmdViewAttach2_Click(sender As Object, e As EventArgs) Handles CmdViewAttach2.Click
        Try
            AllFiles.SHOWSYSTEMINFO()
            Dim baseDirectory As String = AllFiles.UPDATEURL
            Dim folderPath As String = baseDirectory & TxtNationalId2.Text
            If TxtNationalId2.Text = Nothing Then
                MessageBox.Show("برجاء اختيار الطالب اولا", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Error)
                Return
            Else
                If Not Directory.Exists(folderPath) Then
                    MessageBox.Show("لا يتوفر ملف مرفقات للطالب", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Error)
                    Return
                Else
                    Process.Start(folderPath)
                End If
            End If
        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try
    End Sub
End Class