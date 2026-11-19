Imports System.Data.SqlClient

Public Class BranchDailyHome
    Dim dbcontext As New SemouEntities
    Private Sub CmdClose_Click(sender As Object, e As EventArgs) Handles CmdClose.Click
        Me.Close()
    End Sub

    Private Sub CmdFindDepart_Click(sender As Object, e As EventArgs) Handles CmdFindDepart.Click
        Try
            AllFiles.CHEKDEFBRANCHINFO(AllFiles.USER_GUID____.ToString)
            Dim w As New NewBranchList
            If AllFiles.DEFBRANCH = True Then
                w.TxtType.Text = 1
                w.TxtUserGuid.Text = AllFiles.USER_GUID____.ToString
                w.TxtFind.ReadOnly = True
            Else
                w.TxtType.Text = 0
                w.TxtIsUse.Text = True
                w.TxtFind.ReadOnly = False
            End If
            w.ShowDialog()
            If w.IS_Yes = True Then
                Dim ii As Integer = w.BranchGrid.CurrentRow.Index
                TxtBranchName.Text = w.BranchGrid.Rows(ii).Cells(1).Value.ToString()
                TxtBranchGuid.Text = w.BranchGrid.Rows(ii).Cells(3).Value.ToString()
            End If
        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try
    End Sub

    Private Sub CmdFindCashBox_Click(sender As Object, e As EventArgs) Handles CmdFindCashBox.Click
        Try
            If TxtBranchGuid.Text <> Nothing Then
                Dim w As New CashBoxList
                w.Text = "قائمة البنوك /الخزائن"
                w.TxtUseInSales.Text = True
                w.TxtIsUse.Text = True
                w.TxtIsPublic.Text = True
                w.ShowDialog()
                If w.IS_Yes = True Then
                    Dim ii As Integer = w.BranchGrid.CurrentRow.Index
                    TxtCashBoxName.Text = w.BranchGrid.Rows(ii).Cells(1).Value.ToString()
                    TxtCashBoxGuid.Text = w.BranchGrid.Rows(ii).Cells(3).Value.ToString()
                End If
                If TxtCashBoxGuid.Text <> Nothing Then
                    LOADREGDOC()
                End If
            Else
                MessageBox.Show("برجاء اختيار الفرع اولا", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Error)
                Return
            End If
        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try
    End Sub
    Sub DAILYTOTAL()
        Dim SUBTOTAL As Double = 0
        For i As Integer = 0 To PayGrid.Rows.Count - 1
            If PayGrid.Rows(i).Cells(2).Value <> Nothing Then
                SUBTOTAL = Val(SUBTOTAL) + (Val(PayGrid.Rows(i).Cells(5).Value))
            End If
        Next
        TxtTotal.Text = Math.Round(Val(SUBTOTAL), 2)
    End Sub
    Sub LOADREGDOC()
        PayGrid.Rows.Clear()
        Dim connection = dbcontext.Database.Connection
        Dim comm As New SqlCommand("ShowBranchBill", connection)
        comm.Parameters.AddWithValue("@FDate", Convert.ToDateTime(TxtFDayDate.Value))
        comm.Parameters.AddWithValue("@TDate", Convert.ToDateTime(TxtTDayDate.Value))
        comm.Parameters.AddWithValue("@BranchGuid", TxtBranchGuid.Text)
        comm.Parameters.AddWithValue("@CashBixGuid", TxtCashBoxGuid.Text)
        comm.CommandType = CommandType.StoredProcedure
        If connection.State = ConnectionState.Open Then
            connection.Close()
        End If
        connection.Open()
        comm.ExecuteNonQuery()
        Dim adp As New SqlDataAdapter(comm)
        Dim ds As DataSet = New DataSet()
        adp.Fill(ds, "ShowBranchBill")
        Dim COUNT__ As Integer = 0
        If ds.Tables(0).Rows.Count > 0 Then
            PayGrid.RowCount = ds.Tables(0).Rows.Count
            TxtCount.Text = PayGrid.Rows.Count()
            For i As Integer = 0 To ds.Tables(0).Rows.Count - 1
                PayGrid.Rows(COUNT__).Cells(0).Value = Convert.ToDateTime(ds.Tables(0).Rows(i)(0)).ToShortDateString()
                PayGrid.Rows(COUNT__).Cells(1).Value = ds.Tables(0).Rows(i)(1).ToString()
                PayGrid.Rows(COUNT__).Cells(2).Value = ds.Tables(0).Rows(i)(2).ToString()
                PayGrid.Rows(COUNT__).Cells(3).Value = ds.Tables(0).Rows(i)(3).ToString()
                PayGrid.Rows(COUNT__).Cells(4).Value = ds.Tables(0).Rows(i)(4).ToString()
                PayGrid.Rows(COUNT__).Cells(5).Value = ds.Tables(0).Rows(i)(5).ToString()
                PayGrid.Rows(COUNT__).Cells(6).Value = ds.Tables(0).Rows(i)(6).ToString()
                COUNT__ = COUNT__ + 1
            Next
            DAILYTOTAL()
        Else
            TxtCount.Text = 0
            DAILYTOTAL()
        End If
    End Sub

    Private Sub SimpleButton1_Click(sender As Object, e As EventArgs) Handles SimpleButton1.Click
        Try
            If TxtCashBoxGuid.Text <> Nothing Then
                LOADREGDOC()
            Else
                MessageBox.Show("برجاء اختيار الخزينة او البنك اولا", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Error)
                Return
            End If
        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try
    End Sub
End Class