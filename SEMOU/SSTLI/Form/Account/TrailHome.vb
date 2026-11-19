Imports System.Data.SqlClient
Imports Microsoft.Office.Interop

Public Class TrailHome
    Dim dbcontext As New SemouEntities
    Sub LOADTRAIL()
        DiplomGrid.Rows.Clear()
        Dim connection = dbcontext.Database.Connection
        Dim comm As New SqlCommand("LoadTrail", connection)
        comm.Parameters.AddWithValue("@Date1", Convert.ToDateTime(TxtFromDate.Value))
        comm.Parameters.AddWithValue("@Date2", Convert.ToDateTime(TxtToDate.Value))
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
        adp.Fill(ds, "LoadTrail")
        Dim COUNT__ As Integer = 0
        If ds.Tables(0).Rows.Count > 0 Then
            DiplomGrid.RowCount = ds.Tables(0).Rows.Count
            For i As Integer = 0 To ds.Tables(0).Rows.Count - 1
                DiplomGrid.Rows(COUNT__).Cells(0).Value = ds.Tables(0).Rows(i)(0).ToString()
                DiplomGrid.Rows(COUNT__).Cells(1).Value = ds.Tables(0).Rows(i)(1).ToString()
                Dim OPENDEF As Double = Val(ds.Tables(0).Rows(i)(3)) - Val(ds.Tables(0).Rows(i)(4))
                If Val(OPENDEF) = 0 Then
                    DiplomGrid.Rows(COUNT__).Cells(2).Value = 0
                    DiplomGrid.Rows(COUNT__).Cells(3).Value = 0
                ElseIf Val(OPENDEF) < 0 Then
                    DiplomGrid.Rows(COUNT__).Cells(2).Value = 0
                    DiplomGrid.Rows(COUNT__).Cells(3).Value = Math.Abs(Math.Round(OPENDEF, 2))
                ElseIf Val(OPENDEF) > 0 Then
                    DiplomGrid.Rows(COUNT__).Cells(2).Value = Math.Round(OPENDEF, 2)
                    DiplomGrid.Rows(COUNT__).Cells(3).Value = 0
                End If
                'DiplomGrid.Rows(COUNT__).Cells(2).Value = Math.Round(ds.Tables(0).Rows(i)(3), 2)
                'DiplomGrid.Rows(COUNT__).Cells(3).Value = Math.Round(ds.Tables(0).Rows(i)(4), 2)
                DiplomGrid.Rows(COUNT__).Cells(4).Value = Math.Round(ds.Tables(0).Rows(i)(5), 2)
                DiplomGrid.Rows(COUNT__).Cells(5).Value = Math.Round(ds.Tables(0).Rows(i)(6), 2)
                Dim TOTALMADEN As Double = Math.Round(ds.Tables(0).Rows(i)(5), 2) + Val(DiplomGrid.Rows(COUNT__).Cells(2).Value)
                Dim TOTALDAEN As Double = Math.Round(ds.Tables(0).Rows(i)(6), 2) + Val(DiplomGrid.Rows(COUNT__).Cells(3).Value)
                Dim DEF As Double = Math.Round((Val(TOTALMADEN) - Val(TOTALDAEN)), 2)
                If Val(DEF) = 0 Then
                    DiplomGrid.Rows(COUNT__).Cells(6).Value = 0
                    DiplomGrid.Rows(COUNT__).Cells(7).Value = 0
                ElseIf Val(DEF) < 0 Then
                    DiplomGrid.Rows(COUNT__).Cells(6).Value = 0
                    DiplomGrid.Rows(COUNT__).Cells(7).Value = Math.Abs(DEF)
                ElseIf Val(DEF) > 0 Then
                    DiplomGrid.Rows(COUNT__).Cells(6).Value = DEF
                    DiplomGrid.Rows(COUNT__).Cells(7).Value = 0
                End If
                COUNT__ = COUNT__ + 1
            Next
            TRAILTOTAL()
        End If
    End Sub
    Sub TRAILTOTAL()
        Dim OPENMADEN As Double = 0
        Dim OPENDAEN As Double = 0
        Dim TRANSMADEN As Double = 0
        Dim TRANSDAEN As Double = 0
        Dim BALANCEMADEN As Double = 0
        Dim BALANCEDAEN As Double = 0
        For i As Integer = 0 To DiplomGrid.Rows.Count - 1
            If DiplomGrid.Rows(i).Cells(0).Value <> Nothing Then
                OPENMADEN = Val(OPENMADEN) + (Val(DiplomGrid.Rows(i).Cells(2).Value))
                OPENDAEN = Val(OPENDAEN) + (Val(DiplomGrid.Rows(i).Cells(3).Value))
                TRANSMADEN = Val(TRANSMADEN) + (Val(DiplomGrid.Rows(i).Cells(4).Value))
                TRANSDAEN = Val(TRANSDAEN) + (Val(DiplomGrid.Rows(i).Cells(5).Value))
                BALANCEMADEN = Val(BALANCEMADEN) + (Val(DiplomGrid.Rows(i).Cells(6).Value))
                BALANCEDAEN = Val(BALANCEDAEN) + (Val(DiplomGrid.Rows(i).Cells(7).Value))
            End If
        Next
        TxtOpenMaden.Text = Math.Round(OPENMADEN, 2)
        TxtOpenDaen.Text = Math.Round(OPENDAEN, 2)
        TxtTransMaden.Text = Math.Round(TRANSMADEN, 2)
        TxtTransDaen.Text = Math.Round(TRANSDAEN, 2)
        TxtBalanceMaden.Text = Math.Round(BALANCEMADEN, 2)
        TxtBalanceDaen.Text = Math.Round(BALANCEDAEN, 2)
    End Sub
    Private Sub TrailHome_Load(sender As Object, e As EventArgs) Handles MyBase.Load

    End Sub
    Sub LOADfackTRAIL()
        DiplomGrid.Rows.Clear()
        Dim connection = dbcontext.Database.Connection
        Dim comm As New SqlCommand("LoadTrail", connection)
        comm.Parameters.AddWithValue("@Date1", Convert.ToDateTime(TxtFromDate.Value))
        comm.Parameters.AddWithValue("@Date2", Convert.ToDateTime(TxtToDate.Value))
        'Select Case CboAccountType.SelectedIndex
        '    Case 0
        '        comm.Parameters.AddWithValue("@AccountType", 2)
        '    Case 1
        '        comm.Parameters.AddWithValue("@AccountType", 0)
        '    Case 2
        '        comm.Parameters.AddWithValue("@AccountType", 1)
        'End Select
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
        adp.Fill(ds, "LoadTrail")
        Dim COUNT__ As Integer = 0
        If ds.Tables(0).Rows.Count > 0 Then
            GridControl1.DataSource = ds.Tables(0)
            GridControl1.MainView.PopulateColumns()
            GridView1.OptionsView.AllowHtmlDrawHeaders = True
            GridView1.Columns(0).Caption = "كود الحساب"
            GridView1.Columns(1).Caption = "اسم الحساب"
            GridView1.Columns(2).Visible = False
            GridView1.Columns(3).Caption = "افتتاحي مدين"
            GridView1.Columns(4).Caption = "افتتاحي دائن"
            GridView1.Columns(5).Caption = "الحركة مدين"
            GridView1.Columns(6).Caption = "الحركة دائن"
            GridView1.Columns(7).Caption = "الرصيد مدين"
            GridView1.Columns(8).Caption = "الرصيد دائن"
            'TRAILTOTAL()
        End If
    End Sub
    Private Sub CMDNew_Click(sender As Object, e As EventArgs) Handles CMDNew.Click
        Try
            LOADTRAIL()
            'LOADfackTRAIL()
        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try
    End Sub

    Private Sub CmdFindDepart_Click(sender As Object, e As EventArgs) Handles CmdFindDepart.Click
        Try
            Dim w As New BranchCostList
            'w.TxtIsUse.Text = True
            'w.TxtType.Text = 0
            w.ShowDialog()
            If w.IS_Yes = True Then
                Dim ii As Integer = w.BranchGrid.CurrentRow.Index
                TxtBranchName.Text = w.BranchGrid.Rows(ii).Cells(1).Value.ToString()
                TxtBranchGuid.Text = w.BranchGrid.Rows(ii).Cells(2).Value.ToString()
            End If
        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try
    End Sub

    Private Sub CmdDelBranch_Click(sender As Object, e As EventArgs) Handles CmdDelBranch.Click
        TxtBranchGuid.Text = Nothing
        TxtBranchName.Text = Nothing
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
                SaveFileDialog1.FileName = "ميزان المراجعة"
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