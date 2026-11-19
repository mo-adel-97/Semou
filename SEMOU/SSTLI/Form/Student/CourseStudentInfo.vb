Imports System.Data.SqlClient

Public Class CourseStudentInfo
    Dim dbcontext As New SemouEntities
    Public IS_YES As Boolean = False
    Sub SHOWBRANCHINFO()
        Dim connection = dbcontext.Database.Connection
        Dim comm As New SqlCommand
        comm = New SqlCommand("ShowCourseStudentInfo", connection)
        comm.CommandType = CommandType.StoredProcedure
        comm.Parameters.AddWithValue("@NationalId", TxtNationalId.Text)
        comm.Parameters.AddWithValue("@BranchCode", TxtBranchCode.Text)
        If connection.State = ConnectionState.Open Then
            connection.Close()
        End If
        connection.Open()
        comm.ExecuteNonQuery()
        Dim adp As New SqlDataAdapter(comm)
        Dim ds As DataSet = New DataSet()
        adp.Fill(ds, "ShowCourseStudentInfo")
        Dim COUNT__ As Integer = 0
        If ds.Tables(0).Rows.Count > 0 Then
            For i As Integer = 0 To ds.Tables(0).Rows.Count - 1
                TxtRegBranch.Text = ds.Tables(0).Rows(i)(0).ToString()
                TxtRegCourse.Text = ds.Tables(0).Rows(i)(1).ToString()
                TxtStartIn.Text = ds.Tables(0).Rows(i)(2).ToString()
                TxtEndIn.Text = ds.Tables(0).Rows(i)(3).ToString()
                TxtStaut.Text = ds.Tables(0).Rows(i)(4).ToString()
                TxtRegNotes.Text = ds.Tables(0).Rows(i)(5).ToString()
                TxtRegNotes2.Text = ds.Tables(0).Rows(i)(6).ToString()
            Next
        End If
    End Sub
    Private Sub GroupPanel7_Click(sender As Object, e As EventArgs) Handles GroupPanel7.Click

    End Sub

    Private Sub CourseStudentInfo_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Try
            SHOWBRANCHINFO()
        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try
    End Sub
    Private Sub CmdExit_Click(sender As Object, e As EventArgs) Handles CmdExit.Click
        IS_YES = False
        Me.Close()
    End Sub

    Private Sub CmdFindDepart_Click(sender As Object, e As EventArgs) Handles CmdFindDepart.Click
        Try
            Dim w As New NewBranchList
            w.TxtType.Text = 0
            w.TxtIsUse.Text = True
            w.ShowDialog()
            If w.IS_Yes = True Then
                Dim ii As Integer = w.BranchGrid.CurrentRow.Index
                TxtBranchName.Text = w.BranchGrid.Rows(ii).Cells(1).Value.ToString()
                TxtBranchId.Text = w.BranchGrid.Rows(ii).Cells(0).Value.ToString()
                TxtBranchGuid.Text = w.BranchGrid.Rows(ii).Cells(3).Value.ToString()
            End If
        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try
    End Sub

    Private Sub CmdFindCourse_Click(sender As Object, e As EventArgs) Handles CmdFindCourse.Click
        Try
            If TxtBranchGuid.Text <> Nothing Then
                Dim w As New CourseList
                w.TxtBranchGuid.Text = TxtBranchGuid.Text
                w.TxtIsUse.Text = True
                w.ShowDialog()
                If w.IS_Yes = True Then
                    Dim ii As Integer = w.BranchGrid.CurrentRow.Index
                    TxtCourseName.Text = w.BranchGrid.Rows(ii).Cells(6).Value.ToString()
                    TxtCourseGuid.Text = w.BranchGrid.Rows(ii).Cells(5).Value.ToString()
                    TxtDiplomGuid.Text = w.BranchGrid.Rows(ii).Cells(7).Value.ToString()
                End If
            Else
                MessageBox.Show("برجاء اختيار الفرع اولا", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Error)
                Return
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

    Private Sub CmdAccept_Click(sender As Object, e As EventArgs) Handles CmdAccept.Click
        Try
            If TxtBranchGuid.Text = Nothing Then
                MessageBox.Show("برجاء اختيار الفرع ", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Error)
                Return
            Else
                If TxtCourseGuid.Text = Nothing Then
                    MessageBox.Show("برجاء اختيار الدورة", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Error)
                    Return
                Else
                    If TxtCaseGuid.Text = Nothing Then
                        MessageBox.Show("برجاء اختيار الحالة", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Error)
                        Return
                    Else
                        IS_YES = True
                        Me.Close()
                    End If
                End If
            End If
        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try
    End Sub
End Class