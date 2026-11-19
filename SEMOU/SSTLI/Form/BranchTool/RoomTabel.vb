Imports System.Data.SqlClient
Imports System.Drawing.Printing

Public Class RoomTabel
    Dim dbcontext As New SemouEntities
    Private WithEvents printDocument As New PrintDocument()
    Private Sub CmdClose_Click(sender As Object, e As EventArgs) Handles CmdClose.Click
        Me.Close()
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
    Sub LOADTABEL()
        TabelGrid.DataSource = Nothing
        Dim connection = dbcontext.Database.Connection
        Dim comm As New SqlCommand("LoadRoomTabel", connection)
        comm.Parameters.AddWithValue("@BranchGuid", TxtBranchGuid.Text)
        comm.Parameters.AddWithValue("@RoomGuid", TxtRoomGuid.Text)
        comm.CommandType = CommandType.StoredProcedure
        If connection.State = ConnectionState.Open Then
            connection.Close()
        End If
        connection.Open()
        comm.ExecuteNonQuery()
        Dim adp As New SqlDataAdapter(comm)
        Dim ds As DataSet = New DataSet()
        adp.Fill(ds, "LoadRoomTabel")
        ' ربط الجدول بـ LoadTranierTabel
        TabelGrid.DataSource = ds.Tables(0)
        ' ضبط تنسيق DataGridView
        TabelGrid.Columns(0).Width = 150 ' ضبط عرض عمود الأيام
        For i As Integer = 1 To TabelGrid.Columns.Count - 1
            TabelGrid.Columns(i).Width = 200 ' ضبط عرض أعمدة الفترات
        Next
    End Sub
    Private Sub CmdFind_Click(sender As Object, e As EventArgs) Handles CmdFind.Click
        Try
            If TxtBranchGuid.Text <> Nothing Then
                Dim w As New RoomClassList
                w.TxtBranchGuid.Text = TxtBranchGuid.Text
                w.ShowDialog()
                If w.IS_Yes = True Then
                    Dim ii As Integer = w.BranchGrid.CurrentRow.Index
                    TxtName.Text = w.BranchGrid.Rows(ii).Cells(1).Value.ToString()
                    TxtRoomGuid.Text = w.BranchGrid.Rows(ii).Cells(3).Value.ToString()
                End If
                If TxtRoomGuid.Text <> Nothing Then
                    LOADTABEL()
                End If
            Else
                MessageBox.Show("برجاء اختيار فرع اولا", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Error)
                Return
            End If
        Catch ex As Exception

        End Try
    End Sub
    Private Sub printDocument_PrintPage(sender As Object, e As PrintPageEventArgs) Handles printDocument.PrintPage
        Dim dgv As DataGridView = TabelGrid  ' استبدل باسم DataGridView الخاص بك
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
        Dim title As String = "معهد السعودي المتخصص العالي للتدريب -- جدول قاعه"
        e.Graphics.DrawString(title, fontTitle, brushHeader, x, y)

        ' تحديث موضع y حتى يكون هناك مسافة كافية قبل بيانات TextBox
        y += 40

        ' 🌟 **طباعة بيانات TextBox (تبدأ من اليمين وفي سطر منفصل)**
        Dim details As String = "الفرع: " & TxtBranchName.Text & " " & "القاعة: " & TxtName.Text
        e.Graphics.DrawString(details, fontHeader, brushHeader, x, y)

        ' تقليل المسافة حتى يكون العنوان والتفاصيل قريبين من الجدول
        y += 50

        ' *** باقي الكود لطباعة الجدول كما هو ***
        Dim headerHeight As Integer = 30
        Dim cellHeight As Integer = 100
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
                    stringFormat.Alignment = StringAlignment.Far
                    stringFormat.LineAlignment = StringAlignment.Far

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
    End Sub

    Private Sub CMDNew_Click(sender As Object, e As EventArgs) Handles CMDNew.Click
        Dim saveDialog As New SaveFileDialog()
        saveDialog.Filter = "PDF Files (*.pdf)|*.pdf"
        saveDialog.Title = "حفظ الملف كـ PDF"
        saveDialog.FileName = "جدول_قاعة.pdf"

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

    Private Sub CmdSave_Click(sender As Object, e As EventArgs) Handles CmdSave.Click
        printDocument.DefaultPageSettings.Landscape = True
        printDocument.DefaultPageSettings.Margins = New Printing.Margins(5, 5, 0, 0)
        PrintPreviewDialog1.Document = printDocument
        PrintPreviewDialog1.ShowDialog()
    End Sub
End Class