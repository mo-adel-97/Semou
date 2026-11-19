Imports System.Data.SqlClient
Imports Microsoft.Office.Interop

Public Class GeneralAccountStatment
    Dim dbcontext As New SemouEntities
    Private Sub CmdFindAccount_Click(sender As Object, e As EventArgs) Handles CmdFindAccount.Click
        Try
            Dim w As New AccountList
            w.TxtAccountType2.Text = 2
            w.TxtIsUse.Text = True
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
        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try
    End Sub

    Private Sub CmdDelBranch_Click(sender As Object, e As EventArgs) Handles CmdDelBranch.Click
        TxtBranchName.Text = Nothing
        TxtBranchGuid.Text = Nothing
    End Sub

    Private Sub CmdDelCostCenter_Click(sender As Object, e As EventArgs) Handles CmdDelCostCenter.Click
        TxtCostCenterName.Text = Nothing
        TxtCostCenterGuid.Text = Nothing
    End Sub
    Sub LOADSTATMENT()
        DiplomGrid.Rows.Clear()
        Dim BALANCE__ As Double = 0
        Dim connection = dbcontext.Database.Connection
        Dim comm As New SqlCommand("GenralAccountStatment", connection)
        comm.Parameters.AddWithValue("@FDate", Convert.ToDateTime(TxtFromDate.Value))
        comm.Parameters.AddWithValue("@TDate", Convert.ToDateTime(TxtToDate.Value))
        comm.Parameters.AddWithValue("@AccountGuid", TxtAccountGuid.Text)
        If TxtBranchGuid.Text <> Nothing Then
            comm.Parameters.AddWithValue("@BranchGuid", TxtBranchGuid.Text)
        End If
        If TxtCostCenterGuid.Text <> Nothing Then
            comm.Parameters.AddWithValue("@CostGuid", TxtCostCenterGuid.Text)
        End If
        comm.CommandType = CommandType.StoredProcedure
        If connection.State = ConnectionState.Open Then
            connection.Close()
        End If
        connection.Open()
        comm.ExecuteNonQuery()
        Dim adp As New SqlDataAdapter(comm)
        Dim ds As DataSet = New DataSet()
        adp.Fill(ds, "GenralAccountStatment")
        Dim COUNT__ As Integer = 0
        ' إضافة صف أول للرصيد الافتتاحي باستخدام القيم من TextBox
        Dim openingRow As DataGridViewRow = New DataGridViewRow()
        openingRow.CreateCells(DiplomGrid)
        openingRow.Cells(0).Value = "" ' عمود الوصف أو الاسم
        openingRow.Cells(1).Value = "" ' عمود التاريخ
        openingRow.Cells(2).Value = "الرصيد السابق" ' عمود فارغ إذا لزم الأمر
        openingRow.Cells(3).Value = "" ' عمود فارغ إذا لزم الأمر
        openingRow.Cells(4).Value = TxtPreMaden.Text ' القيمة الافتتاحية المدخلة من TextBox
        openingRow.Cells(5).Value = TxtPreDaen.Text ' القيمة الافتتاحية الأخرى (مقيدة بـ 0)
        BALANCE__ = Val(TxtPreMaden.Text) - Val(TxtPreDaen.Text) ' تحديث الرصيد الافتتاحي) 
        openingRow.Cells(6).Value = Math.Round(BALANCE__, 2) ' وضع الرصيد الافتتاحي
        openingRow.Cells(7).Value = TxtOpenNotes.Text
        openingRow.Cells(8).Value = "" ' عمود فارغ إذا لزم الأمر
        openingRow.Cells(9).Value = "" ' عمود فارغ إذا لزم الأمر
        ' إدراج الصف في أول موضع في DataGridView
        DiplomGrid.Rows.Insert(0, openingRow)

        If ds.Tables(0).Rows.Count > 0 Then
            DiplomGrid.RowCount = ds.Tables(0).Rows.Count + 1 ' +1 لإضافة الصف الجديد
            COUNT__ = 1 ' ابدأ من الصف الثاني بعد الرصيد الافتتاحي
            For i As Integer = 0 To ds.Tables(0).Rows.Count - 1
                DiplomGrid.Rows(COUNT__).Cells(0).Value = ds.Tables(0).Rows(i)(0).ToString()
                DiplomGrid.Rows(COUNT__).Cells(1).Value = Convert.ToDateTime(ds.Tables(0).Rows(i)(1)).ToShortDateString()
                DiplomGrid.Rows(COUNT__).Cells(2).Value = ds.Tables(0).Rows(i)(2).ToString()
                DiplomGrid.Rows(COUNT__).Cells(3).Value = ds.Tables(0).Rows(i)(3).ToString()
                DiplomGrid.Rows(COUNT__).Cells(4).Value = ds.Tables(0).Rows(i)(4).ToString()
                DiplomGrid.Rows(COUNT__).Cells(5).Value = ds.Tables(0).Rows(i)(5).ToString()
                BALANCE__ = Val(BALANCE__) + (Val(DiplomGrid.Rows(COUNT__).Cells(4).Value) - Val(DiplomGrid.Rows(COUNT__).Cells(5).Value))
                DiplomGrid.Rows(COUNT__).Cells(6).Value = Math.Round(BALANCE__, 2)
                DiplomGrid.Rows(COUNT__).Cells(7).Value = ds.Tables(0).Rows(i)(6).ToString()
                DiplomGrid.Rows(COUNT__).Cells(8).Value = ds.Tables(0).Rows(i)(7).ToString()
                DiplomGrid.Rows(COUNT__).Cells(9).Value = ds.Tables(0).Rows(i)(8).ToString()
                COUNT__ = COUNT__ + 1
            Next
            DAILYTOTAL()
        End If
    End Sub
    Sub LOADOPENBALANCE()
        Dim BALANCE__ As Double = 0
        Dim connection = dbcontext.Database.Connection
        Dim comm As New SqlCommand("ShowOpenBalnceStatment", connection)
        comm.Parameters.AddWithValue("@AccountGuid", TxtAccountGuid.Text)
        If TxtBranchGuid.Text <> Nothing Then
            comm.Parameters.AddWithValue("@BranchGuid", TxtBranchGuid.Text)
        End If
        comm.CommandType = CommandType.StoredProcedure
        If connection.State = ConnectionState.Open Then
            connection.Close()
        End If
        connection.Open()
        comm.ExecuteNonQuery()
        Dim adp As New SqlDataAdapter(comm)
        Dim ds As DataSet = New DataSet()
        adp.Fill(ds, "ShowOpenBalnceStatment")
        Dim COUNT__ As Integer = 0
        If ds.Tables(0).Rows.Count > 0 Then
            For i As Integer = 0 To ds.Tables(0).Rows.Count - 1
                TxtOpenMaden.Text = ds.Tables(0).Rows(i)(0).ToString()
                TxtOpenDaen.Text = ds.Tables(0).Rows(i)(1).ToString()
                TxtOpenNotes.Text = "رصيد اول المدة"
                COUNT__ = COUNT__ + 1
            Next
        End If
    End Sub
    Sub LOADBEFORBALANCE()
        Dim BALANCE__ As Double = 0
        Dim connection = dbcontext.Database.Connection
        Dim comm As New SqlCommand("BeforAccountStatmentBalance", connection)
        comm.Parameters.AddWithValue("@FDate", Convert.ToDateTime(TxtFromDate.Value))
        comm.Parameters.AddWithValue("@AccountGuid", TxtAccountGuid.Text)
        If TxtBranchGuid.Text <> Nothing Then
            comm.Parameters.AddWithValue("@BranchGuid", TxtBranchGuid.Text)
        End If
        If TxtCostCenterGuid.Text <> Nothing Then
            comm.Parameters.AddWithValue("@CostGuid", TxtCostCenterGuid.Text)
        End If
        comm.CommandType = CommandType.StoredProcedure
        If connection.State = ConnectionState.Open Then
            connection.Close()
        End If
        connection.Open()
        comm.ExecuteNonQuery()
        Dim adp As New SqlDataAdapter(comm)
        Dim ds As DataSet = New DataSet()
        adp.Fill(ds, "BeforAccountStatmentBalance")
        Dim COUNT__ As Integer = 0
        If ds.Tables(0).Rows.Count > 0 Then
            For i As Integer = 0 To ds.Tables(0).Rows.Count - 1
                TxtPreMaden.Text = ds.Tables(0).Rows(i)(0).ToString()
                TxtPreDaen.Text = ds.Tables(0).Rows(i)(1).ToString()
                TxtOpenNotes.Text = "الرصيد السابق"
                COUNT__ = COUNT__ + 1
            Next
        End If
    End Sub
    Sub DAILYTOTAL()
        Dim DIPLOMTAX As Double = 0
        Dim DIPLOMTOTAL As Double = 0
        For i As Integer = 0 To DiplomGrid.Rows.Count - 1
            If DiplomGrid.Rows(i).Cells(2).Value <> Nothing Then
                DIPLOMTOTAL = Val(DIPLOMTOTAL) + (Val(DiplomGrid.Rows(i).Cells(4).Value))
                DIPLOMTAX = Val(DIPLOMTAX) + (Val(DiplomGrid.Rows(i).Cells(5).Value))
            End If
        Next
        TxtTotalMaden.Text = Math.Round(Val(DIPLOMTOTAL), 2)
        TxtTotalDaen.Text = Math.Round(Val(DIPLOMTAX), 2)
        TxtDef.Text = Math.Round(Val(TxtTotalMaden.Text) - Val(TxtTotalDaen.Text), 2)
    End Sub
    Private Sub GeneralAccountStatment_Load(sender As Object, e As EventArgs) Handles MyBase.Load

    End Sub

    Private Sub CMDNew_Click(sender As Object, e As EventArgs) Handles CMDNew.Click
        Try
            If TxtAccountGuid.Text <> Nothing Then
                'LOADOPENBALANCE()
                LOADBEFORBALANCE()
                LOADSTATMENT()
                DAILYTOTAL()
            Else
                MessageBox.Show("برجاء اختيار الحساب اولا", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Error)
                Return
            End If
        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try
    End Sub

    Private Sub CmdExit_Click(sender As Object, e As EventArgs) Handles CmdExit.Click
        Me.Close()
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
                SaveFileDialog1.FileName = "كشف حساب"
                SaveFileDialog1.Filter = "Excel Files(2003)|*.xls| Excel Files(2007)|*.xlsx"

                If SaveFileDialog1.ShowDialog() <> DialogResult.Cancel Then
                    ' إعداد ProgressBar
                    ProgressBar1.Visible = True
                    ProgressBar1.Minimum = 0
                    ProgressBar1.Maximum = DiplomGrid.Rows.Count
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
                    Dim headerArray(0, DiplomGrid.Columns.Count - 1) As Object

                    For i As Integer = 0 To DiplomGrid.Columns.Count - 1
                        'If Not columnsToExclude.Contains(i) Then
                        headerArray(0, i) = DiplomGrid.Columns(i).HeaderText
                        'End If
                    Next

                    ' كتابة البيانات من DataGridView إلى مصفوفة
                    Dim dataArray(DiplomGrid.Rows.Count - 1, DiplomGrid.Columns.Count - 1) As Object
                    For i As Integer = 0 To DiplomGrid.Rows.Count - 1
                        For j As Integer = 0 To DiplomGrid.Columns.Count - 1
                            'If Not columnsToExclude.Contains(j) Then
                            dataArray(i, j) = DiplomGrid.Rows(i).Cells(j).Value
                            'End If
                        Next
                        ' تحديث ProgressBar بعد كل صف
                        ProgressBar1.PerformStep()
                    Next

                    ' نسخ البيانات دفعة واحدة إلى Excel
                    sheet.Range("A1").Resize(1, DiplomGrid.Columns.Count).Value = headerArray
                    sheet.Range("A2").Resize(DiplomGrid.Rows.Count, DiplomGrid.Columns.Count).Value = dataArray

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