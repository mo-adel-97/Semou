Imports System.Data.SqlClient

Public Class ChangeUserPassword
    Dim dbcontext As New SemouEntities
    Private Sub CmdShowOldPassWord_Click(sender As Object, e As EventArgs) Handles CmdShowOldPassWord.Click

    End Sub

    Private Sub CmdShowOldPassWord_MouseDown(sender As Object, e As MouseEventArgs) Handles CmdShowOldPassWord.MouseDown
        TxtOldPassWord.PasswordChar = ""
    End Sub

    Private Sub CmdShowNewPassword_Click(sender As Object, e As EventArgs) Handles CmdShowNewPassword.Click

    End Sub

    Private Sub CmdShowNewPassword_MouseDown(sender As Object, e As MouseEventArgs) Handles CmdShowNewPassword.MouseDown
        TxtNewPassword.PasswordChar = ""
    End Sub
    Sub SHOWUSERINFO()
        Dim USERGUID As Guid = Guid.Parse(AllFiles.USER_GUID____.ToString)
        Dim USER_GUID_ = dbcontext.User_Info.FirstOrDefault(Function(m) m.Guid = USERGUID)
        TxtCurrentPassword.Text = USER_GUID_.Password.ToString
    End Sub
    Private Sub ChangeUserPassword_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Try
            SHOWUSERINFO()
        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try
    End Sub

    Private Sub CmdClose_Click(sender As Object, e As EventArgs) Handles CmdClose.Click
        Me.Close()
    End Sub

    Private Sub TxtOldPassWord_TextChanged(sender As Object, e As EventArgs) Handles TxtOldPassWord.TextChanged

    End Sub

    Private Sub TxtOldPassWord_KeyPress(sender As Object, e As KeyPressEventArgs) Handles TxtOldPassWord.KeyPress
        Try
            If e.KeyChar = Chr(13) Then
                If TxtOldPassWord.Text = TxtCurrentPassword.Text Then
                    TxtNewPassword.Enabled = True
                    TxtNewPassword.Focus()
                Else
                    MessageBox.Show("كلمة المرور الحالية غير صحيحة برجاء المحاولة مرة اخري", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Error)
                    TxtOldPassWord.Text = Nothing
                    Return
                End If
            End If
        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try
    End Sub

    Private Sub CmdShowOldPassWord_MouseUp(sender As Object, e As MouseEventArgs) Handles CmdShowOldPassWord.MouseUp
        TxtOldPassWord.PasswordChar = "*"
    End Sub

    Private Sub CmdShowNewPassword_MouseUp(sender As Object, e As MouseEventArgs) Handles CmdShowNewPassword.MouseUp
        TxtNewPassword.PasswordChar = "*"
    End Sub
    Sub UPDATE_PASSWORD_()
        Dim connection = dbcontext.Database.Connection
        Dim comm = New SqlCommand("ChangePassword", connection)
        comm.CommandType = CommandType.StoredProcedure
        comm.Parameters.AddWithValue("@Guid", AllFiles.USER_GUID____)
        comm.Parameters.AddWithValue("@Password", TxtNewPassword.Text)
        If connection.State = ConnectionState.Open Then
            connection.Close()
        End If
        connection.Open()
        comm.ExecuteNonQuery()
        comm.Parameters.Clear()
    End Sub
    Private Sub CmdoK_Click(sender As Object, e As EventArgs) Handles CmdoK.Click
        Try
            If TxtNewPassword.Text <> Nothing Then
                UPDATE_PASSWORD_()
                MessageBox.Show("تم تغيير كلمة المرور بنجاح", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Information)
                Me.Close()
            Else
                MessageBox.Show("برجاء ادخال كلمة المرور الجديدة", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Information)
                Return
            End If
        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try
    End Sub
End Class