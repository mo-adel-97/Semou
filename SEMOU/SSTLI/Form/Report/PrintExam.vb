Public Class PrintExam
    Dim dbcontext As New SemouEntities
    Public IS_Yes As Boolean = False
    Private Sub CmdFindExam_Click(sender As Object, e As EventArgs) Handles CmdFindExam.Click
        Try
            Dim w As New ExamList
            w.TxtDiplomGuid.Text = TxtDiplomGuid.Text
            w.TxtLevelGuid.Text = TxtLevelGuid.Text
            w.ChkDiplom.Enabled = False
            w.ChkLevel.Enabled = False
            w.ChkBook.Checked = True
            w.ShowDialog()
            If w.IS_Yes = True Then
                Dim ii As Integer = w.BranchGrid.CurrentRow.Index
                TxtCode.Text = w.BranchGrid.Rows(ii).Cells(3).Value.ToString()
                TxtExamGuid.Text = w.BranchGrid.Rows(ii).Cells(5).Value.ToString()
            End If
        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try
    End Sub

    Private Sub CmdOK_Click(sender As Object, e As EventArgs) Handles CmdOK.Click
        Try
            If TxtExamGuid.Text <> Nothing Then
                IS_Yes = True
                Me.Close()
            Else
                MessageBox.Show("لم تقم باختيار اختبار ولذلك لن تتم عملية الطباعه", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Me.Close()
            End If
        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try
    End Sub
End Class