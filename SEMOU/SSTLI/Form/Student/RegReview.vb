Imports System.Data.SqlClient
Imports Microsoft.Office.Interop

Public Class RegReview
    Dim dbcontext As New SemouEntities
    Private Sub CmdExit_Click(sender As Object, e As EventArgs) Handles CmdExit.Click
        Me.Close()
    End Sub

    Private Sub TxtSalesManName_TextChanged(sender As Object, e As EventArgs) Handles TxtSalesManName.TextChanged

    End Sub

    Private Sub Label5_Click(sender As Object, e As EventArgs) Handles Label5.Click

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
            If TxtSalesManGuid.Text <> Nothing Then
                BranchGrid.Columns(12).Visible = False
                LOADREGDOC()
            End If
        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try
    End Sub

    Private Sub SimpleButton2_Click(sender As Object, e As EventArgs) Handles CmdDelSalesMan.Click
        Try
            TxtSalesManName.Text = Nothing
            TxtSalesManGuid.Text = Nothing
            If TxtSalesManGuid.Text = Nothing Then
                BranchGrid.Columns(12).Visible = True
                LOADREGDOC()
            End If
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

    Private Sub CmdFindBatch_Click(sender As Object, e As EventArgs) Handles CmdFindBatch.Click
        Try
            Dim w As New BatchList
            w.TxtIsUse.Text = True
            w.ShowDialog()
            If w.IS_Yes = True Then
                Dim ii As Integer = w.BranchGrid.CurrentRow.Index
                TxtBatchName.Text = w.BranchGrid.Rows(ii).Cells(1).Value.ToString()
                TxtBatchGuid.Text = w.BranchGrid.Rows(ii).Cells(3).Value.ToString()
            End If
            If TxtBatchGuid.Text <> Nothing Then
                LOADREGDOC()
            End If
        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try
    End Sub

    Private Sub CmdDelBatch_Click(sender As Object, e As EventArgs) Handles CmdDelBatch.Click
        Try
            TxtBatchName.Text = Nothing
            TxtBatchGuid.Text = Nothing
            LOADREGDOC()
        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try
    End Sub
    Sub DAILYTOTAL()
        Dim DIPLOMTAX As Double = 0
        Dim DIPLOMTOTAL As Double = 0
        Dim SUBTOTAL As Double = 0
        For i As Integer = 0 To BranchGrid.Rows.Count - 1
            If BranchGrid.Rows(i).Cells(2).Value <> Nothing Then
                DIPLOMTOTAL = Val(DIPLOMTOTAL) + (Val(BranchGrid.Rows(i).Cells(6).Value))
                DIPLOMTAX = Val(DIPLOMTAX) + (Val(BranchGrid.Rows(i).Cells(7).Value))
                SUBTOTAL = Val(SUBTOTAL) + (Val(BranchGrid.Rows(i).Cells(8).Value))
            End If
        Next
        TxtTotal.Text = Math.Round(Val(DIPLOMTOTAL), 2)
        TxtTax.Text = Math.Round(Val(DIPLOMTAX), 2)
        TxtSubTotal.Text = Math.Round(Val(SUBTOTAL), 2)
    End Sub
    Sub LOADREGDOC()
        BranchGrid.Rows.Clear()
        Dim connection = dbcontext.Database.Connection
        Dim comm As New SqlCommand("RegDocReview", connection)
        comm.Parameters.AddWithValue("@FDate", Convert.ToDateTime(TxtFromDate.Value))
        comm.Parameters.AddWithValue("@TDate", Convert.ToDateTime(TxtToDate.Value))
        If TxtBranchGuid.Text <> Nothing Then
            comm.Parameters.AddWithValue("@BranchGuid", TxtBranchGuid.Text)
        End If
        If TxtSalesManGuid.Text <> Nothing Then
            comm.Parameters.AddWithValue("@SalesManGuid", TxtSalesManGuid.Text)
        End If
        If TxtBatchGuid.Text <> Nothing Then
            comm.Parameters.AddWithValue("@BatchGuid", TxtBatchGuid.Text)
        End If
        If CboType.Text <> Nothing Then
            If CboType.SelectedIndex <> 2 Then
                comm.Parameters.AddWithValue("@RegType", CboType.SelectedIndex)
            End If
        End If
        If TxtDiplomGuid.Text <> Nothing Then
            comm.Parameters.AddWithValue("@DiplomGuid", TxtDiplomGuid.Text)
        End If
        If TxtCourseGuid.Text <> Nothing Then
            comm.Parameters.AddWithValue("@DiplomGuid", TxtCourseGuid.Text)
        End If
        If TxtFind2.Text <> Nothing Then
            If FindById.Checked = True Then
                comm.Parameters.AddWithValue("@RegCode", TxtFind2.Text)
            ElseIf FindByName.Checked = True Then
                comm.Parameters.AddWithValue("@StudentName", TxtFind2.Text)
            End If

        End If
        comm.CommandType = CommandType.StoredProcedure
        If connection.State = ConnectionState.Open Then
            connection.Close()
        End If
        connection.Open()
        comm.ExecuteNonQuery()
        Dim adp As New SqlDataAdapter(comm)
        Dim ds As DataSet = New DataSet()
        adp.Fill(ds, "RegDocReview")
        Dim COUNT__ As Integer = 0
        If ds.Tables(0).Rows.Count > 0 Then
            BranchGrid.RowCount = ds.Tables(0).Rows.Count
            TxtCount.Text = BranchGrid.Rows.Count()
            TxtMsg.Visible = False
            For i As Integer = 0 To ds.Tables(0).Rows.Count - 1
                BranchGrid.Rows(COUNT__).Cells(0).Value = Convert.ToDateTime(ds.Tables(0).Rows(i)(0)).ToShortDateString()
                BranchGrid.Rows(COUNT__).Cells(1).Value = ds.Tables(0).Rows(i)(1).ToString()
                BranchGrid.Rows(COUNT__).Cells(2).Value = ds.Tables(0).Rows(i)(2).ToString()
                BranchGrid.Rows(COUNT__).Cells(3).Value = ds.Tables(0).Rows(i)(3).ToString()
                BranchGrid.Rows(COUNT__).Cells(4).Value = ds.Tables(0).Rows(i)(4).ToString()
                BranchGrid.Rows(COUNT__).Cells(5).Value = ds.Tables(0).Rows(i)(5).ToString()
                BranchGrid.Rows(COUNT__).Cells(6).Value = ds.Tables(0).Rows(i)(6).ToString()
                BranchGrid.Rows(COUNT__).Cells(7).Value = ds.Tables(0).Rows(i)(7).ToString()
                BranchGrid.Rows(COUNT__).Cells(8).Value = ds.Tables(0).Rows(i)(8).ToString()
                BranchGrid.Rows(COUNT__).Cells(10).Value = ds.Tables(0).Rows(i)(9).ToString()
                BranchGrid.Rows(COUNT__).Cells(11).Value = ds.Tables(0).Rows(i)(10).ToString()
                BranchGrid.Rows(COUNT__).Cells(12).Value = ds.Tables(0).Rows(i)(11).ToString()
                BranchGrid.Rows(COUNT__).Cells(9).Value = ds.Tables(0).Rows(i)(13).ToString()
                BranchGrid.Rows(COUNT__).Cells(15).Value = ds.Tables(0).Rows(i)(12).ToString()
                COUNT__ = COUNT__ + 1
            Next
            DAILYTOTAL()
        Else
            TxtCount.Text = 0
            TxtMsg.Visible = True
            DAILYTOTAL()
        End If
    End Sub

    Private Sub TxtFind2_TextChanged(sender As Object, e As EventArgs)
        Try
            LOADREGDOC()
        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try
    End Sub

    Private Sub CboPay_SelectedIndexChanged(sender As Object, e As EventArgs)

    End Sub

    Private Sub CboPay_SelectionChangeCommitted(sender As Object, e As EventArgs)
    End Sub

    Private Sub BranchGrid_CellContentClick(sender As Object, e As DataGridViewCellEventArgs) Handles BranchGrid.CellContentClick

    End Sub

    Private Sub BranchGrid_CellClick(sender As Object, e As DataGridViewCellEventArgs) Handles BranchGrid.CellClick
        Try
            Dim i As Integer = BranchGrid.CurrentCell.RowIndex
            Dim j As Integer = BranchGrid.CurrentCell.ColumnIndex
            If j = 13 Then
                Dim w As New RegisterForm
                w.TxtCode.Text = BranchGrid.Rows(i).Cells(1).Value.ToString
                w.TxtTypeGuid.Text = BranchGrid.Rows(i).Cells(12).Value.ToString()
                w.SHOWREGDOCINFO()
                If w.TxtRegType.Text = 1 Then
                    w.SHOWCOURSEINFO()
                End If
                w.TxtStudentCount.Visible = False
                w.TxtAvaCount.Visible = False
                w.Label26.Visible = False
                w.Label30.Visible = False
                w.CHKWHATREGTYPE()
                If w.ChkOtherFees.Checked = True Then
                    w.SHOWLISTFEES()
                    w.SHOWREGDOCFEES()
                End If
                w.SHOWREGDOCDIPLOM()
                w.SHOWBILLDIPLOM()
                w.PAYTOTAL()
                w.SHOWDAY1INFO()
                w.DIPLOMTOTAL()
                w.FESSTOTAL()
                w.ShowDialog()
            ElseIf j = 14 Then
                If AllFiles.FORFIND("studentstatment") = False Then
                    Return
                Else
                    Dim w As New StusdentStatment
                    w.TxtId.Text = BranchGrid.Rows(i).Cells(15).Value.ToString
                    w.TxtStudentName.Text = BranchGrid.Rows(i).Cells(2).Value.ToString
                    w.TxtAccountGuid.Text = BranchGrid.Rows(i).Cells(11).Value.ToString
                    w.TxtNationalId.Text = BranchGrid.Rows(i).Cells(4).Value.ToString
                    w.ShowDialog()
                End If
            End If
        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try
    End Sub

    Private Sub RegReview_Load(sender As Object, e As EventArgs) Handles MyBase.Load

    End Sub

    Private Sub TxtFromDate_ValueChanged(sender As Object, e As EventArgs) Handles TxtFromDate.ValueChanged

    End Sub

    Private Sub CmdFindCourse_Click_1(sender As Object, e As EventArgs) Handles CmdFindCourse.Click
        Try
            Dim w As New DiplomList
            w.TxtIsUse.Text = True
            w.TxtType.Text = 0
            w.TxtStudyType.Text = 1
            w.ShowDialog()
            If w.IS_Yes = True Then
                Dim ii As Integer = w.BranchGrid.CurrentRow.Index
                TxtCourseName.Text = w.BranchGrid.Rows(ii).Cells(1).Value.ToString()
                TxtCourseGuid.Text = w.BranchGrid.Rows(ii).Cells(3).Value.ToString()
            End If
            If TxtCourseGuid.Text <> Nothing Then
                LOADREGDOC()
            End If
        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try
    End Sub

    Private Sub CmdFindDiplom_Click(sender As Object, e As EventArgs) Handles CmdFindDiplom.Click
        Try
            Dim w As New DiplomList
            w.TxtIsUse.Text = True
            w.TxtType.Text = 0
            w.TxtStudyType.Text = 0
            w.ShowDialog()
            If w.IS_Yes = True Then
                Dim ii As Integer = w.BranchGrid.CurrentRow.Index
                TxtDiplomName.Text = w.BranchGrid.Rows(ii).Cells(1).Value.ToString()
                TxtDiplomGuid.Text = w.BranchGrid.Rows(ii).Cells(3).Value.ToString()
            End If
            If TxtDiplomGuid.Text <> Nothing Then
                LOADREGDOC()
            End If
        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try
    End Sub

    Private Sub CboType_SelectedIndexChanged(sender As Object, e As EventArgs) Handles CboType.SelectedIndexChanged
        Try
            LOADREGDOC()
        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try
    End Sub

    Private Sub CboType_SelectionChangeCommitted(sender As Object, e As EventArgs) Handles CboType.SelectionChangeCommitted
        Try
            If CboType.SelectedIndex = 0 Then
                TxtBatchName.Enabled = True
                TxtDiplomName.Enabled = True
                CmdFindDiplom.Enabled = True
                CmdDelDiplom.Enabled = True
                CmdFindBatch.Enabled = True
                CmdDelBatch.Enabled = True
                TxtCourseName.Enabled = False
                CmdFindCourse.Enabled = False
                CmdDelCourse.Enabled = False
                TxtCourseName.Text = Nothing
                TxtCourseGuid.Text = Nothing
            ElseIf CboType.SelectedIndex = 1 Then
                TxtBatchName.Enabled = False
                TxtDiplomName.Enabled = False
                CmdFindDiplom.Enabled = False
                CmdDelDiplom.Enabled = False
                CmdFindBatch.Enabled = False
                CmdDelBatch.Enabled = False
                TxtCourseName.Enabled = True
                CmdFindCourse.Enabled = True
                CmdDelCourse.Enabled = True
                TxtBatchName.Text = Nothing
                TxtBatchGuid.Text = Nothing
                TxtDiplomName.Text = Nothing
                TxtDiplomGuid.Text = Nothing
            ElseIf CboType.SelectedIndex = 2 Then
                TxtBatchName.Enabled = False
                TxtDiplomName.Enabled = False
                CmdFindDiplom.Enabled = False
                CmdDelDiplom.Enabled = False
                CmdFindBatch.Enabled = False
                CmdDelBatch.Enabled = False
                TxtCourseName.Enabled = False
                CmdFindCourse.Enabled = False
                CmdDelCourse.Enabled = False
                TxtBatchName.Text = Nothing
                TxtBatchGuid.Text = Nothing
                TxtDiplomName.Text = Nothing
                TxtDiplomGuid.Text = Nothing
                TxtCourseName.Text = Nothing
                TxtCourseGuid.Text = Nothing
            End If

        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try
    End Sub

    Private Sub CmdView_Click(sender As Object, e As EventArgs) Handles CmdView.Click
        Try
            LOADREGDOC()
        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try
    End Sub

    Private Sub CboType_SelectedValueChanged(sender As Object, e As EventArgs) Handles CboType.SelectedValueChanged
        LOADREGDOC()
    End Sub

    Private Sub CmdDelCourse_Click(sender As Object, e As EventArgs) Handles CmdDelCourse.Click
        TxtCourseName.Text = Nothing
        TxtCourseGuid.Text = Nothing
        LOADREGDOC()
    End Sub

    Private Sub CmdDelDiplom_Click(sender As Object, e As EventArgs) Handles CmdDelDiplom.Click
        TxtDiplomName.Text = Nothing
        TxtDiplomGuid.Text = Nothing
        LOADREGDOC()
    End Sub

    Private Sub TxtFind2_TextChanged_1(sender As Object, e As EventArgs) Handles TxtFind2.TextChanged
        Try
            LOADREGDOC()
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
                SaveFileDialog1.FileName = "تقرير التسجيلات"
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
                    Dim columnsToExclude As New List(Of Integer) From {10, 11, 12, 13, 14, 15}

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
End Class