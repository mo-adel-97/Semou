Public Class UpdateResaon
    Public IS_Yes As Boolean = False
    Private Sub CmdClose_Click(sender As Object, e As EventArgs) Handles CmdClose.Click
        Me.Close()
    End Sub

    Private Sub TxtNotes_TextChanged(sender As Object, e As EventArgs) Handles TxtNotes.TextChanged

    End Sub

    Private Sub CmdOk_Click(sender As Object, e As EventArgs) Handles CmdOk.Click
        Try
            If TxtNotes.Text.Length = 0 Then
                MessageBox.Show("برجاء ادخال سبب الاجراء اولا", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Error)
                Return
            Else
                IS_Yes = True
                Me.Close()
            End If
        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try
    End Sub
End Class