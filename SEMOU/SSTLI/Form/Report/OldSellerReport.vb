Imports System.Data.SqlClient
Imports DataGridViewAutoFilter

Public Class OldSellerReport
    Dim dbcontext As New SSTLI2023Entities

    Private Sub CmdClose_Click(sender As Object, e As EventArgs) Handles CmdClose.Click
        Me.Close()
    End Sub

    Private Sub CmdFindBranch_Click(sender As Object, e As EventArgs) Handles CmdFindBranch.Click
        Try
            Dim w As New BranchList
            w.ShowDialog()
            If w.IS_Yes = True Then
                Dim ii As Integer = w.BranchGrid.CurrentRow.Index
                TxtBranchCode.Text = w.BranchGrid.Rows(ii).Cells(0).Value.ToString()
                TxtBranchName.Text = w.BranchGrid.Rows(ii).Cells(1).Value.ToString()
                TxtBranchName2.Text = w.BranchGrid.Rows(ii).Cells(1).Value.ToString()
            End If
            If TxtBranchCode.Text <> Nothing Then
                LOADSALLERREG()
                If ItemGrid.Rows.Count <> 0 Then
                    DELETETEMPTBL()
                    ADDTEMPTABEL()
                    LOADREFUND()
                    LOADFIRSTPAY()
                    SUMMRYTOTAL()
                Else
                    DELETETEMPTBL()
                    ItemGrid.Rows.Clear()
                    RefundGrid.Rows.Clear()
                    PayGrid.Rows.Clear()
                    TOATLITEM()
                    TOATLPAY()
                    TOATLREFUND()
                    SUMMRYTOTAL()
                End If
            End If
        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try
    End Sub

    Private Sub CmdDelBranch_Click(sender As Object, e As EventArgs) Handles CmdDelBranch.Click
        Try
            TxtBranchCode.Text = Nothing
            TxtBranchName.Text = Nothing
            TxtBranchName2.Text = "كل الفروع"
            LOADSALLERREG()
            If ItemGrid.Rows.Count <> 0 Then
                DELETETEMPTBL()
                ADDTEMPTABEL()
                LOADREFUND()
                LOADFIRSTPAY()
                SUMMRYTOTAL()
            Else
                DELETETEMPTBL()
                ItemGrid.Rows.Clear()
                RefundGrid.Rows.Clear()
                PayGrid.Rows.Clear()
                TOATLITEM()
                TOATLPAY()
                TOATLREFUND()
                SUMMRYTOTAL()
            End If
        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try
    End Sub

    Private Sub CmdFindSeller_Click(sender As Object, e As EventArgs) Handles CmdFindSeller.Click
        Try
            Dim w As New SellerList
            w.ShowDialog()
            If w.IS_Yes = True Then
                Dim ii As Integer = w.SellerGrid.CurrentRow.Index
                TxtSellerCode.Text = w.SellerGrid.Rows(ii).Cells(0).Value.ToString()
                TxtSellerName.Text = w.SellerGrid.Rows(ii).Cells(1).Value.ToString()
                TxtSellerName2.Text = w.SellerGrid.Rows(ii).Cells(1).Value.ToString()
            End If
            If TxtSellerCode.Text <> Nothing Then
                LOADSALLERREG()
                If ItemGrid.Rows.Count <> 0 Then
                    DELETETEMPTBL()
                    ADDTEMPTABEL()
                    LOADREFUND()
                    LOADFIRSTPAY()
                    SUMMRYTOTAL()
                Else
                    DELETETEMPTBL()
                    ItemGrid.Rows.Clear()
                    RefundGrid.Rows.Clear()
                    PayGrid.Rows.Clear()
                    TOATLITEM()
                    TOATLPAY()
                    TOATLREFUND()
                    SUMMRYTOTAL()
                End If
            End If
        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try
    End Sub

    Private Sub CmdDelSeller_Click(sender As Object, e As EventArgs) Handles CmdDelSeller.Click
        TxtSellerName.Text = Nothing
        TxtSellerName2.Text = Nothing
        TxtSellerCode.Text = Nothing
        LOADSALLERREG()
        If ItemGrid.Rows.Count <> 0 Then
            DELETETEMPTBL()
            ADDTEMPTABEL()
            LOADREFUND()
            LOADFIRSTPAY()
            SUMMRYTOTAL()
        Else
            DELETETEMPTBL()
            ItemGrid.Rows.Clear()
            RefundGrid.Rows.Clear()
            PayGrid.Rows.Clear()
            TOATLITEM()
            TOATLPAY()
            TOATLREFUND()
            SUMMRYTOTAL()
        End If
    End Sub
    Sub TOATLITEM()
        Dim TOTAL_ As Double = 0
        For i As Integer = 0 To ItemGrid.Rows.Count - 1
            TOTAL_ = Val(TOTAL_) + (Val(ItemGrid.Rows(i).Cells(6).Value))
        Next
        TxtTotal1.Text = Math.Round(TOTAL_, 2)
        TxtCount1.Text = ItemGrid.Rows.Count()
    End Sub
    Sub TOATLREFUND()
        Dim TOTAL_ As Double = 0
        For i As Integer = 0 To RefundGrid.Rows.Count - 1
            TOTAL_ = Val(TOTAL_) + (Val(RefundGrid.Rows(i).Cells(6).Value))
        Next
        TxtTotal2.Text = Math.Round(TOTAL_, 2)
        TxtCount2.Text = RefundGrid.Rows.Count()
    End Sub
    Sub TOATLPAY()
        Dim TOTAL_ As Double = 0
        For i As Integer = 0 To PayGrid.Rows.Count - 1
            TOTAL_ = Val(TOTAL_) + (Val(PayGrid.Rows(i).Cells(4).Value))
        Next
        TxtTotal3.Text = Math.Round(TOTAL_, 2)
        TxtCount3.Text = PayGrid.Rows.Count()
    End Sub
    Sub LOADSALLERREG()
        ItemGrid.Rows.Clear()
        Dim STUDENTNAME As String = Nothing
        Dim ITEM As String = Nothing
        Dim BRNID As String = Nothing
        Dim SELLERID As String = Nothing
        Dim ITEM2 As String = "رسوم"
        Dim connection = dbcontext.Database.Connection
        Dim comm As New SqlCommand
        comm = New SqlCommand("LoadSellerReg", connection)
        comm.Parameters.AddWithValue("@FDate", Convert.ToDateTime(TxtFromDate.Value))
        comm.Parameters.AddWithValue("@TDate", Convert.ToDateTime(TxtToDate.Value))
        If TxtBranchCode.Text <> Nothing Then
            BRNID = TxtBranchCode.Text
            comm.Parameters.AddWithValue("@BRNID", BRNID)
        Else
            comm.Parameters.AddWithValue("@BRNID", BRNID)
        End If
        If TxtSellerCode.Text <> Nothing Then
            SELLERID = TxtSellerCode.Text
            comm.Parameters.AddWithValue("@SELLERNO", SELLERID)
        Else
            comm.Parameters.AddWithValue("@SELLERNO", SELLERID)
        End If
        If TxtFind2.Text <> Nothing Then
            If FindByStudent.Checked = True Then
                STUDENTNAME = TxtFind2.Text
                comm.Parameters.AddWithValue("@BENEFNAME", STUDENTNAME)
                comm.Parameters.AddWithValue("@ITMEXPLAN", ITEM)
            ElseIf FindByItem.Checked = True Then
                ITEM = TxtFind2.Text
                comm.Parameters.AddWithValue("@BENEFNAME", STUDENTNAME)
                comm.Parameters.AddWithValue("@ITMEXPLAN", ITEM)
            End If
        End If
        If TxtFind3.Text <> Nothing Then
            ITEM2 = TxtFind3.Text
            comm.Parameters.AddWithValue("@ITMEXPLAN2", ITEM2)
        Else
            comm.Parameters.AddWithValue("@ITMEXPLAN2", ITEM2)
        End If
        comm.CommandType = CommandType.StoredProcedure
        If connection.State = ConnectionState.Open Then
            connection.Close()
        End If
        connection.Open()
        comm.ExecuteNonQuery()
        Dim adp As New SqlDataAdapter(comm)
        Dim ds As DataSet = New DataSet()
        adp.Fill(ds, "LoadSellerReg")
        Dim COUNT__ As Integer = 0
        If ds.Tables(0).Rows.Count > 0 Then
            ItemGrid.RowCount = ds.Tables(0).Rows.Count
            'ItemGrid.DataSource = ds.Tables(0)
            For i As Integer = 0 To ds.Tables(0).Rows.Count - 1
                ItemGrid.Rows(COUNT__).Cells(0).Value = Convert.ToDateTime(ds.Tables(0).Rows(i)(0)).ToShortDateString()
                ItemGrid.Rows(COUNT__).Cells(1).Value = ds.Tables(0).Rows(i)(1).ToString()
                ItemGrid.Rows(COUNT__).Cells(2).Value = ds.Tables(0).Rows(i)(2).ToString()
                ItemGrid.Rows(COUNT__).Cells(3).Value = ds.Tables(0).Rows(i)(3).ToString()
                ItemGrid.Rows(COUNT__).Cells(4).Value = ds.Tables(0).Rows(i)(4).ToString()
                ItemGrid.Rows(COUNT__).Cells(5).Value = ds.Tables(0).Rows(i)(5).ToString()
                ItemGrid.Rows(COUNT__).Cells(6).Value = ds.Tables(0).Rows(i)(6).ToString()
                ItemGrid.Rows(COUNT__).Cells(7).Value = ds.Tables(0).Rows(i)(7).ToString()
                ItemGrid.Rows(COUNT__).Cells(8).Value = ds.Tables(0).Rows(i)(8).ToString()
                ItemGrid.Rows(COUNT__).Cells(9).Value = ds.Tables(0).Rows(i)(9).ToString()
                ItemGrid.Rows(COUNT__).Cells(10).Value = ds.Tables(0).Rows(i)(10).ToString()
                COUNT__ = COUNT__ + 1
            Next
            TOATLITEM()
        End If
    End Sub
    Sub LOADREFUND()
        RefundGrid.Rows.Clear()
        Dim STUDENTNAME As String = Nothing
        Dim ITEM As String = Nothing
        Dim SELLERID As String = Nothing
        Dim ITEM2 As String = "رسوم"
        Dim connection = dbcontext.Database.Connection
        Dim comm As New SqlCommand
        comm = New SqlCommand("RefundSellerReg", connection)
        If TxtFind2.Text <> Nothing Then
            If FindByStudent.Checked = True Then
                STUDENTNAME = TxtFind2.Text
                comm.Parameters.AddWithValue("@BENEFNAME", STUDENTNAME)
                comm.Parameters.AddWithValue("@ITMEXPLAN", ITEM)
            ElseIf FindByItem.Checked = True Then
                ITEM = TxtFind2.Text
                comm.Parameters.AddWithValue("@BENEFNAME", STUDENTNAME)
                comm.Parameters.AddWithValue("@ITMEXPLAN", ITEM)
            End If
        End If
        If TxtFind5.Text <> Nothing Then
            ITEM2 = TxtFind5.Text
            comm.Parameters.AddWithValue("@ITMEXPLAN2", ITEM2)
        Else
            comm.Parameters.AddWithValue("@ITMEXPLAN2", ITEM2)
        End If
        comm.CommandType = CommandType.StoredProcedure
        If connection.State = ConnectionState.Open Then
            connection.Close()
        End If
        connection.Open()
        comm.ExecuteNonQuery()
        Dim adp As New SqlDataAdapter(comm)
        Dim ds As DataSet = New DataSet()
        adp.Fill(ds, "RefundSellerReg")
        Dim COUNT__ As Integer = 0
        If ds.Tables(0).Rows.Count > 0 Then
            RefundGrid.RowCount = ds.Tables(0).Rows.Count
            'ItemGrid.DataSource = ds.Tables(0)
            For i As Integer = 0 To ds.Tables(0).Rows.Count - 1
                RefundGrid.Rows(COUNT__).Cells(0).Value = Convert.ToDateTime(ds.Tables(0).Rows(i)(0)).ToShortDateString()
                RefundGrid.Rows(COUNT__).Cells(1).Value = ds.Tables(0).Rows(i)(1).ToString()
                RefundGrid.Rows(COUNT__).Cells(2).Value = ds.Tables(0).Rows(i)(2).ToString()
                RefundGrid.Rows(COUNT__).Cells(3).Value = ds.Tables(0).Rows(i)(3).ToString()
                RefundGrid.Rows(COUNT__).Cells(4).Value = ds.Tables(0).Rows(i)(4).ToString()
                RefundGrid.Rows(COUNT__).Cells(5).Value = ds.Tables(0).Rows(i)(5).ToString()
                RefundGrid.Rows(COUNT__).Cells(6).Value = ds.Tables(0).Rows(i)(6).ToString()
                RefundGrid.Rows(COUNT__).Cells(7).Value = ds.Tables(0).Rows(i)(7).ToString()
                RefundGrid.Rows(COUNT__).Cells(8).Value = ds.Tables(0).Rows(i)(8).ToString()
                COUNT__ = COUNT__ + 1
            Next
            TOATLREFUND()
        End If
    End Sub
    Sub LOADFIRSTPAY()
        PayGrid.Rows.Clear()
        Dim connection = dbcontext.Database.Connection
        Dim comm As New SqlCommand
        comm = New SqlCommand("WhatFirstPay", connection)
        comm.CommandType = CommandType.StoredProcedure
        If connection.State = ConnectionState.Open Then
            connection.Close()
        End If
        connection.Open()
        comm.ExecuteNonQuery()
        Dim adp As New SqlDataAdapter(comm)
        Dim ds As DataSet = New DataSet()
        adp.Fill(ds, "WhatFirstPay")
        Dim COUNT__ As Integer = 0
        If ds.Tables(0).Rows.Count > 0 Then
            PayGrid.RowCount = ds.Tables(0).Rows.Count
            For i As Integer = 0 To ds.Tables(0).Rows.Count - 1
                PayGrid.Rows(COUNT__).Cells(0).Value = Convert.ToDateTime(ds.Tables(0).Rows(i)(0)).ToShortDateString()
                PayGrid.Rows(COUNT__).Cells(1).Value = ds.Tables(0).Rows(i)(1).ToString()
                PayGrid.Rows(COUNT__).Cells(2).Value = ds.Tables(0).Rows(i)(2).ToString()
                PayGrid.Rows(COUNT__).Cells(3).Value = ds.Tables(0).Rows(i)(3).ToString()
                PayGrid.Rows(COUNT__).Cells(4).Value = ds.Tables(0).Rows(i)(4).ToString()
                PayGrid.Rows(COUNT__).Cells(5).Value = ds.Tables(0).Rows(i)(5).ToString()
                COUNT__ = COUNT__ + 1
            Next
            TOATLPAY()
        End If
    End Sub
    Sub LOADGRIDSUMMRY()
        SummryGrid.Rows.Clear()
        Dim NAME1 As String = "رسوم"
        Dim BRNID As String = Nothing
        Dim connection = dbcontext.Database.Connection
        Dim comm As New SqlCommand
        comm = New SqlCommand("ShowSummerySellerReg", connection)
        comm.Parameters.AddWithValue("@FDate", Convert.ToDateTime(TxtFromDate.Value))
        comm.Parameters.AddWithValue("@TDate", Convert.ToDateTime(TxtToDate.Value))
        comm.Parameters.AddWithValue("@BrnId", TxtBranchCode.Text)
        comm.Parameters.AddWithValue("@SellerId", TxtSellerCode.Text)
        comm.Parameters.AddWithValue("@NoName1", NAME1)
        comm.CommandType = CommandType.StoredProcedure
        If connection.State = ConnectionState.Open Then
            connection.Close()
        End If
        connection.Open()
        comm.ExecuteNonQuery()
        Dim adp As New SqlDataAdapter(comm)
        Dim ds As DataSet = New DataSet()
        adp.Fill(ds, "ShowSummerySellerReg")
        Dim COUNT__ As Integer = 0
        If ds.Tables(0).Rows.Count > 0 Then
            SummryGrid.RowCount = ds.Tables(0).Rows.Count
            For i As Integer = 0 To ds.Tables(0).Rows.Count - 1
                SummryGrid.Rows(COUNT__).Cells(0).Value = ds.Tables(0).Rows(i)(0).ToString()
                SummryGrid.Rows(COUNT__).Cells(1).Value = ds.Tables(0).Rows(i)(1).ToString()
                COUNT__ = COUNT__ + 1
            Next
        End If
    End Sub
    Sub CALCREFUND()
        Dim row As DataGridViewRow
        For Each row In ItemGrid.Rows
            For i As Integer = 0 To ItemGrid.Rows.Count - 1
                If row.Cells(0).Value = Nothing Then
                    Exit For
                Else
                    'LOADREFUND(row.Cells(9).Value, row.Cells(10).Value)
                End If
            Next
        Next
    End Sub
    Sub ADDTEMPTABEL()
        Dim connection = dbcontext.Database.Connection
        Dim comm = New SqlCommand("AddTempCalc", connection)
        comm.CommandType = CommandType.StoredProcedure
        For i As Integer = 0 To ItemGrid.Rows.Count - 1
            If ItemGrid.Rows(i).Cells(10).Value = Nothing Then
                Exit For
            Else
                comm.Parameters.AddWithValue("@AcctCode", ItemGrid.Rows(i).Cells(9).Value.ToString())
                comm.Parameters.AddWithValue("@BrnId", ItemGrid.Rows(i).Cells(10).Value.ToString())
                comm.Parameters.AddWithValue("@Date_", Convert.ToDateTime(ItemGrid.Rows(i).Cells(0).Value))
            End If
            If connection.State = ConnectionState.Open Then
                connection.Close()
            End If
            connection.Open()
            Dim rs As Integer = comm.ExecuteNonQuery()
            If rs > 0 Then
                comm.Parameters.Clear()
            End If
        Next
    End Sub
    Private Sub EnableGridFilter(ByVal value As Boolean)
        'ColumnDate.FilteringEnabled = value
        'ColumnId.FilteringEnabled = value
        'ColumnItem.FilteringEnabled = value
        'ColumnStudent.FilteringEnabled = value
        'ColumnTot.FilteringEnabled = value
        'ColumnPr.FilteringEnabled = value
        'ColumnMan.FilteringEnabled = value
        'ColumnTel.FilteringEnabled = value
        'ColumnBra.FilteringEnabled = value
    End Sub
    Private Sub OldSellerReport_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        'ItemGrid.AutoGenerateColumns = True
    End Sub
    Sub DELETETEMPTBL()
        Dim connection = dbcontext.Database.Connection
        Dim comm = New SqlCommand("DeleteTemp", connection)
        comm.CommandType = CommandType.StoredProcedure
        If connection.State = ConnectionState.Open Then
            connection.Close()
        End If
        connection.Open()
        Dim rs As Integer = comm.ExecuteNonQuery()
        If rs > 0 Then
            comm.Parameters.Clear()
        End If
    End Sub
    Sub SUMMRYTOTAL()
        TxtRegOkCount.Text = Val(TxtCount1.Text) - Val(TxtCount2.Text)
        TxtRegNoPayCount.Text = Val(TxtRegOkCount.Text) - Val(TxtCount3.Text)
        TxtTotalRegOk.Text = Val(TxtTotal1.Text) - Val(TxtTotal2.Text)
    End Sub
    Private Sub CmdRefresh_Click(sender As Object, e As EventArgs) Handles CmdRefresh.Click
        Try
            LOADSALLERREG()
            If ItemGrid.Rows.Count <> 0 Then
                DELETETEMPTBL()
                ADDTEMPTABEL()
                LOADREFUND()
                LOADFIRSTPAY()
                SUMMRYTOTAL()
            Else
                DELETETEMPTBL()
                ItemGrid.Rows.Clear()
                RefundGrid.Rows.Clear()
                PayGrid.Rows.Clear()
                TOATLITEM()
                TOATLPAY()
                TOATLREFUND()
                SUMMRYTOTAL()
            End If
        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try
    End Sub

    Private Sub TxtFind2_TextChanged(sender As Object, e As EventArgs) Handles TxtFind2.TextChanged
        Try
            LOADSALLERREG()
            If ItemGrid.Rows.Count <> 0 Then
                DELETETEMPTBL()
                ADDTEMPTABEL()
                LOADREFUND()
                LOADFIRSTPAY()
            End If
        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try
    End Sub

    Private Sub TxtFind3_TextChanged(sender As Object, e As EventArgs) Handles TxtFind3.TextChanged
        Try
            LOADSALLERREG()
        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try
    End Sub

    Private Sub ShowAllLabel_Click(sender As Object, e As EventArgs)
        'DataGridViewAutoFilterTextBoxColumn.RemoveFilter(ItemGrid)
    End Sub

    Private Sub ItemGrid_CellContentClick(sender As Object, e As DataGridViewCellEventArgs) Handles ItemGrid.CellContentClick

    End Sub

    Private Sub ItemGrid_DataBindingComplete(sender As Object, e As DataGridViewBindingCompleteEventArgs) Handles ItemGrid.DataBindingComplete
        'Dim filterStatus As String = DataGridViewAutoFilterColumnHeaderCell.GetFilterStatus(ItemGrid)
        'Dim filterStatus As String = DataGridViewAutoFilterColumnHeaderCell.GetFilterStatus(ItemGrid)
        ' Handle filterStatus as needed
        'If String.IsNullOrEmpty(filterStatus) Then
        '    ShowAllLabel.Visible = False
        '    FilterStatusLabel.Visible = False
        'Else
        '    ShowAllLabel.Visible = True
        '    FilterStatusLabel.Visible = True
        '    FilterStatusLabel.Text = filterStatus
        'End If
    End Sub

    Private Sub ItemGrid_KeyDown(sender As Object, e As KeyEventArgs) Handles ItemGrid.KeyDown
        'If e.Alt AndAlso (e.KeyCode = Keys.Down Or e.KeyCode = Keys.Up) _
        '   AndAlso ItemGrid.CurrentCell IsNot Nothing _
        '   AndAlso TypeOf ItemGrid.CurrentCell.OwningColumn.HeaderCell Is DataGridViewAutoFilterColumnHeaderCell Then
        '    Dim filterCell As DataGridViewAutoFilterColumnHeaderCell = CType(ItemGrid.CurrentCell.OwningColumn.HeaderCell, DataGridViewAutoFilterColumnHeaderCell)
        '    filterCell.ShowDropDownList()
        '    e.Handled = True
        'End If
    End Sub

    Private Sub SimpleButton1_Click(sender As Object, e As EventArgs) Handles SimpleButton1.Click
        CALCREFUND()
    End Sub

    Private Sub CmdShow_Click(sender As Object, e As EventArgs) Handles CmdShow.Click
        Try
            If TxtBranchCode.Text = Nothing Or TxtSellerCode.Text = Nothing Then
                MessageBox.Show("لتتمكن من عرض ترتيب تسجيل الدبلوم - الدورة لابد من تحديد الفرع والمندوب", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Error)
                Return
            Else
                LOADGRIDSUMMRY()
            End If
        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try
    End Sub
End Class