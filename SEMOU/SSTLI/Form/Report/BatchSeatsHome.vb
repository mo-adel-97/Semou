Imports System.Data.SqlClient

Public Class BatchSeatsHome
    Dim dbcontext As New SemouEntities
    Private Sub CmdExit_Click(sender As Object, e As EventArgs) Handles CmdExit.Click
        Me.Close()
    End Sub
    Sub LOADBATCH()
        BranchGrid.Rows.Clear()
        Dim NAME As String = Nothing
        Dim ISUSE As String = Nothing
        Dim connection = dbcontext.Database.Connection
        Dim comm As New SqlCommand("ShowSeatsOnBatch", connection)
        comm.Parameters.AddWithValue("@BranchGuid", TxtBranchGuid.Text)
        If TxtBatchGuid.Text <> Nothing Then
            comm.Parameters.AddWithValue("@BatchGuid", TxtBatchGuid.Text)
        End If
        comm.CommandType = CommandType.StoredProcedure
        If connection.State = ConnectionState.Open Then
            connection.Close()
        End If
        connection.Open()
        comm.ExecuteNonQuery()
        Dim adp As New SqlDataAdapter(comm)
        Dim ds As DataSet = New DataSet()
        adp.Fill(ds, "ShowSeatsOnBatch")
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
    Sub LOADSEATSDETAIL()
        SeatsGrid.Rows.Clear()
        Dim connection = dbcontext.Database.Connection
        Dim comm As New SqlCommand("ShowBatchSeatsDetails", connection)
        comm.Parameters.AddWithValue("@BranchGuid", TxtBranchGuid.Text)
        comm.Parameters.AddWithValue("@BatchGuid", TxtSelectedSeats.Text)
        comm.CommandType = CommandType.StoredProcedure
        If connection.State = ConnectionState.Open Then
            connection.Close()
        End If
        connection.Open()
        comm.ExecuteNonQuery()
        Dim adp As New SqlDataAdapter(comm)
        Dim ds As DataSet = New DataSet()
        adp.Fill(ds, "ShowBatchSeatsDetails")
        Dim COUNT__ As Integer = 0
        If ds.Tables(0).Rows.Count > 0 Then
            SeatsGrid.RowCount = ds.Tables(0).Rows.Count
            For i As Integer = 0 To ds.Tables(0).Rows.Count - 1
                SeatsGrid.Rows(COUNT__).Cells(0).Value = ds.Tables(0).Rows(i)(0).ToString()
                SeatsGrid.Rows(COUNT__).Cells(1).Value = ds.Tables(0).Rows(i)(2).ToString()
                SeatsGrid.Rows(COUNT__).Cells(2).Value = ds.Tables(0).Rows(i)(1).ToString()
                SeatsGrid.Rows(COUNT__).Cells(3).Value = Val(ds.Tables(0).Rows(i)(2)) - Val(ds.Tables(0).Rows(i)(1))
                COUNT__ = COUNT__ + 1
            Next
        End If
    End Sub
    Private Sub CmdFindDepart_Click(sender As Object, e As EventArgs) Handles CmdFindDepart.Click
        Try
            AllFiles.CHEKDEFBRANCHINFO(AllFiles.USER_GUID____.ToString)
            Dim w As New NewBranchList
            'If AllFiles.DEFBRANCH = True Then
            '    w.TxtType.Text = 1
            '    w.TxtUserGuid.Text = AllFiles.USER_GUID____.ToString
            '    w.TxtFind.ReadOnly = True
            'Else
            w.TxtType.Text = 0
            w.TxtIsUse.Text = True
            w.TxtFind.ReadOnly = False
            'End If
            w.ShowDialog()
            If w.IS_Yes = True Then
                Dim ii As Integer = w.BranchGrid.CurrentRow.Index
                TxtBranchName.Text = w.BranchGrid.Rows(ii).Cells(1).Value.ToString()
                TxtBranchGuid.Text = w.BranchGrid.Rows(ii).Cells(3).Value.ToString()
            End If
            If TxtBranchGuid.Text <> Nothing Then
                LOADBATCH()
                SeatsGrid.Rows.Clear()
            End If
        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try
    End Sub

    Private Sub CmdFindBatch_Click(sender As Object, e As EventArgs) Handles CmdFindBatch.Click
        Try
            If TxtBranchGuid.Text <> Nothing Then
                Dim w As New BatchList
                w.TxtIsUse.Text = True
                w.ShowDialog()
                If w.IS_Yes = True Then
                    Dim ii As Integer = w.BranchGrid.CurrentRow.Index
                    TxtBatchName.Text = w.BranchGrid.Rows(ii).Cells(1).Value.ToString()
                    TxtBatchGuid.Text = w.BranchGrid.Rows(ii).Cells(3).Value.ToString()
                End If
                If TxtBatchGuid.Text <> Nothing Then
                    LOADBATCH()
                End If
            Else
                MessageBox.Show("برجاء اختيار الفرع اولا", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Error)
                Return
            End If
        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try
    End Sub

    Private Sub CmdDelSalesMan_Click(sender As Object, e As EventArgs) Handles CmdDelSalesMan.Click
        Try
            TxtBatchName.Text = Nothing
            TxtBatchGuid.Text = Nothing
            If TxtBranchGuid.Text <> Nothing Then
                LOADBATCH()
            End If
        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try
    End Sub

    Private Sub BranchGrid_CellContentClick(sender As Object, e As DataGridViewCellEventArgs) Handles BranchGrid.CellContentClick

    End Sub

    Private Sub BranchGrid_CellClick(sender As Object, e As DataGridViewCellEventArgs) Handles BranchGrid.CellClick
        Try
            Dim i As Integer = BranchGrid.CurrentCell.RowIndex
            Dim j As Integer = BranchGrid.CurrentCell.ColumnIndex
            TxtSelectedSeats.Text = BranchGrid.Rows(i).Cells(5).Value.ToString
            LOADSEATSDETAIL()
        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try
    End Sub
End Class