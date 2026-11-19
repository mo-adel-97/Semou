Imports System.Data.SqlClient

Public Class CustomerBalanceHome
    Dim dbcontext As New SemouEntities
    Private Sub CmdExit_Click(sender As Object, e As EventArgs) Handles CmdExit.Click
        Me.Close()
    End Sub
    Sub LOADOLDSTUEDENT()
        BranchGrid.Rows.Clear()
        Dim connection = dbcontext.Database.Connection
        Dim comm As New SqlCommand("ShowAllCustBalance", connection)
        comm.Parameters.AddWithValue("@FDate", Convert.ToDateTime(TxtFromDate.Value))
        comm.Parameters.AddWithValue("@TDate", Convert.ToDateTime(TxtToDate.Value))
        comm.CommandType = CommandType.StoredProcedure
        If connection.State = ConnectionState.Open Then
            connection.Close()
        End If
        connection.Open()
        comm.ExecuteNonQuery()
        Dim adp As New SqlDataAdapter(comm)
        Dim ds As DataSet = New DataSet()
        adp.Fill(ds, "ShowAllCustBalance")
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
                BranchGrid.Rows(COUNT__).Cells(6).Value = Math.Round(Val(ds.Tables(0).Rows(i)(4)) - Val(ds.Tables(0).Rows(i)(5)), 2)
                BranchGrid.Rows(COUNT__).Cells(7).Value = ds.Tables(0).Rows(i)(6).ToString()
                COUNT__ = COUNT__ + 1
            Next
        End If
        DAILYTOTAL()
    End Sub
    Sub DAILYTOTAL()
        Dim MADEN As Double = 0
        Dim DAEN As Double = 0
        Dim BALANCE As Double = 0
        For i As Integer = 0 To BranchGrid.Rows.Count - 1
            If BranchGrid.Rows(i).Cells(2).Value <> Nothing Then
                MADEN = Val(MADEN) + (Val(BranchGrid.Rows(i).Cells(4).Value))
                DAEN = Val(DAEN) + (Val(BranchGrid.Rows(i).Cells(5).Value))
                BALANCE = Val(BALANCE) + (Val(BranchGrid.Rows(i).Cells(6).Value))
            End If
        Next
        TxtMaden.Text = Math.Round(Val(MADEN), 2)
        TxtDaen.Text = Math.Round(Val(DAEN), 2)
        TxtBalance.Text = Math.Round(Val(BALANCE), 2)
    End Sub
    Private Sub CustomerBalanceHome_Load(sender As Object, e As EventArgs) Handles MyBase.Load

    End Sub

    Private Sub CmdView_Click(sender As Object, e As EventArgs) Handles CmdView.Click
        Try
            LOADOLDSTUEDENT()
        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try
    End Sub
End Class