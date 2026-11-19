Imports System.Data.SqlClient

Public Class AddStudentOtherFess
    Dim dbcontext As New SemouEntities
    Public IS_Yes As Boolean = False
    Sub SHOWLISTFEES()
        MainFeesGrid.Rows.Clear()
        Dim connection = dbcontext.Database.Connection
        Dim comm As New SqlCommand("ShowPriceListDiplom", connection)
        comm.Parameters.AddWithValue("@PriceListGuid", TxtPriceListGuid.Text)
        comm.Parameters.AddWithValue("@Type", 1)
        'If TxtFindFees.Text <> Nothing Then
        '    comm.Parameters.AddWithValue("@DiplomName", TxtFindFees.Text)
        'End If
        comm.CommandType = CommandType.StoredProcedure
        If connection.State = ConnectionState.Open Then
            connection.Close()
        End If
        connection.Open()
        comm.ExecuteNonQuery()
        Dim adp As New SqlDataAdapter(comm)
        Dim ds As DataSet = New DataSet()
        adp.Fill(ds, "ShowPriceListDiplom")
        Dim COUNT__ As Integer = 0
        If ds.Tables(0).Rows.Count > 0 Then
            MainFeesGrid.RowCount = ds.Tables(0).Rows.Count
            For i As Integer = 0 To ds.Tables(0).Rows.Count - 1
                MainFeesGrid.Rows(COUNT__).Cells(0).Value = ds.Tables(0).Rows(i)(0).ToString()
                MainFeesGrid.Rows(COUNT__).Cells(1).Value = ds.Tables(0).Rows(i)(1).ToString()
                MainFeesGrid.Rows(COUNT__).Cells(2).Value = ds.Tables(0).Rows(i)(2).ToString()
                MainFeesGrid.Rows(COUNT__).Cells(3).Value = ds.Tables(0).Rows(i)(4).ToString()
                MainFeesGrid.Rows(COUNT__).Cells(4).Value = ds.Tables(0).Rows(i)(5).ToString()
                MainFeesGrid.Rows(COUNT__).Cells(5).Value = ds.Tables(0).Rows(i)(6).ToString()
                COUNT__ = COUNT__ + 1
            Next
        End If
    End Sub
    Private Sub AddStudentOtherFess_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Try
            SHOWLISTFEES()
        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try
    End Sub

    Private Sub MainFeesGrid_CellContentClick(sender As Object, e As DataGridViewCellEventArgs) Handles MainFeesGrid.CellContentClick

    End Sub

    Private Sub MainFeesGrid_CellDoubleClick(sender As Object, e As DataGridViewCellEventArgs) Handles MainFeesGrid.CellDoubleClick
        IS_Yes = True
        Me.Close()
    End Sub
End Class