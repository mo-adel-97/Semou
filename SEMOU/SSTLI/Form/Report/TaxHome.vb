Imports System.Data.SqlClient
Imports Microsoft.Office.Interop

Public Class TaxHome
    Dim dbcontext As New SemouEntities
    Private Sub CmdExit_Click(sender As Object, e As EventArgs) Handles CmdExit.Click
        Me.Close()
    End Sub

    Private Sub TaxHome_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Try
            RegDocPosted.WHATFORMTYPEGUID("salesinvoice")
            TxtTypeGuid.Text = RegDocPosted.TYPEGUID
        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try
    End Sub
    Sub LOADREGDOC()
        BranchGrid.Rows.Clear()
        Dim connection = dbcontext.Database.Connection
        Dim comm As New SqlCommand("ShowTaxReport", connection)
        comm.Parameters.AddWithValue("@FDate", Convert.ToDateTime(TxtFromDate.Value))
        comm.Parameters.AddWithValue("@TDate", Convert.ToDateTime(TxtToDate.Value))
        If TxtBranchGuid.Text <> Nothing Then
            comm.Parameters.AddWithValue("@BranchGuid", TxtBranchGuid.Text)
        End If
        If TxtFind2.Text <> Nothing Then
            If FindById.Checked = True Then
                comm.Parameters.AddWithValue("@BillCode", AllFiles.TAXNOTOCODE(TxtFind2.Text))
            ElseIf FindByName.Checked = True Then
                comm.Parameters.AddWithValue("@NationalId", TxtFind2.Text)
            End If
        End If
        If ChkActiveImvoice.Checked = True Then
            comm.Parameters.AddWithValue("@IsUse", True)
        ElseIf ChkOflineInvoice.Checked = True Then
            comm.Parameters.AddWithValue("@IsUse", False)
        End If
        comm.CommandType = CommandType.StoredProcedure
        If connection.State = ConnectionState.Open Then
            connection.Close()
        End If
        connection.Open()
        comm.ExecuteNonQuery()
        Dim adp As New SqlDataAdapter(comm)
        Dim ds As DataSet = New DataSet()
        adp.Fill(ds, "ShowSalesReport")
        Dim COUNT__ As Integer = 0
        Dim taxMap = AllFiles.GETTAXNOMAP()
        If ds.Tables(0).Rows.Count > 0 Then
            BranchGrid.RowCount = ds.Tables(0).Rows.Count
            TxtCount.Text = BranchGrid.Rows.Count()
            TxtMsg.Visible = False
            For i As Integer = 0 To ds.Tables(0).Rows.Count - 1
                BranchGrid.Rows(COUNT__).Cells(0).Value = Convert.ToDateTime(ds.Tables(0).Rows(i)(0)).ToShortDateString()
                Dim billCode As String = ds.Tables(0).Rows(i)(1).ToString().Trim()
                BranchGrid.Rows(COUNT__).Cells(1).Value = If(taxMap.ContainsKey(billCode), taxMap(billCode), billCode)
                BranchGrid.Rows(COUNT__).Cells(2).Value = ds.Tables(0).Rows(i)(2).ToString()
                BranchGrid.Rows(COUNT__).Cells(3).Value = ds.Tables(0).Rows(i)(3).ToString()
                BranchGrid.Rows(COUNT__).Cells(4).Value = ds.Tables(0).Rows(i)(4).ToString()
                BranchGrid.Rows(COUNT__).Cells(5).Value = ds.Tables(0).Rows(i)(5).ToString()
                BranchGrid.Rows(COUNT__).Cells(6).Value = ds.Tables(0).Rows(i)(6).ToString()
                BranchGrid.Rows(COUNT__).Cells(7).Value = Math.Round(ds.Tables(0).Rows(i)(7), 2)
                BranchGrid.Rows(COUNT__).Cells(8).Value = Math.Round(ds.Tables(0).Rows(i)(8), 2)
                BranchGrid.Rows(COUNT__).Cells(9).Value = Math.Round(ds.Tables(0).Rows(i)(9), 2)
                BranchGrid.Rows(COUNT__).Cells(10).Value = ds.Tables(0).Rows(i)(10).ToString()
                BranchGrid.Rows(COUNT__).Cells(11).Value = ds.Tables(0).Rows(i)(11).ToString()
                BranchGrid.Rows(COUNT__).Cells(13).Value = ds.Tables(0).Rows(i)(12).ToString()
                BranchGrid.Rows(COUNT__).Cells(14).Value = ds.Tables(0).Rows(i)(13).ToString()
                COUNT__ = COUNT__ + 1
            Next
            DAILYTOTAL()
        Else
            TxtMsg.Visible = True
            TxtCount.Text = 0
            DAILYTOTAL()
        End If
    End Sub
    Sub DAILYTOTAL()
        Dim DIPLOMTAX As Double = 0
        Dim DIPLOMTOTAL As Double = 0
        Dim SUBTOTAL As Double = 0
        For i As Integer = 0 To BranchGrid.Rows.Count - 1
            If BranchGrid.Rows(i).Cells(2).Value <> Nothing Then
                DIPLOMTOTAL = Val(DIPLOMTOTAL) + (Val(BranchGrid.Rows(i).Cells(7).Value))
                DIPLOMTAX = Val(DIPLOMTAX) + (Val(BranchGrid.Rows(i).Cells(8).Value))
                SUBTOTAL = Val(SUBTOTAL) + (Val(BranchGrid.Rows(i).Cells(9).Value))
            End If
        Next
        TxtTotal.Text = Math.Round(Val(DIPLOMTOTAL), 2)
        TxtTax.Text = Math.Round(Val(DIPLOMTAX), 2)
        TxtSubTotal.Text = Math.Round(Val(SUBTOTAL), 2)
    End Sub

    Private Sub CmdView_Click(sender As Object, e As EventArgs) Handles CmdView.Click
        Try
            LOADREGDOC()
        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try
    End Sub

    Private Sub CmdFindDepart_Click(sender As Object, e As EventArgs) Handles CmdFindDepart.Click
        Try
            AllFiles.CHEKDEFBRANCHINFO(AllFiles.USER_GUID____.ToString)
            Dim w As New NewBranchList
            If AllFiles.DEFBRANCH = True Then
                w.TxtType.Text = 1
                w.TxtUserGuid.Text = AllFiles.USER_GUID____.ToString
            Else
                w.TxtType.Text = 0
                w.TxtIsUse.Text = True
            End If
            w.ShowDialog()
            If w.IS_Yes = True Then
                Dim ii As Integer = w.BranchGrid.CurrentRow.Index
                TxtBranchName.Text = w.BranchGrid.Rows(ii).Cells(1).Value.ToString()
                TxtBranchGuid.Text = w.BranchGrid.Rows(ii).Cells(3).Value.ToString()
            End If
            If TxtBranchGuid.Text <> Nothing Then
                LOADREGDOC()
            End If
        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try
    End Sub

    Private Sub CmdDelBranch_Click(sender As Object, e As EventArgs) Handles CmdDelBranch.Click
        Try
            TxtBranchName.Text = Nothing
            TxtBranchGuid.Text = Nothing
            LOADREGDOC()
        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try
    End Sub

    Private Sub CmdFindSalesMan_Click(sender As Object, e As EventArgs) Handles CmdFindSalesMan.Click
        Try
            Dim w As New UserList
            w.ShowDialog()
            If w.IS_Yes = True Then
                Dim ii As Integer = w.BranchGrid.CurrentRow.Index
                TxtUserName.Text = w.BranchGrid.Rows(ii).Cells(1).Value.ToString()
                TxtUserGuid.Text = w.BranchGrid.Rows(ii).Cells(3).Value.ToString()
            End If
            If TxtUserGuid.Text <> Nothing Then
                LOADREGDOC()
            End If
        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try
    End Sub

    Private Sub CmdDelSalesMan_Click(sender As Object, e As EventArgs) Handles CmdDelSalesMan.Click
        Try
            TxtUserGuid.Text = Nothing
            TxtUserName.Text = Nothing
            LOADREGDOC()
        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try
    End Sub

    Private Sub CmdFindBatch_Click(sender As Object, e As EventArgs) Handles CmdFindBatch.Click
        Try
            Dim w As New DocRegSettingList
            w.TxtTypeGuid.Text = TxtTypeGuid.Text
            w.ShowDialog()
            If w.IS_Yes = True Then
                Dim ii As Integer = w.BranchGrid.CurrentRow.Index
                TxtDocName.Text = w.BranchGrid.Rows(ii).Cells(1).Value.ToString()
                TxtDocGuid.Text = w.BranchGrid.Rows(ii).Cells(2).Value.ToString()
            End If
            If TxtDocGuid.Text <> Nothing Then
                LOADREGDOC()
            End If
        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try
    End Sub

    Private Sub CmdDelBatch_Click(sender As Object, e As EventArgs) Handles CmdDelBatch.Click
        Try
            TxtDocGuid.Text = Nothing
            TxtDocName.Text = Nothing
            LOADREGDOC()
        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try
    End Sub

    Private Sub ChkActiveImvoice_CheckedChanged(sender As Object, e As EventArgs) Handles ChkActiveImvoice.CheckedChanged
        'Try
        '    If ChkActiveImvoice.Checked = True Then
        '        LOADREGDOC()
        '    End If
        'Catch ex As Exception
        '    MessageBox.Show(ex.ToString)
        'End Try
    End Sub

    Private Sub ChkOflineInvoice_CheckedChanged(sender As Object, e As EventArgs) Handles ChkOflineInvoice.CheckedChanged
        'Try
        '    If ChkOflineInvoice.Checked = True Then
        '        LOADREGDOC()
        '    End If
        'Catch ex As Exception
        '    MessageBox.Show(ex.ToString)
        'End Try
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
                SaveFileDialog1.FileName = "تقرير الضرائب"
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
                    Dim columnsToExclude As New List(Of Integer) From {2, 6, 11, 12, 15}

                    For i As Integer = 0 To BranchGrid.Columns.Count - 1
                        If Not columnsToExclude.Contains(i) Then
                            sheet.Cells(1, i + 1) = BranchGrid.Columns(i).HeaderText
                        End If
                    Next
                    ' Write data from DataGridView rows
                    For i As Integer = 0 To BranchGrid.Rows.Count - 1
                        For j As Integer = 0 To BranchGrid.Columns.Count - 1
                            If Not columnsToExclude.Contains(j) Then
                                sheet.Cells(i + 2, j + 1) = BranchGrid.Rows(i).Cells(j).Value.ToString()
                            End If
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

    Private Sub TxtFind2_TextChanged(sender As Object, e As EventArgs) Handles TxtFind2.TextChanged
        Try
            LOADREGDOC()
        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try
    End Sub

    Private Sub BranchGrid_CellContentClick(sender As Object, e As DataGridViewCellEventArgs) Handles BranchGrid.CellContentClick

    End Sub

    Private Sub BranchGrid_CellClick(sender As Object, e As DataGridViewCellEventArgs) Handles BranchGrid.CellClick

    End Sub
End Class