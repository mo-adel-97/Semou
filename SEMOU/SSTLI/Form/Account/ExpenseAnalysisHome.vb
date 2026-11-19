Imports System.Data.SqlClient
Imports Microsoft.Office.Interop

Public Class ExpenseAnalysisHome
    Dim dbcontext As New SemouEntities

    Private Sub ExpenseAnalysisHome_Load(sender As Object, e As EventArgs) Handles MyBase.Load

    End Sub

    Private Sub CmdFindAccount_Click(sender As Object, e As EventArgs) Handles CmdFindAccount.Click
        Try
            Dim w As New CashBoxList
            w.Text = "قائمة الخزائن/البنوك"
            w.ShowDialog()
            If w.IS_Yes = True Then
                Dim ii As Integer = w.BranchGrid.CurrentRow.Index
                TxtAccountName.Text = w.BranchGrid.Rows(ii).Cells(1).Value.ToString()
                TxtAccountGuid.Text = w.BranchGrid.Rows(ii).Cells(3).Value.ToString()
            End If
        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try
    End Sub

    Private Sub CmdFindCostCenter_Click(sender As Object, e As EventArgs) Handles CmdFindCostCenter.Click
        Try
            If TxtAccountGuid.Text <> Nothing Then
                Dim w As New BranchCostList
                w.ShowDialog()
                If w.IS_Yes = True Then
                    Dim ii As Integer = w.BranchGrid.CurrentRow.Index
                    TxtCostCenterName.Text = w.BranchGrid.Rows(ii).Cells(1).Value.ToString()
                    TxtCostCenterGuid.Text = w.BranchGrid.Rows(ii).Cells(2).Value.ToString()
                End If
            Else
                MessageBox.Show("برجاء اختيار الخزينة/البنك اولا", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Error)
                Return
            End If
        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try
    End Sub
    Sub DAILYTOTAL()
        Dim DIPLOMTOTAL As Double = 0
        For i As Integer = 0 To DiplomGrid.Rows.Count - 1
            If DiplomGrid.Rows(i).Cells(1).Value <> Nothing Then
                DIPLOMTOTAL = Val(DIPLOMTOTAL) + (Val(DiplomGrid.Rows(i).Cells(3).Value))
            End If
        Next
        TxtTotal.Text = Math.Round(Val(DIPLOMTOTAL), 2)
    End Sub
    Sub LOADDATA()
        DiplomGrid.Rows.Clear()
        Dim BALANCE__ As Double = 0
        Dim connection = dbcontext.Database.Connection
        Dim comm As New SqlCommand("ShowExpenseAnalysis", connection)
        comm.Parameters.AddWithValue("@FDate", Convert.ToDateTime(TxtFromDate.Value))
        comm.Parameters.AddWithValue("@TDate", Convert.ToDateTime(TxtToDate.Value))
        comm.Parameters.AddWithValue("@CashBoxGuid", TxtAccountGuid.Text)
        If TxtCostCenterGuid.Text <> Nothing Then
            comm.Parameters.AddWithValue("@CostCenterGuid", TxtCostCenterGuid.Text)
        End If
        comm.CommandType = CommandType.StoredProcedure
        If connection.State = ConnectionState.Open Then
            connection.Close()
        End If
        connection.Open()
        comm.ExecuteNonQuery()
        Dim adp As New SqlDataAdapter(comm)
        Dim ds As DataSet = New DataSet()
        adp.Fill(ds, "ShowExpenseAnalysis")
        Dim COUNT__ As Integer = 0
        If ds.Tables(0).Rows.Count > 0 Then
            DiplomGrid.RowCount = ds.Tables(0).Rows.Count
            For i As Integer = 0 To ds.Tables(0).Rows.Count - 1
                DiplomGrid.Rows(COUNT__).Cells(0).Value = Convert.ToDateTime(ds.Tables(0).Rows(i)(0)).ToShortDateString()
                DiplomGrid.Rows(COUNT__).Cells(1).Value = ds.Tables(0).Rows(i)(1).ToString()
                DiplomGrid.Rows(COUNT__).Cells(2).Value = ds.Tables(0).Rows(i)(2).ToString()
                DiplomGrid.Rows(COUNT__).Cells(3).Value = ds.Tables(0).Rows(i)(3).ToString()
                DiplomGrid.Rows(COUNT__).Cells(4).Value = ds.Tables(0).Rows(i)(4).ToString()
                BALANCE__ = Val(BALANCE__) + (Val(DiplomGrid.Rows(COUNT__).Cells(3).Value) - Val(DiplomGrid.Rows(COUNT__).Cells(4).Value))
                DiplomGrid.Rows(COUNT__).Cells(5).Value = Math.Round(BALANCE__, 2)
                DiplomGrid.Rows(COUNT__).Cells(6).Value = ds.Tables(0).Rows(i)(5).ToString()
                DiplomGrid.Rows(COUNT__).Cells(7).Value = ds.Tables(0).Rows(i)(6).ToString()
                DiplomGrid.Rows(COUNT__).Cells(8).Value = ds.Tables(0).Rows(i)(7).ToString()
                DiplomGrid.Rows(COUNT__).Cells(9).Value = ds.Tables(0).Rows(i)(8).ToString()
                DiplomGrid.Rows(COUNT__).Cells(10).Value = ds.Tables(0).Rows(i)(9).ToString()
                COUNT__ = COUNT__ + 1
            Next
            DAILYTOTAL()
        End If
    End Sub
    Private Sub CmdClose_Click(sender As Object, e As EventArgs) Handles CmdClose.Click
        Me.Close()
    End Sub

    Private Sub CMDNew_Click(sender As Object, e As EventArgs) Handles CMDNew.Click
        Try
            If TxtAccountGuid.Text <> Nothing Then
                LOADDATA()
            Else
                MessageBox.Show("برجاء اختيار مركز الخزينة/البنك اولا", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Error)
                Return
            End If
        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try
    End Sub

    Private Sub CmdDelCostCenter_Click(sender As Object, e As EventArgs) Handles CmdDelCostCenter.Click
        TxtCostCenterGuid.Text = Nothing
        TxtCostCenterName.Text = Nothing
    End Sub

    Private Sub CmdExport_Click(sender As Object, e As EventArgs) Handles CmdExport.Click
        Try
            If DiplomGrid.Rows.Count = 0 Then
                MessageBox.Show("لا توجد بيانات لتصديرها", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Error)
                Return
            Else
                ' Open a SaveFileDialog to choose the output location
                SaveFileDialog1.InitialDirectory = "C:"
                SaveFileDialog1.Title = "Save as Excel File"
                SaveFileDialog1.FileName = "تحليل المصروفات"
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
                    For i As Integer = 0 To DiplomGrid.Columns.Count - 1
                        sheet.Cells(1, i + 1) = DiplomGrid.Columns(i).HeaderText
                    Next
                    ' Write data from DataGridView rows
                    For i As Integer = 0 To DiplomGrid.Rows.Count - 1
                        For j As Integer = 0 To DiplomGrid.Columns.Count - 1
                            sheet.Cells(i + 2, j + 1) = DiplomGrid.Rows(i).Cells(j).Value.ToString()
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