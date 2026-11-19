Public Class ChooseRoom
    Public IS_Yes As Boolean = False
    Private Sub ChooseRoom_Load(sender As Object, e As EventArgs) Handles MyBase.Load

    End Sub

    Private Sub CmdOk_Click(sender As Object, e As EventArgs) Handles CmdOk.Click
        IS_Yes = True
        Me.Close()
    End Sub
End Class