Imports System.Data.SqlClient
Imports DevExpress.XtraReports.UI
Imports Microsoft.Office.Interop

Public Class CashBoxHistory
    Dim dbcontext As New SemouEntities
    Private Sub CmdClose_Click(sender As Object, e As EventArgs) Handles CmdClose.Click
        Me.Close()
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
            If TxtAccountGuid.Text <> Nothing Then
                LOADSTATMENT()
            End If
        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try
    End Sub

    Private Sub CmdFindDepart_Click(sender As Object, e As EventArgs) Handles CmdFindDepart.Click
        Try
            Dim w As New NewBranchList
            w.TxtType.Text = 0
            w.TxtIsUse.Text = True
            w.ShowDialog()
            If w.IS_Yes = True Then
                Dim ii As Integer = w.BranchGrid.CurrentRow.Index
                TxtBranchName.Text = w.BranchGrid.Rows(ii).Cells(1).Value.ToString()
                TxtBranchGuid.Text = w.BranchGrid.Rows(ii).Cells(3).Value.ToString()
            End If
            If TxtBranchGuid.Text <> Nothing Then
                'If TxtAccountGuid.Text <> Nothing Then
                LOADSTATMENT()
                'Else
                '    TxtBranchName.Text = Nothing
                '    TxtBranchGuid.Text = Nothing
                '    MessageBox.Show("برجاء اختيار الخزينة/البنك اولا", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Error)
                '    Return
                'End If
            End If
        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try
    End Sub

    Private Sub CmdFindCostCenter_Click(sender As Object, e As EventArgs) Handles CmdFindCostCenter.Click
        Try
            Dim w As New CostCenterList
            w.TxtAccountType.Text = 1
            w.ShowDialog()
            If w.IS_Yes = True Then
                Dim ii As Integer = w.BranchGrid.CurrentRow.Index
                TxtCostCenterName.Text = w.BranchGrid.Rows(ii).Cells(1).Value.ToString()
                TxtCostCenterGuid.Text = w.BranchGrid.Rows(ii).Cells(3).Value.ToString()
            End If
            If TxtCostCenterGuid.Text <> Nothing Then
                'If TxtAccountGuid.Text <> Nothing Then
                LOADSTATMENT()
                'Else
                '    TxtCostCenterName.Text = Nothing
                '    TxtCostCenterGuid.Text = Nothing
                '    MessageBox.Show("برجاء اختيار الخزينة/البنك اولا", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Error)
                '    Return
                'End If
            End If
        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try
    End Sub

    Private Sub CmdDelBranch_Click(sender As Object, e As EventArgs) Handles CmdDelBranch.Click
        TxtBranchName.Text = Nothing
        TxtBranchGuid.Text = Nothing
        'If TxtAccountGuid.Text <> Nothing Then
        LOADSTATMENT()
        'End If
    End Sub

    Private Sub CmdDelCostCenter_Click(sender As Object, e As EventArgs) Handles CmdDelCostCenter.Click
        TxtCostCenterName.Text = Nothing
        TxtCostCenterGuid.Text = Nothing
        'If TxtAccountGuid.Text <> Nothing Then
        LOADSTATMENT()
        'End If
    End Sub
    Sub LOADSTATMENT()
        DiplomGrid.Rows.Clear()
        Dim BALANCE__ As Double = 0
        Dim connection = dbcontext.Database.Connection
        Dim comm As New SqlCommand("cashBoxHistory", connection)
        comm.Parameters.AddWithValue("@FDate", Convert.ToDateTime(TxtFromDate.Value))
        comm.Parameters.AddWithValue("@TDate", Convert.ToDateTime(TxtToDate.Value))
        If TxtAccountGuid.Text <> Nothing Then
            comm.Parameters.AddWithValue("@AccountGuid", TxtAccountGuid.Text)
            DiplomGrid.Columns(5).Visible = True
        Else
            DiplomGrid.Columns(5).Visible = False
        End If
        If TxtBranchGuid.Text <> Nothing Then
            comm.Parameters.AddWithValue("@BranchGuid", TxtBranchGuid.Text)
            DiplomGrid.Columns(9).Visible = False
        Else
            DiplomGrid.Columns(9).Visible = True
        End If
        If TxtCostCenterGuid.Text <> Nothing Then
            comm.Parameters.AddWithValue("@CostGuid", TxtCostCenterGuid.Text)
            DiplomGrid.Columns(8).Visible = False
        Else
            DiplomGrid.Columns(8).Visible = True
        End If
        If TxtDocTypeGuid.Text <> Nothing Then
            comm.Parameters.AddWithValue("@TypeGuid", TxtDocTypeGuid.Text)
            DiplomGrid.Columns(1).Visible = False
        Else
            DiplomGrid.Columns(1).Visible = True
        End If
        If TxtUserGuid.Text <> Nothing Then
            comm.Parameters.AddWithValue("@UserGuid", TxtUserGuid.Text)
        End If
        comm.CommandType = CommandType.StoredProcedure
        If connection.State = ConnectionState.Open Then
            connection.Close()
        End If
        connection.Open()
        comm.ExecuteNonQuery()
        Dim adp As New SqlDataAdapter(comm)
        Dim ds As DataSet = New DataSet()
        adp.Fill(ds, "cashBoxHistory")
        Dim COUNT__ As Integer = 0
        If ds.Tables(0).Rows.Count > 0 Then
            DiplomGrid.RowCount = ds.Tables(0).Rows.Count
            TxtCount.Text = DiplomGrid.Rows.Count()
            For i As Integer = 0 To ds.Tables(0).Rows.Count - 1
                DiplomGrid.Rows(COUNT__).Cells(0).Value = Convert.ToDateTime(ds.Tables(0).Rows(i)(1)).ToShortDateString()
                DiplomGrid.Rows(COUNT__).Cells(1).Value = ds.Tables(0).Rows(i)(2).ToString()
                DiplomGrid.Rows(COUNT__).Cells(2).Value = ds.Tables(0).Rows(i)(3).ToString()
                DiplomGrid.Rows(COUNT__).Cells(3).Value = ds.Tables(0).Rows(i)(4).ToString()
                DiplomGrid.Rows(COUNT__).Cells(4).Value = ds.Tables(0).Rows(i)(5).ToString()
                BALANCE__ = Val(BALANCE__) + (Val(DiplomGrid.Rows(COUNT__).Cells(3).Value) - Val(DiplomGrid.Rows(COUNT__).Cells(4).Value))
                DiplomGrid.Rows(COUNT__).Cells(5).Value = Math.Round(BALANCE__, 2)
                DiplomGrid.Rows(COUNT__).Cells(6).Value = ds.Tables(0).Rows(i)(6).ToString()
                DiplomGrid.Rows(COUNT__).Cells(7).Value = ds.Tables(0).Rows(i)(7).ToString()
                DiplomGrid.Rows(COUNT__).Cells(8).Value = ds.Tables(0).Rows(i)(8).ToString()
                DiplomGrid.Rows(COUNT__).Cells(9).Value = ds.Tables(0).Rows(i)(9).ToString()
                DiplomGrid.Rows(COUNT__).Cells(10).Value = ds.Tables(0).Rows(i)(10).ToString()
                DiplomGrid.Rows(COUNT__).Cells(11).Value = ds.Tables(0).Rows(i)(11).ToString()
                DiplomGrid.Rows(COUNT__).Cells(12).Value = ds.Tables(0).Rows(i)(12).ToString()
                DiplomGrid.Rows(COUNT__).Cells(13).Value = ds.Tables(0).Rows(i)(13).ToString()
                DiplomGrid.Rows(COUNT__).Cells(14).Value = ds.Tables(0).Rows(i)(14).ToString()
                DiplomGrid.Rows(COUNT__).Cells(15).Value = ds.Tables(0).Rows(i)(15).ToString()
                DiplomGrid.Rows(COUNT__).Cells(16).Value = ds.Tables(0).Rows(i)(16).ToString()
                If (IsDBNull(ds.Tables(0).Rows(i)(17))) Then
                    DiplomGrid.Rows(COUNT__).Cells(17).Value = DiplomGrid.Rows(COUNT__).Cells(0).Value
                Else
                    DiplomGrid.Rows(COUNT__).Cells(17).Value = Convert.ToDateTime(ds.Tables(0).Rows(i)(17)).ToShortDateString()
                End If
                COUNT__ = COUNT__ + 1
            Next
            DAILYTOTAL()
        End If
    End Sub
    Sub DAILYTOTAL()
        Dim DIPLOMTAX As Double = 0
        Dim DIPLOMTOTAL As Double = 0
        For i As Integer = 0 To DiplomGrid.Rows.Count - 1
            If DiplomGrid.Rows(i).Cells(2).Value <> Nothing Then
                DIPLOMTOTAL = Val(DIPLOMTOTAL) + (Val(DiplomGrid.Rows(i).Cells(3).Value))
                DIPLOMTAX = Val(DIPLOMTAX) + (Val(DiplomGrid.Rows(i).Cells(4).Value))
            End If
        Next
        TxtTotalMaden.Text = Math.Round(Val(DIPLOMTOTAL), 2)
        TxtTotalDaen.Text = Math.Round(Val(DIPLOMTAX), 2)
        TxtDef.Text = Math.Round(Val(TxtTotalMaden.Text) - Val(TxtTotalDaen.Text), 2)
    End Sub
    Private Sub CMDNew_Click(sender As Object, e As EventArgs) Handles CMDNew.Click
        Try
            'If TxtAccountGuid.Text <> Nothing Then
            LOADSTATMENT()
            'Else
            '    MessageBox.Show("برجاء اختيار الخزينة/البنك اولا", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Error)
            '    Return
            'End If
        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try
    End Sub
    Private Sub DiplomGrid_CellContentClick(sender As Object, e As DataGridViewCellEventArgs) Handles DiplomGrid.CellContentClick

    End Sub

    Private Sub ChkShowSupp_CheckedChanged(sender As Object, e As EventArgs) Handles ChkShowSupp.CheckedChanged
        Try
            If ChkShowSupp.Checked = True Then
                DiplomGrid.Columns(10).Visible = True
                DiplomGrid.Columns(11).Visible = True
                DiplomGrid.Columns(12).Visible = True
            Else
                DiplomGrid.Columns(10).Visible = False
                DiplomGrid.Columns(11).Visible = False
                DiplomGrid.Columns(12).Visible = False
            End If
        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try
    End Sub

    Private Sub ChkShowBill_CheckedChanged(sender As Object, e As EventArgs) Handles ChkShowBill.CheckedChanged
        Try
            If ChkShowBill.Checked = True Then
                DiplomGrid.Columns(13).Visible = True
                DiplomGrid.Columns(14).Visible = True
                DiplomGrid.Columns(15).Visible = True
                DiplomGrid.Columns(16).Visible = True
                DiplomGrid.Columns(17).Visible = True
                DiplomGrid.Columns(7).Visible = True
                DiplomGrid.Columns(6).Visible = False
            Else
                DiplomGrid.Columns(13).Visible = False
                DiplomGrid.Columns(14).Visible = False
                DiplomGrid.Columns(15).Visible = False
                DiplomGrid.Columns(16).Visible = False
                DiplomGrid.Columns(17).Visible = False
                DiplomGrid.Columns(7).Visible = False
                DiplomGrid.Columns(6).Visible = True
            End If
        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try
    End Sub

    Private Sub CmdFindDocType_Click(sender As Object, e As EventArgs) Handles CmdFindDocType.Click
        Try
            Dim w As New DocTypeList
            w.ShowDialog()
            If w.IS_Yes = True Then
                Dim ii As Integer = w.BranchGrid.CurrentRow.Index
                TxtDocTypeName.Text = w.BranchGrid.Rows(ii).Cells(0).Value.ToString()
                TxtDocTypeGuid.Text = w.BranchGrid.Rows(ii).Cells(1).Value.ToString()
            End If
            If TxtDocTypeGuid.Text <> Nothing Then
                'If TxtAccountGuid.Text <> Nothing Then
                LOADSTATMENT()
                'Else
                '    TxtDocTypeName.Text = Nothing
                '    TxtDocTypeGuid.Text = Nothing
                '    MessageBox.Show("برجاء اختيار الخزينة/البنك اولا", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Error)
                '    Return
                'End If
            End If
        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try
    End Sub

    Private Sub CmdDelDoc_Click(sender As Object, e As EventArgs) Handles CmdDelDoc.Click
        TxtDocTypeName.Text = Nothing
        TxtDocTypeGuid.Text = Nothing
        LOADSTATMENT()
    End Sub

    Private Sub CmdDelBank_Click(sender As Object, e As EventArgs) Handles CmdDelBank.Click
        TxtAccountGuid.Text = Nothing
        TxtAccountName.Text = Nothing
        LOADSTATMENT()
    End Sub
    Sub PRINT()
        Dim BALANCE__ As Double = 0
        Dim connection = dbcontext.Database.Connection
        Dim comm As New SqlCommand("cashBoxHistory", connection)
        comm.Parameters.AddWithValue("@FDate", Convert.ToDateTime(TxtFromDate.Value))
        comm.Parameters.AddWithValue("@TDate", Convert.ToDateTime(TxtToDate.Value))
        If TxtAccountGuid.Text <> Nothing Then
            comm.Parameters.AddWithValue("@AccountGuid", TxtAccountGuid.Text)
        End If
        If TxtBranchGuid.Text <> Nothing Then
            comm.Parameters.AddWithValue("@BranchGuid", TxtBranchGuid.Text)
        End If
        If TxtCostCenterGuid.Text <> Nothing Then
            comm.Parameters.AddWithValue("@CostGuid", TxtCostCenterGuid.Text)
        End If
        If TxtDocTypeGuid.Text <> Nothing Then
            comm.Parameters.AddWithValue("@TypeGuid", TxtDocTypeGuid.Text)
        End If
        If TxtUserGuid.Text <> Nothing Then
            comm.Parameters.AddWithValue("@UserGuid", TxtUserGuid.Text)
        End If
        comm.CommandType = CommandType.StoredProcedure
        If connection.State = ConnectionState.Open Then
            connection.Close()
        End If
        connection.Open()
        comm.ExecuteNonQuery()
        Dim adp As New SqlDataAdapter(comm)
        Dim ds As DataSet = New DataSet()
        adp.Fill(ds, "cashBoxHistory")
        Dim dt = ds.Tables(0)
        Dim report As New CashBoxHistoryReport
        report.DataSource = dt
        report.DataAdapter = adp
        report.DataMember = "DataTable1"
        report.RequestParameters = False
        report.Parameters(0).Value = TxtFromDate.Text
        report.Parameters(1).Value = TxtToDate.Text
        If TxtAccountName.Text <> Nothing Then
            report.Parameters(2).Value = TxtAccountName.Text
        Else
            report.Parameters(2).Value = "الكل"
        End If
        If TxtBranchName.Text <> Nothing Then
            report.Parameters(3).Value = TxtBranchName.Text
        Else
            report.Parameters(3).Value = "الكل"
        End If
        If TxtCostCenterName.Text <> Nothing Then
            report.Parameters(4).Value = TxtCostCenterName.Text
        Else
            report.Parameters(4).Value = "الكل"
        End If
        If TxtDocTypeName.Text <> Nothing Then
            report.Parameters(5).Value = TxtDocTypeName.Text
        Else
            report.Parameters(5).Value = "الكل"
        End If
        report.Parameters(6).Value = TxtTotalMaden.Text
        report.Parameters(7).Value = TxtTotalDaen.Text
        report.Parameters(8).Value = TxtDef.Text
        Dim Tool_Report As ReportPrintTool = New ReportPrintTool(report)
        Dim detail As DetailBand = TryCast(report.Bands(BandKind.Detail), DetailBand)
        report.CreateDocument()
        Dim frm As New ViewReport
        frm.DocumentViewer1.DocumentSource = report
        frm.ShowDialog()
    End Sub
    Private Sub CmdPrint_Click(sender As Object, e As EventArgs) Handles CmdPrint.Click
        Try
            PRINT()
        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try
    End Sub

    Private Sub CmdFindUser_Click(sender As Object, e As EventArgs) Handles CmdFindUser.Click
        Try
            Dim w As New UserList
            w.ShowDialog()
            If w.IS_Yes = True Then
                Dim ii As Integer = w.BranchGrid.CurrentRow.Index
                TxtUserName.Text = w.BranchGrid.Rows(ii).Cells(1).Value.ToString()
                TxtUserGuid.Text = w.BranchGrid.Rows(ii).Cells(3).Value.ToString()
            End If
            If TxtUserGuid.Text <> Nothing Then
                LOADSTATMENT()
            End If
        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try
    End Sub

    Private Sub CmdDelUser_Click(sender As Object, e As EventArgs) Handles CmdDelUser.Click
        TxtUserName.Text = Nothing
        TxtUserGuid.Text = Nothing
        LOADSTATMENT()
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
                SaveFileDialog1.FileName = CboType.Text
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
                    'عام
                    Dim columnsToExclude As New List(Of Integer) From {7, 10, 11, 12, 13, 14, 15, 16, 17}
                    ' مخصص تحصيل
                    Dim columnsToExclude1 As New List(Of Integer) From {10, 11, 12}
                    ' مخصص مصروف
                    Dim columnsToExclude2 As New List(Of Integer) From {7, 13, 14, 15, 16, 17}
                    For i As Integer = 0 To DiplomGrid.Columns.Count - 1
                        Select Case CboType.SelectedIndex
                        ' عام
                            Case 0
                                If Not columnsToExclude.Contains(i) Then
                                    sheet.Cells(1, i + 1) = DiplomGrid.Columns(i).HeaderText
                                End If
                        ' مخصص تحصيل
                            Case 1
                                If Not columnsToExclude1.Contains(i) Then
                                    sheet.Cells(1, i + 1) = DiplomGrid.Columns(i).HeaderText
                                End If
                         ' مخصص مصروف
                            Case 2
                                If Not columnsToExclude2.Contains(i) Then
                                    sheet.Cells(1, i + 1) = DiplomGrid.Columns(i).HeaderText
                                End If
                        End Select
                    Next
                    ' Write data from DataGridView rows
                    For i As Integer = 0 To DiplomGrid.Rows.Count - 1
                        For j As Integer = 0 To DiplomGrid.Columns.Count - 1
                            Select Case CboType.SelectedIndex
                        ' عام
                                Case 0
                                    If Not columnsToExclude.Contains(j) Then
                                        sheet.Cells(i + 2, j + 1) = DiplomGrid.Rows(i).Cells(j).Value.ToString()
                                    End If
                        ' مخصص تحصيل
                                Case 1
                                    If Not columnsToExclude1.Contains(j) Then
                                        sheet.Cells(i + 2, j + 1) = DiplomGrid.Rows(i).Cells(j).Value.ToString()
                                    End If
                         ' مخصص مصروف
                                Case 2
                                    If Not columnsToExclude2.Contains(j) Then
                                        sheet.Cells(i + 2, j + 1) = DiplomGrid.Rows(i).Cells(j).Value.ToString()
                                    End If
                            End Select
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

    Private Sub CashBoxHistory_Load(sender As Object, e As EventArgs) Handles MyBase.Load

    End Sub
End Class