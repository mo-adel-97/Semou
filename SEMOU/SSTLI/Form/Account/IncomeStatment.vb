Imports System.Data.SqlClient

Public Class IncomeStatment
    Dim dbcontext As New SemouEntities
    Sub INCOMETOTAL()
        Dim MADEN As Double = 0
        For i As Integer = 0 To IncomeGrid.Rows.Count - 1
            If IncomeGrid.Rows(i).Cells(1).Value <> Nothing Then
                MADEN = Val(MADEN) + (Val(IncomeGrid.Rows(i).Cells(2).Value))
            End If
        Next
        TxtIncomeTotal.Text = Math.Round(Val(MADEN), 2)
    End Sub
    Sub LOADINCOME()
        IncomeGrid.Rows.Clear()
        Dim connection = dbcontext.Database.Connection
        Dim comm As New SqlCommand("ShowIncomeStatment", connection)
        comm.Parameters.AddWithValue("@FDate", Convert.ToDateTime(TxtFromDate.Value))
        comm.Parameters.AddWithValue("@TDate", Convert.ToDateTime(TxtToDate.Value))
        comm.Parameters.AddWithValue("@Type_", 0)
        If TxtBranchGuid.Text <> Nothing Then
            comm.Parameters.AddWithValue("@CostGuid", TxtBranchGuid.Text)
        End If
        comm.CommandType = CommandType.StoredProcedure
        If connection.State = ConnectionState.Open Then
            connection.Close()
        End If
        connection.Open()
        comm.ExecuteNonQuery()
        Dim adp As New SqlDataAdapter(comm)
        Dim ds As DataSet = New DataSet()
        adp.Fill(ds, "ShowIncomeStatment")
        Dim COUNT__ As Integer = 0
        If ds.Tables(0).Rows.Count > 0 Then
            IncomeGrid.RowCount = ds.Tables(0).Rows.Count
            For i As Integer = 0 To ds.Tables(0).Rows.Count - 1
                Dim TOTAL As Double = Val(ds.Tables(0).Rows(i)(2)) - Val(ds.Tables(0).Rows(i)(3))
                IncomeGrid.Rows(COUNT__).Cells(0).Value = ds.Tables(0).Rows(i)(0).ToString()
                IncomeGrid.Rows(COUNT__).Cells(1).Value = ds.Tables(0).Rows(i)(1).ToString()
                IncomeGrid.Rows(COUNT__).Cells(2).Value = Math.Abs(Math.Round(TOTAL, 2))
                COUNT__ = COUNT__ + 1
            Next
        End If
        INCOMETOTAL()
    End Sub
    Sub EXPENSETOTAL()
        Dim MADEN As Double = 0
        For i As Integer = 0 To ExpenseGrid.Rows.Count - 1
            If ExpenseGrid.Rows(i).Cells(1).Value <> Nothing Then
                MADEN = Val(MADEN) + (Val(ExpenseGrid.Rows(i).Cells(2).Value))
            End If
        Next
        TxtExpensTotal.Text = Math.Round(Val(MADEN), 2)
    End Sub
    Sub LOADEXPENS()
        ExpenseGrid.Rows.Clear()
        Dim connection = dbcontext.Database.Connection
        Dim comm As New SqlCommand("ShowIncomeStatment", connection)
        comm.Parameters.AddWithValue("@FDate", Convert.ToDateTime(TxtFromDate.Value))
        comm.Parameters.AddWithValue("@TDate", Convert.ToDateTime(TxtToDate.Value))
        comm.Parameters.AddWithValue("@Type_", 1)
        If TxtBranchGuid.Text <> Nothing Then
            comm.Parameters.AddWithValue("@CostGuid", TxtBranchGuid.Text)
        End If
        comm.CommandType = CommandType.StoredProcedure
        If connection.State = ConnectionState.Open Then
            connection.Close()
        End If
        connection.Open()
        comm.ExecuteNonQuery()
        Dim adp As New SqlDataAdapter(comm)
        Dim ds As DataSet = New DataSet()
        adp.Fill(ds, "ShowIncomeStatment")
        Dim COUNT__ As Integer = 0
        If ds.Tables(0).Rows.Count > 0 Then
            ExpenseGrid.RowCount = ds.Tables(0).Rows.Count
            For i As Integer = 0 To ds.Tables(0).Rows.Count - 1
                Dim TOTAL As Double = Val(ds.Tables(0).Rows(i)(2)) - Val(ds.Tables(0).Rows(i)(3))
                ExpenseGrid.Rows(COUNT__).Cells(0).Value = ds.Tables(0).Rows(i)(0).ToString()
                ExpenseGrid.Rows(COUNT__).Cells(1).Value = ds.Tables(0).Rows(i)(1).ToString()
                ExpenseGrid.Rows(COUNT__).Cells(2).Value = Math.Round(TOTAL, 2)
                COUNT__ = COUNT__ + 1
            Next
        End If
        EXPENSETOTAL()
    End Sub
    Private Sub CmdExit_Click(sender As Object, e As EventArgs) Handles CmdExit.Click
        Me.Close()
    End Sub

    Private Sub CmdFindDepart_Click(sender As Object, e As EventArgs) Handles CmdFindDepart.Click
        Try
            Dim w As New BranchCostList
            w.ShowDialog()
            If w.IS_Yes = True Then
                Dim ii As Integer = w.BranchGrid.CurrentRow.Index
                TxtBranchName.Text = w.BranchGrid.Rows(ii).Cells(1).Value.ToString()
                TxtBranchGuid.Text = w.BranchGrid.Rows(ii).Cells(2).Value.ToString()
            End If
        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try
    End Sub

    Private Sub CmdDelBranch_Click(sender As Object, e As EventArgs) Handles CmdDelBranch.Click
        TxtBranchGuid.Text = Nothing
        TxtBranchName.Text = Nothing
    End Sub

    Private Sub CmdView_Click(sender As Object, e As EventArgs) Handles CmdView.Click
        Try
            LOADINCOME()
            LOADEXPENS()
            TxtFTotal.Text = Math.Round((Val(TxtIncomeTotal.Text) - Val(TxtExpensTotal.Text)), 2)
        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try
    End Sub
End Class