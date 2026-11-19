Imports System.Data.SqlClient

Public Class MssgHome
    Dim dbcontext As New SemouEntities
    Dim remainingTime As TimeSpan = TimeSpan.FromMinutes(5) ' 5 دقائق
    Sub LOADREGDOC()
        BranchGrid.Rows.Clear()
        Dim connection = dbcontext.Database.Connection
        Dim comm As New SqlCommand("ShowRegDocRemainingDays", connection)
        comm.Parameters.AddWithValue("@SellerGuid", AllFiles.SELLERMAN_GUID____)
        comm.CommandType = CommandType.StoredProcedure
        If connection.State = ConnectionState.Open Then
            connection.Close()
        End If
        connection.Open()
        comm.ExecuteNonQuery()
        Dim adp As New SqlDataAdapter(comm)
        Dim ds As DataSet = New DataSet()
        adp.Fill(ds, "ShowRegDocRemainingDays")
        Dim COUNT__ As Integer = 0
        If ds.Tables(0).Rows.Count > 0 Then
            BranchGrid.RowCount = ds.Tables(0).Rows.Count
            TxtCount.Text = BranchGrid.Rows.Count()
            For i As Integer = 0 To ds.Tables(0).Rows.Count - 1
                BranchGrid.Rows(COUNT__).Cells(0).Value = ds.Tables(0).Rows(i)(0).ToString()
                BranchGrid.Rows(COUNT__).Cells(1).Value = Convert.ToDateTime(ds.Tables(0).Rows(i)(1)).ToShortDateString()
                BranchGrid.Rows(COUNT__).Cells(2).Value = ds.Tables(0).Rows(i)(2).ToString()
                BranchGrid.Rows(COUNT__).Cells(3).Value = ds.Tables(0).Rows(i)(3).ToString()
                BranchGrid.Rows(COUNT__).Cells(4).Value = ds.Tables(0).Rows(i)(4).ToString()
                BranchGrid.Rows(COUNT__).Cells(5).Value = ds.Tables(0).Rows(i)(5).ToString()
                BranchGrid.Rows(COUNT__).Cells(6).Value = ds.Tables(0).Rows(i)(6).ToString()
                BranchGrid.Rows(COUNT__).Cells(7).Value = ds.Tables(0).Rows(i)(7).ToString()
                BranchGrid.Rows(COUNT__).Cells(8).Value = ds.Tables(0).Rows(i)(8).ToString()
                COUNT__ = COUNT__ + 1
            Next
        Else
            TxtCount.Text = 0
        End If
    End Sub
    Private Sub MssgHome_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Try
            ' تهيئة عرض الوقت عند التحميل
            lblTimer.Text = remainingTime.ToString("mm\:ss")
            LOADREGDOC()
            ' زر لبدء العداد
            StartCountdown()
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
            If j = 9 Then
                If AllFiles.FORFIND("studentstatment") = False Then
                    Return
                Else
                    Dim w As New StusdentStatment
                    w.TxtId.Text = BranchGrid.Rows(i).Cells(0).Value.ToString
                    w.TxtStudentName.Text = BranchGrid.Rows(i).Cells(8).Value.ToString
                    w.TxtAccountGuid.Text = BranchGrid.Rows(i).Cells(7).Value.ToString
                    w.TxtNationalId.Text = BranchGrid.Rows(i).Cells(5).Value.ToString
                    w.ShowDialog()
                End If
            ElseIf j = 10 Then
                Dim w As New ContractHome
                w.TxtFind2.Text = BranchGrid.Rows(i).Cells(5).Value.ToString
                w.SHOWRECEPITINFO()
                w.TxtFind2.Enabled = False
                w.ChkContract.Checked = True
                w.ChkContract.Enabled = False
                w.ShowDialog()
            End If
        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try
    End Sub

    Private Sub Timer1_Tick(sender As Object, e As EventArgs) Handles Timer1.Tick
        ' تقليل الوقت المتبقي
        If remainingTime.TotalSeconds > 0 Then
            remainingTime = remainingTime.Subtract(TimeSpan.FromSeconds(1))
            lblTimer.Text = remainingTime.ToString("mm\:ss")
        Else
            '' إيقاف المؤقت عند انتهاء العد
            'Timer1.Stop()
            'lblTimer.Text = "00:00"
            ''MessageBox.Show("انتهى الوقت!")
            LOADREGDOC()
            ' إعادة ضبط الوقت وبدء العداد من جديد
            ResetTimer()
        End If
    End Sub
    Private Sub ResetTimer()
        ' إعادة ضبط العداد
        remainingTime = TimeSpan.FromMinutes(5)
        lblTimer.Text = remainingTime.ToString("mm\:ss")
        Timer1.Start()
    End Sub
    Private Sub StartCountdown()
        ' بدء العداد التنازلي
        Timer1.Start()
    End Sub
End Class