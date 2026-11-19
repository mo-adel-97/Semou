Public Class TransAllDiplomStudent
    Public IS_Yes As Boolean = False
    Private Sub CmdExit_Click(sender As Object, e As EventArgs) Handles CmdExit.Click
        Me.Close()
    End Sub

    Private Sub ChkUpdate1_CheckedChanged(sender As Object, e As EventArgs) Handles ChkUpdate1.CheckedChanged
        Try
            If ChkUpdate1.Checked = True Then
                CmdFindBranch1.Enabled = True
                TxtBranch1Name.Text = Nothing
                TxtBranch1Guid.Text = Nothing
            Else
                CmdFindBranch1.Enabled = False
                TxtBranch1Name.Text = Nothing
                TxtBranch1Guid.Text = "00000000-0000-0000-0000-000000000000"
            End If
        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try
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
                TxtBranch1Guid.Text = w.BranchGrid.Rows(ii).Cells(3).Value.ToString()
            End If
        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try
    End Sub

    Private Sub CmdSave_Click(sender As Object, e As EventArgs) Handles CmdSave.Click
        IS_Yes = True
        Me.Close()
    End Sub

    Private Sub ChkUpdate2_CheckedChanged(sender As Object, e As EventArgs) Handles ChkUpdate2.CheckedChanged
        Try
            If ChkUpdate2.Checked = True Then
                CmdFindBranch2.Enabled = True
                TxtBranch2Name.Text = Nothing
                TxtBranch2Guid.Text = Nothing
            Else
                CmdFindBranch2.Enabled = False
                TxtBranch2Name.Text = Nothing
                TxtBranch2Guid.Text = "00000000-0000-0000-0000-000000000000"
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
                TxtBranch2Guid.Text = w.BranchGrid.Rows(ii).Cells(3).Value.ToString()
            End If
        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try
    End Sub
End Class