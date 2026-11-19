Imports System.Data.SqlClient

Public Class ChangeStudentStaut
    Dim dbcontext As New SemouEntities
    Public IS_Yes As Boolean = False
    Private Sub CmdExit_Click(sender As Object, e As EventArgs) Handles CmdExit.Click
        Me.Close()
    End Sub
    Sub CHANGESTAUT()
        Dim connection = dbcontext.Database.Connection
        Dim comm = New SqlCommand("ChangeStudentStaut", connection)
        comm.CommandType = CommandType.StoredProcedure
        comm.Parameters.AddWithValue("@AccountGuid", TxtGuid.Text)
        comm.Parameters.AddWithValue("@IsUse", ChkStaut.Value)
        If connection.State = ConnectionState.Open Then
            connection.Close()
        End If
        connection.Open()
        comm.ExecuteNonQuery()
        comm.Parameters.Clear()
    End Sub

    Private Sub ChangeStudentStaut_Load(sender As Object, e As EventArgs) Handles MyBase.Load

    End Sub

    Private Sub CmdSave_Click(sender As Object, e As EventArgs) Handles CmdSave.Click
        Try
            If AllFiles.FORUPDATE("addstudent") = False Then
                Return
            Else
                CHANGESTAUT()
                MessageBox.Show("تم تغيير حالة الطالب بنجاح", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Information)
                IS_Yes = True
                Me.Close()
            End If
        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try
    End Sub
End Class