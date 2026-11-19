Public Class NewCallType
    Public IS_Yes As Boolean = False
    Private Sub NewCallType_Load(sender As Object, e As EventArgs) Handles MyBase.Load

    End Sub

    Private Sub CmdOk_Click(sender As Object, e As EventArgs) Handles CmdOk.Click
        Try
            If CboTybe.Text <> Nothing Then
                IS_Yes = True
                Me.Close()
            Else
                MessageBox.Show("برجاء اختيار نوع الاتصال اولا", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Error)
                Return
            End If
        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try
    End Sub
End Class