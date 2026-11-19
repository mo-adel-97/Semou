Imports System.Data.SqlClient

Public Class SellerList
    Dim dbcontext As New SSTLI2023Entities
    Public IS_Yes As Boolean = False
    Sub LOADSELLER()
        Dim connection = dbcontext.Database.Connection
        Dim comm As New SqlCommand("LoadSeller", connection)
        comm.CommandType = CommandType.StoredProcedure
        If connection.State = ConnectionState.Open Then
            connection.Close()
        End If
        connection.Open()
        comm.ExecuteNonQuery()
        Dim adp As New SqlDataAdapter(comm)
        Dim ds As DataSet = New DataSet()
        adp.Fill(ds, "LoadSeller")
        Dim COUNT__ As Integer = 0
        If ds.Tables(0).Rows.Count > 0 Then
            SellerGrid.RowCount = ds.Tables(0).Rows.Count
            For i As Integer = 0 To ds.Tables(0).Rows.Count - 1
                SellerGrid.Rows(COUNT__).Cells(0).Value = ds.Tables(0).Rows(i)(0).ToString()
                SellerGrid.Rows(COUNT__).Cells(1).Value = ds.Tables(0).Rows(i)(1).ToString()
                COUNT__ = COUNT__ + 1
            Next
        End If
    End Sub
    Private Sub SellerList_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        LOADSELLER()
    End Sub

    Private Sub SellerGrid_CellContentClick(sender As Object, e As DataGridViewCellEventArgs) Handles SellerGrid.CellContentClick

    End Sub

    Private Sub SellerGrid_CellClick(sender As Object, e As DataGridViewCellEventArgs) Handles SellerGrid.CellClick
        IS_Yes = True
        Me.Close()
    End Sub
End Class