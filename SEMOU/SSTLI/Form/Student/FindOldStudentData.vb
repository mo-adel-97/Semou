Imports System.Data.SqlClient

Public Class FindOldStudentData
    Dim Olddbcontext As New SSTLI2023Entities
    Sub LOADOLDSTUEDENT()
        OldDataGrid.Rows.Clear()
        Dim connection = Olddbcontext.Database.Connection
        Dim comm As New SqlCommand("FindStudentData", connection)
        comm.Parameters.AddWithValue("@PROOFNO", TxtNationalId.Text)
        comm.CommandType = CommandType.StoredProcedure
        If connection.State = ConnectionState.Open Then
            connection.Close()
        End If
        connection.Open()
        comm.ExecuteNonQuery()
        Dim adp As New SqlDataAdapter(comm)
        Dim ds As DataSet = New DataSet()
        adp.Fill(ds, "FindStudentData")
        Dim COUNT__ As Integer = 0
        If ds.Tables(0).Rows.Count > 0 Then
            OldDataGrid.RowCount = ds.Tables(0).Rows.Count
            For i As Integer = 0 To ds.Tables(0).Rows.Count - 1
                OldDataGrid.Rows(COUNT__).Cells(0).Value = ds.Tables(0).Rows(i)(0).ToString()
                OldDataGrid.Rows(COUNT__).Cells(1).Value = ds.Tables(0).Rows(i)(1).ToString()
                OldDataGrid.Rows(COUNT__).Cells(2).Value = ds.Tables(0).Rows(i)(2).ToString()
                OldDataGrid.Rows(COUNT__).Cells(3).Value = ds.Tables(0).Rows(i)(3).ToString()
                OldDataGrid.Rows(COUNT__).Cells(4).Value = ds.Tables(0).Rows(i)(4).ToString()
                OldDataGrid.Rows(COUNT__).Cells(5).Value = ds.Tables(0).Rows(i)(5).ToString()
                OldDataGrid.Rows(COUNT__).Cells(6).Value = ds.Tables(0).Rows(i)(6).ToString()
                OldDataGrid.Rows(COUNT__).Cells(8).Value = ds.Tables(0).Rows(i)(7).ToString()
                COUNT__ = COUNT__ + 1
            Next
        End If
    End Sub
    Private Sub FindOldStudentData_Load(sender As Object, e As EventArgs) Handles MyBase.Load

    End Sub

    Private Sub OldDataGrid_CellContentClick(sender As Object, e As DataGridViewCellEventArgs) Handles OldDataGrid.CellContentClick

    End Sub

    Private Sub OldDataGrid_CellClick(sender As Object, e As DataGridViewCellEventArgs) Handles OldDataGrid.CellClick
        Try
            Dim i As Integer = OldDataGrid.CurrentCell.RowIndex
            Dim j As Integer = OldDataGrid.CurrentCell.ColumnIndex
            If j = 9 Then
                Dim w As New OldStudentStatment
                w.TxtStudentName.Text = OldDataGrid.Rows(i).Cells(0).Value.ToString
                w.LOADOLDSTATMENT(OldDataGrid.Rows(i).Cells(6).Value.ToString, OldDataGrid.Rows(i).Cells(5).Value.ToString)
                w.LOADOLDREG(OldDataGrid.Rows(i).Cells(6).Value.ToString, OldDataGrid.Rows(i).Cells(5).Value.ToString)
                w.LOADOPENBALANCE(OldDataGrid.Rows(i).Cells(6).Value.ToString, OldDataGrid.Rows(i).Cells(5).Value.ToString)
                w.LASTTOTAL()
                w.ShowDialog()
            ElseIf j = 7 Then
                Dim w As New IfStudentNotRegister
                w.TxtAction.Text = 1
                w.TxtNotes.Text = OldDataGrid.Rows(i).Cells(4).Value.ToString
                w.TxtOldCustNo.Text = OldDataGrid.Rows(i).Cells(6).Value.ToString
                w.TxtAccountGuid.Text = TxtAccountGuid.Text
                w.SHOWBALANCE(OldDataGrid.Rows(i).Cells(5).Value.ToString())
                w.ShowDialog()

            End If
        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try
    End Sub
End Class