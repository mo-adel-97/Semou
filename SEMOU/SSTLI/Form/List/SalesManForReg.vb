Imports System.Data.SqlClient

Public Class SalesManForReg
    Dim dbcontext As New SemouEntities
    Public IS_Yes As Boolean = False
    Sub LOADSALESMAN()
        BranchGrid.Rows.Clear()
        Dim connection = dbcontext.Database.Connection
        Dim comm As New SqlCommand("ViewSalesManForReg", connection)
        comm.Parameters.AddWithValue("@BranchGuid", TxtBranchGuid.Text)
        If TxtFind.Text <> Nothing Then
            comm.Parameters.AddWithValue("@Name", TxtFind.Text)
        End If
        comm.CommandType = CommandType.StoredProcedure
        If connection.State = ConnectionState.Open Then
            connection.Close()
        End If
        connection.Open()
        comm.ExecuteNonQuery()
        Dim adp As New SqlDataAdapter(comm)
        Dim ds As DataSet = New DataSet()
        adp.Fill(ds, "ViewSalesManForReg")
        Dim COUNT__ As Integer = 0
        If ds.Tables(0).Rows.Count > 0 Then
            BranchGrid.RowCount = ds.Tables(0).Rows.Count
            For i As Integer = 0 To ds.Tables(0).Rows.Count - 1
                BranchGrid.Rows(COUNT__).Cells(0).Value = ds.Tables(0).Rows(i)(0).ToString()
                BranchGrid.Rows(COUNT__).Cells(1).Value = ds.Tables(0).Rows(i)(1).ToString()
                BranchGrid.Rows(COUNT__).Cells(2).Value = ds.Tables(0).Rows(i)(2).ToString()
                COUNT__ = COUNT__ + 1
            Next
        End If
    End Sub
    Private Sub SalesManForReg_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Try
            LOADSALESMAN()
        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try
    End Sub

    Private Sub TxtFind_TextChanged(sender As Object, e As EventArgs) Handles TxtFind.TextChanged
        Try
            LOADSALESMAN()
        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try
    End Sub

    Private Sub BranchGrid_CellContentClick(sender As Object, e As DataGridViewCellEventArgs) Handles BranchGrid.CellContentClick

    End Sub

    Private Sub BranchGrid_CellDoubleClick(sender As Object, e As DataGridViewCellEventArgs) Handles BranchGrid.CellDoubleClick
        IS_Yes = True
        Me.Close()
    End Sub
End Class