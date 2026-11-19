Imports System.Data.SqlClient

Public Class CourseHome
    Dim dbcontext As New SemouEntities
    Private Sub CmdExit_Click(sender As Object, e As EventArgs) Handles CmdExit.Click
        Me.Close()
    End Sub
    Sub LOADCOURSE()
        StudentGrid.Rows.Clear()
        Dim connection = dbcontext.Database.Connection
        Dim comm As New SqlCommand("AllCourse", connection)
        If TxtBranchGuid.Text <> Nothing Then
            comm.Parameters.AddWithValue("@BranchGuid", TxtBranchGuid.Text)
            StudentGrid.Columns(8).Visible = False
        Else
            StudentGrid.Columns(8).Visible = True
        End If
        If TxtDiplomGuid.Text <> Nothing Then
            comm.Parameters.AddWithValue("@DiplomGuid", TxtDiplomGuid.Text)
            StudentGrid.Columns(7).Visible = False
        Else
            StudentGrid.Columns(7).Visible = True
        End If
        If CboType.SelectedIndex <> 0 Then
            If CboType.SelectedIndex = 1 Then
                comm.Parameters.AddWithValue("@Staut", True)
            ElseIf CboType.SelectedIndex = 2 Then
                comm.Parameters.AddWithValue("@Staut", False)
            End If
        End If
        comm.CommandType = CommandType.StoredProcedure
        If connection.State = ConnectionState.Open Then
            connection.Close()
        End If
        connection.Open()
        comm.ExecuteNonQuery()
        Dim adp As New SqlDataAdapter(comm)
        Dim ds As DataSet = New DataSet()
        adp.Fill(ds, "AllCourse")
        Dim COUNT__ As Integer = 0
        If ds.Tables(0).Rows.Count > 0 Then
            StudentGrid.RowCount = ds.Tables(0).Rows.Count
            For i As Integer = 0 To ds.Tables(0).Rows.Count - 1
                StudentGrid.Rows(COUNT__).Cells(0).Value = ds.Tables(0).Rows(i)(0).ToString()
                StudentGrid.Rows(COUNT__).Cells(1).Value = ds.Tables(0).Rows(i)(1).ToString()
                StudentGrid.Rows(COUNT__).Cells(2).Value = ds.Tables(0).Rows(i)(2).ToString()
                StudentGrid.Rows(COUNT__).Cells(3).Value = ds.Tables(0).Rows(i)(3).ToString()
                StudentGrid.Rows(COUNT__).Cells(4).Value = ds.Tables(0).Rows(i)(4).ToString()
                StudentGrid.Rows(COUNT__).Cells(5).Value = ds.Tables(0).Rows(i)(5).ToString()
                StudentGrid.Rows(COUNT__).Cells(6).Value = ds.Tables(0).Rows(i)(6).ToString()
                StudentGrid.Rows(COUNT__).Cells(7).Value = ds.Tables(0).Rows(i)(7).ToString()
                StudentGrid.Rows(COUNT__).Cells(8).Value = ds.Tables(0).Rows(i)(8).ToString()
                COUNT__ = COUNT__ + 1
            Next
        End If
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
                LOADCOURSE()
            End If
        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try
    End Sub

    Private Sub CmdFindDiplom_Click(sender As Object, e As EventArgs) Handles CmdFindDiplom.Click
        Try
            If TxtBranchGuid.Text <> Nothing Then
                Dim w As New DiplomList
                w.TxtIsUse.Text = True
                w.TxtStudyType.Text = 1
                w.ShowDialog()
                If w.IS_Yes = True Then
                    Dim ii As Integer = w.BranchGrid.CurrentRow.Index
                    TxtDiplomName.Text = w.BranchGrid.Rows(ii).Cells(1).Value.ToString()
                    TxtDiplomGuid.Text = w.BranchGrid.Rows(ii).Cells(3).Value.ToString()
                End If
                If TxtDiplomGuid.Text <> Nothing Then
                    If TxtBranchGuid.Text <> Nothing Then
                        LOADCOURSE()
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

    Private Sub CmdView_Click(sender As Object, e As EventArgs) Handles CmdView.Click
        Try
            If TxtBranchGuid.Text <> Nothing Then
                LOADCOURSE()
            Else
                MessageBox.Show("برجاء اختيار الفرع اولا", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Error)
                Return
            End If
        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try
    End Sub

    Private Sub CmdDelBranch_Click(sender As Object, e As EventArgs) Handles CmdDelBranch.Click
        TxtBranchGuid.Text = Nothing
        TxtBranchName.Text = Nothing
        StudentGrid.Rows.Clear()
        'LOADCOURSE()
    End Sub

    Private Sub CmdDelDiplom_Click(sender As Object, e As EventArgs) Handles CmdDelDiplom.Click
        TxtDiplomGuid.Text = Nothing
        TxtDiplomName.Text = Nothing
        If TxtBranchGuid.Text <> Nothing Then
            LOADCOURSE()
        Else
            StudentGrid.Rows.Clear()
        End If
    End Sub

    Private Sub CboType_SelectedIndexChanged(sender As Object, e As EventArgs) Handles CboType.SelectedIndexChanged

    End Sub

    Private Sub CboType_SelectionChangeCommitted(sender As Object, e As EventArgs) Handles CboType.SelectionChangeCommitted
        If TxtBranchGuid.Text <> Nothing Then
            LOADCOURSE()
        Else
            MessageBox.Show("برجاء اختيار الفرع اولا", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return
        End If
    End Sub

    Private Sub CboType_SelectedValueChanged(sender As Object, e As EventArgs) Handles CboType.SelectedValueChanged

    End Sub

    Private Sub StudentGrid_CellContentClick(sender As Object, e As DataGridViewCellEventArgs) Handles StudentGrid.CellContentClick

    End Sub

    Private Sub StudentGrid_CellClick(sender As Object, e As DataGridViewCellEventArgs) Handles StudentGrid.CellClick
        Try
            Dim i As Integer = StudentGrid.CurrentCell.RowIndex
            Dim j As Integer = StudentGrid.CurrentCell.ColumnIndex
            If j = 9 Then
                Dim w As New EncodingCourse
                w.TxtCode.Text = StudentGrid.Rows(i).Cells(0).Value.ToString
                w.SHOWCOURSEINFO()
                w.ShowDialog()
            End If
        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try
    End Sub

    Private Sub CourseHome_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Try
            'AllFiles.CHEKDEFBRANCHINFO(AllFiles.USER_GUID____.ToString)
            'If AllFiles.DEFBRANCH = True Then
            '    CmdFindDepart.Enabled = False
            '    CmdDelBranch.Enabled = False
            '    TxtBranchName.Text = AllFiles.DEFBRANCHNAME
            '    TxtBranchGuid.Text = AllFiles.DEFBRANCHGUID.ToString()
            '    If TxtBranchGuid.Text <> Nothing Then
            '        LOADCOURSE()
            '    End If
            'Else
            '    CmdFindDepart.Enabled = True
            '    CmdDelBranch.Enabled = True
            '    TxtBranchName.Text = Nothing
            '    TxtBranchGuid.Text = Nothing
            'End If
        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try
    End Sub
End Class