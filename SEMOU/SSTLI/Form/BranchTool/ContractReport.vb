Imports System.Data.SqlClient
Imports System.IO
Imports Microsoft.Office.Interop

Public Class ContractReport
    Dim dbcontext As New SemouEntities
    Private Sub CmdClose_Click(sender As Object, e As EventArgs) Handles CmdClose.Click
        Me.Close()
    End Sub
    Sub LOADREGDOC()
        BranchGrid.Rows.Clear()
        Dim connection = dbcontext.Database.Connection
        Dim comm As New SqlCommand("ShowContractReport", connection)
        comm.Parameters.AddWithValue("@FDate", Convert.ToDateTime(TxtFromDate.Value))
        comm.Parameters.AddWithValue("@TDate", Convert.ToDateTime(TxtToDate.Value))
        If TxtSalesManGuid.Text <> Nothing Then
            comm.Parameters.AddWithValue("@SallesMan", TxtSalesManGuid.Text)
        End If
        If CboType.SelectedIndex = 1 Then
            comm.Parameters.AddWithValue("@ChkContract", True)
        ElseIf CboType.SelectedIndex = 2 Then
            comm.Parameters.AddWithValue("@ChkContract", False)
        End If
        comm.CommandType = CommandType.StoredProcedure
        If connection.State = ConnectionState.Open Then
            connection.Close()
        End If
        connection.Open()
        comm.ExecuteNonQuery()
        Dim adp As New SqlDataAdapter(comm)
        Dim ds As DataSet = New DataSet()
        adp.Fill(ds, "ShowContractReport")
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
                BranchGrid.Rows(COUNT__).Cells(8).Value = ds.Tables(0).Rows(i)(10).ToString()
                BranchGrid.Rows(COUNT__).Cells(9).Value = ds.Tables(0).Rows(i)(8).ToString()
                BranchGrid.Rows(COUNT__).Cells(10).Value = ds.Tables(0).Rows(i)(9).ToString()
                COUNT__ = COUNT__ + 1
            Next
        Else
            TxtCount.Text = 0
        End If
    End Sub
    Private Sub CmdFindSalesMan_Click(sender As Object, e As EventArgs) Handles CmdFindSalesMan.Click
        Try
            Dim w As New SalesManList
            w.TxtIsUse.Text = True
            w.ShowDialog()
            If w.IS_Yes = True Then
                Dim ii As Integer = w.BranchGrid.CurrentRow.Index
                TxtSalesManName.Text = w.BranchGrid.Rows(ii).Cells(1).Value.ToString()
                TxtSalesManGuid.Text = w.BranchGrid.Rows(ii).Cells(3).Value.ToString()
            End If
        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try
    End Sub

    Private Sub CmdDelMan_Click(sender As Object, e As EventArgs) Handles CmdDelMan.Click
        TxtSalesManName.Text = Nothing
        TxtSalesManGuid.Text = Nothing
    End Sub

    Private Sub ContractReport_Load(sender As Object, e As EventArgs) Handles MyBase.Load

    End Sub

    Private Sub CmdView_Click(sender As Object, e As EventArgs) Handles CmdView.Click
        Try
            LOADREGDOC()
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
            If j = 11 Then
                If AllFiles.FORFIND("studentstatment") = False Then
                    Return
                Else
                    Dim w As New StusdentStatment
                    w.TxtId.Text = BranchGrid.Rows(i).Cells(9).Value.ToString
                    w.TxtStudentName.Text = BranchGrid.Rows(i).Cells(3).Value.ToString
                    w.TxtAccountGuid.Text = BranchGrid.Rows(i).Cells(9).Value.ToString
                    w.TxtNationalId.Text = BranchGrid.Rows(i).Cells(5).Value.ToString
                    w.ShowDialog()
                End If
            ElseIf j = 12 Then
                Dim folderPath As String = "\\192.168.11.236\Arc\" & BranchGrid.Rows(i).Cells(5).Value.ToString
                If BranchGrid.Rows(i).Cells(5).Value.ToString = Nothing Then
                    MessageBox.Show("برجاء اختيار الطالب اولا", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Error)
                    Return
                Else
                    If Not Directory.Exists(folderPath) Then
                        MessageBox.Show("لا يتوفر ملف مرفقات للطالب", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Error)
                        Return
                    Else
                        Process.Start(folderPath)
                    End If
                End If
            End If
        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try
    End Sub

    Private Sub CmdExport_Click(sender As Object, e As EventArgs) Handles CmdExport.Click
        Try
            If BranchGrid.Rows.Count = 0 Then
                MessageBox.Show("لا توجد بيانات لتصديرها", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Error)
                Return
            Else
                ' Open a SaveFileDialog to choose the output location
                SaveFileDialog1.InitialDirectory = "C:"
                SaveFileDialog1.Title = "Save as Excel File"
                SaveFileDialog1.FileName = "تقرير الاتفاقيات"
                SaveFileDialog1.Filter = "Excel Files(2003)|*.xls| Excel Files(2007)|*.xlsx"
                If SaveFileDialog1.ShowDialog() <> DialogResult.Cancel Then
                    ' Create a new Excel Application instance
                    Dim ExcelApp As New Excel.Application()
                    ' Add a new workbook
                    ExcelApp.Workbooks.Add(Type.Missing)

                    ' Autofit columns
                    ExcelApp.Columns.AutoFit()

                    ' Get the active worksheet
                    Dim sheet As Excel.Worksheet = ExcelApp.ActiveSheet

                    ' Write column headers from DataGridView
                    For i As Integer = 0 To BranchGrid.Columns.Count - 1
                        sheet.Cells(1, i + 1) = BranchGrid.Columns(i).HeaderText
                    Next
                    ' Write data from DataGridView rows
                    For i As Integer = 0 To BranchGrid.Rows.Count - 1
                        For j As Integer = 0 To BranchGrid.Columns.Count - 1
                            sheet.Cells(i + 2, j + 1) = BranchGrid.Rows(i).Cells(j).Value.ToString()
                        Next
                    Next
                    ' Show success message
                    MessageBox.Show("تم تصدير البيانات بنجاح")
                    ' Save the workbook
                    sheet.SaveAs(SaveFileDialog1.FileName)
                    Dim result As DialogResult = MessageBox.Show("هل تريد عرض الملف", "تنبيه", MessageBoxButtons.OKCancel, MessageBoxIcon.Question)
                    If result = DialogResult.OK Then
                        ' Make Excel visible (optional)
                        ExcelApp.Visible = True
                    Else
                        ExcelApp.Visible = False
                        ' Quit Excel application
                        ExcelApp.Quit()
                    End If
                End If
            End If
        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try
    End Sub
End Class