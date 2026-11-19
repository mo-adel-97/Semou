Imports System.Data.SqlClient

Public Class AddStudentLevelNotes
    Dim dbcontext As New SemouEntities
    Public IS_Yes As Boolean = False
    Sub ADDNOTES()
        Dim connection = dbcontext.Database.Connection
        Dim comm = New SqlCommand("AddStudentLevelNotes", connection)
        comm.CommandType = CommandType.StoredProcedure
        comm.Parameters.AddWithValue("@StudentGuid", TxtAccountGuid.Text)
        comm.Parameters.AddWithValue("@DiplomGuid", TxtDiplomGuid.Text)
        comm.Parameters.AddWithValue("@BranchGuid", TxtBranchGuid.Text)
        comm.Parameters.AddWithValue("@LevelNotes", TxtNotes.Text)
        If connection.State = ConnectionState.Open Then
            connection.Close()
        End If
        connection.Open()
        comm.ExecuteNonQuery()
        comm.Parameters.Clear()
    End Sub
    Private Sub CmdAccept_Click(sender As Object, e As EventArgs) Handles CmdAccept.Click
        Try
            If TxtNotes.Text <> Nothing Then
                ADDNOTES()
                MessageBox.Show("تم أضافة الملاحظة بنجاح", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Information)
                IS_Yes = True
                Me.Close()
            Else
                MessageBox.Show("برجاء ادخال الملاحظة المراد اضافتها", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Error)
                Return
            End If
        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try
    End Sub

    Private Sub CmdExit_Click(sender As Object, e As EventArgs) Handles CmdExit.Click
        Me.Close()
    End Sub
End Class