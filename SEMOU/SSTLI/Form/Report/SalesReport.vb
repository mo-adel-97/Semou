Imports System.Data.SqlClient
Imports Microsoft.Office.Interop

Public Class SalesReport
    Dim dbcontext As New SemouEntities
    Private Sub CmdExit_Click(sender As Object, e As EventArgs) Handles CmdExit.Click
        Me.Close()
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
    Sub LOADREGDOC()
        BranchGrid.Rows.Clear()
        Dim connection = dbcontext.Database.Connection
        Dim comm As New SqlCommand("ShowSalesReport", connection)
        comm.Parameters.AddWithValue("@FDate", Convert.ToDateTime(TxtFromDate.Value))
        comm.Parameters.AddWithValue("@TDate", Convert.ToDateTime(TxtToDate.Value))
        If TxtBranchGuid.Text <> Nothing Then
            comm.Parameters.AddWithValue("@BranchGuid", TxtBranchGuid.Text)
            BranchGrid.Columns(10).Visible = False
        Else
            BranchGrid.Columns(10).Visible = True
        End If
        If TxtUserGuid.Text <> Nothing Then
            comm.Parameters.AddWithValue("@UserGuid", TxtUserGuid.Text)
            BranchGrid.Columns(11).Visible = False
        Else
            BranchGrid.Columns(11).Visible = True
        End If
        If TxtDocGuid.Text <> Nothing Then
            comm.Parameters.AddWithValue("@DocGuid", TxtDocGuid.Text)
            BranchGrid.Columns(2).Visible = False
        Else
            BranchGrid.Columns(2).Visible = True
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
        If ChkStartPay.Checked = True Then
            comm.Parameters.AddWithValue("@ChkStartPay", 0)
        End If
        If ChkMoPay.Checked = True Then
            comm.Parameters.AddWithValue("@ChkMonPay", 1)
        End If
        If ChkFessPay.Checked = True Then
            comm.Parameters.AddWithValue("@ChkFeesPay", 2)
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
                ' رقم الفاتورة الضريبية للعرض - والرقم الداخلي في العمود المخفي لزرار عرض
                Dim billCode As String = If(ds.Tables(0).Columns.Contains("InternalCode"), ds.Tables(0).Rows(i)("InternalCode"), ds.Tables(0).Rows(i)(1)).ToString()
                BranchGrid.Rows(COUNT__).Cells(1).Value = If(taxMap.ContainsKey(billCode.Trim()), taxMap(billCode.Trim()), ds.Tables(0).Rows(i)(1).ToString())
                BranchGrid.Rows(COUNT__).Cells(12).Value = billCode
                BranchGrid.Rows(COUNT__).Cells(2).Value = ds.Tables(0).Rows(i)(2).ToString()
                BranchGrid.Rows(COUNT__).Cells(3).Value = ds.Tables(0).Rows(i)(3).ToString()
                BranchGrid.Rows(COUNT__).Cells(4).Value = ds.Tables(0).Rows(i)(4).ToString()
                BranchGrid.Rows(COUNT__).Cells(5).Value = ds.Tables(0).Rows(i)(5).ToString()
                BranchGrid.Rows(COUNT__).Cells(6).Value = ds.Tables(0).Rows(i)(6).ToString()
                BranchGrid.Rows(COUNT__).Cells(7).Value = ds.Tables(0).Rows(i)(7).ToString()
                BranchGrid.Rows(COUNT__).Cells(8).Value = ds.Tables(0).Rows(i)(8).ToString()
                BranchGrid.Rows(COUNT__).Cells(9).Value = ds.Tables(0).Rows(i)(9).ToString()
                BranchGrid.Rows(COUNT__).Cells(10).Value = ds.Tables(0).Rows(i)(10).ToString()
                BranchGrid.Rows(COUNT__).Cells(11).Value = ds.Tables(0).Rows(i)(11).ToString()
                'BranchGrid.Rows(COUNT__).Cells(13).Value = ds.Tables(0).Rows(i)(12).ToString()
                'BranchGrid.Rows(COUNT__).Cells(14).Value = ds.Tables(0).Rows(i)(13).ToString()
                COUNT__ = COUNT__ + 1
            Next
            DAILYTOTAL()
        Else
            TxtMsg.Visible = True
            TxtCount.Text = 0
            DAILYTOTAL()
        End If
    End Sub
    Private Sub CenterProgressBar()
        ' حساب الموقع الجديد للـ ProgressBar بحيث يكون في منتصف النموذج
        ProgressBar1.Left = (Me.ClientSize.Width - ProgressBar1.Width) \ 2
        ProgressBar1.Top = (Me.ClientSize.Height - ProgressBar1.Height) \ 2
    End Sub
    Private Sub SalesReport_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Try
            ' استدعاء الدالة عند تحميل النموذج
            CenterProgressBar()
            RegDocPosted.WHATFORMTYPEGUID("salesinvoice")
            TxtTypeGuid.Text = RegDocPosted.TYPEGUID
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
        TxtUserGuid.Text = Nothing
        TxtUserName.Text = Nothing
        LOADREGDOC()
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
        TxtDocGuid.Text = Nothing
        TxtDocName.Text = Nothing
        LOADREGDOC()
    End Sub

    Private Sub TxtFind2_TextChanged(sender As Object, e As EventArgs) Handles TxtFind2.TextChanged
        LOADREGDOC()
    End Sub

    Private Sub BranchGrid_CellContentClick(sender As Object, e As DataGridViewCellEventArgs) Handles BranchGrid.CellContentClick

    End Sub

    Private Sub BranchGrid_CellClick(sender As Object, e As DataGridViewCellEventArgs) Handles BranchGrid.CellClick
        Try
            Dim i As Integer = BranchGrid.CurrentCell.RowIndex
            Dim j As Integer = BranchGrid.CurrentCell.ColumnIndex
            If j = 13 Then
                Dim w As New EncodingNewPayInvoice
                w.TxtCode.Text = If(BranchGrid.Rows(i).Cells(12).Value, BranchGrid.Rows(i).Cells(1).Value).ToString
                w.SHOWBILLINFO()
                w.SHOWBILLDIPLOM()
                w.PAYTOTAL()
                w.ShowDialog()
            End If
        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try
    End Sub

    Private Sub ChkActiveImvoice_CheckedChanged(sender As Object, e As EventArgs) Handles ChkActiveImvoice.CheckedChanged
        Try
            If ChkActiveImvoice.Checked = True Then
                LOADREGDOC()
            End If
        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try
    End Sub

    Private Sub ChkOflineInvoice_CheckedChanged(sender As Object, e As EventArgs) Handles ChkOflineInvoice.CheckedChanged
        Try
            If ChkOflineInvoice.Checked = True Then
                LOADREGDOC()
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
                SaveFileDialog1.FileName = "تقرير المبيعات"
                SaveFileDialog1.Filter = "Excel Files(2003)|*.xls| Excel Files(2007)|*.xlsx"

                If SaveFileDialog1.ShowDialog() <> DialogResult.Cancel Then
                    ' إعداد ProgressBar
                    ProgressBar1.Visible = True
                    ProgressBar1.Minimum = 0
                    ProgressBar1.Maximum = BranchGrid.Rows.Count
                    ProgressBar1.Value = 0
                    ProgressBar1.Step = 1
                    ' إنشاء مثيل جديد لتطبيق Excel
                    Dim ExcelApp As New Excel.Application()
                    ExcelApp.ScreenUpdating = False ' تعطيل تحديث الشاشة لتحسين الأداء
                    ExcelApp.DisplayAlerts = False ' تعطيل التنبيهات

                    ' إضافة ورقة عمل جديدة
                    Dim workbook As Excel.Workbook = ExcelApp.Workbooks.Add(Type.Missing)
                    Dim sheet As Excel.Worksheet = workbook.Sheets(1)

                    ' كتابة رؤوس الأعمدة
                    Dim columnsToExclude As New List(Of Integer) From {12, 13}
                    Dim headerArray(0, BranchGrid.Columns.Count - 1) As Object

                    For i As Integer = 0 To BranchGrid.Columns.Count - 1
                        If Not columnsToExclude.Contains(i) Then
                            headerArray(0, i) = BranchGrid.Columns(i).HeaderText
                        End If
                    Next

                    ' كتابة البيانات من DataGridView إلى مصفوفة
                    Dim dataArray(BranchGrid.Rows.Count - 1, BranchGrid.Columns.Count - 1) As Object
                    For i As Integer = 0 To BranchGrid.Rows.Count - 1
                        For j As Integer = 0 To BranchGrid.Columns.Count - 1
                            If Not columnsToExclude.Contains(j) Then
                                dataArray(i, j) = BranchGrid.Rows(i).Cells(j).Value
                            End If
                        Next
                        ' تحديث ProgressBar بعد كل صف
                        ProgressBar1.PerformStep()
                    Next

                    ' نسخ البيانات دفعة واحدة إلى Excel
                    sheet.Range("A1").Resize(1, BranchGrid.Columns.Count).Value = headerArray
                    sheet.Range("A2").Resize(BranchGrid.Rows.Count, BranchGrid.Columns.Count).Value = dataArray

                    ' ضبط عرض الأعمدة تلقائيًا
                    sheet.Columns.AutoFit()

                    ' حفظ الملف وإظهار رسالة النجاح
                    workbook.SaveAs(SaveFileDialog1.FileName)
                    MessageBox.Show("تم تصدير البيانات بنجاح")

                    ' سؤال المستخدم ما إذا كان يريد عرض الملف
                    Dim result As DialogResult = MessageBox.Show("هل تريد عرض الملف", "تنبيه", MessageBoxButtons.OKCancel, MessageBoxIcon.Question)
                    If result = DialogResult.OK Then
                        ExcelApp.Visible = True
                    Else
                        ExcelApp.Quit()
                    End If

                    ' تنظيف الموارد
                    workbook = Nothing
                    sheet = Nothing
                    ExcelApp = Nothing
                    ' إخفاء ProgressBar بعد الانتهاء
                    ProgressBar1.Visible = False
                End If
            End If
        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try
    End Sub
End Class