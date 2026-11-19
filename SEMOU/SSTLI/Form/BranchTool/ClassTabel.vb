Imports System.Data.SqlClient
Imports System.Drawing.Printing

Public Class ClassTabel
    Dim dbcontext As New SemouEntities
    Private WithEvents printDocument As New PrintDocument()
    Sub DELETETABELTIME()
        Dim connection = dbcontext.Database.Connection
        Dim comm = New SqlCommand("DeleteClassTimeTabel", connection)
        comm.CommandType = CommandType.StoredProcedure
        comm.Parameters.AddWithValue("@ClassGuid", TxtClassGuid.Text)
        If connection.State = ConnectionState.Open Then
            connection.Close()
        End If
        connection.Open()
        comm.ExecuteNonQuery()
        comm.Parameters.Clear()
    End Sub
    Sub DELETETABELBOOK()
        Dim connection = dbcontext.Database.Connection
        Dim comm = New SqlCommand("DeleteCalssBook", connection)
        comm.CommandType = CommandType.StoredProcedure
        comm.Parameters.AddWithValue("@ClassGuid", TxtClassGuid.Text)
        If connection.State = ConnectionState.Open Then
            connection.Close()
        End If
        connection.Open()
        comm.ExecuteNonQuery()
        comm.Parameters.Clear()
    End Sub
    Private Sub CmdClose_Click(sender As Object, e As EventArgs) Handles CmdClose.Click
        Try
            If TxtClassGuid.Text <> Nothing Then
                Dim result As DialogResult = MessageBox.Show("هل تريد حذف الجدول المرتبط بتلك الشعبة؟", "تأكيد", MessageBoxButtons.YesNo, MessageBoxIcon.Question)
                If result = DialogResult.Yes Then
                    DELETETABELTIME()
                    DELETETABELBOOK()
                    MessageBox.Show("تم حذف الجدول بنجاح", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Information)
                    LOADBOOK()
                    LOADTABEL()
                End If
            Else
                MessageBox.Show("برجاء اختيار شعبة اولا", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Error)
                Return
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
                w.TxtFind.ReadOnly = True
            Else
                w.TxtType.Text = 0
                w.TxtIsUse.Text = True
                w.TxtFind.ReadOnly = False
            End If
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

    Private Sub CmdFindDiplom_Click(sender As Object, e As EventArgs) Handles CmdFindDiplom.Click
        Try
            If TxtBranchGuid.Text <> Nothing Then
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
            Else
                MessageBox.Show("برجاء اختيار الفرع اولا", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Error)
                Return
            End If
        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try
    End Sub

    Private Sub CmdFindLevel_Click(sender As Object, e As EventArgs) Handles CmdFindLevel.Click
        Try
            If TxtDiplomGuid.Text <> Nothing Then
                Dim w As New LevelList
                w.TxtIsUse.Text = True
                w.ShowDialog()
                If w.IS_Yes = True Then
                    Dim ii As Integer = w.BranchGrid.CurrentRow.Index
                    TxtLevelName.Text = w.BranchGrid.Rows(ii).Cells(1).Value.ToString()
                    TxtLevelGuid.Text = w.BranchGrid.Rows(ii).Cells(3).Value.ToString()
                End If
            Else
                MessageBox.Show("برجاء اختيار الدبلوم اولا", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Error)
                Return
            End If
        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try
    End Sub
    Sub LOADBOOK()
        BookGrid.Rows.Clear()
        Dim connection = dbcontext.Database.Connection
        Dim comm As New SqlCommand("ShowClassBook", connection)
        comm.Parameters.AddWithValue("@ClassGuid", TxtClassGuid.Text)
        comm.CommandType = CommandType.StoredProcedure
        If connection.State = ConnectionState.Open Then
            connection.Close()
        End If
        connection.Open()
        comm.ExecuteNonQuery()
        Dim adp As New SqlDataAdapter(comm)
        Dim ds As DataSet = New DataSet()
        adp.Fill(ds, "ShowClassBook")
        Dim COUNT__ As Integer = 0
        If ds.Tables(0).Rows.Count > 0 Then
            BookGrid.RowCount = ds.Tables(0).Rows.Count
            For i As Integer = 0 To ds.Tables(0).Rows.Count - 1
                BookGrid.Rows(COUNT__).Cells(0).Value = ds.Tables(0).Rows(i)(0).ToString()
                BookGrid.Rows(COUNT__).Cells(1).Value = ds.Tables(0).Rows(i)(1).ToString()
                BookGrid.Rows(COUNT__).Cells(2).Value = ds.Tables(0).Rows(i)(2).ToString()
                COUNT__ = COUNT__ + 1
            Next
        End If
    End Sub
    Sub LOADTABEL()
        'TabelGrid.Rows.Clear()
        Dim connection = dbcontext.Database.Connection
        Dim comm As New SqlCommand("ShowStudyTabel", connection)
        comm.Parameters.AddWithValue("@BranchGuid", TxtBranchGuid.Text)
        comm.Parameters.AddWithValue("@DiplomGuid", TxtDiplomGuid.Text)
        comm.Parameters.AddWithValue("@ClassGuid", TxtClassGuid.Text)
        comm.CommandType = CommandType.StoredProcedure
        If connection.State = ConnectionState.Open Then
            connection.Close()
        End If
        connection.Open()
        comm.ExecuteNonQuery()
        Dim adp As New SqlDataAdapter(comm)
        Dim ds As DataSet = New DataSet()
        adp.Fill(ds, "ShowStudyTabel")
        ' ربط الجدول بـ DataGridView
        TabelGrid.DataSource = ds.Tables(0)
        ' ضبط تنسيق DataGridView
        TabelGrid.Columns(0).Width = 150 ' ضبط عرض عمود الأيام
        For i As Integer = 1 To TabelGrid.Columns.Count - 1
            TabelGrid.Columns(i).Width = 200 ' ضبط عرض أعمدة الفترات
        Next
    End Sub
    Private Sub CmdFindClass_Click(sender As Object, e As EventArgs) Handles CmdFindClass.Click
        Try
            If TxtLevelGuid.Text <> Nothing Then
                Dim w As New ClassList
                w.TxtBranchGuid.Text = TxtBranchGuid.Text
                w.TxtDiplomGuid.Text = TxtDiplomGuid.Text
                w.TxtLevelGuid.Text = TxtLevelGuid.Text
                w.ShowDialog()
                If w.IS_Yes = True Then
                    Dim ii As Integer = w.BranchGrid.CurrentRow.Index
                    TxtCalssCode.Text = w.BranchGrid.Rows(ii).Cells(1).Value.ToString()
                    TxtClassGuid.Text = w.BranchGrid.Rows(ii).Cells(2).Value.ToString()
                End If
                If TxtClassGuid.Text <> Nothing Then
                    LOADBOOK()
                    LOADTABEL()
                End If
            Else
                MessageBox.Show("برجاء اختيار المستوي اولا", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Error)
                Return
            End If
        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try
    End Sub
    'Dim title As String = "معهد السعودي المتخصص العالي للتدريب -- جدول الشعب"
    'Dim details As String = TxtCalssCode.Text & " " & "التخصص: " & TxtDiplomName.Text & " " & "المستوى: " & TxtLevelName.Text
    'تهيئة الطباعه
    'Private Sub printDocument_PrintPage(sender As Object, e As PrintPageEventArgs) Handles printDocument.PrintPage
    '    Dim dgv As DataGridView = TabelGrid  ' استبدل باسم DataGridView الخاص بك
    '    Dim fontHeader As New Font("Calibri", 12, FontStyle.Bold)
    '    Dim fontCell As New Font("Calibri", 12)
    '    Dim fontTitle As New Font("Calibri", 14, FontStyle.Bold) ' خط العنوان
    '    Dim brushHeader As Brush = Brushes.Black
    '    Dim brushCell As Brush = Brushes.Black
    '    Dim brushBackground As Brush = New SolidBrush(Color.Orange)
    '    Dim pen As New Pen(Color.Black, 1)

    '    ' إحداثيات البداية
    '    Dim x As Integer = e.MarginBounds.Left
    '    Dim y As Integer = e.MarginBounds.Top

    '    ' 🌟 **طباعة العنوان (يبدأ من اليمين)**
    '    Dim title As String = "معهد السعودي المتخصص العالي للتدريب -- جدول الشعب"
    '    e.Graphics.DrawString(title, fontTitle, brushHeader, x, y)

    '    ' تحديث موضع y حتى يكون هناك مسافة كافية قبل بيانات TextBox
    '    y += 40

    '    ' 🌟 **طباعة بيانات TextBox (تبدأ من اليمين وفي سطر منفصل)**
    '    Dim details As String = TxtCalssCode.Text & " " & "التخصص: " & TxtDiplomName.Text & " " & "المستوى: " & TxtLevelName.Text
    '    e.Graphics.DrawString(details, fontHeader, brushHeader, x, y)

    '    ' تقليل المسافة حتى يكون العنوان والتفاصيل قريبين من الجدول
    '    y += 50

    '    ' *** باقي الكود لطباعة الجدول كما هو ***
    '    Dim headerHeight As Integer = 30
    '    Dim cellHeight As Integer = 90
    '    Dim columnWidths As Integer() = {100, 180, 180, 180, 180, 180}

    '    ' رسم عناوين الأعمدة الزمنية
    '    Dim colX As Integer = x + columnWidths(0)
    '    For i As Integer = 1 To dgv.Columns.Count - 1
    '        e.Graphics.FillRectangle(Brushes.White, colX, y, columnWidths(i), headerHeight)
    '        e.Graphics.DrawString(dgv.Columns(i).HeaderText, fontHeader, brushHeader, colX + 5, y + 10)
    '        e.Graphics.DrawRectangle(pen, colX, y, columnWidths(i), headerHeight)
    '        colX += columnWidths(i)
    '    Next
    '    y += headerHeight

    '    ' رسم الصفوف (الأيام والخلايا)
    '    For Each row As DataGridViewRow In dgv.Rows
    '        e.Graphics.FillRectangle(brushBackground, x, y, columnWidths(0), cellHeight)
    '        e.Graphics.DrawString(row.Cells(0).Value.ToString(), fontHeader, brushHeader, x + 5, y + 40)
    '        e.Graphics.DrawRectangle(pen, x, y, columnWidths(0), cellHeight)

    '        colX = x + columnWidths(0)
    '        For i As Integer = 1 To dgv.Columns.Count - 1
    '            e.Graphics.FillRectangle(Brushes.White, colX, y, columnWidths(i), cellHeight)

    '            If row.Cells(i).Value IsNot Nothing Then
    '                Dim cellText As String = row.Cells(i).Value.ToString()
    '                cellText = cellText.Replace(Chr(13), vbCrLf)

    '                Dim lines As String() = cellText.Split(New String() {vbCrLf}, StringSplitOptions.None)
    '                Dim stringFormat As New StringFormat()
    '                stringFormat.Alignment = StringAlignment.Center
    '                stringFormat.LineAlignment = StringAlignment.Center

    '                Dim lineHeight As Integer = CInt(e.Graphics.MeasureString("Test", fontCell).Height)
    '                Dim lineY As Integer = y + (cellHeight - (lines.Length * lineHeight)) / 2

    '                For Each line As String In lines
    '                    Dim textRectangle As New RectangleF(colX + 5, lineY, columnWidths(i) - 10, lineHeight)
    '                    e.Graphics.DrawString(line.Trim(), fontCell, brushCell, textRectangle, stringFormat)
    '                    lineY += lineHeight
    '                Next
    '            End If

    '            e.Graphics.DrawRectangle(pen, colX, y, columnWidths(i), cellHeight)
    '            colX += columnWidths(i)
    '        Next

    '        y += cellHeight
    '    Next
    'End Sub
    Private Sub printDocument_PrintPage(sender As Object, e As PrintPageEventArgs) Handles printDocument.PrintPage
        Dim dgv As DataGridView = TabelGrid  ' الجريد الرئيسي
        Dim dgvSmall As DataGridView = BookGrid  ' الجريد الصغيرة
        Dim fontHeader As New Font("Calibri", 9, FontStyle.Bold)
        Dim fontCell As New Font("Calibri", 8)
        Dim fontTitle As New Font("Calibri", 14, FontStyle.Bold) ' خط العنوان
        Dim brushHeader As Brush = Brushes.Black
        Dim brushCell As Brush = Brushes.Black
        Dim brushBackground As Brush = New SolidBrush(Color.White)
        Dim pen As New Pen(Color.Black, 1)

        ' إحداثيات البداية
        Dim x As Integer = e.MarginBounds.Left
        Dim y As Integer = e.MarginBounds.Top

        ' 🌟 **طباعة العنوان (يبدأ من اليمين)**
        Dim title As String = "معهد السعودي المتخصص العالي للتدريب -- جدول الشعب"
        e.Graphics.DrawString(title, fontTitle, brushHeader, x, y)

        ' تحديث موضع y حتى يكون هناك مسافة كافية قبل بيانات TextBox
        y += 30

        ' 🌟 **طباعة بيانات TextBox (تبدأ من اليمين وفي سطر منفصل)**
        Dim details As String = TxtCalssCode.Text & " " & "التخصص: " & TxtDiplomName.Text & " " & "المستوى: " & TxtLevelName.Text
        e.Graphics.DrawString(details, fontHeader, brushHeader, x, y)

        y += 30

        ' *** طباعة الجدول الرئيسي أولًا ***
        Dim headerHeight As Integer = 30
        Dim cellHeight As Integer = 90
        Dim columnWidths As Integer() = {40, 140, 140, 140, 140, 140, 140, 140, 140}

        ' رسم عناوين الأعمدة الزمنية
        Dim colX As Integer = x + columnWidths(0)
        For i As Integer = 1 To dgv.Columns.Count - 1
            e.Graphics.FillRectangle(Brushes.White, colX, y, columnWidths(i), headerHeight)
            Dim headerFormat As New StringFormat()
            headerFormat.Alignment = StringAlignment.Center
            headerFormat.LineAlignment = StringAlignment.Center

            Dim headerRect As New RectangleF(colX, y, columnWidths(i), headerHeight)
            e.Graphics.DrawString(dgv.Columns(i).HeaderText, fontHeader, brushHeader, headerRect, headerFormat)

            e.Graphics.DrawRectangle(pen, colX, y, columnWidths(i), headerHeight)
            colX += columnWidths(i)
        Next
        y += headerHeight

        ' رسم الصفوف (الأيام والخلايا)
        For Each row As DataGridViewRow In dgv.Rows
            e.Graphics.FillRectangle(brushBackground, x, y, columnWidths(0), cellHeight)
            e.Graphics.DrawString(row.Cells(0).Value.ToString(), fontHeader, brushHeader, x + 5, y + 40)
            e.Graphics.DrawRectangle(pen, x, y, columnWidths(0), cellHeight)

            colX = x + columnWidths(0)
            For i As Integer = 1 To dgv.Columns.Count - 1
                e.Graphics.FillRectangle(Brushes.White, colX, y, columnWidths(i), cellHeight)

                If row.Cells(i).Value IsNot Nothing Then
                    Dim cellText As String = row.Cells(i).Value.ToString()
                    cellText = cellText.Replace(Chr(13), vbCrLf)

                    Dim lines As String() = cellText.Split(New String() {vbCrLf}, StringSplitOptions.None)
                    Dim stringFormat As New StringFormat()
                    stringFormat.Alignment = StringAlignment.Center
                    stringFormat.LineAlignment = StringAlignment.Center

                    Dim lineHeight As Integer = CInt(e.Graphics.MeasureString("Test", fontCell).Height)
                    Dim lineY As Integer = y + (cellHeight - (lines.Length * lineHeight)) / 2

                    For Each line As String In lines
                        Dim textRectangle As New RectangleF(colX + 5, lineY, columnWidths(i) - 10, lineHeight)
                        e.Graphics.DrawString(line.Trim(), fontCell, brushCell, textRectangle, stringFormat)
                        lineY += lineHeight
                    Next
                End If

                e.Graphics.DrawRectangle(pen, colX, y, columnWidths(i), cellHeight)
                colX += columnWidths(i)
            Next

            y += cellHeight
        Next

        ' 🌟 **ترك مسافة بعد الجدول الرئيسي لطباعة الجريد الصغيرة**
        y += 10

        ' 🌟 **طباعة الجريد الصغيرة (BookGrid) بعد الجدول الرئيسي**
        Dim smallHeaderHeight As Integer = 30
        Dim smallCellHeight As Integer = 30
        Dim smallColumnWidths As Integer() = {200, 120, 120} ' عرض الأعمدة في الجريد الصغيرة

        ' رسم عناوين الجريد الصغيرة
        colX = x
        For i As Integer = 0 To dgvSmall.Columns.Count - 1
            e.Graphics.FillRectangle(Brushes.LightGray, colX, y, smallColumnWidths(i), smallHeaderHeight)
            e.Graphics.DrawString(dgvSmall.Columns(i).HeaderText, fontHeader, brushHeader, colX + 5, y + 5)
            e.Graphics.DrawRectangle(pen, colX, y, smallColumnWidths(i), smallHeaderHeight)
            colX += smallColumnWidths(i)
        Next

        y += smallHeaderHeight

        ' رسم بيانات الجريد الصغيرة
        For Each row As DataGridViewRow In dgvSmall.Rows
            colX = x
            For i As Integer = 0 To dgvSmall.Columns.Count - 1
                Dim cellText As String = If(row.Cells(i).Value IsNot Nothing, row.Cells(i).Value.ToString(), "")

                ' تنسيق النص
                Dim textFormat As New StringFormat()
                textFormat.Alignment = StringAlignment.Center
                textFormat.LineAlignment = StringAlignment.Center

                Dim rect As New RectangleF(colX, y, smallColumnWidths(i), smallCellHeight)
                e.Graphics.FillRectangle(Brushes.White, rect)
                e.Graphics.DrawRectangle(pen, rect.X, rect.Y, rect.Width, rect.Height)
                e.Graphics.DrawString(cellText, fontCell, brushCell, rect, textFormat)

                colX += smallColumnWidths(i)
            Next
            y += smallCellHeight
        Next
    End Sub
    Private Sub ClassTabel_Load(sender As Object, e As EventArgs) Handles MyBase.Load

    End Sub

    Private Sub CmdSave_Click(sender As Object, e As EventArgs) Handles CmdSave.Click
        printDocument.DefaultPageSettings.Landscape = True
        printDocument.DefaultPageSettings.Margins = New Printing.Margins(5, 5, 0, 0)
        PrintPreviewDialog1.Document = printDocument
        PrintPreviewDialog1.ShowDialog()
    End Sub

    Private Sub CMDNew_Click(sender As Object, e As EventArgs) Handles CMDNew.Click
        Dim saveDialog As New SaveFileDialog()
        saveDialog.Filter = "PDF Files (*.pdf)|*.pdf"
        saveDialog.Title = "حفظ الملف كـ PDF"
        saveDialog.FileName = "جدول_الشعب.pdf"

        If saveDialog.ShowDialog() = DialogResult.OK Then
            Dim printDoc As New PrintDocument()
            printDoc.DefaultPageSettings.Landscape = True ' 🔹 الطباعة بالعرض
            printDoc.DefaultPageSettings.Margins = New Margins(4, 0, 0, 0) ' 🔹 تعيين الهوامش إلى 0

            AddHandler printDoc.PrintPage, AddressOf printDocument_PrintPage

            Dim printDialog As New PrintDialog()
            printDialog.Document = printDoc
            printDialog.AllowPrintToFile = True
            printDialog.PrinterSettings.PrinterName = "Microsoft Print to PDF"
            printDialog.PrinterSettings.PrintFileName = saveDialog.FileName
            printDialog.PrinterSettings.PrintToFile = True

            printDoc.Print()
        End If

    End Sub

    Private Sub CmdExit_Click(sender As Object, e As EventArgs) Handles CmdExit.Click
        Me.Close()
    End Sub
    Sub INSERTINTOTABELBOOK()
        Dim connection = dbcontext.Database.Connection
        Dim comm = New SqlCommand("UpdatClassBook", connection)
        comm.CommandType = CommandType.StoredProcedure
        comm.Parameters.AddWithValue("@BranchGuid", TxtBranchGuid.Text)
        comm.Parameters.AddWithValue("@DiplomGuid", TxtDiplomGuid.Text)
        comm.Parameters.AddWithValue("@LevelGuid", TxtLevelGuid.Text)
        comm.Parameters.AddWithValue("@ClassGuid", TxtClassGuid.Text)
        If connection.State = ConnectionState.Open Then
            connection.Close()
        End If
        connection.Open()
        comm.ExecuteNonQuery()
        comm.Parameters.Clear()
    End Sub
    Private Sub CmdUpdateBook_Click(sender As Object, e As EventArgs) Handles CmdUpdateBook.Click
        Try
            If TxtClassGuid.Text <> Nothing Then
                DELETETABELBOOK()
                INSERTINTOTABELBOOK()
                LOADBOOK()
                MessageBox.Show("تم معالجة المقرارات بنجاح", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Information)
            Else
                MessageBox.Show("برجاء اختيار شعبة اولا", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Error)
                Return
            End If
        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try
    End Sub
End Class