Public Class PaySelect
    Public IS_Yes As Boolean = False
    Private Sub CmdPay1_Click(sender As Object, e As EventArgs) Handles CmdPay1.Click
        TxtPayType.Text = 1
        IS_Yes = True
        Me.Close()
    End Sub

    Private Sub CmdPay3_Click(sender As Object, e As EventArgs) Handles CmdPay3.Click
        TxtPayType.Text = 3
        IS_Yes = True
        Me.Close()
    End Sub
End Class