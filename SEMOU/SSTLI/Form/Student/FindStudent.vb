Imports System.Data.SqlClient

Public Class FindStudent
    Dim dbcontext As New SemouEntities
    Public IS_Yes As Boolean = False
    Sub LOADINFO()
        BranchGrid.Rows.Clear()
        Dim connection = dbcontext.Database.Connection
        Dim comm As New SqlCommand("FindStudebtData", connection)
        comm.Parameters.AddWithValue("@NationalId", TxtNationalId.Text)
        comm.CommandType = CommandType.StoredProcedure
        If connection.State = ConnectionState.Open Then
            connection.Close()
        End If
        connection.Open()
        comm.ExecuteNonQuery()
        Dim adp As New SqlDataAdapter(comm)
        Dim ds As DataSet = New DataSet()
        adp.Fill(ds, "FindStudebtData")
        Dim COUNT__ As Integer = 0
        If ds.Tables(0).Rows.Count > 0 Then
            BranchGrid.RowCount = ds.Tables(0).Rows.Count
            For i As Integer = 0 To ds.Tables(0).Rows.Count - 1
                BranchGrid.Rows(COUNT__).Cells(0).Value = ds.Tables(0).Rows(i)(0).ToString()
                BranchGrid.Rows(COUNT__).Cells(1).Value = ds.Tables(0).Rows(i)(1).ToString()
                BranchGrid.Rows(COUNT__).Cells(2).Value = ds.Tables(0).Rows(i)(2).ToString()
                BranchGrid.Rows(COUNT__).Cells(3).Value = ds.Tables(0).Rows(i)(3).ToString()
                BranchGrid.Rows(COUNT__).Cells(4).Value = ds.Tables(0).Rows(i)(4).ToString()
                BranchGrid.Rows(COUNT__).Cells(5).Value = ds.Tables(0).Rows(i)(5).ToString()
                COUNT__ = COUNT__ + 1
            Next
        End If
    End Sub
    Private Sub CmdExit_Click(sender As Object, e As EventArgs) Handles CmdExit.Click
        Me.Close()
    End Sub

    Private Sub CmdRefresh_Click(sender As Object, e As EventArgs) Handles CmdRefresh.Click
        Try
            TxtNationalId.Text = Nothing
            BranchGrid.Rows.Clear()
        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try
    End Sub

    Private Sub FindStudent_Load(sender As Object, e As EventArgs) Handles MyBase.Load

    End Sub

    Private Sub TxtNationalId_TextChanged(sender As Object, e As EventArgs) Handles TxtNationalId.TextChanged

    End Sub

    Private Sub TxtNationalId_KeyPress(sender As Object, e As KeyPressEventArgs) Handles TxtNationalId.KeyPress
        Try
            If e.KeyChar = Chr(13) Then
                LOADINFO()
            End If
        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try
    End Sub
End Class