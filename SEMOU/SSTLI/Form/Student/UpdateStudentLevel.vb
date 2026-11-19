Imports System.Data.SqlClient

Public Class UpdateStudentLevel
    Dim dbcontext As New SemouEntities
    Public IS_Yes As Boolean = False
    Private Sub CmdExit_Click(sender As Object, e As EventArgs) Handles CmdExit.Click
        Me.Close()
    End Sub

    Private Sub GroupPanel2_Click(sender As Object, e As EventArgs) Handles GroupPanel2.Click

    End Sub

    Private Sub CmdFindLevel_Click(sender As Object, e As EventArgs) Handles CmdFindLevel.Click
        Try
            Dim w As New LevelList
            w.TxtIsUse.Text = True
            w.ShowDialog()
            If w.IS_Yes = True Then
                Dim ii As Integer = w.BranchGrid.CurrentRow.Index
                TxtLevelName.Text = w.BranchGrid.Rows(ii).Cells(1).Value.ToString()
                TxtLevelGuid.Text = w.BranchGrid.Rows(ii).Cells(3).Value.ToString()
            End If
        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try
    End Sub
    Sub UPDATELEVEL()
        Dim connection = dbcontext.Database.Connection
        Dim comm = New SqlCommand("UpdateStudentLevel", connection)
        comm.CommandType = CommandType.StoredProcedure
        comm.Parameters.AddWithValue("@StudentGuid", TxtAccountGuid.Text)
        comm.Parameters.AddWithValue("@BatchGuid", TxtBatchGuid.Text)
        comm.Parameters.AddWithValue("@DiplomGuid", TxtDiplomGuid.Text)
        comm.Parameters.AddWithValue("@BranchGuid", TxtBranchGuid.Text)
        comm.Parameters.AddWithValue("@LevelGuid", TxtLevelGuid.Text)
        If connection.State = ConnectionState.Open Then
            connection.Close()
        End If
        connection.Open()
        comm.ExecuteNonQuery()
        comm.Parameters.Clear()
    End Sub
    Private Sub CmdAccept_Click(sender As Object, e As EventArgs) Handles CmdAccept.Click
        Try
            If TxtLevelGuid.Text <> Nothing Then
                UPDATELEVEL()
                MessageBox.Show("تم ترحيل الطالب بنجاح", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Information)
                IS_Yes = True
                Me.Close()
            Else
                MessageBox.Show("برجاء اختيار المستوي الجديد", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Error)
                Return
            End If
        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try
    End Sub
End Class