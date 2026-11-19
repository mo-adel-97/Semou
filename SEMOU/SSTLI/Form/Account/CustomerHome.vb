Imports System.Data.SqlClient
Imports Microsoft.Office.Interop

Public Class CustomerHome
    Dim dbcontext As New SemouEntities
    Private Sub CmdExit_Click(sender As Object, e As EventArgs) Handles CmdExit.Click
        Me.Close()
    End Sub
    Sub DAILYTOTAL()
        Dim MADEN As Double = 0
        Dim DAEN As Double = 0
        Dim BALANCE As Double = 0
        Dim PREBALANCE As Double = 0
        For i As Integer = 0 To BranchGrid.Rows.Count - 1
            If BranchGrid.Rows(i).Cells(2).Value <> Nothing Then
                MADEN = Val(MADEN) + (Val(BranchGrid.Rows(i).Cells(6).Value))
                DAEN = Val(DAEN) + (Val(BranchGrid.Rows(i).Cells(7).Value))
                BALANCE = Val(BALANCE) + (Val(BranchGrid.Rows(i).Cells(8).Value))
                PREBALANCE = Val(PREBALANCE) + (Val(BranchGrid.Rows(i).Cells(5).Value))
            End If
        Next
        TxtMaden.Text = Math.Round(Val(MADEN), 2)
        TxtDaen.Text = Math.Round(Val(DAEN), 2)
        TxtBalance.Text = Math.Round(Val(BALANCE), 2)
        TxtPreBalance.Text = Math.Round(Val(PREBALANCE), 2)
    End Sub
    Sub LOADOLDSTUEDENT()
        BranchGrid.Rows.Clear()
        Dim connection = dbcontext.Database.Connection
        Dim comm As New SqlCommand("AllStudentBalance", connection)
        comm.Parameters.AddWithValue("@FDate", Convert.ToDateTime(TxtFromDate.Value))
        comm.Parameters.AddWithValue("@ToDate", Convert.ToDateTime(TxtToDate.Value))
        If TxtFind.Text <> Nothing Then
            If FindByNaId.Checked = True Then
                comm.Parameters.AddWithValue("@NationalId", TxtFind.Text)
            ElseIf FindByName.Checked = True Then
                comm.Parameters.AddWithValue("@StudentName", TxtFind.Text)
            End If
        End If
        If TxtBranchGuid.Text <> Nothing Then
            comm.Parameters.AddWithValue("@BranchGuid", TxtBranchGuid.Text)
            BranchGrid.Columns(4).Visible = False
        Else
            BranchGrid.Columns(4).Visible = True
        End If
        If TxtDiplomGuid.Text <> Nothing Then
            comm.Parameters.AddWithValue("@DiplomGuid", TxtDiplomGuid.Text)
            'BranchGrid.Columns(9).Visible = False
        Else
            'BranchGrid.Columns(9).Visible = True
        End If
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
        adp.Fill(ds, "AllStudentBalance")
        Dim COUNT__ As Integer = 0
        If ds.Tables(0).Rows.Count > 0 Then
            BranchGrid.RowCount = ds.Tables(0).Rows.Count
            'TxtCount.Text = OldDataGrid.Rows.Count()
            For i As Integer = 0 To ds.Tables(0).Rows.Count - 1
                BranchGrid.Rows(COUNT__).Cells(0).Value = ds.Tables(0).Rows(i)(0).ToString()
                BranchGrid.Rows(COUNT__).Cells(1).Value = ds.Tables(0).Rows(i)(1).ToString()
                BranchGrid.Rows(COUNT__).Cells(2).Value = ds.Tables(0).Rows(i)(2).ToString()
                BranchGrid.Rows(COUNT__).Cells(3).Value = ds.Tables(0).Rows(i)(3).ToString()
                BranchGrid.Rows(COUNT__).Cells(4).Value = ds.Tables(0).Rows(i)(4).ToString()
                BranchGrid.Rows(COUNT__).Cells(5).Value = ds.Tables(0).Rows(i)(5).ToString()
                BranchGrid.Rows(COUNT__).Cells(6).Value = ds.Tables(0).Rows(i)(6).ToString()
                BranchGrid.Rows(COUNT__).Cells(7).Value = ds.Tables(0).Rows(i)(7).ToString()
                BranchGrid.Rows(COUNT__).Cells(8).Value = ds.Tables(0).Rows(i)(8).ToString()
                BranchGrid.Rows(COUNT__).Cells(9).Value = ds.Tables(0).Rows(i)(9).ToString()
                'BranchGrid.Rows(COUNT__).Cells(10).Value = ds.Tables(0).Rows(i)(10).ToString()
                COUNT__ = COUNT__ + 1
            Next
        End If
        DAILYTOTAL()
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
                LOADOLDSTUEDENT()
            End If
        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try
    End Sub

    Private Sub CmdDelBranch_Click(sender As Object, e As EventArgs) Handles CmdDelBranch.Click
        TxtBranchName.Text = Nothing
        TxtBranchGuid.Text = Nothing
        BranchGrid.Rows.Clear()
        DAILYTOTAL()
        'LOADOLDSTUEDENT()
    End Sub

    Private Sub CustomerHome_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Try
            'LOADOLDSTUEDENT()
        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try
    End Sub

    Private Sub TxtFind_TextChanged(sender As Object, e As EventArgs) Handles TxtFind.TextChanged
        Try
            If TxtBranchGuid.Text <> Nothing Then
                LOADOLDSTUEDENT()
            Else
                MessageBox.Show("برجاء اختيار فرع اولا", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Error)
                Return
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
            If j = 10 Then
                If AllFiles.FORFIND("studentstatment") = False Then
                    Return
                Else
                    Dim w As New StusdentStatment
                    w.TxtId.Text = BranchGrid.Rows(i).Cells(0).Value.ToString
                    w.TxtStudentName.Text = BranchGrid.Rows(i).Cells(1).Value.ToString
                    w.TxtAccountGuid.Text = BranchGrid.Rows(i).Cells(9).Value.ToString
                    w.TxtNationalId.Text = BranchGrid.Rows(i).Cells(3).Value.ToString
                    w.ShowDialog()
                End If
            End If
        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try
    End Sub

    Private Sub CMDNew_Click(sender As Object, e As EventArgs) Handles CMDNew.Click
        Try
            If BranchGrid.Rows.Count = 0 Then
                MessageBox.Show("لا توجد بيانات لتصديرها", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Error)
                Return
            Else
                ' Open a SaveFileDialog to choose the output location
                SaveFileDialog1.InitialDirectory = "C:"
                SaveFileDialog1.Title = "Save as Excel File"
                SaveFileDialog1.FileName = "أرصدة العملاء"
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

    Private Sub CmdFindDiplom_Click(sender As Object, e As EventArgs) Handles CmdFindDiplom.Click
        Try
            Dim w As New DiplomList
            w.TxtIsUse.Text = True
            w.TxtType.Text = 0
            w.ShowDialog()
            If w.IS_Yes = True Then
                Dim ii As Integer = w.BranchGrid.CurrentRow.Index
                TxtDiplomName.Text = w.BranchGrid.Rows(ii).Cells(1).Value.ToString()
                TxtDiplomGuid.Text = w.BranchGrid.Rows(ii).Cells(3).Value.ToString()
            End If
            If TxtDiplomGuid.Text <> Nothing Then
                LOADOLDSTUEDENT()
            End If
        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try
    End Sub

    Private Sub CmdDelDiplom_Click(sender As Object, e As EventArgs) Handles CmdDelDiplom.Click
        TxtDiplomName.Text = Nothing
        TxtDiplomGuid.Text = Nothing
        If TxtBranchGuid.Text <> Nothing Then
            LOADOLDSTUEDENT()
        End If
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
                LOADOLDSTUEDENT()
            End If
        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try
    End Sub

    Private Sub CmdDelBatch_Click(sender As Object, e As EventArgs) Handles CmdDelBatch.Click
        TxtBatchName.Text = Nothing
        TxtBatchGuid.Text = Nothing
        If TxtBranchGuid.Text <> Nothing Then
            LOADOLDSTUEDENT()
        End If
    End Sub

    Private Sub CmdView_Click(sender As Object, e As EventArgs) Handles CmdView.Click
        Try
            'If TxtBranchGuid.Text <> Nothing Then
            LOADOLDSTUEDENT()
            'Else
            'MessageBox.Show("برجاء اختيار فرع اولا", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Error)
            'Return
            'End If
        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try
    End Sub
End Class